//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "FileTypes.h"

// Windows headers
#include <PropVarUtil.h>
#include <ShlObj.h>
#include <StructuredQuery.h>

// std lib headers
#include <vector>

//
// Provides methods to enumerate shell items given a file type (such as folders or image files),
// both recursively or in a top top level folder
//
class ShellItemsLoader
{
public:
    static HRESULT EnumerateFolderItems(IShellItem* currentBrowseLocation, ShellFileType fileType, bool recursive, std::vector<ComPtr<IShellItem> >& shellItems);

private:
    ShellItemsLoader();
    virtual ~ShellItemsLoader();

    static HRESULT EnumerateFolderItemsRecursive(IShellItem* currentBrowseLocation, ShellFileType fileType, std::vector<ComPtr<IShellItem> >& shellItems);
    static HRESULT EnumerateFolderItemsNonRecursive(IShellItem* currentBrowseLocation, ShellFileType fileType, std::vector<ComPtr<IShellItem> >& shellItems);

    static HRESULT CreateScope(IShellItem* currentBrowseLocation, __out IShellItemArray **shellItemArray);
    static HRESULT CreateCondition(ShellFileType fileType, __out ICondition** condition);
};
