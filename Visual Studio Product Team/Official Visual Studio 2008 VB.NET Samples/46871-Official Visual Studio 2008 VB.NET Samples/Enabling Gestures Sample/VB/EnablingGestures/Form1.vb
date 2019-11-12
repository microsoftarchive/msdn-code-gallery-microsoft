Imports Microsoft.Ink
Imports System.Text

Public Class Form1
    'Indices within checkedListBox1 of "special action" gestures
    Dim allGesturesIndex As Integer
    Dim noGesturesIndex As Integer

    'Flag for ItemCheck delegate: differentiate between user-initiated and program-initiated state change
    Dim programmaticRecursionSoReturn As Boolean = False

    'Ink-Gathering object
    Dim WithEvents myInkOverlay As InkOverlay

    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add all the types of ApplicationGesture to the checkedListBox for possible selection
        CheckedListBox1.Items.AddRange(System.Enum.GetNames(GetType(ApplicationGesture)))

        'Find the "AllGestures" and "NoGestures" items and store their indices for later special handling
        Dim i As Integer
        For i = 0 To CheckedListBox1.Items.Count - 1
            Dim gestureName As String = CheckedListBox1.Items(i).ToString()
            If (gestureName = "AllGestures") Then
                allGesturesIndex = i
            End If

            If (gestureName = "NoGesture") Then
                noGesturesIndex = i
            End If
        Next

        'Set up ink-collecting object
        myInkOverlay = New InkOverlay(Panel1)
        myInkOverlay.Enabled = True
        'By default, CollectionMode is Ink. Final option is InkAndGesture. 
        myInkOverlay.CollectionMode = CollectionMode.GestureOnly
        'Since we checked this in the listbox, set the status correctly
        myInkOverlay.SetGestureStatus(ApplicationGesture.AllGestures, True)

        'Initialize flag so initial call to ItemCheck delegate goes through
        programmaticRecursionSoReturn = False
    End Sub


    Private Sub CheckedListBox1_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles CheckedListBox1.ItemCheck
        'If event raised programmatically, it's just a UI, not behavioral, change, so return immediately
        If (programmaticRecursionSoReturn = True) Then
            Return
        End If
        'Set recursion flag so programmatically raised events return quickly
        programmaticRecursionSoReturn = True

        'Are we turning on or turning off the gesture?
        Dim listening As Boolean
        If (e.NewValue = CheckState.Checked) Then
            listening = True
        Else
            listening = False

        End If

        'Which gesture?
        Dim gestureName As String = CheckedListBox1.Items(e.Index).ToString()
        Dim gesture As ApplicationGesture = CType(System.Enum.Parse(GetType(ApplicationGesture), gestureName), ApplicationGesture)

        'On the special values, clear all previously-selected gestures
        If (e.Index = allGesturesIndex Or e.Index = noGesturesIndex) Then
            Dim checkedIndex As Integer
            For Each checkedIndex In CheckedListBox1.CheckedIndices
                'This will raise ItemCheck event, but programmaticRecursionSoReturn flag is set
                CheckedListBox1.SetItemCheckState(checkedIndex, CheckState.Unchecked)
            Next
        Else
            'Clear the special values. (Note: Programmatically raised ItemCheck event)
            CheckedListBox1.SetItemCheckState(allGesturesIndex, CheckState.Unchecked)
            CheckedListBox1.SetItemCheckState(noGesturesIndex, CheckState.Unchecked)
        End If

        'Apply listening value to chosen gesture
        myInkOverlay.SetGestureStatus(gesture, listening)

        'Clear recursion flag
        programmaticRecursionSoReturn = False
    End Sub

    Sub InkOvrlayGestureHandler(ByVal sender As Object, ByVal e As InkCollectorGestureEventArgs) Handles myInkOverlay.Gesture
        'Gesture-handler: Note how often NoGesture has a higher confidence than sought-for gesture
        Dim sb As New StringBuilder()
        Dim candidate As Gesture
        For Each candidate In e.Gestures
            sb.AppendFormat("{0} : {1}" + System.Environment.NewLine, candidate.Id, candidate.Confidence)
        Next
        Label1.Text = sb.ToString()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Start with "AllGestures" checked
        CheckedListBox1.SetItemChecked(allGesturesIndex, True)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Clear ink
        myInkOverlay.Ink.DeleteStrokes()
        Panel1.Invalidate()
    End Sub
End Class
