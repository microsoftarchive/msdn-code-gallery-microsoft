This Hotspot Authentication Sample demonstrates how to use the Hotspot Authentication API on both Windows and Windows Phone clients. The sample consists of 4 projects: Windows Phone, Windows, Shared and a Background Task. The code in the shared and background task is common to both platforms. Even though a large part of the API is converged, and the code is shared between both the projects, there are few key differences that a developer should be aware of:

1.WISPr requirement for access points: 
    a. On Windows clients: In order to invoke the hotspot authentication flow (background task), the access point being connected to must provide a captive portal response with WISPr support claimed in the XML blob being returned to the client. 
    b. On Windows Phone clients: There is no requirement for captive portal or WISPr support on the Access Point. In this case, the background task is launched as soon as the Wi-Fi connection to the access point is completed and an IP address is acquired – regardless of whether the access point claims WISPr support or not.

2. Authentication using WISPr:
    a. On Windows client, the Hotspot Authentication API supports two ways of completing authentication
        i.Issuing WISPr credentials using IssueCredentialsAsync API, which uses the native WISPr implementation of Windows.
        ii.Perform a custom WISPr authentication using the information obtained through HotspotAuthenticationContext.TryGetAuthenticationContext. In this case, you must call  HotspotAuthenticationContext.SkipAuthentication API to skip the Native WISPr authentication process once the custom authentication is complete

    b. On Windows Phone platform, there is no Native WISPr support. As a result, the IssueCredentialsAsync API is not supported (throws NotImplementedException). The only way to perform WISPr authentication on phone platform is to implement it within the app and then call HotspotAuthenticationContext.SkipAuthentication API.

3. Contents of HostpotAuthenticationContext class:
    a. On Windows client, a HotspotAuthenticationContext object contain a set of properties documented at http://msdn.microsoft.com/en-us/library/windows/apps/windows.networking.networkoperators.hotspotauthenticationcontext.aspx
    b. On Windows phone, the only valid Properties of HotspotAuthenticationContext are WirelessNetworkId  (SSID) and NetworkAdapter. This is because the remaining properties are obtained from the native WISPr implementation of Windows, which is not supported on phone.

If your supported list of Access Points are all WISPr-based, and your app implements WISPr authentication itself, it will result in 100% sharing of Hotspot Authentication API code between Windows Phone and Windows.
