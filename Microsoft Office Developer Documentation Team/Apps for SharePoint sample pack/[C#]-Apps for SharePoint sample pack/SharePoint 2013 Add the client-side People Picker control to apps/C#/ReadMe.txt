SharePoint 2013: Add the client-side People Picker control to apps 

This sample SharePoint-hosted app shows how to add the client-side People Picker control to your app and how to query the picker from other client-side controls. The app lets you enter user names and then calls the picker's GetAllUserInfo and GetAllUserKeys methods and displays the returned user information on the page.

The client-side People Picker control is an HTML and JavaScript control that lets users quickly find and select people, groups, and claims from their organization. The picker is represented by the SPClientPeoplePicker object, which provides methods that other client-side controls can use to get information or to perform other operations. You can use SPClientPeoplePicker properties to configure the picker with control-specific settings, such as allowing users to select multiple people or specifying caching options. The picker also uses the web application configuration settings that are specified in the SPWebApplication.PeoplePickerSettings property.

The sample demonstrates how to add the picker to your page markup, and how to initialize it and query it from your script. Rendering, initializing, and other functionality for the picker are handled by the server, including searching and resolving user input against the SharePoint authentication providers.

For instructions about how to create this sample, see "How to: Use the client-side People Picker control in apps for SharePoint" (http://msdn.microsoft.com/en-us/library/jj713593.aspx).


Language implementations
===============================
This sample is available in the following language implementations:
    JavaScript


Prerequisites
===============================
This sample requires the following:

    - An Office 365 Developer Site
    - "Napa" Office 365 Development Tools

     or

    - A SharePoint 2013 development environment that is configured for app isolation. If you're developing remotely, either the server must support sideloading of apps or you must install the app on a Developer Site.
    - Visual Studio 2012 and Office Developer Tools for Visual Studio 2012
    - Local administrator permissions on the development computer.
    - "Manage Web Site" and "Create Subsites" user permissions to the SharePoint site where you're installing the app. (By default, these permissions are available only to users who have the Full Control permission level or who are in the site Owners group.) To install the app, you must be logged on as someone other than the system account.

Note: See "Sign up for an Office 365 Developer Site" (http://msdn.microsoft.com/en-us/library/office/apps/fp179924%28v=office.15%29) to sign up for a Developer Site and start using "Napa" Office 365 Development Tools or see "How to: Set up an on-premises SharePoint 2013 development environment for apps" (http://msdn.microsoft.com/en-us/library/office/apps/fp179923(v=office.15)) for guidance about how to set up an on-premises development environment (and how to disable the loopback check, if necessary). If you are developing remotely, see "Developing apps for SharePoint on a remote system" (http://msdn.microsoft.com/en-us/library/jj220047.aspx).
 

Files
===============================
The client-side People Picker control sample app contains the following key components:

    - The clientPeoplePickerApp.sln solution file
    - The clientPeoplePickerApp.csproj project file
    - The Default.aspx file (in the Pages folder), which contains the HTML markup for the div element that hosts the picker and the references to the picker's script dependencies
    - The App.js file, which contains the script that initializes the picker, gets the picker object from the page, and queries the picker from a client-side control
    - The appManifest.xml file, which specifies properties for the app for SharePoint

Note: The ClientWebPart.aspx file and the ClientWebPart1 folder are automatically generated when you create a SharePoint-hosted app in "Napa" Office 365 Development Tools. They are not used in this sample.
 

To configure the sample app
===============================
    1. Run Visual Studio 2012 as an administrator, and then open the extracted sample solution file.
    2. In the Properties window for the clientPeoplePickerApp project, update the SiteUrl property with the URL for your SharePoint 2013 site.


To run and test the sample
===============================
    1. Press F5 to build and deploy the app.
       Note: You'll be prompted to log in to the SharePoint site if you're running the app remotely. You'll also be prompted to log in to the isolated app domain if it isn't listed as a trusted site in your browser.
    2. In the picker's text box, enter the names or email addresses of valid SharePoint accounts for users, groups, or claims, separated by semicolons.
    3. Choose the Get User Info button to display information about the resolved accounts.


Troubleshooting
===============================
If the app fails to install, verify that the logged-on account is not the system account and that it has the required user permissions to the SharePoint site that you're deploying the app to (as described in Prerequisites [#O15Readme_Prereq]). Also verify that the SiteUrl property you specified matches the URL of your SharePoint 2013 site. If you made any changes to the appManifest.xml file, verify that the changes are correct and the XML parses successfully.


Related content
===============================
People Picker and claims providers overview (SharePoint 2013) (http://technet.microsoft.com/en-us/library/gg602078(office.15).aspx)
Apps for SharePoint overview (http://msdn.microsoft.com/en-us/library/office/apps/fp179930(v=office.15))