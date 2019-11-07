//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "ShellItemsLoader.h"
#include <propkey.h>
#include <algorithm>

ShellItemsLoader::ShellItemsLoader()
{
}

ShellItemsLoader::~ShellItemsLoader()
{
}

//
// Enumerate Shell items in a given Shell namespace location, either recursively going through each subfolder, or in the top level folder only.
//
HRESULT ShellItemsLoader::EnumerateFolderItems(IShellItem* currentBrowseLocation, ShellFileType fileType, bool recursive, std::vector<ComPtr<IShellItem> >& shellItems)
{
    HRESULT hr = S_OK;

    if (recursive)
    {
        hr = ShellItemsLoader::EnumerateFolderItemsRecursive(currentBrowseLocation, fileType, shellItems);
    }
    else
    {
        hr = ShellItemsLoader::EnumerateFolderItemsNonRecursive(currentBrowseLocation, fileType, shellItems);
    }

    return hr;
}

HRESULT ShellItemsLoader::CreateScope(IShellItem* currentBrowseLocation, IShellItemArray **shellItemArray)
{
    *shellItemArray = nullptr;

    ComPtr<IObjectCollection> shellObjects;
    ComPtr<IShellLibrary> shellLibrary;

    HRESULT hr = CoCreateInstance(CLSID_ShellLibrary, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&shellLibrary));
    if (SUCCEEDED(hr))
    {
        hr = shellLibrary->GetFolders(LFF_FORCEFILESYSTEM, IID_PPV_ARGS(&shellObjects));
    }

    if (SUCCEEDED(hr))
    {
        hr = shellObjects->AddObject(currentBrowseLocation);
    }

    if (SUCCEEDED(hr))
    {
        hr = shellObjects.QueryInterface(shellItemArray);
    }

    return hr;
}

HRESULT ShellItemsLoader::CreateCondition(ShellFileType fileType, ICondition** condition)
{
    *condition = nullptr;

    std::vector<std::wstring> itemKinds;

    if ((fileType & FileTypeImage) == FileTypeImage)
    {
        itemKinds.push_back(L"picture");
    }

    if ((fileType & FileTypeImage) == FileTypeVideo)
    {
        itemKinds.push_back(L"video");
    }

    if ((fileType & FileTypeImage) == FileTypeAudio)
    {
        itemKinds.push_back(L"music");
    }

    size_t arrayLen = itemKinds.size();
    std::vector<ComPtr<ICondition> > conditionsVector(arrayLen);

    // Create the condition factory.  This interface helps create conditions.
    ComPtr<IConditionFactory2> conditionFactory;
    HRESULT hr = CoCreateInstance(CLSID_ConditionFactory, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&conditionFactory));

    if (SUCCEEDED(hr))
    {
        for (unsigned int i = 0; i < arrayLen; i++)
        {
            hr = conditionFactory->CreateStringLeaf(
                PKEY_Kind,
                COP_EQUAL,
                itemKinds[i].c_str(),
                nullptr,
                CONDITION_CREATION_DEFAULT,
                IID_PPV_ARGS(&conditionsVector[i]));
        }
    }

    if (SUCCEEDED(hr))
    {
        // Once all of the leaf conditions are created successfully, "AND" them together
        hr = conditionFactory->CreateCompoundFromArray(
            CT_OR_CONDITION,
            &conditionsVector[0],
            static_cast<unsigned long>(conditionsVector.size()),
            CONDITION_CREATION_DEFAULT,
            IID_PPV_ARGS(condition));
    }

    return hr;
}


