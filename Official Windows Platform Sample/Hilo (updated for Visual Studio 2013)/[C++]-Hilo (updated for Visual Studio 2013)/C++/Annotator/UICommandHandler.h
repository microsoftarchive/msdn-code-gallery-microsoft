//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

// Forward declare
__interface IImageEditor;

[uuid("795DF7AA-63EA-4A40-8727-EC6216CC1B2F")]
__interface IImageEditorCommandHandler : public IUnknown
{
    HRESULT __stdcall SetImageEditor(__in IImageEditor* imageEditor);
};

class UICommandHandler :  public IUICommandHandler, public IImageEditorCommandHandler
{
public:
    // IUICommandHandler methods
    HRESULT __stdcall UpdateProperty(unsigned int commandId, __in REFPROPERTYKEY key, __in_opt const PROPVARIANT* currentValue, __out PROPVARIANT* newValue);
    HRESULT __stdcall Execute (unsigned int commandId, UI_EXECUTIONVERB verb, __in_opt const PROPERTYKEY* key, __in_opt const PROPVARIANT* value, __in_opt IUISimplePropertySet* commandExecutionProperties);

    // IImageEditorCommandHandler methods
    HRESULT __stdcall SetImageEditor(__in IImageEditor* imageEditor);
    HRESULT __stdcall SetUIFramework(__in IUIFramework* framework);

protected:
    UICommandHandler();
    virtual ~UICommandHandler();
    
    // Interface helpers
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IUICommandHandler>::CastTo(iid, this, object) || CastHelper<IImageEditorCommandHandler>::CastTo(iid, this, object);
    }

private:
    static const int LineRibbonImageIds[];
    static const float StrokeSizes[];

    // Image edtor
    ComPtr<IImageEditor> m_imageEditor;

    // Ribbon image factory
    ComPtr<IUIImageFromBitmap> m_ribbonImageFactory;

    // Helper methods
    HRESULT CreateUIImageFromBitmapResource(int imageID, __out IUIImage** image);

    bool GetStatus(unsigned int commandId);
    bool GetToggleButtonStatus(unsigned int commandId);

};

