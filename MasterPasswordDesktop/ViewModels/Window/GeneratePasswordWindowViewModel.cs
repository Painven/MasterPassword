using MasterPasswordDesktop.Commands;
using MasterPasswordDesktop.Infrastructure;
using MasterPasswordDesktop.Model;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MasterPasswordDesktop.Infrastructure.Helpers;
using System.Collections.ObjectModel;

namespace MasterPasswordDesktop.ViewModels
{
    public class GeneratePasswordWindowViewModel : ViewModelBase
    {
        public int MinimumPasswordLength { get; set; } = 4;
        public int MaximumPasswordLength { get; set; } = 32;

        public ObservableCollection<string> GeneratedItems { get; }

        private bool _useGuid = false;
        public bool UseGuid
        {
            get => _useGuid;
            set
            {
                Set(ref _useGuid, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private bool _useEnglishLetters = true;
        public bool UseEnglishLetters
        {
            get => _useEnglishLetters;
            set
            {
                Set(ref _useEnglishLetters, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private bool _useRussianLetters = false;
        public bool UseRussianLetters
        {
            get => _useRussianLetters;
            set
            {
                Set(ref _useRussianLetters, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private bool _useLowerCase = true;
        public bool UseLowerCase
        {
            get => _useLowerCase;
            set
            {
                Set(ref _useLowerCase, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private bool _useDigits = true;
        public bool UseDigits
        {
            get => _useDigits;
            set
            {
                Set(ref _useDigits, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private bool _useUpperCase = true;
        public bool UseUpperCase
        {
            get => _useUpperCase;
            set
            {
                Set(ref _useUpperCase, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private bool _useSpecialSymbols = false;
        public bool UseSpecialSymbols
        {
            get => _useSpecialSymbols;
            set
            {
                Set(ref _useSpecialSymbols, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        private int _passwordLength = 12;
        public int PasswordLength
        {
            get => _passwordLength;
            set
            { 
                Set(ref _passwordLength, value);
                RaisePropertyChanged(nameof(GeneratedPassword));
            }
        }

        public string GeneratedPassword { get { return PasswordGenerator.Generate(PasswordLength, Options); } }

        public ICommand GeneratePasswordCommand { get; }
        public ICommand CopyGeneratedPasswordCommand { get; }
        public ICommand CopyAllPasswordsCommand { get; }
        public ICommand ClearAllAddedPasswordsCommand { get; }
        public ICommand RemoveLastAddedPasswordCommand { get; }
        public ICommand AddPasswordCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public GeneratePasswordWindowViewModel()
        {
            GeneratedItems = new ObservableCollection<string>();
            if(App.IsDesignMode)
            {
                GeneratedItems.Add("12345");
                GeneratedItems.Add("вапрол");
                GeneratedItems.Add("pass12345");
            }

            CloseWindowCommand = new CloseWindowCommand();
            GeneratePasswordCommand = new LambdaCommand((o) => RaisePropertyChanged(nameof(GeneratedPassword)), (param) => Options != PasswordGeneratorOptions.None);
            AddPasswordCommand = new LambdaCommand((password) =>
            {
                GeneratedItems.Add(password.ToString());
                RaisePropertyChanged(nameof(GeneratedPassword));

            }, (param) => !string.IsNullOrWhiteSpace(param?.ToString()));
            CopyGeneratedPasswordCommand = new CopyParameterToClipboardCommand(null);

            CopyAllPasswordsCommand = new LambdaCommand(CopyAllAddedPasswords, (o) => GeneratedPassword.Any());
            ClearAllAddedPasswordsCommand = new LambdaCommand((o) => GeneratedItems.Clear(), (o) => GeneratedItems.Any());
            RemoveLastAddedPasswordCommand = new LambdaCommand((o) => GeneratedItems.Remove(GeneratedItems.Last()), (o) => GeneratedItems.Any());

        }

        public PasswordGeneratorOptions Options
        {
            get
            {
                var optionsProperties = this.GetType().GetProperties()
                    .Where(pi => pi.PropertyType == typeof(bool) && pi.Name.StartsWith("Use"))
                    .Select(pi => new
                    {
                        Name = pi.Name.Replace("Use", string.Empty),
                        Value = (bool)pi.GetValue(this)
                    })
                    .ToDictionary(i => i.Name, j => j.Value);

                PasswordGeneratorOptions flags = new PasswordGeneratorOptions();

                foreach (var kvp in optionsProperties)
                {
                    if (!kvp.Value) { continue; }

                    var flag = kvp.Key.ToEnumOfType<PasswordGeneratorOptions>();
                    
                    if(flags == PasswordGeneratorOptions.None)
                    {
                        flags = flag;
                    }
                    else
                    {
                        flags |= flag;
                    }
                }

                return flags;
            }
        }

        private void CopyAllAddedPasswords(object parameter)
        {
            if(GeneratedItems.Any())
            {
                var data = string.Join(Environment.NewLine, GeneratedItems.Select(item => item.ToString()));
                Clipboard.SetText(data);
            }
        }
    }
}
