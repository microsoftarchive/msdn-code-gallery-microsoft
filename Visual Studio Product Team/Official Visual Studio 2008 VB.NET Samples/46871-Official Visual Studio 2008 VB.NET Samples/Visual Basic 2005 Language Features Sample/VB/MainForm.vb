' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Collections.Generic

Public Class MainForm

#Region "1. Operator Overloading"

    Private Sub JoinStringsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JoinStringsButton.Click
        ' Variables used to store custom string objects.
        ' ** go to definition in ValidatedString.vb file**
        Dim vsA, vsB, vsResult As ValidatedString

        ' Initialize objects.
        vsA = New ValidatedString(Me.vsATextBox.Text)
        vsB = New ValidatedString(Me.vsBTextBox.Text)

        ' Join or concatenate custom objects using familiar & operator.
        '** go to definition in ValidatedString.vb file**
        vsResult = vsA & vsB

        ' Display the joined result.
        Me.vsResultTextbox.Text = vsResult.Value
    End Sub
#End Region

#Region "2. Generics Consumption"

    ''' <remarks>Generic List provides Intellisense and compile time checking for ValidatedString parameters</remarks>
    Private Sub FillListboxButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FillListboxButton.Click

        ' Pass ValidatedString type in as parameter to generic List container.
        Dim vsList As New List(Of ValidatedString)

        For i As Integer = 0 To 25
            ' Note that ValidatedString has intellisense and type checking.
            Dim newValidatedString As ValidatedString
            newValidatedString = New ValidatedString("ListItem") & New ValidatedString(CStr(i))

            If newValidatedString.IsValid = True Then
                ' Note that Add method only accepts ValidatedString type.
                ' Try adding an invalid type like an integer.
                vsList.Add(newValidatedString)
            End If
        Next

        ' Data binding understands how to work with generic lists.
        Me.ListBox1.DataSource = vsList

    End Sub
#End Region

#Region "3. Generics Creation"

    Private Sub pairMatchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pairMatchButton.Click
        ' Variables that store pairs of values of type string.
        ' Try changing type parameters from String to something else like Integer.
        '** go to definition of Pair generic class in Pair.vb **'
        Dim pair1 As New Pair(Of String, String)
        Dim pair2 As New Pair(Of String, String)

        pair1.FirstValue = Me.pair1aTextBox.Text
        pair1.SecondValue = Me.pair1bTextBox.Text

        pair2.FirstValue = Me.pair2aTextbox.Text
        pair2.SecondValue = Me.pair2bTextbox.Text

        '** go to definition of Pair.Matches generic class method in Pair.vb **'
        If pair1.Matches(pair2) Then
            Me.pairResultTextbox.Text = "Yes"
        Else
            Me.pairResultTextbox.Text = "No"
        End If
    End Sub

#End Region

#Region "4. Using Statement"
    Private Sub RequestWebStreamButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RequestWebStreamButton.Click

        Dim htmlText As String = ""
        Dim wclient As New System.Net.WebClient

        ' s stream object is created in this Using block and disposed at the end
        ' It is a good idea to dispose system resources, connections, and streams
        ' s stream is assigned from result of a Web request
        Using s As System.IO.Stream = wclient.OpenRead("http://www.microsoft.com/")
            Dim sReader As New System.IO.StreamReader(s)
            htmlText = sReader.ReadToEnd()
        End Using

        Me.webStreamTextbox.Text = htmlText

    End Sub

#End Region

#Region "5. TryCast and IsNot"

    ''' <remarks>Initializes TreeView control with IDs and values</remarks>
    Private Sub LoadValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadValuesButton.Click
        Dim root, node1, node2 As TreeNode

        ' Root node has a value but no ID.
        root = Me.TreeView1.Nodes.Add("Root")

        ' node1 and node2 contain values and IDs
        ' ID is stored in Tag property, enabling lookup by ID
        node1 = root.Nodes.Add("Value1")
        node1.Tag = "1"

        node2 = root.Nodes.Add("Value2")
        node2.Tag = "2"

        root.Expand()

    End Sub

    ''' <remarks>Displays value and IDs for selected node</remarks>
    Private Sub TreeView1_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect

        Dim selectedID As String

        '**try casting or converting Tag object to a string
        'if the conversion succeeds selectedID will have a valid value;
        'else TryCast will return Nothing
        selectedID = TryCast(e.Node.Tag, String)

        '**test if selectedID is null using IsNot operator
        If selectedID IsNot Nothing Then
            Me.SelectedIDTextbox.Text = selectedID
        Else
            Me.SelectedIDTextbox.Text = "[Nothing]"
        End If

        Me.SelectedValueTextbox.Text = e.Node.Text
    End Sub

#End Region

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
