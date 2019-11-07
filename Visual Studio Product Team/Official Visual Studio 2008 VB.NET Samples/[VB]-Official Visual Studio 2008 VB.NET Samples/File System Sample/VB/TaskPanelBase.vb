' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.VisualBasic.FileIO

Public Class TaskPanelBase

    Private parameterList As System.Collections.Generic.List(Of Control)
    Private panelInstance As TaskPanelBase

    ' Constants
    Private Shared START_PARAMETER_LOCATION As New System.Drawing.Point(105, 65)
    Private Const Y_OFFSET As Integer = 30
    Private Const X_OFFSET As Integer = 230

    ''' <summary>
    ''' This method creates controls for each parameter and lays them out on the form in a 
    ''' reasonable fashion
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    ''' <remarks></remarks>
    Protected Sub AddParameter(ByVal name As String, ByVal value As Control)

        Me.SuspendLayout()

        If (parameterList Is Nothing) Then
            parameterList = New System.Collections.Generic.List(Of Control)
        End If

        'Create a label for the parameter name
        Dim newParameter As New Label()
        newParameter.AutoSize = True
        newParameter.Location = GetNextLocation()
        newParameter.Name = name
        newParameter.Text = name
        newParameter.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        parameterList.Add(newParameter)

        'Now setup the Value control
        value.Location = New Point(newParameter.Location.X + X_OFFSET, newParameter.Location.Y)

        'Add the parameter and its value control to the panel
        Me.GroupBox2.Controls.Add(newParameter)
        Me.GroupBox2.Controls.Add(value)

        Me.EndParenLabel.Location = New Point(Me.EndParenLabel.Location.X, GetNextLocation().Y)
        Me.ResumeLayout()
        Me.Update()

    End Sub

    ''' <summary>
    ''' Get the next control location
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetNextLocation() As Point
        'If this is the first parameter, start at the default location
        If (parameterList.Count < 1) Then
            Return START_PARAMETER_LOCATION
        Else
            Return New Point(START_PARAMETER_LOCATION.X, START_PARAMETER_LOCATION.Y + (parameterList.Count * Y_OFFSET))
        End If
    End Function

    ''' <summary>
    ''' Utility function to convert the UICancelOption string to its Enum value
    ''' </summary>
    ''' <param name="userControl"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function ParseUICancelOption(ByVal userControl As ComboBox) As UICancelOption
        Dim cancelOption As UICancelOption

        Select Case CType(userControl.SelectedItem, String)
            Case "Do Nothing"
                cancelOption = UICancelOption.DoNothing
            Case "Throw Exception"
                cancelOption = UICancelOption.ThrowException
        End Select
        Return cancelOption
    End Function

    ''' <summary>
    ''' Converts the String description of the encoding to the correct type.
    ''' </summary>
    ''' <param name="encodingStr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function ParseEncoding(ByVal encodingStr As String) As System.Text.Encoding
        Dim retEncoding As System.Text.Encoding = System.Text.Encoding.UTF8
        Select Case encodingStr
            Case "ASCII", "Unicode"
                retEncoding = System.Text.Encoding.GetEncoding(encodingStr)
            Case "BigEndianUnicode"
                retEncoding = System.Text.Encoding.BigEndianUnicode
            Case "UTF7"
                retEncoding = System.Text.Encoding.UTF7
            Case "UTF8"
                retEncoding = System.Text.Encoding.UTF8
            Case "UTF32"
                retEncoding = System.Text.Encoding.UTF32
        End Select
        Return retEncoding
    End Function

End Class