HRESULT ShellItemsLoader::EnumerateFolderItemsNonRecursive(IShellItem* currentBrowseLocation, ShellFileType fileType, std::vector<ComPtr<IShellItem> >& shellItems)
{
    std::vector<std::wstring> itemKinds;

    if ((fileType & FileTypeImage) == FileTypeImage)
    {
        itemKinds.push_back(L"picture");
    }

    if ((fileType & FileTypeImage) == FileTypeVideo)
    {
        itemKinds.push_back(L"video");
    }

    if ((fileType & FileTypeImage) == FileTypeAudio)
    {
        itemKinds.push_back(L"music");
    }

    // Enumerate all objects in the current search folder
    ComPtr<IShellFolder> searchFolder;
    HRESULT hr = currentBrowseLocation->BindToHandler(nullptr, BHID_SFObject, IID_PPV_ARGS(&searchFolder));
    if (SUCCEEDED(hr))
    {
        bool const isEnumFolders = (fileType & FileTypeFolder) == FileTypeFolder;
        SHCONTF const flags = isEnumFolders ? SHCONTF_FOLDERS : SHCONTF_NONFOLDERS;

        ComPtr<IEnumIDList> fileList;
        if (S_OK == searchFolder->EnumObjects(nullptr, flags, &fileList)) // EnumObjects has "empty success semantics", so it could also return S_FALSE. Thus, we only check for S_OK
        {
            ITEMID_CHILD* idList = nullptr;
            unsigned long fetched;
            while (S_OK == fileList->Next(1, &idList, &fetched))
            {
                ComPtr<IShellItem2> shellItem;
                hr = SHCreateItemWithParent(nullptr, searchFolder, idList, IID_PPV_ARGS(&shellItem));
                if (SUCCEEDED(hr))
                {
                    if (isEnumFolders)
                    {
                        shellItems.push_back(static_cast<IShellItem*>(shellItem));
                    }
                    else
                    {
                        // Check if we the item is correct
                        wchar_t *itemType = nullptr;
                        hr = shellItem->GetString(PKEY_Kind, &itemType);
                        if (SUCCEEDED(hr))
                        {
                            auto found = std::find(itemKinds.begin(), itemKinds.end(), itemType);
                            if (found != itemKinds.end())
                            {
                                shellItems.push_back(static_cast<IShellItem*>(shellItem));
                            }
                            ::CoTaskMemFree(itemType);
                        }
                    }
                }

                ILFree(idList);
            }
        }
    }
    return hr;
}

HRESULT ShellItemsLoader::EnumerateFolderItemsRecursive(IShellItem* currentBrowseLocation, ShellFileType fileType, std::vector<ComPtr<IShellItem> >& shellItems)
{
    ComPtr<ISearchFolderItemFactory> searchFolderItemFactory;
    ComPtr<IShellItemArray> searchScope;

    HRESULT hr = CoCreateInstance(
        CLSID_SearchFolderItemFactory,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_PPV_ARGS(&searchFolderItemFactory));
    if (SUCCEEDED(hr))
    {
        hr = CreateScope(currentBrowseLocation, &searchScope);
    }

    if (SUCCEEDED(hr))
    {
        hr = searchFolderItemFactory->SetScope(searchScope);
    }

    ComPtr<ICondition> searchCondition;
    if (SUCCEEDED(hr))
    {
        hr = CreateCondition(fileType, &searchCondition);
    }

    if (SUCCEEDED(hr))
    {
        hr = searchFolderItemFactory->SetCondition(searchCondition);
    }

    ComPtr<IShellItem> shellItemSearch;
    if (SUCCEEDED(hr))
    {
        hr = searchFolderItemFactory->GetShellItem(IID_PPV_ARGS(&shellItemSearch));
    }

    // Do something with shellItemSearch
    if (SUCCEEDED(hr))
    {
        ComPtr<IEnumShellItems> enumItems;
        hr = shellItemSearch->BindToHandler(nullptr, BHID_EnumItems, IID_PPV_ARGS(&enumItems));
        if (SUCCEEDED(hr))
        {
            // note, this consumes all errors
            ComPtr<IShellItem> shellItem;
            unsigned long fetched;
            while SUCCEEDED(enumItems->Next(1, &shellItem, &fetched))
            {
                shellItems.push_back(shellItem);
                shellItem = nullptr; 
            }
        }
    }

    return hr;
}
