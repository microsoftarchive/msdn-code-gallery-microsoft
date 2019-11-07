Imports System
Imports Windows.UI.Notifications
Imports Windows.Devices.Printers.Extensions
Imports Windows.Data.Xml.Dom

''' <summary>
''' This class is the entry point for background notification tasks, as declared in the manifest
''' The Run method is called when an event is fired.  We then create and show a toast.  Clicking this
''' toast will take the user to the App
''' </summary>
Public NotInheritable Class PrintBackgroundTask
    Implements Windows.ApplicationModel.Background.IBackgroundTask

    '
    ' Save the printer name and asyncUI xml
    '
    Private Const keyPrinterName As String = "BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39"
    Private Const keyAsyncUIXML As String = "55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685"
    Private settings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

    Public Sub Run(ByVal taskInstance As Windows.ApplicationModel.Background.IBackgroundTaskInstance) Implements Windows.ApplicationModel.Background.IBackgroundTask.Run
        Dim details As PrintNotificationEventDetails = CType(taskInstance.TriggerDetails, PrintNotificationEventDetails)
        settings.Values(keyPrinterName) = details.PrinterName
        settings.Values(keyAsyncUIXML) = details.EventData

        ' With the print notification event details we can choose to show a toast, update the tile, or update the badge.
        ' It is not recommended to always show a toast, especially for non-actionable events, as it may become annoying for most users.
        ' User may even just turn off all toasts from this app, which is not a desired outcome.
        ' For events that does not require user's immediate attention, it is recommended to update the tile/badge and not show a toast.
        ShowToast(details.PrinterName, details.EventData)
        UpdateTile(details.PrinterName, details.EventData)
        UpdateBadge()
    End Sub

    Private Sub UpdateTile(ByVal printerName As String, ByVal bidiMessage As String)
        Dim tileUpdater As TileUpdater = TileUpdateManager.CreateTileUpdaterForApplication()
        tileUpdater.Clear()

        Dim tileXml As XmlDocument = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text09)
        Dim tileTextAttributes As XmlNodeList = tileXml.GetElementsByTagName("text")
        tileTextAttributes(0).InnerText = printerName
        tileTextAttributes(1).InnerText = bidiMessage

        Dim tileNotification As New TileNotification(tileXml)
        tileNotification.Tag = "tag01"
        tileUpdater.Update(tileNotification)
    End Sub

    Private Sub UpdateBadge()
        Dim badgeXml As XmlDocument = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph)
        Dim badgeElement As XmlElement = CType(badgeXml.SelectSingleNode("/badge"), XmlElement)
        badgeElement.SetAttribute("value", "error")

        Dim badgeNotification = New BadgeNotification(badgeXml)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification)
    End Sub

    Private Sub ShowToast(ByVal title As String, ByVal body As String)
        '
        ' Get Toast template
        '
        Dim toastXml As XmlDocument = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)

        '
        ' Pass to app as eventArgs.detail.arguments
        '
        CType(toastXml.SelectSingleNode("/toast"), XmlElement).SetAttribute("launch", title)

        '
        ' The ToastText02 template has 2 text nodes (a header and a body)
        ' Assign title to the first one, and body to the second one
        '
        Dim textList As XmlNodeList = toastXml.GetElementsByTagName("text")
        textList(0).AppendChild(toastXml.CreateTextNode(title))
        textList(1).AppendChild(toastXml.CreateTextNode(body))

        '
        ' Show the Toast
        '
        Dim toast As New ToastNotification(toastXml)
        ToastNotificationManager.CreateToastNotifier().Show(toast)
    End Sub
End Class
