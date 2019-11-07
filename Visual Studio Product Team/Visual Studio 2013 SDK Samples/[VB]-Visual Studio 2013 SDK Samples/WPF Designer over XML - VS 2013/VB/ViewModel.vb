'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Collections
Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.Xml.Serialization
Imports EnvDTE
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Package
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.XmlEditor
Imports VSLangProj
Imports System.Text.RegularExpressions
Imports System.Text

Namespace Microsoft.VsTemplateDesigner
	''' <summary>
	''' ViewModel is where the interesting portion of the VsTemplate Designer lives. The View binds to an instance of this class.
	''' 
	''' The View binds the various designer controls to the methods derived from IViewModel that get and set values in the XmlModel.
	''' The ViewModel and an underlying XmlModel manage how an IVsTextBuffer is shared between the designer and the XML editor (if opened).
	''' </summary>
	Public Class ViewModel
        Implements IViewModel, IDataErrorInfo, INotifyPropertyChanged

        Private Const MaxIdLength As Integer = 100
        Private Const MaxProductNameLength As Integer = 60
        Private Const MaxDescriptionLength As Integer = 1024

        Private _xmlModel As XmlModel
        Private _xmlStore As XmlStore
        Private _vstemplateModel As VSTemplate

        Private _serviceProvider As IServiceProvider
        Private _buffer As IVsTextLines

        Private _synchronizing As Boolean
        Private _dirtyTime As Long
        Private _editingScopeCompletedHandler As EventHandler(Of XmlEditingScopeEventArgs)
        Private _undoRedoCompletedHandler As EventHandler(Of XmlEditingScopeEventArgs)
        Private _bufferReloadedHandler As EventHandler

        Private _xmlLanguageService As LanguageService

        Public Sub New(ByVal xmlStore_Renamed As XmlStore, ByVal xmlModel_Renamed As XmlModel,
                       ByVal provider As IServiceProvider, ByVal buffer As IVsTextLines)
            If xmlModel_Renamed Is Nothing Then
                Throw New ArgumentNullException("xmlModel")
            End If
            If xmlStore_Renamed Is Nothing Then
                Throw New ArgumentNullException("xmlStore")
            End If
            If provider Is Nothing Then
                Throw New ArgumentNullException("provider")
            End If
            If buffer Is Nothing Then
                Throw New ArgumentNullException("buffer")
            End If

            Me.BufferDirty = False
            Me.DesignerDirty = False

            Me._serviceProvider = provider
            Me._buffer = buffer

            Me._xmlStore = xmlStore_Renamed
            ' OnUnderlyingEditCompleted
            _editingScopeCompletedHandler = New EventHandler(Of XmlEditingScopeEventArgs)(AddressOf OnUnderlyingEditCompleted)
            AddHandler Me._xmlStore.EditingScopeCompleted, _editingScopeCompletedHandler
            ' OnUndoRedoCompleted
            _undoRedoCompletedHandler = New EventHandler(Of XmlEditingScopeEventArgs)(AddressOf OnUndoRedoCompleted)
            AddHandler Me._xmlStore.UndoRedoCompleted, _undoRedoCompletedHandler

            Me._xmlModel = xmlModel_Renamed
            ' BufferReloaded
            _bufferReloadedHandler = New EventHandler(AddressOf BufferReloaded)

            AddHandler Me._xmlModel.BufferReloaded, _bufferReloadedHandler

            LoadModelFromXmlModel()
        End Sub

        Public Sub Close() Implements IViewModel.Close
            'Unhook the events from the underlying XmlStore/XmlModel
            If _xmlStore IsNot Nothing Then
                RemoveHandler Me._xmlStore.EditingScopeCompleted, _editingScopeCompletedHandler
                RemoveHandler Me._xmlStore.UndoRedoCompleted, _undoRedoCompletedHandler
            End If
            If Me._xmlModel IsNot Nothing Then
                RemoveHandler Me._xmlModel.BufferReloaded, _bufferReloadedHandler
            End If
        End Sub

        ''' <summary>
        ''' Property indicating if there is a pending change in the underlying text buffer
        ''' that needs to be reflected in the ViewModel.
        ''' 
        ''' Used by DoIdle to determine if we need to sync.
        ''' </summary>
        Private Property BufferDirty() As Boolean

        ''' <summary>
        ''' Property indicating if there is a pending change in the ViewModel that needs to 
        ''' be committed to the underlying text buffer.
        ''' 
        ''' Used by DoIdle to determine if we need to sync.
        ''' </summary>
        Public Property DesignerDirty() As Boolean Implements IViewModel.DesignerDirty

        ''' <summary>
        ''' We must not try and update the XDocument while the XML Editor is parsing as this may cause
        ''' a deadlock in the XML Editor!
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property IsXmlEditorParsing() As Boolean
            Get
                Dim langsvc As LanguageService = GetXmlLanguageService()
                Return If(langsvc IsNot Nothing, langsvc.IsParsing, False)
            End Get
        End Property

        ''' <summary>
        ''' Called on idle time. This is when we check if the designer is out of sync with the underlying text buffer.
        ''' </summary>
        Public Sub DoIdle() Implements IViewModel.DoIdle
            If BufferDirty OrElse DesignerDirty Then
                Dim delay As Integer = 100

                If (Environment.TickCount - _dirtyTime) > delay Then
                    ' Must not try and sync while XML editor is parsing otherwise we just confuse matters.
                    If IsXmlEditorParsing Then
                        _dirtyTime = Environment.TickCount
                        Return
                    End If

                    'If there is contention, give the preference to the designer.
                    If DesignerDirty Then
                        SaveModelToXmlModel(Resources.SynchronizeBuffer)
                        'We don't do any merging, so just overwrite whatever was in the buffer.
                        BufferDirty = False
                    ElseIf BufferDirty Then
                        LoadModelFromXmlModel()
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Load the model from the underlying text buffer.
        ''' </summary>
        Private Sub LoadModelFromXmlModel()
            Try
                Dim serializer As New XmlSerializer(GetType(VSTemplate))

                Using reader As XmlReader = GetParseTree().CreateReader()
                    _vstemplateModel = CType(serializer.Deserialize(reader), VSTemplate)
                End Using

                If _vstemplateModel Is Nothing OrElse _vstemplateModel.TemplateData Is Nothing Then
                    Throw New Exception(Resources.InvalidVsTemplateData)
                End If
            Catch e As Exception
                'Display error message
                ErrorHandler.ThrowOnFailure(VsShellUtilities.ShowMessageBox(_serviceProvider,
                                                                            Resources.InvalidVsTemplateData & e.Message,
                                                                            Resources.ErrorMessageBoxTitle,
                                                                            OLEMSGICON.OLEMSGICON_CRITICAL,
                                                                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                                                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST))
            End Try

            BufferDirty = False

            ' Update the Designer View
            RaiseEvent ViewModelChanged(Me, New EventArgs())
        End Sub


        ''' <summary>
        ''' Get an up to date XML parse tree from the XML Editor.
        ''' </summary>
        Private Function GetParseTree() As XDocument
            Dim langsvc As LanguageService = Me.GetXmlLanguageService()

            ' don't crash if the language service is not available
            If langsvc IsNot Nothing Then
                Dim src As Source = langsvc.GetSource(_buffer)

                ' We need to access this method to get the most up to date parse tree.
                ' public virtual XmlDocument GetParseTree(Source source, IVsTextView view, int line, int col, ParseReason reason) {
                Dim mi As MethodInfo = langsvc.GetType().GetMethod("GetParseTree")
                Dim line = 0, col = 0
                mi.Invoke(langsvc, New Object() {src, Nothing, line, col, ParseReason.Check})
            End If

            ' Now the XmlDocument should be up to date also.
            Return _xmlModel.Document
        End Function

        ''' <summary>
        ''' Get the XML Editor language service
        ''' </summary>
        ''' <returns></returns>
        Private Function GetXmlLanguageService() As LanguageService
            If _xmlLanguageService Is Nothing Then
                Dim vssp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = TryCast(_serviceProvider.GetService(GetType(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)), Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
                Dim xmlEditorGuid As New Guid("f6819a78-a205-47b5-be1c-675b3c7f0b8e")
                Dim iunknown As New Guid("00000000-0000-0000-C000-000000000046")
                Dim ptr As IntPtr
                If ErrorHandler.Succeeded(vssp.QueryService(xmlEditorGuid, iunknown, ptr)) Then
                    Try
                        _xmlLanguageService = TryCast(Marshal.GetObjectForIUnknown(ptr), LanguageService)
                    Finally
                        Marshal.Release(ptr)
                    End Try
                End If
            End If
            Return _xmlLanguageService
        End Function

        ''' <summary>
        ''' Reformat the text buffer
        ''' </summary>
        Private Sub FormatBuffer(ByVal src As Source)
            Using edits As New EditArray(src, Nothing, False, Resources.ReformatBuffer)
                Dim span As TextSpan = src.GetDocumentSpan()
                src.ReformatSpan(edits, span)
            End Using
        End Sub

        ''' <summary>
        ''' Get the XML Editor Source object for this document.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetSource() As Source
            Dim langsvc As LanguageService = GetXmlLanguageService()
            If langsvc Is Nothing Then
                Return Nothing
            End If
            Dim src As Source = langsvc.GetSource(_buffer)
            Return src
        End Function

        ''' <summary>
        ''' This method is called when it is time to save the designer values to the
        ''' underlying buffer.
        ''' </summary>
        ''' <param name="undoEntry"></param>
        Private Sub SaveModelToXmlModel(ByVal undoEntry As String)
            Dim langsvc As LanguageService = GetXmlLanguageService()

            Try
                'We can't edit this file (perhaps the user cancelled a SCC prompt, etc...)
                If Not CanEditFile() Then
                    DesignerDirty = False
                    BufferDirty = True
                    Throw New Exception()
                End If

                'PopulateModelFromReferencesBindingList();
                'PopulateModelFromContentBindingList();

                Dim serializer As New XmlSerializer(GetType(VSTemplate))
                Dim documentFromDesignerState As New XDocument()
                Using w As XmlWriter = documentFromDesignerState.CreateWriter()
                    serializer.Serialize(w, _vstemplateModel)
                End Using

                _synchronizing = True
                Dim document As XDocument = GetParseTree()
                Dim src As Source = GetSource()
                If src Is Nothing OrElse langsvc Is Nothing Then
                    Return
                End If

                langsvc.IsParsing = True ' lock out the background parse thread.

                ' Wrap the buffer sync and the formatting in one undo unit.
                Using ca As New CompoundAction(src, Resources.SynchronizeBuffer)
                    Using scope As XmlEditingScope = _xmlStore.BeginEditingScope(Resources.SynchronizeBuffer, Me)
                        'Replace the existing XDocument with the new one we just generated.
                        document.Root.ReplaceWith(documentFromDesignerState.Root)
                        scope.Complete()
                    End Using
                    ca.FlushEditActions()
                    FormatBuffer(src)
                End Using
                DesignerDirty = False
            Catch e1 As Exception
                ' if the synchronization fails then we'll just try again in a second.
                _dirtyTime = Environment.TickCount
            Finally
                langsvc.IsParsing = False
                _synchronizing = False
            End Try
        End Sub

        ''' <summary>
        ''' Fired when all controls should be re-bound.
        ''' </summary>
        Public Event ViewModelChanged As EventHandler Implements IViewModel.ViewModelChanged

        Private Sub BufferReloaded(ByVal sender As Object, ByVal e As EventArgs)
            If Not _synchronizing Then
                BufferDirty = True
                _dirtyTime = Environment.TickCount
            End If
        End Sub

        ''' <summary>
        ''' Handle undo/redo completion event.  This happens when the user invokes Undo/Redo on a buffer edit operation.
        ''' We need to resync when this happens.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub OnUndoRedoCompleted(ByVal sender As Object, ByVal e As XmlEditingScopeEventArgs)
            If Not _synchronizing Then
                BufferDirty = True
                _dirtyTime = Environment.TickCount
            End If
        End Sub

        ''' <summary>
        ''' Handle edit scope completion event.  This happens when the XML editor buffer decides to update
        ''' it's XDocument parse tree.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub OnUnderlyingEditCompleted(ByVal sender As Object, ByVal e As XmlEditingScopeEventArgs)
            If e.EditingScope.UserState IsNot Me AndAlso (Not _synchronizing) Then
                BufferDirty = True
                _dirtyTime = Environment.TickCount
            End If
        End Sub

#Region "Source Control"

        Private _canEditFile? As Boolean
        Private _gettingCheckoutStatus As Boolean

        ''' <summary>
        ''' This function asks the QueryEditQuerySave service if it is possible to edit the file.
        ''' This can result in an automatic checkout of the file and may even prompt the user for
        ''' permission to checkout the file.  If the user says no or the file cannot be edited 
        ''' this returns false.
        ''' </summary>
        Private Function CanEditFile() As Boolean
            ' Cache the value so we don't keep asking the user over and over.
            If _canEditFile.HasValue Then
                Return CBool(_canEditFile)
            End If

            ' Check the status of the recursion guard
            If _gettingCheckoutStatus Then
                Return False
            End If

            _canEditFile = False ' assume the worst
            Try
                ' Set the recursion guard
                _gettingCheckoutStatus = True

                ' Get the QueryEditQuerySave service
                Dim queryEditQuerySave As IVsQueryEditQuerySave2 = TryCast(_serviceProvider.GetService(GetType(SVsQueryEditQuerySave)), IVsQueryEditQuerySave2)

                Dim filename As String = _xmlModel.Name

                ' Now call the QueryEdit method to find the edit status of this file
                Dim documents() As String = {filename}
                Dim result As UInteger
                Dim outFlags As UInteger

                ' Note that this function can popup a dialog to ask the user to checkout the file.
                ' When this dialog is visible, it is possible to receive other request to change
                ' the file and this is the reason for the recursion guard
                Dim hr As Integer = queryEditQuerySave.QueryEditFiles(0, 1, documents, Nothing, Nothing, result, outFlags) ' Additional flags -  result of the checkout -  Input array of VSQEQS_FILE_ATTRIBUTE_DATA -  Input flags -  Files to edit -  Number of elements in the array -  Flags
                If ErrorHandler.Succeeded(hr) AndAlso (result = CUInt(tagVSQueryEditResult.QER_EditOK)) Then
                    ' In this case (and only in this case) we can return true from this function
                    _canEditFile = True
                End If
            Finally
                _gettingCheckoutStatus = False
            End Try
            Return CBool(_canEditFile)
        End Function

#End Region

#Region "IViewModel methods"

        Public ReadOnly Property TemplateData() As VSTemplateTemplateData Implements IViewModel.TemplateData
            Get
                Return _vstemplateModel.TemplateData
            End Get
        End Property

        Public ReadOnly Property TemplateContent() As VSTemplateTemplateContent Implements IViewModel.TemplateContent
            Get
                Return _vstemplateModel.TemplateContent
            End Get
        End Property

        Public Property TemplateID() As String Implements IViewModel.TemplateID
            Get
                Return _vstemplateModel.TemplateData.TemplateID
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.TemplateID <> value Then
                    _vstemplateModel.TemplateData.TemplateID = value
                    DesignerDirty = True
                    NotifyPropertyChanged("TemplateID")
                End If
            End Set
        End Property

        Public Property Name() As String Implements IViewModel.Name
            Get
                If Not IsNameEnabled Then
                    Return _vstemplateModel.TemplateData.Name.Package & " " & _vstemplateModel.TemplateData.Name.ID
                End If
                Return _vstemplateModel.TemplateData.Name.Value
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.Name.Value <> value Then
                    _vstemplateModel.TemplateData.Name.Value = value
                    DesignerDirty = True
                    NotifyPropertyChanged("Name")
                End If
            End Set
        End Property

        Public ReadOnly Property IsNameEnabled() As Boolean Implements IViewModel.IsNameEnabled
            Get
                ' only enable if not associated with a package (guid)
                Return String.IsNullOrEmpty(_vstemplateModel.TemplateData.Name.Package)
            End Get
        End Property

        Public Property Description() As String Implements IViewModel.Description
            Get
                If Not IsDescriptionEnabled Then
                    Return _vstemplateModel.TemplateData.Description.Package & " " & _vstemplateModel.TemplateData.Description.ID
                End If
                Return _vstemplateModel.TemplateData.Description.Value
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.Description.Value <> value Then
                    _vstemplateModel.TemplateData.Description.Value = value
                    DesignerDirty = True
                    NotifyPropertyChanged("Description")
                End If
            End Set
        End Property

        Public ReadOnly Property IsDescriptionEnabled() As Boolean Implements IViewModel.IsDescriptionEnabled
            Get
                ' only enable if not associated with a package (guid)
                Return String.IsNullOrEmpty(_vstemplateModel.TemplateData.Description.Package)
            End Get
        End Property

        Public Property Icon() As String Implements IViewModel.Icon
            Get
                If Not IsIconEnabled Then
                    Return _vstemplateModel.TemplateData.Icon.Package & " " & _vstemplateModel.TemplateData.Icon.ID
                End If
                Return _vstemplateModel.TemplateData.Icon.Value
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.Icon.Value <> value Then
                    _vstemplateModel.TemplateData.Icon.Value = value
                    DesignerDirty = True
                    NotifyPropertyChanged("Icon")
                End If
            End Set
        End Property

        Public ReadOnly Property IsIconEnabled() As Boolean Implements IViewModel.IsIconEnabled
            Get
                ' only enable if not associated with a package (guid)
                Return String.IsNullOrEmpty(_vstemplateModel.TemplateData.Icon.Package)
            End Get
        End Property


        Public Property PreviewImage() As String Implements IViewModel.PreviewImage
            Get
                Return _vstemplateModel.TemplateData.PreviewImage
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.PreviewImage <> value Then
                    _vstemplateModel.TemplateData.PreviewImage = value
                    DesignerDirty = True
                    NotifyPropertyChanged("PreviewImage")
                End If
            End Set
        End Property

        Public Property ProjectType() As String Implements IViewModel.ProjectType
            Get
                Return _vstemplateModel.TemplateData.ProjectType
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.ProjectType <> value Then
                    _vstemplateModel.TemplateData.ProjectType = value
                    DesignerDirty = True
                    NotifyPropertyChanged("ProjectType")
                End If
            End Set
        End Property

        Public Property ProjectSubType() As String Implements IViewModel.ProjectSubType
            Get
                Return _vstemplateModel.TemplateData.ProjectSubType
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.ProjectSubType <> value Then
                    _vstemplateModel.TemplateData.ProjectSubType = value
                    DesignerDirty = True
                    NotifyPropertyChanged("ProjectSubType")
                End If
            End Set
        End Property

        Public Property DefaultName() As String Implements IViewModel.DefaultName
            Get
                Return _vstemplateModel.TemplateData.DefaultName
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.DefaultName <> value Then
                    _vstemplateModel.TemplateData.DefaultName = value
                    DesignerDirty = True
                    NotifyPropertyChanged("DefaultName")
                End If
            End Set
        End Property

        Public Property GroupID() As String Implements IViewModel.GroupID
            Get
                Return _vstemplateModel.TemplateData.TemplateGroupID
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.TemplateGroupID <> value Then
                    _vstemplateModel.TemplateData.TemplateGroupID = value
                    DesignerDirty = True
                    NotifyPropertyChanged("TemplateGroupID")
                End If
            End Set
        End Property

        Public Property SortOrder() As String Implements IViewModel.SortOrder
            Get
                Return _vstemplateModel.TemplateData.SortOrder
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.SortOrder <> value Then
                    _vstemplateModel.TemplateData.SortOrder = value
                    DesignerDirty = True
                    NotifyPropertyChanged("SortOrder")
                End If
            End Set
        End Property

        Public Property LocationFieldMRUPrefix() As String Implements IViewModel.LocationFieldMRUPrefix
            Get
                Return _vstemplateModel.TemplateData.LocationFieldMRUPrefix
            End Get
            Set(ByVal value As String)
                If _vstemplateModel.TemplateData.LocationFieldMRUPrefix <> value Then
                    _vstemplateModel.TemplateData.LocationFieldMRUPrefix = value
                    DesignerDirty = True
                    NotifyPropertyChanged("LocationFieldMRUPrefix")
                End If
            End Set
        End Property

        Public Property ProvideDefaultName() As Boolean Implements IViewModel.ProvideDefaultName
            Get
                Return _vstemplateModel.TemplateData.ProvideDefaultName
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.ProvideDefaultName <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.ProvideDefaultNameSpecified = True
                    _vstemplateModel.TemplateData.ProvideDefaultName = value
                    DesignerDirty = True
                    NotifyPropertyChanged("ProvideDefaultName")
                End If
            End Set
        End Property

        Public Property CreateNewFolder() As Boolean Implements IViewModel.CreateNewFolder
            Get
                Return _vstemplateModel.TemplateData.CreateNewFolder
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.CreateNewFolder <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.CreateNewFolderSpecified = True
                    _vstemplateModel.TemplateData.CreateNewFolder = value
                    DesignerDirty = True
                    NotifyPropertyChanged("CreateNewFolder")
                End If
            End Set
        End Property

        Public Property PromptForSaveOnCreation() As Boolean Implements IViewModel.PromptForSaveOnCreation
            Get
                Return _vstemplateModel.TemplateData.PromptForSaveOnCreation
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.PromptForSaveOnCreation <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.PromptForSaveOnCreationSpecified = True
                    _vstemplateModel.TemplateData.PromptForSaveOnCreation = value
                    DesignerDirty = True
                    NotifyPropertyChanged("PromptForSaveOnCreation")
                End If
            End Set
        End Property

        Public Property Hidden() As Boolean Implements IViewModel.Hidden
            Get
                Return _vstemplateModel.TemplateData.Hidden
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.Hidden <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.HiddenSpecified = True
                    _vstemplateModel.TemplateData.Hidden = value
                    DesignerDirty = True
                    NotifyPropertyChanged("Hidden")
                End If
            End Set
        End Property

        Public Property SupportsMasterPage() As Boolean Implements IViewModel.SupportsMasterPage
            Get
                Return _vstemplateModel.TemplateData.SupportsMasterPage
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.SupportsMasterPage <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.SupportsMasterPageSpecified = True
                    _vstemplateModel.TemplateData.SupportsMasterPage = value
                    DesignerDirty = True
                    NotifyPropertyChanged("SupportsMasterPage")
                End If
            End Set
        End Property

        Public Property SupportsCodeSeparation() As Boolean Implements IViewModel.SupportsCodeSeparation
            Get
                Return _vstemplateModel.TemplateData.SupportsCodeSeparation
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.SupportsCodeSeparation <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.SupportsCodeSeparationSpecified = True
                    _vstemplateModel.TemplateData.SupportsCodeSeparation = value
                    DesignerDirty = True
                    NotifyPropertyChanged("SupportsCodeSeparation")
                End If
            End Set
        End Property

        Public Property SupportsLanguageDropDown() As Boolean Implements IViewModel.SupportsLanguageDropDown
            Get
                Return _vstemplateModel.TemplateData.SupportsLanguageDropDown
            End Get
            Set(ByVal value As Boolean)
                If _vstemplateModel.TemplateData.SupportsLanguageDropDown <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.SupportsLanguageDropDownSpecified = True
                    _vstemplateModel.TemplateData.SupportsLanguageDropDown = value
                    DesignerDirty = True
                    NotifyPropertyChanged("SupportsLanguageDropDown")
                End If
            End Set
        End Property

        Public ReadOnly Property IsLocationFieldSpecified() As Boolean Implements IViewModel.IsLocationFieldSpecified
            Get
                Return _vstemplateModel.TemplateData.LocationFieldSpecified
            End Get
        End Property

        Public Property LocationField() As VSTemplateTemplateDataLocationField Implements IViewModel.LocationField
            Get
                Return _vstemplateModel.TemplateData.LocationField
            End Get
            Set(ByVal value As VSTemplateTemplateDataLocationField)
                If _vstemplateModel.TemplateData.LocationField <> value Then
                    ' if we don't make sure the XML model knows this value is specified,
                    ' it won't save it (and it will get reset the next time we read the model)
                    _vstemplateModel.TemplateData.LocationFieldSpecified = True
                    _vstemplateModel.TemplateData.LocationField = value
                    DesignerDirty = True
                    NotifyPropertyChanged("LocationField")
                End If
            End Set
        End Property

        Public Property WizardAssembly() As String Implements IViewModel.WizardAssembly
            Get
                If (_vstemplateModel.WizardExtension IsNot Nothing) AndAlso (_vstemplateModel.WizardExtension.Count() = 1) AndAlso (_vstemplateModel.WizardExtension(0).Assembly.Count() = 1) Then
                    Return TryCast(_vstemplateModel.WizardExtension(0).Assembly(0), String)
                End If
                Return Nothing
            End Get
            Set(ByVal value As String)
                ' intentionally not implemented until the correct behavior is determined
            End Set
        End Property

        Public Property WizardClassName() As String Implements IViewModel.WizardClassName
            Get
                If (_vstemplateModel.WizardExtension IsNot Nothing) AndAlso (_vstemplateModel.WizardExtension.Count() = 1) AndAlso (_vstemplateModel.WizardExtension(0).FullClassName.Count() = 1) Then
                    Return TryCast(_vstemplateModel.WizardExtension(0).FullClassName(0), String)
                End If
                Return Nothing
            End Get
            Set(ByVal value As String)
                ' intentionally not implemented until the correct behavior is determined
            End Set
        End Property

        Public Property WizardData() As String Implements IViewModel.WizardData
            Get
                Dim result = ""
                If _vstemplateModel.WizardData Is Nothing Then
                    Return result
                End If
                For Each wizData In _vstemplateModel.WizardData
                    For Each xmlItem In wizData.Any
                        result += xmlItem.ToString()

                    Next xmlItem
                Next wizData
                Return result
            End Get
            Set(ByVal value As String)
                ' intentionally not implemented until the correct behavior is determined
            End Set
        End Property

#End Region

#Region "IDataErrorInfo"
        Public ReadOnly Property [Error]() As String Implements IDataErrorInfo.Error
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal columnName As String) As String Implements System.ComponentModel.IDataErrorInfo.Item
            Get
                Dim [error] As String = Nothing
                Select Case columnName
                    Case "ID"
                        [error] = Me.ValidateId()
                    Case "Description"
                        [error] = Me.ValidateDescription()
                End Select
                Return [error]
            End Get
        End Property

        Private Function ValidateId() As String
            If String.IsNullOrEmpty(Me.TemplateID) Then
                Return String.Format(Resources.ValidationRequiredField, Resources.FieldNameId)
            End If

            If Me.TemplateID.Length > MaxIdLength Then
                Return String.Format(Resources.ValidationFieldMaxLength, Resources.FieldNameId, MaxIdLength)
            End If
            Return Nothing
        End Function

        Private Function ValidateDescription() As String
            If String.IsNullOrEmpty(Me.Description) Then
                Return String.Format(Resources.ValidationRequiredField, Resources.FieldNameDescription)
            End If

            If Me.Description.Length > MaxDescriptionLength Then
                Return String.Format(Resources.ValidationFieldMaxLength, Resources.FieldNameDescription, MaxDescriptionLength)
            End If
            Return Nothing
        End Function


#End Region

#Region "INotifyPropertyChanged"

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub NotifyPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

#End Region

#Region "TreeView SelectionChanged"

        Private trackSel As ITrackSelection
        Private ReadOnly Property TrackSelection() As ITrackSelection
            Get
                If trackSel Is Nothing Then
                    trackSel = TryCast(_serviceProvider.GetService(GetType(STrackSelection)), ITrackSelection)
                End If
                Return trackSel
            End Get
        End Property

        Private selContainer As Microsoft.VisualStudio.Shell.SelectionContainer
        Public Sub OnSelectChanged(ByVal p As Object) Implements IViewModel.OnSelectChanged
            selContainer = New Microsoft.VisualStudio.Shell.SelectionContainer(True, False)
            Dim items As New ArrayList()
            items.Add(p)
            selContainer.SelectableObjects = items
            selContainer.SelectedObjects = items

            Dim track As ITrackSelection = TrackSelection
            If track IsNot Nothing Then
                track.OnSelectChange(CType(selContainer, ISelectionContainer))
            End If
        End Sub

#End Region

    End Class
End Namespace
