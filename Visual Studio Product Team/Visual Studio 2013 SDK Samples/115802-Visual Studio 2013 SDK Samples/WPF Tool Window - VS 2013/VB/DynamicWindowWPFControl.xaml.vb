'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System
Imports Microsoft.Samples.VisualStudio.IDE.ToolWindow
Imports System.Globalization

Public Class DynamicWindowWPFControl


    Private currentState_Renamed As WindowStatus = Nothing
    ''' <summary>
    ''' This is the object that will keep track of the state of the IVsWindowFrame
    ''' that is hosting this control. The pane should set this property once
    ''' the frame is created to enable us to stay up to date.
    ''' </summary>
    Public Property CurrentState() As WindowStatus
        Get
            Return currentState_Renamed
        End Get
        Set(ByVal value As WindowStatus)
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If
            currentState_Renamed = value
            ' Subscribe to the change notification so we can update our UI.
            AddHandler currentState_Renamed.StatusChange, AddressOf Me.RefreshValues
            ' Update the display now.
            Me.RefreshValues(Me, Nothing)
        End Set
    End Property



    ''' <summary>
    ''' This method is the call back for state changes events.
    ''' </summary>
    ''' <param name="sender">Event senders</param>
    ''' <param name="arguments">Event arguments</param>
    Private Sub RefreshValues(ByVal sender As Object, ByVal arguments As EventArgs)
        Me.xText.Text = currentState_Renamed.X.ToString(CultureInfo.CurrentCulture)
        Me.yText.Text = currentState_Renamed.Y.ToString(CultureInfo.CurrentCulture)
        Me.widthText.Text = currentState_Renamed.Width.ToString(CultureInfo.CurrentCulture)
        Me.heightText.Text = currentState_Renamed.Height.ToString(CultureInfo.CurrentCulture)
        Me.dockedCheckBox.IsChecked = currentState_Renamed.IsDockable
    End Sub
End Class

