'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel.Design
Imports System.Globalization

Imports Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.MenuCommands
    ''' <summary>
    ''' This class implements a very specific type of command: this command will count the
    ''' number of times the used has clicked on it and will change its text to show this count.
    ''' </summary>
    Friend Class DynamicTextCommand
        Inherits OleMenuCommand
        ' Counter of the clicks.
        Private clickCount As Integer

        ''' <summary>
        ''' This is the function that is called when the user clicks on the menu command.
        ''' It will check that the selected object is actually an instance of this class and
        ''' increment its click counter.
        ''' </summary>
        Private Shared Sub ClickCallback(ByVal sender As Object, ByVal args As EventArgs)
            Dim cmd As DynamicTextCommand = TryCast(sender, DynamicTextCommand)
            If Not Nothing Is cmd Then
                cmd.clickCount += 1
            End If
        End Sub

        ''' <summary>
        ''' Creates a new DynamicTextCommand object with a specific CommandID and base text.
        ''' </summary>
        Public Sub New(ByVal id As CommandID, ByVal text As String)
            MyBase.New(New EventHandler(AddressOf ClickCallback), id, text)
        End Sub

        ''' <summary>
        ''' If a command is defined with the TEXTCHANGES flag in the VSCT file and this package is
        ''' loaded, then Visual Studio will call this property to get the text to display.
        ''' </summary>
        Public Overrides Property Text() As String
            Get

                Return String.Format(CultureInfo.CurrentCulture, VSPackage.DynamicTextFormat, MyBase.Text, clickCount)
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
            End Set
        End Property
    End Class
End Namespace
