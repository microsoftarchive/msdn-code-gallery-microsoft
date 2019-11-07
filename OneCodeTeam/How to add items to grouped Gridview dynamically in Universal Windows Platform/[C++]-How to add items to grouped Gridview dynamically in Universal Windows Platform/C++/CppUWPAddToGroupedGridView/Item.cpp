#include "pch.h"

#include "Item.h"
using namespace CppUWPAddToGroupedGridView::SampleData;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Foundation;

Item::Item()
{
	_isPropertyChangedObserved = false;	
}

Item::Item(Platform::String^ title, Platform::String^ category)
{
	Category = category;
	Title = title;
	_isPropertyChangedObserved = false;
}

Platform::String^ Item::Title::get() {
	return _title;
}

void Item::Title::set(Platform::String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

Platform::String^ Item::Category::get() {
	return _category;
}

void Item::Category::set(Platform::String^ value)
{
	if (_category != value)
	{
		_category = value;
		OnPropertyChanged("Category");
	}
}

Platform::String^ Item::ImageUrl::get() {
	return _imageUrl;
}

void Item::ImageUrl::set(Platform::String^ value)
{
	if (_imageUrl != value)
	{
		_imageUrl = value;
		OnPropertyChanged("ImageUrl");
	}
}

Platform::String^ Item::ToString() {
	return Title;
}