/****************************** Module Header ******************************\
 * Module Name:  Customer.cs
 * Project:      CSUnvsAppEnumRadioButton
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the demo data
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.ComponentModel;
namespace CSUnvsAppEnumRadioButton
{
    public class Customer : INotifyPropertyChanged           
    {
        private string name;
        public string Name
        {
            get { return name; }
            set 
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private int age;
        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                NotifyPropertyChanged("Age");
            }
        }

        private bool sex;
        public bool Sex
        {
            get { return sex; }
            set
            {
                sex = value;
                NotifyPropertyChanged("Sex");
            }
        }

        private Sport favouriteSport;
        public Sport FavouriteSport
        {
            get { return favouriteSport; }
            set
            {
                favouriteSport = value;
                NotifyPropertyChanged("FavouriteSport");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum Sport
    { 
        Football,
        Basketball,
        Baseball,
        Swimming
    }
}
