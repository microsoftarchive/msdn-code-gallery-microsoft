// Copyright (c) Microsoft Corporation. All rights reserved

using Doto.Model;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;

namespace Doto
{
    /// <summary>
    /// ViewModel that drives the majority of the logic behind the MainPage in Doto
    /// including Login, and Registration.
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private SynchronizationContext synchronizationContext;
        private IMobileServiceTable<Setting> settingsTable;
        private IMobileServiceTable<Invite> invitesTable;
        private IMobileServiceTable<ListMember> listMembersTable;
        private IMobileServiceTable<Device> devicesTable;
        private IMobileServiceTable<Profile> profilesTable;
        private IMobileServiceTable<Item> itemsTable;
        private List<Item> selectedItems;
        private IFlyoutProvider flyoutProvider;
        
        // Doto doesn't support pagination of list items, instead there is an explicit maximum of 
        // 250 items in a list that can be viewed.
        private const int maxListSize = 250;
        
        /// <summary>
        /// Loads the changeable application settings (accent color and background image)
        /// from Windows Azure Mobile Service
        /// </summary>
        private async void LoadSettingsAsync()
        {
            List<Setting> settings = null;
            try
            {
                settings = await settingsTable.ToListAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                HandleError(ex);
            }
            catch (HttpRequestException ex1)
            {
                // If the sample hasn't been configured properly and the mobile service url and key isn't set in App.xaml.cs 
                //  then HttpRequestException will be thrown.
                ShowError(ex1.Message);
            }

            if (settings == null || settings.Count == 0)
            {
                // no settings - default color. 
                AccentColor = "#63a9eb";
                return;
            }
            AccentColor = settings.Single(s => s.Key == "accentColor").Value;
            BackgroundImage = settings.Single(s => s.Key == "backgroundImage").Value;
        }

