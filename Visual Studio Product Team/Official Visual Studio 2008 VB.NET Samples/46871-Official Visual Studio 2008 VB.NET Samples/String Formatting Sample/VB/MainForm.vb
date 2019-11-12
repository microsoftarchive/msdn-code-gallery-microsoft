' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Threading
Imports System.Globalization
Imports System.Text


Public Class MainForm

    Public Class Culture
        Private IdValue As String
        Private descValue As String

        Sub New(ByVal strDesc As String, ByVal strID As String)
            IdValue = strID
            descValue = strDesc
        End Sub

        Public ReadOnly Property ID() As String
            Get
                Return IdValue
            End Get
        End Property

        Public ReadOnly Property Description() As String
            Get
                Return descValue
            End Get
        End Property
    End Class

    Private formHasLoaded As Boolean = False
    Private cultureValue As String

    ' Calls the method to display the DateTime formatting examples based on a 
    ' user-selected CultureInfo.
    Private Sub cboCultureInfoDateTime_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCultureInfoDateTime.SelectedIndexChanged
        ' Handler should work only if the Form has loaded as SelectedValueChanged 
        ' fires during databinding and causes undesirable results.
        If formHasLoaded Then
            LoadDateTimeFormats()
        End If
    End Sub

    ' Calls the method to display the Numeric formatting examples based on a 
    ' user-selected CultureInfo.
    Private Sub cboCultureInfoNumeric_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCultureInfoNumeric.SelectedIndexChanged
        ' Handler should work only if the Form has loaded as SelectedValueChanged 
        ' fires during databinding and causes undesirable results.
        If formHasLoaded Then
            LoadNumericFormats()
        End If
    End Sub

    ' Loads the ComboBox controls from an ArrayList and calls the methods to display 
    ' the various formatting examples.
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Databind the ComboBox controls to an ArrayList of custom objects. Refer to
        ' the comments about the Culture class for more information.
        Dim arlCultureInfo As New ArrayList()
        With arlCultureInfo
            .Add(New Culture("English - United States", "en-US"))
            .Add(New Culture("English - United Kingdom", "en-GB"))
            .Add(New Culture("English - New Zealand", "en-NZ"))
            .Add(New Culture("German - Germany", "de-DE"))
            .Add(New Culture("Spanish - Spain", "es-ES"))
            .Add(New Culture("French - France", "fr-FR"))
            .Add(New Culture("Portuguese - Brazil", "pt-BR"))
            .Add(New Culture("Malay - Malaysia", "ms-MY"))
            .Add(New Culture("Afrikaans - South Africa", "af-ZA"))
        End With

        cboCultureInfoDateTime.DataSource = arlCultureInfo
        cboCultureInfoDateTime.DisplayMember = "Description"
        cboCultureInfoDateTime.ValueMember = "ID"

        cboCultureInfoNumeric.DataSource = arlCultureInfo
        cboCultureInfoNumeric.DisplayMember = "Description"
        cboCultureInfoNumeric.ValueMember = "ID"

        LoadEnumFormats()
        LoadDateTimeFormats()
        LoadNumericFormats()

        formHasLoaded = True
    End Sub

    ' Calls the methods to display the formatting examples based on whether
    ' the user selects "standard" or "custom" formatting.
    Private Sub RadioButtons_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optCustomNumeric.CheckedChanged, optCustomDateTime.CheckedChanged, optStandardDateTime.CheckedChanged, optStandardNumeric.CheckedChanged
        ' Handler should work only if the Form has loaded as SelectedValueChanged 
        ' fires during databinding and causes undesirable results.
        If formHasLoaded Then
            Dim opt As RadioButton = CType(sender, RadioButton)
            Select Case opt.Name
                Case "optStandardNumeric", "optCustomNumeric"
                    LoadNumericFormats()
                Case "optStandardDateTime", "optCustomDateTime"
                    LoadDateTimeFormats()
                Case Else
                    LoadEnumFormats()
            End Select
        End If
    End Sub

    ' Displays the DateTime formatting examples.
    Private Sub LoadDateTimeFormats()
        Dim dtmNow As DateTime = Now
        Dim sb As New StringBuilder()
        cultureValue = cboCultureInfoDateTime.SelectedValue.ToString
        Thread.CurrentThread.CurrentCulture = New CultureInfo(cultureValue)

        sb.Append("When using " & cultureValue & " CultureInfo, today's date and time will format as follows:")
        sb.Append(vbCrLf)
        sb.Append(vbCrLf)

        If optStandardDateTime.Checked Then
            AppendLine(sb, dtmNow.ToString("d"), " [Short date pattern]")
            AppendLine(sb, dtmNow.ToString("D"), " [Long date pattern]")
            AppendLine(sb, dtmNow.ToString("t"), " [Short time pattern]")
            AppendLine(sb, dtmNow.ToString("T"), " [Long time pattern]")
            AppendLine(sb, dtmNow.ToString("F"), " [Full date/time pattern (long)]")
            AppendLine(sb, dtmNow.ToString("f"), " [Full date/time pattern (short)]")
            AppendLine(sb, dtmNow.ToString("G"), " [General date/time pattern (long)]")
            AppendLine(sb, dtmNow.ToString("g"), " [General date/time pattern (short)]")
            AppendLine(sb, dtmNow.ToString("M"), " [Month day pattern]")
            AppendLine(sb, dtmNow.ToString("R"), " [RFC1123 pattern]")
            AppendLine(sb, dtmNow.ToString("s"), " [Sortable date/time pattern]")
            AppendLine(sb, dtmNow.ToString("u"), " [Universable sortable date/time pattern]")
            AppendLine(sb, dtmNow.ToString("y"), " [Year month pattern]")
        ElseIf optCustomDateTime.Checked Then
            AppendLine(sb, dtmNow.ToString("d, M"), " [d, M]")
            AppendLine(sb, dtmNow.ToString("d MMMM"), " [d MMMM]")
            AppendLine(sb, dtmNow.ToString("dddd MMMM yy gg"), " [dddd MMMM yy gg]")
            AppendLine(sb, dtmNow.ToString("h , m: s"), " [h , m: s]")
            AppendLine(sb, dtmNow.ToString("hh,mm:ss"), " [hh,mm:ss]")
            AppendLine(sb, dtmNow.ToString("HH-mm-ss-tt"), " [HH-mm-ss-tt]")
            AppendLine(sb, dtmNow.ToString("hh:mm, G\MT z"), " [hh:mm, G\MT z]")
            AppendLine(sb, dtmNow.ToString("hh:mm, G\MT zzz"), " [hh:mm, G\MT zzz]")
        End If
        sb.Append(vbCrLf)

        txtDateTime.Text = sb.ToString
    End Sub

    ' Displays the Enum formatting examples.
    Private Sub LoadEnumFormats()
        Dim day As DayOfWeek = DayOfWeek.Friday
        Dim sb As New StringBuilder()
        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")

        sb.Append("When using any CultureInfo, the system enumeration DayOfWeek.Friday will format as follows:")
        sb.Append(vbCrLf)
        sb.Append(vbCrLf)
        AppendLine(sb, day.ToString("G"), " [G or g]")
        AppendLine(sb, day.ToString("F"), " [F or f]")
        AppendLine(sb, day.ToString("D"), " [D or d]")
        AppendLine(sb, day.ToString("X"), " [X or x]")

        txtEnum.Text = sb.ToString
    End Sub

    ' Displays the Numeric formatting examples.
    Private Sub LoadNumericFormats()
        Dim intNumber As Int32 = 1234567890
        Dim sb As New StringBuilder()
        cultureValue = cboCultureInfoNumeric.SelectedValue.ToString
        Thread.CurrentThread.CurrentCulture = New CultureInfo(cultureValue)

        sb.Append("When using " & cultureValue & " CultureInfo, the Integer 1234567890 will format as follows:")
        sb.Append(vbCrLf)
        sb.Append(vbCrLf)

        If optStandardNumeric.Checked Then
            AppendLine(sb, intNumber.ToString("C"), " [Currency]")
            AppendLine(sb, intNumber.ToString("E"), " [Scientific (Exponential)]")
            AppendLine(sb, intNumber.ToString("P"), " [Percent]")
            AppendLine(sb, intNumber.ToString("N"), " [Number]")
            AppendLine(sb, intNumber.ToString("F"), " [Fixed-point]")
            AppendLine(sb, intNumber.ToString("X"), " [Hexadecimal]")
        ElseIf optCustomNumeric.Checked Then
            AppendLine(sb, intNumber.ToString("#####"), " [#####]")
            AppendLine(sb, intNumber.ToString("00000"), " [00000]")
            AppendLine(sb, intNumber.ToString("(###) ### - ####"), "[(###) ### - ####]")
            AppendLine(sb, intNumber.ToString("#.##"), " [#.##]")
            AppendLine(sb, intNumber.ToString("00.00"), " [00.00]")
            AppendLine(sb, intNumber.ToString("#,#"), " [#,#]")
            AppendLine(sb, intNumber.ToString("#,,"), " [#,,]")
            AppendLine(sb, intNumber.ToString("#.##"), " [#.##]")
            AppendLine(sb, intNumber.ToString("#,,,"), " [#,,,]")
            AppendLine(sb, intNumber.ToString("#,##0,,"), " [#,##0,,]")
            AppendLine(sb, intNumber.ToString("#0.##%"), " [#0.##%]")
            AppendLine(sb, intNumber.ToString("0.###E+000"), " [0.###E+000]")
            AppendLine(sb, intNumber.ToString("##;(##)"), " [##;(##)]")
        End If
        sb.Append(vbCrLf)

        txtNumeric.Text = sb.ToString
    End Sub

    Private Sub AppendLine(ByVal sb As StringBuilder, ByVal string1 As String, ByVal string2 As String)
        sb.Append(string1)
        sb.Append(string2)
        sb.Append(vbCrLf)
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
