using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatApp.Model
{
    /// <summary>
    /// This model defines the full conversation between two users, as well as all messages between them.
    /// </summary>
    public class ConversationModel
    {
        private string endpoint;
        /// <summary>
        /// Fetches the endpoint of the conversation (the other user in the conversation).
        /// </summary>
        public string Endpoint { get { return endpoint; } set { endpoint = value; } }

        private string name;
        /// <summary>
        /// Fetches the name of the other participant in the conversation.
        /// </summary>
        public string Name { get; set; }

        private DateTime lastActivity;
        /// <summary>
        /// Fetches the timestamp of the last activity in this conversation.
        /// </summary>
        public DateTime LastActivity { get => lastActivity; set { lastActivity = value; } }
        /// <summary>
        /// Fetches the time since the last activity in the conversation as a string.
        /// </summary>
        public string LastActivityToString { get { return DateToString(lastActivity); } }

        private bool unreadMessages = false;
        /// <summary>
        /// Used to verify if there are unread messages in the conversation.
        /// </summary>
        public bool UnreadMessages { get { return unreadMessages; } set { unreadMessages = value; } }

        private bool unreadBuzz = false;
        /// <summary>
        /// Used to verify if there are unread buzzes in the conversation.
        /// </summary>
        public bool UnreadBuzz { get { return unreadBuzz; } set { unreadBuzz = value; } }

        public string Username { get { return user.Name; } }

        private UserModel user;
        /// <summary>
        /// Fetches the UserModel of the other participant in the conversation.
        /// </summary>
        public UserModel User { get { return user; } set { user = value; } }  

        private ObservableCollection<DataModel> messages = new ObservableCollection<DataModel>();
        /// <summary>
        /// This observable collection contains all messages in the conversation.
        /// </summary>
        public ObservableCollection<DataModel> Messages { get { return messages; } }

        /// <summary>
        /// Empty constructor is needed to allow for serialization of this object to work.
        /// </summary>
        public ConversationModel() { }

        /// <summary>
        /// This constructor only accepts an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public ConversationModel(string endpoint)
        {
            this.endpoint = endpoint;
        }     

        /// <summary>
        /// Creates a new conversation.
        /// </summary>
        /// <param name="user">The UserModel of the other participant in the conversation.</param>
        public ConversationModel(UserModel user)
        {
            this.user = user;
        }

        /// <summary>
        /// Used to receive a message in this conversation.
        /// </summary>
        /// <param name="message">The DataModel (MessageModel) to be added.</param>
        public void ReceiveMessage(DataModel message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lastActivity = message.Date;
                messages.Add(message);
                if (!(ConversationManager.Instance.CurrentConversation == User.Address))
                {
                    unreadMessages = true;
                    UnreadBuzz = false;
                } else
                {
                    unreadMessages = false;
                }
            });
           
        }

        /// <summary>
        /// Converts the DateTime to a properly formatted string for display in the GUI. (Time since last activity)
        /// </summary>
        /// <param name="dateTime">The DateTime-object.</param>
        /// <returns>A string with the proper formatting for the GUI.</returns>
        private static string DateToString(DateTime dateTime)
        {
            int seconds = (int)(DateTime.Now - dateTime).TotalSeconds;

            if (seconds < 30)
            {
                return "Just now";
            }
            else if (seconds < 60)
            {
                return $"{seconds} sec ago";
            }
            else if (seconds < 60 * 60)
            {
                int minutes = seconds / 60;
                return $"{minutes} min ago";
            }
            else if (seconds < (60 * 60 * 24))
            {
                int hours = seconds / 60 / 60;
                string tmp = hours == 1 ? "hour" : "hours";
                return $"{hours} {tmp} ago";
            }
            else if (seconds < (60 * 60 * 24 * 30))
            { 
                int days = seconds / 60 / 60 / 24;
                string tmp = days == 1 ? "day" : "days";
                return $"{days} {tmp} ago";
            } else
            {
                return "Long ago";
            }
        }
    }
}
