using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MasterPasswordDesktop.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposing || _disposed)
            {
                return;
            }
            _disposed = true;
        }

        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if(Equals(field, value)) { return false; }
            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}