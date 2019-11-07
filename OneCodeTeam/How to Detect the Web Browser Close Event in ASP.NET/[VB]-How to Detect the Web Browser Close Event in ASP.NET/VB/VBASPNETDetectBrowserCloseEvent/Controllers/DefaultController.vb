Imports System.Web.Mvc
Imports Newtonsoft.Json

Namespace Controllers
    Public Class DefaultController
        Inherits Controller
        ' GET: Default 
        Public Function Index() As ActionResult
            Return View()
        End Function

        ' GET: Default/GetRefreshTime
        Public Function GetRefreshTime(clientId As String) As String
            Dim dataSource As New ClientInfoDataSource()
            Dim clientInfo = dataSource.GetClientInfoByClientId(clientId)
            If clientInfo IsNot Nothing Then
                clientInfo.RefreshTime = DateTime.Now
                dataSource.UpdateClientInfo(clientInfo)
                dataSource.Save()
                Return JsonConvert.SerializeObject(clientInfo)
            Else
                Dim newClientInfo As New ClientInfo() With { _
                     .ClientID = clientId, _
                     .ActiveTime = DateTime.Now, _
                     .RefreshTime = DateTime.Now _
                }
                dataSource.InsertClientInfo(newClientInfo)
                dataSource.Save()
                Return JsonConvert.SerializeObject(newClientInfo)
            End If
        End Function

        ' Head: Default/RecordActiveTime
        <HttpHead> _
        Public Sub RecordActiveTime(clientId As String)
            Dim dataSource As New ClientInfoDataSource()
            Dim clientInfo = dataSource.GetClientInfoByClientId(clientId)
            If clientInfo IsNot Nothing Then
                clientInfo.ActiveTime = DateTime.Now
                dataSource.UpdateClientInfo(clientInfo)
            Else
                Dim newClientInfo As New ClientInfo() With { _
                     .ClientID = clientId, _
                     .ActiveTime = DateTime.Now, _
                     .RefreshTime = DateTime.Now _
                }

                dataSource.InsertClientInfo(newClientInfo)
            End If
            dataSource.Save()
        End Sub

        ' Head: Default/RecordCloseTime
        <HttpHead> _
        Public Sub RecordCloseTime(clientId As String)
            Dim dataSource As New ClientInfoDataSource()
            Dim clientInfo = dataSource.GetClientInfoByClientId(clientId)
            If clientInfo IsNot Nothing Then
                clientInfo.RefreshTime = DateTime.Now
                dataSource.UpdateClientInfo(clientInfo)
            End If
            dataSource.Save()
        End Sub
    End Class
End Namespace