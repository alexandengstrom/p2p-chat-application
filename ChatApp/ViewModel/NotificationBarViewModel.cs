using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the Notification Bar view.
    /// </summary>
    class NotificationBarViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The notification string that will be displayed to the user.
        /// </summary>
        private string notification = "";
        public string Notification { get { return notification; } set { notification = value; OnPropertyChanged("Notification"); } }

        /// <summary>
        /// An instance of the Notification Manager.
        /// </summary>
        private NotificationManager notificationManager;
        public NotificationManager NotificationManager { get { return notificationManager; } set { notificationManager = value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Constructor that creates a notification manager and passes it the Network Manager and Conversation Manager.
        ///</summary>
        public NotificationBarViewModel(NotificationManager manager) 
        {
            notificationManager = manager;
            NetworkManager.Instance.NotificationManager = notificationManager;
            ConversationManager.Instance.NotificationManager = notificationManager;

            notificationManager.newNotification += DisplayNotification;
        }

        /// <summary>
        /// Displays all notifications in the queue if there is any.
        /// </summary>
        /// <returns></returns>
        private async Task SendNotification()
        {
            while (notificationManager.HasMoreNotifications())
            {
                Notification = notificationManager.GetLatestNotification();
                await Task.Delay(3000);
                notificationManager.DequeueNotification();
            }
            Notification = "";
        }

        /// <summary>
        /// Called when there is a new notification to display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisplayNotification(object sender, EventArgs e)
        {
            Task.Run(() => SendNotification().ConfigureAwait(false));
        }
    }
}
