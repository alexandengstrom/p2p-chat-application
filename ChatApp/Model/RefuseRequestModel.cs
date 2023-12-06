using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// The polymorphic variant of DataModel that declines an incoming request.
    /// </summary>
    public class RefuseRequestModel : DataModel
    {
        public RefuseRequestModel(UserModel sender, string receiver) : base(sender, receiver) { }
    }
}