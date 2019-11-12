// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include <ipsectypes.h>
#include "Utils.h"
#include "ServerConfiguration.h"

//********************************************************************************************
// Function: SetCustomIpsecConfigurationOnServer
//
// Description: Configures custom IPSec policies on the remote access server that would be applied to 
//                   all the VPN clients.
//
//********************************************************************************************
VOID SetCustomIpsecConfigurationOnServer(
    _In_opt_ LPWSTR serverName
    )
{
    DWORD status = ERROR_SUCCESS;
    MPR_SERVER_HANDLE serverHandleAdmin = NULL;
    HANDLE serverHandleConfig = NULL;

    wprintf(L"---------------------------------------------------------\n\n");
    wprintf(L"Executing SetCustomIpsecConfigurationOnServer on '%s'\n", \
        (serverName == NULL) ? L"Current machine" : serverName);
    
    // Try connecting to remoteAccess (RRAS) server for both administration and and configuration to 
    // get the handles serverHandleAdmin and serverHandleConfig. We cannot create/modify/retrieve 
    // server configurations, if it fails to get both of these handles. 
    //
    // If successfully gets the serverHandleAdmin handle, the MprAdmin APIs would be used 
    // to create/modify/retrieve the server configurations. Otherwise MprConfig APIs would be used.
    //
    status = RemoteAccessServerConenct(serverName, &serverHandleAdmin, &serverHandleConfig);

    // Any one of these handles (serverHandleAdmin or serverHandleConfig) has to be valid.
    //
    if ((ERROR_SUCCESS != status) && 
        (NULL == serverHandleAdmin) &&
        (NULL == serverHandleConfig))
    {
        wprintf(L"RemoteAccessServerConenct failed. \
            The RemoteAccess service might be stopped - Try after starting the RemoteAccess service.\n");
        DisplayError(status);
        goto Done;
    }
    
    // Set custom IPSec policies on server
    //
    status = ConfigureCustomIPSecPolicyOnServer(serverHandleAdmin, serverHandleConfig);
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"ConfigureCustomIPSecPolicyOnServer failed.\n");
        DisplayError(status);
        goto Done;
    }

    // Retrieve and display the custom IPSec configuration on server
    //
    status = DisplayCustomIPSecConfigurationOnServer(serverHandleAdmin, serverHandleConfig);
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"DisplayCustomIPSecConfiguration failed.\n");
        DisplayError(status);
        goto Done;
    }

Done:
    RemoteAccessServerDisconenct(serverHandleAdmin, serverHandleConfig);
    
    if (status != ERROR_SUCCESS)
    {
         wprintf(L"SetCustomIpsecConfigurationOnServer failed\n");
         DisplayError(status);
    }
    wprintf(L"---------------------------------------------------------\n\n");
}

