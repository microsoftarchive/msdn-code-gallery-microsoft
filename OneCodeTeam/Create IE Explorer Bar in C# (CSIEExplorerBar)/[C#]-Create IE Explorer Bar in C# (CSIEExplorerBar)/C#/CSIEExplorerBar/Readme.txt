================================================================================
       Windows APPLICATION: CSIEExplorerBar Overview                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to create and deploy an IE Explorer Bar which could
list all the images in a web page.


/////////////////////////////////////////////////////////////////////////////
Setup and Removal:

A. Setup

For 32bit IE on x86 or x64 OS, install CSIEExplorerBarSetup(x86).msi, the output
of the CSIEExplorerBarSetup(x86) setup project.

For 64bit IE on x64 OS, install CSIEExplorerBarSetup(x64).msi outputted by the 
CSIEExplorerBarSetup(x64) setup project.

B. Removal

For 32bit IE on x86 or x64 OS, uninstall CSIEExplorerBarSetup(x86).msi, the 
output of the CSIEExplorerBarSetup(x86) setup project. 

For 64bit IE on x64 OS, uninstall CSIEExplorerBarSetup(x64).msi, the output of
the CSIEExplorerBarSetup(x64) setup project.



////////////////////////////////////////////////////////////////////////////////
Demo:
 
Step1. Open this project in VS2010 and set the platform of the solution to x86. Make
       sure that the projects CSIEExplorerBar and CSIEExplorerBarSetup(x86)
	   are selected to build in Configuration Manager.

	   NOTE: If you want to run this sample in 64bit IE, set the platform to x64 and 
	         select CSIEExplorerBar and CSIEExplorerBarSetup(x64) to build.
        
Step2. Build the solution.
 
Step3. Right click the project CSIEExplorerBarSetup(x86) in Solution Explorer, 
       and choose "Install".

	   After the installation, open 32bit IE and click Tools=>Manage Add-ons, in the 
	   Manage Add-ons dialog, you can find the item "Image List Explorer Bar".

Step4. Open 32bit IE and click Tools=>Explorer Bars, and select "Image List Explorer Bar".
       You will find a Explorer Bar show in the IE.

Step5. Visit http://www.microsoft.com/. Click the "Get all images" button in the Explorer
       Bar. You will see there are many image urls in the explorer bar. Click one item,
	   and IE will open a new tab to open the image.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Create the project and add references

In Visual Studio 2010, create a Visual C# / Windows / Class Library project 
named "CSIEExplorerBar". 

Right click the project in Solution Explorer and choose "Add Reference". Add
"Microsoft.mshtml" in the .NET tab and "Microsoft Internet Controls" in COM tab.
-----------------------------------------------------------------------------

B. Create IE Explorer Bar.

To add an Explorer Bar in IE, you can follow these steps:

1. Create a valid GUID for this ComVisible class. 

2. Implement the IObjectWithSite, IDeskBand, IDockingWindow, IOleWindow and 
   IInputObject interfaces.
   
3. Register this assembly to COM.

4. 4.Create a new key using the category identifier (CATID) of the type of 
   Explorer Bar you are creating as the name of the key. This can be one of
   the following values: 
   {00021494-0000-0000-C000-000000000046} Defines a horizontal Explorer Bar. 
   {00021493-0000-0000-C000-000000000046} Defines a vertical Explorer Bar. 
   
   The result should look like:

   HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021494-0000-0000-C000-000000000046}
   or  
   HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021493-0000-0000-C000-000000000046}
   
5. Delete following registry keys because they cache the ExplorerBar enum.

   HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
   Component Categories\{00021493-0000-0000-C000-000000000046}\Enum
   or
   HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
   Component Categories\{00021494-0000-0000-C000-000000000046}\Enum


6. Set the size of the Explorer Bar in the registry.

   HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer\Explorer Bars\<Your GUID>\BarSize

 

-----------------------------------------------------------------------------

