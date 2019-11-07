Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.Ink

Partial Public Class Form1
    Inherits Form
    Dim WithEvents myInkOverlay As InkOverlay

    Public Sub New()
        InitializeComponent()

        'Initialize myInkOverlay and associate it with basicTextBox
        myInkOverlay = New InkOverlay(basicTextBox)
        myInkOverlay.Enabled = True

    End Sub

    Sub CursorDownHandler(ByVal sender As Object, ByVal e As InkCollectorCursorDownEventArgs) Handles myInkOverlay.CursorDown
        'Turn off timer until the pen rises and a Stroke is added
        Timer1.Stop()
    End Sub

    Sub StrokeHandler(ByVal sender As Object, ByVal e As InkCollectorStrokeEventArgs) Handles myInkOverlay.Stroke
        'Recognition occurs when timer1.Interval (5 seconds) passes without a new Stroke being added
        Timer1.Start()
    End Sub

    Sub TimerTickHandler(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        Try
            'Is there ink to recognize?
            If myInkOverlay.Ink.Strokes Is Nothing Or myInkOverlay.Ink.Strokes.Count = 0 Then
                Return
            End If
            'Retrieve the default recognizer
            Dim recognizers As Recognizers = New Recognizers()
            Dim recognizer As Recognizer = recognizers.GetDefaultRecognizer()

            'Recognition is done with a recognition context
            Dim context As RecognizerContext = recognizer.CreateRecognizerContext()
            'Add current ink to the context
            context.Strokes = myInkOverlay.Ink.Strokes
            'Stop ink collection for context
            context.EndInkInput()

            'Perform recognition
            Dim status As RecognitionStatus
            Dim result As RecognitionResult = context.Recognize(status)
            If status = RecognitionStatus.NoError Then
                Dim topString As String = result.TopString

                'Replace or, if no selection, append at cursor
                basicTextBox.SelectedText += topString
            End If
        Finally
            'Clear the ink
            myInkOverlay.Ink.DeleteStrokes()
            'Redraw the textbox
            basicTextBox.Invalidate()

            'Timer has done its job until more ink added
            Timer1.Stop()
        End Try
    End Sub

End Class
