//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "UICommandHandler.h"
#include "ImageEditor.h"
#include "PropertySet.h"
#include "RibbonIds.h"
#include "Resource.h"

const float UICommandHandler::StrokeSizes[] = {2, 4, 8, 15};

const int UICommandHandler::LineRibbonImageIds[] = { 
    IDB_BITMAP_LINE01, 
    IDB_BITMAP_LINE02, 
    IDB_BITMAP_LINE03, 
    IDB_BITMAP_LINE04 
};

UICommandHandler::UICommandHandler()
{
}

UICommandHandler::~UICommandHandler()
{
}

//
//  Called by the Ribbon framework when a command property (PKEY) needs to be updated.
//  This function is used to provide new command property values, such as labels, icons, or
//  tooltip information, when requested by the Ribbon framework.
//
HRESULT UICommandHandler::UpdateProperty(unsigned int commandId, REFPROPERTYKEY key, const PROPVARIANT* currentValue, PROPVARIANT* newValue)
{
    HRESULT hr = E_FAIL;

    if (UI_PKEY_Categories == key)
    {
        // A return value of S_FALSE or E_NOTIMPL will result in a gallery with no categories.
        // If you return any error other than E_NOTIMPL, the contents of the gallery will not display.
        hr = S_FALSE;
    }
    else if (UI_PKEY_ItemsSource == key)
    {
        ComPtr<IUICollection> collection;
        hr = currentValue->punkVal->QueryInterface(IID_PPV_ARGS(&collection));
        if (FAILED(hr))
        {
            return hr;
        }

        // Populate the gallery with the four available line sizes.
        for (int i = 0; i < ARRAYSIZE(LineRibbonImageIds); i++)
        {
            // Create a new property set for each item.
            ComPtr<IPropertySet> item;
            hr = SharedObject<PropertySet>::Create(&item);
            if (FAILED(hr))
            {
                return hr;
            }

            // Create an IUIImage from a resource id.
            ComPtr<IUIImage> image;
            hr = CreateUIImageFromBitmapResource(LineRibbonImageIds[i], &image);
            if (FAILED(hr))
            {
                return hr;
            }
            
            // Initialize the property set with the image that were just loaded and no category.
            item->InitializeItemProperties(image, L"", UI_COLLECTION_INVALIDINDEX);

            // Add the newly-created property set to the collection supplied by the framework.
            collection->Add(item);
        }

        hr = S_OK;
    }
    else if (UI_PKEY_SelectedItem == key)
    {
        // Use the current size as the selection.
        hr = ::UIInitPropertyFromUInt32(UI_PKEY_SelectedItem, 0, newValue);
    }
    else if (UI_PKEY_Enabled == key)
    {
        return ::UIInitPropertyFromBoolean(UI_PKEY_Enabled, GetStatus(commandId) ? TRUE : FALSE, newValue);
    }
    else if (UI_PKEY_BooleanValue == key)
    {
        return ::UIInitPropertyFromBoolean(UI_PKEY_Enabled, GetToggleButtonStatus(commandId) ? TRUE : FALSE, newValue);
    }

    return hr;
}

