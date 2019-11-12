//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FolderEnumeration
{
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario2()
        {
            this.InitializeComponent();
            GroupByMonthButton.Click += new RoutedEventHandler(GroupByMonth_Click);
            GroupByRatingButton.Click += new RoutedEventHandler(GroupByRating_Click);
            GroupByTagButton.Click += new RoutedEventHandler(GroupByTag_Click);
        }

        /// <summary>
        /// list all the files and folders in Pictures library by month
        /// </summary>
        async void GroupByMonth_Click(object sender, RoutedEventArgs e)
        {
            await GroupByHelperAsync(new QueryOptions(CommonFolderQuery.GroupByMonth));
        }

        /// <summary>
        /// list all the files and folders in Pictures library by rating
        /// </summary>
        async void GroupByRating_Click(object sender, RoutedEventArgs e)
        {
            await GroupByHelperAsync(new QueryOptions(CommonFolderQuery.GroupByRating));
        }

        /// <summary>
        /// list all the files and folders in Pictures library by tag
        /// </summary>
        async void GroupByTag_Click(object sender, RoutedEventArgs e)
        {
            await GroupByHelperAsync(new QueryOptions(CommonFolderQuery.GroupByTag));
        }

        /// <summary>
        /// helper for all list by functions
        /// </summary>
        async Task GroupByHelperAsync(QueryOptions queryOptions)
        {
            OutputPanel.Children.Clear();

            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;
            StorageFolderQueryResult queryResult = picturesFolder.CreateFolderQueryWithOptions(queryOptions);

            IReadOnlyList<StorageFolder> folderList = await queryResult.GetFoldersAsync();
            foreach (StorageFolder folder in folderList)
            {
                IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
                OutputPanel.Children.Add(CreateHeaderTextBlock(folder.Name + " (" + fileList.Count + ")"));
                foreach (StorageFile file in fileList)
                {
                    OutputPanel.Children.Add(CreateLineItemTextBlock(file.Name));
                }
            }
        }

        TextBlock CreateHeaderTextBlock(string contents)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = contents;
            textBlock.Style = (Style)Application.Current.Resources["H2Style"];
            textBlock.TextWrapping = TextWrapping.Wrap;
            return textBlock;
        }

        TextBlock CreateLineItemTextBlock(string contents)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = contents;
            textBlock.Style = (Style)Application.Current.Resources["BasicTextStyle"];
            textBlock.TextWrapping = TextWrapping.Wrap;
            Thickness margin = textBlock.Margin;
            margin.Left = 20;
            textBlock.Margin = margin;
            return textBlock;
        }
    }
}
