#include "pch.h"
#include "PrintBackgroundTask.h"

using namespace BackgroundTask;
using namespace Platform;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;

PrintBackgroundTask::PrintBackgroundTask(void)
{
    _keyPrinterName = ref new Platform::String(L"BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39");
    _keyAsyncUIXML = ref new Platform::String(L"55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685");
}

PrintBackgroundTask::~PrintBackgroundTask(void)
{
}

void PrintBackgroundTask::Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance)
{
    // Retrieve the printer name and event data
    Windows::Devices::Printers::Extensions::PrintNotificationEventDetails^ details = (Windows::Devices::Printers::Extensions::PrintNotificationEventDetails^)(taskInstance->TriggerDetails);
    Windows::Storage::ApplicationData^ applicationData = Windows::Storage::ApplicationData::Current;
    Windows::Foundation::Collections::IPropertySet^ propertySet = applicationData->LocalSettings->Values;
    propertySet->Remove(_keyPrinterName);
    propertySet->Remove(_keyAsyncUIXML);
    propertySet->Insert(_keyPrinterName, details->PrinterName);
    propertySet->Insert(_keyAsyncUIXML, details->EventData);

    // With the print notification event details we can choose to show a toast, update the tile, or update the badge.
    // It is not recommended to always show a toast, especially for non-actionable events, as it may become annoying for most users.
    // User may even just turn off all toasts from this app, which is not a desired outcome.
    // For events that does not require user's immediate attention, it is recommended to update the tile/badge and not show a toast.
    ShowToast(details->PrinterName, details->EventData);
    UpdateTile(details->PrinterName, details->EventData);
    UpdateBadge();
}

void PrintBackgroundTask::ShowToast(Platform::String^ title, Platform::String^ body)
{
    XmlDocument^ toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastText02);

    //
    // The ToastText02 template has 2 text nodes (a header and a body)
    // Assign title to the first one, and body to the second one
    //
    XmlNodeList^ textList = toastXml->GetElementsByTagName(L"text");
    textList->Item(0)->AppendChild(toastXml->CreateTextNode(title));
    textList->Item(1)->AppendChild(toastXml->CreateTextNode(body));

    //
    // Set launch flag so the app can tell if it is being launched by the toast or from the tile
    //
    IXmlNode^ node = toastXml->SelectSingleNode(L"/toast");
    XmlElement^ element = (XmlElement^) node;
    element->SetAttribute(L"launch", title);

    //
    // Show the Toast
    //
    ToastNotification^ toast = ref new ToastNotification(toastXml);
    ToastNotificationManager::CreateToastNotifier()->Show(toast);
}

void PrintBackgroundTask::UpdateTile(Platform::String^ printerName, Platform::String^ bidiMessage)
{
    TileUpdater^ tileUpdater = TileUpdateManager::CreateTileUpdaterForApplication();
    if (nullptr != tileUpdater)
    {
        tileUpdater->Clear();

        XmlDocument^ tileXml = TileUpdateManager::GetTemplateContent(TileTemplateType::TileWideText09);
        if (nullptr != tileXml)
        {
            // Fill in the template
            XmlNodeList^ tileTextAttributes = tileXml->GetElementsByTagName("text");
            tileTextAttributes->GetAt(0)->InnerText = printerName;
            tileTextAttributes->GetAt(1)->InnerText = bidiMessage;

            // Create a tile update
            TileNotification^ tileNotification = ref new TileNotification(tileXml);
            if (nullptr != tileNotification)
            {
                tileNotification->Tag = "tag01";
                TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileNotification);
            }
        }
    }
}

void PrintBackgroundTask::UpdateBadge()
{
    // Get the template
    XmlDocument^ badgeXml = BadgeUpdateManager::GetTemplateContent(BadgeTemplateType::BadgeGlyph);
    if (nullptr != badgeXml)
    {
        XmlElement^ badgeElement = safe_cast<XmlElement^>(badgeXml->SelectSingleNode("/badge"));
        if (nullptr != badgeElement)
        {
            // Fill in the template
            badgeElement->SetAttribute("value", "error");
            BadgeNotification^ badgeNotification = ref new BadgeNotification(badgeXml);
            if (nullptr != badgeNotification)
            {
                // Create a badge update
                BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badgeNotification);
            }
        }
    }
}