//
//  Called by the Ribbon framework when a command is executed by the user.  
// For example, when a button is pressed.
//
HRESULT UICommandHandler::Execute(unsigned int commandId, UI_EXECUTIONVERB verb,  const PROPERTYKEY* key, const PROPVARIANT* value, IUISimplePropertySet* commandExecutionProperties)
{
    HRESULT hr = S_OK;

    switch (commandId)
    {
    case ID_FILE_OPEN:
        {
            hr = m_imageEditor->OpenFile();
            break;
        }
    case ID_FILE_SAVE:
        {
            hr  = m_imageEditor->SaveFiles();
            break;
        }
    case ID_FILE_SAVE_AS:
        {
            hr = m_imageEditor->SaveFileAs();
            break;
        }
    case ID_FILE_EXIT:
        {
            hr = m_imageEditor->SaveFiles();
            ::PostQuitMessage(0);
            break;
        }
    case rotate90Clockwise:
    case rotateButton:
        {
            hr = m_imageEditor->SetDrawingOperation(ImageOperationTypeRotateClockwise);
            break;
        }
    case rotate90CounterClockwise:
        {
            hr = m_imageEditor->SetDrawingOperation(ImageOperationTypeRotateCounterClockwise);
            break;
        }
    case flipHorizontal:
        {
            hr = m_imageEditor->SetDrawingOperation(ImageOperationTypeFlipHorizontal);
            break;
        }
    case flipVertical:
        {
            hr = m_imageEditor->SetDrawingOperation(ImageOperationTypeFlipVertical);
            break;
        }
    case ID_STANDARD_COLORPICKER:
        {
            if (*key == UI_PKEY_ColorType)
            {
                UI_SWATCHCOLORTYPE colorType = static_cast<UI_SWATCHCOLORTYPE>(::PropVariantToUInt32WithDefault(*value, UI_SWATCHCOLORTYPE_NOCOLOR));

                if (UI_SWATCHCOLORTYPE_RGB == colorType)
                {
                    PROPVARIANT var;
                    commandExecutionProperties->GetValue(UI_PKEY_Color, &var);
                    COLORREF color = ::PropVariantToUInt32WithDefault(var, 0);
                    hr = m_imageEditor->SetPenColor( 
                        D2D1::ColorF(
                        static_cast<float>(GetRValue(color)) / 255.0f,
                        static_cast<float>(GetGValue(color)) / 255.0f,
                        static_cast<float>(GetBValue(color)) / 255.0f
                        ));
                }
            }
            break;
        }
    case pencilButton:
        {
            hr = m_imageEditor->SetDrawingOperation(ImageOperationTypePen);
            break;
        }
    case cropButton:
        {
            hr = m_imageEditor->SetDrawingOperation(ImageOperationTypeCrop);
            break;
        }
    case ID_BUTTON_UNDO:
        {
            hr = m_imageEditor->Undo();
            break;
        }
    case ID_BUTTON_REDO:
        {
            hr = m_imageEditor->Redo();
            break;
        }
    case ID_BUTTON_ZOOM_IN:
        {
            hr = m_imageEditor->ZoomIn();
            break;
        }
    case ID_BUTTON_ZOOM_OUT:
        {
            hr = m_imageEditor->ZoomOut();
            break;
        }
    case ID_BUTTON_ZOOM_100:
        {
            hr = m_imageEditor->ZoomFull();
            break;
        }
    } // switch
    
    if (UI_EXECUTIONVERB_EXECUTE == verb)
    {
        if (nullptr != key && UI_PKEY_SelectedItem == (*key))
        {
            // Update the size
             unsigned int selectedIndex;
             hr = ::UIPropertyToUInt32(*key, *value, &selectedIndex);

             if (SUCCEEDED (hr))
             {
                 hr = m_imageEditor->SetPenSize(StrokeSizes[selectedIndex]);
             }
        }
    }

    m_imageEditor->UpdateUIFramework();

    return hr;
}

// Factory method to create IUIImages from resource identifiers.
HRESULT UICommandHandler::CreateUIImageFromBitmapResource(int imageID, IUIImage** image)
{
    *image = nullptr;

    HRESULT hr = E_FAIL;

    if (!m_ribbonImageFactory)
    {
        hr = CoCreateInstance(CLSID_UIRibbonImageFromBitmapFactory, 0, CLSCTX_ALL, IID_PPV_ARGS(&m_ribbonImageFactory));
        if (FAILED(hr))
        {
            return hr;
        }
    }

    // Load the bitmap from the resource file.
    HBITMAP hbm = static_cast<HBITMAP>(::LoadImage(HINST_THISCOMPONENT, MAKEINTRESOURCE(imageID), IMAGE_BITMAP, 0, 0, LR_CREATEDIBSECTION));

    if (hbm)
    {
        // Use the factory implemented by the framework to produce an IUIImage.
        hr = m_ribbonImageFactory->CreateImage(hbm, UI_OWNERSHIP_TRANSFER, image);
        if (FAILED(hr))
        {
            ::DeleteObject(hbm);
        }
    }
    return hr;
}


HRESULT UICommandHandler::SetImageEditor(__in IImageEditor* imageEditor)
{
    m_imageEditor = imageEditor;
    return S_OK;
}

bool UICommandHandler::GetStatus(unsigned int commandId)
{
    bool enabled = true;

    ImageOperationType imageDrawingOperation;
    m_imageEditor->GetDrawingOperation(&imageDrawingOperation);

    switch (commandId)
    {
    // A file can be saved if it has operations
    case ID_BUTTON_UNDO:
    case ID_FILE_SAVE:
        {
            m_imageEditor->CanUndo(&enabled);
            break;
        }
    case ID_BUTTON_REDO:
        {
            m_imageEditor->CanRedo(&enabled);
            break;
        }
    }

    return enabled;
}

bool UICommandHandler::GetToggleButtonStatus(unsigned int commandId)
{
    bool toggled = false;

    ImageOperationType imageDrawingOperation;
    m_imageEditor->GetDrawingOperation(&imageDrawingOperation);

    switch (commandId)
    {
    case pencilButton:
        {
            toggled = imageDrawingOperation == ImageOperationTypePen;
            break;
        }
    case cropButton:
        {
            toggled = imageDrawingOperation == ImageOperationTypeCrop;
            break;
        }
    }

    return toggled;
}
