using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTLE_Explorer.Dictionary
{
    public class ServiceDictionaryEntry
    {
        #region ------------------------ Properties ------------------------
        public string Name;
        public const string SERVICE_MISSING_STRING = "Unknown Service";
        public Guid Uuid;
        public bool IsDefault;
        private bool _changed;
        #endregion // Properties

        public void Initialize(Guid uuid, string name = SERVICE_MISSING_STRING, bool isDefault = false)
        {
            Uuid = uuid;
            Name = name;
            IsDefault = isDefault; 
        }

        public void ChangeFriendlyName(string newName)
        {
            if (IsDefault)
            {
                throw new InvalidOperationException("Cannot change friendly name of a default service.");
            }
            Name = newName;
            _changed = true; 
        }

        public bool HasChanged()
        {
            bool result = _changed;
            _changed = false;
            return result; 
        }

    }
}
