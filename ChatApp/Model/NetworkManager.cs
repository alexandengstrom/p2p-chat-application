using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;

namespace ChatApp.Model
{
    /// <summary>
    /// This singleton object manages all network requests. All messages that will be sent throuh the network will go through this class, and all servers and clients will be managed on separate threads.
    /// </summary>
    public sealed class NetworkManager
    {
        private static readonly Lazy<NetworkManager> _networkManager = new Lazy<NetworkManager>(() => new NetworkManager());

        private NotificationManager? notificationManager = null;
        
        /// <summary>
        /// Fetches or sets the instance of the NotificationManager, which is instantiated through ConversationManager.
        /// </summary>
        public NotificationManager NotificationManager { get { return notificationManager; } set { notificationManager = value; } }

        private Dictionary<string, TcpClient> connections;
        private readonly object _lock = new();
        private Protocol protocol = new();
        private UserModel? host;
        private TcpListener? server = null;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public event EventHandler listenerSuccessEvent;
        public event EventHandler listenerFailedEvent;

        /// <summary>
        /// Instantiates the global NetworkManager instance.
        /// </summary>
        private NetworkManager()
        {
            connections = new Dictionary<string, TcpClient>();
        }

        /// <summary>
        /// Fetches the global instance of the NetworkManager.
        /// </summary>
        public static NetworkManager Instance
        {
            get
            {
                return _networkManager.Value;
            }
        }

        /// <summary>
        /// A helper method to fetch the current host.
        /// </summary>
        public UserModel Host { get { return host; } }

        /// <summary>
        /// This method manages all incoming messages on any particular client on new threads. This is used for any incoming or outgoing connection.
        /// </summary>
        /// <param name="client">The client to be managed.</param>
        private async Task ManageClientConnection(TcpClient client)
        {
            string clientAddress = client.Client.RemoteEndPoint.ToString();
            string clientName = "";
            bool exit = false;

            try
            {
                while (!exit)
                {
                    Byte[] bytes = new byte[4096];
                    string? data = null;
                    NetworkStream stream = client.GetStream();
                    DataModel message;

                    int i;
                    while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                        {

                        try
                        {
                            message = protocol.Decode(bytes);
                        } catch (JsonSerializationException) 
                        {
                            exit = true;
                            notificationManager.AddNotification($"❌ {clientAddress}: Unable to decode message from {clientName}. Closing connection.");
                            break;
                        } catch (ArgumentException)
                        {
                            exit = true;
                            notificationManager.AddNotification($"❌¨{clientAddress}: Message received with the wrong protocol version. Closing connection.");
                            break;
                        }

                        // These cases manage the different incoming message models
                        if (message is ConnectionRequestModel) // initial connection request - recipient only
                        {
                            ConversationManager.Instance.OnNewRequest(message.Sender);

                            // we update the key for the client to be the IP and port the UserModel is listening to
                            // this simplifies dictionary lookups for future messages as we can explicitly look at the UserModel
                            connections[message.SenderAddr] = connections[clientAddress];
                            connections.Remove(clientAddress);
                            clientAddress = message.SenderAddr;
                            clientName = message.Sender.Name;
                        } else if (message is AcceptRequestModel) // accepting initial connection request - sender only
                        {
                            // as we're keeping the client, we need to update these strings for notifications
                            clientName = message.Sender.Name;
                            clientAddress = message.SenderAddr;
                            if (notificationManager != null)
                                notificationManager.AddNotification($"✔️ {clientAddress}: {clientName} has accepted your connection request! You can now chat.");
                            ConversationManager.Instance.InitializeConversation(message.Sender);
                        } else if (message is RefuseRequestModel) // refuse initial connection request - sender only
                        {
                            // we only need to update the senderaddress as we're discarding the client
                            clientAddress = message.SenderAddr;
                            if (notificationManager != null)
                                notificationManager.AddNotification($"❌ {clientAddress}: {message.Sender.Name} has refused your connection request.");
                            exit = true;
                            break;
                            
                        } else if (message is CloseConnectionModel) // explicitly close the current connection
                        {
                            // as this may in some (rare) circumstances be the first message received we update the senderaddress
                            clientAddress = message.SenderAddr;
                            if (notificationManager != null)
                                notificationManager.AddNotification($"❌ {clientAddress}: {message.Sender.Name} has closed your chat.");
                            exit = true;
                            break;

                        }
                        else if (message is MessageModel) // any regular message
                        {
                            ConversationManager.Instance.ReceiveMessage(message);
                        } else if (message is BuzzModel) // any buzz
                        {
                            ConversationManager.Instance.ReceiveBuzz(message);
                        }
                        bytes = new byte[4096]; // reset the bytearray for the next loop
                    }

                    // Any message that explicitly closes the connection should force stop the loop
                    if (exit)
                        break;
                }

            }
            catch (SocketException e) // triggered when the socket closes for any unexpected reason (for instance, force exited applications)
            {
                if (notificationManager != null)
                {
                    notificationManager.AddNotification($"❗️ {clientAddress}: Something unexpected forced your connection to {clientName} to close.");
                }
            }
            catch (IOException e) // triggered when a message cannot be read from the stream
            {
                if (notificationManager != null)
                {
                    notificationManager.AddNotification($"❗️ {clientAddress}: Failed to read message received from {clientName}. The connection will now close.");
                }
            }
            finally
            {
                client.Close();
                // any unexpected reason for the connection to close will trigger this message
                if (!exit && notificationManager != null)
                {
                    notificationManager.AddNotification($"❌ {clientAddress}: Your connection to {clientName} has been closed.");
                }

                // the lock ensures only one thread can manipulate the dictionaries at once
                lock (_lock)
                {
                    ConversationManager.Instance.CloseConversation(clientAddress);
                    connections.Remove(clientAddress);
                }

            }
        }

