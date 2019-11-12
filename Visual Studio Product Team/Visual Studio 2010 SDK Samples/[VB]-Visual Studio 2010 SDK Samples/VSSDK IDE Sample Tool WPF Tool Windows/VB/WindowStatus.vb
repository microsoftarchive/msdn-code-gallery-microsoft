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
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This class keeps track of the position, size and dockable state of the
	''' window it is associated with. By registering an instance of this class
	''' with a window frame (this can be a tool window or a document window)
	''' Visual Studio will call back the IVsWindowFrameNotify3 methods when
	''' changes occur.
	''' </summary>
	Public NotInheritable Class WindowStatus
		Implements IVsWindowFrameNotify3
        ' Private fields to keep track of the last known state.
		Private x_Renamed As Integer = 0
		Private y_Renamed As Integer = 0
		Private width_Renamed As Integer = 0
		Private height_Renamed As Integer = 0
		Private dockable As Boolean = False
        ' Output window service.
        Private outputPane As IVsOutputWindowPane = Nothing
        Private frame As IVsWindowFrame

		#Region "Public properties"
		''' <summary>
        ''' Return the current horizontal position of the window.
		''' </summary>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")> _
		Public ReadOnly Property X() As Integer
			Get
				Return x_Renamed
			End Get
		End Property
		''' <summary>
        ''' Return the current vertical position of the window.
		''' </summary>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")> _
		Public ReadOnly Property Y() As Integer
			Get
				Return y_Renamed
			End Get
		End Property
		''' <summary>
        ''' Return the current width of the window.
		''' </summary>
		Public ReadOnly Property Width() As Integer
			Get
				Return width_Renamed
			End Get
		End Property
		''' <summary>
        ''' Return the current height of the window.
		''' </summary>
		Public ReadOnly Property Height() As Integer
			Get
				Return height_Renamed
			End Get
		End Property
		''' <summary>
        ''' Is the window dockable.
		''' </summary>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")> _
		Public ReadOnly Property IsDockable() As Boolean
			Get
				Return dockable
			End Get
		End Property

		''' <summary>
        ''' Event that gets fired when the position or the docking state of the window changes.
		''' </summary>
		Public Event StatusChange As EventHandler(Of EventArgs)

		#End Region

		''' <summary>
		''' WindowStatus Constructor.
		''' </summary>
		''' <param name="outputWindowPane">Events will be reported in the output pane if this interface is provided.</param>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702")> _
		Public Sub New(ByVal outputWindowPane As IVsOutputWindowPane, frame As IVsWindowFrame)
            outputPane = outputWindowPane
            Me.frame = frame


            ' Beta2: There is a bug with the width and height passed to this notification, so
            ' for now we get the width and height directly from the frame
            ' This is a workaround that should be removed by RTM
            If (Me.frame IsNot Nothing) Then
                Dim pos(1) As VSSETFRAMEPOS
                pos(1) = New VSSETFRAMEPOS()

                Dim x As Integer
                Dim y As Integer
                Dim width As Integer
                Dim height As Integer
                Dim unusedGuid As Guid
                frame.GetFramePos(pos, unusedGuid, x_Renamed, y_Renamed, width, height)
                Me.dockable = (pos(0) And VSSETFRAMEPOS.SFP_fFloat) <> VSSETFRAMEPOS.SFP_fFloat
            End If

        End Sub

		#Region "IVsWindowFrameNotify3 Members"
		''' <summary>
        ''' This is called when the window is being closed.
		''' </summary>
		''' <param name="pgrfSaveOptions">Should the document be saved and should the user be prompted.</param>
		''' <returns>HRESULT</returns>
		Public Function OnClose(ByRef pgrfSaveOptions As UInteger) As Integer Implements IVsWindowFrameNotify3.OnClose
            If outputPane IsNot Nothing Then
                Return outputPane.OutputString("  IVsWindowFrameNotify3.OnClose()" & Microsoft.VisualBasic.Constants.vbLf)
            Else
                Return Microsoft.VisualStudio.VSConstants.S_OK
            End If
		End Function

		''' <summary>
		''' This is called when a window "dock state" changes. This could be the
		''' result of dragging the window to result in the dock state changing
		''' or this could be as a result of changing the dock style (tabbed, mdi,
		''' dockable, floating,...).
		''' This will likely also result in a different position/size
		''' </summary>
		''' <param name="fDockable">Is the window dockable with an other window</param>
		''' <param name="x">New horizontal position</param>
		''' <param name="y">New vertical position</param>
		''' <param name="w">New width</param>
		''' <param name="h">New Height</param>
		''' <returns>HRESULT</returns>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId := "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId := "x")> _
		Public Function OnDockableChange(ByVal fDockable As Integer, ByVal x_Renamed As Integer, ByVal y_Renamed As Integer, ByVal w As Integer, ByVal h As Integer) As Integer Implements IVsWindowFrameNotify3.OnDockableChange
            If (Me.frame IsNot Nothing) Then
                ' Beta2: There is a bug with the width and height passed to this notification, so
                ' for now we get the width and height directly from the frame
                ' This is a workaround that should be removed by RTM
                Dim pos(1) As VSSETFRAMEPOS
                pos(1) = New VSSETFRAMEPOS()

                Dim unusedx As Integer
                Dim unusedy As Integer
                Dim unusedGuid As Guid
                frame.GetFramePos(pos, unusedGuid, unusedx, unusedy, w, h)
            End If

            Me.x_Renamed = x_Renamed
			Me.y_Renamed = y_Renamed
			Me.width_Renamed = w
			Me.height_Renamed = h
			Me.dockable = (fDockable <> 0)

			Me.GenerateStatusChangeEvent(Me, New EventArgs())

			Return Microsoft.VisualStudio.VSConstants.S_OK
		End Function

		''' <summary>
        ''' This is called when the window is moved.
		''' </summary>
		''' <param name="x">New horizontal position</param>
		''' <param name="y">New vertical position</param>
		''' <param name="w">New width</param>
		''' <param name="h">New Height</param>
		''' <returns>HRESULT</returns>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId := "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId := "x")> _
		Public Function OnMove(ByVal x_Renamed As Integer, ByVal y_Renamed As Integer, ByVal w As Integer, ByVal h As Integer) As Integer Implements IVsWindowFrameNotify3.OnMove

            If (Me.frame IsNot Nothing) Then
                ' Beta2: There is a bug with the width and height passed to this notification, so
                ' for now we get the width and height directly from the frame
                ' This is a workaround that should be removed by RTM
                Dim pos(1) As VSSETFRAMEPOS
                pos(1) = New VSSETFRAMEPOS()

                Dim unusedx As Integer
                Dim unusedy As Integer
                Dim unusedGuid As Guid
                frame.GetFramePos(pos, unusedGuid, unusedx, unusedy, w, h)
            End If

            Me.x_Renamed = x_Renamed
			Me.y_Renamed = y_Renamed
			Me.width_Renamed = w
			Me.height_Renamed = h

			Me.GenerateStatusChangeEvent(Me, New EventArgs())

			Return Microsoft.VisualStudio.VSConstants.S_OK
		End Function

		''' <summary>
        ''' This is called when the window is shown or hidden.
		''' </summary>
		''' <param name="fShow">State of the window</param>
		''' <returns>HRESULT</returns>
		Public Function OnShow(ByVal fShow As Integer) As Integer Implements IVsWindowFrameNotify3.OnShow
			Dim state As __FRAMESHOW = CType(fShow, __FRAMESHOW)
            If outputPane IsNot Nothing Then
                Return outputPane.OutputString(String.Format(CultureInfo.CurrentCulture, "  IVsWindowFrameNotify3.OnShow({0})" & Microsoft.VisualBasic.Constants.vbLf, state.ToString()))
            Else
                Return Microsoft.VisualStudio.VSConstants.S_OK
            End If
		End Function

		''' <summary>
        ''' This is called when the window is resized.
		''' </summary>
		''' <param name="x">New horizontal position</param>
		''' <param name="y">New vertical position</param>
		''' <param name="w">New width</param>
		''' <param name="h">New Height</param>
		''' <returns>HRESULT</returns>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId := "x"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId := "y")> _
		Public Function OnSize(ByVal x_Renamed As Integer, ByVal y_Renamed As Integer, ByVal w As Integer, ByVal h As Integer) As Integer Implements IVsWindowFrameNotify3.OnSize

            If (Me.frame IsNot Nothing) Then
                ' Beta2: There is a bug with the width and height passed to this notification, so
                ' for now we get the width and height directly from the frame
                ' This is a workaround that should be removed by RTM
                Dim pos(1) As VSSETFRAMEPOS
                pos(1) = New VSSETFRAMEPOS()

                Dim unusedx As Integer
                Dim unusedy As Integer
                Dim unusedGuid As Guid
                frame.GetFramePos(pos, unusedGuid, unusedx, unusedy, w, h)
            End If

            Me.x_Renamed = x_Renamed
            Me.y_Renamed = y_Renamed
            Me.width_Renamed = w
            Me.height_Renamed = h

            Me.GenerateStatusChangeEvent(Me, New EventArgs())

            Return Microsoft.VisualStudio.VSConstants.S_OK
        End Function

		#End Region

		''' <summary>
        ''' Generate the event if someone is listening to it.
		''' </summary>
		''' <param name="sender">Event Sender</param>
		''' <param name="arguments">Event arguments</param>
		Private Sub GenerateStatusChangeEvent(ByVal sender As Object, ByVal arguments As EventArgs)
            If Me.StatusChangeEvent IsNot Nothing Then
                Me.StatusChangeEvent.Invoke(Me, New EventArgs())
            End If
		End Sub
	End Class
End Namespace
