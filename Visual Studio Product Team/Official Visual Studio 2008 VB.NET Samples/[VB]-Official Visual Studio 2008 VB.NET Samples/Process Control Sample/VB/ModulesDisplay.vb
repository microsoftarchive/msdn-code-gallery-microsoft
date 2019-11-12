' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class ModulesDisplay

    Private processIdValue As Integer

    Public Property ProcessID() As Integer
        Get
            Return processIdValue
        End Get
        Set(ByVal Value As Integer)
            processIdValue = Value
        End Set
    End Property

    Private Sub ModulesDisplay_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Takes the string of modules built up by the btnModules_Click event procedure in 
        ' the TaskManager form and displays into the user in the RichTextBox.
        Dim ProcessInfo As Process = _
            Process.GetProcessById(ProcessID)
        Dim modl As ProcessModuleCollection = ProcessInfo.Modules
        Dim strMod As New System.Text.StringBuilder()
        For Each proMod As ProcessModule In modl
            strMod.Append("Module Name: " + proMod.ModuleName + vbCrLf)
        Next proMod
        rchText.Text = strMod.ToString
    End Sub



End Class