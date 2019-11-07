#include "pch.h"
#include "GroupInfoCollection.h"
#include "Item.h"


GroupInfoCollection::GroupInfoCollection(Platform::String^ key, Item^ item) {
	Key = key;
	auto collection = ref new Vector<Object^>(1,item);
	_storage = collection;
}


GroupInfoCollection::GroupInfoCollection() {
	_storage = ref new Vector<Object^>();
}


Platform::String^ GroupInfoCollection::Key::get() {
	return _key;
}

void GroupInfoCollection::Key::set(Platform::String^ value)
{
	if (_key != value)
	{
		_key = value;
	}
}

Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ GroupInfoCollection::Items::get() {
	return _storage;
}

void GroupInfoCollection::Items::set(Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ value)
{
	if (_storage != value)
	{
		_storage = dynamic_cast<Vector<Platform::Object^>^ >(value);
	}
}
