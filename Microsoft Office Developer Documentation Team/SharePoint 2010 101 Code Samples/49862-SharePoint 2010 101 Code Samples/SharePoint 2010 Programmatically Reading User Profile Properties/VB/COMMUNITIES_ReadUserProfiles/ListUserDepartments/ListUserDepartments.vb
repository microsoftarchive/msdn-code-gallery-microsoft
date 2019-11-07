Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
'The following two namespaces are required to work with user profiles
Imports Microsoft.Office.Server
Imports Microsoft.Office.Server.UserProfiles
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

''' <summary>
''' This web part gets all the user profiles and displays the name of each
''' user and the department they are in.
''' </summary>
''' <remarks>
''' You must add references to the Microsoft.Office.Server and
''' Microsoft.Office.Server.UserProfiles dlls in the 14 hive ISAPI directory. 
''' You must have a User Profile service application in place.
''' Also the code below only works in the context of an account that has the
''' "manage user profiles" right. If you don't have that right you could get
''' similar results by using the Search API and the People search scope.
''' </remarks>
<ToolboxItemAttribute(false)> _
Public Class ListUserDepartments
    Inherits WebPart

    'Controls
    Dim runQueryButton As Button
    Dim resultsLabel As Label

    Protected Overrides Sub CreateChildControls()
        'Create the Run Query button
        runQueryButton = New Button()
        runQueryButton.Text = "Run Query"
        AddHandler runQueryButton.Click, AddressOf runQueryButton_Click
        Me.Controls.Add(runQueryButton)
        'Create the results display label
        resultsLabel = New Label()
        resultsLabel.Text = String.Empty
        Me.Controls.Add(resultsLabel)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'Render all controls
        runQueryButton.RenderControl(writer)
        writer.Write("<br />")
        resultsLabel.RenderControl(writer)
    End Sub

    Private Sub runQueryButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        'Get the My Sites site collection, ensuring proper disposal
        Using mySitesCollection As SPSite = New SPSite("http://intranet.contoso.com/my")
            'Get the user profile manager
            Dim context As SPServiceContext = SPServiceContext.GetContext(mySitesCollection)
            Dim profileManager As UserProfileManager = New UserProfileManager(context)
            'How many user profiles are there?
            resultsLabel.Text = "There are " + profileManager.Count.ToString() + " user profiles.<br />"
            'Loop through all the user profiles
            For Each currentProfile As UserProfile In profileManager
                'here is always a display name
                resultsLabel.Text += "User: " + currentProfile.DisplayName + "<br />"
                'Be careful to avoid null errors
                If currentProfile("Department").Value IsNot Nothing Then
                    'There is a department listed so display it
                    resultsLabel.Text += "Department: " + currentProfile("Department").Value.ToString() + "<br /><br />"
                Else
                    resultsLabel.Text += "Department: None<br /><br />"
                End If
            Next
        End Using
    End Sub

End Class
