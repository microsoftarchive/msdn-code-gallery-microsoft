Public Class SilverlightPluginGenerator

    ''' <summary>Specifies the initial width of the Silverlight plug-in area in the HTML page. Can be as a pixel value or as a percentage (a value that ends with the % character specifies a percentage value). For example, "400" specifies 400 pixels, and "50%" specifies 50% (half) of the available width of the browser content area.</summary>
    Private privateWidth As Unit
    Public Property Width() As Unit
        Get
            Return privateWidth
        End Get
        Set(ByVal value As Unit)
            privateWidth = value
        End Set
    End Property
    ''' <summary>Specifies the initial height of the Silverlight plug-in area in the HTML page. Can be set either as a pixel value or a percentage (a value that ends with the % character specifies a percentage value). For example, "300" specifies 300 pixels, and "50%" specifies 50% (half) of the available height of the browser content area. </summary>
    Private privateHeight As Unit
    Public Property Height() As Unit
        Get
            Return privateHeight
        End Get
        Set(ByVal value As Unit)
            privateHeight = value
        End Set
    End Property
    ''' <summary>Gets or sets the Uniform Resource Identifier (URI) of the XAP package</summary>
    Private privateSource As Uri
    Public Property Source() As Uri
        Get
            Return privateSource
        End Get
        Set(ByVal value As Uri)
            privateSource = value
        End Set
    End Property
    ''' <summary>Specifies the ID of the object element</summary>
    Private privateID As String
    Public Property ID() As String
        Get
            Return privateID
        End Get
        Set(ByVal value As String)
            privateID = value
        End Set
    End Property
    ''' <summary>Gets or sets a value that indicates whether the hosted content in the Silverlight plug-in can use the HtmlPage.PopupWindow method to display a new browser window.</summary>
    Private privateAllowHtmlPopupWindow? As Boolean
    Public Property AllowHtmlPopupWindow() As Boolean?
        Get
            Return privateAllowHtmlPopupWindow
        End Get
        Set(ByVal value? As Boolean)
            privateAllowHtmlPopupWindow = value
        End Set
    End Property
    ''' <summary>Sets a value that indicates whether a Silverlight plug-in version earlier than the specified minRuntimeVersion will attempt to update automatically.</summary>
    Private privateAutoUpgrade? As Boolean
    Public Property AutoUpgrade() As Boolean?
        Get
            Return privateAutoUpgrade
        End Get
        Set(ByVal value? As Boolean)
            privateAutoUpgrade = value
        End Set
    End Property
    ''' <summary>Gets or sets a value that indicates whether the hosted content in the Silverlight plug-in and in the associated run-time code has access to the browser Document Object Model (DOM).</summary>
    Private privateEnablehtmlaccess? As Boolean
    Public Property Enablehtmlaccess() As Boolean?
        Get
            Return privateEnablehtmlaccess
        End Get
        Set(ByVal value? As Boolean)
            privateEnablehtmlaccess = value
        End Set
    End Property
    ''' <summary>Gets or sets a value that indicates whether the hosted content in the Silverlight plug-in can use a HyperlinkButton to navigate to external URIs.</summary>
    Private privateEnableNavigation? As Boolean
    Public Property EnableNavigation() As Boolean?
        Get
            Return privateEnableNavigation
        End Get
        Set(ByVal value? As Boolean)
            privateEnableNavigation = value
        End Set
    End Property

    ''' <summary>Gets or sets the background color of the rectangular region that displays XAML content.</summary>
    Private privateBackGround As Drawing.Color
    Public Property BackGround() As Drawing.Color
        Get
            Return privateBackGround
        End Get
        Set(ByVal value As Drawing.Color)
            privateBackGround = value
        End Set
    End Property

    ''' <summary>Specifies the name of the handler to call when the Silverlight plug-in generates a XAML parse error or run-time error at the native-code level.</summary>
    Private privateOnError As String
    Public Property OnError() As String
        Get
            Return privateOnError
        End Get
        Set(ByVal value As String)
            privateOnError = value
        End Set
    End Property
    ''' <summary>Specifies the handler for a FullScreenChanged event that occurs whenever the FullScreen property of the Silverlight plug-in changes.</summary>
    Private privateOnFullScreenChanged As String
    Public Property OnFullScreenChanged() As String
        Get
            Return privateOnFullScreenChanged
        End Get
        Set(ByVal value As String)
            privateOnFullScreenChanged = value
        End Set
    End Property
    ''' <summary>Establishes the handler for a Loaded event that occurs when the Silverlight plug-in has finished loading into the browser DOM.</summary>
    Private privateOnLoad As String
    Public Property OnLoad() As String
        Get
            Return privateOnLoad
        End Get
        Set(ByVal value As String)
            privateOnLoad = value
        End Set
    End Property
    ''' <summary>Specifies a handler for the Resized event that occurs when the Silverlight plug-in's object tag is resized and the  ActualHeight or the ActualWidth of the Silverlight plug-in change.</summary>
    Private privateOnResize As String
    Public Property OnResize() As String
        Get
            Return privateOnResize
        End Get
        Set(ByVal value As String)
            privateOnResize = value
        End Set
    End Property
    ''' <summary>Gets or sets the name of the event handler that is called when the source download has finished.</summary>
    Private privateOnSourceDownloadComplete As String
    Public Property OnSourceDownloadComplete() As String
        Get
            Return privateOnSourceDownloadComplete
        End Get
        Set(ByVal value As String)
            privateOnSourceDownloadComplete = value
        End Set
    End Property
    ''' <summary>Gets or sets the name of the event handler that is called when the source download progress changes.</summary>
    Private privateOnSourceDownloadProgressChanged As String
    Public Property OnSourceDownloadProgressChanged() As String
        Get
            Return privateOnSourceDownloadProgressChanged
        End Get
        Set(ByVal value As String)
            privateOnSourceDownloadProgressChanged = value
        End Set
    End Property

    ''' <summary>Gets or sets user-defined initialization parameters.</summary>
    Public InitParams As New List(Of InitParam)()

    ''' <summary>Gets or sets the Silverlight version of the control.  Sets the minimum version and install links</summary>
    Private privateVersion As SilverlightVersion
    Public Property Version() As SilverlightVersion
        Get
            Return privateVersion
        End Get
        Set(ByVal value As SilverlightVersion)
            privateVersion = value
        End Set
    End Property


    Public Sub New()
    End Sub

    Public Sub New(ByVal source As Uri)
        Me.Source = source
    End Sub

    Public Sub New(ByVal source As Uri, ByVal width As Unit, ByVal height As Unit)
        Me.Source = source
        Me.Width = width
        Me.Height = height
    End Sub

    Public Overrides Function ToString() As String
        Dim xDocument As New XDocument()
        Dim divXElement As New XElement("div")
        divXElement.Add(New XAttribute("id", "silverlightControlHost"))

        Dim objectXElement As New XElement("object")

        If Me.ID IsNot Nothing Then
            objectXElement.Add(New XAttribute("id", Me.ID))
        End If
        objectXElement.Add(New XAttribute("data", "data:application/x-silverlight-2,"))
        objectXElement.Add(New XAttribute("type", "application/x-silverlight-2"))

        ' Only add these if they are specified
        If Me.Width.IsEmpty = False Then
            objectXElement.Add(New XAttribute("width", Me.Width.Value.ToString() + Me.GetUnitString(Me.Width.Type)))
        End If
        If Me.Height.IsEmpty = False Then
            objectXElement.Add(New XAttribute("height", Me.Height.Value.ToString() + Me.GetUnitString(Me.Height.Type)))
        End If


        ' Conditionally add params to the source
        objectXElement.Add(Me.CreateParameter("source", Me.Source.ToString()))

        If Me.AllowHtmlPopupWindow IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("allowHtmlPopupWindow", Me.AllowHtmlPopupWindow.ToString()))
        End If
        If Me.AutoUpgrade IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("autoUpgrade", Me.AutoUpgrade.ToString()))
        End If
        If Me.Enablehtmlaccess IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("enablehtmlaccess", Me.Enablehtmlaccess.ToString()))
        End If
        If Me.EnableNavigation IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("enableNavigation", Me.EnableNavigation.ToString()))
        End If
        If Me.Version <> SilverlightVersion.Unknown Then
            objectXElement.Add(Me.CreateParameter("minRuntimeVersion", SilverlightVersionHelper.GetMinimumVersion(Me.Version)))
        End If
        If Me.OnError IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("onError", Me.OnError))
        End If
        If Me.OnFullScreenChanged IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("onFullScreenChanged", Me.OnFullScreenChanged))
        End If
        If Me.OnLoad IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("onLoad", Me.OnLoad))
        End If
        If Me.OnResize IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("onResize", Me.OnResize))
        End If
        If Me.OnSourceDownloadComplete IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("onSourceDownloadComplete", Me.OnSourceDownloadComplete))
        End If
        If Me.OnSourceDownloadProgressChanged IsNot Nothing Then
            objectXElement.Add(Me.CreateParameter("onSourceDownloadProgressChanged", Me.OnSourceDownloadProgressChanged))
        End If
        If Me.BackGround.IsEmpty = False Then
            objectXElement.Add(Me.CreateParameter("background", Me.BackGround.Name.ToLower()))
        End If

        ' Add the initparams
        If Me.InitParams IsNot Nothing AndAlso Me.InitParams.Count > 0 Then
            Dim stringBuilder As New System.Text.StringBuilder()
            For Each initParam As InitParam In Me.InitParams
                If stringBuilder.Length > 0 Then
                    stringBuilder.Append(",")
                End If
                stringBuilder.Append(initParam.Key & "=" & initParam.Value)
            Next initParam
            objectXElement.Add(Me.CreateParameter("initParams", stringBuilder.ToString()))
        End If


        ' Create the no Silverlight installed link
        Dim installLinkXElemenet As New XElement("a")
        installLinkXElemenet.Add(New XAttribute("href", SilverlightVersionHelper.GetUpgradeUrl(Me.Version)))
        installLinkXElemenet.Add(New XAttribute("style", "text-decoration:none"))

        Dim silverlightImageXElement As New XElement("img")
        silverlightImageXElement.Add(New XAttribute("src", "http://go.microsoft.com/fwlink/?LinkId=108181"))
        silverlightImageXElement.Add(New XAttribute("alt", "Get Microsoft Silverlight"))
        silverlightImageXElement.Add(New XAttribute("style", "border-style:none"))

        installLinkXElemenet.Add(silverlightImageXElement)
        objectXElement.Add(installLinkXElemenet)


        ' This iframe is for Safari, it prevents Safari from caching the page
        Dim iFrameXElement As New XElement("iframe", " ") ' Adding a space for the content forces the iframe to not self close. If it self closes, the ribbon breaks.
        iFrameXElement.Add(New XAttribute("id", "_sl_historyFrame"))
        iFrameXElement.Add(New XAttribute("style", "visibility:hidden;height:0px;width:0px;border:0px"))

        ' Add the object tag and iFrame to the div
        divXElement.Add(objectXElement)
        divXElement.Add(iFrameXElement)

        ' Set the div on the root node
        xDocument.Add(divXElement)

        Return xDocument.ToString()
    End Function

    Private Function CreateParameter(ByVal name As String, ByVal value As String) As XElement
        Dim paramXElement As New XElement("param")
        paramXElement.Add(New XAttribute("name", name))
        paramXElement.Add(New XAttribute("value", value))
        Return paramXElement
    End Function

    ''' <summary>Returns the HTML version of the unit</summary>
    ''' <param name="unitType">Unit type to retrieve the HTML string for</param>
    ''' <returns></returns>
    Private Function GetUnitString(ByVal unitType As UnitType) As String
        Dim ret As String = ""
        Select Case unitType
            Case UnitType.Pixel
                ret = "px"
            Case UnitType.Em
                ret = "em"
            Case UnitType.Ex
                ret = "ex"
            Case UnitType.Inch
                ret = "in"
            Case UnitType.Cm
                ret = "cm"
            Case UnitType.Mm
                ret = "mm"
            Case UnitType.Point
                ret = "pt"
            Case UnitType.Pica
                ret = "pc"
            Case Else
                Throw New NotImplementedException("UnitType " & unitType.ToString() & " is not supported")
        End Select

        Return ret
    End Function
