//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

class ContentImageSource;

ref class Document;
ref class Element;

namespace Magazine
{
    //  Page model binds one page of content to one item in the FlipView's items source.
    //  It implements the custom property interfaces used to resolve property name string to
    //  actual runtime object.
    //
    //  A page model exposes one property for background content and another for top-level
    //  content. It assumes that the first <layer> content element of the <page> element is
    //  the background, and the next <layer> element is the top-level content. It also exposes
    //  the size of the content it binds. It is important for VirtualSurfaceImageSource that
    //  the size of the content is identical to the size of the UI element hosting the content
    //  or the content will be stretched to fit.
    //
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PageModel sealed :
        public Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    public:
        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        property Windows::UI::Xaml::Media::ImageSource^ Content
        {
            Windows::UI::Xaml::Media::ImageSource^ get()
            {
                return m_content->GetImageSource();
            }
        }

        property Windows::UI::Xaml::Media::ImageSource^ Background
        {
            Windows::UI::Xaml::Media::ImageSource^ get()
            {
                return m_background->GetImageSource();
            }
        }

        property int32 ContentWidth
        {
            int32 get()
            {
                return m_content->GetImageSize().cx;
            }
        }

        property int32 ContentHeight
        {
            int32 get()
            {
                return m_content->GetImageSize().cy;
            }
        }

    internal:
        PageModel(
            _In_ Element^ content,
            _In_ Document^ document
            );

        void UpdateContent(_In_ Element^ content);

    private:
        // Image source of the content image
        Microsoft::WRL::ComPtr<ContentImageSource> m_content;

        // Image source of the background image
        Microsoft::WRL::ComPtr<ContentImageSource> m_background;
    };
}
