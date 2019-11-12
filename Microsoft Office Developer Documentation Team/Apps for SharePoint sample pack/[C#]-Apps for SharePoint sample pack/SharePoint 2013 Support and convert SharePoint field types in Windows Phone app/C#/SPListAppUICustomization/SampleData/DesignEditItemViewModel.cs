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

namespace SPListAppUICustomization
{
    [DataContract]
    public class DesignEditItemViewModel : EditItemViewModelBase
    {
        /// <summary>
        /// Provides edit values for fields of the List, given its name. Also used for data binding to Edit form UI
        /// </summary>
        public DesignEditItemViewModel()
        {

            //Title
            this["Title"] = "Sample Text";

            //Description
            this["Description"] = "Sample Text";

            //Product_x0020_Category
            this["Product_x0020_Category"] = new ObservableCollection<ChoiceFieldViewModel>
                                                            { 
                                                                new ChoiceFieldViewModel { Name = "Option 1", IsChecked = true }, 
                                                                new ChoiceFieldViewModel { Name = "Option 1", IsChecked = false }, 
                                                                new ChoiceFieldViewModel { Name = "Option 1", IsChecked = false }
                                                            };

            //Quantity
            this["Quantity"] = "99.5";

            //Order_x0020_Date
            this["Order_x0020_Date"] = "1/21/2012";

            //Fulfillment_x0020_Date
            this["Fulfillment_x0020_Date"] = "1/21/2012";

            //Rush
            this["Rush"] = true;

            //Contact_x0020_Number
            this["Contact_x0020_Number"] = "Sample Text";

            //Attachments
            this["Attachments"] = new ObservableCollection<AttachmentFieldViewModel>
                                                            { 
                                                                new AttachmentFieldViewModel { Name = "FileName1.txt", IsChecked = true },
                                                                new AttachmentFieldViewModel { Name = "Picture1.jpg", IsChecked = true }, 
                                                                new AttachmentFieldViewModel { Name = "Test.docx", IsChecked = true }
                                                            };

        }

        /// <summary>
        /// Provides edit values for fields of the List, given its name. Also used for data binding to Edit form UI
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
