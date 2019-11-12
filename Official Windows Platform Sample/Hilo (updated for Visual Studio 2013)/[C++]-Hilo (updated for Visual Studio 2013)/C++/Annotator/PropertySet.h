//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include <uiribbon.h>
#include "PropertySetInterface.h"

// The implementation of IUISimplePropertySet. This handles all of the properties used for the 
// ItemsSource and Categories PKEYs and provides functions to set only the properties required 
// for each type of gallery contents.
class PropertySet : public IUISimplePropertySet,
                    public IPropertySet
{
public:
    // Implementation for IPropertySet
    HRESULT __stdcall InitializeCommandProperties(__in int categoryId, __in int commandId, __in UI_COMMANDTYPE commandType);
    HRESULT __stdcall InitializeItemProperties(__in IUIImage* image, __in const std::wstring& label, int categoryId);

    // Required method that enables property key values to be retrieved on gallery collection items
    HRESULT __stdcall GetValue(__in REFPROPERTYKEY key, __out PROPVARIANT *propertyValue);

protected:
    // Constructor/destructor
    PropertySet();
    virtual ~PropertySet();

     // Interface support
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IUISimplePropertySet>::CastTo(iid, this, object) ||
            CastHelper<IPropertySet>::CastTo(iid, this, object);
    }

private:
    static const int MaxResourceLength;
    // Used for items and categories
    std::wstring m_label; 
    // Used for items, categories, and commands.
    int m_categoryId;
    // Used for items only
    ComPtr<IUIImage> m_imageItem;
    // Used for commands only.
    int m_commandId;
    // Used for commands only.
    UI_COMMANDTYPE m_commandType;

};

