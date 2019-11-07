#pragma once

#include <collection.h>

namespace ScheduledNotificationsSampleCPP
{
	[Windows::UI::Xaml::Data::Bindable]
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class NotificationData sealed
    {
    public:
        NotificationData(void);

        property Platform::String^ ItemType { Platform::String^ get(); void set(Platform::String^ value); }
        property Platform::String^ ItemId { Platform::String^ get(); void set(Platform::String^ value); }
        property Platform::String^ DueTime { Platform::String^ get(); void set(Platform::String^ value); }
        property Platform::String^ InputString { Platform::String^ get(); void set(Platform::String^ value); }
		bool get_IsTile(void);
        void set_IsTile(bool isTile);

        event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

    protected:
        void OnPropertyChanged(Platform::String^ propertyName);

    private:
        Platform::String^ _itemType;
        Platform::String^ _itemId;
        Platform::String^ _dueTime;
        Platform::String^ _inputString;
        bool _isTile;
    };

    public ref class NotificationDataSource sealed
    {
        public:
            NotificationDataSource();
            property Windows::Foundation::Collections::IObservableVector<Object^>^ Items
            {
                Windows::Foundation::Collections::IObservableVector<Object^>^ get();
            }
            void Clear();
            void LoadData();
            void Refresh();
        private:
            void AddItem(Platform::String^ itemType, Platform::String^ itemId, Platform::String^ dueTime, Platform::String^ inputString, bool isTile);
            Platform::Collections::Vector<Object^>^ _items;
            Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ _formatter;
    };
}
