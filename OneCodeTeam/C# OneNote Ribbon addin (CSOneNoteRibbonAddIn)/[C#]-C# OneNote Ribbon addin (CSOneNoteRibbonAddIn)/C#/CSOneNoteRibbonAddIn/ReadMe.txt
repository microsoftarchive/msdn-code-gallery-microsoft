=============================================================================
          APPLICATION : CSOneNoteRibbonAddIn Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary: 

The code sample demonstrates a OneNote 2010 COM add-in that implements
IDTExtensibility2. 
The add-in also supports customizing the Ribbon by implementing the
IRibbonExtensibility interface.
In addition, the sample also demonstrates the usage of the
OneNote 2010 Object Model.

CSOneNoteRibbonAddIn: The project which generates CSOneNoteRibbonAddIn.dll
for project CSOneNoteRibbonAddInSetup.

CSOneNoteRibbonAddInSetup: Setup project which generates setup.exe and
CSOneNoteRibbonAddInSetup.msi for OneNote 2010.

/////////////////////////////////////////////////////////////////////////////
Prerequisite:

You must run this code sample on a computer that has Microsoft OneNote 2010 
installed.

/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the CSOneNoteRibbonAddIn
sample.

Step1. Open the solution file CSOneNoteRibbonAddIn.sln as Administrator;

Step2. Build CSOneNoteRibbonAddIn first, then build setup project
CSOneNoteRibbonAddInSetup in Visual Studio 2010, then you will get a 
bootstrapper setup.exe and the application CSOneNoteRibbonAddInSetup.msi;

Step3.Install setup.exe;

Step4. Open OneNote 2010 and you will see three MesseageBoxs:
MessageBox.Show("CSOneNoteRibbonAddIn OnConnection"),
MessageBox.Show("CSOneNoteRibbonAddIn OnAddInsUpdate"),
MessageBox.Show("CSOneNoteRibbonAddIn OnStartupComplete");

Step5. Click Review Tab and you will see Statistics group which contains a
button ShowForm that the add-in added to the Ribbon. Click the ShowForm 
button, a window form will pop up and you can click the button on the form
to get the title of the current page;

Step6. When closing OneNote 2010, you will see two MessageBoxs:
MessageBox.Show("CSOneNoteRibbonAddIn OnBeginShutdown"),
MessageBox.Show("CSOneNoteRibbonAddIn OnDisconnection")

/////////////////////////////////////////////////////////////////////////////
Creation:

Step1. Create a Shared Add-in Extensibility,and the shared Add-in Wizard is 
as follows:
	Open the Visual Studio 2010 as Administrator;
	Create an Shared Add-in (Other Project Types->Extensibility) 
	using Visual C#; 
	choose Microsoft Access	(since there doesn't exist Microsoft OneNote
	option to choose, you can choose Microsoft Access first, but remeber 
	to modify Setup project registry HKCU to be OneNote);
	fill name and description of the Add-in;
	select the two checkboxes in Choose Add-in Options.

Step2. Modify the CSOneNoteRibbonAddInSetup Registry 
(right click Project->View->Registry) 
[HKEY_CLASSES_ROOT\AppID\{Your GUID}]
"DllSurrogate"=""
[HKEY_CLASSES_ROOT\CLSID\{Your GUID}]
"AppID"="{Your GUID}"

[HKEY_CURRENT_USER\Software\Microsoft\Office\OneNote\AddIns\
CSOneNoteRibbonAddIn.Connect]
"LoadBehavior"=dword:00000003
"FriendlyName"="CSOneNoteRibbionAddIn"
"Description"="OneNote2010 Ribbon AddIn Sample"

[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\AppID\{Your GUID}]
"DllSurrogate"=""
[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{Your GUID}]
"AppID"="{Your GUID}"

Step3. Add customUI.xml and showform.png resource files into
 CSOneNoteRibbonAddIn project.

Step4. Make Connect class inherent IRibbonExtensibility and implement the method
GetCustomUI.
       
	/// <summary>
        ///     Loads the XML markup from an XML customization file 
        ///     that customizes the Ribbon user interface.
        /// </summary>
        /// <param name="RibbonID">The ID for the RibbonX UI</param>
        /// <returns>string</returns>
        public string GetCustomUI(string RibbonID)
        {
            return Properties.Resources.customUI;
        }

