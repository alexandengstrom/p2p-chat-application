using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// ViewModel for the main window.
    /// </summary>
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Error message that contais a specific error message depending on what kind of error has occured.
        /// </summary>
        private string errorMessage = "";
        public string ErrorMessage { get { return errorMessage; } set { errorMessage = value; OnPropertyChanged("ErrorMessage"); } }

        /// <summary>
        /// A list of available IP-addresses to start listenening on.
        /// </summary>
        private ObservableCollection<string> ipAddresses = new ObservableCollection<string>();
        public ObservableCollection<string> IpAddresses { get { return ipAddresses; } }

        /// <summary>
        /// The currently choosen IP address.
        /// </summary>
        private string selectedIp = "127.0.0.1";
        public string SelectedIp { get { return selectedIp; } set { selectedIp = value;  } }

        /// <summary>
        /// The name that the user enters in the textbox.
        /// </summary>
        private string name;
        public string Name { 
            get { return name; } 
            set { name = value; }
        }

        /// <summary>
        /// The port that the users enters in the textbox.
        /// </summary>
        private string port;
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Command to start the chat window.
        /// </summary>
        private ICommand startClient;
        public ICommand StartClient { 
            get {
                if (startClient == null)
                {
                    startClient = new LoginCommand(this);
                }
                return startClient; 
            } 
            set { startClient = value; }
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
        /// Constructor that setups event listeners and fetches all available IP addresses from the Network Manager.
        /// </summary>
        public MainWindowViewModel() {
            NetworkManager.Instance.listenerFailedEvent += OnError;
            NetworkManager.Instance.listenerSuccessEvent += OnSuccess;
            ipAddresses.Add("127.0.0.1");

            string localIp = NetworkManager.GetIpAddress();

            if (localIp != null)
            {
                ipAddresses.Add(localIp);
            }

            OnPropertyChanged("IpAddresses");
        }

        /// <summary>
        /// Starts the chat window or displays an error message after the user try to start.
        /// </summary>
        public void StartChatClient()
        {
            if (name.Length < 2)
            {
                ErrorMessage = "Name must be at least two characters long.";
            }
            else if (!IsValidPort())
            {
                ErrorMessage = "Please choose a port between 10 000 and 64 0000.";
            }
            else if (NetworkManager.IsPortOccupied(port))
            {
                ErrorMessage = "The port " + port + " is currently occupied.";
            }
            else
            {
                ErrorMessage = "";
                NetworkManager.Instance.Listen(new UserModel("127.0.0.1", port, name));
                ChatClientWindow chatClientWindow = new ChatClientWindow();
                chatClientWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Displays an error message if the Network Manager failed to start listening on the port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnError(object sender, EventArgs e)
        {
            ErrorMessage = $"Failed to start listening on port {port}";
        }

        /// <summary>
        /// Starts the chat client if there was no errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSuccess(object sender, EventArgs e)
        {
            ErrorMessage = "";
            NetworkManager.Instance.Listen(new UserModel(selectedIp, port, name));
            ChatClientWindow chatClientWindow = new ChatClientWindow();
            chatClientWindow.ShowDialog();                
        }

        /// <summary>
        /// Verifies if a port is valid.
        /// </summary>
        /// <returns></returns>
        private bool IsValidPort()
        {
            Int32 portInt = 0;

            try
            {
                portInt = Convert.ToInt32(port);
            } catch (FormatException)
            {
                return false;
            }

            if (portInt < 9999 || portInt > 64001)
            {
                return false;
            }

            return true;
        }
    }
}
