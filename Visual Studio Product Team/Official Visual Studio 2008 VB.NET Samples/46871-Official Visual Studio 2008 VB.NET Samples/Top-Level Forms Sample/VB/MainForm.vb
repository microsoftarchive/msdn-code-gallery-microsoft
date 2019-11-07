' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    ''' <summary>
    ''' These Custom events ared used to notify the Forms class
    ''' when the user cancels a Save during the Form_Closing event or
    ''' when they chose Exit from the File menu.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event SaveWhileClosingCancelled As System.EventHandler
    Public Event ExitApplication As System.EventHandler

    Private dirtyValue As Boolean = False
    Private closingCompleteValue As Boolean = False
    Private documentNameValue As String
    Private fileNameValue As String

    Public ReadOnly Property ClosingComplete() As Boolean
        Get
            Return closingCompleteValue
        End Get
    End Property

    Public ReadOnly Property DocumentName() As String
        Get
            Return documentNameValue
        End Get
    End Property

    ''' <summary>
    ''' This property determines if we need to save our data before we close the form.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Property Dirty() As Boolean
        Get
            Return dirtyValue
        End Get
        Set(ByVal Value As Boolean)
            If Value Then
                If Not Me.Text.EndsWith("*") Then
                    Me.Text = Me.Text & "*"

                    Me.sbDocInfo.Text = "Changes need to be saved."
                End If
            Else
                Me.sbDocInfo.Text = "Ready"
                ' Remove the *
                Me.Text = Me.Text.Substring(0, (Me.Text.Length - 1))
            End If

            dirtyValue = Value
        End Set
    End Property

    Public Property FileName() As String
        Get
            Return fileNameValue
        End Get
        Set(ByVal Value As String)
            fileNameValue = Value
            documentNameValue = System.IO.Path.GetFileNameWithoutExtension(fileNameValue)
            Me.Text = Me.DocumentName
        End Set
    End Property

    Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        Try
            ' Set our ClosingComplete Property to true
            ' to let the Forms class know it can remove 
            ' us from its collection
            closingCompleteValue = True

            If Me.dirty Then
                ' Ask the use to Save (Yes), Not Save (No), or Stop the closing (Cancel)
                Dim strDocTitle As String
                If Me.Text.EndsWith("*") Then
                    strDocTitle = Me.Text.Substring(0, (Me.Text.Length - 1))
                Else
                    strDocTitle = Me.Text
                End If

                Dim strMsg As String = String.Format("Do you want to save {0}?", strDocTitle)

                Select Case MessageBox.Show(strMsg, "Closing", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Case Windows.Forms.DialogResult.Yes                        ' Save the document
                        Me.SaveDocument()
                    Case Windows.Forms.DialogResult.No
                        ' Don't save the document just exit
                        ' Put your code here
                    Case Windows.Forms.DialogResult.Cancel
                        ' Stop the form from closing.
                        e.Cancel = True
                        ' If the user cancel's the close, we need to keep
                        ' our form in the main Forms collection
                        closingCompleteValue = False
                        ' Raise an event to stop the application 
                        ' from closing any other open documents
                        RaiseEvent SaveWhileClosingCancelled(Me, Nothing)
                End Select
            End If
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuExit.Click
        ' Exit the application
        RaiseEvent ExitApplication(Me, Nothing)
    End Sub

    Private Sub mnuNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuNew.Click
        ' Create a new document instnace
        Forms.NewForm()
    End Sub

    Private Sub mnuSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSave.Click
        Me.SaveDocument()
    End Sub

    Private Sub mnuClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuClose.Click
        Me.Close()
    End Sub

    Private Sub txtData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtData.TextChanged
        ' If the data is changed, set the form's dirty property to true
        Me.Dirty = True
    End Sub

    Private Sub SaveDocument()
        ' This code DOES NOT perform any file I/O. 
        Try
            ' Check to see if the document is dirty
            If Me.Dirty Then
                ' Check to see if we have a file (document) name already
                If Not Me.FileName Is Nothing Then
                    ' Save the existing document to the file
                Else
                    ' We don't have a file name, ask for one.
                    ' See the Common Dialog How-to for an example of
                    ' Use the Save Common Dialog

                    ' We're going to create a file name based upon the document
                    ' title and the current application's directory
                    Me.FileName = AppDomain.CurrentDomain.BaseDirectory & "Saved" & Me.Text
                End If

                ' Once the document has been saved, reset the dirty bit
                Me.Dirty = False
            End If
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class

