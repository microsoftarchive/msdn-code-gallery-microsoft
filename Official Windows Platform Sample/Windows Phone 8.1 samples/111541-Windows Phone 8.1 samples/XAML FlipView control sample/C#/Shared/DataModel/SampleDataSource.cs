using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Controls_FlipView.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : SDKTemplate.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String title, String type, String picture)
        {
            this._title = title;
            this._type = type;
            this._picture = picture;
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _type = string.Empty;
        public string Type
        {
            get { return this._type; }
            set { this.SetProperty(ref this._type, value); }
        }

        private Uri _image = null;
        private String _picture = null;
        public Uri Image
        {
            get
            {
                return new Uri(SampleDataCommon._baseUri, this._picture);
            }

            set
            {
                this._picture = null;
                this.SetProperty(ref this._image, value);
            }
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String title, String type, String picture)
            : base(title, type, picture)
        {
        }

    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class SampleDataSource
    {
        private ObservableCollection<object> _items = new ObservableCollection<object>();
        public ObservableCollection<object> Items
        {
            get { return this._items; }
        }

        public SampleDataSource()
        {
            Items.Add(new SampleDataItem("Cliff",
                    "item",
                    "Assets/Cliff.jpg"
                    ));
            Items.Add(new SampleDataItem("Grapes",
                    "item",
                    "Assets/Grapes.jpg"
                    ));
            Items.Add(new SampleDataItem("Rainier",
                    "item",
                    "Assets/Rainier.jpg"
                    ));
            Items.Add(new SampleDataItem("Sunset",
                    "item",
                    "Assets/Sunset.jpg"
                    ));
            Items.Add(new SampleDataItem("Valley",
                    "item",
                    "Assets/Valley.jpg"
                    ));
        }
    }
}
