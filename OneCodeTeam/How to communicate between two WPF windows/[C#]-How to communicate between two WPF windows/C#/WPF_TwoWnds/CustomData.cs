using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WPF_TwoWnds
{
    public class CustomData : INotifyPropertyChanged
    {
        public string _userName { get; set; }        
        public string UserName
        {
            get
            {
                return _userName;
            }
            set {
                _userName = value;
                OnPropertyChanged("UserName");
            
            }
        }

        public string _info { get; set; }
        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                OnPropertyChanged("Info");

            }
        }


        private bool _bIsVisible = false;
        public bool IsVisible
        {
            get { return _bIsVisible; }
            set
            {
                _bIsVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }




        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(
                    this, new PropertyChangedEventArgs(propName));
        }

        #endregion


    }
}
