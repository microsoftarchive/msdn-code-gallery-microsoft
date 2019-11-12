' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Xml

Public Class Browser
    Friend WithEvents tsbWebSite1 As System.Windows.Forms.ToolStripButton

    Dim xmldoc As New XmlDocument

    Private Sub GoToUrl(ByVal sWebsite As String)
        Dim xPath As String = "//WebSite[Name='" & sWebsite & "']/URL"
        Dim xn As XmlNode = xmldoc.SelectSingleNode(xPath)

        WebBrowser1.Url = New Uri(xn.InnerText)
    End Sub

    Private Sub tsbBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbBack.Click
        WebBrowser1.GoBack()
    End Sub

    Private Sub tsbForward_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbForward.Click
        WebBrowser1.GoForward()
    End Sub

    Private Sub tsbHome_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbHome.Click
        WebBrowser1.GoHome()
    End Sub

    Private Sub tsbRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbRefresh.Click
        WebBrowser1.Refresh()
    End Sub

    Private Sub tsbManage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbManage.Click
        Dim frmManage As New Sites
        frmManage.ShowDialog()

        AddToolStripButtons()
    End Sub

    Private Sub frmBrowser_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AddToolStripButtons()
    End Sub

    Private Sub AddTSButton(ByVal buttonName As String, ByVal webSiteName As String)
        Dim newbutton As New System.Windows.Forms.ToolStripButton

        newbutton.Name = buttonName
        newbutton.Text = webSiteName
        newbutton.DisplayStyle = ToolStripItemDisplayStyle.Text

        ToolStrip3.Items.Add(newbutton)

        AddHandler newbutton.Click, AddressOf tsbutton_Click

    End Sub

    Private Sub AddToolStripButtons()
        ' Load the XML file with user defined Web sites.
        xmldoc.Load(My.Application.Info.DirectoryPath & "\" & My.Settings.XmlFileName)

        ' Empty the toolstrip with the user defined Web sites
        ToolStrip3.Items.Clear()

        ' Loop through the XML file and add a toolstrip button for each
        ' user defined Web site.
        Dim websiteNodes As XmlNodeList
        Dim webSiteNode As XmlNode
        websiteNodes = xmldoc.GetElementsByTagName("WebSite")
        Dim iCount As Integer = 1

        For Each webSiteNode In websiteNodes
            Dim eachSiteNodes As XmlNodeList
            Dim eachSiteNode As XmlNode
            eachSiteNodes = webSiteNode.ChildNodes

            For Each eachSiteNode In eachSiteNodes
                If eachSiteNode.Name = "Name" Then
                    AddTSButton("tsWebsite" & iCount.ToString, eachSiteNode.InnerText)
                    iCount += 1
                End If
            Next
        Next

    End Sub

    Private Sub tsbutton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim aButton As ToolStripButton = CType(sender, ToolStripButton)
        GoToUrl(aButton.Text)
    End Sub

    Private Sub tsbMSDN_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbMSDN.Click
        WebBrowser1.Url = New Uri("http://msdn.microsoft.com")
    End Sub

    Private Sub tsbVisualBasic_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbVisualBasic.Click
        WebBrowser1.Url = New Uri("http://msdn.microsoft.com/vbasic")
    End Sub

    Private Sub tsbVisualStudio_DisplayStyleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbVisualStudio.DisplayStyleChanged
        WebBrowser1.Url = New Uri("http://msdn.microsoft.com/vstudio")
    End Sub

    Private Sub tsbGo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsbGo.Click
        Try
            WebBrowser1.Url = New Uri(txtUrl.Text)
        Catch
            MsgBox("Could not connect to site. " & _
                "One possible problem is that the address does not begin with http://")
        End Try
    End Sub
End Class
