using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

using BTLE_Explorer.Models; 

namespace BTLE_Explorer.ViewModels
{
    // A class that implements INotifyPropertyChanged so that XAML elements can be updated. 
    public abstract class BEGattVMBase<GattObjectType> : INotifyPropertyChanged
    {
        public BEGattModelBase<GattObjectType> Model { get; protected set; }

        #region ----------- Interface with the model (register/unregister) -----------
        public void UnregisterVMFromModel()
        {
            Model.UnregisterVMFromModel(this);
        }
        #endregion 

        #region ----------- Interface with the view (PropertyChanged signaling) ---------
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires a property change notification to the UI thread
        /// </summary>
        /// <param name="name"></param>
        public void SignalChanged(string name)
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
