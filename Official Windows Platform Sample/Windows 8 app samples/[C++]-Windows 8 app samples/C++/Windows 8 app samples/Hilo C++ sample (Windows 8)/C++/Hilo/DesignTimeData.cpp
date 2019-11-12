// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "DesignTimeData.h"

using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;

#pragma region DesignTimePhoto Members

ImageSource^ DesignTimePhoto::Image::get()
{
    return ref new BitmapImage(ref new Uri("ms-appx:///Assets/HiloLogo.png"));
}

ImageSource^ DesignTimePhoto::Thumbnail::get()
{
    return Image;
}

String^ DesignTimePhoto::Name::get()
{
    return "Photo name";
}

String^ DesignTimePhoto::DisplayType::get()
{
    return "JPG File";
}

String^ DesignTimePhoto::Resolution::get()
{
    return "1024 x 768";
}

String^ DesignTimePhoto::FormattedDateTaken::get()
{
    return "01/01/2012";
}

String^ DesignTimePhoto::FormattedTimeTaken::get()
{
    return "10:48";
}

uint64 DesignTimePhoto::FileSize::get()
{
    return 1234567;
}

String^ DesignTimePhoto::FormattedPath::get()
{
    return "C:\\Users";
}

int DesignTimePhoto::RowSpan::get()
{
    return m_rowSpan;
}

void DesignTimePhoto::RowSpan::set(int value)
{
    m_rowSpan = value;
}

int DesignTimePhoto::ColumnSpan::get()
{
    return m_columnSpan;
}

void DesignTimePhoto::ColumnSpan::set(int value)
{
    m_columnSpan = value;
}

#pragma endregion

#pragma region DesignTimePhotoGroup Members

DesignTimePhotoGroup::DesignTimePhotoGroup(String^ title) : m_title(title)
{
}

IObservableVector<DesignTimePhoto^>^ DesignTimePhotoGroup::Items::get()
{
    auto photos = ref new Vector<DesignTimePhoto^>();
    auto designTimePhoto = ref new DesignTimePhoto();
    bool firstPhoto = true;
    for (int i = 0; i < 6; i++)
    {
        if (firstPhoto)
        {
            auto photo = ref new DesignTimePhoto();
            photo->ColumnSpan = 2;
            photo->RowSpan = 2;
            photos->Append(photo);
            firstPhoto = false;
        }
        else
        {
            photos->Append(designTimePhoto);
        }
    }
    return photos;
}

String^ DesignTimePhotoGroup::Title::get()
{
    return m_title;
}

#pragma endregion

#pragma region DesignTimeMonthBlock Members

DesignTimeMonthBlock::DesignTimeMonthBlock(bool hasPhotos) : m_hasPhotos(hasPhotos)
{
}

String^ DesignTimeMonthBlock::Name::get()
{
    return "Month";
}

bool DesignTimeMonthBlock::HasPhotos::get()
{
    return m_hasPhotos;
}

#pragma endregion

#pragma region DesignTimeYearGroup Members

DesignTimeYearGroup::DesignTimeYearGroup(String^ title) : m_title(title), m_months(nullptr)
{
}

String^ DesignTimeYearGroup::Title::get()
{
    return m_title;
}

IObservableVector<DesignTimeMonthBlock^>^ DesignTimeYearGroup::Items::get()
{
    if (m_months == nullptr)
    {
        const unsigned int months = 12;
        bool hasPhotos = true;
        std::vector<DesignTimeMonthBlock^> monthBlocks;
        monthBlocks.reserve(months);
        for (int i = 0; i < months; i++)
        {
            auto monthBlock = ref new DesignTimeMonthBlock(hasPhotos);
            monthBlocks.push_back(monthBlock);
            hasPhotos = !hasPhotos;
        }
        m_months = ref new Vector<DesignTimeMonthBlock^>(std::move(monthBlocks));
    }
    return m_months;
}

#pragma endregion

#pragma region DesignTimeData Members

DesignTimeData::DesignTimeData() : m_designTimePhoto(ref new DesignTimePhoto())
{
}

ImageSource^ DesignTimeData::Image::get()
{
    return m_designTimePhoto->Image;
}

IObservableVector<DesignTimePhoto^>^ DesignTimeData::Photos::get()
{
    auto photos = ref new Vector<DesignTimePhoto^>();
    for (int i = 0; i < 10; i++)
    {
        photos->Append(m_designTimePhoto);
    }
    return photos;
}

IObservableVector<DesignTimePhotoGroup^>^ DesignTimeData::HubPhotosGroup::get()
{
    auto photosGroup = ref new Vector<DesignTimePhotoGroup^>();
    auto photoGroup = ref new DesignTimePhotoGroup("Pictures");
    photosGroup->Append(photoGroup);
    return photosGroup;
}

IObservableVector<DesignTimePhotoGroup^>^ DesignTimeData::MonthGroups::get()
{
    auto monthsGroup = ref new Vector<DesignTimePhotoGroup^>();
    auto photoGroup = ref new DesignTimePhotoGroup("Month and year");
    monthsGroup->Append(photoGroup);
    monthsGroup->Append(photoGroup);
    return monthsGroup;
}

IObservableVector<DesignTimeYearGroup^>^ DesignTimeData::YearGroups::get()
{
    auto yearsGroup = ref new Vector<DesignTimeYearGroup^>();
    auto yearGroup1 = ref new DesignTimeYearGroup("Year");
    auto yearGroup2 = ref new DesignTimeYearGroup("Year");
    yearsGroup->Append(yearGroup1);
    yearsGroup->Append(yearGroup2);
    return yearsGroup;
}

DesignTimePhoto^ DesignTimeData::Photo::get()
{
    return m_designTimePhoto;
}

DesignTimePhoto^ DesignTimeData::SelectedItem::get()
{
    return Photo;
}

String^ DesignTimeData::Coarseness::get()
{
    return "Coarseness"; 
}

String^ DesignTimeData::Passes::get()
{
    return "Passes";
}

String^ DesignTimeData::MonthAndYear::get()
{
    return "Month and year";
}

String^ DesignTimeData::AppName::get()
{
    return "Hilo";
}

String^ DesignTimeData::ImageBrowserPageTitle::get()
{
    return "Pictures";
}

#pragma endregion