//********************************************************************************************
// Function: RemoveCustomIpsecConfigurationFromServer
//
// Description: Removes the custom IPSec configuration (if configured) from the specified remote access server.
//
//********************************************************************************************
VOID RemoveCustomIpsecConfigurationFromServer(
    _In_opt_ LPWSTR serverName
    )
{
    DWORD status = ERROR_SUCCESS;
    MPR_SERVER_HANDLE serverHandleAdmin = NULL;
    HANDLE serverHandleConfig = NULL;
    MPR_SERVER_SET_CONFIG_EX serverSetConfigInfo;
    MPR_SERVER_EX serverConfigInfo;
    
    wprintf(L"---------------------------------------------------------\n\n");
    wprintf(L"Executing RemoveCustomIpsecConfigurationFromServer on '%s'\n", \
        (serverName == NULL) ? L"Current machine" : serverName);
    
    ZeroMemory(&serverConfigInfo, sizeof(serverConfigInfo));
    ZeroMemory(&serverSetConfigInfo, sizeof(MPR_SERVER_SET_CONFIG_EX));

    
    // Try connecting to remoteAccess (RRAS) server for both administration and and configuration to 
    // get the handles serverHandleAdmin and serverHandleConfig. We cannot create/modify/retrieve 
    // server configurations, if it fails to get both of these handles. 
    //
    // If successfully gets the serverHandleAdmin handle, the MprAdmin APIs would be used 
    // to create/modify/retrieve the server configurations. Otherwise MprConfig APIs would be used.
    //
    status = RemoteAccessServerConenct(serverName, &serverHandleAdmin, &serverHandleConfig);

    // Any one of these handles (serverHandleAdmin or serverHandleConfig) has to be valid.
    //
    if ((ERROR_SUCCESS != status) && 
        (NULL == serverHandleAdmin) &&
        (NULL == serverHandleConfig))
    {
        wprintf(L"RemoteAccessServerConenct failed. \
            The RemoteAccess service might be stopped - Try after starting the RemoteAccess service.\n");
        DisplayError(status);
        goto Done;
    }

    // Retrieve and display the custom IPSec configuration on server (before removing the custom configuration);
    //
    status = DisplayCustomIPSecConfigurationOnServer(serverHandleAdmin, serverHandleConfig);
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"DisplayCustomIPSecConfiguration failed.\n");
        DisplayError(status);
        goto Done;
    }
    
    // Set the header on MPR_SERVER_EX before querying for the server configuration
    //
    serverConfigInfo.Header.revision = MPRAPI_MPR_SERVER_OBJECT_REVISION_2;
    serverConfigInfo.Header.type     = MPRAPI_OBJECT_TYPE_MPR_SERVER_OBJECT;
    serverConfigInfo.Header.size     = sizeof(MPR_SERVER_EX);
    
    // Retrieve the current configuration on remote access server 
    //
    (serverHandleAdmin != NULL) ? (status = g_pMprAdminServerGetInfoEx (serverHandleAdmin,&serverConfigInfo)) :
           (status = g_pMprConfigServerGetInfoEx (serverHandleConfig, &serverConfigInfo));
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"%s failed.\n", (serverHandleAdmin != NULL) ? \
                L"MprAdminServerGetInfoEx" : L"MprConfigServerGetInfoEx");
        DisplayError(status);
        goto Done;
    }
    
    // Check whether custom IPSec configuration is available on the RRAS server
    //
    if ((serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.cbData == 0) &&
        (serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy == NULL))
    {
        // Neither custom policy nor machine certificate is configured
        // 
        wprintf(L"Custom IPSec configuration not available on the server.\n");
        goto Done;
    }
    
    wprintf(L"Removing custom IPsec configuration from server.\n");

    // Set the header on MPR_SERVER_SET_CONFIG_EX
    //
    serverSetConfigInfo.Header.revision = MPRAPI_MPR_SERVER_SET_CONFIG_OBJECT_REVISION_2;
    serverSetConfigInfo.Header.type     = MPRAPI_OBJECT_TYPE_MPR_SERVER_SET_CONFIG_OBJECT;
    serverSetConfigInfo.Header.size     = sizeof(MPR_SERVER_SET_CONFIG_EX);    
    serverSetConfigInfo.setConfigForProtocols = MPRAPI_SET_CONFIG_PROTOCOL_FOR_IKEV2;

    // Copy the current server configuration into serverSetConfigInfo 
    //
    memcpy(&serverSetConfigInfo.ConfigParams.IkeConfigParams, &serverConfigInfo.ConfigParams.IkeConfigParams, sizeof(IKEV2_CONFIG_PARAMS));

    // Set the custom IPSec policies on MPR_SERVER_SET_CONFIG_EX
    //
    serverSetConfigInfo.ConfigParams.IkeConfigParams.dwTunnelConfigParamFlags |= MPRAPI_IKEV2_SET_TUNNEL_CONFIG_PARAMS;
    
    // In order to remove machine certificate configuration, 
    // set the cbData of machineCertificateName (CERT_NAME_BLOB) to 0 and 
    // set the cbData of machineCertificateName (CERT_NAME_BLOB) to NULL.
    //
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.cbData = 0;
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.pbData = NULL;

    // In order to remove custom IPsec policy set TunnelConfigParams.customPolicy to NULL
    //
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy = NULL;

    // Update the remote access server configuration 
    //
    (serverHandleAdmin != NULL) ? (status = g_pMprAdminServerSetInfoEx (serverHandleAdmin,&serverSetConfigInfo)) :
           (status = g_pMprConfigServerSetInfoEx (serverHandleConfig, &serverSetConfigInfo));
    if (ERROR_SUCCESS_RESTART_REQUIRED == status)
    {
        // This is not a failure case - rather a warning for restarting the remoteaccess service. 
        wprintf(L"WARNING:RemoteAccess service needs to be restarted.\n");
        status = ERROR_SUCCESS;
    }

    if (ERROR_SUCCESS != status)
    {
        wprintf(L"%s failed.\n", (serverHandleAdmin != NULL) ? \
        L"MprAdminServerSetInfoEx" : L"MprConfigServerSetInfoEx");
        DisplayError(status);
        goto Done;
    }

    // Retrieve and display the custom IPSec configuration on server (after removing the custom configuration);
    //
    status = DisplayCustomIPSecConfigurationOnServer(serverHandleAdmin, serverHandleConfig);
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"DisplayCustomIPSecConfiguration failed.\n");
        DisplayError(status);
        goto Done;
    }
    
