using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;

using BTLE_Explorer;
using BTLE_Explorer.Models;
using Windows.UI.Xaml;
using System.ComponentModel; 

namespace BTLE_Explorer.ViewModels
{
    /// <summary>
    /// A list containing Bluetooth Characteristics with wrappers to the XAML UI.
    /// </summary>
    public class BECharacteristicListVM : ObservableCollection<BECharacteristicVM>
    {
        public void Initialize(ICollection<BECharacteristicModel> characteristicModels)
        {
            this.Clear();
            foreach (BECharacteristicModel characteristicM in characteristicModels)
            {
                BECharacteristicVM characteristicVM = new BECharacteristicVM();
                characteristicVM.Initialize(characteristicM);
                this.Add(characteristicVM);
            }
        }

        /// <summary>
        /// Unregister all notifiable characteristics in this list
        /// </summary>
        public void Unregister()
        {
            foreach (var VMinstance in this)
            {
                VMinstance.UnregisterVMFromModel();
            }
        }
    }
}