using MasterPasswordDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MasterPasswordDesktop.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).LoadedCommand.Execute(null);
        }
    }
}
