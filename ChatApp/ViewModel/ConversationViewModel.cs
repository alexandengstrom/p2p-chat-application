using ChatApp.Model;
using ChatApp.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the Conversation View. Responsible for fetching data from the currently selected conversation.
    /// </summary>
    internal class ConversationViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Reference to the currently selected conversation.
        /// </summary>
        private ConversationModel conversation = null;

        /// <summary>
        /// An observable collection of all messages in the currently selected conversation.
        /// </summary>
        private ObservableCollection<DataModel> messages = new ObservableCollection<DataModel>();
        public ObservableCollection<DataModel> Messages { get { return messages; } }

        /// <summary>
        /// Boolean that is true if its possible to send a message in the currently selected conversation.
        /// </summary>
        public bool CanSendMessage { get { return conversation != null && ConversationManager.Instance.CurrentConversationIsActive; } }
        public bool CanReconnect { get { return conversation != null && !ConversationManager.Instance.CurrentConversationIsActive; } }

        /// <summary>
        /// Boolean that trigger the GUI to autoscroll to the bottom when a new message is received.
        /// </summary>
        private bool shouldScrollToEnd;
        public bool ShouldScrollToEnd
        {
            get { return shouldScrollToEnd; }
            set
            {
                shouldScrollToEnd = value;
                OnPropertyChanged(nameof(ShouldScrollToEnd));
            }
        }

        /// <summary>
        /// The message that the user wants to send. Binds to a textbox in the GUI.
        /// </summary>
        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged("Message"); }
        }

        /// <summary>
        /// Command that is triggered when the user wants to send a new message.
        /// </summary>
        private ICommand sendMessageCommand = null;
        public ICommand SendMessageCommand
        {
            get
            {
                if (sendMessageCommand == null)
                {
                    sendMessageCommand = new SendMessageCommand(this);
                }
                return sendMessageCommand;
            }
            set { sendMessageCommand = value; }
        }

        /// <summary>
        /// Command that is triggered when the user wants to send a buzz.
        /// </summary>
        private ICommand sendBuzzCommand = null;
        public ICommand SendBuzzCommand
        {
            get
            {
                if (sendBuzzCommand == null)
                {
                    sendBuzzCommand = new SendBuzzCommand(this);
                }
                return sendBuzzCommand;
            }
            set { sendBuzzCommand = value; }
        }

        /// <summary>
        /// Command that is triggered when the user wants to reconnect to an old conversation.
        /// </summary>
        private ICommand reconnectCommand = null;
        public ICommand ReconnectCommand
        {
            get
            {
                if (reconnectCommand == null)
                {
                    reconnectCommand = new ReconnectCommand(this);
                }
                return reconnectCommand;
            }
            set { reconnectCommand = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ConversationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateConversation();
        }

        private void ConversationPropertyChanged(object sender, EventArgs e)
        {
            this.UpdateConversation();

        }

        /// <summary>
        /// Constructor that setups the eventlisteners.
        /// </summary>
        public ConversationViewModel()
        {
            ConversationManager.Instance.PropertyChanged += ConversationPropertyChanged;
            ConversationManager.Instance.conversationsUpdatedEvent += ConversationPropertyChanged;
        }

        /// <summary>
        /// Updates the conversation and sends a message to the Conversation Manager.
        /// </summary>
        public void SendMessage()
        {
            UpdateConversation();
            if (this.conversation != null) {
                MessageModel msg = new MessageModel(NetworkManager.Instance.Host, ConversationManager.Instance.CurrentConversation, message);
                ConversationManager.Instance.SendMessage(msg);
            }

            Message = "";
        }

        /// <summary>
        /// Sends a buzz to the Conversation Manager.
        /// </summary>
        public void SendBuzz()
        {
            if (this.conversation != null)
            {
                BuzzModel msg = new BuzzModel(NetworkManager.Instance.Host, ConversationManager.Instance.CurrentConversation);
                ConversationManager.Instance.SendBuzz(msg);
            }
        }

        /// <summary>
        /// Loads the conversation that is currently selected by the user.
        /// </summary>
        private void UpdateConversation()
        {
            conversation = ConversationManager.Instance.GetConversation();
            if (conversation != null)
            {
                messages = conversation.Messages;
                ShouldScrollToEnd = true;
            } else
            {
                messages = new ObservableCollection<DataModel>();
            }
            OnPropertyChanged("Messages");
            OnPropertyChanged("CanSendMessage");
            OnPropertyChanged("CanReconnect");
        }

        /// <summary>
        /// Sends a request to the Network Manager that the user wants to reconnect to a specific ip and port.
        /// </summary>
        public void AttemptReconnection()
        {
            try
            {
                string ip = conversation.User.Ip;
                string port = conversation.User.Port;
                NetworkManager.Instance.Connect(ip, port);
            } catch (KeyNotFoundException e)
            {
                ConversationManager.Instance.SendNotification($"❌ Error: Cannot reconnect to {conversation.User.Ip}:{conversation.User.Port}.");
            }

        }

    }
}