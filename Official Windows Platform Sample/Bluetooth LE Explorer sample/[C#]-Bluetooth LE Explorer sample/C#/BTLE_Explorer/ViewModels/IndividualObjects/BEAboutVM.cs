using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.ApplicationModel.Core;

using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;

using BTLE_Explorer.Models;

namespace BTLE_Explorer.ViewModels
{
    /// <summary>
    /// View model of the About page
    /// </summary>
    public class BEAboutVM : INotifyPropertyChanged
    {
        #region Properties
        public Visibility CustomNamesClearedVisibility
        {
            get
            {
                if (GlobalSettings.DictionariesCleared)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public bool UseCachedModeChecked
        {
            get
            {
                return GlobalSettings.UseCachedMode;
            }
        }
        #endregion

        public void Initialize()
        {
        }

        /// <summary>
        /// Sets the bluetooth cached mode for the entire app
        /// </summary>
        /// <param name="value"></param>
        public void UseCachedMode(bool value)
        {
            GlobalSettings.UseCachedMode = value;
            SignalChanged("UseCachedModeChecked");
        }

        /// <summary>
        /// Clears out our dictionaries of custom service/characteristic names
        /// </summary>
        /// <returns></returns>
        public async Task ClearDictionariesAsync()
        {
            // Because we aren't cleaning up all the view models, the UI will be out of date.
            // Tell the user to restart the app to complete the clearing of dictionaries.
            await GlobalSettings.ClearDictionariesAsync();
            
            // Fire change notifications
            SignalChanged("CustomNamesClearedVisibility");
        }

        #region ----------- Interface with the view (PropertyChanged signaling) ---------
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Fires a property change notification to the UI thread
        /// </summary>
        /// <param name="name"></param>
        private void SignalChanged(string name)
        {
            if (PropertyChanged != null)
            {
                // Make sure that the property change runs in the UI thread
                // If this is called from an MTA, you will get an RPC_E_WRONG_THREAD exception
                Utilities.RunActionOnUiThread(
                    () =>
                    {
                        var handler = PropertyChanged;
                        if (handler != null)
                        {
                            handler(this, new PropertyChangedEventArgs(name));
                        }
                    });
            }
        }
        #endregion
    }

}