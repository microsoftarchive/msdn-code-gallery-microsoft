//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Common;
using AppUIBasics.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GridViewPage : ControlPage
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public GridViewPage()
        {
            this.InitializeComponent();
        }        
        
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var _controlInfoDataGroups = await ControlInfoDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = _controlInfoDataGroups;
        }

        private void IconTextButton_Click(object sender, RoutedEventArgs e)
        {
            Control1.ItemTemplate = (DataTemplate)this.Resources["IconTextTemplate"];
        }

        private void ImageOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            Control1.ItemTemplate = (DataTemplate)this.Resources["ImageOverlayTemplate"];
        }

        private void ImageTextButton_Click(object sender, RoutedEventArgs e)
        {
            Control1.ItemTemplate = (DataTemplate)this.Resources["ImageTextTemplate"];
        }

        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            Control1.ItemTemplate = (DataTemplate)this.Resources["TextTemplate"];
        }

        private void Control1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridView = sender as GridView;
            if (gridView != null)
            {
                SelectionOutput.Text = string.Format("You have selected {0} item(s).", gridView.SelectedItems.Count);             
            }      
        }

        private void Control1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickOutput.Text = "You clicked " + e.ClickedItem.ToString() + ".";
        }

        private void ItemClickCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ClickOutput.Text = string.Empty;
        }

        private void FlowDirectionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (Control1.FlowDirection == FlowDirection.LeftToRight)
            {
                Control1.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                Control1.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        private void SelectionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Control1 != null)
            {
                string colorName = e.AddedItems[0].ToString();
                switch (colorName)
                {
                    case "None":
                        Control1.SelectionMode = ListViewSelectionMode.None;
                        SelectionOutput.Text = string.Empty;
                        break;
                    case "Single":
                        Control1.SelectionMode = ListViewSelectionMode.Single;
                        break;
                    case "Multiple":
                        Control1.SelectionMode = ListViewSelectionMode.Multiple;
                        break;
                    case "Extended":
                        Control1.SelectionMode = ListViewSelectionMode.Extended;
                        break;
                }
            }
        }
    }
}
