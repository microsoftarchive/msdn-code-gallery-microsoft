Public Class CheckifSessionisActive
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Write("Current Session value is " + Session("SessionCreatedTime"))
    End Sub

End Class