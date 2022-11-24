using MasterPasswordDesktop.Commands;
using MasterPasswordDesktop.DataAccess;
using MasterPasswordDesktop.Infrastructure;
using MasterPasswordDesktop.Services;
using MasterPasswordDesktop.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MasterPasswordDesktop.ViewModels
{
    public class LoginWindowViewModel : ViewModelBase
    {
        private string _commandLinePassword;

        private bool _hidePassword = true;
        public bool IsHidePassword
        {
            get => _hidePassword;
            set
            {
                Set(ref _hidePassword, value);
                RaisePropertyChanged(nameof(ViewPasswordIconContent));
                RaisePropertyChanged(nameof(UnmaskedPasswordFieldVisibility));
                RaisePropertyChanged(nameof(MaskedPasswordFieldVisibility));
            }
        }
        public string ViewPasswordIconContent { get { return IsHidePassword ? "\uf06e" : "\uf070"; } }
        public Visibility UnmaskedPasswordFieldVisibility { get { return IsHidePassword ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility MaskedPasswordFieldVisibility { get { return IsHidePassword ? Visibility.Visible : Visibility.Collapsed; } }

        private int _tryCountLeft = 5;
        public int TryCountLeft
        {
            get => _tryCountLeft;
            set { Set(ref _tryCountLeft, value); RaisePropertyChanged(nameof(TryCounterVisibility)); }
        }
        public Visibility TryCounterVisibility { get { return TryCountLeft < 5 ? Visibility.Visible : Visibility.Hidden; } }

        private string _languageTwoLetter;
        public string LanguageTwoLetter
        {
            get => _languageTwoLetter;
            set => Set(ref _languageTwoLetter, value);
        }

        private KeyboardLanguageChecker LanguageTimer { get; }

        public ICommand ShowPasswordCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand CloseWindowCommand { get; }
        public ICommand LoadedCommand { get; }

        public LoginWindowViewModel()
        {
            ShowPasswordCommand = new LambdaCommand(ShowHidePassword);
            LoginCommand = new LambdaCommand(Login);
            LoadedCommand = new LambdaCommand(Login);
            CloseWindowCommand = new CloseWindowCommand();

            LanguageTimer = new KeyboardLanguageChecker();
            LanguageTimer.OnChange += (o, e) => LanguageTwoLetter = e;
            LanguageTimer.Start();
        }

        public LoginWindowViewModel(string startPassword) : this()
        {
            _commandLinePassword = startPassword;           
        }

        private void ShowHidePassword(object parameter)
        {                   
            var window = parameter as LoginWindow;
            var unmaskedPasswordBox = window.txtUnmaskedPassword;
            var maskedPasswordBox = window.passwordBox;

            IsHidePassword = !IsHidePassword;

            if (IsHidePassword)
            {
                maskedPasswordBox.Password = unmaskedPasswordBox.Text;
                maskedPasswordBox.Focus();
            }
            else
            {
                unmaskedPasswordBox.Text = maskedPasswordBox.Password;
                unmaskedPasswordBox.Focus();
            }
            
        }

        private void Login(object parameter)
        {
            var loginWindow = App.Current.Windows[0] as LoginWindow;

            string password = _commandLinePassword == null ? 
                (IsHidePassword ? loginWindow.passwordBox.Password : loginWindow.txtUnmaskedPassword.Text) :
                _commandLinePassword;

            if (!string.IsNullOrEmpty(password))
            {
                if (IsValidPassword(password))
                {
                    ((MyEncryptedFileStorage)App.Database).SetPassword(password);

                    var mainWindow = new MainWindow();
                    mainWindow.DataContext = new MainWindowViewModel();

                    
                    mainWindow.Show();
                    loginWindow.Close();
                }
                else
                {
                    TryCountLeft--;

                    if (TryCountLeft == 0)
                    {
                        Application.Current.Shutdown();
                    }
                }
                             
            }
        }

        private bool IsValidPassword(string password)
        {
            return App.Encryptor.Check(password, ConfigurationManager.AppSettings["public_key"]);
        }
    }
}
