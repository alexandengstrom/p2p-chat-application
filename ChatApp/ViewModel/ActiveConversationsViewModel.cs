using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Newtonsoft.Json.Linq;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for ActiveConversations View. Responsible for providing data about active conversations.
    /// </summary>
    internal class ActiveConversationsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A list of active conversations.
        /// </summary>
        private ObservableCollection<ConversationModel> conversations = new ObservableCollection<ConversationModel>(ConversationManager.Instance.GetActiveConversations());
        public ObservableCollection<ConversationModel> Conversations { get { return conversations; } }

        /// <summary>
        /// The conversation currently selected. 
        /// </summary>
        private ConversationModel selectedConversation;
        public ConversationModel SelectedConversation
        {
            get { return selectedConversation; }
            set
            {
                selectedConversation = value;
                if (value != null)
                {
                    ConversationManager.Instance.AssignCurrentConversation(value.User.Address);
                }
                OnPropertyChanged(nameof(SelectedConversation));
            }
        }

        /// <summary>
        /// Constructor that setups the event listeners.
        /// </summary>
        
        public ActiveConversationsViewModel() {
            ConversationManager.Instance.conversationsUpdatedEvent  += ReloadConversations;
            ConversationManager.Instance.inactiveConversationSetEvent += InactiveConversationSet;
            ConversationManager.Instance.activeConversationSetEvent += ActiveConversationSet;
        }

        /// <summary>
        /// Sets selected conversations to null when the user selects a conversation from inactive conversations instead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InactiveConversationSet(object sender, EventArgs e)
        {
            SelectedConversation = null;
        }

        /// <summary>
        /// Updates current conversation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActiveConversationSet(object sender, EventArgs e)
        {
            if (SelectedConversation != ConversationManager.Instance.GetConversation())
                SelectedConversation = ConversationManager.Instance.GetConversation();
        }

        /// <summary>
        /// Fetches all active conversations from the Conversation Manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadConversations(object sender, EventArgs e)
        {
            conversations = new ObservableCollection<ConversationModel>(ConversationManager.Instance.GetActiveConversations());
            OnPropertyChanged("Conversations");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
