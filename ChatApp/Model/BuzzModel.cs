using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// The polymorphic variant of DataModel that allows a buzz to be sent.
    /// </summary>
    public class BuzzModel : DataModel
    {
        public BuzzModel(UserModel sender, string receiver) : base (sender, receiver) { }
    }
}
