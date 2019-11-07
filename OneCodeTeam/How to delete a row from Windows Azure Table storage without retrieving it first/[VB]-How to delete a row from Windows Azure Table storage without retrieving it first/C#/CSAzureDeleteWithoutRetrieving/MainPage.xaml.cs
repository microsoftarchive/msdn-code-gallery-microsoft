/***************************** Module Header ******************************\
* Module Name:	MainPage.xaml.cs
* Project:		CSAzureDeleteWithoutRetrieving
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to reduce the request to Azure storage service.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using CSAzureDeleteWithoutRetrieving.DataSource;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CSAzureDeleteWithoutRetrieving
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<DynamicTableEntity> tablelist=new ObservableCollection<DynamicTableEntity>();

        public MainPage()
        {
            this.InitializeComponent();

            Onload();
        }

        private async void Onload()
        {
            tablelist = await TableDataSource.CreateSampleData();
            lstEntities.ItemsSource = tablelist;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var entity = (DynamicTableEntity)lstEntities.SelectedItem;
            if (await TableDataSource.DeleteEntity(entity))
            {
                tablelist.Remove(entity);
                lstEntities.ItemsSource = tablelist;
            }
        }

        private async void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (await TableDataSource.DeleteEntities(tablelist))
            {
                tablelist = null;
                lstEntities.ItemsSource = null;
            }     
        }

        //This will take a long time, because it require to delete the Table first, 
        //then recreate it and add data to it.
        private async void btnRegenerate_Click(object sender, RoutedEventArgs e)
        {
            btnDelete.IsEnabled = false;
            btnDeleteAll.IsEnabled = false;
            btnRegenerate.IsEnabled = false;
            
            if (await TableDataSource.DeleteTable())
            {
                tablelist = await TableDataSource.CreateSampleData();

                lstEntities.ItemsSource = tablelist;
            }
            btnDelete.IsEnabled = true;
            btnDeleteAll.IsEnabled = true;
            btnRegenerate.IsEnabled = true;
        }

       


    }
}
