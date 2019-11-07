Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports SampleQueries.SampleSupport


Public Class SampleForm
    Private currentHarness As SampleHarness
    Private currentSample As Sample

    Private Shared Sub ColorizeCode(ByVal rtb As RichTextBox)
        Dim textArray As String() = New String() {"AddHandler", "AddressOf", "Alias", "And", "AndAlso", "", "Ansi", "As", "Assembly", "Auto", "Boolean", "ByRef", "Byte", "ByVal", "Call", "Case", "Catch", "CBool", "CByte", "CChar", "CDate", "ec", "CDbl", "Char", "CInt", "Class", "CLng", "CObj", "Const", "CShort", "CSng", "CStr", "CType", "Date", "Decimal", "Declare", "Default", "Delegate", "Dim", "DirectCast", "Do", "Double", "Each", "Else", "ElseIf", "End", "Enum", "Erase", "Error", "ent", "Exit", "False", "Finally", "For", "Friend", "Function", "Get", "GetType", "GoSub", "GoTo", "Handles", "If", "Implements", "Imports", "In", "Inherits", "Integer", "Interface", "Is", "Let", "Lib", "Like", "Long", "Loop", "Me", "Mod", "Module", "MustInherit", "MustOverride", "MyBase", "MyClass", "Namespace", "New", "Next", "Not", "Nothing", "NotInheritable", "NotOverridable", "Object", "On", "Option", "Optional", "Or", "OrElse", "Overloads", "Overridable", "Overrides", "ParamArray", "Preserve", "Private", "Property", "Protected", "Public", "RaiseEvent", "ReadOnly", "ReDim", "REM", "RemoveHandler", "Resume", "Return", "Select", "Set", "Shadows", "Shared", "Short", "Single", "Static", "Step", "Stop", "String", "Structure", "Sub", "SyncLock", "Then", "Throw", "To", "True", "Try", "TypeOf", "Unicode", "Until", "Variant", "When", "While", "With", "WithEvents", "WriteOnly", "Xor", "ExternalSource", "Region", "GoSub", "Let", "Variant", "From", "In", "Where", "Order", "Group", "Ascending", "Descending", "Distinct", "IIf", "If", "By", "Take", "Skip", "Equals", "Join", "Into", "IsNot", "Aggregate"}
        Dim text As String = rtb.Text
        rtb.SelectAll()
        rtb.SelectionColor = rtb.ForeColor
        Dim keyword As String
        For Each keyword In textArray
            Dim keywordPos As Integer = rtb.Find(keyword, RichTextBoxFinds.WholeWord)
            While (keywordPos <> -1)
                Dim commentPos As Integer = text.LastIndexOf("'", keywordPos)
                Dim newLinePos As Integer = text.LastIndexOf(Chr(10), keywordPos)
                Dim quoteCount As Integer = 0
                Dim quotePos As Integer = text.IndexOf("\'", newLinePos + 1, keywordPos - newLinePos)
                While (quotePos <> -1)
                    quoteCount += 1
                    quotePos = text.IndexOf("\'", quotePos + 1, keywordPos - (quotePos + 1))
                End While
                If (newLinePos >= commentPos AndAlso quoteCount Mod 2 = 0) Then
                    rtb.SelectionColor = Color.Blue

                End If
                keywordPos = rtb.Find(keyword, keywordPos + rtb.SelectionLength, RichTextBoxFinds.WholeWord)
            End While
        Next
        rtb.Select(0, 0)
    End Sub


    Private Sub runButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles runButton.Click
        If (currentHarness IsNot Nothing) Then
            MyBase.UseWaitCursor = True
            outputTextBox.Text = ""
            Dim newOut As StreamWriter = currentHarness.OutputStreamWriter
            Dim out As TextWriter = Console.Out
            Console.SetOut(newOut)
            Dim baseStream As MemoryStream = DirectCast(newOut.BaseStream, MemoryStream)
            baseStream.SetLength(CLng(0))
            currentSample.InvokeSafe()
            newOut.Flush()
            Console.SetOut(out)
            outputTextBox.Text = (outputTextBox.Text & newOut.Encoding.GetString(baseStream.ToArray))
            MyBase.UseWaitCursor = False
        End If
    End Sub

    Private Sub samplesTreeView_AfterCollapse(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles samplesTreeView.AfterCollapse
        Select Case e.Node.Level
            Case 1
                e.Node.ImageKey = "BookStack"
                e.Node.SelectedImageKey = "BookStack"
                Return
            Case 2
                e.Node.ImageKey = "BookClose"
                e.Node.SelectedImageKey = "BookClose"
                Return
        End Select
    End Sub

    Private Sub samplesTreeView_AfterExpand(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles samplesTreeView.AfterExpand
        Select Case e.Node.Level
            Case 1, 2
                e.Node.ImageKey = "BookOpen"
                e.Node.SelectedImageKey = "BookOpen"
                Return
        End Select
    End Sub


    Private Sub samplesTreeView_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles samplesTreeView.AfterSelect
        Dim selectedNode As TreeNode = samplesTreeView.SelectedNode
        currentSample = DirectCast(selectedNode.Tag, Sample)
        If (currentSample IsNot Nothing) Then
            currentHarness = currentSample.Harness
            runButton.Enabled = True
            descriptionTextBox.Text = currentSample.Description
            codeRichTextBox.Clear()
            codeRichTextBox.Text = currentSample.Code
            SampleForm.ColorizeCode(codeRichTextBox)
            outputTextBox.Clear()
        Else
            currentHarness = Nothing
            runButton.Enabled = False
            descriptionTextBox.Text = "Select a query from the tree to the left."
            codeRichTextBox.Clear()
            outputTextBox.Clear()
            If ((e.Action <> TreeViewAction.Collapse) AndAlso (e.Action <> TreeViewAction.Unknown)) Then
                e.Node.Expand()
            End If
        End If
    End Sub

    Private Sub samplesTreeView_BeforeCollapse(ByVal sender As Object, ByVal e As TreeViewCancelEventArgs) Handles samplesTreeView.BeforeCollapse
        If (e.Node.Level = 0) Then
            e.Cancel = True
        End If
    End Sub

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Dim title = "LINQ VB Project Sample Query Explorer"

        '******************************************
        Dim harnesses As New List(Of SampleHarness)
        harnesses.Add(New LinqSamples())
        harnesses.Add(New LinqToSQLSamples())
        harnesses.Add(New LinqToDataSetSamples())
        harnesses.Add(New LinqToXMLSamples())
        harnesses.Add(New XQueryUseCases())
        '******************************************


        currentHarness = Nothing
        currentSample = Nothing
        Text = title
        Dim tn As New TreeNode(title)
        tn.Tag = Nothing
        tn.ImageKey = "BookStack"
        tn.SelectedImageKey = "BookStack"
        samplesTreeView.Nodes.Add(tn)
        tn.Expand()
        Dim harness As SampleHarness
        For Each harness In harnesses
            Dim node2 As New TreeNode(harness.Title)
            node2.Tag = Nothing
            node2.ImageKey = "BookStack"
            node2.SelectedImageKey = "BookStack"
            tn.Nodes.Add(node2)
            Dim category As String = ""
            Dim node3 As TreeNode = Nothing
            Dim sample As Sample
            For Each sample In harness
                If (sample.Category <> category) Then
                    category = sample.Category
                    node3 = New TreeNode(category)
                    node3.Tag = Nothing
                    node3.ImageKey = "BookClose"
                    node3.SelectedImageKey = "BookClose"
                    node2.Nodes.Add(node3)
                End If
                Dim node4 As New TreeNode(sample.ToString)
                node4.Tag = sample
                node4.ImageKey = "Run"
                node4.SelectedImageKey = "Run"
                node3.Nodes.Add(node4)
            Next
        Next
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If codeRichTextBox.Text <> "" Then My.Computer.Clipboard.SetText(codeRichTextBox.Text)
    End Sub

End Class