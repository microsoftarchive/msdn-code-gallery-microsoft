/****************************** Module Header ******************************\
 * Module Name:    CustomerViewModel.cs
 * Project:        CSUnvsAppCommandBindInDT
 * Copyright (c) Microsoft Corporation.
 * 
 * This is a ViewModel class which defines properties and Command will be used 
 * by View.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using CSUnvsAppCommandBindInDT.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
namespace CSUnvsAppCommandBindInDT.ViewModel
{
    class CustomerViewModel : INotifyPropertyChanged
    {
        // This property will be bound to GridView's ItemsDataSource property for providing data
        private ObservableCollection<Customer> m_customers;
        public ObservableCollection<Customer> Customers
        {
            get
            {
                return m_customers;
            }
            set
            {
                if (m_customers != value)
                {
                    m_customers = value;
                    OnPropertyChanged("Customers");
                }
            }
        }
 
        // This property will be bound to button's Command property for deleting item
        public ICommand DeleteCommand { set; get; }

        public CustomerViewModel()
        {
            // create a DeleteCommand instance
            this.DeleteCommand = new DelegateCommand(ExecuteDeleteCommand);

            // Get data source
            Customers = InitializeSampleData.GetData();
        }

        void ExecuteDeleteCommand(object param)
        {
            int id = (Int32)param;
            Customer cus = GetCustomerById(id);
            
            if (cus != null)
            {
                Customers.Remove(cus);
            }
        }

        // Get the deleting item by Id property
        private Customer GetCustomerById(int id)
        {
            try
            {
                return Customers.First(x => x.Id == id);
            }
            catch
            {
                return null;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion  
    }
}
