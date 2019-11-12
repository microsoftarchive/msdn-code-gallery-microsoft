//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.ComponentModel;

namespace DataBinding
{
    // More information on INotifyPropertyChanged can be found @ http://go.microsoft.com/fwlink/?LinkId=254639#change_notification
    // For another way to accomplish this, see SampleDataSource.cs and BindableBase.cs in the Grid Application C# project template.
    public class Employee : INotifyPropertyChanged //Implement INotifiyPropertyChanged interface to subscribe for property change notifications
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        private string _organization;
        private int? _age;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public string Organization
        {
            get { return _organization; }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    RaisePropertyChanged("Organization");
                }
            }
        }

        public int? Age
        {
            get { return _age; }
            set
            {
                if (_age != value)
                {
                    _age = value;
                    RaisePropertyChanged("Age");
                }
            }
        }

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
