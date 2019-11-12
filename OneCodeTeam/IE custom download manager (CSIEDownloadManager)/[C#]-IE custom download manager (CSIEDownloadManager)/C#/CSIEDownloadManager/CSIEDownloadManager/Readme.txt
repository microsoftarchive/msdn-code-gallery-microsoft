================================================================================
	            IE Extension: CSIEDownloadManager
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to implement a custom download manager for IE. 
When IE starts to download a file, the CSWebDownloader.exe will be launched to
download the file.

NOTE: Some third party download extensions for IE may conflict with this code sample,
      so it is better to disable them temporarily before you try out the sample. 

/////////////////////////////////////////////////////////////////////////////
Setup and Removal:

A. Setup

For 32bit IE on x86 or x64 OS, install CSIEDownloadManagerSetup(x86).msi, the output
of the CSIEDownloadManagerSetup(x86) setup project.

For 64bit IE on x64 OS, install CSIEDownloadManagerSetup(x64).msi outputted by the 
CSIEDownloadManagerSetup(x64) setup project.

B. Removal

For 32bit IE on x86 or x64 OS, uninstall CSIEDownloadManagerSetup(x86).msi, the 
output of the CSIEDownloadManagerSetup(x86) setup project. 

For 64bit IE on x64 OS, uninstall CSIEDownloadManagerSetup(x64).msi, the output of
the CSIEDownloadManagerSetup(x64) setup project.
   
////////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Open this project in VS2010 and set the platform of the solution to x86. Make
       sure that the projects CSIEDownloadManagerSetup, CSWebDownloader and 
       CSIEDownloadManagerSetup(x86) are selected to build in Configuration Manager.

	   NOTE: If you want to run this sample in 64bit IE, set the platform to x64 and 
	         select CSIEDownloadManagerSetup, CSWebDownloader and 
             CSIEDownloadManagerSetup(x64) to build.
        
Step2. Build the solution.
 
Step3. Right click the project CSIEDownloadManagerSetup(x86) in Solution Explorer, 
       and choose "Install".


Step4. Open 32bit IE and visit the the download link of Microsoft .NET Framework 4 
       http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&FamilyID=0a391abd-25c1-4fc0-919f-b21f31ab88b7.
       
       Click the "Download" button on this page, and then CSWebDownloader.exe will 
       be launched. In CSWebDownloader.exe, you will find that the url is 
       http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe
       and the local path is D:\dotNetFx40_Full_x86_x64.exe

Step5. Click the button "Download" in CSWebDownloader.exe, it will start to download 
       the file, and after a few minutes, you will find a file D:\dotNetFx40_Full_x86_x64.exe
       in Windows Explorer.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

A. Create the CSWebDownloader project with following features:
   1. Set the buffer and cache size.
   2. Download a specified block data of the whole file. 
   3. Start, Pause, Resume and Cancel a download.  
   4. Supply the file size, download speed and used time.
   5. Expose the events StatusChanged, DownloadProgressChanged and DownloadCompleted.
   6. Can be launched with an argument (the file to download).

   For more detailed information about the CSWebDownloader.exe, see 
   http://code.msdn.microsoft.com/CSWebDownloader-5fdcfb74 

B. Create the CSIEDownloadManager project.

   In Visual Studio 2010, create a Visual C# / Windows / Class Library project 
   named "CSIEDownloadManager". 
   
   The ability to implement a custom download manager was introduced in Microsoft 
   Internet Explorer 5.5. This feature enables you to extend the functionality of 
   Windows Internet Explorer and WebBrowser applications by implementing a Component
   Object Model (COM) object to handle the file download process.
   
   A download manager is implemented as a COM object that exposes the IUnknown and
   IDownloadManager interface. IDownloadManager has only one method, 
   IDownloadManager::Download, which is called by Internet Explorer or a WebBrowser
   application to download a file. 
   
   For Internet Explorer 6 and later, if the WebBrowser application does not implement
   the IServiceProvider::QueryService method, or when using Internet Explorer itself 
   for which IServiceProvider::QueryService cannot be implemented, the application 
   checks for the presence of a registry key containing the class identifier (CLSID) 
   of the download manager COM object. The CLSID can be provided in either of the 
   following registry values.
   
       HKEY_LOCAL_MACHINE
            Software
                 Microsoft
                      Internet Explorer
                           DownloadUI
       HKEY_CURRENT_USER
            Software
                 Microsoft
                      Internet Explorer
                           DownloadUI
   
   DownloadUI is not a key, it is a REG_SZ value under Software\Microsoft\Internet Explorer.
   
   If the application cannot locate a custom download manager the default download user 
   interface is used.
   
   The IEDownloadManager class implements the IDownloadManager interface. When IE starts to 
   download a file, the Download method of this class will be called, and then the 
   CSWebDownloader.exe will be launched to download the file.

        public int Download(IMoniker pmk, IBindCtx pbc, uint dwBindVerb, int grfBINDF, 
            IntPtr pBindInfo, string pszHeaders, string pszRedir, uint uiCP)
        {

            // Get the display name of the pointer to an IMoniker interface that specifies
            // the object to be downloaded.
            string name = string.Empty;
            pmk.GetDisplayName(pbc, null, out name);

            if (!string.IsNullOrEmpty(name))
            {
                Uri url = null;
                bool result = Uri.TryCreate(name, UriKind.Absolute, out url);

                if (result)
                {

                    // Launch CSWebDownloader.exe to download the file. 
                    FileInfo assemblyFile = 
                        new FileInfo(Assembly.GetExecutingAssembly().Location);
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        Arguments = name,
                        FileName =
                        string.Format("{0}\\CSWebDownloader.exe", assemblyFile.DirectoryName)
                    };
                    Process.Start(start);
                    return 0;
                }              
            }
            return 1;
        }
   
   This class will also set the registry values when this assembly is registered to COM.
   
        /// <summary>
        /// Called when derived class is registered as a COM server.
        /// </summary>
        [System.Runtime.InteropServices.ComRegisterFunction]
        public static void Register(Type t)
        {
            string ieKeyPath = @"SOFTWARE\Microsoft\Internet Explorer\";
            using (RegistryKey ieKey = Registry.LocalMachine.CreateSubKey(ieKeyPath))
            {
                ieKey.SetValue("DownloadUI", t.GUID.ToString("B"));
            }
        }

        /// <summary>
        /// Called when derived class is unregistered as a COM server.
        /// </summary>
        [System.Runtime.InteropServices.ComUnregisterFunction]
        public static void Unregister(Type t)
        {
            string ieKeyPath = @"SOFTWARE\Microsoft\Internet Explorer\";
            using (RegistryKey ieKey = Registry.LocalMachine.OpenSubKey(ieKeyPath, true))
            {
                ieKey.DeleteValue("DownloadUI");
            }
        }

