'#+++++++++++++++DISCLAIMER+++++++++++++++++++++++++++++
'#--------------------------------------------------------------------------------- 
'#The sample scripts are Not supported under any Microsoft standard support program Or service. The sample scripts are provided AS Is without warranty  
'#of any kind. Microsoft further disclaims all implied warranties including, without limitation, any implied warranties of merchantability Or of fitness for 
'#a particular purpose. The entire risk arising out of the use Or performance of the sample scripts And documentation remains with you. In no event shall 
'#Microsoft, its authors, Or anyone else involved in the creation, production, Or delivery of the scripts be liable for any damages whatsoever (including, 
'#without limitation, damages for loss of business profits, business interruption, loss of business information, Or other pecuniary loss) arising out of the use 
'#of Or inability to use the sample scripts Or documentation, even if Microsoft has been advised of the possibility of such damages 
'#---------------------------------------------------------------------------------

Imports System.Web
Public Class header
    Implements IHttpModule

    Public Sub New()
    End Sub

    ' In the Init function, register for HttpApplication 
    ' events by adding your handlers. 
    Public Sub Init(ByVal app As HttpApplication) Implements IHttpModule.Init
        AddHandler app.PreSendRequestHeaders, AddressOf Me.PreSendRequestHeaders
    End Sub

    Public Sub PreSendRequestHeaders(ByVal s As Object, ByVal e As EventArgs)
        Dim app As HttpApplication = CType(s, HttpApplication)
        app.Context.Response.Headers.Remove("Server")

    End Sub

    Public Sub Dispose() Implements IHttpModule.Dispose
    End Sub


End Class
