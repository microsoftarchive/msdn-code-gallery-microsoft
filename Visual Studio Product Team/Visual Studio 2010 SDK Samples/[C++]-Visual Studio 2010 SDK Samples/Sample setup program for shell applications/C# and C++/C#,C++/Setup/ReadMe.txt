================================================================================
 Copyright (c) Microsoft Corporation. All rights reserved.
 This code is licensed under the Visual Studio SDK license terms.
 THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
 IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
 PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
================================================================================

================================================================================
    Loading and Understanding this Solution
================================================================================

A fuller explanation of this sample can be found at:
    http://msdn.microsoft.com/en-us/library/bb932484.aspx

In order to load all the projects in this solution you will need to install:
    Microsoft Visual Studio 2012 Visualization & Modeling SDK
        http://www.microsoft.com/en-us/download/details.aspx?id=30680
    WiX Toolset 3.6
        http://wix.codeplex.com/releases/view/93929

================================================================================
    Modifying Setup for Visual Studio Shell 2012 Based Applications
================================================================================

In the string table, modify the following values:
    IDS_BOUTIQUE_MSI    The name of your MSI, e.g. PhotoStudio.msi
    IDS_BOUTIQUE_NAME   The friendly name of your application, e.g. "VSP's Photo Studio 1.0"
    IDS_KEY_MSIROOT     The uninstall key with your product GUID to detect install status

================================================================================
    Using Setup to Deploy Your Visual Studio Shell 2012 Based Application
================================================================================

To use Setup, place the required shell SFX or SFXes and your application's MSI in the same directory as setup.exe.

Command Line Options:
    passive      Displays install progress but requires no user interaction.
    quiet        Suppresses all UI

For example:
    C:\SampleInstaller\ contains the folllowing files
        Setup.exe
        PhotoStudio.msi
        VS_IsoShell.exe
        VS_IntShellAdditional.exe
    From command prompt - C:\SampleInstaller\setup.exe /passive

================================================================================
    Using Setup to Deploy in a different language
================================================================================

Change the definition of ProductLanguage
    PhotoStudioIntShell\Defines.wxi
    PhotoStudioIsoShell\Defines.wxi     e.g.    <?define ProductLanguage = "1033" ?>

Access the Properties and change the culture of any resource files in your application and the setup project
   PhotoStudio.rc
   PhotoStudioui.rc
   Setup.rc                            e.g.    English (United States) (0x409) (/l 0x0409)

Change the LANGUAGE macro and code pages in any .rc files to the correct language and sublanguage
   PhotoStudio.rc
   PhotoStudioUI.rc
   Setup.rc                            e.g.    LANGUAGE LANG_ENGLISH, SUBLANG_ENGLISH_US
                                               #pragma code_page(1252)

Change the language of the Version Information Block Header for all applications
   PhotoStudio.rc
   Setup.rc                            e.g.    English (United States)

In the string table, modify the LCID and translate the needed strings from their English equivalents
If your language requires Unicode characters then you will need to open the .rc file to View Code,
save it as a Unicode file, and use the code editor to make any changes to your .rc files.
    IDS_KEY_ISOSHELLDETECT  "SOFTWARE\\Microsoft\\DevDiv\\vs\\Servicing\\11.0\\isoshell\\1033"
    IDS_KEY_INTSHELLDETECT  "SOFTWARE\\Microsoft\\DevDiv\\vs\\Servicing\\11.0\\devenv\\1033"
    IDS_PROGRESS_INIT       "Initializing..."
    IDS_TITLE               "%s Setup"
    IDS_TITLE_WELCOME       "%s - Welcome"
    IDS_TITLE_COMPONENTS    "%s - Components"
    IDS_TITLE_PROGRESS      "%s - Installing"
    IDS_TITLE_FINISH        "%s - Completed"
    IDS_FAIL_DETECT_MSG     "Error while trying to detect if the required version of the SFX was installed."
    IDS_FAIL_DETECTMSI_MSG  Error while trying to detect if the MSI has already been installed."
    IDS_PROGRESS_DOWN       "Downloading (%i of %i KB).  Transfer rate: %i KB/s"
    IDS_PROGRESS_INSTALL    "Installing..."
    IDS_PROGRESS_CURRACTION "Current Action: %s"
    IDS_PROGRESS_ROLLBACK   "Rolling back..."
    IDS_FINISH_FAIL         "There was an error during setup.  Installation did not complete successfully!\nError Code: %d"
    IDS_WELCOME_MSG         "Welcome to %s Installer.  This wizard will guide you through the process of installing your application.\n\n\n\n\nPress 'Next' to continue..."
    IDS_PENDING_REBOOT      "A restart is required for installation to continue.  Press Restart Now to restart your computer and continue installation."
    IDS_FINISH_SUCCESS      "Installation completed successfully!"
    IDS_FINISH_NOTADMIN     "Install did not complete successfully.  Setup requires administrator priveleges.  Please restart as an adminstrator to continue installation."
    IDS_ALREADY_INSTALLED   "%s is already installed on this computer."
    IDS_RESTART_NOW         "Restart Now"

In setup.rc, change the text for the Components page
    "The following components will be installed:"
    "Press next to begin the installation process."
