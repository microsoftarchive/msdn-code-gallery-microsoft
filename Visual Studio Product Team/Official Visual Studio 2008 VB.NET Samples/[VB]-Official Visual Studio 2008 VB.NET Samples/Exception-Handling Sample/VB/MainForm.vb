' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class MainForm

    ' Load the message for the text box that is below the file name text box.
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' The code below uses the new StringWriter to build a string in memory.
        ' The WriteLine commands append code to the string buffer with carriage return and line feed.
        Dim writer As StringWriter = New StringWriter()

        With writer
            .WriteLine("Enter a file name & path to test error handling. ")
            .WriteLine("Try different combinations for example:")
            .WriteLine("")
            .WriteLine("  C:\Filename.txt")
            .WriteLine("  C:\FolderName\Filename.txt")
            .WriteLine("  \\ServerName\FolderName\Filename.txt")
        End With

        ' Ask the StringWriter to covert its buffer to a string
        Me.txtMessage.Text = writer.ToString()
    End Sub


    Private Sub cmdNoTryCatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdNoTryCatch.Click
        ' Ask to make sure the user is willing to possibly blow up the program.
        Dim message As String = "The following code has no error handling and will cause an unhandled exception if a file is not found. Do you want to continue?"
        If MessageBox.Show(message, Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            ' Use the FileStream class from the System.IO Namespace (see Imports at top of file).
            Dim fs As FileStream

            ' This command will fail if the file does not exist.
            fs = File.Open(Me.txtFileName.Text, FileMode.Open)
            MessageBox.Show("The size of the file is: " & fs.Length, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            fs.Close()

        End If
    End Sub

    Private Sub cmdBasicTryCatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBasicTryCatch.Click
        ' This procedure will perform a basic Try, Catch.
        ' Use the FileStream class from the System.IO Namespace (see Imports at top of file).
        Dim fs As FileStream

        Try
            ' This command will fail if the file does not exist.
            fs = File.Open(Me.txtFileName.Text, FileMode.Open)
            MessageBox.Show("The size of the file is: " & fs.Length, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            fs.Close()
        Catch exp As Exception
            ' Will catch any error that we're not explicitly trapping.
            MessageBox.Show(exp.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try
    End Sub

    Private Sub cmdDetailedTryCatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDetailedTryCatch.Click
        ' This procedure will perform a specific Try, Catch looking for any IO related errors.
        ' Use the FileStream class from the System.IO Namespace (see Imports at top of file).
        Dim fs As FileStream

        Try
            ' This command will fail if the file does not exist.
            fs = File.Open(Me.txtFileName.Text, FileMode.Open)
            MessageBox.Show("The size of the file is: " & fs.Length, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            fs.Close()
        Catch exp As FileNotFoundException
            ' Will catch an error when the file requested does not exist.
            MessageBox.Show("The file you requested does not exist.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        Catch exp As DirectoryNotFoundException
            ' Will catch an error when the directory requested does not exist.
            MessageBox.Show("The directory you requested does not exist.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        Catch exp As IOException
            ' Will catch any generic IO exception.
            MessageBox.Show(exp.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        Catch exp As Exception
            ' Will catch any error that we're not explicitly trapping.
            MessageBox.Show(exp.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)

        End Try
    End Sub

    Private Sub cmdCustomMessage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCustomMessage.Click
        ' This procedure will perform a specific Try, Catch looking for any IO related errors
        ' Use the FileStream class from the System.IO Namespace (see Imports at top of file)
        Dim fs As FileStream

        Try
            ' This command will fail if the file does not exist
            fs = File.Open(Me.txtFileName.Text, FileMode.Open)
            MessageBox.Show("The size of the file is: " & fs.Length, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            fs.Close()
        Catch exp As IOException
            ' Will catch any generic IO exception.
            ' You could use the StringWriter to build a multi-line string in memory.
            ' However, it's overkill for this simple message.
            ' FYI StringWriter comes from the System.IO Namespace.
            Dim message As String
            message = "Unable to open the file you requested, " & Me.txtFileName.Text & vbCrLf & vbCrLf & _
              "Detailed Error Information below:" & vbCrLf & vbCrLf & _
              "  Message: " & exp.Message & vbCrLf & _
              "  Source: " & exp.Source & vbCrLf & vbCrLf & _
              "  Stack Trace:" & vbCrLf

            Dim trace As String

            ' Accessing an exception objects StackTrace could cause an exception
            ' thus we need to wrap the call in its own Try, Catch block.
            Try
                trace = exp.StackTrace()
            Catch stExp As Security.SecurityException
                trace = "Unable to access stack trace due to security restrictions."
            Catch stExp As Exception
                trace = "Unable to access stack trace."
            End Try

            message = message & trace

            MessageBox.Show(message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        Catch exp As System.Exception
            ' This catch will trap any error unexpected error.
            MessageBox.Show(exp.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)

        End Try
    End Sub

    Private Sub cmdTryCatchFinally_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdTryCatchFinally.Click
        ' This procedure will perform a basic Try, Catch, and then a Finally.
        ' Use the FileStream class from the System.IO Namespace (see Imports at top of file).
        Dim fs As FileStream = Nothing

        Try
            ' This command will fail if the file does not exist.
            fs = File.Open(Me.txtFileName.Text, FileMode.Open)
            MessageBox.Show("The size of the file is: " & fs.Length, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch exp As Exception
            ' Will catch any error that we're not explicitly trapping.
            MessageBox.Show(exp.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        Finally
            ' If we didn't open the file successfully, then our reference will be Nothing.
            If Not fs Is Nothing Then
                fs.Close()
                MessageBox.Show("File closed successfully in Finally block", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Try
    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
