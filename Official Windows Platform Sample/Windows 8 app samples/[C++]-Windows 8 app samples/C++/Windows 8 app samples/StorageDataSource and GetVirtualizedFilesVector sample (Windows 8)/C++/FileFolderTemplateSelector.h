// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// FileFolderTemplateSelector.h
// Declaration of the FileFolderTemplateSelector class
//

#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace DataSourceAdapter
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class FileFolderTemplateSelector sealed : Windows::UI::Xaml::Controls::DataTemplateSelector
        {
        public:
            FileFolderTemplateSelector();
    
            property Windows::UI::Xaml::DataTemplate^ FileInformationTemplate;
            property Windows::UI::Xaml::DataTemplate^ FolderInformationTemplate;
    
        protected:
            virtual Windows::UI::Xaml::DataTemplate^ SelectTemplateCore(Object^ item, Windows::UI::Xaml::DependencyObject^ container) override;
    
    	private:
    		~FileFolderTemplateSelector();
        };
    }
}
