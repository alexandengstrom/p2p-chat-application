using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// The polymorphic variant of DataModel that accepts an incoming request.
    /// </summary>
    public class AcceptRequestModel : DataModel
    {
        public AcceptRequestModel(UserModel sender, string receiver) : base(sender, receiver) { }
    }
}