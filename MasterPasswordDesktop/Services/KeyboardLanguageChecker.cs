using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;

namespace MasterPasswordDesktop.Services
{
    public class KeyboardLanguageChecker : IDisposable
    {
        Timer languageTimer = new Timer();

        string tempLanguageTwoLetters;
        public event EventHandler<string> OnChange;
        Dispatcher mainDispatcher;

        public KeyboardLanguageChecker()
        {
            languageTimer.Interval = TimeSpan.FromMilliseconds(200).TotalMilliseconds;
            languageTimer.Elapsed += LanguageTimer_Elapsed;
            mainDispatcher = Dispatcher.CurrentDispatcher;

            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-EN");
        }

        private void LanguageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var currentLanguageTwoLetters = mainDispatcher.Invoke(() =>  InputLanguageManager.Current.CurrentInputLanguage.TwoLetterISOLanguageName.ToUpper());
                if (currentLanguageTwoLetters != tempLanguageTwoLetters)
                {
                    OnChange(this, currentLanguageTwoLetters);
                    tempLanguageTwoLetters = currentLanguageTwoLetters;
                }
            }
            catch(Exception ex)
            {

            }

        }

        public void Dispose()
        {
            languageTimer.Elapsed -= LanguageTimer_Elapsed;
            languageTimer.Stop();
            languageTimer.Dispose();
        }

        internal void Start()
        {
            languageTimer.Start();
        }
    }
}
