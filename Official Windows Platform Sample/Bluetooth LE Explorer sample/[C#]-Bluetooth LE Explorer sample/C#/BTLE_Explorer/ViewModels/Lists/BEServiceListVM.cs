using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;

using BTLE_Explorer.Models;

namespace BTLE_Explorer.ViewModels
{
    /// <summary>
    /// A list containing Bluetooth Services with wrappers to the XAML UI.
    /// </summary>
    public class BEServiceListVM : ObservableCollection<BEServiceVM>
    {
        public void Initialize(ICollection<BEServiceModel> serviceModels)
        {
            this.Clear();
            foreach (BEServiceModel serviceM in serviceModels)
            {
                BEServiceVM serviceVM = new BEServiceVM();
                serviceVM.Initialize(serviceM);
                this.Add(serviceVM);
            }
        }

        /// <summary>
        /// Unregisters view-models in this from their models.
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