Done:
    
    // Free the memory inside server config info (MPR_SERVER_EX)
    //
    FreeServerConfigObject((NULL != serverHandleAdmin), &serverConfigInfo);

    RemoteAccessServerDisconenct(serverHandleAdmin, serverHandleConfig);
    
    if (status != ERROR_SUCCESS)
    {
         wprintf(L"SetCustomIpsecConfigurationOnServer failed\n");
         DisplayError(status);
    }
    wprintf(L"---------------------------------------------------------\n\n");
}

//********************************************************************************************
// Function: DisplayCustomIPSecConfigurationOnServer
//
// Description: Configures custom IPSec configuration, to be used applied to the IKEv2 
// tunnel based VPN clients, on the RRAS server 
//
//********************************************************************************************
DWORD ConfigureCustomIPSecPolicyOnServer(
    _In_ MPR_SERVER_HANDLE serverHandleAdmin,
    _In_ HANDLE serverHandleConfig
    )
{
    DWORD status = ERROR_SUCCESS;
    MPR_SERVER_SET_CONFIG_EX serverSetConfigInfo;
    PROUTER_CUSTOM_IKEv2_POLICY0 customIkev2Policy = NULL;
    CERT_NAME_BLOB machineCertificate;
    MPR_SERVER_EX serverConfigInfo;

    ZeroMemory(&serverConfigInfo, sizeof(serverConfigInfo));
    ZeroMemory(&machineCertificate, sizeof(CERT_NAME_BLOB));
    ZeroMemory(&serverSetConfigInfo, sizeof(MPR_SERVER_SET_CONFIG_EX));
    
    // Create a custom IKEv2 policy object
    //
    customIkev2Policy = (ROUTER_CUSTOM_IKEv2_POLICY0*) MPRAPI_SAMPLE_MALLOC(sizeof(ROUTER_CUSTOM_IKEv2_POLICY0));
    if (NULL == customIkev2Policy)
    {
        status = ERROR_OUTOFMEMORY;
        wprintf(L"Error:Failed to allocate memory.\n");
        DisplayError(status);
        goto Done;
    }
    customIkev2Policy->dwIntegrityMethod = IKEEXT_INTEGRITY_SHA_256;
    customIkev2Policy->dwEncryptionMethod = IKEEXT_CIPHER_AES_256;
    customIkev2Policy->dwCipherTransformConstant = IKEEXT_CIPHER_AES_256;
    customIkev2Policy->dwAuthTransformConstant = IPSEC_AUTH_SHA_256;
    customIkev2Policy->dwPfsGroup = IPSEC_PFS_2048;
    customIkev2Policy->dwDhGroup = IKEEXT_DH_GROUP_2;

    // Get the machine certificate to be configured 
    //
    if (ERROR_SUCCESS != (status = GetCertifictateNameBlob(&machineCertificate)))
    {
        wprintf(L"GetCertifictateNameBlob failed\n");
        DisplayError(status);
        // ignore the error - we will not configure a certficate on the interface 
        // 
    }

    // Set the header on MPR_SERVER_EX before querying for the server configuration
    //
    serverConfigInfo.Header.revision = MPRAPI_MPR_SERVER_OBJECT_REVISION_2;
    serverConfigInfo.Header.type     = MPRAPI_OBJECT_TYPE_MPR_SERVER_OBJECT;
    serverConfigInfo.Header.size     = sizeof(MPR_SERVER_EX);
    
    // Either serverHandleAdmin or  serverHandleConfig has to have a valid handle value.
    //
    // If we have a valid handle value for serverHandleAdmin, the MprAdmin APIs would be used 
    // to create/modify/retrieve the server configurations. Otherwise MprConfig APIs would be used.
    //

    // Retrieve the current configuration on remote access server 
    //
    (serverHandleAdmin != NULL) ? (status = g_pMprAdminServerGetInfoEx (serverHandleAdmin,&serverConfigInfo)) :
           (status = g_pMprConfigServerGetInfoEx (serverHandleConfig, &serverConfigInfo));
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"%s failed.\n", (serverHandleAdmin != NULL) ? \
                L"MprAdminServerGetInfoEx" : L"MprConfigServerGetInfoEx");
        DisplayError(status);
        goto Done;
    }

    // Set the header on MPR_SERVER_SET_CONFIG_EX
    //
    serverSetConfigInfo.Header.revision = MPRAPI_MPR_SERVER_SET_CONFIG_OBJECT_REVISION_2;
    serverSetConfigInfo.Header.type     = MPRAPI_OBJECT_TYPE_MPR_SERVER_SET_CONFIG_OBJECT;
    serverSetConfigInfo.Header.size     = sizeof(MPR_SERVER_SET_CONFIG_EX);    
    serverSetConfigInfo.setConfigForProtocols = MPRAPI_SET_CONFIG_PROTOCOL_FOR_IKEV2;
    
    // Copy the current server configuration into serverSetConfigInfo 
    //
    memcpy(&serverSetConfigInfo.ConfigParams.IkeConfigParams, &serverConfigInfo.ConfigParams.IkeConfigParams, sizeof(IKEV2_CONFIG_PARAMS));

    // Set the custom IPSec policies on MPR_SERVER_SET_CONFIG_EX
    //
    serverSetConfigInfo.ConfigParams.IkeConfigParams.dwTunnelConfigParamFlags |= MPRAPI_IKEV2_SET_TUNNEL_CONFIG_PARAMS; 
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.cbData = machineCertificate.cbData;
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.pbData = machineCertificate.pbData;
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy = customIkev2Policy;
    serverSetConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.dwEncryptionType = 2; //RRAS requires encryption to be negotiated

    // Update the remote access server configuration with the custom IPSec policy
    //
    (serverHandleAdmin != NULL) ? (status = g_pMprAdminServerSetInfoEx (serverHandleAdmin,&serverSetConfigInfo)) :
           (status = g_pMprConfigServerSetInfoEx (serverHandleConfig, &serverSetConfigInfo));
    if (ERROR_SUCCESS_RESTART_REQUIRED == status)
    {
        // This is not a failure case - rather a warning for restarting the remoteaccess service. 
        wprintf(L"WARNING:RemoteAccess service needs to be restarted.\n");
        status = ERROR_SUCCESS;
    }
   
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"%s failed.\n", (serverHandleAdmin != NULL) ? \
        L"MprAdminServerSetInfoEx" : L"MprConfigServerSetInfoEx");
        DisplayError(status);
        goto Done;
    }

