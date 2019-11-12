/****************************** Module Header ******************************\
 Module Name:  Order.cs
 Project:      CSWPFMasterDetailBinding
 Copyright (c) Microsoft Corporation.
 
 This example demonstrates how to do master/detail data binding in WPF.
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel;

namespace CSWPFMasterDetailBinding.Data
{
    class Order : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _date;
        private string _shipCity;

        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        public string ShipCity
        {
            get { return _shipCity; }
            set
            {
                _shipCity = value;
                OnPropertyChanged("ShipCity");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
