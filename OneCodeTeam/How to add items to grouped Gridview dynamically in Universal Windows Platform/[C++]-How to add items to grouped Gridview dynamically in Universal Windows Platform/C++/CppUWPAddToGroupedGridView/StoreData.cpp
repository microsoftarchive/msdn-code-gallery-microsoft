#include "pch.h"
#include "StoreData.h"

IObservableVector<Item^>^ StoreData::Collection::get() {
	return _collection;
}

void StoreData::Collection::set(IObservableVector<Item^>^ value)
{
	if (_collection != value)
	{
		_collection = dynamic_cast<Vector<Item^>^ > (value);		
	}
}


StoreData::StoreData() 
{
	Collection = ref new Vector<Item^>();
	Item^ item =ref new Item("Banana Blast Frozen Yogurt", "Low-fat frozen yogurt");
	item->ImageUrl = "60Banana.png";
	Collection->Append(item);

	item = ref new Item("Lavish Lemon Ice", "Sorbet");
	item->ImageUrl = "60Lemon.png";
	Collection->Append(item);

	item = ref new Item("Marvelous Mint", "Gelato");
	item->ImageUrl = "60Mint.png";
	Collection->Append(item);

	item = ref new Item("Creamy Orange", "Sorbet");
	item->ImageUrl = "60Orange.png";
	Collection->Append(item);

	item = ref new Item("Succulent Strawberry", "Sorbet");
	item->ImageUrl = "60Strawberry.png";
	Collection->Append(item);

	item = ref new Item("Very Vanilla", "Ice Cream");
	item->ImageUrl = "60Vanilla.png";
	Collection->Append(item);

	item = ref new Item("Creamy Caramel Frozen Yogurt", "Low-fat frozen yogurt");
	item->ImageUrl = "60SauceCaramel.png";
	Collection->Append(item);

	item = ref new Item("Chocolate Lovers Frozen Yougurt", "Low-fat frozen yogurt");
	item->ImageUrl = "60SauceChocolate.png";
	Collection->Append(item);

	item = ref new Item("Roma Strawberry", "Gelato");
	item->ImageUrl = "60Strawberry.png";
	Collection->Append(item);

	item = ref new Item("Italian Rainbow", "Gelato");
	item->ImageUrl = "60SprinklesRainbow.png";
	Collection->Append(item);

	item = ref new Item("Strawberry", "Ice Cream");
	item->ImageUrl = "60Strawberry.png";
	Collection->Append(item);

	item = ref new Item("Strawberry Frozen Yogurt", "Low-fat frozen yogurt");
	item->ImageUrl = "60Strawberry.png";
	Collection->Append(item);

	item = ref new Item("Bongo Banana", "Sorbet");
	item->ImageUrl = "60Banana.png";
	Collection->Append(item);

	item = ref new Item("Firenze Vanilla", "Gelato");
	item->ImageUrl = "60Vanilla.png";
	Collection->Append(item);

	item = ref new Item("Choco-wocko", "Sorbet");
	item->ImageUrl = "60SauceChocolate.png";
	Collection->Append(item);

	item = ref new Item("Chocolate","Ice Cream" );
	item->ImageUrl = "60SauceChocolate.png";
	Collection->Append(item);
}

/// <summary>
/// The method returns the list of groups, each containing a key and a list of items. 
/// The key of each group is the category of the items. 
/// </summary>
/// <returns>
/// The List of groups of items. 
/// </returns>
Platform::Collections::Vector<GroupInfoCollection^>^ StoreData::GetGroupsByCategory()
{
	Map<Platform::String^, int>^  map = ref new Map<Platform::String^, int>();
	Vector<GroupInfoCollection^>^ groups = ref new Vector<GroupInfoCollection^> ();
	for each (auto item in Collection)
	{
		if (map->HasKey(item->Category)) {
			auto groupInfoCollection = groups->GetAt(map->Lookup(item->Category));
			groupInfoCollection->Items->Append(item);
		}
		else {
			groups->Append(ref new GroupInfoCollection(item->Category,item));
			map->Insert(item->Category, groups->Size - 1);
		}
	}
	return groups;
}