        /// <summary>
        /// Sets up the execution logic for all the DelegateCommands in the ViewModel
        /// </summary>
        private void SetupCommands()
        {
            // Register user when they login the very first time. 
            RegisterCommand = new DelegateCommand(async () =>
            {
                // Show validation fail flyout
                if (string.IsNullOrWhiteSpace(User.Name) ||
                    string.IsNullOrWhiteSpace(User.City) ||
                    string.IsNullOrWhiteSpace(User.State))
                {
                    flyoutProvider.ShowRegisterValidationFlyout();
                    return;
                }
                await profilesTable.InsertAsync(User);
                DisplayRegistrationForm = false;
                RegisterDeviceAsync();
                LoadListsAsync(true);
                LoadInvitesAsync();
            }, true);

            // Shows the flyout for user to create a new list the first time 
            ShowNewUserNewListCommand = new DelegateCommand(() =>
            {
                flyoutProvider.ShowNewUserNewListFlyout();
            }, false);

            // Shows the flyout for user to enter the new list name
            ShowNewListCommand = new DelegateCommand(() =>
            {
                flyoutProvider.ShowNewListFlyout();
            }, false);

            // Creates a new list in the backend with the user as the owner
            NewListCommand = new DelegateCommand(async () =>
            {
                if (String.IsNullOrWhiteSpace(NewListName))
                {
                    flyoutProvider.ShowNameRequiredFlyout();
                    return;
                }
                ListMember membership = new ListMember
                {
                    Name = NewListName,
                    UserId = App.MobileService.CurrentUser.UserId,
                };
                NewListName = String.Empty;
                await listMembersTable.InsertAsync(membership);
                Lists.Add(membership);
                ShowList(Lists.IndexOf(membership));
                flyoutProvider.HideNewListFlyout();
            });

            // Shows the flyout with all the lists having user as the owner
            ShowChooseListCommand = new DelegateCommand(() =>
            {
                flyoutProvider.ShowChooseListFlyout();
            });

            // Opens the list as selected by the user
            SelectListCommand = new DelegateCommand<ListMember>(listMember =>
            {
                ShowList(Lists.IndexOf(listMember));
            });

            // Asks the user for confirmation before leaving list
            ConfirmLeaveListCommand = new DelegateCommand(() =>
            {
                flyoutProvider.ShowLeaveListConfirmationFlyout();
            });

            // Removes the user as the owner of the list
            LeaveListCommand = new DelegateCommand(async () =>
            {
                await listMembersTable.DeleteAsync(CurrentList);
                Lists.Remove(CurrentList);
                ShowList(0);
                flyoutProvider.HideLeaveListConfirmationFlyout();
            });

            // Shows the floyout where the user can add the name of an item in a list 
            ShowAddItemCommand = new DelegateCommand(() =>
            {
                flyoutProvider.ShowAddItemFlyout();
            }, false);

            // Adds an item in the list in the backend 
            AddItemCommand = new DelegateCommand(async () =>
            {
                if (String.IsNullOrWhiteSpace(NewItemText))
                {
                    flyoutProvider.ShowDescriptionRequiredFlyout();
                    return;
                }
                var item = new Item
                {
                    Text = NewItemText,
                    ListId = CurrentList.ListId,
                    CreatedBy = User.Name,
                };
                NewItemText = String.Empty;
                await itemsTable.InsertAsync(item);
                Items.Add(item);
            });

            // Removes an item from a list when Remove Items button is pressed
            RemoveItemsCommand = new DelegateCommand(async () =>
            {
                foreach (Item item in selectedItems)
                {
                    try
                    {
                        await itemsTable.DeleteAsync(item);
                    }
                    catch (MobileServiceInvalidOperationException exc)
                    {
                        // a 404 is an expected scenario here, another user 
                        // has most likely deleted the item whilst we've been
                        // viewing. 
                        if (exc.Response.StatusCode != HttpStatusCode.NotFound)
                        {
                            ShowError("Another user deleted the item already.");
                        }
                    }
                    Items.Remove(item);
                }
            }, false);

            // Refreshes a list when Refresh Items button is pressed
            // this will populate any items added by any other list owner elsewhere
            RefreshCommand = new DelegateCommand(() =>
            {
                LoadSettingsAsync();
                LoadInvitesAsync();
                if (CurrentList != null)
                {
                    LoadListsAsync();
                    LoadItemsAsync();
                }
            }, false);

            // Searches for a user in the backend
            ShowUserSearchCommand = new DelegateCommand(() =>
            {
                var inviteUserViewModel = new InviteUserViewModel(this, profilesTable);
                flyoutProvider.ShowInviteUserFlyout(inviteUserViewModel);
            }, false);

            // Shows all the available invites for the user
            ViewInvitesCommand = new DelegateCommand(() =>
            {
                var viewInvitesViewModel = new ViewInvitesViewModel(this, invitesTable);
                flyoutProvider.ShowViewInvitesFlyout(viewInvitesViewModel);
            }, false);
        }

        /// <summary>
        /// Performs login via Windows Azure Mobile Service
        /// </summary>
        private async Task LoginAsync()
        {
            // Force the user to login on app startup
            // You can also use the Live Connect SDK to implement single sign-on - http://go.microsoft.com/fwlink/?LinkID=304139&clcid=0x409
            await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);

            // If you see the error message - "We can't connect to the service you need right now. Check your network connection or try this again later"
            // please ensure that you have setup the app correctly using the ReadMe.txt with the sample. 

            List<Profile> profiles = new List<Profile>();
            try
            {
                profiles = await profilesTable.Where(p => p.UserId == App.MobileService.CurrentUser.UserId).ToListAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                HandleError(ex);
            }

            if (profiles.Count == 0)
            {
                RegisterUser(App.MobileService.CurrentUser.UserId);
            }
            else
            {
                User = profiles.First();
                RegisterDeviceAsync();
                LoadListsAsync(true);
                LoadInvitesAsync();
            }

