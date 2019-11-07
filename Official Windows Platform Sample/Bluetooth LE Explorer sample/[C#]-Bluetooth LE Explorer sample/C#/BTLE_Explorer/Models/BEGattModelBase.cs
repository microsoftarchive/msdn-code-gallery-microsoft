using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTLE_Explorer.ViewModels; 

namespace BTLE_Explorer.Models
{
    /// <summary>
    /// A base class containing functions common to all Gatt-object wrappers acting as models.
    /// 
    /// For a child model class of BEGattModelBase, BEGattVMBase<BEGattModelBase<GattObjectType>> 
    /// is the view-model class associated with the model.
    /// 
    /// An example:
    /// BEDeviceModel inherits from BEGattModelBase<BluetoothGattDevice>
    /// BEDeviceVM would be in the ViewModelInstances list. 
    /// </summary>
    /// <typeparam name="GattObjectType"></typeparam>
    public class BEGattModelBase<GattObjectType>
    {
        #region --------------------- Manipulate view-models dependent on this model ---------------------
        protected List<BEGattVMBase<GattObjectType>> _viewModelInstances;

        /// <summary>
        /// Registers the view model with the model, so that the model can fire change notifications
        /// </summary>
        /// <param name="vm"></param>
        public void Register(BEGattVMBase<GattObjectType> vm)
        {
            lock (_viewModelInstances)
            {
                if (vm == null)
                {
                    throw new ArgumentNullException("Tried to register a null-valued view-model to a model.");
                }
                _viewModelInstances.Add(vm);
            }
        }

        /// <summary>
        /// Unregisters the view model from the model
        /// </summary>
        /// <param name="vm"></param>
        public void UnregisterVMFromModel(BEGattVMBase<GattObjectType> vm)
        {
            lock (_viewModelInstances)
            {
                if (vm == null)
                {
                    throw new ArgumentNullException("Tried to remove a null-valued view-model from a model");
                }
                if (_viewModelInstances.Contains(vm))
                {
                    _viewModelInstances.Remove(vm);
                }
            }
        }
        
        /// <summary>
        /// Iterates through all view models tracked by this model and fires the
        /// PropertyChanged event handler on them.
        /// </summary>
        /// <param name="property"></param>
        protected void SignalChanged(string property)
        {
            BEGattVMBase<GattObjectType>[] viewModels;
            lock (_viewModelInstances)
            {
                viewModels = _viewModelInstances.ToArray();
            }

            // Call each VM outside the lock, as they can signal a change to the UI, resulting
            // in a deadlock if the UI thread callback is trying to Register or Unregister a
            // VM above
            foreach (BEGattVMBase<GattObjectType> vm in viewModels)
            {
                vm.SignalChanged(property);
            }
        }
        #endregion
    }
}
