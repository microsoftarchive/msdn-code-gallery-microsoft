//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "ShellFileDialog.h"
#include "..\resource.h"

using namespace Hilo::WindowApiHelpers;
using namespace std;

const COMDLG_FILTERSPEC ShellFileDialog::m_openPictureFilter[] = 
                                          { {L"All Picture Files", L"*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png"},
                                            {L"Bitmap Files", L"*.bmp;*.dib"},
                                            {L"JPEG", L"*.jpg;*.jpeg;*.jpe;*.jfif"},
                                            {L"GIF", L"*.gif"},
                                            {L"TIFF", L"*.tif;*.tiff"},
                                            {L"PNG", L"*.png"},
                                          };

const COMDLG_FILTERSPEC ShellFileDialog::m_savePictureFilter[] = 
                                          {
                                              { L"Bitmap Files", L"*.bmp" },
                                              {L"JPEG", L"*.jpg;*.jpeg;*.jpe;*.jfif"},
                                              {L"GIF", L"*.gif"},
                                              {L"TIFF", L"*.tif;*.tiff"},
                                              {L"PNG", L"*.png"}
                                          };

ShellFileDialog::ShellFileDialog()
{
}

ShellFileDialog::~ShellFileDialog()
{
}

HRESULT ShellFileDialog::SetDefaultFolder(const GUID& folderId)
{
    ComPtr<IShellItem> defaultFolder;

    HRESULT hr = ::SHCreateItemInKnownFolder(folderId, 0, nullptr, IID_PPV_ARGS(&defaultFolder));
    if (SUCCEEDED(hr))
    {
        m_defaultFolder = defaultFolder;
    }

    return hr;
}

bool ShellFileDialog::IsValidExtention(wstring extension)
{
    bool valid = false;

    unsigned int filterCount = ARRAYSIZE(m_openPictureFilter);
    for( unsigned int i = 0 ; i < filterCount ; ++i )
    {
        // To prevent sub-matches, we need to make sure every extention ends with a ;.
        wstring delimitedSpec;
        delimitedSpec += m_openPictureFilter[i].pszSpec;
        delimitedSpec += L";";

        wstring delimitedExtension;
        delimitedExtension += extension;
        delimitedExtension += L";";
        
        if( wstring::npos != delimitedSpec.find(delimitedExtension) )
        {
            valid = true;
            break;
        }
    }

    return valid;
}

HRESULT ShellFileDialog::ProcessOpenDialogResults(IShellItemArray* results, bool* invalidFilesDetected, wstring* invalidFilesString, vector<ComPtr<IShellItem> >* shellItems)
{
    if ((nullptr == results) || (nullptr == invalidFilesDetected) || (nullptr == invalidFilesString) || (nullptr == shellItems))
    {
        return E_POINTER;
    }

    *invalidFilesDetected = false;
    *invalidFilesString = L"";

    unsigned long count = 0;
    HRESULT hr = results->GetCount(&count);

    if (SUCCEEDED(hr))
    {
        for (unsigned long i = 0; i < count; ++i)
        {
            ComPtr<IShellItem> shellItem;
            hr = results->GetItemAt(i, &shellItem);
            if (SUCCEEDED(hr))
            {
                WCHAR* name;

                shellItem->GetDisplayName(SIGDN_FILESYSPATH, &name);
                if (SUCCEEDED(hr) && (nullptr != name))
                {
                    wstring fileName = name;
                    CoTaskMemFree(name);

                    // File extension to determine which container format to use for the output file
                    wstring fileExtension(fileName.substr(fileName.find_last_of('.')));

                    // Convert all characters to lower case
                    transform(fileExtension.begin(), fileExtension.end(), fileExtension.begin (), tolower);

                    if( IsValidExtention(fileExtension) )
                    {
                        shellItems->push_back(shellItem);
                    }
                    else
                    {
                        *invalidFilesDetected = true;
                        *invalidFilesString += L"\n";
                        *invalidFilesString += fileName;
                    }
                }
            }
        }
    }

    return hr;
}

