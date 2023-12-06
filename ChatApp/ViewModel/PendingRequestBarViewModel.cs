using ChatApp.Model;
using ChatApp.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the Pending Requests View.
    /// </summary>
    public class PendingRequestBarViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// String that displays which user has sent a request.
        /// </summary>
        private string requestMessage = "";
        public string RequestMessage { get { return requestMessage; } set { requestMessage = $"{value} sent a chat request!"; OnPropertyChanged("RequestMessage"); } }

        /// <summary>
        /// Command for accepting a chat request.
        /// </summary>
        private ICommand acceptRequestCommand = null;
        public ICommand AcceptRequestCommand
        {
            get
            {
                if (acceptRequestCommand == null)
                {
                    acceptRequestCommand = new AcceptRequestCommand(this);
                }
                return acceptRequestCommand;
            }
            set { acceptRequestCommand = value; }
        }

        /// <summary>
        /// Command for declining a chat request.
        ///</summary>
        private ICommand declineRequestCommand = null;
        public ICommand DeclineRequestCommand
        {
            get
            {
                if (declineRequestCommand == null)
                {
                    declineRequestCommand = new DeclineRequestCommand(this);
                }
                return declineRequestCommand;
            }
            set { declineRequestCommand = value; }
        }

        /// <summary>
        /// Boolean that is true when there is a new request that should be answered.
        /// </summary>
        private bool hasNewRequest = false;
        public bool HasNewRequest { get { return hasNewRequest; } set { hasNewRequest = value; OnPropertyChanged("HasNewRequest"); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Called when there is a new chat request available.
        /// </summary>
        private void OnNewRequest(object sender, EventArgs e)
        {
            UserModel user = ConversationManager.Instance.GetPendingRequest();
            RequestMessage = user.Address + ": " + user.Name;
            HasNewRequest = true;
        }

        /// <summary>
        /// Constructor that setups the event listener.
        /// </summary>
        public PendingRequestBarViewModel()
        {
            ConversationManager.Instance.newRequestEvent += OnNewRequest;
        }

        /// <summary>
        /// Accepts the current request.
        /// </summary>
        public void AcceptRequest()
        {
            HasNewRequest = false;
            ConversationManager.Instance.AcceptRequest();
        }

        /// <summary>
        /// Declines the current request.
        /// </summary>
        public void DeclineRequest()
        {
            HasNewRequest = false;
            ConversationManager.Instance.DeclineRequest();
        }
    }
}
