using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input; 
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input; 
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;

using BTLE_Explorer.Models;
using BTLE_Explorer.ViewModels;


namespace BTLE_Explorer
{
    /// <summary>
    /// Page representing all the values in a Gatt characteristic
    /// </summary>
    public sealed partial class CharacteristicInfo : Page
    {

        #region ----------------------- Variables -----------------------
        public BECharacteristicVM CharacteristicVM { get; private set; }
        public BEServiceVM ServiceVM { get; private set; }
        public BEDeviceVM DeviceVM { get; private set; }
        #endregion // Variables

        #region ----------------------- Page Init / Navigation -----------------------
        public CharacteristicInfo()
        {
            this.InitializeComponent();   // default init for page object

            // Handle back button press
            this.Loaded += (sender, e) => { HardwareButtons.BackPressed += OnBackPressed; };
            // De-register back button when the page is no longer visible
            this.Unloaded += (sender, e) => { HardwareButtons.BackPressed -= OnBackPressed; };
            CharacteristicVM = new BECharacteristicVM();
            ServiceVM = new BEServiceVM();
            DeviceVM = new BEDeviceVM();
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                CharacteristicVM.UnregisterVMFromModel();
                this.Frame.GoBack();
                e.Handled = true;

                if (CharacteristicVM.CharacteristicM.DictionaryModelChanged)
                {
                    // This is not critical and doesn't have to block execution, run on a separate thread
                    Utilities.RunFuncAsTask(GlobalSettings.CharacteristicDictionaryUnknown.SaveDictionaryAsync);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            message = "";

            // These must be initialized before the UI loads
            CharacteristicVM.Initialize(GlobalSettings.SelectedCharacteristic);
            ServiceVM.Initialize(GlobalSettings.SelectedCharacteristic.ServiceM);
            DeviceVM.Initialize(GlobalSettings.SelectedCharacteristic.ServiceM.DeviceM);

            // Read the characteristic value on a separate thread
            Utilities.RunFuncAsTask(GlobalSettings.SelectedCharacteristic.ReadValueAsync);
        }
        #endregion // Page Init/Navigation
        
        #region ----------------------- Property Change Functionality -----------------------
        #region change characteristic name 
        private void OnCharacteristicNameChanged(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                TextBox t = (TextBox)sender;
                CharacteristicVM.CharacteristicM.UpdateName(t.Text);

                // Close the keyboard. (Slightly hacky, but works.)
                t.IsEnabled = false;
                t.IsEnabled = true;
            }
        }

        private void OnCharacteristicNameLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            CharacteristicVM.CharacteristicM.UpdateName(t.Text);
        }
        #endregion // Change Characteristic Name

        private void toastButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the registration/unregistration of a toast on a separate thread
            Utilities.RunFuncAsTask(CharacteristicVM.CharacteristicM.ToastClickAsync);
        }


        #region change read format
        private async void hexButton_Click(object sender, RoutedEventArgs e)
        {
            await CharacteristicVM.CharacteristicM.ChangeDisplayTypeAsync(
                Dictionary.CharacteristicDictionaryEntry.ReadUnknownAsEnum.HEX);
        }

        private async void intButton_Click(object sender, RoutedEventArgs e)
        {
            await CharacteristicVM.CharacteristicM.ChangeDisplayTypeAsync(
                Dictionary.CharacteristicDictionaryEntry.ReadUnknownAsEnum.UINT8);
        }
        
        private async void stringButton_Click(object sender, RoutedEventArgs e)
        {
            await CharacteristicVM.CharacteristicM.ChangeDisplayTypeAsync(
                Dictionary.CharacteristicDictionaryEntry.ReadUnknownAsEnum.STRING);
        }
        #endregion // change read format

        #region write to device
        private string message; 
        // Send string to device
        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            // Send new text
            if (message == "")
            {
                return;
            }

            // Write the message on a separate thread
            Utilities.RunFuncAsTask(
                async () => await CharacteristicVM.CharacteristicM.WriteMessageAsync(message));
        }

        /// <summary>
        /// Save our string as we type it. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageCharacterTyped(object sender, KeyRoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            message = t.Text; 

            if (e.Key == VirtualKey.Enter)
            {
                // Close the keyboard. (Slightly hacky, but works.)
                t.IsEnabled = false;
                t.IsEnabled = true;
            }
        }

        private void MessageLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            message = t.Text;
        }
        #endregion // write to device
        #endregion // Property Change Functionality

    }
}
