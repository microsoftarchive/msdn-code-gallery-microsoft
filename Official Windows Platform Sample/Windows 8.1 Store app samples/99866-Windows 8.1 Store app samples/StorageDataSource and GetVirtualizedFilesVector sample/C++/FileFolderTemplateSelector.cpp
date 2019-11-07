// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// FileFolderTemplateSelector.cpp
// Implementation of the FileFolderTemplateSelector class
//

#include "pch.h"
#include "FileFolderTemplateSelector.h"

using namespace SDKSample::DataSourceAdapter;

using namespace Windows::Storage;
using namespace Windows::UI::Xaml;

FileFolderTemplateSelector::FileFolderTemplateSelector()
{
}

FileFolderTemplateSelector::~FileFolderTemplateSelector()
{
}

DataTemplate^ FileFolderTemplateSelector::SelectTemplateCore(Object^ item, DependencyObject^ container)
{
    // determine the type of the item and set the correct template to display the item.
    auto folder = dynamic_cast<StorageFolder^>(item);
    return (folder != nullptr) ? FolderInformationTemplate : FileInformationTemplate;
}
