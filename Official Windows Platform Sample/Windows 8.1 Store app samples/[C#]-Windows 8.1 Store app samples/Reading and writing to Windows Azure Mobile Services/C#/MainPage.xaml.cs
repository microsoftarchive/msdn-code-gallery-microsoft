//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Runtime.Serialization;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using System.Net.Http;

namespace AzureMobileStorage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        public MainPage()
        {
            this.InitializeComponent();

            // Populate the sample title from the constant in the Constants.cs file.
            SetFeatureName(FEATURE_NAME);
        }

        private async void ShowError()
        {
            MessageDialog dialog = new MessageDialog("Correctly configure the Mobile Service url and key in the sample.", "Configuration error");
            await dialog.ShowAsync();
        }

        private async void InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            try
            {
                await todoTable.InsertAsync(todoItem);
                items.Add(todoItem);
            }
            catch (HttpRequestException)
            {
                ShowError();
            }
        }

        private async void RefreshTodoItems()
        {
            try
            {
                items = await todoTable
                   .Where(todoItem => todoItem.Complete == false).ToCollectionAsync();
                ListItems.ItemsSource = items;
            }
            catch (HttpRequestException)
            {
                ShowError();
            }
        }

        private async void UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
            // responds, the item is removed from the list.
            await todoTable.UpdateAsync(item);
            RefreshTodoItems();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTodoItems();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Text = TextInput.Text, CreatedAt = DateTime.Now };
            InsertTodoItem(todoItem);
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            UpdateCheckedTodoItem(item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshTodoItems();
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void SetFeatureName(string str)
        {
            FeatureName.Text = str;
        }
    }

    [DataContract (Name = "TodoItem")]
    public class TodoItem
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public bool Complete { get; set; }

        [DataMember]
        public DateTime? CreatedAt { get; set; }
    }
}
