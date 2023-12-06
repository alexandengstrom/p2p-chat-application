using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.ViewModel.Command;
using ChatApp.View;
using System.Windows;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the main window.
    /// </summary>
    internal class ChatClientWindowViewModel : INotifyPropertyChanged {

        /// <summary>
        /// The title of the window. It will be a combination of name and endpoint.
        /// </summary>
        private string windowTitle = "";
        public string WindowTitle
        {
            get
            {
                return windowTitle;
            }
            set
            {
                windowTitle = value;
                OnPropertyChanged("WindowTitle");
            }
        }

        /// <summary>
        /// Boolean that triggers the window to shake. Activates when the user receives a buzz.
        /// </summary>
        private bool shouldShake;
        public bool ShouldShake
        {
            get { return shouldShake; }
            set
            {
                if (shouldShake != value)
                {
                    shouldShake = value;
                    OnPropertyChanged(nameof(ShouldShake));

                    Task.Delay(500).ContinueWith(t => ShouldShake = false);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Command that runs when the user closes the window.
        /// </summary>
        public ICommand onClose { get; private set; }
        public ICommand OnClose
        {
            get
            {
                if (onClose == null)
                {
                    onClose = new CloseWindowCommand(param => OnWindowClose(), null);
                }
                return onClose;
            }
            set { onClose = value; }
        }

        /// <summary>
        /// Constructor that setups the eventlistener and creates the window title.
        /// </summary>
        public ChatClientWindowViewModel()
        {
            ConversationManager.Instance.buzzEvent += ActivateBuzz;
            UserModel host = NetworkManager.Instance.Host;
            WindowTitle = $"{host.Name} - {host.Address}";
        }

        /// <summary>
        /// Sets the ShouldShake variable to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivateBuzz(object sender, EventArgs e)
        {
            ShouldShake = true;
        }

        /// <summary>
        /// Called by the onClose command. Asks network manager to close all connections. 
        /// </summary>
        public static void OnWindowClose()
        {
            NetworkManager.Instance.CloseServer();
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }
    }
}
