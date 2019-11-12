using CSUWPAppCommandBindInDT.Model;
using CSUWPAppCommandBindInDT.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace CSUWPAppCommandBindInDT.ViewModel
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
            if (param.GetType().Equals(typeof(Customer)))
            {
                Customer customer = param as Customer;
                if (Customers.Contains(customer))
                {
                    Customers.Remove(customer);
                }
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
