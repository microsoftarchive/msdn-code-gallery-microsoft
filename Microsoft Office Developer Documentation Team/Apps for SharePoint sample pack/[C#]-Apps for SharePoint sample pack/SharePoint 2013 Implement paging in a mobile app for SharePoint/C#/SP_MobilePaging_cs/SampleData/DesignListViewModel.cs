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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Device.Location;
using Microsoft.SharePoint.Phone.Application;
using Microsoft.SharePoint.Client;
using System.Collections.ObjectModel;

namespace SP_MobilePaging_cs
{
    [DataContract]
    public class DesignListViewModel : ListViewModelBase
    {
        /// <summary>
        /// Provides ListView items for List, given its name. Also used for data binding to List form UI
        /// </summary>
        public DesignListViewModel()
        {
            //View1
            this["View1"] = new ObservableCollection<DesignDisplayItemViewModel>
                                                    { 
                                                        new DesignDisplayItemViewModel(), 
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(), 
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(),
                                                    };

            //View2
            this["View2"] = new ObservableCollection<DesignDisplayItemViewModel>
                                                    { 
                                                        new DesignDisplayItemViewModel(), 
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(), 
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(),
                                                    };

            //View3
            this["View3"] = new ObservableCollection<DesignDisplayItemViewModel>
                                                    { 
                                                        new DesignDisplayItemViewModel(), 
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(), 
                                                        new DesignDisplayItemViewModel(),
                                                        new DesignDisplayItemViewModel(),
                                                    };

        }

        /// <summary>
        /// Provides ListView items for List, given its name. Also used for data binding to List form UI
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
