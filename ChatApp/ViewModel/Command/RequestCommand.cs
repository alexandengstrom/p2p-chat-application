using ChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    /// <summary>
    /// Command for sending a new chat request.
    /// </summary>
    internal class RequestCommand : ICommand
    {
        private SidebarRequestViewModel parent;

        public RequestCommand(SidebarRequestViewModel parent)
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
            parent.SendNewRequest();
        }
    }
}
