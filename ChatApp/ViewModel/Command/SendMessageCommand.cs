using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    /// <summary>
    /// Command for sending a new message.
    /// </summary>
    internal class SendMessageCommand : ICommand
    {
        private ConversationViewModel parent;

        public SendMessageCommand(ConversationViewModel parent)
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
            parent.SendMessage();
        }
    }

}