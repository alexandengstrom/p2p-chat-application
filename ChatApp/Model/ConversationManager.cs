using ChatApp.View.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Xml.Linq;

namespace ChatApp.Model
{
    /// <summary>
    /// This singleton object is responsible for managing all conversations -- both active and inactive. Furthermore, this singleton is responsible for sending many network requests, and adding new data to the appropriate models in order to properly display it. 
    /// Mainly, this model ensures that the same data is saved and displayed.
    /// </summary>
    public sealed class ConversationManager : INotifyPropertyChanged
    {

        private static readonly Lazy<ConversationManager> _conversationManager = new Lazy<ConversationManager>(() => new ConversationManager());
        private Dictionary<string, ConversationModel?> conversations;

        private Dictionary<string, ConversationModel> inactiveConversations;
        private ConversationSerializer serializer;

        public event EventHandler activeConversationSetEvent;
        public event EventHandler inactiveConversationSetEvent;

        public event EventHandler buzzEvent;

        private NotificationManager? notificationManager = null;
        public NotificationManager NotificationManager { get { return notificationManager; } set { notificationManager = value; } }
        
        private string currentConversation = null;
        public string CurrentConversation { get { return currentConversation; } set { currentConversation = value; OnPropertyChanged("CurrentConversation"); } }
        public bool CurrentConversationIsActive { get { if (currentConversation == null) return false; return conversations.ContainsKey(currentConversation); } }

        /// <summary>
        /// Fetches a list of all active conversations.
        /// </summary>
        /// <returns>A list of all active conversations.</returns>
        public List<ConversationModel> GetActiveConversations()
        {
            return conversations.Values.ToList();
        }

        /// <summary>
        /// Fetches a list of all inactive conversations.
        /// </summary>
        /// <returns>A list of all inactive conversations.</returns>
        public List<ConversationModel> GetInactiveConversations()
        {
            return inactiveConversations.Values.ToList().OrderByDescending(item => item.LastActivity).ToList();
        }

        /// <summary>
        /// Assigns a conversation as the currently active one.
        /// </summary>
        /// <param name="endpoint">The endpoint of the conversation to be assigned as the currently active one.</param>
        public void AssignCurrentConversation(string? endpoint)
        {
            if (endpoint == null || CurrentConversation == endpoint) return; // end early if the endpoint is already the active conversation!

            if (conversations.Keys.Contains(endpoint)) // if the conversation is an active one
            {
                conversations[endpoint].UnreadMessages = false;
                conversations[endpoint].UnreadBuzz = false;
                CurrentConversation = endpoint;   
                activeConversationSetEvent?.Invoke(this, new EventArgs());
                conversationsUpdatedEvent?.Invoke(this, EventArgs.Empty);
            } else if (inactiveConversations.Keys.Contains(endpoint)) // if the conversation is inactive
            {
                CurrentConversation = endpoint;
                inactiveConversationSetEvent?.Invoke(this, new EventArgs());
            } else // this is exeedingly rare and we haven't found a way to trigger this
            {
                SendNotification("❗ Something unexpected happened when switching conversations...");
            }
        }

        private List<UserModel> pendingRequests = new List<UserModel>();

        public event EventHandler newRequestEvent;
        public event EventHandler conversationsUpdatedEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A notification for any changed property.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        private void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// On init, this singleton generates exactly one instance. It loads all inactive conversations and populates the inactive conversation dictionary.
        /// </summary>
        private ConversationManager()
        {
            serializer = new ConversationSerializer();
            conversations = new Dictionary<string, ConversationModel>();
            inactiveConversations = new Dictionary<string, ConversationModel>();
            List<ConversationModel> fetchedConversations = serializer.LoadAll();

            foreach (ConversationModel conversation in fetchedConversations)
            {
                inactiveConversations[conversation.User.Address] = conversation;
            }

        }

        /// <summary>
        /// Fetches the global instance of the conversation manager.
        /// </summary>
        public static ConversationManager Instance
        {
            get
            {
                return _conversationManager.Value;
            }
        }

