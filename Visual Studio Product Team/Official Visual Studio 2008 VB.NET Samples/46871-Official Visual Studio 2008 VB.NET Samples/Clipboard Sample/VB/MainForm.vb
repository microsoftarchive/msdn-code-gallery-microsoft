' Copyright (c) Microsoft Corporation. All rights reserved.
Option Explicit On
Option Strict Off

Public Class MainForm

    Dim StringHTML As String
    Dim StringRTF As String
    Dim StringPlainText As String
    Dim Pxl As New Pixel

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Load the picture from My.Resources
        PictureBox1.Image = My.Resources.Sunset

        'Load some text into the web brower
        StringHTML = "<HTML><BODY>This is <B>HTML</B></BODY></HTML>"
        WebBrowser1.DocumentText = StringHTML

        'Load some text in the rich text box
        StringRTF = "{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{" & _
                    "\f0\fswiss\fcharset0 Arial;}}\viewkind4\uc1\pard\f0\fs20\ul\b\fs24 " & _
                    "This\ulnone\b0  \fs20 is \fs36 RTF!\par}"
        RichTextBox1.Rtf = StringRTF

        'Load some text in the plain text box
        StringPlainText = "This is plain text"
        TextBox1.Text = StringPlainText

        'Initialize the properies of the pixel object and show these in a text box
        Pxl.X = 10
        Pxl.Y = 20
        Pxl.Color = Color.Red
        LoadPixel(Pxl, TextBox3)
    End Sub

    Private Sub CopyImageButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyImageButton.Click
        'Copy the picture from the picture box onto the clipboard
        My.Computer.Clipboard.SetImage(PictureBox1.Image)
    End Sub

    Private Sub PasteImageButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteImageButton.Click
        'If image exists in the clipboard, paste it into the picture box
        If My.Computer.Clipboard.ContainsImage Then
            PictureBox2.Image = My.Computer.Clipboard.GetImage
        Else
            MsgBox("Clipboard does not contain an image", MsgBoxStyle.Exclamation And MsgBoxStyle.OKOnly)
        End If
    End Sub

    Private Sub CopyHTMLButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyHTMLButton.Click
        'Copy HTML from web browser onto the clipboard
        My.Computer.Clipboard.SetText(WebBrowser1.DocumentText, TextDataFormat.Html)
    End Sub

    Private Sub PasteHTMLButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteHTMLButton.Click
        'If clipboard has HTML, paste it into the web browser control
        If My.Computer.Clipboard.ContainsText(TextDataFormat.Html) Then
            WebBrowser2.DocumentText = My.Computer.Clipboard.GetText(TextDataFormat.Html)
        Else
            MsgBox("Clipboard does not contain any HTML", MsgBoxStyle.Exclamation And MsgBoxStyle.OKOnly)
        End If
    End Sub

    Private Sub CopyRTFButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyRTFButton.Click
        'Copy text from rich text box onto the clipboard
        My.Computer.Clipboard.SetText(RichTextBox1.Rtf, TextDataFormat.Rtf)
    End Sub

    Private Sub PasteRTFButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteRTFButton.Click
        'If clipboard has rich text, paste it into the rich text box
        If My.Computer.Clipboard.ContainsText(TextDataFormat.Rtf) Then
            RichTextBox2.Rtf = My.Computer.Clipboard.GetText(TextDataFormat.Rtf)
        Else
            MsgBox("Clipboard does not contain any rich text", MsgBoxStyle.Exclamation And MsgBoxStyle.OKOnly)
        End If
    End Sub

    Private Sub CopyTextButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyTextButton.Click
        'Copy text from text box onto the clipboard
        My.Computer.Clipboard.SetText(TextBox1.Text)
    End Sub

    Private Sub PasteTextButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteTextButton.Click
        'If clipboard has text, paste it into the text box
        If My.Computer.Clipboard.ContainsText() Then
            TextBox2.Text = My.Computer.Clipboard.GetText()
        Else
            MsgBox("Clipboard does not contain any text", MsgBoxStyle.Exclamation And MsgBoxStyle.OKOnly)
        End If
    End Sub

    Private Sub CopyObjectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyObjectButton.Click
        'Copy Pixel object onto the clipboard
        My.Computer.Clipboard.SetData("Pixel", Pxl)
    End Sub

    Private Sub PasteObjectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteObjectButton.Click
        Dim tempPixel As Pixel

        'If Clipboard has a pixel object, get it, assign in to another object, and load 
        'the values of the properties of this object into a text box
        If My.Computer.Clipboard.ContainsData("Pixel") Then
            tempPixel = CType(My.Computer.Clipboard.GetData("Pixel"), Pixel)
            LoadPixel(tempPixel, TextBox4)
        Else
            MsgBox("Clipboard does not contain a pixel object", MsgBoxStyle.Exclamation And MsgBoxStyle.OKOnly)
        End If
    End Sub

    Private Sub LoadPixel(ByVal Pixel As Pixel, ByVal Textbox As TextBox)
        'Show values of all properties of the specified pixel object in the specified text box
        Textbox.Text = "X [" & Pixel.X & "] " & vbCrLf & _
                       "Y [" & Pixel.Y & "] " & vbCrLf & _
                       Pixel.Color.ToString
    End Sub

    Private Sub WindowsExplorerLinkLabel_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles WindowsExplorerLinkLabel.LinkClicked
        'Open windows explorer
        Shell("explorer.exe", AppWinStyle.NormalFocus, False)
    End Sub

    Private Sub PasteFilesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteFilesButton.Click
        'If Clipboard has a File Drop List, paste the names of all files in this list in a list box
        If My.Computer.Clipboard.ContainsFileDropList Then
            Dim fileDropList As System.Collections.Specialized.StringCollection
            fileDropList = My.Computer.Clipboard.GetFileDropList()
            For Each fileName As String In fileDropList
                ListBox1.Items.Add(fileName)
            Next
        Else
            MsgBox("Clipboard does not contain a file drop list", MsgBoxStyle.Exclamation And MsgBoxStyle.OKOnly)
        End If
    End Sub

    Private Sub exitToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub clearClipboardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles clearClipboardToolStripMenuItem.Click
        'Clear the clipboard
        My.Computer.Clipboard.Clear()
    End Sub
End Class