            return;
        }

        /// <summary>
        /// Loads the invites from Windows Azure Mobile Service invites table
        /// </summary>
        private async void LoadInvitesAsync()
        {
            IEnumerable<Invite> invites = await invitesTable.Where(i => i.ToUserId == User.UserId).ToEnumerableAsync();

            Invites.Clear();
            Invites.AddRange(invites);

            IsPendingInvitesButtonVisible = ViewInvitesCommand.IsEnabled = Invites.Count > 0;
        }

        /// <summary>
        /// Loads Items for the current list from the Windows Azure Mobile Service items table
        /// </summary>
        private async void LoadItemsAsync()
        {
            List<Item> newItems = new List<Item>();
            try
            {
                newItems = await itemsTable.Where(i => i.ListId == CurrentList.ListId).Take(maxListSize).ToListAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                HandleError(ex);
            }

            // Perform a simple sync of the existing list using the Ids.
            Queue<Item> matched = new Queue<Item>();
            Queue<Item> removals = new Queue<Item>();

            foreach (var item in Items)
            {
                bool didMatch = false;

                foreach (var newItem in newItems)
                {
                    if (newItem.Id == item.Id)
                    {
                        matched.Enqueue(newItem);
                        didMatch = true;
                        break;
                    }
                }

                // If no match, we should remove from Items collection
                if (!didMatch)
                {
                    removals.Enqueue(item);
                }

                // Remove new items as quickly as soon as they're matched 
                while (matched.Count > 0)
                {
                    newItems.Remove(matched.Dequeue());
                }
            }

            while (removals.Count > 0)
            {
                Items.Remove(removals.Dequeue());
            }

            // Add any remaining newItems - they must be genuinely new items
            Items.AddRange(newItems);
        }

        private void HandleError(MobileServiceInvalidOperationException ex)
        {
            if (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // Mobile services table itself does not exist. Unrecoverable error. 
                // Customer can't do anything about it. 
                // App developer must make sure that the backend tables have been created 
                ShowError("An unexpected error occurred.");
            }
            else
            {
                ShowError("An unexpected error occurred. Please try again later.");
            } 
        }

        /// <summary>
        /// Registers the user's profile with Windows Azure Mobile Service
        /// </summary>
        /// <param name="userId"></param>
        private void RegisterUser(string userId)
        {
            User = new Profile
            {
                Name = String.Empty,
                UserId = userId,
                City = String.Empty,
                State = String.Empty,
                ImageUri = String.Empty
            };
            DisplayRegistrationForm = true;
        }

        /// <summary>
        /// Requests a push channel URI and uploads this to Windows Azure Mobile Service
        /// devices table
        /// </summary>
        private async void RegisterDeviceAsync()
        {
            PushNotificationChannel channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += channel_PushNotificationReceived;

            string installationId = InstallationId.GetInstallationIdAsync();

            // The server side script devices.insert.js will ensure that it creates a new row only when a new User + Device combination is getting added. 
            await devicesTable.InsertAsync(
                    new Device
                    {
                        UserId = User.UserId,
                        ChannelUri = channel.Uri.ToString(),
                        InstallationId = installationId
                    });
        }

        /// <summary>
        /// Sets the view into a particular state, enabling and disabling all controls
        /// based on that state.
        /// </summary>
        private void SetViewMode(ViewMode viewMode)
        {
            switch (viewMode)
            {
                case ViewMode.LoggedOut:
                    synchronizationContext.Post(async x =>
                    {
                        Reset();
                        ViewInvitesCommand.IsEnabled = false;
                        RemoveItemsCommand.IsEnabled = false;
                        ShowUserSearchCommand.IsEnabled = false;
                        ShowNewListCommand.IsEnabled = false;
                        ShowNewUserNewListCommand.IsEnabled = false;
                        ShowAddItemCommand.IsEnabled = false;
                        ConfirmLeaveListCommand.IsEnabled = false;
                        ConfirmLeaveListCommand.IsEnabled = false;
                        RefreshCommand.IsEnabled = false;
                        await LoginAsync();
                    }, null);
                    break;
                
                case ViewMode.NoLists:
                    ShowNewListCommand.IsEnabled = true;
                    ShowNewUserNewListCommand.IsEnabled = true;
                    ShowAddItemCommand.IsEnabled = false;
                    ViewInvitesCommand.IsEnabled = false;
                    RemoveItemsCommand.IsEnabled = false;
                    ShowUserSearchCommand.IsEnabled = false;
                    ConfirmLeaveListCommand.IsEnabled = false;
                    ConfirmLeaveListCommand.IsEnabled = false;
                    RefreshCommand.IsEnabled = true;
                    break;
                
                case ViewMode.ListSelected:
                    ShowNewListCommand.IsEnabled = true;
                    ShowAddItemCommand.IsEnabled = true;
                    RefreshCommand.IsEnabled = true;
                    ShowUserSearchCommand.IsEnabled = true;
                    ConfirmLeaveListCommand.IsEnabled = true;
                    ConfirmLeaveListCommand.IsEnabled = true;
                    ViewInvitesCommand.IsEnabled = false;
                    break;
            }
            ViewMode = viewMode;
        }

        public DelegateCommand AddItemCommand { get; private set; }
        public DelegateCommand ShowAddItemCommand { get; private set; }
        public DelegateCommand NewListCommand { get; private set; }
        public DelegateCommand ShowNewListCommand { get; private set; }
        public DelegateCommand ShowNewUserNewListCommand { get; private set; }
        public DelegateCommand ShowChooseListCommand { get; private set; }
        public DelegateCommand ViewInvitesCommand { get; private set; }
        public DelegateCommand ShowUserSearchCommand { get; private set; }
        public DelegateCommand RemoveItemsCommand { get; private set; }
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand ConfirmLeaveListCommand { get; private set; }
        public DelegateCommand LeaveListCommand { get; private set; }
        public DelegateCommand<ListMember> SelectListCommand { get; private set; }

        public MainViewModel(IFlyoutProvider flyoutProviderArgument, SynchronizationContext synchonizationContextArgument)
        {
            flyoutProvider = flyoutProviderArgument;
            synchronizationContext = synchonizationContextArgument;
            invitesTable = App.MobileService.GetTable<Invite>();
            itemsTable = App.MobileService.GetTable<Item>();
            profilesTable = App.MobileService.GetTable<Profile>();
            listMembersTable = App.MobileService.GetTable<ListMember>();
            devicesTable = App.MobileService.GetTable<Device>();
            settingsTable = App.MobileService.GetTable<Setting>();

            SetupCommands();
            LoadSettingsAsync();
        }
        
        /// <summary>
        /// Kickstarts the ViewModel into life
        /// </summary>
        public async void Initialize()
        {
            await LoginAsync();
        }

        /// <summary>
        /// Clears all lists, sets the current list and user to null 
        /// </summary>
        public void Reset()
        {
            Lists.Clear();
            Items.Clear();
            Invites.Clear();
            CurrentList = null;
            User = null;
        }
       
        /// <summary>
        /// Load all the user's lists
        /// </summary>
        /// <param name="showFirstList">An optional parameter that can force the method to load the 0th list as the current</param>
        public async void LoadListsAsync(bool showFirstList = false)
        {
            IEnumerable<ListMember> lists = await listMembersTable.Where(m => m.UserId == User.UserId).ToEnumerableAsync();
            Lists.Clear();
            Lists.AddRange(lists);
            if (showFirstList)
            {
                ShowList(0);
            }
        }

        /// <summary>
        /// Sets the current list to specified index. If there are no lists, switches ViewModel
        /// to the NoLists ViewMode
        /// </summary>
        public void ShowList(int activeListIndex)
        {
            if (Lists.Count == 0)
            {
                CurrentList = null;
                SetViewMode(ViewMode.NoLists);
                IsDropdownListVisible = false;
            }
            else
            {
                SetViewMode(ViewMode.ListSelected);
                CurrentList = Lists[activeListIndex];
                LoadItemsAsync();
                IsDropdownListVisible = true;
                flyoutProvider.HideChooseListFlyout();
            }
        }

        /// <summary>
        /// Handles a push notification being received while the app is running. 
        /// An invite is received with a toast notification and a new item is received with a tile notification
        /// </summary>
        void channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            synchronizationContext.Post(x =>
            {
                if (args.NotificationType == PushNotificationType.Toast)
                {
                    // We received a toast notification - let's check for invites
                    LoadInvitesAsync();
                }
                else if (args.NotificationType == PushNotificationType.Tile)
                {
                    // We received a tile - reload current items in case it was for this list
                    LoadItemsAsync();
                }
            }, null);
        }

        private readonly ObservableCollection<Item> items = new ObservableCollection<Item>();

        public ObservableCollection<Item> Items
        {
            get { return items; }
        }

        private readonly ObservableCollection<Invite> invites = new ObservableCollection<Invite>();

        public ObservableCollection<Invite> Invites
        {
            get { return invites; }
        }

        private readonly ObservableCollection<ListMember> lists = new ObservableCollection<ListMember>();

        public ObservableCollection<ListMember> Lists
        {
            get { return lists; }
        }
                
        public IFlyoutProvider FlyoutProvider
        {
            get { return flyoutProvider; }
        }

        private Profile user;

        public Profile User
        {
            get { return user; }
            set { SetValue(ref user, value); }
        }

        private string accentColor;

        public string AccentColor
        {
            get { return accentColor; }
            set { SetValue(ref accentColor, value); }
        }

        private string backgroundImage;

        public string BackgroundImage
        {
            get { return backgroundImage; }
            set { SetValue(ref backgroundImage, value); }
        }

        private string newItemText;

        public string NewItemText
        {
            get { return newItemText; }
            set { SetValue(ref newItemText, value); }
        }

        private string newListName;

        public string NewListName
        {
            get { return newListName; }
            set { SetValue(ref newListName, value); }
        }

        private bool isRemoveItemsButtonVisible = false;

        public bool IsRemoveItemsButtonVisible
        {
            get { return isRemoveItemsButtonVisible; }
            set { SetValue(ref isRemoveItemsButtonVisible, value); }
        }

        private bool isPendingInvitesButtonVisible = false;

        public bool IsPendingInvitesButtonVisible
        {
            get { return isPendingInvitesButtonVisible; }
            set { SetValue(ref isPendingInvitesButtonVisible, value); }
        }

        private bool isDropdownListVisible = false;

        public bool IsDropdownListVisible
        {
            get { return isDropdownListVisible; }
            set { SetValue(ref isDropdownListVisible, value); }
        }
        
        private ListMember currentList;

        public ListMember CurrentList
        {
            get { return currentList; }
            set { SetValue(ref currentList, value); }
        }

        private ViewMode viewMode;

        public ViewMode ViewMode
        {
            get { return viewMode; }
            set { SetValue(ref viewMode, value); }
        }

        private bool displayRegistrationForm = false;

        public bool DisplayRegistrationForm
        {
            get { return displayRegistrationForm; }
            set { SetValue(ref displayRegistrationForm, value); }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetValue(ref isBusy, value); }
        }

        /// <summary>
        /// Returns a list of SimpleCommands based on the app's state. This is used
        /// to populate the Settings panel.
        /// </summary>
        public List<SimpleCommand> GetSettingsCommands()
        {
            List<SimpleCommand> commands = new List<SimpleCommand>();

            // If no user, no need to return any commands
            if (App.MobileService.CurrentUser == null)
            {
                return commands;
            }

            // Create the sign out command
            SimpleCommand signOut = new SimpleCommand();
            signOut.Text = string.Format("Sign Out {0}", User.Name);
            signOut.Action = () =>
            {
                App.MobileService.Logout();
                SetViewMode(ViewMode.LoggedOut);
            };
            commands.Add(signOut);
            return commands;
        }

        /// <summary>
        /// Allows the View to communicate with the ViewModel when 
        /// items are selected (as binding is not supported for this)
        /// </summary>
        public void SetSelectedItems(List<Item> selectedItemsList)
        {
            selectedItems = selectedItemsList;
            IsRemoveItemsButtonVisible = RemoveItemsCommand.IsEnabled = selectedItems.Count > 0;
        }

        /// <summary>
        /// Invites a user to join the current list
        /// </summary>
        public async Task InviteUserAsync(Profile user)
        {
            await invitesTable.InsertAsync(new Invite()
            {
                FromUserId = User.UserId,
                FromUserName = User.Name,
                FromImageUri = User.ImageUri,
                ToUserId = user.UserId,
                ToUserName = user.Name,
                ListId = CurrentList.ListId,
                ListName = CurrentList.Name
            });

            return;
        }

        /// <summary>
        /// Displays a simple 'Error' dialog to the user
        /// </summary>
        public void ShowError(string message)
        {
            synchronizationContext.Post(x =>
            {
                // Removing quotes per UX guidance
                flyoutProvider.ShowDialogAsync("Error", message.Replace("\"",""));
            }, null);
        }
    }

    public enum ViewMode
    {
        LoggedOut,
        NoLists,
        ListSelected
    }
}