        /// <summary>
        /// Begin to listen on any port on a separate thread. Starts up a new client connection on a new thread through ManageClientConnection whenever a new connection is established.
        /// </summary>
        /// <param name="user">Your UserModel, which contains the IP and port to listen to.</param>
        public async Task Listen(UserModel user)
        {
            connections.Clear();
            host = user;
            TcpClient incomingClient;

            IPAddress localAddr = IPAddress.Parse(host.Ip);
            Int32 portInt = Convert.ToInt32(host.Port);

            try
            {
                server = new TcpListener(localAddr, portInt);
                server.Start();
            } catch (SocketException) // triggered when we cannot listen to the port - for instance when the port is taken
            {
                listenerFailedEvent?.Invoke(this, EventArgs.Empty);
                return;
            }

            // this listen loop allows any application instance to listen on any port - the cancellation token is used to stop this thread when the app should exit.
            while (!cts.Token.IsCancellationRequested)
            {
                if (server.Pending())
                {
                    // any new client will be accepted in order to send request messages
                    incomingClient = await server.AcceptTcpClientAsync();

                    // the lock ensures only one thread can manipulate the dictionary at once
                    lock (_lock)
                    {
                        // begin by grabbing the random client endpoint from the TcpClient as the key
                        // will be changed in ManageClientConnection - ConnectionRequestModel
                        string clientAddress = incomingClient.Client.RemoteEndPoint.ToString();
                        connections[clientAddress] = incomingClient;
                    }

                    // let the new client be managed on a new thread
                    Task.Run(() => ManageClientConnection(incomingClient).ConfigureAwait(false));
                } else
                {
                    // wait a short bit to not overload the cpu
                    await Task.Delay(100);
                }
            }
        }

