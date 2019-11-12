Imports System.Collections
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.ComponentModelHost 'scomponentmodel

Imports Microsoft.VisualStudio.Editor 'editor
Imports Microsoft.VisualStudio.Text 'text
Imports Microsoft.VisualStudio.Text.Operations 'EditorOperations
Imports Microsoft.VisualStudio.Text.Editor 'Text
Imports Microsoft.VisualStudio.Utilities ' for content type
Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.TextManager.Interop 'IVsTextView

Imports Microsoft.VisualStudio.OLE.Interop


Namespace Samples.EditorToolwindow

    ''' <summary>
    ''' This toolwindow hosts an editor inside. In order to successfully route commands and events to the editor, we have to implement
    ''' IOleCommandTarget ourselves as our base implementation will not forward commands to the hosted editor.
    ''' </summary>
    <Guid("e3e490c6-e7b5-425a-a44d-191d5c4a9e5b")>
    Public Class MyToolWindow
        Inherits ToolWindowPane
        Implements IOleCommandTarget

        Private viewAdapter As IVsTextView
        Private bufferAdapter As IVsTextBuffer

        Private m_EditorAdapterFactory As IVsEditorAdaptersFactoryService
        Private oleServiceProvider As Microsoft.VisualStudio.OLE.Interop.IServiceProvider
        Private bufferFactory As ITextBufferFactoryService

        ''' <summary>
        ''' Standard constructor for the tool window.
        ''' </summary>
        Public Sub New()
            MyBase.New(Nothing)

            ' Set the window title reading it from the resources.
            Me.Caption = Resources.ToolWindowTitle
            ' Set the image that will appear on the tab of the window frame
            ' when docked with an other window
            ' The resource ID correspond to the one defined in the resx file
            ' while the Index is the offset in the bitmap strip. Each image in
            ' the strip being 16x16.
            Me.BitmapResourceID = 301
            Me.BitmapIndex = 1
        End Sub

        ''' <summary>
        ''' This property returns the control that should be hosted in the Tool Window.
        ''' In this case we return the wpf Text View Host for the editor
        ''' </summary>
        Public Overrides Property Content As Object
            Get
                Return TextViewHost
            End Get

            Set(ByVal value As Object)
                'do nothing here.
            End Set
        End Property

        Private Sub InitializeEditor()
            Dim m_componentModel As IComponentModel = CType(Microsoft.VisualStudio.Shell.Package.GetGlobalService(GetType(SComponentModel)), IComponentModel)
            oleServiceProvider = CType(GetService(GetType(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)), Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
            bufferFactory = m_componentModel.GetService(Of ITextBufferFactoryService)()

            m_EditorAdapterFactory = m_componentModel.GetService(Of IVsEditorAdaptersFactoryService)()
            bufferAdapter = m_EditorAdapterFactory.CreateVsTextBufferAdapter(oleServiceProvider, bufferFactory.TextContentType)
            Dim result As Integer = bufferAdapter.InitializeContent("ed", 2)
            viewAdapter = m_EditorAdapterFactory.CreateVsTextViewAdapter(oleServiceProvider)
            CType(viewAdapter, IVsWindowPane).SetSite(oleServiceProvider)

            Dim _initView() As INITVIEW = {New INITVIEW}
            _initView(0).fSelectionMargin = 0
            _initView(0).fWidgetMargin = 0
            _initView(0).fVirtualSpace = 0
            _initView(0).fDragDropMove = 1
            _initView(0).fVirtualSpace = 0

            viewAdapter.Initialize(TryCast(bufferAdapter, IVsTextLines), IntPtr.Zero, CUInt(TextViewInitFlags.VIF_HSCROLL) Or CUInt(TextViewInitFlags3.VIF_NO_HWND_SUPPORT), _initView)
        End Sub

        'INSTANT VB NOTE: The variable textViewHost was renamed since Visual Basic does not allow class members with the same name:
        Private _textViewHost As IWpfTextViewHost

        ''' <summary>
        ''' Gets the editor wpf host that we can use as the tool windows content.
        ''' </summary>
        Public ReadOnly Property TextViewHost As IWpfTextViewHost
            Get
                If _textViewHost Is Nothing Then
                    InitializeEditor()
                    Dim data As IVsUserData = TryCast(viewAdapter, IVsUserData)

                    If data IsNot Nothing Then
                        Dim guid As Guid = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost
                        Dim obj As Object
                        Dim hr As Integer = data.GetData(guid, obj)
                        If (hr = Microsoft.VisualStudio.VSConstants.S_OK) AndAlso obj IsNot Nothing AndAlso TypeOf obj Is IWpfTextViewHost Then
                            _textViewHost = TryCast(obj, IWpfTextViewHost)
                        End If
                    End If
                End If

                Return _textViewHost
            End Get
        End Property

        Public Overrides Sub OnToolWindowCreated()
            ' Register key bindings to use in the editor
            Dim windowFrame = CType(Frame, IVsWindowFrame)
            Dim cmdUi As Guid = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory
            Dim hr As Integer = windowFrame.SetGuidProperty(CInt(__VSFPROPID.VSFPROPID_InheritKeyBindings), cmdUi)
            MyBase.OnToolWindowCreated()
        End Sub

        ''' <summary>
        ''' This override allows us to forward these messages to the editor instance as well
        ''' </summary>
        ''' <param name="m"></param>
        ''' <returns></returns>
        Protected Overrides Function PreProcessMessage(ByRef m As Message) As Boolean
            If Me.TextViewHost IsNot Nothing Then
                ' copy the Message into a MSG[] array, so we can pass
                ' it along to the active core editor's IVsWindowPane.TranslateAccelerator
                Dim pMsg(0) As MSG
                pMsg(0).hwnd = m.HWnd
                pMsg(0).message = CUInt(m.Msg)
                pMsg(0).wParam = m.WParam
                pMsg(0).lParam = m.LParam

                Dim vsWindowPane As IVsWindowPane = CType(Me.viewAdapter, IVsWindowPane)
                If 0 = vsWindowPane.TranslateAccelerator(pMsg) Then
                    Return True
                Else
                    Return False
                End If
            End If

            Return MyBase.PreProcessMessage(m)
        End Function

        Private Function Exec(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger, ByVal nCmdexecopt As UInteger, ByVal pvaIn As IntPtr, ByVal pvaOut As IntPtr) As Integer Implements IOleCommandTarget.Exec
            Dim hr As Integer = CInt(Fix(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED))

            ' if a core editor object has the focus defer call to it's
            ' IOleCommandTarget.QueryStatus implementation.
            If Me.viewAdapter IsNot Nothing Then
                Dim cmdTarget As IOleCommandTarget = CType(viewAdapter, IOleCommandTarget)
                hr = cmdTarget.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            End If

            Return hr
        End Function

        Private Function QueryStatus(ByRef pguidCmdGroup As Guid, ByVal cCmds As UInteger, ByVal prgCmds() As OLECMD, ByVal pCmdText As IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
            Dim hr As Integer = CInt(Fix(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED))

            ' if a core editor object has the focus defer call to it's
            ' IOleCommandTarget.QueryStatus implementation.
            If Me.viewAdapter IsNot Nothing Then
                Dim cmdTarget As IOleCommandTarget = CType(viewAdapter, IOleCommandTarget)
                hr = cmdTarget.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText)
            End If

            Return hr
        End Function

    End Class
End Namespace


