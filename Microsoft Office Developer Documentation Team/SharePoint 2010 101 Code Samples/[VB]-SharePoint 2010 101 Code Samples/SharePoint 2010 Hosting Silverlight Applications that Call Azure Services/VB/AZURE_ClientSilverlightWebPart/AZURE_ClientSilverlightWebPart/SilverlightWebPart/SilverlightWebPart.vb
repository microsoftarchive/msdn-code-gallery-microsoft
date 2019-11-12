'This example uses the Silverlight Web Part project template
'that you can get from http://archive.msdn.microsoft.com/vsixforsp
'This project is a Web Part that hosts the silverlight application

<ToolboxItemAttribute(False)> _
Public Class SilverlightWebPart
    Inherits WebPart
    Private _silverlightPluginGenerator As SilverlightPluginGenerator = Nothing


    Public Sub New()

        Me._silverlightPluginGenerator = New SilverlightPluginGenerator
        With Me._silverlightPluginGenerator
            .Source = New Uri("/SiteAssets/Silverlight/AZURE_ClientSilverlightApp/AZURE_ClientSilverlightApp.xap", UriKind.Relative)
            .Width = New Unit(400, UnitType.Pixel)
            .Height = New Unit(150, UnitType.Pixel)
            .BackGround = Drawing.Color.White
            .Version = SilverlightVersion.v3
            .AutoUpgrade = True
        End With

    End Sub

    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        ' Set the SiteUrl here.  Can't do this earlier since SPContext may be null during instantiation
        Me._silverlightPluginGenerator.InitParams.Add(New InitParam("SiteUrl", SPContext.Current.Site.Url))
        Me.Controls.Add(New LiteralControl(Me._silverlightPluginGenerator.ToString()))
    End Sub

    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        MyBase.RenderContents(writer)
    End Sub
End Class
