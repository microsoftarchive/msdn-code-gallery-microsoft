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

namespace CustomersSPListApp
{
    [DataContract]
    public class NewItemViewModel : NewItemViewModelBase
    {
        /// <summary>
        /// Provides values for fields of the List, given its name. Also used for databinding to New form UI
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
        /// Initializes the ViewModel properties
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Code to execute when a NewItem needs to be created on server
        /// </summary>
        /// <param name="callback"></param>
        public override void CreateItem()
        {
            base.CreateItem();
        }

        /// <summary>
        /// Code to Validate values for a field with given name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public override void Validate(string fieldName, object value)
        {
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
