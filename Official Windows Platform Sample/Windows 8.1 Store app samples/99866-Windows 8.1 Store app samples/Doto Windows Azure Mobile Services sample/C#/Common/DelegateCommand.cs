// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Doto
{
    /// <summary>
    /// Represents a command for use in databinding and MVVM, supports enable, disable
    /// and execution with a command parameter
    /// </summary>
    /// <typeparam name="T">The expected type of the command parameter</typeparam>
    public class DelegateCommand<T> : INotifyPropertyChanged, ICommand where T : class
    {
        private bool isEnabled = true;
        private readonly Action<T> action;
        private readonly Predicate<T> canExecute;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
                RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecuteArgument = null)
        {
            action = execute;
            canExecute = canExecuteArgument ?? (t => isEnabled);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <summary>
    /// Represents a command for use in databinding and MVVM, supports enable, disable
    /// and execution
    /// </summary>
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute, bool isEnabled = true)
            : base(o => execute())
        {
        }
    }
}
