using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    /// <summary>
    /// Command for accepting a chat request.
    /// </summary>
    internal class AcceptRequestCommand : ICommand
    {
        private PendingRequestBarViewModel parent;

        public AcceptRequestCommand(PendingRequestBarViewModel parent)
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
            parent.AcceptRequest();
        }
    }

}