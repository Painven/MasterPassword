using MasterPasswordDesktop.Commands;
using MasterPasswordDesktop.Model;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MasterPasswordDesktop.ViewModels.Controls
{
    public class DataLineViewModel : ViewModelBase
    {
        public event Action<DataLine> OnEditButtonPressed;
        public event Action<DataLine> OnDataCopy;

        public ICommand CopyPasswordCommand { get; }
        public ICommand CopyHostCommand { get; }
        public ICommand CopyEmailCommand { get; }
        public ICommand OpenEditWindowCommand { get; }
        public ICommand CopyPhoneNumberCommand { get; }
        public ICommand CopyLoginCommand { get; }

        public string LastViewString
        {
            get
            {
                if (DataLine.CreationDate == DataLine.LastViewDate)
                {
                    return "никогда";
                }
                else
                {
                    if (DataLine.LastViewDate.Date == DateTime.Now.Date)
                    {
                        return "сегодня";
                    }

                    int daysElapsed = (int)Math.Ceiling((DateTime.Now - DataLine.LastViewDate).TotalDays);
                    return $"{daysElapsed} дн. назад";
                }
           
            }
        }

        private DataLine _dataLine = new DataLine();
        public DataLine DataLine
        {
            get => _dataLine;
            set => Set(ref _dataLine, value);
        }

        public DataLineViewModel(DataLine dataLine) : this()
        {
            DataLine = dataLine;

            OpenEditWindowCommand = new LambdaCommand((o) => OnEditButtonPressed?.Invoke(DataLine));

            CopyPasswordCommand = new CopyParameterToClipboardCommand(() => OnDataCopy?.Invoke(DataLine));
            CopyEmailCommand = new CopyParameterToClipboardCommand(() => OnDataCopy?.Invoke(DataLine));
            CopyHostCommand = new CopyParameterToClipboardCommand(() => OnDataCopy?.Invoke(DataLine));
            CopyPhoneNumberCommand = new CopyParameterToClipboardCommand(() => OnDataCopy?.Invoke(DataLine));
            CopyLoginCommand = new CopyParameterToClipboardCommand(() => OnDataCopy?.Invoke(DataLine));

        }

        public DataLineViewModel()
        {

        }
    }
}
