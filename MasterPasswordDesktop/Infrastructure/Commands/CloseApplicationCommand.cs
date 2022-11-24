using MasterPasswordDesktop.ViewModels;
using System.Windows;

namespace MasterPasswordDesktop.Commands
{
    public class CloseApplicationCommand : Command
    {
        public override bool CanExecute(object p) => true;
        public override void Execute(object p) => Application.Current.Shutdown();
    }
}
