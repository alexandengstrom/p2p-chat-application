using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// The polymorphic variant of DataModel that is sent when a client connects to a host.
    /// </summary>
    public class ConnectionRequestModel : DataModel
    {
        public ConnectionRequestModel(UserModel sender, string receiver) : base(sender, receiver) { }
    }
}
