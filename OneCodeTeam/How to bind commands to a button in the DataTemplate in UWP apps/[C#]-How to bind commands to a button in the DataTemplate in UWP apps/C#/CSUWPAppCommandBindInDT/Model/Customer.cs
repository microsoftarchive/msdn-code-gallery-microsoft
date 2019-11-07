using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSUWPAppCommandBindInDT.Model
{
    public class Customer : INotifyPropertyChanged
    {
        #region private fields
        private int id;
        private string name;
        private bool sex;
        private int age;
        private bool vip;
        #endregion

        #region Properties
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnNotifyPropertyChanged("Id");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnNotifyPropertyChanged("Name");
                }
            }
        }

        public bool Sex
        {
            get
            {
                return sex;
            }
            set
            {
                if (sex != value)
                {
                    sex = value;
                    OnNotifyPropertyChanged("Sex");
                }
            }
        }

        public int Age
        {
            get
            {
                return age;
            }
            set
            {
                if (age != value)
                {
                    age = value;
                    OnNotifyPropertyChanged("Age");
                }
            }
        }

        public bool Vip
        {
            get
            {
                return vip;
            }
            set
            {
                if (vip != value)
                {
                    vip = value;
                    OnNotifyPropertyChanged("Vip");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged Interface
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

}
