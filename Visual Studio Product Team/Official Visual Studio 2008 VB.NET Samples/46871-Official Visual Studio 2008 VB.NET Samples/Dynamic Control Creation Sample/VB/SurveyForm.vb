' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class SurveyForm

    ' Create necessary private variables to hold information.
    Private surveyTitleValue As String = "Survey"
    Private surveyResponseValue As String = "Survey Not Completed"

    ' Create a property so the controls can be easily retrieved.
    Public ReadOnly Property SurveyFormControls() As Control.ControlCollection
        Get
            Return Me.Controls
        End Get
    End Property

    ' Create a height property so height can be changed easily.
    Public Property SurveyHeight() As Integer
        Get
            Return Me.Height
        End Get
        Set(ByVal Value As Integer)
            Me.Height = Value
        End Set
    End Property

    ' Create a property so the response can be easily retrieved.
    Public ReadOnly Property SurveyResponse() As String
        Get
            Return surveyResponseValue
        End Get
    End Property

    ' Create a property so the title of the form can be easily
    '   retrieved and set.
    Public Property SurveyTitle() As String
        Get
            Return surveyTitleValue
        End Get
        Set(ByVal Value As String)
            surveyTitleValue = Value
            Me.Text = surveyTitleValue
        End Set
    End Property

    ' Create a width property so width can be changed easily.
    Public Property SurveyWidth() As Integer
        Get
            Return Me.Width
        End Get
        Set(ByVal Value As Integer)
            Me.Width = Value
        End Set
    End Property

    ' This simply resets the SurveyResponse string and closes the form.
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ' Reset the SurveyResponse string.
        surveyResponseValue = "Survey Not Completed"

        ' Close the form.
        Me.Close()
    End Sub

    ' This button first fills out the SurveyResponse string then closes the form.
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click

        ' Create the controls we'll be using in all of the later loops.
        Dim aControl As Control
        Dim aGroupControl As Control
        Dim anObject As Object

        ' Reset the Response string
        Me.surveyResponseValue = ""

        ' Loop through each control and output the user response into the
        '   the SurveyResponse string. (The string could easily be replaced
        '   with a collection of some sort.)
        For Each aControl In Me.Controls
            ' Differentiate output based on the type of the control
            Select Case TypeName(aControl)
                Case "ComboBox"
                    ' Simple get the text out of the group box and add it to the
                    '   response string, along with the question name
                    surveyResponseValue += aControl.Name + " - "
                    surveyResponseValue += aControl.Text
                    surveyResponseValue += vbCrLf
                Case "TextBox"
                    ' Simple get the text out of the group box and add it to the
                    '   response string, along with the question name
                    surveyResponseValue += aControl.Name + " - "
                    surveyResponseValue += aControl.Text
                    surveyResponseValue += vbCrLf
                Case "GroupBox"
                    ' Need to go inside of the GroupBox to yank out the 
                    '   RadioButtons
                    For Each aGroupControl In CType(aControl, GroupBox).Controls
                        If TypeOf aGroupControl Is RadioButton Then
                            If CType(aGroupControl, RadioButton).Checked Then
                                ' Simple get the question and response of the 
                                '   user being surveyed.
                                surveyResponseValue += aControl.Name + " - "
                                surveyResponseValue += aGroupControl.Text
                                surveyResponseValue += vbCrLf
                            End If
                        End If

                    Next
                Case "ListBox"
                    ' For this one we must get each of the selected lines, and 
                    '   return them.
                    surveyResponseValue += aControl.Name + " - "
                    For Each anObject In CType(aControl, ListBox).SelectedItems
                        If TypeOf anObject Is String Then
                            ' Simple get the question and response of the 
                            '   user being surveyed.
                            surveyResponseValue += vbTab + CStr(anObject)
                            surveyResponseValue += vbCrLf
                        End If
                    Next
            End Select
        Next

        ' Close the form.
        Me.Close()
    End Sub


End Class