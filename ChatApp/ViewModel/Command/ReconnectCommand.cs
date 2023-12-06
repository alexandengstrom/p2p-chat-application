using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    /// <summary>
    /// Command for reconnecting to an old conversation.
    /// </summary>
    internal class ReconnectCommand : ICommand
    {
        private ConversationViewModel parent;

        public ReconnectCommand(ConversationViewModel parent)
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
            System.Diagnostics.Debug.WriteLine("Button pressed...");
            parent.AttemptReconnection();
        }
    }
}
