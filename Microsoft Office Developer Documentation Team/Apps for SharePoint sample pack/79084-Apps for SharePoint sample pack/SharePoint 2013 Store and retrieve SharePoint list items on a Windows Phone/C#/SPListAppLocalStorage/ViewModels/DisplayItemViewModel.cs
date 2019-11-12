using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using Microsoft.SharePoint.Phone.Application;
using Microsoft.SharePoint.Client;

namespace SPListAppLocalStorage
{
    [DataContract]
    public class DisplayItemViewModel : DisplayItemViewModelBase
    {
        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for databinding to Display form UI
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public override object this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                base[fieldName] = value;
            }
        }

        /// <summary>
        /// Intializes the ViewModel properties
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Deletes the current ListItem from SharePoint server
        /// </summary>
        /// <param name="callback"></param>
        public override void DeleteItem()
        {
            base.DeleteItem();
        }
    }
}
