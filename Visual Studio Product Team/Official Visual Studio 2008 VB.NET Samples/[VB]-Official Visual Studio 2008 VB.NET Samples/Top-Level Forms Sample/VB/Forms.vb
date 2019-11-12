' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Forms
    ' Internal Collection to manage list of document forms.
    ' Other forms such as the About form will not be in 
    ' this list, only frmMain which is our 'document' screen.
    Private Shared m_Forms As New Collection()
    ' Internal counter for use in creating a default title
    ' for each new form
    Private Shared m_FormsCreated As Integer = 0
    ' This property is used in order to determine if
    ' we need to stop application shutdown if the user
    ' clicks the cancel button on the Save document
    ' dialog displayed by forms that have dirty content.
    Private Shared m_CancelExit As Boolean = False
    ' Used to check if a shutdown is in progress
    Private Shared m_ShutdownInProgress As Boolean = False

    ' Number of forms currently loaded
    Public Shared ReadOnly Property Count() As Integer
        Get
            Return m_Forms.Count
        End Get
    End Property

    Public Shared Sub Main()
        ' Open the first document window
        Try
            Forms.NewForm()
        Catch exp As Exception
            ' if we get here, we're in trouble.
            MessageBox.Show("Sorry, we were unable to load a document. Good Bye.", "Application Main", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Application.Exit()
        End Try

        ' Set the main thread to run outside the control
        ' of a particular form so that closing one document
        ' does not terminate the whole process.
        Application.Run()
    End Sub

    Public Shared Sub NewForm()
        Try
            Forms.m_FormsCreated += 1
            Dim frm As New MainForm()
            frm.Text = "Document" & Forms.m_FormsCreated.ToString()
            m_Forms.Add(frm, frm.GetHashCode.ToString())
            ' Hook the new form's Closed event so that we know when
            ' the they close the document window
            AddHandler frm.Closed, AddressOf Forms.frmMain_Closed
            ' Hook the custom SaveWhileClosingCancelled so that we know if the
            ' use clicks the Cancel button when prompted to save a dirty document.
            AddHandler frm.SaveWhileClosingCancelled, AddressOf Forms.frmMain_SaveWhileClosingCancelled
            ' Hook the custom ExitApplicaiton so that we know if a user wants to 
            ' shut down the application by selecting the Exit menu item from a document form.
            AddHandler frm.ExitApplication, AddressOf Forms.frmMain_ExitApplication

            ' Make the form visible
            frm.Show()

        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
            If Forms.Count = 0 Then
                ' Rethrow the error to Main so
                ' we can shut down the process
                Throw exp
            End If
        End Try
    End Sub

    Private Shared Sub FormClosed(ByVal frm As MainForm)
        ' Remove the form that has closed from the internal collection.
        m_Forms.Remove(frm.GetHashCode.ToString())

        ' If we have no more forms, shut down the process.
        If m_Forms.Count = 0 Then
            Application.Exit()
        End If
    End Sub

    Public Shared Sub ExitApp()
        Try
            m_ShutdownInProgress = True

            ' Shutdown once all the forms have been closed.
            Dim frm As MainForm
            Dim i As Integer

            ' Loop through the collection stepping backwards
            ' one form at a time, asking each form to close
            ' itself. Only ask form's that are dirty that way
            ' if the user says Cancel, we won't close open forms.
            For i = m_Forms.Count To 1 Step -1
                frm = CType(m_Forms(i), MainForm)
                If frm.Dirty Then
                    frm.Close()
                End If

                ' Check our internal flag in case
                ' the user wants to stop the shutdown.
                If m_CancelExit = True Then
                    m_CancelExit = False
                    Exit Sub
                End If
            Next

            ' Now close any of documents that aren't dirty.
            ' At this point no other windows will cancel
            ' the shutdown.
            If m_Forms.Count > 0 Then
                For i = m_Forms.Count To 1 Step -1
                    frm = CType(m_Forms(i), MainForm)
                    frm.Close()
                Next
            End If
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Bug out if we get an error here
            Application.Exit()
        Finally
            m_ShutdownInProgress = False
        End Try
    End Sub

    Private Shared Sub frmMain_Closed(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            ' We catch this event when a form has finished closing.
            Dim frm As MainForm = CType(sender, MainForm)

            ' Remove our event handlers we added when the form was created.
            RemoveHandler frm.Closed, AddressOf Forms.frmMain_Closed
            RemoveHandler frm.SaveWhileClosingCancelled, AddressOf Forms.frmMain_SaveWhileClosingCancelled
            RemoveHandler frm.ExitApplication, AddressOf Forms.frmMain_ExitApplication

            ' Call our function to clean up
            Forms.FormClosed(frm)
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Shared Sub frmMain_SaveWhileClosingCancelled(ByVal sender As Object, ByVal e As System.EventArgs)
        ' This event will be caught if the user clicks cancel
        ' when asked to save a dirty document.
        If m_ShutdownInProgress Then
            ' Only change our internal value if
            ' we're actually in the process of shutting down.
            Forms.m_CancelExit = True
        End If

    End Sub

    Private Shared Sub frmMain_ExitApplication(ByVal sender As Object, ByVal e As System.EventArgs)
        ' This event will be caught if the user clicks the
        ' Exit menu command.
        Forms.ExitApp()
    End Sub


End Class
