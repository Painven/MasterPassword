using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;

namespace MasterPasswordDesktop.Infrastructure
{
    public class AfkChecker : IDisposable
    {
        public event Action AfkTimeLimitExceeded;
        public TimeSpan MaximumAfkTiem { get; }
        public TimeSpan CheckInterval { get; } = TimeSpan.FromSeconds(15);

        public TimeSpan CurrentAfkTime { get; private set; }
        private DateTime lastActiveDateTime = DateTime.Now;
        Timer afkTimer;

        public AfkChecker(TimeSpan maximumAfkTime)
        {
            MaximumAfkTiem = maximumAfkTime;
            Application.Current.Deactivated += Current_Deactivated;
            Application.Current.Activated += Current_Activated;

            afkTimer = new Timer(CheckInterval.TotalMilliseconds);
            afkTimer.Elapsed += AfkElapsed;
            afkTimer.Start();
        }

        public void AfkElapsed(object o, EventArgs e)
        {
            if (lastActiveDateTime + CheckInterval < DateTime.Now)
            {
                CurrentAfkTime += CheckInterval;
                if (CurrentAfkTime >= MaximumAfkTiem)
                {
                    AfkTimeLimitExceeded?.Invoke();
                }
            }
            else
            {
                lastActiveDateTime = DateTime.Now;
                CurrentAfkTime = TimeSpan.Zero;
            }
        }

        private void Current_Activated(object sender, EventArgs e)
        {
            lastActiveDateTime = DateTime.Now;
            CurrentAfkTime = TimeSpan.Zero;
        }

        private void Current_Deactivated(object sender, EventArgs e)
        {
            lastActiveDateTime = DateTime.Now;
        }

        public void Dispose()
        {
            afkTimer.Stop();
            afkTimer.Elapsed -= AfkElapsed;
            afkTimer.Dispose();
            Application.Current.Deactivated -= Current_Deactivated;
            Application.Current.Activated -= Current_Activated;
        }
    }


}
