using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// The polymorphic variant of DataModel that tells the recipient to close the connection.
    /// </summary>
    public class CloseConnectionModel : DataModel
    {
        public CloseConnectionModel(UserModel sender, string receiver) : base(sender, receiver) { }
    }
}