Step5. Implement the methods OnGetImage and ShowForm according to the customUI.xml
content.

        /// <summary>
        ///     Implements the OnGetImage method in customUI.xml
        /// </summary>
        /// <param name="imageName">the image name in customUI.xml</param>
        /// <returns>memory stream contains image</returns>
        public IStream OnGetImage(string imageName)
        {
            MemoryStream stream = new MemoryStream();
            if (imageName == "showform.png")
            {
                Resources.ShowForm.Save(stream, ImageFormat.Png);
            }

            return new ReadOnlyIStreamWrapper(stream);
        }

        /// <summary>
        ///     show Windows Form method
        /// </summary>
        /// <param name="control">Represents the object passed into every
        /// Ribbon user interface (UI) control's callback procedure.</param>
        public void ShowForm(IRibbonControl control)
        {
            OneNote.Window context = control.Context as OneNote.Window;
            CWin32WindowWrapper owner =
                new CWin32WindowWrapper((IntPtr)context.WindowHandle);
            TestForm form = new TestForm(applicationObject as OneNote.Application);
            form.ShowDialog(owner);

            form.Dispose();
            form = null;
            context = null;
            owner = null;           
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

Step6. Add ReadOnlyIStreamWrapper class and CWin32WindowWrapper class into
CSOneNoteRibbonAddIn project and add Windows Form for testing to open.

Step7. Add the follwing methods in the TestForm which using OneNote 2010 Object Model
to show the title of the current page:

 	/// <summary>
        /// Get the title of the page
        /// </summary>
        /// <returns>string</returns>
        private string GetPageTitle()
        {
            string pageXmlOut = GetActivePageContent();        
            var doc = XDocument.Parse(pageXmlOut);
            string pageTitle = "";
            pageTitle = doc.Descendants().FirstOrDefault().Attribute("ID").NextAttribute.Value;

            return pageTitle;
        }

        /// <summary>
        /// Get active page content and output the xml string
        /// </summary>
        /// <returns>string</returns>
        private string GetActivePageContent()
        {
            string activeObjectID = this.GetActiveObjectID(_ObjectType.Page);
            string pageXmlOut = "";
            oneNoteApp.GetPageContent(activeObjectID,out pageXmlOut);

            return pageXmlOut;
        }

        /// <summary>
        /// Get ID of current page 
        /// </summary>
        /// <param name="obj">_Object Type</param>
        /// <returns>current page Id</returns>
        private string GetActiveObjectID(_ObjectType obj)
        {
            string currentPageId = "";
            uint count = oneNoteApp.Windows.Count;
            foreach (OneNote.Window window in oneNoteApp.Windows)
            {
                if (window.Active)
                {
                    switch (obj)
                    {
                        case _ObjectType.Notebook:
                            currentPageId = window.CurrentNotebookId;
                            break; 
                        case _ObjectType.Section:
                            currentPageId = window.CurrentSectionId;
                            break; 
                        case _ObjectType.SectionGroup:
                            currentPageId = window.CurrentSectionGroupId;
                            break; 
                    }

                    currentPageId = window.CurrentPageId;
                }
            }

            return currentPageId;

        }

        /// <summary>
        /// Nested types
        /// </summary>
        private enum _ObjectType
        {
            Notebook,
            Section,
            SectionGroup,
            Page,
            SelectedPages,
            PageObject
        } 

Step8. Register the output assembly as COM component.

To do this, click Project->Project Properties... button. And in the project
properties page, navigate to Build tab and check the box "Register for COM
interop".

Step8. Build your CSOneNoteRibbonAddIn project, 
and then build CSOneNoteRibbonAddInSetup project to generate setup.exe and
CSOneNoteRibbonAddInSetup.msi.



/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Creating OneNote 2010 Extensions with the OneNote Object Model

http://msdn.microsoft.com/en-us/magazine/ff796230.aspx

Jeff Cardon's Blog
http://blogs.msdn.com/b/onenotetips/


/////////////////////////////////////////////////////////////////////////////

