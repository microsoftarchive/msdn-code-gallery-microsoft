using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace Shared_Library
{

    public class AppSettings
    {
        // Our isolated storage settings 
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings 
        const string AuthTokenSettingKeyName = "AuthTokenSetting";
        const string UserIdSettingKeyName = "AuthTokenSetting";

        // The default value of our settings 
        const string AuthTokenSettingDefault = "";
        const string UserIdSettingDefault = "";


        /// <summary> 
        /// Constructor that gets the application settings. 
        /// </summary> 
        public AppSettings()
        {
            try
            {
                // Get the settings for this application. 
                settings = IsolatedStorageSettings.ApplicationSettings;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }
        }

        /// <summary> 
        /// Update a setting value for our application. If the setting does not 
        /// exist, then add the setting. 
        /// </summary> 
        /// <param name="Key"></param> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists 
            if (settings.Contains(Key))
            {
                // If the value has changed 
                if (settings[Key] != value)
                {
                    // Store the new value 
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key. 
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }


        /// <summary> 
        /// Get the current value of the setting, or if it is not found, set the  
        /// setting to the default setting. 
        /// </summary> 
        /// <typeparam name="valueType"></typeparam> 
        /// <param name="Key"></param> 
        /// <param name="defaultValue"></param> 
        /// <returns></returns> 
        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value. 
            if (settings.Contains(Key))
            {
                value = (valueType)settings[Key];
            }
            // Otherwise, use the default value. 
            else
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary> 
        /// Save the settings. 
        /// </summary> 
        public void Save()
        {
            settings.Save();
        }

        /// <summary> 
        /// Property to get and set the AuthToken Setting. 
        /// </summary> 
        public string AuthTokenSetting
        {
            get
            {
                return GetValueOrDefault<string>(AuthTokenSettingKeyName, AuthTokenSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(AuthTokenSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        /// <summary> 
        /// Property to get and set the UserId. 
        /// </summary> 
        public string UserIdSetting
        {
            get
            {
                return GetValueOrDefault<string>(UserIdSettingKeyName, UserIdSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(UserIdSettingKeyName, value))
                {
                    Save();
                }
            }
        }

    }
}
