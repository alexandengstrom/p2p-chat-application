using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// This model contains all relevant information about the current user. When any DataModel is set, this is used to include the information about the sender.
    /// </summary>
    public class UserModel
    {
        private string ip {  get; }
        private string port { get; }
        private string address { get; }
        private string name { get; }

        /// <summary>
        /// Constructs a new UserModel.
        /// </summary>
        /// <param name="ip">The IP the user is listening on.</param>
        /// <param name="port">The port the user is listening on.</param>
        /// <param name="name">The name of the user.</param>
        public UserModel(string ip, string port, string name) { 
            this.ip = ip;
            this.port = port;
            this.address = ip + ":" + port;
            this.name = name;
        }

        /// <summary>
        /// Fetches the IP the user is listening on.
        /// </summary>
        public string Ip { get { return ip; } }
        /// <summary>
        /// Fetches the port number the user is listening on.
        /// </summary>
        public string Port { get { return port; } }
        /// <summary>
        /// Fetches the full endpoint (IP:Port) of the user. Formatted as "0.0.0.0:1000"
        /// </summary>
        public string Address { get { return ip + ":" + port; } }
        /// <summary>
        /// Fetches the name of the user.
        /// </summary>
        public string Name { get { return name; } }
    }
}
