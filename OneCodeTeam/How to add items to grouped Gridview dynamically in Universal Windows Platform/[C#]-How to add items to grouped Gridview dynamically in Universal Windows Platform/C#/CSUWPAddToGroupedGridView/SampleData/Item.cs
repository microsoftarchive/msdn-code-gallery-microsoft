/****************************** Module Header ******************************\
 * Module Name:  Item.cs
 * Project:      CSUWPAddToGroupedGridView
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the sample data which stored in the collection
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

namespace CSUWPAddToGroupedGridView.SampleData
{
    using System;

    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The class refers to a single item in the collection.
    /// </summary>
    public class Item : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Backing variable for the Title property.
        /// </summary>
        private string _title = string.Empty;

        /// <summary>
        /// Backing variable for the Image property.
        /// </summary>
        private ImageSource _image;

        /// <summary>
        /// Backing variable for the Category property.
        /// </summary>
        private string _category = string.Empty;

        /// <summary>
        /// The event notifies all listeners that a property changed. 
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the title of the item.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// Gets or sets the image showing the item.
        /// </summary>
        public ImageSource Image
        {
            get
            {
                return _image;
            }

            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged("Image");
                }
            }
        }

        /// <summary>
        /// Gets or sets the category of the item.
        /// If used in the grouped grid view samples, the items will be grouped by this property.
        /// </summary>
        public string Category
        {
            get
            {
                return _category;
            }

            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        /// <summary>
        /// The method loads an image into a <see cref="BitmapImage"/> 
        /// and sets the <see cref="Image"/> property. 
        /// </summary>
        /// <param name="baseUri">
        /// The base URI of the image to load.
        /// </param>
        /// <param name="path">
        /// The path of the image to load.
        /// </param>
        public void SetImage(Uri baseUri, string path)
        {
            Image = new BitmapImage(new Uri(baseUri, path));
        }

        /// <summary>
        /// If handlers are subscribed to the event, the handlers will be invoked.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The to string method will be called by the accessibility provider.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> identifying the item.
        /// </returns>
        public override string ToString()
        {
            return Title;
        }
    }
}
