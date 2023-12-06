using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the InactiveConversation view.
    /// </summary>
    internal class InactiveConversationsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A list of all old conversations.
        /// </summary>
        private ObservableCollection<ConversationModel> conversations;
        public ObservableCollection<ConversationModel> Conversations { get { return conversations; } }

        /// <summary>
        /// A filtered list of the old conversation based on a search query.
        /// </summary>
        private ObservableCollection<ConversationModel> filteredConversations = new ObservableCollection<ConversationModel>();
        public ObservableCollection<ConversationModel> FilteredConversations { get { return filteredConversations; } set { filteredConversations = value; OnPropertyChanged("FilteredConversations"); } }

        /// <summary>
        /// Search query for filtering old conversations. Binds to the search textbox.
        /// </summary>
        private string searchQuery = "";
        public string SearchQuery { 
            get { return searchQuery; } 
            set { 
                searchQuery = value; 
                OnPropertyChanged("SearchQuery");
                UpdateSearch();
            } 
        }

        /// <summary>
        /// The currently selected conversation.
        ///</summary>
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
        /// Refreshes the list of old conversation every minute.
        /// </summary>
        private async Task Refresh()
        {
            while (true)
            {
                UpdateSearch();
                await Task.Delay(60000);
            }
        }

        /// <summary>
        /// Constructor that setups all eventlisteners and loads all old conversations from the Conversation Manager.
        /// </summary>
        public InactiveConversationsViewModel()
        {
            ConversationManager.Instance.conversationsUpdatedEvent += ReloadInactiveConversations;
            ConversationManager.Instance.inactiveConversationSetEvent += InactiveConversationSet;
            ConversationManager.Instance.activeConversationSetEvent += ActiveConversationSet;
            conversations = new ObservableCollection<ConversationModel>(ConversationManager.Instance.GetInactiveConversations());
            FilteredConversations = conversations;
            Task.Run(() => { Refresh(); }).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the current conversation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InactiveConversationSet(object sender, EventArgs e)
        {
            if (SelectedConversation != ConversationManager.Instance.GetConversation())
                SelectedConversation = ConversationManager.Instance.GetConversation();
        }

        /// <summary>
        /// Sets the selected conversation to null when the user has chosen an active conversation instead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActiveConversationSet(object sender, EventArgs e)
        {
            SelectedConversation = null;
        }

        /// <summary>
        /// Fetches all old conversations from the Conversation Model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadInactiveConversations(object sender, EventArgs e)
        {
            conversations = new ObservableCollection<ConversationModel>(ConversationManager.Instance.GetInactiveConversations());
            UpdateSearch();
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

        /// <summary>
        /// Updates the list of filtered conversations every time the user updates the search query.
        /// </summary>
        private void UpdateSearch()
        {
            if (searchQuery.Length > 0)
            {
                FilteredConversations = new ObservableCollection<ConversationModel>(conversations.Where(item => item.User.Name.ToUpper().Contains(searchQuery.ToUpper())).ToList());
            }
            else
            {
                FilteredConversations = new ObservableCollection<ConversationModel>(conversations);
            }
        }
    }
}