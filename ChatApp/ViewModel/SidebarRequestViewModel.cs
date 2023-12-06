using ChatApp.Model;
using ChatApp.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the Sidebar Request View.
    /// </summary>
    internal class SidebarRequestViewModel
    {
        /// <summary>
        /// The ip that the user wants to send a request to.
        /// </summary>
        private string ip;
        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        /// <summary>
        /// The port that the user wants to send a request to.
        /// </summary>
        private string port;
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Command for sending a new chat request.
        /// </summary>
        private ICommand sendRequest = null;
        public ICommand SendRequest
        {
            get
            {
                if (sendRequest == null)
                {
                    sendRequest = new RequestCommand(this);
                    Ip = "";
                    Port = "";
                }
                return sendRequest;
            }
            set { sendRequest = value; }
        }

        /// <summary>
        /// Sends a new request to the given ip and port.
        /// </summary>
        public void SendNewRequest()
        {
            NetworkManager manager = NetworkManager.Instance;
            string addr = ip + ":" + port;
            if (manager.Host.Address == addr)
            {
                ConversationManager.Instance.SendNotification("You cannot connect to yourself!");
            } else if (manager.IsClientConnected(addr))
            {
                ConversationManager.Instance.SendNotification($"You are already connected to {addr}!");
                Ip = "";
                Port = "";
            }
            else
            {
                manager.Connect(Ip, Port);
            }
        }
    }
}
