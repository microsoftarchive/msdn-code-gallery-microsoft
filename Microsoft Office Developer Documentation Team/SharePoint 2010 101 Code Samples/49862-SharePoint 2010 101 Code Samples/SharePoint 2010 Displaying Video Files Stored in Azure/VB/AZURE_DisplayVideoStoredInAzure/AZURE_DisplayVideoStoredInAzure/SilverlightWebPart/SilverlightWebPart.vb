<ToolboxItemAttribute(False)> _
Public Class SilverlightWebPart
    Inherits WebPart
    Private _silverlightPluginGenerator As SilverlightPluginGenerator = Nothing

    Public Sub New()

        Me._silverlightPluginGenerator = New SilverlightPluginGenerator
        With Me._silverlightPluginGenerator
            .Source = New Uri("/SiteAssets/Silverlight/AZURE_SilverlightVideoApp/AZURE_SilverlightVideoApp.xap", UriKind.Relative)
            .Width = New Unit(400, UnitType.Pixel)
            .Height = New Unit(300, UnitType.Pixel)
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
