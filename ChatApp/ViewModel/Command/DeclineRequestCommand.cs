using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    /// <summary>
    /// Command for declining a chat request.
    /// </summary>
    internal class DeclineRequestCommand : ICommand
    {
        private PendingRequestBarViewModel parent;

        public DeclineRequestCommand(PendingRequestBarViewModel parent)
        {
            this.parent = parent;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.DeclineRequest();
        }
    }

}