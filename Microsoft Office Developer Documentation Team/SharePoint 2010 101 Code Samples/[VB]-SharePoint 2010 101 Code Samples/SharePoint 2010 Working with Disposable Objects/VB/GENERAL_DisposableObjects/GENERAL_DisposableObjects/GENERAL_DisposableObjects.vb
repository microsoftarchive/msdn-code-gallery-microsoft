Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

'This Web Part illustrates how to correctly dispose of objects that implement the
'IDisposable interface. This is vital in SharePoint development because, if you do 
'not correctly dispose of SPWeb, SPSite, and certain other objects, memory usage
'will rise unnecessarily.
<ToolboxItemAttribute(false)> _
Public Class GENERAL_DisposableObjects
    Inherits WebPart

    'Controls
    Private getSiteButton As Button
    Private currentSiteInfo As Label
    Private getWebButton As Button
    Private currentWebInfo As Label

    Protected Overrides Sub CreateChildControls()
        'Set up the Get Site button
        getSiteButton = New Button()
        getSiteButton.Text = "Get Site Info"
        AddHandler getSiteButton.Click, AddressOf getSiteButton_Click
        Me.Controls.Add(getSiteButton)
        'Set up the Current Site Info Label
        currentSiteInfo = New Label()
        Me.Controls.Add(currentSiteInfo)
        'Set up the Get Web button
        getWebButton = New Button()
        getWebButton.Text = "Get Web Info"
        AddHandler getWebButton.Click, AddressOf getWebButton_Click
        Me.Controls.Add(getWebButton)
        'Set up the Current Web Info Label
        currentWebInfo = New Label()
        Me.Controls.Add(currentWebInfo)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'Render the controls
        getSiteButton.RenderControl(writer)
        currentSiteInfo.RenderControl(writer)
        writer.WriteBreak()
        getWebButton.RenderControl(writer)
        currentWebInfo.RenderControl(writer)
    End Sub

    Private Sub getSiteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        'This code is unlikely to fail, but it illustrates the importance of a finally
        'block, to ensure that your dispose of objects. Without the finally block, an
        'exception would prevent the correct object disposal.
        Dim currentSite As SPSite = Nothing
        Try
            'Get the current site
            currentSite = SPContext.Current.Site
            'Display the site ID, just as an example
            currentSiteInfo.Text = "Current Site ID: " + currentSite.ID.ToString()
        Catch ex As Exception
            currentSiteInfo.Text = "There was an error getting the site info: " + ex.Message
        Finally
            'The currentSite object is always disposed
            currentSite.Dispose()
        End Try
    End Sub

    Private Sub getWebButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Using currentWeb As SPWeb = SPContext.Current.Web
            currentWebInfo.Text = "Current Web Title: " + currentWeb.Title
        End Using
        'At the end of the using block, currentWeb is disposed of implicitly
    End Sub

End Class
