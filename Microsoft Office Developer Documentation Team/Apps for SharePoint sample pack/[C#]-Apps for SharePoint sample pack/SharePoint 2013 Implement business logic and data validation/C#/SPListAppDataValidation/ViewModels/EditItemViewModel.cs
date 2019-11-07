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
using System.Globalization;
using System.Text.RegularExpressions;


namespace SPListAppDataValidation
{
    [DataContract]
    public class EditItemViewModel : EditItemViewModelBase
    {
        /// <summary>
        /// Provides edit values for fields of the List, given its name. Also used for databinding to Edit form UI
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
                Validate(fieldName, value);
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
        /// Updates current instance of List Item to server
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdateItem()
        {
            base.UpdateItem();
        }

        /// <summary>
        /// Code to Validate values for a field with given name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public override void Validate(string fieldName, object value)
        {
            string fieldValue;
            if (value != null)
            {
                fieldValue = value.ToString();
            }
            else
            {
                fieldValue = null;
            }
            if (!string.IsNullOrEmpty(fieldValue)) //Allowing for blank fields.
            {
                bool isProperValue = false;

                switch (fieldName)
                {
                    case "Quantity":
                        // Enforce ordering Fuzzy Dice in pairs only.
                        int quantityOrdered;
                        decimal quantityField;
                        isProperValue = Decimal.TryParse(fieldValue, out quantityField);

                        if (isProperValue)
                        {
                            quantityOrdered = Decimal.ToInt32(quantityField);
                            if ((quantityOrdered % 2) != 0) // Odd number of product items ordered.
                            {
                                if ((string)this["Title"] == "Fuzzy Dice")
                                {
                                    AddError("Item[Quantity]", "Fuzzy Dice must be ordered in pairs. No such thing as a Fuzzy Die!");
                                }
                                else
                                {
                                    // Restriction on ordering in pairs doesn't apply to other products.
                                    RemoveAllErrors("Item[Quantity]");
                                }
                            }
                            else
                            {
                                RemoveAllErrors("Item[Quantity]");
                            }
                        }
                        break;
                    case "Fulfillment_x0020_Date":
                        // Determine whether fulfillment date is later than order date.
                        DateTime fulfillmentDate;
                        isProperValue = DateTime.TryParse(fieldValue, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out fulfillmentDate);
                        if (isProperValue)
                        {
                            DateTime orderDate;
                            isProperValue = DateTime.TryParse((string)this["Order_x0020_Date"], CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out orderDate);

                            if (fulfillmentDate.CompareTo(orderDate) > 0)
                            {
                                RemoveAllErrors("Item[Fulfillment_x0020_Date]");
                            }
                            else
                            {
                                AddError("Item[Fulfillment_x0020_Date]", "Fulfillment Date must be later than Order Date.");
                            }
                        }
                        break;
                    case "Contact_x0020_Number":
                        // Check that contact number is in an appropriate format.
                        Regex rx = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
                        if (rx.IsMatch(fieldValue))
                        {
                            RemoveAllErrors("Item[Contact_x0020_Number]");
                        }
                        else
                        {
                            //Specified Contact Number is not a valid phone number.
                            AddError("Item[Contact_x0020_Number]", "Specified Contact Number is invalid.");
                        }
                        break;
                    default:
                        // Not adding custom validation for other fields.
                        break;
                }
            }

            //And then proceed with default validation from base class.
            base.Validate(fieldName, value);
        }

        /// <summary>
        /// Code to execute when location of device changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnGeolocationStatusChanged(object sender, System.Device.Location.GeoPositionStatusChangedEventArgs e)
        {
            base.OnGeolocationStatusChanged(sender, e);
        }

        /// <summary>
        /// Code to execute when photo is selected from media library or a photo is taken using camera to add attachment to ListItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPhotoSelectionCompleted(object sender, Microsoft.Phone.Tasks.PhotoResult e)
        {
            base.OnPhotoSelectionCompleted(sender, e);
        }
    }
}
