//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "stdafx.h"
#include "PropertySet.h"
#include <uiribbonpropertyhelpers.h>

const int PropertySet::MaxResourceLength = 256;

 PropertySet::PropertySet():
        m_categoryId(UI_COLLECTION_INVALIDINDEX),
        m_commandId(-1),
        m_commandType(UI_COMMANDTYPE_UNKNOWN)
{
}

PropertySet::~PropertySet()
{
}

//
// Initializes a property set for use with the ItemsSource property of a gallery of type "Commands."
//
HRESULT PropertySet::InitializeCommandProperties(int categoryId, int commandId, UI_COMMANDTYPE commandType)
{
    m_categoryId = categoryId;
    m_commandId = commandId;
    m_commandType = commandType;
    return S_OK;
}

//
// Initializes a property set for use with the ItemsSource property of any type of gallery other than "Commands."
//
HRESULT PropertySet::InitializeItemProperties(IUIImage* image, const std::wstring& label, int categoryId)
{
    if (image)
    {
        m_imageItem = image;
    }
    m_label = label;
    m_categoryId = categoryId;
    return S_OK;
}

// Retrieves the value of one of the properties used when adding something to a gallery.
HRESULT PropertySet::GetValue(__in REFPROPERTYKEY key, PROPVARIANT* propertyValue)
{
    if (key == UI_PKEY_ItemImage)
    {
        if (m_imageItem)
        {
            return UIInitPropertyFromImage(UI_PKEY_ItemImage, m_imageItem, propertyValue);
        }
        return S_FALSE;
    }
    else if (key == UI_PKEY_Label)
    {
        return UIInitPropertyFromString(UI_PKEY_Label, m_label.c_str(), propertyValue);
    }
    else if (key == UI_PKEY_CategoryId)
    {
        return UIInitPropertyFromUInt32(UI_PKEY_CategoryId, m_categoryId, propertyValue);
    }
    else if (key == UI_PKEY_CommandId)
    {
        if(m_commandId != -1)
        {
            return UIInitPropertyFromUInt32(UI_PKEY_CommandId, m_commandId, propertyValue);
        }
        return S_FALSE;
    }
    else if (key == UI_PKEY_CommandType)
    {
        return UIInitPropertyFromUInt32(UI_PKEY_CommandType, m_commandType, propertyValue);
    }
    return E_FAIL;
}
