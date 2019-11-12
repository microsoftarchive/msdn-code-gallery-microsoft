'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.VisualStudio
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop


Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	Friend Class MockServicesProvider
		#Region "Fields"
		Private Shared uiShellFactory As GenericMockFactory
		Private Shared runningDocFactory As GenericMockFactory
		Private Shared windowFrameFactory As GenericMockFactory
		Private Shared qeqsFactory As GenericMockFactory
#End Region

		#Region "Getter functions for the general Mock objects"
		#Region "UiShell Getters"
		''' <summary>
        ''' Returns an IVsUiShell that does not implement any methods.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetUiShellInstance() As BaseMock
			If uiShellFactory Is Nothing Then
				uiShellFactory = New GenericMockFactory("UiShell", New Type() { GetType(IVsUIShell), GetType(IVsUIShellOpenDocument) })
			End If
			Dim uiShell As BaseMock = uiShellFactory.GetInstance()
			Return uiShell
		End Function

		''' <summary>
        ''' Get an IVsUiShell that implements SetWaitCursor, SaveDocDataToFile, ShowMessageBox.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetUiShellInstance0() As BaseMock
			Dim uiShell As BaseMock = GetUiShellInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "SetWaitCursor")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf SetWaitCursorCallBack))
			name = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "SaveDocDataToFile")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf SaveDocDataToFileCallBack))
			name = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf ShowMessageBoxCallBack))
			name = String.Format("{0}.{1}", GetType(IVsUIShellOpenDocument).FullName, "OpenCopyOfStandardEditor")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf OpenCopyOfStandardEditorCallBack))
			Return uiShell
		End Function
		#End Region

		#Region "RunningDocumentTable Getters"

		''' <summary>
		''' Gets the IVsRunningDocumentTable mock object which implements FindAndLockIncrement, 
        ''' NotifyDocumentChanged and UnlockDocument.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetRunningDocTableInstance() As BaseMock
            If runningDocFactory Is Nothing Then
                runningDocFactory = New GenericMockFactory("RunningDocumentTable", New Type() {GetType(IVsRunningDocumentTable)})
            End If
			Dim runningDoc As BaseMock = runningDocFactory.GetInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsRunningDocumentTable).FullName, "FindAndLockDocument")
			runningDoc.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf FindAndLockDocumentCallBack))
			name = String.Format("{0}.{1}", GetType(IVsRunningDocumentTable).FullName, "NotifyDocumentChanged")
			runningDoc.AddMethodReturnValues(name, New Object(){VSConstants.S_OK})
			name = String.Format("{0}.{1}", GetType(IVsRunningDocumentTable).FullName, "UnlockDocument")
			runningDoc.AddMethodReturnValues(name, New Object(){VSConstants.S_OK})
			Return runningDoc
		End Function
		#End Region

		#Region "Window Frame Getters"

		''' <summary>
        ''' Gets an IVsWindowFrame mock object which implements the SetProperty method.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetWindowFrameInstance() As BaseMock
            If windowFrameFactory Is Nothing Then
                windowFrameFactory = New GenericMockFactory("WindowFrame", New Type() {GetType(IVsWindowFrame)})
            End If
			Dim windowFrame As BaseMock = windowFrameFactory.GetInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsWindowFrame).FullName, "SetProperty")
			windowFrame.AddMethodReturnValues(name, New Object() { VSConstants.S_OK })
			Return windowFrame
		End Function
		#End Region

		#Region "QueryEditQuerySave Getters"

		''' <summary>
        ''' Gets an IVsQueryEditQuerySave2 mock object which implements QuerySaveFile and QueryEditFiles methods.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetQueryEditQuerySaveInstance() As BaseMock
            If qeqsFactory Is Nothing Then
                qeqsFactory = New GenericMockFactory("QueryEditQuerySave", New Type() {GetType(IVsQueryEditQuerySave2)})
            End If

			Dim qeqs As BaseMock = qeqsFactory.GetInstance()

			Dim name As String = String.Format("{0}.{1}", GetType(IVsQueryEditQuerySave2).FullName, "QuerySaveFile")
			qeqs.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf QuerySaveFileCallBack))
			name = String.Format("{0}.{1}", GetType(IVsQueryEditQuerySave2).FullName, "QueryEditFiles")
			qeqs.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf QueryEditFilesCallBack))
			Return qeqs
		End Function
		#End Region
#End Region

		#Region "Callbacks"
		Private Shared Sub SetWaitCursorCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		Private Shared Sub SaveDocDataToFileCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

			Dim dwSave As VSSAVEFLAGS = CType(arguments.GetParameter(0), VSSAVEFLAGS)
			Dim editorInstance As IPersistFileFormat = CType(arguments.GetParameter(1), IPersistFileFormat)
            Dim fileName As String = CType(arguments.GetParameter(2), String)

            ' Call Save on the EditorInstance depending on the Save Flags.
			Select Case dwSave
				Case VSSAVEFLAGS.VSSAVE_Save, VSSAVEFLAGS.VSSAVE_SilentSave
                    editorInstance.Save(fileName, 0, 0)
                    ' Setting pbstrMkDocumentNew.
                    arguments.SetParameter(3, fileName)
                    ' Setting pfSaveCanceled.
                    arguments.SetParameter(4, 0)
				Case VSSAVEFLAGS.VSSAVE_SaveAs
                    Dim newFileName As String = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName))
                    ' Call Save with new file and remember=1.
                    editorInstance.Save(newFileName, 1, 0)
                    ' Setting pbstrMkDocumentNew.
                    arguments.SetParameter(3, newFileName)
                    ' Setting pfSaveCanceled.
                    arguments.SetParameter(4, 0)
				Case VSSAVEFLAGS.VSSAVE_SaveCopyAs
                    Dim newFileName As String = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName))
                    ' Call Save with new file and remember=0.
                    editorInstance.Save(newFileName, 0, 0)
                    ' Setting pbstrMkDocumentNew.
                    arguments.SetParameter(3, newFileName)
                    ' Setting pfSaveCanceled.
                    arguments.SetParameter(4, 0)
			End Select
		End Sub

		Private Shared Sub FindAndLockDocumentCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

            ' Setting IVsHierarchy.
            arguments.SetParameter(2, Nothing)
            ' Setting itemID.
            arguments.SetParameter(3, CUInt(1))
            ' Setting DocData.
            arguments.SetParameter(4, IntPtr.Zero)
            ' Setting docCookie.
            arguments.SetParameter(5, CUInt(1))
		End Sub

		Private Shared Sub QuerySaveFileCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

			arguments.SetParameter(3, CUInt(tagVSQuerySaveResult.QSR_SaveOK)) 'set result
		End Sub

		Private Shared Sub QueryEditFilesCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

			arguments.SetParameter(5, CUInt(tagVSQueryEditResult.QER_EditOK)) 'setting result
			arguments.SetParameter(6, CUInt(0)) 'setting outFlags
		End Sub

		Private Shared Sub ShowMessageBoxCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

			arguments.SetParameter(10, CInt(Fix(DialogResult.Yes)))
		End Sub

		Private Shared Sub OpenCopyOfStandardEditorCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK
			arguments.SetParameter(2, CType(GetWindowFrameInstance(), IVsWindowFrame))
		End Sub
		#End Region
	End Class
End Namespace