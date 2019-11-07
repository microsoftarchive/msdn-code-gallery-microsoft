'****************************** Module Header ******************************\
' Module Name:  ProgressInfo.vb
' Project:	    VBWMIEnableDisableNetworkAdapter
' Copyright (c) Microsoft Corporation.
' 
' This is a form that shows the progress information while enabling or 
' disabling a Network Adapter.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Windows.Forms
Imports System.Drawing

Public Class ProgressInfoForm
    Inherits Form

#Region "Private Properties"

    ''' <summary>
    ''' Form.Location.X
    ''' </summary>
    ''' <remarks></remarks>
    Private _intLocationX As Integer
    Public Property LocationX As Integer
        Get
            Return _intLocationX
        End Get
        Set(ByVal value As Integer)
            _intLocationX = value
        End Set
    End Property

    ''' <summary>
    ''' Form.Location.Y
    ''' </summary>
    ''' <remarks></remarks>
    Private _intLocationY As Integer
    Public Property LocationY As Integer
        Get
            Return _intLocationY
        End Get
        Set(ByVal value As Integer)
            _intLocationY = value
        End Set
    End Property

#End Region

#Region "Construct ProgressInfo"

    Public Sub New()
        InitializeComponent()
    End Sub

#End Region

#Region "Event Handler"

    Private Sub ProgressInfoLoad(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load

        Location = New Point(LocationX, LocationY)
    End Sub

#End Region

End Class