using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Samples.VisualStudio.IDE.WinformsControlsInstaller
{
    /// <summary>
    /// This is a simple custom control that will appear on the Winforms designer toolbox.
    /// </summary>
    public class MyCustomTextBox : TextBox
    {
        public MyCustomTextBox()
        {
            this.Multiline = true;
            this.Size = new System.Drawing.Size(100, 50);
        }
    }

    /// <summary>
    /// This Winforms control has a custom ToolboxItem that displays a wizard.
    /// To mark this control as not visible uncomment the [ToolboxItem(false)]
    /// and comment [ToolboxItem(typeof(MyToolboxItem))] attribute. 
    /// </summary>
    // [ToolboxItem(false)]
    [ToolboxItem(typeof(MyToolboxItem))]
    public class MyCustomTextBoxWithPopup : TextBox
    {
        public MyCustomTextBoxWithPopup()
        {
        }
    }

    /// <summary>
    /// This custom ToolboxItem displays a simple dialog asking whether to 
    /// initialize a certain value.
    /// </summary>
    [Serializable]
    class MyToolboxItem : ToolboxItem
    {
        public MyToolboxItem()
        {
        }

        private MyToolboxItem(SerializationInfo info, StreamingContext context)
        {
            Deserialize(info, context);
        }

        const int IDYES = 6;

        protected override IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            return RunWizard(host, base.CreateComponentsCore(host, defaultValues));
        }

        /// <summary>
        /// This method sets various values on the newly created component.
        /// </summary>
        private IComponent[] RunWizard(IDesignerHost host, IComponent[] comps)
        {
            DialogResult result = DialogResult.No;
            IVsUIShell uiShell = null;
            if (host != null)
            {
                uiShell = (IVsUIShell)host.GetService(typeof(IVsUIShell));
            }

            //always use the UI shell service to show a messagebox if possible
            if (uiShell != null)
            {
                int nResult = 0;
                Guid emptyGuid = Guid.Empty;

                uiShell.ShowMessageBox(0, ref emptyGuid, "Question", "Do you want to set the Text property?", null, 0,
                        OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND, OLEMSGICON.OLEMSGICON_QUERY, 0, out nResult);

                if (nResult == IDYES)
                {
                    result = DialogResult.Yes;
                }
            }
            else
            {
                result = MessageBox.Show("Do you want to set the Text property?", "Question", MessageBoxButtons.YesNo);
            }

            if (result == DialogResult.Yes)
            {
                if (comps.Length > 0)
                {
                    //use Types from the ITypeResolutionService.  Do not use locally defined types.
                    ITypeResolutionService typeResolver = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
                    if (typeResolver != null)
                    {
                        Type t = typeResolver.GetType(typeof(MyCustomTextBoxWithPopup).FullName);
                        //check to ensure we got the right Type
                        if (t != null && comps[0].GetType().IsAssignableFrom(t))
                        {
                            //Use a property descriptor instead of direct property access.
                            //This will allow the change to appear in the undo stack and it will get
                            //serialized correctly.
                            PropertyDescriptor pd = TypeDescriptor.GetProperties(comps[0])["Text"];
                            if (pd != null)
                            {
                                pd.SetValue(comps[0], "Text Property was initialized!");
                            }
                        }
                    }
                }
            }
            return comps;
        }
    }

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidWinformsControlsInstallerPkgString)]
    [ProvideToolboxItems(4)]
    public sealed class PackageWinformsToolbox : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// </summary>
        public PackageWinformsToolbox()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
            this.ToolboxInitialized += new EventHandler(PackageWinformsToolbox_ToolboxInitialized);
            this.ToolboxUpgraded += new EventHandler(PackageWinformsToolbox_ToolboxUpgraded);
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that relies on services provided by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();
        }
        #endregion

        /// <summary>
        /// This method is called when the toolbox content version (the parameter to the ProvideToolboxItems
        /// attribute) changes.  This tells Visual Studio that items may have changed 
        /// and need to be reinstalled.
        /// </summary>
        void PackageWinformsToolbox_ToolboxUpgraded(object sender, EventArgs e)
        {
            RemoveToolboxItems();
            InstallToolboxItems();
        }

        /// <summary>
        /// This method will add items to the toolbox.  It is called the first time the toolbox
        /// is used after this package has been installed.
        /// </summary>
        void PackageWinformsToolbox_ToolboxInitialized(object sender, EventArgs e)
        {
            InstallToolboxItems();
        }

        /// <summary>
        /// Removes toolbox items
        /// </summary>
        void RemoveToolboxItems()
        {
            Assembly a = typeof(PackageWinformsToolbox).Assembly;

            IToolboxService tbxService = (IToolboxService)GetService(typeof(IToolboxService));

            foreach (ToolboxItem item in ToolboxService.GetToolboxItems(a, null))
            {
                tbxService.RemoveToolboxItem(item);
            }
        }

        /// <summary>
        /// Installs toolbox items
        /// </summary>
        void InstallToolboxItems()
        {
            /// For demonstration purposes, this assembly includes toolbox items and loads them from itself.
            /// It is of course possible to load toolbox items from a different assembly by either:
            /// a)  loading the assembly yourself and calling ToolboxService.GetToolboxItems
            /// b)  call AssemblyName.GetAssemblyName("...") and then ToolboxService.GetToolboxItems(assemblyName)
            Assembly a = typeof(PackageWinformsToolbox).Assembly;

            IToolboxService tbxService = (IToolboxService)GetService(typeof(IToolboxService));

            foreach (ToolboxItem item in ToolboxService.GetToolboxItems(a, null))
            {
                /// This tab name can be whatever you would like it to be.
                tbxService.AddToolboxItem(item, "MyOwnTab");
            }
        }

    }
}
