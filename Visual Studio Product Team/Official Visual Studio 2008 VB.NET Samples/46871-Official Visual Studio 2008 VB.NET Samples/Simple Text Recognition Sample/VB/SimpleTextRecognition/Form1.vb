Option Strict On

Imports Microsoft.Ink

Public Class Form1

    'Ink collecting object
    Private inkOverlay As InkOverlay

    'Active recognizer
    Friend activeRecognizer As Recognizer

    Sub New()
        InitializeComponent()

        'Gather ink on the panel
        inkOverlay = New InkOverlay(Panel1)
        'Activate ink-gathering
        inkOverlay.Enabled = True

        'Get all the installed recognizers
        Dim recognizers As New Recognizers()

        'Get the default recognizer
        Dim defaultReco As Recognizer = recognizers.GetDefaultRecognizer()
        activeRecognizer = defaultReco
        AddRecognizerToMenu(defaultReco, True)

        'Recognizer.Equals() and Recognizer.GetHashCode() don't generate comparable values
        'so have to make our own
        Dim defaultRecoId As String = defaultReco.Vendor + defaultReco.Name

        'Iterate over all recognizers, including the default one
        Dim recognizer As Recognizer
        For Each recognizer In recognizers
            Dim recognizerId As String = recognizer.Vendor + recognizer.Name
            'Don't add the default recognizer again
            If recognizerId <> defaultRecoId Then
                AddRecognizerToMenu(recognizer, False)
            End If
        Next
    End Sub

    Private Sub AddRecognizerToMenu(ByVal recognizer As Recognizer, ByVal selected As Boolean)
        'Create a new menu item for the recognizer
        Dim recognizerSelector As ToolStripMenuItem = New ToolStripMenuItem(recognizer.Name)

        'Initialize whether it's checked or not and behavior
        recognizerSelector.CheckOnClick = True
        recognizerSelector.Checked = selected

        'Handle the click event
        Dim selectRecognizer As New Recognizerselector(Me, recognizerSelector, recognizer)
        AddHandler recognizerSelector.Click, AddressOf selectRecognizer.SelectRecognizer

        'Add new menu item to menu
        RecognizersToolStripMenuItem.DropDownItems.Add(recognizerSelector)
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private NotInheritable Class RecognizerSelector
        ' Methods
        Public Sub New(ByVal form1 As Form1, ByVal recognizerSelector As ToolStripMenuItem, ByVal recognizer As Recognizer)
            Me.form1 = form1
            Me.recognizer = recognizer
            Me.recognizerSelector = recognizerSelector
        End Sub

        Public Sub SelectRecognizer(ByVal sender As Object, ByVal e As EventArgs)
            form1.activeRecognizer = Me.recognizer
            Dim item1 As ToolStripMenuItem
            For Each item1 In form1.RecognizersToolStripMenuItem.DropDownItems
                item1.Checked = False
            Next
            Me.recognizerSelector.Checked = True
        End Sub

        ' Fields
        Public form1 As Form1
        Public recognizer As Recognizer
        Public recognizerSelector As ToolStripMenuItem
    End Class

    Private Sub ClearToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearToolStripMenuItem.Click
        'Clear the last-recognized text (if any)
        ListBox1.Items.Clear()

        'Clear the ink
        inkOverlay.Ink.DeleteStrokes()

        'Redraw the screen
        Panel1.Invalidate()
    End Sub

    Private Sub RecognizeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RecognizeToolStripMenuItem.Click
        'Clear previously recognized text (if any)
        ListBox1.Items.Clear()

        'Create a recognition context
        Dim context As RecognizerContext = activeRecognizer.CreateRecognizerContext()

        'Recognize all the ink 
        context.Strokes = inkOverlay.Ink.Strokes

        'That's it for the ink we'll recognize this time
        context.EndInkInput()

        'Recognizer will throw an exception if called with no Strokes
        If context.Strokes Is Nothing Or context.Strokes.Count = 0 Then
            Return
        End If

        'Note the status is ByRef 
        Dim status As RecognitionStatus
        Dim result As RecognitionResult = context.Recognize(status)

        'If no error
        If status = RecognitionStatus.NoError Then
            'Most likely string
            Dim topString As String = result.TopString
            ListBox1.Items.Add(topString)

            'Sometimes, you'll want to see alternates (will include TopString as first result)
            Dim alternate As RecognitionAlternate
            For Each alternate In result.GetAlternatesFromSelection()
                Dim alternateString As String = alternate.ToString()
                ListBox1.Items.Add(alternateString)
            Next
        End If

        'Probably enough to require scrolling, so enable listBox1
        ListBox1.Enabled = True
    End Sub
End Class




