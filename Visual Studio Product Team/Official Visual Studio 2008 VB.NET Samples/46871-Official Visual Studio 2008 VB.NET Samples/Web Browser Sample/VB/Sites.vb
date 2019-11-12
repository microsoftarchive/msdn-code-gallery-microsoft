' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Xml

Public Class Sites

    Private Sub frmSites_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadList()
    End Sub

    Private Sub LoadList()
        lvwWebSites.Items.Clear()

        Dim rdrXML As New XmlTextReader(My.Application.Info.DirectoryPath & "\" & My.Settings.XmlFileName)
        rdrXML.MoveToContent()

        Dim ElementName As String = ""
        Dim NextItem As Boolean = True
        Dim objListViewItem As ListViewItem = Nothing

        Do While rdrXML.Read
            If NextItem Then
                objListViewItem = New ListViewItem
                NextItem = False
            End If
            Select Case rdrXML.NodeType
                Case XmlNodeType.Element
                    ElementName = rdrXML.Name
                Case XmlNodeType.Text
                    If ElementName = "Name" Then
                        objListViewItem.Text = rdrXML.Value
                    End If
                    If ElementName = "URL" Then
                        objListViewItem.SubItems.Add(rdrXML.Value)
                        lvwWebSites.Items.Add(objListViewItem)
                        NextItem = True
                    End If
            End Select
        Loop
        rdrXML.Close()
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Dim wtrXML As New XmlTextWriter(My.Application.Info.DirectoryPath & "\" & My.Settings.XmlFileName, System.Text.Encoding.UTF8)
        With wtrXML
            .Formatting = Formatting.Indented
            .WriteStartDocument()
            .WriteStartElement("WebSites")
            Dim objListViewItem As New ListViewItem
            For Each objListViewItem In lvwWebSites.Items
                .WriteStartElement("WebSite")
                .WriteElementString("Name", objListViewItem.Text)
                .WriteElementString("URL", objListViewItem.SubItems(1).Text)
                .WriteEndElement()
            Next
            .WriteEndElement()
            .WriteEndDocument()
            .Flush()
            .Close()
        End With

        Me.Close()
    End Sub

    Private Sub btnAddSite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddSite.Click
        Dim objListViewItem As New ListViewItem
        objListViewItem.Text = txtWebSite.Text
        objListViewItem.SubItems.Add(txtURL.Text)
        lvwWebSites.Items.Add(objListViewItem)

        txtWebSite.Text = ""
        txtURL.Text = ""
    End Sub

    Private Sub btnRemoveSite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveSite.Click
        Dim indexes As ListView.SelectedIndexCollection = lvwWebSites.SelectedIndices
        Dim index As Integer
        For Each index In indexes
            lvwWebSites.Items.RemoveAt(index)
        Next
    End Sub

End Class