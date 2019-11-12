' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Xml

Public Class MainForm

    ' Create constants to use in the form.
    Private Const controlWidth As Integer = 300
    Private Const charPerLine As Integer = 30
    Private Const lineHeight As Integer = 19

    ' Create class variables to use during the form.
    Private controlCount As Integer = 0
    Private controlLocation As New Point(10, 50)


    ' This subroutine adds a new button to the form, and sets up event handlers
    ' for it that will be fired on the Click and MouseHover events.
    Private Sub btnAddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddButton.Click

        ' Increment the control count.
        controlCount += 1

        ' Only allow 5 buttons, just to simplify drawing of the user interface.
        If controlCount <= 5 Then
            ' Create a new Button
            Dim newButton As New Button

            ' Add properties to the form.
            newButton.Name = "btn" + controlCount.ToString()
            newButton.Text = "btn" + controlCount.ToString()
            newButton.Location = New Point(controlLocation.X + 250, controlLocation.Y)
            controlLocation.Y += newButton.Height + 5

            ' Add the two event handlers.
            AddHandler newButton.Click, AddressOf myButtonHandler_Click
            AddHandler newButton.MouseHover, AddressOf myButtonHandler_MouseHover

            ' Add the control to the collection of controls.
            Controls.Add(newButton)
        Else
            ' Just allow 5 controls to simplify UI.
            MsgBox("You've reached 5 controls. Clear controls to start again.", _
                MsgBoxStyle.OKOnly, Me.Text)
        End If
    End Sub

    ' This subroutine clears all of the dynamically generated controls 
    ' on the page.  It does this by removing all of the controls, then calling
    ' the InitializeComponent() subroutine that was automatically generated
    ' by Visual Studio .NET.  This is a very easy way of resetting a form to
    ' its original state.
    Private Sub btnClearControls_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearControls.Click

        ' Clear all the controls.
        Controls.Clear()

        ' Add all of the original controls again.
        InitializeComponent()

        ' Reset the m_Location to its original location.
        controlLocation = New Point(10, 50)

        ' Reset the number of controls.
        controlCount = 0

        ' Show the form again.
        Show()

    End Sub

    ' This subroutine handles the btnCreateSurvey.Click event and creates
    ' a new frmSurveyForm. The controls that are generated are added to the
    ' created survey form. There are no event handlers associated with the 
    ' created controls.
    ' The created form is fairly general, and creates a survey with questions
    ' that are based on information provided by the Questions.xml document.
    ' By changing, adding, or removing nodes in the XML document, you can 
    ' change the structure and form of the survey.
    Private Sub btnCreateSurvey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateSurvey.Click

        ' Create a new Survey Form to display to the User.
        Dim survey As New SurveyForm

        ' Get the controls collection of the Survey form.
        Dim surveyControls As Control.ControlCollection = survey.SurveyFormControls

        ' Set a location for the first control.
        Dim location As Point
        location = New Point(10, 10)

        ' Create an XML document to read in the survey questions.
        Dim xr As New Xml.XmlDocument
        xr.LoadXml(My.Resources.Questions)

        ' Get the Tag used for each of the Controls we'll create. This may
        ' be useful later, if the example was extended to break apart
        ' different types of questions/responses for analysis.
        Dim myTag As String = xr.SelectSingleNode("//survey").Attributes("name").Value

        ' Set the Title on the survey form to the Display Name of the Survey.
        survey.SurveyTitle = xr.SelectSingleNode("//survey").Attributes("displayName").Value

        ' Create an XmlNodeList to contain each of the questions. Fill it.
        Dim nodeList As Xml.XmlNodeList
        nodeList = xr.GetElementsByTagName("question")

        ' Create a temporary XML Node to use when retrieving information
        ' about the nodes in the nodeList just created.
        Dim myNode As XmlNode
        For Each myNode In nodeList
            If Not myNode.Attributes Is Nothing Then
                ' Determine what type of control should be created. Pass
                ' in the required information, including the Controls collection
                ' from the frmSurveyForm form.
                Select Case myNode.Attributes("type").Value
                    Case "dropdown"
                        location = Survey_AddComboBox(myNode, surveyControls, _
                            location, myTag)
                    Case "multilist"
                        location = Survey_AddListBox(myNode, surveyControls, _
                            location, myTag, True)
                    Case "text"
                        location = Survey_AddTextBox(myNode, surveyControls, _
                            location, myTag)
                    Case "radio"
                        location = Survey_AddRadioButtons(myNode, surveyControls, _
                            location, myTag)
                End Select
            End If
        Next

        ' Set the size of the form, based off of how many controls
        ' have been placed on the form, and their dimensions.
        survey.Width = location.X + controlWidth + 30
        ' Add a bit extra to leave room for the OK and Cancel buttons.
        survey.Height = location.Y + 75

        ' Show the form.  You can also use the Show() method if you like.
        survey.ShowDialog()

        ' Show the response to the user.
        MsgBox(survey.SurveyResponse, MsgBoxStyle.OKOnly, Me.Text)
    End Sub

    ' This subroutine handles the btnTightlyBoundControls.Click event and creates
    ' two tightly bound controls. It uses the event handlers that have been 
    ' previously defined to handle the events. These event handlers are 
    ' have to be defined beforehand, unless Reflection.Emit is used.
    ' The two controls are a Button and a TextBox. When the Button is pressed, the
    ' text in the TextBox is displayed in a MsgBox.  In order to ensure that
    ' we know which TextBox is being used, it is added to the Tag property of
    ' the Button. (We don't know the name of the TextBox while creating the 
    ' event handler, since the TextBox will be created dynamically.
    Private Sub btnTightlyBoundControls_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTightlyBoundControls.Click

        ' Increment the number of controls (only by one, even though two 
        ' will be added.
        controlCount += 1

        ' Only allow 5 buttons, just to simplify drawing of the user interface.
        If controlCount <= 5 Then

            ' Create the TextBox that will contain the text to speak.
            Dim txtSpeakText As New TextBox

            ' Set up some properties for the TextBox.
            txtSpeakText.Text = "Hello, World"
            txtSpeakText.Name = "txtSpeakText"
            txtSpeakText.Location = New Point(controlLocation.X + 250, controlLocation.Y)
            txtSpeakText.Size = New Size(200, txtSpeakText.Height)

            ' Add the TextBox to the controls collection.
            Controls.Add(txtSpeakText)

            ' Increment the m_LocationY so the next control won't overwrite it.
            controlLocation.Y += txtSpeakText.Height + 5

            ' Create a button that will be used to react to clicks
            ' Since this button is tightly coupled to the TextBox which will
            ' provide it with the text to display, we'll add the TextBox created
            ' above as the Tag of this Button. 
            Dim btnSpeakText As New Button

            ' Set up some properties for the TextBox.
            btnSpeakText.Text = "Speak Text"
            btnSpeakText.Name = "btnSpeakText"
            btnSpeakText.Location = New Point(controlLocation.X + 250, controlLocation.Y)
            btnSpeakText.Size = New Size(100, btnSpeakText.Height)

            ' Add the previously created TextBox to this button.
            btnSpeakText.Tag = txtSpeakText

            ' Add the TextBox to the controls collection.
            Controls.Add(btnSpeakText)

            ' Increment the m_LocationY so the next control won't overwrite it.
            controlLocation.Y += btnSpeakText.Height + 5

            ' Add the event handler that will handle the event when the
            ' button is pressed.
            AddHandler btnSpeakText.Click, AddressOf SpeakTextClickHandler
        Else
            ' Just allow 5 controls to simplify UI.
            MsgBox("You've reached 5 controls. Clear controls to start again.", _
                MsgBoxStyle.OKOnly, Me.Text)
        End If

    End Sub

    ' This subroutine handles the Click events of all the dynamically generated
    ' buttons.  It is attached to all the buttons using the AddHandler function
    ' at the time of button creation.
    Private Sub myButtonHandler_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Verify that the type of control triggering this event is indeed
        ' a Button. This is necessary since this handler can be attached
        ' to any event.
        If TypeOf sender Is Button Then
            ' Let the user know what Button was pressed.
            MsgBox(CType(sender, Button).Text + " was pressed!", _
                    MsgBoxStyle.OKOnly, Me.Text)
        End If
    End Sub

    ' This subroutine handles the MouseHover events of all the dynamically generated
    ' buttons.  It is attached to all the buttons using the AddHandler function
    ' at the time of button creation.
    Private Sub myButtonHandler_MouseHover(ByVal sender As Object, ByVal e As EventArgs)
        ' Verify that the type of control triggering this event is indeed
        ' a Button. This is necessary since this handler can be attached
        ' to any event.
        If TypeOf sender Is Button Then
            ' Let the user know what Button was hovered over.
            MsgBox(CType(sender, Button).Text + " was hovered over!", _
                    MsgBoxStyle.OKOnly, Me.Text)
        End If
    End Sub


    ' This subroutine handles the Click event of the Button created in the
    ' tightly bound controls example. It displays in a MsgBox the text that
    ' is inside of the Tag of the Button (which is provided in the 'sender' 
    ' parameter.  Although those event handlers are sophisticated, 
    ' they do have to be defined beforehand, unless Reflection.Emit is used.
    Private Sub SpeakTextClickHandler(ByVal sender As System.Object, _
        ByVal e As System.EventArgs)

        ' Check to see if the sender is a button, and that it has
        ' a valid, tightly-coupled TextBox object attached as its
        ' Tag property.
        If TypeOf sender Is Button Then
            ' Create a button object to use in its place.
            Dim myButton As Button = CType(sender, Button)
            ' Check to see if the Button has a TextBox in its Tag property.
            If TypeOf myButton.Tag Is TextBox Then
                ' Display the text to the user.
                MsgBox(CType(myButton.Tag, TextBox).Text, MsgBoxStyle.OKOnly, _
                    Me.Text)
            End If
        End If
    End Sub

    ' This function adds a ComboBox to the passed control collection, along
    ' with an associated Label control to display the survey question.
    Private Function Survey_AddComboBox(ByVal inNode As XmlNode, _
        ByVal inControls As Control.ControlCollection, _
        ByVal location As Point, ByVal tag As String) As Point

        ' Create a new control.
        Dim newComboBox As New ComboBox

        ' Set up some properties for the control.
        newComboBox.Text = ""
        newComboBox.Name = inNode.Attributes("name").Value
        newComboBox.Tag = tag
        newComboBox.Width = controlWidth

        ' Create a temporary XML Node to use when retrieving information
        ' about the response nodes in the passed node.
        ' Get the response nodes.
        For Each node As XmlNode In inNode.SelectNodes("responses/response")
            ' Add the InnerText of the response nodes as the values for
            ' the drop down options.
            newComboBox.Items.Add(node.InnerText)
            ' If a default has been specified, use it as the current text.
            If Not node.Attributes("default") Is Nothing Then
                If node.Attributes("default").Value = "true" Then
                    newComboBox.Text = node.InnerText
                End If
            End If
        Next

        ' Create a Label and add it to the collection.
        Dim newLabel As New Label

        ' Set up some properties for the control.
        newLabel.Name = newComboBox.Name & "Label"
        newLabel.Text = inNode.SelectSingleNode("text").InnerText
        newLabel.Width = controlWidth

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newLabel.Location = location
        inControls.Add(newLabel)
        location.Y += newLabel.Height

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newComboBox.Location = location
        inControls.Add(newComboBox)
        location.Y += newComboBox.Height + 10

        ' Send back the location for the next control to be added.
        Return location
    End Function

    ' This function adds a ListBox to the passed control collection, along
    ' with an associated Label control to display the survey question.
    Private Function Survey_AddListBox(ByVal inNode As XmlNode, _
        ByVal inControls As Control.ControlCollection, _
        ByVal location As Point, ByVal tag As String, _
        ByVal isMultiSelect As Boolean) As Point

        ' Create a new control.
        Dim newList As New ListBox

        ' Set up some properties for the control.
        newList.Text = ""
        newList.Name = inNode.Attributes("name").Value
        newList.Tag = tag
        newList.Width = controlWidth

        ' Since this function can be used with either multi or single select
        ' list boxes, set the proper SelectionMode based on the passed
        ' isMultiSelect Boolean variable.
        If isMultiSelect Then
            newList.SelectionMode = SelectionMode.MultiSimple
        Else
            newList.SelectionMode = SelectionMode.One
        End If


        ' Create a temporary XML Node to use when retrieving information
        ' about the response nodes in the passed node.
        ' Add the InnerText of the response nodes as the values for
        ' the list box options.
        For Each node As XmlNode In inNode.SelectNodes("responses/response")
            newList.Items.Add(node.InnerText)
            ' If a default has been specified, use it as the current text.
            If Not node.Attributes("default") Is Nothing Then
                If node.Attributes("default").Value = "true" Then
                    newList.Text = node.InnerText
                End If
            End If
        Next

        ' Create a Label and add it to the collection
        Dim newLabel As New Label

        ' Set up some properties for the control
        newLabel.Name = newList.Name & "Label"
        newLabel.Text = inNode.SelectSingleNode("text").InnerText
        newLabel.Width = controlWidth

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newLabel.Location = location
        inControls.Add(newLabel)
        location.Y += newLabel.Height

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newList.Location = location
        inControls.Add(newList)
        location.Y += newList.Height + 10

        ' Send back the location for the next control.
        Return location
    End Function

    ' This function adds a GroupBox to the passed control collection, along
    ' with all the appropriate radio buttons, one for each available response.
    ' It also adds an associated Label control to display the survey question.
    Private Function Survey_AddRadioButtons(ByVal inNode As XmlNode, _
        ByVal inControls As Control.ControlCollection, _
        ByVal location As Point, ByVal tag As String) As Point

        ' Must create a GroupBox to contain the radio buttons
        ' otherwise they are not logically distinct from the other
        ' radio buttons on the form.
        Dim newGroupBox As New GroupBox

        ' Set up some properties for the control.
        newGroupBox.Text = ""
        newGroupBox.Name = inNode.Attributes("name").Value
        newGroupBox.Tag = tag
        newGroupBox.Width = controlWidth + 20

        ' Create some useful variables to use in the following block of code.
        Dim newRadio As RadioButton
        Dim radioPoint As New Point(5, 10)

        ' Loop through each response, and add it as a new radio button.
        For Each node As XmlNode In inNode.SelectNodes("responses/response")
            ' Create the radio button.
            newRadio = New RadioButton
            ' Add the appropriate properties.
            newRadio.Text = node.InnerText
            newRadio.Location = radioPoint
            radioPoint.Y += newRadio.Height

            ' Set the default value as the selected radio button, but
            ' only if the default attribute exists and is set to true.
            If Not node.Attributes("default") Is Nothing Then
                If node.Attributes("default").Value = "true" Then
                    newRadio.Checked = True
                End If
            End If
            ' Add the control to the group box.
            newGroupBox.Controls.Add(newRadio)
        Next

        ' Reset the height for the textbox, based on the 
        ' contained Radio Buttons.
        newGroupBox.Height = radioPoint.Y + 5

        ' Create a Label and add it to the collection.
        Dim newLabel As New Label

        ' Fix the label properties.
        newLabel.Name = newGroupBox.Name & "Label"
        newLabel.Text = inNode.SelectSingleNode("text").InnerText
        newLabel.Width = controlWidth

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newLabel.Location = location
        inControls.Add(newLabel)
        location.Y += newLabel.Height - 5

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newGroupBox.Location = location
        inControls.Add(newGroupBox)
        location.Y += newGroupBox.Height + 10

        ' Send back the location for the next control.
        Return location
    End Function

    ' This function adds a TextBox to the passed control collection, along
    ' with an associated Label control to display the survey question.
    Private Function Survey_AddTextBox(ByVal inNode As XmlNode, _
        ByVal inControls As Control.ControlCollection, _
        ByVal location As Point, ByVal tag As String _
        ) As Point

        ' Create a new control.
        Dim newText As New TextBox

        ' Fill in some of the appropriate properties.
        If Not inNode.SelectSingleNode("defaultResponse") Is Nothing Then
            newText.Text = inNode.SelectSingleNode("defaultResponse").InnerText
        End If
        If Not inNode.Attributes("name") Is Nothing Then
            newText.Name = inNode.Attributes("name").Value
        End If
        newText.Tag = tag
        newText.Width = controlWidth

        ' Set the MaxLength property based off of the XML node information.
        If Not inNode.SelectSingleNode("maxCharacters") Is Nothing Then
            newText.MaxLength = Integer.Parse(inNode.SelectSingleNode("maxCharacters").InnerText)
        End If

        ' Calculate the number of lines that should be allowed for.
        If newText.MaxLength > 0 Then
            Dim numLines As Integer = (newText.MaxLength \ charPerLine) + 1

            ' Calculate how large the textbox should be, and whether 
            ' scrollbars are necessary.
            If numLines = 1 Then
                newText.Multiline = False
            Else
                If numLines >= 4 Then
                    newText.Multiline = True
                    newText.Height = 4 * lineHeight
                    newText.ScrollBars = ScrollBars.Vertical
                Else
                    newText.Multiline = True
                    newText.Height = numLines * lineHeight
                    newText.ScrollBars = ScrollBars.None
                End If

            End If

        End If

        ' Create a Label and add it to the collection.
        Dim newLabel As New Label
        newLabel.Name = newText.Name & "Label"
        If Not inNode.SelectSingleNode("text") Is Nothing Then
            newLabel.Text = inNode.SelectSingleNode("text").InnerText
        End If
        newLabel.Width = controlWidth

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newLabel.Location = location
        inControls.Add(newLabel)
        location.Y += newLabel.Height

        ' Add the control to the Controls collection, and reset
        ' the location to the location for the next control.
        newText.Location = location
        inControls.Add(newText)
        location.Y += newText.Height + 10

        ' Send back the location for the next control.
        Return location
    End Function



    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub
End Class