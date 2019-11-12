Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.Win32
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell
Imports System.Windows.Forms
Imports System.Drawing.Design
Imports System.ComponentModel
Imports System.Reflection
Imports System.Runtime.Serialization

''' <summary>
''' This is a simple custom control that will appear on the Windows Forms designer toolbox.
''' </summary>
Public Class MyCustomTextBox
    Inherits TextBox

    Public Sub New()
        Me.Multiline = True
        Me.Size = New System.Drawing.Size(100, 50)
    End Sub
End Class

''' <summary>
''' This Windows Forms control has a custom ToolboxItem that displays a wizard.
''' To mark this control as not visible uncomment the [ToolboxItem(false)]
''' and comment [ToolboxItem(typeof(MyToolboxItem))] attribute. 
''' </summary>
' [ToolboxItem(false)]
<ToolboxItem(GetType(MyToolboxItem))> _
Public Class MyCustomTextBoxWithPopup
    Inherits TextBox

    Public Sub New()
    End Sub
End Class

''' <summary>
''' This custom ToolboxItem displays a simple dialog asking whether to 
''' initialize a certain value.
''' </summary>
<Serializable()> _
Class MyToolboxItem
    Inherits ToolboxItem

    Const IDYES As Integer = 6

    Public Sub New()
    End Sub

    Private Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        Deserialize(info, context)
    End Sub

    Protected Overloads Overrides Function CreateComponentsCore(ByVal host As IDesignerHost, ByVal defaultValues As IDictionary) As IComponent()
        Return RunWizard(host, MyBase.CreateComponentsCore(host, defaultValues))
    End Function

    ''' <summary>
    ''' This method sets various values on the newly created component.
    ''' </summary>
    Private Function RunWizard(ByVal host As IDesignerHost, ByVal comps As IComponent()) As IComponent()
        Dim result As DialogResult = DialogResult.No
        Dim uiShell As IVsUIShell = Nothing
        If Not host Is Nothing Then
            uiShell = DirectCast(host.GetService(GetType(IVsUIShell)), IVsUIShell)
        End If

        ' Always use the UI shell service to show a message box if possible
        If Not uiShell Is Nothing Then
            Dim nResult As Integer = 0
            Dim emptyGuid As Guid = Guid.Empty

            uiShell.ShowMessageBox(0, emptyGuid, "Question", "Do you want to set the Text property?", Nothing, 0, _
             OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND, OLEMSGICON.OLEMSGICON_QUERY, 0, nResult)

            If nResult = IDYES Then
                result = DialogResult.Yes
            End If
        Else
            result = MessageBox.Show("Do you want to set the Text property?", "Question", MessageBoxButtons.YesNo)
        End If

        If result = DialogResult.Yes Then
            If comps.Length > 0 Then
                ' Use types from the ITypeResolutionService.  Do not use locally defined types.
                Dim typeResolver As ITypeResolutionService = DirectCast(host.GetService(GetType(ITypeResolutionService)), ITypeResolutionService)
                If Not typeResolver Is Nothing Then
                    ' Check to ensure we got the right Type
                    Dim t As Type = typeResolver.GetType(GetType(MyCustomTextBoxWithPopup).FullName)

                    If Not t Is Nothing AndAlso comps(0).GetType().IsAssignableFrom(t) Then
                        ' Use a property descriptor instead of direct property access.
                        ' This will allow the change to appear in the undo stack and it will get
                        ' serialized correctly.
                        Dim pd As PropertyDescriptor = TypeDescriptor.GetProperties(comps(0))("Text")
                        If Not pd Is Nothing Then
                            pd.SetValue(comps(0), "Text Property was initialized!")
                        End If
                    End If
                End If
            End If
        End If

        Return comps
    End Function
End Class

''' <summary>
''' This is the class that implements the package exposed by this assembly.
'''
''' The minimum requirement for a class to be considered a valid package for Visual Studio
''' is to implement the IVsPackage interface and register itself with the shell.
''' This package uses the helper classes defined inside the Managed Package Framework (MPF)
''' to do it: it derives from the Package class that provides the implementation of the 
''' IVsPackage interface and uses the registration attributes defined in the framework to 
''' register itself and its components with the shell.
''' </summary>
' The PackageRegistration attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class
' is a package.
'
' The InstalledProductRegistration attribute is used to register the information needed to show this package
' in the Help/About dialog of Visual Studio.

<PackageRegistration(UseManagedResourcesOnly:=True), _
InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400), _
Guid(GuidList.guidWinformsControlsInstallerPkgString), _
ProvideToolboxItems(4)>
Public NotInheritable Class WinformsControlsInstallerPackage
    Inherits Package

    ''' <summary>
    ''' Default constructor of the package.
    ''' </summary>
    Public Sub New()
        Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", Me.GetType().Name))
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Overridden Package Implementation
#Region "Package Members"

    ''' <summary>
    ''' Initialization of the package; this method is called right after the package is sited, so this is the place
    ''' where you can put all the initialization code that rely on services provided by VisualStudio.
    ''' </summary>
    Protected Overrides Sub Initialize()
        Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", Me.GetType().Name))
        MyBase.Initialize()
    End Sub
#End Region

    ''' <summary>
    ''' This method is called when the toolbox content version (the parameter to the ProvideToolboxItems
    ''' attribute) changes.  This tells Visual Studio that items may have changed 
    ''' and need to be reinstalled.
    ''' </summary>
    Sub WinformsControlsInstaller_ToolboxUpgraded(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ToolboxUpgraded
        RemoveToolboxItems()
        InstallToolboxItems()
    End Sub

    ''' <summary>
    ''' This method will add items to the toolbox.  It is called the first time the toolbox
    ''' is used after this package has been installed.
    ''' </summary>
    Sub WinformsControlsInstaller_ToolboxInitialized(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ToolboxInitialized
        InstallToolboxItems()
    End Sub

    ''' <summary>
    ''' Removes toolbox items
    ''' </summary>
    Sub RemoveToolboxItems()
        Dim a As Assembly = GetType(WinformsControlsInstallerPackage).Assembly

        Dim tbxService As IToolboxService = DirectCast(GetService(GetType(IToolboxService)), IToolboxService)

        For Each item As ToolboxItem In ToolboxService.GetToolboxItems(a, Nothing)
            tbxService.RemoveToolboxItem(item)
        Next item
    End Sub

    ''' <summary>
    ''' Installs toolbox items
    ''' </summary>
    Sub InstallToolboxItems()
        ''' For demonstration purposes, this assembly includes toolbox items and loads them from itself.
        ''' It is of course possible to load toolbox items from a different assembly by either:
        ''' a)  loading the assembly yourself and calling ToolboxService.GetToolboxItems
        ''' b)  call AssemblyName.GetAssemblyName("...") and then ToolboxService.GetToolboxItems(assemblyName)
        Dim a As Assembly = GetType(WinformsControlsInstallerPackage).Assembly

        Dim tbxService As IToolboxService = DirectCast(GetService(GetType(IToolboxService)), IToolboxService)

        For Each item As ToolboxItem In ToolboxService.GetToolboxItems(a, Nothing)
            ''' This tab name can be whatever you would like it to be.
            tbxService.AddToolboxItem(item, "MyOwnTab")
        Next
    End Sub
End Class
