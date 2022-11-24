using MasterPasswordDesktop.DataAccess;
using MasterPasswordDesktop.Infrastructure;
using MasterPasswordDesktop.ViewModels;
using MasterPasswordDesktop.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MasterPasswordDesktop
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex = null;

        public static bool IsDesignMode { get; private set; } = true;

        public static IDataStorage Database { get; } = new MyEncryptedFileStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.bin"));
        public static IEncryptProvider Encryptor { get; } = new Sha256Encryptor();

        protected override async void OnStartup(StartupEventArgs e)
        {           
            IsDesignMode = false;

            bool createdNew;
            _mutex = new Mutex(true, @"PainvenMasterPassword", out createdNew);
            if (!createdNew)
            {
                _mutex = null;
                Application.Current.Shutdown();
                return;
            }
            

            var window = new LoginWindow();
            if (e.Args.Any())
            {
                window.DataContext = new LoginWindowViewModel(e.Args.Last());
            }
            else
            {
                window.DataContext = new LoginWindowViewModel();
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
                _mutex.ReleaseMutex();

            base.OnExit(e);
        }
    }
}