        /// <summary>
        /// Helper method to check if a port is occupied.
        /// </summary>
        /// <param name="port">The port you wish to control.</param>
        /// <returns>True if the port is available, otherwise false.</returns>
        public static bool IsPortOccupied(string port)
        {
            var activeConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            
            foreach (var activeConnection in activeConnections)
            {
                if (activeConnection.Port == Convert.ToInt32(port))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Helper method to fetch your public IP.
        /// </summary>
        /// <returns>Your public IP or null.</returns>
        public static string GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string found_ip = null;

            try
            {
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        found_ip = ip.ToString();
                        break;
                    }
                }
            }
            catch (SocketException e) // triggered if we can't fetch any IP
            {
                found_ip = null;
            }

            return found_ip;
        }

        /// <summary>
        /// Closes the listening server explicitly.
        /// </summary>
        public async Task CloseServer()
        {
            // send messages to all connected clients to ensure they close their clients
            var sendTasks = connections.Select(connection =>
                SendMessage(new CloseConnectionModel(host, connection.Key)));

            // wait for all messages to be sent
            await Task.WhenAll(sendTasks);

            ConversationManager.Instance.OnExit();

            if (server != null)
            {
                // cancel the listen-thread
                cts.Cancel();
                server.Stop();
                server = null;
            }
        }

        /// <summary>
        /// Attempt to send a connection request to any IP and port.
        /// </summary>
        /// <param name="ip">The IP to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public async Task Connect(string ip, string port)
        {
            string targetIp = ip + ":" + port;
            notificationManager.AddNotification($"❗ Sending connection request to {targetIp}...");
            Int32 portInt = Convert.ToInt32(port);
            TcpClient client = new TcpClient();

            try
            {
                IPAddress localAddr = IPAddress.Parse(ip);
                await client.ConnectAsync(ip, portInt);
            }
            catch (SocketException e)
            {
                notificationManager.AddNotification($"❌ No host appears to be listening at {targetIp}. No connection established.");
                return;
            }
            
            connections[targetIp] = client;
            // explicitly send a connection request before managing the client in a new thread
            await SendMessage(new ConnectionRequestModel(host, targetIp));
            _ = Task.Run(() => ManageClientConnection(client).ConfigureAwait(false));
        }

        /// <summary>
        /// Sends any type of DataModel to the UserModel defined in the DataModel.
        /// </summary>
        /// <param name="dataModel">The DataModel to be sent.</param>
        public async Task SendMessage(DataModel dataModel)
        {
            NetworkStream stream = connections[dataModel.Receiver].GetStream();
            try
            {
                await stream.WriteAsync(protocol.Encode(dataModel));
            } catch (ArgumentOutOfRangeException) { // thrown by encode if the message is too long
                if (notificationManager != null)
                {
                    notificationManager.AddNotification($"❌ Your message is too long, and has not been sent.");
                }
            } catch (Exception e) // any other type of error means that the message couldn't be sent
            {
                if (notificationManager != null)
                {
                    notificationManager.AddNotification($"❌ Failed to send message to {dataModel.Receiver}");
                }
            }
        }

        /// <summary>
        /// Sends a message to accept a request.
        /// </summary>
        /// <param name="user">The UserModel that sent the connection request.</param>
        public void AcceptRequest(UserModel user)
        {
            AcceptRequestModel msg = new AcceptRequestModel(Host, user.Address);
            SendMessage(msg);
        }

        /// <summary>
        /// Sends a message to refuse a request.
        /// </summary>
        /// <param name="user">The UserModel that sent the connection request.</param>
        public void RefuseRequest(UserModel user)
        {
            RefuseRequestModel msg = new RefuseRequestModel(Host, user.Address);
            SendMessage(msg);

        }
        
        /// <summary>
        /// Simple helper method to check if a client is connected.
        /// </summary>
        /// <param name="addr">The IP and port to check - formatted as "0.0.0.0:1000</param>
        /// <returns>True if the client is connected, otherwise false.</returns>
        public bool IsClientConnected(string addr)
        {
            return connections.ContainsKey(addr);
        }
    }
}