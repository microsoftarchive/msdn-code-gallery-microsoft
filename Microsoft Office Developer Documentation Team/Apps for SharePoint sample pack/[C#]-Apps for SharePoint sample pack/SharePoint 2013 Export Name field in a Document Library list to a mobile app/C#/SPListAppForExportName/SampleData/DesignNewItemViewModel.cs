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
using System.Collections.ObjectModel;

namespace SPListAppForExportName
{
    [DataContract]
    public class DesignNewItemViewModel : NewItemViewModelBase
    {
        /// <summary>
        /// Provides new item values for fields of the List, given its name. Also used for data binding to New form UI
        /// </summary>
        public DesignNewItemViewModel()
        {

            //Title
            this["Title"] = "Sample Text";

        }


        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for data binding to New form UI
        /// </summary>
        /// <param name="fieldName" />
        /// <returns />
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
    }
}
