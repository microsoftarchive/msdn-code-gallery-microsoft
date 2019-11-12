' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private counter As PerformanceCounter


    ''' <summary>
    ''' This subroutine decrements the value of a custom counter. It 
    ''' can only be executed when the counter selected is a
    ''' custom counter -- only custom counters can have their ReadOnly properies
    ''' set to False. 
    ''' </summary>
    Private Sub btnDecrementCounter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDecrementCounter.Click
        ' This button is only enabled when the selected
        ' counter is a Custom counter, so we can set the read-only to
        ' False, and use it. Still, use a Try-Catch...
        Try
            ' Set ReadOnly to False, so that the counter can be manipulated
            counter.ReadOnly = False

            ' Only decrement the counter if it has a value > 0
            ' Although this isn't technically necessary, it makes logical
            ' sense.  
            If counter.RawValue > 0 Then
                counter.Decrement()
                ToolStripStatusLabel1.Text = ""
            Else
                ' Display the status to the user.
                ToolStripStatusLabel1.Text = "Counter is already zero."
            End If
        Catch exc As Exception
            ' In case an Exception is raised, explain that the counter
            ' could not be decremented.
            ToolStripStatusLabel1.Text = "Could not decrement counter."
        End Try

    End Sub

    ''' <summary>
    ''' This subroutine increments the value of a custom counter. It 
    ''' can only be executed when the counter selected is a
    ''' custom counter -- only custom counters can have their ReadOnly properies
    ''' set to False. 
    ''' </summary>
    Private Sub btnIncrementCounter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIncrementCounter.Click
        ' This button is only enabled when the selected
        ' counter is a Custom counter, so we can set the read-only to
        ' False, and use it. Still, use a Try-Catch...
        Try
            ' Set ReadOnly to False, so that the counter can be manipulated
            counter.ReadOnly = False
            counter.Increment()
            ToolStripStatusLabel1.Text = ""
        Catch
            ' In case an Exception is raised, explain that the counter
            ' could not be incremented.
            ToolStripStatusLabel1.Text = "Could not increment counter."
        End Try
    End Sub

    ''' <summary>
    ''' This subroutine enables the user to refresh the form after adding
    ''' a custom PerformanceCounter using the Server Explorer.
    ''' </summary>
    Private Sub btnRefreshCategories_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefreshCategories.Click
        ' Reset the User Interface
        Me.cboCategories.Items.Clear()
        Me.cboCounters.Items.Clear()
        Me.counter = Nothing
        Me.txtBuiltInOrCustom.Text = ""
        Me.txtCounterHelp.Text = ""
        Me.txtCounterType.Text = ""
        Me.txtCounterValue.Text = ""
        Me.btnDecrementCounter.Enabled = False
        Me.btnIncrementCounter.Enabled = False
        Me.cboCategories.Text = ""
        Me.cboCounters.Text = ""

        ' Call the Form_Load event
        Me.Form1_Load(Me, New System.EventArgs())
    End Sub

    ''' <summary>
    ''' This event handler is fired whenever the user changes the selected 
    ''' counter category. It then changes the cboCounters combo box to 
    ''' reflect the counters available for that category. 
    ''' This routine uses the CounterDisplayItem
    ''' class that is defined in this project. The CounterDisplayItem 
    ''' takes advantage of the way a combo box displays items -- if it
    ''' contains an object, the ToString() method is called to fill the display.
    ''' This is very important, since we must take into account both the 
    ''' "instance" (what process to watch) and the particular counter that
    ''' is associated with that instance. For example, you can measure the 
    ''' number of CLR bytes compiled overall in the system, or just for a 
    ''' particular instance of a running .NET program.
    ''' </summary>
    Private Sub cboCategories_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCategories.SelectedIndexChanged

        Dim category As PerformanceCounterCategory
        Dim counters As New ArrayList()
        Dim thisCounter As PerformanceCounter
        Dim counterNames() As String

        If cboCategories.SelectedIndex <> -1 Then
            ' Fill cboCounters with Counter names.
            Try
                ' Get the available category.
                category = New PerformanceCounterCategory( _
                    Me.cboCategories.SelectedItem.ToString())

                ' Get all available instances for the selected category.
                counterNames = category.GetInstanceNames()

                ' If there are no instances, then the counters are likely
                ' generic, so get the counters that are available (they
                ' will have no instance value).
                If counterNames.Length = 0 Then
                    counters.AddRange(category.GetCounters())
                Else
                    Dim i As Integer
                    For i = 0 To counterNames.Length - 1
                        counters.AddRange( _
                            category.GetCounters(counterNames(i)))
                    Next
                End If

                ' Clear the cboCounter box & reset text.
                Me.cboCounters.Items.Clear()
                Me.cboCounters.Text = ""

                ' Add the counters to the cboCounters combo box. Use the
                ' CounterDisplayItem class to ensure that they are properly
                ' displayed, and also to ensure that there is a reference
                ' to the actual counter stored in the combo box item.
                For Each thisCounter In counters
                    Me.cboCounters.Items.Add(New CounterDisplayItem(thisCounter))
                Next

            Catch exc As Exception
                ' Alert the user, if the program is unable to list the 
                ' counters in this category.
                MsgBox("Unable to list the Counters in this Category." + _
                    "Please select another Category.")
            End Try

        End If
    End Sub

    ''' <summary>
    ''' This event handler is fired whenever the user changes the selected 
    ''' counter. It then set the class variable 'counter' to the actual
    ''' value of the counter (using the CounterDisplayItem object that was
    ''' stored in the cboCounters combo box). After the assignment is made,
    ''' information about the counter is displayed in the UI.
    ''' This routine makes uses the CounterDisplayItem
    ''' class that is defined in this project.
    ''' </summary>
    Private Sub cboCounters_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCounters.SelectedIndexChanged

        Dim displayItem As CounterDisplayItem
        Try
            ' Get the CounterDisplayItem associated with the selection
            displayItem = CType(cboCounters.SelectedItem, CounterDisplayItem)
            ' Get the PerformanceCounter object stored in the CounterDisplayItem
            counter = displayItem.Counter
            ' Display information about the Counter to the user
            Me.txtCounterType.Text = counter.CounterType.ToString()
            Me.txtCounterHelp.Text = counter.CounterHelp.ToString()
            ToolStripStatusLabel1.Text = ""

            ' If the counter is a custom counter, enable the appropriate
            ' buttons.  Only custom items can be written to.
            ' Note: the CounterDisplayItem shows the code necessary to determine
            ' if a counter is custom or not.
            If displayItem.IsCustom Then
                ' Enable Increment and Decrement buttons
                Me.txtBuiltInOrCustom.Text = "Custom"
                Me.btnDecrementCounter.Enabled = True
                Me.btnIncrementCounter.Enabled = True
            Else
                ' Disable Increment and Decrement buttons
                Me.txtBuiltInOrCustom.Text = "Built-In"
                Me.btnDecrementCounter.Enabled = False
                Me.btnIncrementCounter.Enabled = False
            End If

        Catch exc As Exception
            ' Set the class counter to Nothing if there was an error.
            counter = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' This subroutine loads the cboCategories combo box with a list
    ''' of all the available categories on the local machine.  It also
    ''' starts the timer, so that the UI will be updated every half second.
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' Fill cboCounters with available counters
        Dim category As PerformanceCounterCategory
        Dim categories() As PerformanceCounterCategory
        ' Set myCategories to contain all of the available Categories
        categories = PerformanceCounterCategory.GetCategories()

        ' Declare a string array with the proper length
        Dim categoryNames(categories.Length - 1) As String
        Dim i As Integer = 0 ' Used as a counter

        ' Loop through the available categories, adding them to an
        ' array of strings. (This is done so that the categories can be
        ' be sorted.)
        For Each category In categories
            categoryNames(i) = category.CategoryName
            i += 1
        Next

        ' Sort the array
        Array.Sort(categoryNames)

        ' Add each value of the array to the cboCategories combo box.
        Dim nameString As String
        For Each nameString In categoryNames
            Me.cboCategories.Items.Add(nameString)
        Next

        ' Start the timer
        Me.tmrUpdateUI.Interval = 500
        Me.tmrUpdateUI.Enabled = True
    End Sub

    ''' <summary>
    ''' This event handler updates the value of the counter in the 
    ''' user interface.
    ''' </summary>
    Private Sub tmrUpdateUI_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdateUI.Tick
        ' Verify that a Counter exists
        ' If it does, get its type and value. (Use Try-Catch, just in case.)
        Try
            If Not counter Is Nothing Then
                Me.txtCounterValue.Text = counter.NextValue().ToString()
            End If
        Catch exc As Exception
            Me.txtCounterValue.Text = ""
        End Try

    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class