var applicationData = Windows.Storage.ApplicationData.current;
var localSettings = applicationData.localSettings;
var value;

function readResetSettingsValue(key) {
    /// <summary>
    /// Function to read a setting value and clear it after reading it
    /// </summary>

    try{
        // Check if the key is present
        var keyPresent = localSettings.values.hasKey[key];      
        
        if (!keyPresent) {            
            // Access data in value
            value = localSettings.values[key];
            localSettings.values.remove(key);         
            
        }
    }
    catch (error) {

        log("readResetSettingsValue:" + error.message);
        log("readResetSettingsValue:" + error.description);
    }

}

function saveSettingsValue(key, value) {
    try{
        var keyPresent = localSettings.values.hasKey[key];

        if (!keyPresent) {
            localSettings.values.insert(key, value);
        }
        else {
            localSettings.values[key] = value;
        }
    }
    catch (error) {

        log("saveSettingsValue:" + error.message);
        log("saveSettingsValue:" + error.description);
    }

}
