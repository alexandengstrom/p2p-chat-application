using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ChatApp.Model
{
    /// <summary>
    /// This abstract class defines the base for all messages to be sent through the TCP connection, which enables generic methods due to the polymorphic nature of the inherited objects.
    /// </summary>
    public abstract class DataModel
    {
        private UserModel sender;
        private string receiver;
        private DateTime date;
        private string? message;

        /// <summary>
        /// The base constructor for the mandatory data that all messages must contain.
        /// </summary>
        /// <param name="sender">The UserModel of the sender.</param>
        /// <param name="receiver">The endpoint of the receiver (formatted as "0.0.0.0:1000")</param>
        /// <param name="message">Optional paramerer for message text, only used by MessageModel but exists here for easier serialization when enciding and decoding.</param>
        public DataModel(UserModel sender, string receiver, string message = null)
        {
            this.sender = sender;
            this.receiver = receiver;
            date = DateTime.Now;
        }

        /// <summary>
        /// Fetches the senders address in the appropriate formatting (as used by the dictionaries in NetworkManager and ConversationManager).
        /// </summary>
        public string SenderAddr {  get { return sender.Address; } }
        /// <summary>
        /// Fetches the senders UserModel.
        /// </summary>
        public UserModel Sender {  get { return sender; } }
        /// <summary>
        /// Fetches the recipients endpoint for use as dictionary keys.
        /// </summary>
        public string Receiver { get { return receiver; } }
        /// <summary>
        /// Fetches date the message was generated.
        /// </summary>
        public DateTime Date { get { return date; } }
        /// <summary>
        /// Allows the view to get the name of the sender.
        /// </summary>
        public string Name { get { return sender.Name; } }
        /// <summary>
        /// Allows the message to be fetched.
        /// </summary>
        public string Message { get { return message; } set { message = value; } }
    }
}
