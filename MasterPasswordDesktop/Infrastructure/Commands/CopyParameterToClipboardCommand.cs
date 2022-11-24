using MasterPasswordDesktop.ViewModels;
using System;
using System.Windows;

namespace MasterPasswordDesktop.Commands
{
    public class CopyParameterToClipboardCommand : Command
    {
        Action action;

        public override void Execute(object p)
        {
            Clipboard.SetText(p?.ToString() ?? string.Empty);
            action?.Invoke();
        }

        public CopyParameterToClipboardCommand(Action action)
        {
            this.action = action;
        }

        public override bool CanExecute(object parameter)
        {
            if(parameter == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(parameter.ToString());
        }
    }
}
