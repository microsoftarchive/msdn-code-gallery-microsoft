//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace AppBarControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : Windows.UI.Xaml.Controls.Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        AppBar originalAppBar = null;

        public Scenario4()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Save original AppBar so we can restore it afterward
            originalAppBar = rootPage.BottomAppBar;

            // Use a CommandBar rather than an AppBar so that we get default layout
            CommandBar commandBar = new CommandBar();

            // Create the 'Add' button
            AppBarButton add = new AppBarButton();
            add.Label = "Add";
            add.Icon = new SymbolIcon(Symbol.Add);

            commandBar.PrimaryCommands.Add(add);

            // Create the 'Remove' button
            AppBarButton remove = new AppBarButton();
            remove.Label = "Remove";
            remove.Icon = new SymbolIcon(Symbol.Remove);

            commandBar.PrimaryCommands.Add(remove);

            commandBar.PrimaryCommands.Add(new AppBarSeparator());

            // Create the 'Delete' button
            AppBarButton delete = new AppBarButton();
            delete.Label = "Delete";
            delete.Icon = new SymbolIcon(Symbol.Delete);

            commandBar.PrimaryCommands.Add(delete);

            // Create the 'Camera' button
            AppBarButton camera = new AppBarButton();
            camera.Label = "Camera";
            camera.Icon = new SymbolIcon(Symbol.Camera);
            commandBar.SecondaryCommands.Add(camera);

            // Create the 'Bold' button
            AppBarButton bold = new AppBarButton();
            bold.Label = "Bold";
            bold.Icon = new SymbolIcon(Symbol.Bold);
            commandBar.SecondaryCommands.Add(bold);

            // Create the 'Italic' button
            AppBarButton italic = new AppBarButton();
            italic.Label = "Italic";
            italic.Icon = new SymbolIcon(Symbol.Italic);
            commandBar.SecondaryCommands.Add(italic);

            // Create the 'Underline' button
            AppBarButton underline = new AppBarButton();
            underline.Label = "Underline";
            underline.Icon = new SymbolIcon(Symbol.Underline);
            commandBar.SecondaryCommands.Add(underline);

            // Create the 'Align Left' button
            AppBarButton left = new AppBarButton();
            left.Label = "Align Left";
            left.Icon = new SymbolIcon(Symbol.AlignLeft);
            commandBar.SecondaryCommands.Add(left);

            // Create the 'Align Center' button
            AppBarButton center = new AppBarButton();
            center.Label = "Align Center";
            center.Icon = new SymbolIcon(Symbol.AlignCenter);
            commandBar.SecondaryCommands.Add(center);

            // Create the 'Align Right' button
            AppBarButton right = new AppBarButton();
            right.Label = "Align Right";
            right.Icon = new SymbolIcon(Symbol.AlignRight);
            commandBar.SecondaryCommands.Add(right);

            rootPage.BottomAppBar = commandBar;
            


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.BottomAppBar = originalAppBar;
        }
    }
}