        /// <summary>
        /// Used to initialize a new conversation, alternatively to move an inactive conversation to be an active one (when applicable). Used by both the client that sent a connection request as well as the accepting client to initiate or move the proper conversation model.
        /// </summary>
        /// <param name="user">The UserModel of the conversation to be added.</param>
        public void InitializeConversation(UserModel user)
        {
            if (inactiveConversations.ContainsKey(user.Address))
            {
                conversations[user.Address] = inactiveConversations[user.Address];
                conversations[user.Address].UnreadMessages = false;
                conversations[user.Address].UnreadBuzz = false;
                inactiveConversations.Remove(user.Address);
            } else
            {
                conversations[user.Address] = new ConversationModel(user);
            }

            CurrentConversation = user.Address;
            activeConversationSetEvent?.Invoke(this, EventArgs.Empty);
            conversationsUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Used to add messages to the appropriate conversation model, as well as send them to te recipient through the NetworkManager.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(DataModel message)
        {
            conversations[message.Receiver].ReceiveMessage(message);
            await NetworkManager.Instance.SendMessage(message);
        }

        /// <summary>
        /// Used to send a buzz to the appropriate client. Can be extended to include a GUI update for the sender after a successful buzz.
        /// </summary>
        /// <param name="buzz"></param>
        /// <returns></returns>
        public async Task SendBuzz(BuzzModel buzz)
        {
            await NetworkManager.Instance.SendMessage(buzz);
        }

        /// <summary>
        /// Used to receive a message into the appropriate conversation model, and to notify the ViewModel of the new message.
        /// </summary>
        /// <param name="message">The DataModel (MessageModel, more specifically) to be received, containing recipient information..</param>
        public void ReceiveMessage(DataModel message)
        {
            conversations[message.SenderAddr].ReceiveMessage(message);
            conversationsUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Used to receive a buzz, and to trigger a GUI update.
        /// </summary>
        /// <param name="buzz">The BuzzModel, containing recipient information.</param>
        public void ReceiveBuzz(DataModel buzz)
        {

            if (buzz.SenderAddr != currentConversation)
            {
                conversations[buzz.SenderAddr].UnreadBuzz = true;
                conversations[buzz.SenderAddr].UnreadMessages = false;
            }

            conversationsUpdatedEvent?.Invoke(this, EventArgs.Empty);
            buzzEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fetches the current conversation model that is the currently active conversation.
        /// </summary>
        /// <returns>The currently active conversation model.</returns>
        public ConversationModel GetConversation() { 
            if (currentConversation == null)
            {
                return null;
            } else if (conversations.ContainsKey(currentConversation))
            {
                return conversations[currentConversation];
            } else
            {
                return inactiveConversations[currentConversation];
            }

        }

        /// <summary>
        /// Used to notify the GUI of a new pending request.
        /// </summary>
        /// <param name="user">The UserModel that sent the request.</param>
        public void OnNewRequest(UserModel user)
        {
            pendingRequests.Add(user);
            newRequestEvent?.Invoke(this, EventArgs.Empty);
            
        }

        /// <summary>
        /// Fetches the oldest pending request.
        /// </summary>
        /// <returns>The UserModel that sent the request.</returns>
        public UserModel? GetPendingRequest()
        {
            return pendingRequests.First();
        }

        /// <summary>
        /// Used to accept a pending request. Triggers GUI updates for the accepter and initializes the conversations. Additionally, it sends a message to the requester to accept the conversation on their end.
        /// </summary>
        public void AcceptRequest()
        {
            UserModel user = pendingRequests.First();
            pendingRequests.RemoveAt(0);
            InitializeConversation(user);
            NetworkManager.Instance.AcceptRequest(user);
            if (pendingRequests.Count > 0)
            {
                newRequestEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Used to decline a pending request. Additionally sends a message to the requester to decline the conversation on their end. The client that sent the request will then close the connection.
        /// </summary>
        public void DeclineRequest()
        {
            UserModel user = pendingRequests.First();
            pendingRequests.RemoveAt(0);
            NetworkManager.Instance.RefuseRequest(user);
            if (!(pendingRequests.Count == 0))
            {
                newRequestEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Used to close a conversation, move it to be inactive as well as save the conversation to JSON in order to cache the history.
        /// </summary>
        /// <param name="clientAddress"></param>
        public void CloseConversation(string clientAddress)
        {
            ConversationModel conversation = conversations[clientAddress];

            serializer.Save(conversation);
            conversations.Remove(clientAddress);
            inactiveConversations[clientAddress] = conversation;
            if (conversations.Count == 0) { 
                CurrentConversation = null;
            } 

            conversationsUpdatedEvent?.Invoke(this, EventArgs.Empty);

        }

        /// <summary>
        /// Helper method to iterate through all active conversations and save them to JSON.
        /// </summary>
        public void OnExit()
        {
            foreach (var conv in conversations)
            {
                if (conv.Value != null)
                    serializer.Save(conv.Value);
            }
        }

        /// <summary>
        /// Used to send notifications to the notification manager. Can be accessed globally through this singleton object.
        /// </summary>
        /// <param name="message"></param>
        public void SendNotification(string message)
        {
            if (notificationManager != null)
                notificationManager.AddNotification(message);
        }
    }
}