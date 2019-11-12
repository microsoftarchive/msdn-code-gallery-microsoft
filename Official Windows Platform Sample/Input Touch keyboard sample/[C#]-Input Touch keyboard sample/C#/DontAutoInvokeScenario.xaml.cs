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

namespace TouchKeyboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DontAutoInvokeScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DontAutoInvokeScenario()
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
        }

        /// <summary>
        /// Invoked when the button is clicked, and used to direct programmatic focus to the field. We had to work around a
        /// problem with focus in order to make this function work. The behavior of the On Screen Keyboard is to appear on
        /// programmatic focus depending on the value of the PreventKeyboardDisplayOnProgrammaticFocus property. However, after
        /// a manual dismiss, this behavior is overridden, and we will not display unless we perform an explicit invocation of
        /// the keyboard, for instance, by tapping on a field.
        /// 
        /// The only type of dismiss that will allow us to trigger this property is a light dismiss, but we have a problem at
        /// the container level, where tapping outside of the field will redirect the focus to the scenario chooser, which is
        /// part of a group of controls that do not dismiss the keyboard when focus goes towards it. Therefore, we have had to
        /// modify the ScrollViewer container to set its property IsTabStop to true. This makes the container focusable and works
        /// around the problem with the focus in the Scenarios.
        /// 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event data that describes the click action on the button.</param>
        public void onFocusClicked(object sender, RoutedEventArgs e)
        {
            textBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        /// <summary>
        /// Invoked when the checkbox gets checked, and used to disable the IHM invocation when a field gets programmatic focus.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event data that describes the toggle state of the checkbox.</param>
        private void onAutoInvokedChecked(object sender, RoutedEventArgs e)
        {
            textBox.PreventKeyboardDisplayOnProgrammaticFocus = true;
        }

        /// <summary>
        /// Invoked when the checkbox gets unchecked, and used to enable the IHM invocation when a field gets programmatic focus.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event data that describes the toggle state of the checkbox.</param>
        private void onAutoInvokedUnchecked(object sender, RoutedEventArgs e)
        {
            textBox.PreventKeyboardDisplayOnProgrammaticFocus = false;
        }

    }
}
