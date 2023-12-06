using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{   
    /// <summary>
    /// This class sends, stores and manages notifications to the GUI.
    /// </summary>
    public class NotificationManager
    {
        private Queue<String> notifications = new Queue<String>();
        public event EventHandler newNotification;
        private readonly object _lock = new();

        /// <summary>
        /// Helper method to determine if there are more pending notifications.
        /// </summary>
        /// <returns>True if there are more notifications to dispay, otherwise false.</returns>
        public bool HasMoreNotifications() => notifications.Count != 0;

        /// <summary>
        /// Adds a new notification to the queue.
        /// </summary>
        /// <param name="message">The notification to enqueue.</param>
        public void AddNotification(string message)
        {
            // can be called from several threads, ensures only one thread at a time manipulates the data
            lock (_lock)
            {
                notifications.Enqueue(message);
            }
            if (notifications.Count == 1)
            {
                // notifies the viewmodel of the new notification if this is the only one in queue
                newNotification?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fetches the oldest notification
        /// </summary>
        /// <returns>The oldest notificaiton.</returns>
        public string GetLatestNotification()
        {
            return notifications.First();
        }

        /// <summary>
        /// To ensure everything is handled in the proper order, this method is used to dequeue a notification after its been displayed.
        /// </summary>
        public void DequeueNotification()
        {
            // can be called from several threads, ensures only one thread at a time manipulates the data
            lock (_lock)
            {
                notifications.Dequeue();
            }
        }
    }
}
