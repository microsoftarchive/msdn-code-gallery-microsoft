' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Text
Imports System.IO

Public Class MainForm
    Inherits System.Windows.Forms.Form


    Private Enum SelectedMethod
        Compare
        CompareOrdinal
        Concat
        EndsWith
        Format
        IndexOf
        IndexOfAny
        Insert
        Join
        LastIndexOf
        LastIndexOfAny
        PadLeft
        PadRight
        Remove
        Replace
        Split
        StartsWith
        Substring
        ToLower
        ToUpper
        Trim
        TrimEnd
        TrimStart
    End Enum

    Private sm As SelectedMethod

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        txtResults.Clear()
    End Sub

    Private Sub btnRecalc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRecalc.Click
        Recalc(FindSelectedButton())
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        RefreshText()
    End Sub

    Private Sub btnStringBuilder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStringBuilder.Click
        ' Demonstrate the StringBuilder class.

        If chkDebug.Checked Then
            ' Trigger a breakpoint.
            System.Diagnostics.Debugger.Break()
        End If

        ' StringBuilder is provided by the System.Text 
        ' namespace. See the Imports statement
        ' at the top of this module.
        Dim sb As New StringBuilder("The quick brown fox jumps over the lazy dog")

        sb.Insert(19, " happily")
        sb.Remove(10, 6)
        sb.Replace("jumps", "leaps")
        sb.AppendFormat(" {0} times in {1} minutes", 17, 2)

        ' Suppose you want to insert a comma after the word "dog".
        ' You need to locate the word, then insert a comma
        ' after the word. To locate the string, you must search
        ' using IndexOf, but StringBuilder doesn't supply this method.
        ' Therefore, you must use the ToString method to retrieve
        ' the string, and work with that object instead.
        Dim intPos As Integer
        intPos = sb.ToString.IndexOf("dog")
        If intPos > 0 Then
            ' Insert the comma at the position
            ' you found + the length of the text "dog".
            sb.Insert(intPos + "dog".Length, ", ")
        End If

        ' The same code using the String object directly
        ' would look like this. Note the number of times 
        ' you require the .NET framework to create new strings.
        ' StringBuilder is significantly more efficient for
        ' this sort of operation.
        Dim str As String = "The quick brown fox jumps over the lazy dog"
        str = str.Insert(19, " happily")
        str = str.Remove(10, 6)
        str = str.Replace("jumps", "leaps")
        str &= String.Format("{0} times in {1} minutes", 17, 2)
        intPos = str.IndexOf("dog")
        If intPos > 0 Then
            str = str.Insert(intPos + "dog".Length, ", ")
        End If

        txtResults.Text = ""
        txtResults.AppendText("StringBuilder output: " & vbCrLf & vbTab & sb.ToString & Environment.NewLine)
        txtResults.AppendText("String output: " & vbCrLf & vbTab & str & Environment.NewLine)
    End Sub

    Private Sub btnStringWriter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStringWriter.Click
        ' Simple demonstration of the StringWriter class.
        ' Write to the StringWriter as if you were 
        ' writing to a file--you're really writing to 
        ' a buffer in memory, instead.

        ' StringWriter is provided by the System.IO
        ' namespace. See the Imports statement
        ' at the top of this module.
        Dim sw As New StringWriter()

        If chkDebug.Checked Then
            ' Trigger a breakpoint.
            System.Diagnostics.Debugger.Break()
        End If

        ' You've been supplied with some data structure,
        ' perhaps an array of strings, containing
        ' address information. Create a formatted
        ' address.
        Dim addressInfo() As String = {"John Smith", "123 Main Street", "Centerville", "WA", "98111"}

        ' Add in the name and address lines:
        sw.WriteLine(addressInfo(0))
        sw.WriteLine(addressInfo(1))

        ' You might use the String.Format method to create
        ' this line of the address, but this demo 
        ' shows how to use both the WriteLine and the
        ' Write methods. Although this is somewhat contrived, you 
        ' can at least see how to use the two methods:
        sw.Write(addressInfo(2))
        sw.Write(", ")
        sw.Write(addressInfo(3))
        sw.Write(" ")
        sw.WriteLine(addressInfo(4))

        ' Or:
        ' sw.WriteLine(String.Format("{0}, {1} {2}", addressInfo(2), addressInfo(3), addressInfo(4)))

        ' Using the String class, the code's uglier:
        Dim str As String
        str = addressInfo(0) & Environment.NewLine
        str &= addressInfo(1) & Environment.NewLine

        ' Add on the city/region/postal code values:
        str &= addressInfo(2) & ", "
        str &= addressInfo(3) & " " & addressInfo(4)
        str &= Environment.NewLine

        ' Or: 
        ' str &= String.Format("{0}, {1} {2}{3}", addressInfo(2), addressInfo(3), addressInfo(4), Environment.NewLine)

        ' Display the output:
        txtResults.Text = ""
        txtResults.AppendText("StringWriter output: " & _
            Environment.NewLine & sw.ToString)

        txtResults.AppendText("String output: " & _
            Environment.NewLine & str)

    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Initialize values.
        ' Selected page:
        tabStringDemo.SelectedIndex = 0

        ' Create the method objects.
        CreateMethodData()

        MethodCall.DisplayForm = Me
        Me.DisplayText(FindSelectedButton)
    End Sub

    Private Sub tabStringDemo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tabStringDemo.SelectedIndexChanged
        Dim pagSelected As TabPage = tabStringDemo.SelectedTab
        ' Move the panel containing the info to the current page. Don't do 
        ' this for the "other" page, however.
        If Not pagSelected Is pagOther Then
            pnlDemo.Parent = pagSelected
        End If
        DisplayText(FindSelectedButton())
    End Sub

    Private Sub DisplayText(ByVal selectedButton As RadioButton)
        If selectedButton Is Nothing Then
            Return
        End If
        ' Take action depending on which radio button you've selected.
        Dim method As MethodCall = CType(selectedButton.Tag, MethodCall)
        Try
            MethodCall.DisplayForm = Me
            If method IsNot Nothing Then
                method.DisplayOnScreen()
                Me.lblResults.Text = method.GetResult().ToString
                Me.lblResultsLabel.Text = method.FormatFunctionCall
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub HandleCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optInsert.CheckedChanged, optPadLeft.CheckedChanged, optPadRight.CheckedChanged, optRemove.CheckedChanged, optReplace.CheckedChanged, optSubstring.CheckedChanged, optToLower.CheckedChanged, optToUpper.CheckedChanged, optTrim.CheckedChanged, optTrimEnd.CheckedChanged, optTrimStart.CheckedChanged, optEndsWith.CheckedChanged, optIndexOf.CheckedChanged, optIndexOfAny.CheckedChanged, optLastIndexOfAny.CheckedChanged, optStartsWith.CheckedChanged, optLastIndexOf.CheckedChanged, optCompare.CheckedChanged, optCompareOrdinal.CheckedChanged, optConcat.CheckedChanged, optFormat.CheckedChanged, optJoin.CheckedChanged, optSplit.CheckedChanged
        Dim selectedButton As RadioButton = CType(sender, RadioButton)
        If selectedButton.Checked Then
            DisplayText(CType(sender, RadioButton))
        End If
    End Sub

    Private Sub Recalc(ByVal selectedButton As RadioButton)
        Dim method As MethodCall = CType(selectedButton.Tag, MethodCall)
        Try
            If Not method.IsShared Then
                method.Sample = Me.txtSample.Text
            End If

            If Me.txtPrm1.Visible Then
                method.Parameter1.Value = Me.txtPrm1.Text
            End If
            If Me.txtPrm2.Visible Then
                method.Parameter2.Value = Me.txtPrm2.Text
            End If
            If Me.txtPrm3.Visible Then
                method.Parameter3.Value = Me.txtPrm3.Text
            End If

            If method IsNot Nothing Then
                method.DisplayOnScreen()
                Me.lblResults.Text = method.GetResult().ToString
                Me.lblResultsLabel.Text = method.FormatFunctionCall
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub RefreshText()
        Try
            Dim selectedButton As RadioButton = FindSelectedButton()
            Dim method As MethodCall = CType(selectedButton.Tag, MethodCall)
            method.ResetToDefaults()
            method.DisplayOnScreen()
            Me.lblResults.Text = method.GetResult().ToString
            Me.lblResultsLabel.Text = method.FormatFunctionCall
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub SetResultsLabel(ByVal Source As String, ByVal Method As String, ByVal Parameters As String)
        lblResultsLabel.Text = String.Format("Results of {0}.{1}({2})", Source, Method, Parameters)
    End Sub

    Private Function FindSelectedButton() As RadioButton
        For Each aControl As Control In Me.tabStringDemo.SelectedTab.Controls
            If aControl.Name.StartsWith("opt") Then
                Dim rButton As RadioButton = CType(aControl, RadioButton)
                If rButton.Checked Then
                    Return rButton
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Sub CreateMethodData()
        Dim foxString As String = "the quick brown fox jumps over the lazy dog"

        optCompare.Tag = New MethodCall( _
            New Parameter("strA", "This is a test", ParameterTypes.StringParameter), _
            New Parameter("strB", "this is a test", ParameterTypes.StringParameter), _
            Nothing, Methods.Compare)
        optCompareOrdinal.Tag = New MethodCall( _
            New Parameter("strA", "This is a test", ParameterTypes.StringParameter), _
            New Parameter("strB", "this is a test", ParameterTypes.StringParameter), _
            Nothing, Methods.CompareOrdinal)
        optConcat.Tag = New MethodCall( _
            New Parameter("strA", "This is a test", ParameterTypes.StringParameter), _
            New Parameter("strB", " of how this works", ParameterTypes.StringParameter), _
            New Parameter("strC", " when you concatenate.", ParameterTypes.StringParameter), _
            Methods.Concat)
        optEndsWith.Tag = New MethodCall( _
            foxString, _
            New Parameter("value", "dog", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.EndsWith)
        optFormat.Tag = New MethodCall( _
            New Parameter("Format", "Your {0:N0} items total {1:C}.", ParameterTypes.StringParameter), _
            New Parameter("Param1", 12, ParameterTypes.IntegerParameter), _
            New Parameter("Param2", 14, ParameterTypes.IntegerParameter), _
            Methods.Format)
        optIndexOf.Tag = New MethodCall( _
            foxString, _
            New Parameter("value", "o", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.IndexOf)
        optIndexOfAny.Tag = New MethodCall( _
            foxString, _
            New Parameter("anyOf()", "abc", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.IndexOfAny)
        optInsert.Tag = New MethodCall( _
            foxString, _
            New Parameter("startIndex", 19, ParameterTypes.IntegerParameter), _
            New Parameter("value", " happily", ParameterTypes.StringParameter), _
            Nothing, Methods.Insert)
        optJoin.Tag = New MethodCall( _
            New Parameter("separator", "-", ParameterTypes.StringParameter), _
            New Parameter("values()", "item1,item2,item3,item4", ParameterTypes.StringParameter), _
            Nothing, Methods.Join)
        optLastIndexOf.Tag = New MethodCall( _
            foxString, _
            New Parameter("value", "o", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.LastIndexOf)
        optLastIndexOfAny.Tag = New MethodCall( _
            foxString, _
            New Parameter("anyOf()", "abc", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.LastIndexOfAny)
        optPadLeft.Tag = New MethodCall( _
            "123.45", _
            New Parameter("totalWidth", 10, ParameterTypes.IntegerParameter), _
            New Parameter("paddingChar", "$", ParameterTypes.StringParameter), _
            Nothing, Methods.PadLeft)
        optPadRight.Tag = New MethodCall( _
            "pad", _
            New Parameter("totalWidth", 10, ParameterTypes.IntegerParameter), _
            New Parameter("paddingChar", "_", ParameterTypes.StringParameter), _
            Nothing, Methods.PadRight)
        optRemove.Tag = New MethodCall( _
            foxString, _
            New Parameter("startIndex", 10, ParameterTypes.IntegerParameter), _
            New Parameter("count", 6, ParameterTypes.IntegerParameter), _
            Nothing, Methods.Remove)
        optReplace.Tag = New MethodCall( _
            foxString, _
            New Parameter("oldValue", "jumps", ParameterTypes.StringParameter), _
            New Parameter("newValue", "leaps", ParameterTypes.StringParameter), _
            Nothing, Methods.Replace)
        optSplit.Tag = New MethodCall( _
            foxString, _
            New Parameter("separator()", "abc", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.Split)
        optStartsWith.Tag = New MethodCall( _
            foxString, _
            New Parameter("value", "The qui", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.StartsWith)
        optSubstring.Tag = New MethodCall( _
            foxString, _
            New Parameter("startIndex", 12, ParameterTypes.IntegerParameter), _
            New Parameter("length", 5, ParameterTypes.IntegerParameter), _
            Nothing, Methods.Substring)
        optToLower.Tag = New MethodCall( _
            "This Sample has SOME mixed-CASE Text!", Nothing, Nothing, Nothing, Methods.ToLower)
        optToUpper.Tag = New MethodCall( _
            "This Sample has SOME mixed-CASE Text!", Nothing, Nothing, Nothing, Methods.ToUpper)
        optTrim.Tag = New MethodCall( _
            foxString, _
            New Parameter("trimChars()", "the dog", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.Trim)
        optTrimEnd.Tag = New MethodCall( _
            foxString, _
            New Parameter("trimChars()", "the dog", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.TrimEnd)
        optTrimStart.Tag = New MethodCall( _
            foxString, _
            New Parameter("trimChars()", "the dog", ParameterTypes.StringParameter), _
            Nothing, Nothing, Methods.TrimStart)
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class