Done:
    MPRAPI_SAMPLE_FREE(customIkev2Policy);

    if ((machineCertificate.cbData > 0) && 
        (NULL != machineCertificate.pbData))
    {
        MPRAPI_SAMPLE_FREE(machineCertificate.pbData);
    }

    // Free the memory inside server config info (MPR_SERVER_EX)
    //
    FreeServerConfigObject((NULL != serverHandleAdmin), &serverConfigInfo);

    return status;
}

//********************************************************************************************
// Function: DisplayCustomIPSecConfigurationOnServer
//
// Description: Retrieves and displays the custom IPSec policies configured on the RRAS server
//
//********************************************************************************************
DWORD DisplayCustomIPSecConfigurationOnServer(
    _In_ MPR_SERVER_HANDLE serverHandleAdmin,
    _In_ HANDLE serverHandleConfig
    )
{
    DWORD status = ERROR_SUCCESS;
    MPR_SERVER_EX serverConfigInfo;

    ZeroMemory(&serverConfigInfo, sizeof(serverConfigInfo));
    
    // Set the header on MPR_SERVER_EX before querying for the server configuration
    //
    serverConfigInfo.Header.revision = MPRAPI_MPR_SERVER_OBJECT_REVISION_2;
    serverConfigInfo.Header.type     = MPRAPI_OBJECT_TYPE_MPR_SERVER_OBJECT;
    serverConfigInfo.Header.size     = sizeof(MPR_SERVER_EX);
    
    // Either serverHandleAdmin or  serverHandleConfig has to have a valid handle value.
    //
    // If we have a valid handle value for serverHandleAdmin, the MprAdmin APIs would be used 
    // to create/modify/retrieve the server configurations. Otherwise MprConfig APIs would be used.
    //

    // Retrieve the current configuration on remote access server 
    //
    (serverHandleAdmin != NULL) ? (status = g_pMprAdminServerGetInfoEx (serverHandleAdmin,&serverConfigInfo)) :
           (status = g_pMprConfigServerGetInfoEx (serverHandleConfig, &serverConfigInfo));
    if (ERROR_SUCCESS != status)
    {
        wprintf(L"%s failed.\n", (serverHandleAdmin != NULL) ? \
                L"MprAdminServerGetInfoEx" : L"MprConfigServerGetInfoEx");
        DisplayError(status);
        goto Done;
    }
    
    wprintf(L"IPSec configuration on server:\n");
    wprintf(L"\t dwSaLifeTime: %u\n", serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.dwSaLifeTime);
    wprintf(L"\t dwSaDataSizeForRenegotiation: %u\n", serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.dwSaDataSizeForRenegotiation);
    
    wprintf(L"\t Machine CertificateName configured: %s\n", \
        (0 == serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.cbData) ? \
        L"No" : L"Yes");
    
    wprintf(L"\t IPSec custom policy:\n");
    if (serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy == NULL)
    {
        wprintf(L"\t\t IPsec custom policy not configured.\n");
    }
    else
    {
        PrintCustomIkev2Policy(L"\t\t", serverConfigInfo.ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy);
    }

Done:
    // Free the memory inside server config info (MPR_SERVER_EX)
    //
    FreeServerConfigObject((NULL != serverHandleAdmin), &serverConfigInfo);

    return status;
}

