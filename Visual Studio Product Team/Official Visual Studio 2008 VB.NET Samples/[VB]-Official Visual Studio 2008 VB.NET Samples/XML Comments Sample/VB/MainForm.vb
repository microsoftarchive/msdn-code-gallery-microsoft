' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Xml

Public Class MainForm

    ''' <summary>
    ''' This method conforms to the ReplaceSparkPlugs delegate in the Automobile class.
    ''' </summary>
    ''' <param name="description"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReplacePlugs(ByVal description As String) As Boolean

    End Function

    ''' <summary>
    ''' Instance of car used to examine XML comments.
    ''' </summary>
    ''' <remarks></remarks>
    Dim WithEvents car As New Automobile()

    Private Sub createCar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles createCar.Click
        Dim towing As Integer = car.CalculateMaxTowing()
        Dim replace As New Automobile.ReplaceSparkPlugs(AddressOf ReplacePlugs)
        MsgBox("Car created.")
    End Sub

    Private Sub car_NeedsTuneUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles car.NeedsTuneUp

    End Sub

    Private Sub listSummaries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles listSummaries.Click
        ' The XML tags are stored in an XML file that resides with the assembly.
        Dim xmlDoc As New XmlDocument
        xmlDoc.Load(My.Application.Info.DirectoryPath & "\XmlComments.xml")
        Dim output As String = ""
        Dim summary As XmlNode = Nothing
        Dim name As String = ""
        Dim members As XmlNodeList = xmlDoc.GetElementsByTagName("member")
        For Each member As XmlNode In members
            ' The name attribute has the member name.
            name = member.Attributes("name").Value
            ' Make the name more readable.
            name = name.Replace("T:", "Type: ")
            name = name.Replace("M:", "Type: ")
            name = name.Replace("E:", "Event: ")
            name = name.Replace("F:", "Field: ")
            name = name.Replace("P:", "Property: ")
            name = name.Replace("#ctor", "New")
            name = name.Replace("XmlComments.", "")
            output &= name & ":"
            ' The summary element has the summary comment.
            summary = member.SelectSingleNode("summary")
            output &= summary.InnerText & vbCrLf
        Next
        MsgBox(output)
    End Sub

    Private Sub exitToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class