D. Deploying the IE Explorer Bar with a setup project.

  (1) In CSIEExplorerBar, add an Installer class (named IEExplorerBarInstaller 
      in this code sample) to define the custom actions in the setup. The class derives from
      System.Configuration.Install.Installer. We use the custom actions to add/remove the IE 
	  Context Menu entries in registry and register/unregister the COM-visible classes in
	  the current managed assembly when user installs/uninstalls the component. 

    [RunInstaller(true), ComVisible(false)]
    public partial class IEExplorerBarInstaller : System.Configuration.Install.Installer
    {
        public IEExplorerBarInstaller()
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


  (2) To add a deployment project, on the File menu, point to Add, and then 
  click New Project. In the Add New Project dialog box, expand the Other 
  Project Types node, expand the Setup and Deployment Projects, click Visual 
  Studio Installer, and then click Setup Project. In the Name box, type 
  CSIEExplorerBarSetup(x86). Click OK to create the project. 
  In the Properties dialog of the setup project, make sure that the 
  TargetPlatform property is set to x86. This setup project is to be deployed 
  for 32bit IE on x86 or x64 Windows operating systems. 

  Right-click the setup project, and choose Add / Project Output ... 
  In the Add Project Output Group dialog box, CSIEExplorerBar will  
  be displayed in the Project list. Select Primary Output and click OK. VS
  will detect the dependencies of the CSIEExplorerBar, including .NET
  Framework 4.0 (Client Profile).

  Right-click the setup project, and choose View / Custom Actions. 
  In the Custom Actions Editor, right-click the root Custom Actions node. On 
  the Action menu, click Add Custom Action. In the Select Item in Project 
  dialog box, double-click the Application Folder. Select Primary output from 
  CSIEExplorerBar. This adds the custom actions that we defined 
  in IEExplorerBarInstaller of CSIEExplorerBar. 
  
  Build the setup project. If the build succeeds, you will get a .msi file 
  and a Setup.exe file. You can distribute them to your users to install or 
  uninstall this Explorer Bar. 

  (3) To deploy the Explorer Bar for 64bit IE on a x64 operating system, you 
  must create a new setup project (e.g. CSIEExplorerBarSetup(x64) 
  in this code sample), and set its TargetPlatform property to x64. 

  Although the TargetPlatform property is set to x64, the native shim 
  packaged with the .msi file is still a 32-bit executable. The Visual Studio 
  embeds the 32-bit version of InstallUtilLib.dll into the Binary table as 
  InstallUtil. So the custom actions will be run in the 32-bit, which is 
  unexpected in this code sample. To workaround this issue and ensure that 
  the custom actions run in the 64-bit mode, you either need to import the 
  appropriate bitness of InstallUtilLib.dll into the Binary table for the 
  InstallUtil record or - if you do have or will have 32-bit managed custom 
  actions add it as a new record in the Binary table and adjust the 
  CustomAction table to use the 64-bit Binary table record for 64-bit managed 
  custom actions. This blog article introduces how to do it manually with 
  Orca http://blogs.msdn.com/b/heaths/archive/2006/02/01/64-bit-managed-custom-actions-with-visual-studio.aspx

  In this code sample, we automate the modification of InstallUtil by using a 
  post-build javascript: Fix64bitInstallUtilLib.js. You can find the script 
  file in the CSIEExplorerBarSetup(x64) project folder. To 
  configure the script to run in the post-build event, you select the 
  CSIEExplorerBarSetup(x64) project in Solution Explorer, and 
  find the PostBuildEvent property in the Properties window. Specify its 
  value to be 
  
	"$(ProjectDir)Fix64bitInstallUtilLib.js" "$(BuiltOuputPath)" "$(ProjectDir)"

  Repeat the rest steps in (2) to add the project output, set the custom 
  actions, configure the prerequisites, and build the setup project.


/////////////////////////////////////////////////////////////////////////////
Diagnostic:

To debug Explorer Bar, you can attach to iexplorer.exe. 


///////////////////////////////////////////////////////////////////////////// 
 
References:

Creating Custom Explorer Bars, Tool Bands, and Desk Bands
http://msdn.microsoft.com/en-us/library/bb776819(VS.85).aspx

Adding Explorer Bars
http://msdn.microsoft.com/en-us/library/aa753590(VS.85).aspx

/////////////////////////////////////////////////////////////////////////////

