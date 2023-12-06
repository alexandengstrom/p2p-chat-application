using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// The polymorphic variant of DataModel that contains a message
    /// </summary>
    public class MessageModel : DataModel
    {
        public MessageModel(UserModel sender, string receiver, string message) : base(sender, receiver)
        {
            Message = message;
        }
    }
}
