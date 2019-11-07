' Copyright (c) Microsoft Corporation. All rights reserved.
Imports TailspinToysCRM

Public Class MainForm

    Private Sub cmdEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
        Dim c As Customer
        Try
            Dim i As Integer = 14213
            c = Customer.EditCustomer(i)
        Catch exp As CustomerNotFoundException
            MsgBox(exp.Message, MsgBoxStyle.OkOnly, exp.AppSource)
        Catch exp As CustomerException
            MsgBox(exp.Message, MsgBoxStyle.OkOnly, exp.AppSource)
        Catch exp As Exception
            MsgBox(exp.Message, MsgBoxStyle.OkOnly, exp.Source)
        End Try
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click

        Try
            Dim i As Integer = 14213
            Customer.DeleteCustomer(i)

            ' We'll never see this, but just for completeness.
            MsgBox(String.Format("Customer Id {0} was deleted.", i), MsgBoxStyle.OKOnly, Me.Text)
        Catch exp As CustomerNotDeletedException
            Dim c As Customer
            c = exp.CustomerInfo
            ' We can now do something more interesting with
            ' the customer if we wanted to.
            MsgBox(exp.Message, MsgBoxStyle.OKOnly, exp.AppSource)
        Catch exp As CustomerException
            MsgBox(exp.Message, MsgBoxStyle.OKOnly, exp.AppSource)
        Catch exp As Exception
            MsgBox(exp.Message, MsgBoxStyle.OKOnly, exp.Source)
        End Try

    End Sub

    Private Sub cmdUntrapped_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUntrapped.Click
        ' Normally an untrapped error in Visual Basic 6.0
        ' and earlier would result in a quick MessageBox and then
        ' our process shutting down.
        ' Windows Forms however have injected a top-level error 
        ' catch between the CLR and our code.
        ' Whether or not you see their dialog depends upon three things:
        ' 1) Is there an active debugger?
        ' 2) Do you have your own exception handler in place?
        ' 3) Have you turned JIT debugging on (set to true) in your App's config file.?

        ' If an untrapped error occurs and you answered no to the
        ' above questions, then you will see the Windows Forms dialog
        ' which gives the user a chance to Continue or Quit. If you are 
        ' running in Debug mode, you will see the Exception Assistant.
        ' To see the handler, you need to run the compiled code.

        Dim i As Short = 1234
        Dim j As Short = 0
        Dim k As Short = -1

        k = CShort(i / j)

        ' You will never see the MsgBox statement below.
        MsgBox("Your results are: " & k.ToString(), MsgBoxStyle.OKOnly, Me.Text)
    End Sub

    Friend Sub OnThreadException(ByVal sender As Object, ByVal t As System.Threading.ThreadExceptionEventArgs)

    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Friend ReadOnly Property ShouldTrapUnhandledException() As Boolean
        Get
            Return Me.chkGET.Checked
        End Get
    End Property
End Class
