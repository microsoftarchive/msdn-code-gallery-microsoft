// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "IResizable.h"

namespace Hilo
{
#pragma region DesignTimePhoto Class
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DesignTimePhoto sealed : public IResizable
    {
    public:
        property Windows::UI::Xaml::Media::ImageSource^ Image
        {
            Windows::UI::Xaml::Media::ImageSource^ get();
        }

        property Windows::UI::Xaml::Media::ImageSource^ Thumbnail
        {
            Windows::UI::Xaml::Media::ImageSource^ get();
        }

        property Platform::String^ Name { Platform::String^ get(); }
        property Platform::String^ DisplayType { Platform::String^ get(); }
        property Platform::String^ Resolution { Platform::String^ get(); }
        property Platform::String^ FormattedDateTaken { Platform::String^ get(); }
        property Platform::String^ FormattedTimeTaken { Platform::String^ get(); }
        property uint64 FileSize { uint64 get(); }
        property Platform::String^ FormattedPath { Platform::String^ get(); }

        property int ColumnSpan 
        {
            virtual int get();
            virtual void set(int value);
        }

        property int RowSpan
        {
            virtual int get();
            virtual void set(int value);
        }

    private:
        int m_columnSpan;
        int m_rowSpan;
    };
#pragma endregion

#pragma region DesignTimePhotoGroup Class
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DesignTimePhotoGroup sealed
    {
    public:
        DesignTimePhotoGroup(Platform::String^ title);

        property Windows::Foundation::Collections::IObservableVector<DesignTimePhoto^>^ Items
        {
            Windows::Foundation::Collections::IObservableVector<DesignTimePhoto^>^ get();
        }

        property Platform::String^ Title 
        { 
            Platform::String^ get(); 
        }

    private:
        Platform::String^ m_title;
    };
#pragma endregion

#pragma region DesignTimeMonthBlock Class
    ref class DesignTimeYearGroup;

    [Windows::UI::Xaml::Data::Bindable]
    public ref class DesignTimeMonthBlock sealed
    {
    public:
        DesignTimeMonthBlock(bool hasPhotos);

        property Platform::String^ Name 
        { 
            Platform::String^ get(); 
        }

        property bool HasPhotos
        { 
            bool get();
        }

    private:
        bool m_hasPhotos;
        
    };
#pragma endregion

#pragma region DesignTimeYearGroup Class
    [Windows::UI::Xaml::Data::Bindable]
    public ref class DesignTimeYearGroup sealed
    {
    public:
        DesignTimeYearGroup(Platform::String^ title);

        property Platform::String^ Title
        { 
            Platform::String^ get();
        }
               
        property Windows::Foundation::Collections::IObservableVector<DesignTimeMonthBlock^>^ Items
        {
            Windows::Foundation::Collections::IObservableVector<DesignTimeMonthBlock^>^ get();
        }

    private:
        Platform::String^ m_title;
        Platform::Collections::Vector<DesignTimeMonthBlock^>^ m_months;
    };
#pragma endregion

#pragma region DesignTimeData Class
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DesignTimeData sealed
    {
    public:
        DesignTimeData();

        property Windows::UI::Xaml::Media::ImageSource^ Image
        {
            Windows::UI::Xaml::Media::ImageSource^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<DesignTimePhoto^>^ Photos
        { 
            Windows::Foundation::Collections::IObservableVector<DesignTimePhoto^>^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<DesignTimePhotoGroup^>^ HubPhotosGroup
        {
            Windows::Foundation::Collections::IObservableVector<DesignTimePhotoGroup^>^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<DesignTimePhotoGroup^>^ MonthGroups
        {
            Windows::Foundation::Collections::IObservableVector<DesignTimePhotoGroup^>^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<DesignTimeYearGroup^>^ YearGroups
        {
            Windows::Foundation::Collections::IObservableVector<DesignTimeYearGroup^>^ get();
        }

        property DesignTimePhoto^ Photo { DesignTimePhoto^ get(); }
        property DesignTimePhoto^ SelectedItem { DesignTimePhoto^ get(); }
        property Platform::String^ Coarseness { Platform::String^ get(); }
        property Platform::String^ Passes { Platform::String^ get(); }
        property Platform::String^ MonthAndYear { Platform::String^ get(); }
        property Platform::String^ AppName { Platform::String^ get(); }
        property Platform::String^ ImageBrowserPageTitle { Platform::String^ get(); }

    private:
        DesignTimePhoto^ m_designTimePhoto;
    };
#pragma endregion
}
