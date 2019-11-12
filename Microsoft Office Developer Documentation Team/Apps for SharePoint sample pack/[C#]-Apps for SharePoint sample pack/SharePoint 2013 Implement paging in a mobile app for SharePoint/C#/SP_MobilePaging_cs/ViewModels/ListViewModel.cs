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
    [KnownType(typeof(ListViewModelBase))]
    [KnownType(typeof(ListDataProvider))]
    [KnownType(typeof(ObservableCollection<DisplayItemViewModel>))]
    public class ListViewModel : ListViewModelBase
    {
        /// <summary>
        /// Makes sure that all the artifacts (eg. ListSchema, DataProvider etc.) required by ViewModel has been initialized properly. 
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads List data for SharePoint View with the specified name
        /// </summary>
        /// <param name="viewName">Name of the SharePoint View which has to be loaded</param>
        public void LoadData(string viewName, params object[] filterParameters)
        {
            IsBusy = true;
            DataProvider.LoadData(viewName, OnLoadViewDataCompleted, filterParameters);
        }

        /// <summary>
        /// Refreshes List data for SharePoint View with the specified name.
        /// </summary>
        /// <param name="viewName">Name of the SharePoint View which has to be loaded</param>
        public void RefreshData(string viewName, params object[] filterParameters)
        {
            IsBusy = true;
            ((ListDataProvider)DataProvider).RefreshData(viewName, OnLoadViewDataCompleted, filterParameters);
        }

        /// <summary>
        /// Code to execute when a SharePoint View has been loaded completely.
        /// </summary>
        /// <param name="e" />
        private void OnLoadViewDataCompleted(LoadViewCompletedEventArgs e)
        {
            IsBusy = false;
            if (e.Error != null)
            {
                OnViewDataLoaded(this, new ViewDataLoadedEventArgs { ViewName = e.ViewName, Error = e.Error });
                return;
            }

            //Create a collection of DisplayItemViewModels
            ObservableCollection<DisplayItemViewModel> displayViewModelCollection = new ObservableCollection<DisplayItemViewModel>();
            foreach (ListItem item in e.Items)
            {
                DisplayItemViewModel displayViewModel = new DisplayItemViewModel { ID = item.Id.ToString(), DataProvider = this.DataProvider };
                displayViewModel.Initialize();

                displayViewModelCollection.Add(displayViewModel);
            }

            OnViewDataLoaded(this, new ViewDataLoadedEventArgs { ViewName = e.ViewName, ViewData = displayViewModelCollection });
        }
    }
}
