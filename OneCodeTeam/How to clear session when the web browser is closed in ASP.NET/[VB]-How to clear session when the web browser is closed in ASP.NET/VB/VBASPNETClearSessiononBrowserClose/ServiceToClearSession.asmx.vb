Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class ServiceToClearSession
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Sub RecordCloseTime()
        HttpContext.Current.Session.Clear()

        'Logging the data t the XML File
        Dim dataSource As New SessionInfoDataSource()
        Dim newSessionInfo As New SessionInfo()
        newSessionInfo.SessionValue = "Current Session value is " + Session("SessionCreatedTime")
        newSessionInfo.BrowserClosedTime = System.DateTime.Now.ToString()
        dataSource.InsertSessionInfo(newSessionInfo)

        dataSource.Save()
    End Sub


End Class