C. Deploying the IE Download Manager with a setup project.
   
   1. In CSIEDownloadManager, add an Installer class (named IEDownloadManagerInstaller 
      in this sample) to define the custom actions in the setup. The class derives from
      System.Configuration.Install.Installer. We use the custom actions to 
      register/unregister the COM-visible classes in the current managed assembly when
      user installs/uninstalls the component. 
   
           [RunInstaller(true), ComVisible(false)]
           public partial class IEDownloadManagerInstaller : Installer
           {
               public IEDownloadManagerInstaller()
               {
                   InitializeComponent();
               }
          
               public override void Install(System.Collections.IDictionary stateSaver)
               {
                   base.Install(stateSaver);
          
                   RegistrationServices regsrv = new RegistrationServices();
                   if (!regsrv.RegisterAssembly(this.GetType().Assembly,
                   AssemblyRegistrationFlags.SetCodeBase))
                   {
                       throw new InstallException("Failed To Register for COM");
                   }
               }
          
               public override void Uninstall(System.Collections.IDictionary savedState)
               {
                   base.Uninstall(savedState);
          
                   RegistrationServices regsrv = new RegistrationServices();
                   if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
                   {
                       throw new InstallException("Failed To Unregister for COM");
                   }
               }
           }
          
   
   2. To add a deployment project, on the File menu, point to Add, and then click 
      New Project. In the Add New Project dialog box, expand the Other Project Types node,
      expand the Setup and Deployment Projects, click Visual Studio Installer, and then 
      click Setup Project. In the Name box, type CSIEDownloadManagerSetup(x86). Click OK
      to create the project. 

      In the Properties dialog of the setup project, make sure that the TargetPlatform 
      property is set to x86. This setup project is to be deployed for 32bit IE on x86 
      or x64 Windows operating systems. 
    
      Right-click the setup project, and choose Add / Project Output .... In the 
      Add Project Output Group dialog box, CSIEDownloadManager and  CSWebDownloader will  
      be displayed in the Project list. Select the Primary Output (with the Debug | Any CPU
      configuration) of them and click OK. VS will detect the dependencies of the 
      CSIEDownloadManager and  CSWebDownloader, including .NET Framework 4.0 (Client Profile).
    
      Right-click the setup project, and choose View / Custom Actions. In the 
      Custom Actions Editor, right-click the root Custom Actions node. On the Action menu, 
      click Add Custom Action. In the Select Item in Project dialog box, double-click the 
      Application Folder. Select Primary output from CSIEDownloadManager. This adds the 
      custom actions that we defined in IEDownloadManagerInstaller of CSIEDownloadManager. 
     
      Build the setup project. If the build succeeds, you will get a .msi file 
      and a Setup.exe file. You can distribute them to your users to install or 
      uninstall this IE Download Manager. 
   
   3. To deploy the Explorer Bar for 64bit IE on a x64 operating system, you must create 
      a new setup project (e.g. CSIEDownloadManagerSetup(x64) in this sample), and set its
      TargetPlatform property to x64. 
   
      Although the TargetPlatform property is set to x64, the native shim packaged with 
      the .msi file is still a 32-bit executable. The Visual Studio embeds the 32-bit 
      version of InstallUtilLib.dll into the Binary table as InstallUtil. So the custom
      actions will be run in the 32-bit, which is unexpected in this code sample. To 
      workaround this issue and ensure that the custom actions run in the 64-bit mode, you
      either need to import the appropriate bitness of InstallUtilLib.dll into the Binary
      table for the InstallUtil record or - if you do have or will have 32-bit managed custom 
      actions add it as a new record in the Binary table and adjust the CustomAction table 
      to use the 64-bit Binary table record for 64-bit managed custom actions. This blog 
      article introduces how to do it manually with Orca
      http://blogs.msdn.com/b/heaths/archive/2006/02/01/64-bit-managed-custom-actions-with-visual-studio.aspx
     
      In this code sample, we automate the modification of InstallUtil by using a post-build
      javascript: Fix64bitInstallUtilLib.js. You can find the script file in the 
      CSIEDownloadManagerSetup(x64) project folder. To configure the script to run in the 
      post-build event, you select the CSIEDownloadManagerSetup(x64) project in Solution Explorer,
      and find the PostBuildEvent property in the Properties window. Specify its value to be 
      
           "$(ProjectDir)Fix64bitInstallUtilLib.js" "$(BuiltOuputPath)" "$(ProjectDir)"
     
      Repeat the rest steps in 2 to add the project output, set the custom actions, configure
      the prerequisites, and build the setup project.
   

/////////////////////////////////////////////////////////////////////////////
Diagnostic:

To debug the ID Download Manager, you can attach to iexplorer.exe. 


/////////////////////////////////////////////////////////////////////////////
References:

Implementing a Custom Download Manager
http://msdn.microsoft.com/en-us/library/aa753618(VS.85).aspx

How to implement a custom download manager
http://support.microsoft.com/kb/327865

IDownloadManager Interface
http://msdn.microsoft.com/en-us/library/aa753613(VS.85).aspx

/////////////////////////////////////////////////////////////////////////////
