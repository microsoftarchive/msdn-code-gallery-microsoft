//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

[uuid("141AA5B7-B583-4216-AB72-233F45EF9542")]
__interface IPropertySet : public IUnknown
{
    HRESULT __stdcall InitializeCommandProperties(__in int categoryId, __in int commandId, __in UI_COMMANDTYPE commandType);
    HRESULT __stdcall InitializeItemProperties(__in IUIImage* image, __in const std::wstring& label, int categoryId);
};
