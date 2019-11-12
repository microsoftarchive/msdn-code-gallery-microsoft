// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Profile;
using Windows.Security.Cryptography; 

namespace Doto
{
    /// <summary>
    /// Generates a persistent unique identifier for this installation that is persisted in
    /// local storage. This is used by Doto to manage channelUris for push notifications
    /// </summary>
    public static class InstallationId
    {
        private static string installationidContainerName = "dotoInstallationIdContainer";
        private static string installationidSettingName = "dotoInstallationIdSetting";
        private static string value = null;

        public static string GetInstallationIdAsync()
        {
            if (value != null)
            {
                return value;
            }

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            bool hasContainer = localSettings.Containers.ContainsKey(installationidContainerName);
            bool hasSetting = false;

            if (hasContainer)
            {
                hasSetting = localSettings.Containers[installationidContainerName].Values.ContainsKey(installationidSettingName);

                if (hasSetting)
                {
                    value = (string)localSettings.Containers[installationidContainerName].Values[installationidSettingName];
                }
            }

            if (!hasContainer)
            {
                // Create a container if there is none
                localSettings.CreateContainer(installationidContainerName, ApplicationDataCreateDisposition.Always);
            }

            if (!hasSetting)
            {
                // Create a setting if there is none and store it locally
                // Note that this is a simple implementation. Refer to this link for more details - http://msdn.microsoft.com/en-us/library/windows/apps/jj553431.aspx
                // The user id is added on the server side and the code below is only for the user device
                var token = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
                string installationId = CryptographicBuffer.EncodeToBase64String(token.Id);
                localSettings.Containers[installationidContainerName].Values[installationidSettingName] = installationId;
                value = installationId;
            }

            return value;
        }
    }
}
