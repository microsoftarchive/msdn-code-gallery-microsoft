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
Imports System.Collections.Generic
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
	Friend Class MockUiShellProvider
		Private Shared uiShellFactory As GenericMockFactory = Nothing
		Private Shared enumWindowsFactory0 As GenericMockFactory = Nothing
		Private Shared enumWindowsFactory2 As GenericMockFactory = Nothing
		Private Shared windowCount As Integer = 0

		#Region "UiShell Getters"
		''' <summary>
        ''' Returns an IVsUiShell that does not implement any methods.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetUiShellInstance() As BaseMock
			If uiShellFactory Is Nothing Then
				uiShellFactory = New GenericMockFactory("UiShell", New Type() { GetType(IVsUIShell) })
			End If
			Dim uiShell As BaseMock = uiShellFactory.GetInstance()
			Return uiShell
		End Function

		''' <summary>
		''' Get an IVsUiShell that implement CreateToolWindow and GetToolWindowEnum.
		''' The enumeration does not contain any window.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetWindowEnumerator0() As BaseMock
			Dim uiShell As BaseMock = GetUiShellInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "CreateToolWindow")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf CreateToolWindowCallBack))
			name = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "GetToolWindowEnum")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetToolWindowEnumCallBack0))
			name = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "FindToolWindow")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf FindToolWindowCallBack))
			Return uiShell
		End Function

		''' <summary>
		''' Get an IVsUiShell that implement CreateToolWindow and GetToolWindowEnum.
		''' The enumeration contains 2 windows.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetWindowEnumerator2() As BaseMock
			Dim uiShell As BaseMock = GetUiShellInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "CreateToolWindow")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf CreateToolWindowCallBack))
			name = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "GetToolWindowEnum")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetToolWindowEnumCallBack2))
			Return uiShell
		End Function


		''' <summary>
        ''' Get an IVsUiShell that implement ShowMessageBox and returns Cancel from pressing the buttons.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetShowMessageBoxCancel() As BaseMock
			Dim uiShell As BaseMock = GetUiShellInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")
			uiShell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf ShowMessageBoxCancel))
			Return uiShell
		End Function

		#End Region


		#Region "Callbacks"
		Private Shared Sub CreateToolWindowCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

            ' Create the output mock object for the frame.
			Dim frame As IVsWindowFrame = MockWindowFrameProvider.GetBaseFrame()
			arguments.SetParameter(9, frame)

            ' The window pane (if one is provided) needs to be sited.
			Dim pane As IVsWindowPane = TryCast(arguments.GetParameter(2), IVsWindowPane)
            If pane IsNot Nothing Then
                ' Create a service provider to site the window pane.
                Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
                ' It needs to provide STrackSelection.
                Dim trackSelectionFactory As GenericMockFactory = MockWindowFrameProvider.TrackSelectionFactory
                serviceProvider.AddService(GetType(STrackSelection), trackSelectionFactory.GetInstance(), False)
                ' Add support for output window.
                serviceProvider.AddService(GetType(SVsOutputWindow), New OutputWindowService(), False)
                ' Finally we need support for FindToolWindow.
                serviceProvider.AddService(GetType(SVsUIShell), GetWindowEnumerator0(), False)

                pane.SetSite(serviceProvider)
            End If
		End Sub

		Private Shared Sub FindToolWindowCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

            ' Create the output mock object for the frame.
			Dim frame As IVsWindowFrame = MockWindowFrameProvider.GetBaseFrame()
			arguments.SetParameter(2, frame)
		End Sub

		Private Shared Sub GetToolWindowEnumCallBack0(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

            ' Create the output mock object.
			If enumWindowsFactory0 Is Nothing Then
				enumWindowsFactory0 = New GenericMockFactory("EnumWindows", New Type() { GetType(IEnumWindowFrames) })
			End If
			Dim enumWindows As BaseMock = enumWindowsFactory0.GetInstance()
            ' Add support for Next.
			Dim name As String = String.Format("{0}.{1}", GetType(IEnumWindowFrames).FullName, "Next")
			enumWindows.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf NextCallBack0))

			arguments.SetParameter(0, enumWindows)
		End Sub

		Private Shared Sub NextCallBack0(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_FALSE
			arguments.SetParameter(2, CUInt(0))
		End Sub

		Private Shared Sub GetToolWindowEnumCallBack2(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

            ' Create the output mock object.
			If enumWindowsFactory2 Is Nothing Then
				enumWindowsFactory2 = New GenericMockFactory("EnumWindows2", New Type() { GetType(IEnumWindowFrames) })
			End If
			Dim enumWindows As BaseMock = enumWindowsFactory2.GetInstance()
            ' Add support for Next.
			Dim name As String = String.Format("{0}.{1}", GetType(IEnumWindowFrames).FullName, "Next")
			enumWindows.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf NextCallBack2))
			windowCount = 0

			arguments.SetParameter(0, enumWindows)
		End Sub

		Private Shared Sub NextCallBack2(ByVal caller As Object, ByVal arguments As CallbackArgs)
			If windowCount >= 2 Then
                ' We already enumerated 2 window frames, we are done (0 left to enumerate).
				NextCallBack0(caller, arguments)
				Return
			End If

			arguments.ReturnValue = VSConstants.S_OK
            ' Create the list of properties we expect being asked for.
            Dim properties As New Dictionary(Of Integer, Object)
            properties.Add(CInt(Fix(__VSFPROPID.VSFPROPID_Caption)), "Tool Window " & windowCount.ToString())
			windowCount += 1
			properties.Add(CInt(Fix(__VSFPROPID.VSFPROPID_GuidPersistenceSlot)), Guid.NewGuid())
            ' Create the output mock object for the frame.
			Dim o As Object = arguments.GetParameter(1)
			Dim frame As IVsWindowFrame() = CType(o, IVsWindowFrame())
			frame(0) = MockWindowFrameProvider.GetFrameWithProperties(properties)
            ' Fetched 1 frame.
			arguments.SetParameter(2, CUInt(1))
		End Sub

		Private Shared Sub ShowMessageBoxCancel(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.SetParameter(10, CInt(Fix(DialogResult.Cancel)))
			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		#End Region
	End Class
End Namespace