End Class

Public NotInheritable Class SilverlightVersionHelper
    ''' <summary>Returns the minimum version for the specified version of Silverlight</summary>
    ''' <param name="version">Silveright version</param>
    Private Sub New()
    End Sub
    Public Shared Function GetMinimumVersion(ByVal version As SilverlightVersion) As String
            Dim minimumVersion As String = ""
            Select Case version
                Case SilverlightVersion.v2
                    minimumVersion = "2.0.31005"
                Case SilverlightVersion.v3
                    minimumVersion = "3.0.40624"
                Case Else
                    minimumVersion = "3.0.40624"
            End Select
            Return minimumVersion
    End Function

    ''' <summary>Returns the upgrade URL for the specified version of Silverlight</summary>
    ''' <param name="version">Silveright version</param>
    Public Shared Function GetUpgradeUrl(ByVal version As SilverlightVersion) As String
            Dim upgradeUrl As String = ""
            Select Case version
                Case SilverlightVersion.v2
                    upgradeUrl = "http://go.microsoft.com/fwlink/?LinkID=149156&v=2.0.31005.0"
                Case SilverlightVersion.v3
                    upgradeUrl = "http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40624.0"
                Case Else
                    upgradeUrl = "http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40624.0"
            End Select
            Return upgradeUrl
    End Function
End Class

Public Enum SilverlightVersion
    Unknown
    v2
    v3
End Enum

Public Class InitParam
    Private privateKey As String
    Public Property Key() As String
        Get
            Return privateKey
        End Get
        Set(ByVal value As String)
            privateKey = value
        End Set
    End Property
    Private privateValue As String
    Public Property Value() As String
        Get
            Return privateValue
        End Get
        Set(ByVal value As String)
            privateValue = value
        End Set
    End Property
    Public Sub New(ByVal key As String, ByVal value As String)
        Me.Key = key
        Me.Value = value
    End Sub
End Class