HRESULT ShellFileDialog::ShowOpenDialog(IWindow *parentWindow, vector<ComPtr<IShellItem> >* shellItems)
{
    if (nullptr == shellItems)
    {
        return E_POINTER;
    }
    shellItems->clear();

    HRESULT hr = S_OK;

    HWND hWnd = nullptr;
    if (nullptr != parentWindow)
    {
        hr = parentWindow->GetWindowHandle(&hWnd);
    }

    ComPtr<IFileOpenDialog> shellDialog;
    if (SUCCEEDED(hr))
    {
        // CoCreate the dialog object.
        hr = CoCreateInstance(CLSID_FileOpenDialog, 
                                nullptr, 
                                CLSCTX_INPROC_SERVER, 
                                IID_PPV_ARGS(&shellDialog));
    }

    // Set up the type filters...
    if (SUCCEEDED(hr))
    {
        hr = shellDialog->SetFileTypes(ARRAYSIZE(m_openPictureFilter), m_openPictureFilter);
    }

    // Set the default folder...
    if (SUCCEEDED(hr))
    {
        if (nullptr != m_defaultFolder)
        {
            hr = shellDialog->SetDefaultFolder(m_defaultFolder);
        }
    }

    // Mark it for multi-select...
    unsigned long dialogOptions = 0;
    if (SUCCEEDED(hr))
    {
        hr = shellDialog->GetOptions(&dialogOptions);
    }

    if (SUCCEEDED(hr))
    {
        hr = shellDialog->SetOptions(dialogOptions | FOS_ALLOWMULTISELECT);
    }

    bool showDialogAgain = false;

    do
    {
        showDialogAgain = false;
        // Show the dialog
        if (SUCCEEDED(hr))
        {
            hr = shellDialog->Show(hWnd);
        }

        // Obtain the result of the user's interaction with the dialog.
        ComPtr<IShellItemArray> results;
        if (SUCCEEDED(hr))
        {
            hr = shellDialog->GetResults(&results);
        }

        bool invalidFilesDetected = false;
        wstring invalidFilesString;
        if (SUCCEEDED(hr))
        {
            hr = ProcessOpenDialogResults(results, &invalidFilesDetected, &invalidFilesString, shellItems);
        }

        if (SUCCEEDED(hr) && invalidFilesDetected)
        {
            wstring invalidFilesMessage;

            static const int MaxResourceStringLength = 100;
            wchar_t resourceString[MaxResourceStringLength];

            // Load the dialog title...
            wstring openFileDialogErrorTitle;
            ::LoadString(GetModuleHandle(nullptr), IDS_OPEN_FILE_DIALOG_ERROR_TITLE, resourceString, MaxResourceStringLength);
            openFileDialogErrorTitle = resourceString;

            // Load the dialog error message...
            unsigned int openFileDialogErrorMessageId = IDS_SOME_FILES_NOT_OPENED;

            if( 0 == shellItems->size() )
            {
                openFileDialogErrorMessageId = IDS_NO_FILES_OPENED;
                showDialogAgain = true;
            }

            wstring openFileDialogErrorMessage;
            ::LoadString(GetModuleHandle(nullptr), openFileDialogErrorMessageId, resourceString, MaxResourceStringLength);
            openFileDialogErrorMessage = resourceString;

            // Load the file list header...
            ::LoadString(GetModuleHandle(nullptr), IDS_UNSUPPORTED_FILE_FORMAT_HEADER, resourceString, MaxResourceStringLength);
            invalidFilesMessage  = resourceString;

            // Concatenate the faulty files...
            invalidFilesMessage += invalidFilesString;

            // Display the message...
            ::TaskDialog(
                hWnd,
                nullptr, 
                openFileDialogErrorTitle.c_str(),
                openFileDialogErrorMessage.c_str(),
                invalidFilesMessage.c_str(),
                TDCBF_OK_BUTTON,
                TD_WARNING_ICON, 
                nullptr);
        }

    }while( SUCCEEDED(hr) && showDialogAgain );

    return hr;
}

HRESULT ShellFileDialog::ShowSaveDialog(IWindow *parentWindow, IShellItem* initialItem, IShellItem **shellItem)
{
    if (nullptr == shellItem)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    HWND hWnd = nullptr;
    if (nullptr != parentWindow)
    {
        hr = parentWindow->GetWindowHandle(&hWnd);
    }

    ComPtr<IFileSaveDialog> shellDialog;
    if (SUCCEEDED(hr))
    {
        // CoCreate the dialog object.
        hr = CoCreateInstance(CLSID_FileSaveDialog, nullptr, CLSCTX_INPROC, IID_PPV_ARGS(&shellDialog));
    }

    // Set the initial item...
    if (SUCCEEDED(hr))
    {
        hr = shellDialog->SetSaveAsItem(initialItem);
    }
    
    if (SUCCEEDED(hr))
    {
        wchar_t * buffer;
        if (SUCCEEDED(initialItem->GetDisplayName(SIGDN_FILESYSPATH, &buffer)))
        {
            // Extract extension and set default extension based on current file
            wstring fileExtension(buffer);
            fileExtension = fileExtension.substr(fileExtension.find_last_of('.') + 1);

            hr = shellDialog->SetDefaultExtension(fileExtension.c_str());
        }
    }

    // Set up the type filters...
    if (SUCCEEDED(hr))
    {
        hr = shellDialog->SetFileTypes(ARRAYSIZE(m_savePictureFilter), m_savePictureFilter);
    }

    // Show the dialog
    if (SUCCEEDED(hr))
    {
        hr = shellDialog->Show(hWnd);
    }

    // Obtain the selected shell item
    if (SUCCEEDED(hr))
    {
        hr = shellDialog->GetResult(shellItem);
    }

    return hr;
}
