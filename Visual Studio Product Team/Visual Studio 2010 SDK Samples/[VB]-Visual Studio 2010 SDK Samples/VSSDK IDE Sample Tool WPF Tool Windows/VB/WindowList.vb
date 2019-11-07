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
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualStudio.Shell.Interop

Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports VsConstants = Microsoft.VisualStudio.VSConstants
Imports ErrorHandler = Microsoft.VisualStudio.ErrorHandler

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This class is responsible for retrieving and keeping
	''' the list of tool windows.
	''' This cache the result of the last refresh.
	''' </summary>
	Friend Class WindowList
        ' List of tool window frames, current as of the last refresh.
		Private framesList As IList(Of IVsWindowFrame) = Nothing
        ' Names of the tool windows.
		Private toolWindowNames As IList(Of String) = Nothing

		''' <summary>
        ''' Get the IVsWindowFrame for the specified index.
		''' </summary>
		''' <param name="index">Index in the list of windows</param>
		''' <returns>frame of the window</returns>
		Default Public ReadOnly Property Item(ByVal index As Integer) As IVsWindowFrame
			Get
				Return framesList(index)
			End Get
		End Property

		''' <summary>
		''' The names of the existing tool windows.
        ''' This gets updated when RefreshList is updated.
		''' </summary>
		Public ReadOnly Property WindowNames() As IList(Of String)
			Get
				Return toolWindowNames
			End Get
		End Property

		''' <summary>
        ''' Update the content of the list by asking VS.
		''' </summary>
		''' <returns></returns>
		Public Sub RefreshList()
			framesList = New List(Of IVsWindowFrame)()
			toolWindowNames = New List(Of String)()

            ' Get the UI Shell service.
            Dim uiShell As IVsUIShell4 = CType(Microsoft.VisualStudio.Shell.Package.GetGlobalService(GetType(SVsUIShell)), IVsUIShell4)
            ' Get the tool windows enumerator.
            Dim windowEnumerator As IEnumWindowFrames = Nothing

            'uint flags = unchecked(((uint)__WindowFrameTypeFlags.WINDOWFRAMETYPE_Tool |(uint)__WindowFrameTypeFlags.WINDOWFRAMETYPE_Uninitailzed));
            'ErrorHandler.ThrowOnFailure(uiShell.GetWindowEnum(flags, out windowEnumerator));

            Dim flags As UInteger = (CType(__WindowFrameTypeFlags.WINDOWFRAMETYPE_Tool, UInteger) + CType(__WindowFrameTypeFlags.WINDOWFRAMETYPE_Uninitialized, UInteger))
            ErrorHandler.ThrowOnFailure(uiShell.GetWindowEnum(flags, windowEnumerator))
            'ErrorHandler.ThrowOnFailure(uiShell.GetToolWindowEnum(windowEnumerator))

            Dim frame As IVsWindowFrame() = New IVsWindowFrame(0) {}
			Dim fetched As UInteger = 0
			Dim hr As Integer = VsConstants.S_OK
            ' Note that we get S_FALSE when there is no more item, so only loop while we are getting S_OK.
			Do While hr = VsConstants.S_OK
                ' For each tool window, add it to the list.
				hr = windowEnumerator.Next(1, frame, fetched)
				ErrorHandler.ThrowOnFailure(hr)
                If fetched = 1 Then

                    If (frame(0).IsVisible() = VsConstants.S_OK) Then
                        ' We successfully retrieved a window frame, update our lists.
                        Dim caption As String = CStr(GetProperty(frame(0), CInt(Fix(__VSFPROPID.VSFPROPID_Caption))))
                        toolWindowNames.Add(caption)
                        framesList.Add(frame(0))
                    End If
                End If
            Loop
		End Sub

		''' <summary>
        ''' This wraps the call to IVsWindowFrame.GetProperty.
		''' </summary>
		''' <param name="frame">Window frame for which we want the property</param>
		''' <param name="propertyID">ID of the property to retrieve</param>
		''' <returns>The value of the property</returns>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")> _
		Friend Function GetProperty(ByVal frame As IVsWindowFrame, ByVal propertyID As Integer) As Object
			Dim result As Object = Nothing
			ErrorHandler.ThrowOnFailure(frame.GetProperty(propertyID, result))
			Return result
		End Function

		''' <summary>
        ''' This wraps the call to IVsWindowFrame.GetGuidProperty.
		''' </summary>
		''' <param name="frame">Window frame for which we want the property</param>
		''' <param name="propertyID">ID of the property to retrieve</param>
		''' <returns>The value of the property</returns>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")> _
		Friend Function GetGuidProperty(ByVal frame As IVsWindowFrame, ByVal propertyID As Integer) As Guid
			Dim result As Guid = Guid.Empty
			ErrorHandler.ThrowOnFailure(frame.GetGuidProperty(propertyID, result))
			Return result
		End Function

		''' <summary>
        ''' Returns a list of SelectionProperties items (1 per tool window listed).
		''' </summary>
		Friend ReadOnly Property WindowsProperties() As ArrayList
			Get
				Dim index As Integer = 0
                Dim properties As New ArrayList()
				For Each frame As IVsWindowFrame In Me.framesList
                    ' Get the properties for this frame.
					Dim [property] As SelectionProperties = GetFrameProperties(frame)
					[property].Index = index
					properties.Add([property])
					index += 1
				Next frame
				Return properties
			End Get
		End Property

		''' <summary>
        ''' Provides the properties object corresponding to the window frame.
		''' </summary>
		''' <param name="frame">Window frame to return properties for</param>
		''' <returns>Properties object</returns>
		Friend Function GetFrameProperties(ByVal frame As IVsWindowFrame) As SelectionProperties
            ' Get the caption and Guid for the current tool window.
			Dim caption As String = CStr(Me.GetProperty(frame, CInt(Fix(__VSFPROPID.VSFPROPID_Caption))))
			Dim persistenceGuid As Guid = Me.GetGuidProperty(frame, CInt(Fix(__VSFPROPID.VSFPROPID_GuidPersistenceSlot)))

            ' Create the property object based on this and add it to the list.
            Dim [property] As New SelectionProperties(caption, persistenceGuid)
			Return [property]
		End Function
	End Class
End Namespace