//********************************************************************************************
// Function: FreeServerConfigObject
//
// Description: Releases the various fields of the MPR_SERVER_EX structure,
// those are allocated by the remote access server.
//
//********************************************************************************************
VOID FreeServerConfigObject(
    _In_ BOOL useAdminApi,
    _In_ MPR_SERVER_EX* serverConfigInfo
    )
{
    // useAdminApi is TRUE maeans the serverConfigInfo was retrieved using MprAdmin API.
    // Hence MprAdminBufferFree API has to be used for releasing the memory.
    //
    
    // useAdminApi is FALSE maeans the serverConfigInfo was retrieved using MprConfig API.
    // Hence MprConfigBufferFree API has to be used for releasing the memory.
    //

    if (serverConfigInfo == NULL)
        return;
    
    if (serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.certificateNames != 0)
    {
        (useAdminApi) ? \
            g_pMprAdminBufferFree(serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.certificateNames) : \
            g_pMprConfigBufferFree(serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.certificateNames);
        serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.certificateNames = NULL;
    }
    
    if (serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.cbData != 0)
    {
        (useAdminApi) ? \
            g_pMprAdminBufferFree(serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.pbData) : \
            g_pMprConfigBufferFree(serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.pbData);
        serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.cbData = 0;
        serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.machineCertificateName.pbData = NULL;
    }
    
    if (serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy != NULL)
    {
        (useAdminApi) ? \
            g_pMprAdminBufferFree(serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy) : \
            g_pMprConfigBufferFree(serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy);
        serverConfigInfo->ConfigParams.IkeConfigParams.TunnelConfigParams.customPolicy = NULL;
    }
}

