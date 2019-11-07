Public Class SessionInfoDataSource
    Private Shared filePath As String = HttpContext.Current.Server.MapPath("~/App_Data/SessionInfo.xml")

    Private Shared SessionInfoXDoc As XDocument

    Public Sub New()
        SessionInfoXDoc = XDocument.Load(filePath)
    End Sub

    ''' <summary>
    ''' Insert SessionInfo message to XML file
    ''' </summary>
    ''' <param name="sessionInfo"></param>
    ''' <returns></returns>
    Public Sub InsertSessionInfo(sessionInfo As SessionInfo)
        SessionInfoXDoc.Root.Add(convertSessionInfoToXElement(sessionInfo))
    End Sub

    ''' <summary>
    ''' Save data source changes
    ''' </summary>
    Public Sub Save()
        SessionInfoXDoc.Save(filePath)
    End Sub

    ''' <summary>
    ''' Convert Class to XML message
    ''' </summary>
    ''' <param name="sessionInfo"></param>
    ''' <returns></returns>
    Private Function convertSessionInfoToXElement(sessionInfo As SessionInfo) As XElement
        If sessionInfo IsNot Nothing Then
            Dim xDoc As New XElement("SessionInfo", New XElement("SessionValue", sessionInfo.SessionValue), New XElement("BrowserCloseTime", sessionInfo.BrowserClosedTime.ToString("MM/dd/yyyy HH:mm:ss")))
            Return xDoc
        End If
        Return Nothing
    End Function
End Class
