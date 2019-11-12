Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Diagnostics

'Note use of Interop wrapper
Imports SpeechLib

Partial Public Class Form1
    Inherits Form
    'The Speech recognizer
    Dim recognizer As SpInprocRecognizer

    'The grammar that we're recognizing
    Dim grammar As ISpeechRecoGrammar

    'The filestream containing the speec
    Dim fileStream As SpFileStream

    'The recognizer context
    Dim WithEvents speechRecoContext As SpInProcRecoContext

    Sub openWAVToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OpenwavToolStripMenuItem.Click
        'Standard dialog
        Dim ofd As OpenFileDialog = New OpenFileDialog()

        'Restrict dialog to .WAV files
        ofd.DefaultExt = ".wav"
        ofd.Filter = "Wav files (.wav)|*.wav"
        'N.B.: 1-based index!
        ofd.FilterIndex = 1

        If ofd.ShowDialog() = Windows.Forms.DialogResult.OK Then
            'Transcribe selected .WAV file
            TranscribeAudioFile(ofd.FileName)
        End If
    End Sub

    Sub exitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Sub TranscribeAudioFile(ByVal fName As String)
        'Create the recognizer
        recognizer = New SpInprocRecognizer()

        'Recognize the speec in the file that we passed in
        fileStream = New SpFileStream()
        fileStream.Open(fName, SpeechStreamFileMode.SSFMOpenForRead, True)
        recognizer.AudioInputStream = fileStream

        'Create a recognition context
        speechRecoContext = CType(recognizer.CreateRecoContext(), SpInProcRecoContext)

        'Create standard grammar
        grammar = speechRecoContext.CreateGrammar(10)
        Try
            'Begin recognition as soon as dictation (in this case, file playback) is begun 
            grammar.DictationSetState(SpeechRuleState.SGDSActive)
        Catch ce As System.Runtime.InteropServices.COMException
            Debug.WriteLine(ce.ToString())
        End Try
    End Sub

    'This will be called many times, as data is analyzed
    Sub OnHypothesis(ByVal StreamNumber As Integer, ByVal StreamPosition As Object, ByVal Result As ISpeechRecoResult) Handles speechRecoContext.Hypothesis
        SyncLock Me
            Dim info As ISpeechPhraseInfo = Result.PhraseInfo
            'You could, of course, store this for further processing / analysis
            Dim el As ISpeechPhraseElement
            For Each el In info.Elements
                Debug.WriteLine(el.DisplayText)
            Next
            Debug.WriteLine("--Hypothesis over--")
        End SyncLock
    End Sub

    'This will be called once, after entire file is analyzed
    Private Sub OnRecognition(ByVal StreamNumber As Integer, ByVal StreamPosition As Object, ByVal RecognitionType As SpeechRecognitionType, ByVal Result As ISpeechRecoResult) Handles speechRecoContext.Recognition
        Dim phraseInfo As ISpeechPhraseInfo = Result.PhraseInfo
        'The best guess at the completely recognized text
        Dim s As String = phraseInfo.GetText(0, -1, True)
        RichTextBox1.AppendText(s)

        'Or you could look at alternates. Here, request up to 10 alternates from index position 0 considering all elements (-1)
        Dim alternate As ISpeechPhraseAlternate
        For Each alternate In Result.Alternates(10, 10, -1)
            Dim altResult As ISpeechRecoResult = alternate.RecoResult
            Dim altInfo As ISpeechPhraseInfo = altResult.PhraseInfo
            Dim altString As String = altInfo.GetText(0, -1, True)
            Debug.WriteLine(altString)
        Next
    End Sub

    Private Sub OnAudioStreamEnd(ByVal StreamNumber As Integer, ByVal StreamPosition As Object, ByVal someBool As Boolean) Handles speechRecoContext.EndStream
        'Stop recognition
        grammar.DictationSetState(SpeechRuleState.SGDSInactive)
        'Unload the active dictation topic
        grammar.DictationUnload()
    End Sub

End Class
