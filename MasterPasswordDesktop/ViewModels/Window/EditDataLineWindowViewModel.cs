using MasterPasswordDesktop.Commands;
using MasterPasswordDesktop.DataAccess;
using MasterPasswordDesktop.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MasterPasswordDesktop.ViewModels
{
    public class EditDataLineWindowViewModel : ViewModelBase
    {
        public event Action<DataLine> OnSave;
        public event Action<DataLine> OnCancel;

        private DataLine _dataLine = new DataLine();
        public DataLine Item
        {
            get => _dataLine;
            set
            {
                Set(ref _dataLine, value);
                if (value != null)
                {
                    bool isNewItem = string.IsNullOrWhiteSpace(value.Title);

                    IsLocked = !isNewItem;
                    LockerVisibility = isNewItem ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private bool _isLocked = true;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                Set(ref _isLocked, value);
                RaisePropertyChanged(nameof(LockerIconContent));
            }
            
        }
       
        private Visibility _lockerVisibility;
        public Visibility LockerVisibility 
        { 
            get => _lockerVisibility; 
            set => Set(ref _lockerVisibility, value); 
        }

        public string LockerIconContent 
        { 
            get { return IsLocked ? "\uf023" : "\uf3c1"; } 
        }

        public ICommand LockerStateChangeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public EditDataLineWindowViewModel()
        {
            LockerStateChangeCommand = new LambdaCommand((parameter) => IsLocked = !IsLocked);
            SaveCommand = new LambdaCommand(Submit, p => !string.IsNullOrWhiteSpace(Item.Title) && !IsLocked);
            CloseWindowCommand = new LambdaCommand(Cancel);
        }

        private void Cancel(object window)
        {
            OnCancel(Item);
            CloseWindow(window);
        }

        private void Submit(object window)
        {      
            OnSave(Item);
            CloseWindow(window);
        }

        private void CloseWindow(object window)
        {
            (window as Window).Close();
        }

    }
}
