using MasterPasswordDesktop.ViewModels;
using System.Windows;

namespace MasterPasswordDesktop.Commands
{
    public class CloseWindowCommand : Command
    {
        public override void Execute(object p)
        {
            var window = (p as Window);
            window?.Close();
        }
    }
}
