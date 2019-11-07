' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Threading

Public Class TaskProgress

    Private Sub TaskProgress_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.ThreadID.Text &= CStr(Thread.CurrentThread.ManagedThreadId)
    End Sub
End Class