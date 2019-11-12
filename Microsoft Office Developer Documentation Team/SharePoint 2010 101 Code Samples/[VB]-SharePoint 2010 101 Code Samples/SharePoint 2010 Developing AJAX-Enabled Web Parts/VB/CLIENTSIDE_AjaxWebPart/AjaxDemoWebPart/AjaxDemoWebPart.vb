Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

''' <summary>
''' This web part just displays the time. However it does so within an AJAX update
''' panel so the entire page is not reloaded whenever the button is clicked. It
''' also demonstrates how to use an UpdateProgress control to feedback to the user
''' and enables the user to set the progress image and text when they configure the
''' web part.
''' </summary>
''' <remarks>
''' For this to work, there must be a <asp:ScriptManager> control on the page. In 
''' SharePoint 2010, this is including in all master pages. Unless you are using
''' a custom master page that doesn't include a script manager, you can use AJAX
''' controls in Web Parts without creating it. 
''' </remarks>
<ToolboxItemAttribute(false)> _
Public Class AjaxDemoWebPart
    Inherits WebPart

    'Internal properties
    Private strImagePath As String
    Private strDisplayText As String

    'External properties
    'This property enables the user to set the progress image path.
    <DefaultValue("_layouts/images/progress.gif")> _
    <WebBrowsable(True)> _
    <Category("ProgressTemplate")> _
    <Personalizable(PersonalizationScope.Shared)> _
    Public Property ImagePath As String
        Get
            Return strImagePath
        End Get
        Set(ByVal value As String)
            strImagePath = value
        End Set
    End Property


    'This property enables the user to set the progress feedback text.
    <DefaultValue("Checking...")> _
    <WebBrowsable(True)> _
    <Category("ProgressTemplate")> _
    <Personalizable(PersonalizationScope.Shared)> _
    Public Property DisplayText As String
        Get
            Return strDisplayText
        End Get
        Set(ByVal value As String)
            strDisplayText = value
        End Set
    End Property

    'Controls
    Dim mainUpdatePanel As UpdatePanel
    Dim progressControl As UpdateProgress
    Dim checkTimeButton As Button
    Dim timeDisplayLabel As Label

    Protected Overrides Sub CreateChildControls()

        'Create the update panel
        mainUpdatePanel = New UpdatePanel()
        mainUpdatePanel.ID = "updateAjaxDemoWebPart"
        'Use conditional mode so that only controls within this panel cause an update
        mainUpdatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional

        'Create the update progress control
        progressControl = New UpdateProgress()
        progressControl.AssociatedUpdatePanelID = "updateAjaxDemoWebPart"
        'The class used for the progrss template is defined below in this code file
        progressControl.ProgressTemplate = New ProgressTemplate(ImagePath, DisplayText)

        'Create the Check Time button
        checkTimeButton = New Button()
        checkTimeButton.ID = "checkTimeButton"
        checkTimeButton.Text = "Check Time"
        AddHandler checkTimeButton.Click, AddressOf checkTimeButton_Click

        'Create the label that displays the time
        timeDisplayLabel = New Label()
        timeDisplayLabel.ID = "timeDisplayLabel"
        timeDisplayLabel.Text = String.Format("The time is: {0}", DateTime.Now.ToLongTimeString())

        'Add the button and label to the Update Panel
        mainUpdatePanel.ContentTemplateContainer.Controls.Add(timeDisplayLabel)
        mainUpdatePanel.ContentTemplateContainer.Controls.Add(checkTimeButton)

        'Add the Update Panel and the progress control to the Web Part controls
        Me.Controls.Add(mainUpdatePanel)
        Me.Controls.Add(progressControl)

    End Sub

    Private Sub checkTimeButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        'This calls a server-side method, but because the button is in 
        'an update panel, only the update panel reloads.
        Me.timeDisplayLabel.Text = String.Format("The time is: {0}", DateTime.Now.ToLongTimeString())
    End Sub

End Class

'This template defines the contents of the Update Progress control
Public Class ProgressTemplate
    Implements ITemplate

    Public ImagePath As String
    Public DisplayText As String

    Public Sub New(ByVal imagePathArg As String, ByVal displayTextArg As String)
        ImagePath = imagePathArg
        DisplayText = displayTextArg
    End Sub

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn

        Dim img As Image = New Image()
        img.ImageUrl = SPContext.Current.Site.Url + "/" + ImagePath

        Dim displayTextLabel As Label = New Label()
        displayTextLabel.Text = DisplayText

        container.Controls.Add(img)
        container.Controls.Add(displayTextLabel)

    End Sub
End Class