#include "pch.h"
#include "NotificationData.h"

using namespace ScheduledNotificationsSampleCPP;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::ViewManagement;

NotificationData::NotificationData(void)
{
    _itemType = "";
    _itemId = "";
    _dueTime = "";
    _inputString = "";
    _isTile = false;
}

String^ NotificationData::ItemType::get()
{
    return _itemType;
}

void NotificationData::ItemType::set(String^ value)
{
    if (_itemType != value) _itemType = value;
}

String^ NotificationData::ItemId::get()
{
    return _itemId;
}

void NotificationData::ItemId::set(String^ value)
{
    if (_itemId != value) _itemId = value;
}

String^ NotificationData::DueTime::get()
{
    return _dueTime;
}

void NotificationData::DueTime::set(String^ value)
{
    if (_dueTime != value) _dueTime = value;
}

String^ NotificationData::InputString::get()
{
    return _inputString;
}

void NotificationData::InputString::set(String^ value)
{
    if (_inputString != value) _inputString = value;
}

bool NotificationData::get_IsTile()
{
    return _isTile;
}

void NotificationData::set_IsTile(bool isTile)
{
	if (_isTile != isTile)
	{
		_isTile = isTile;
	}
}

void NotificationData::OnPropertyChanged(String^ propertyName)
{
    PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
}

NotificationDataSource::NotificationDataSource()
{
    _items = ref new Vector<Object^>();
    _formatter = Windows::Globalization::DateTimeFormatting::DateTimeFormatter::LongTime;
}

void NotificationDataSource::LoadData()
{
    auto scheduledToasts = ToastNotificationManager::CreateToastNotifier()->GetScheduledToastNotifications();
    auto scheduledTiles = TileUpdateManager::CreateTileUpdaterForApplication()->GetScheduledTileNotifications();

    int toastLength = scheduledToasts->Size;
    int tileLength = scheduledTiles->Size;

    for (int i = 0; i < toastLength; i++)
    {
        auto toast = scheduledToasts->GetAt(i);

        AddItem(
            "Toast", 
            toast->Id, 
            _formatter->Format(toast->DeliveryTime), 
            toast->Content->GetElementsByTagName("text")->Item(0)->InnerText,
            false
        );
    }

    for (int i = 0; i < tileLength; i++)
    {
        auto tile = scheduledTiles->GetAt(i);
        AddItem(
            "Tile",
            tile->Id,
            _formatter->Format(tile->DeliveryTime),
            tile->Content->GetElementsByTagName("text")->Item(0)->InnerText,
            true
        );
    }
}

void NotificationDataSource::Clear()
{
    _items->Clear();
}

void NotificationDataSource::Refresh()
{
    Clear();
    LoadData();
}

void NotificationDataSource::AddItem(String^ itemType, String^ itemId, String^ dueTime, String^ inputString, bool isTile)
{
    auto newData = ref new NotificationData();

    newData->ItemType = itemType;
    newData->ItemId = itemId;
    newData->DueTime = dueTime;
    newData->InputString = inputString;
    newData->set_IsTile(isTile);

    _items->Append(newData);
}

IObservableVector<Object^>^ NotificationDataSource::Items::get()
{
    return _items;
}
