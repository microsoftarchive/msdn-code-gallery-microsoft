//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "ShareDialog.h"
#include "Resource.h"
#include "FlickrUploader.h"

// Static members defined for this static dialog class
const std::vector<ComPtr<IThumbnail>> * ShareDialog::m_images;
const HBRUSH ShareDialog::m_dialogBackgroundColor = ::CreateSolidBrush(RGB(255, 255, 255));
HWND ShareDialog::m_dialogHandle = nullptr;
bool ShareDialog::m_isUploadingActive = false;
std::wstring ShareDialog::m_flickrToken;

ShareDialog::ShareDialog()
{
}

ShareDialog::~ShareDialog()
{
}

//
// Display the sharing dialog box to users to upload their photos
//
void ShareDialog::Show(Hilo::WindowApiHelpers::IWindow* parent, const std::vector<ComPtr<IThumbnail>>* images)
{
    FlickrUploader flickrUploader;

    // Check if we have valid Flickr values defined in the source code
    // This code can be removed once the appropriate values have been defined
    if (!flickrUploader.CheckForValidFlickrValues())
    {
        return;
    }

    // Store pointer to collection of thumbnail controls for later use
    m_images = images;

    // Show the sharing dialog
    HWND parentHandle;
    if (SUCCEEDED(parent->GetWindowHandle(&parentHandle)))
    {
        ::DialogBox(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDD_SHARE_DLG), parentHandle, ShareDialog::DlgProc);
    }
}

INT_PTR ShareDialog::DlgProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_INITDIALOG:
        {
            // Store dialog handle for later use
            m_dialogHandle = hDlg;

            // Initialize default values for dialog controls
            ::SendDlgItemMessage(m_dialogHandle, IDC_SERVICE, CB_ADDSTRING, 0, (LPARAM)L"Flickr");
            ::SendDlgItemMessage(m_dialogHandle, IDC_SERVICE, CB_SETCURSEL, 0, 0);
            ::SendDlgItemMessage(m_dialogHandle, IDC_RADIO_UPLOAD_SELECTION, BM_SETCHECK , BST_CHECKED, 0);

            return static_cast<INT_PTR>(TRUE);
        }

    case WM_COMMAND:
        {
            if (LOWORD(wParam) == IDOK)
            {
                UploadImages();
            }
            else if (LOWORD(wParam) == IDCANCEL)
            {
                m_isUploadingActive = false;
                ::EndDialog(hDlg, LOWORD(wParam));
            }

            return static_cast<INT_PTR>(TRUE);
        }


    case WM_CTLCOLORDLG:
        {
            return (long)m_dialogBackgroundColor;
        }

    case WM_CTLCOLORSTATIC:
        {
            HDC hdcStatic = (HDC)wParam;
            ::SetBkMode(hdcStatic, TRANSPARENT);

            return (long)m_dialogBackgroundColor;
        }

    case WM_NOTIFY:
        {
            // Check if the syslink control was clicked
            LPNMHDR pnmh = (LPNMHDR)lParam;
            if (pnmh->idFrom == IDC_VIEW_PHOTOS_LINK)
            {
                if ((pnmh->code == NM_CLICK) || (pnmh->code == NM_RETURN))
                {
                    PNMLINK link = (PNMLINK)lParam;
                    ::ShellExecute(nullptr, L"open", link->item.szUrl, nullptr, nullptr, SW_SHOWNORMAL);

                    return static_cast<INT_PTR>(TRUE);
                }
            }

            break;
        }
    }

    return static_cast<INT_PTR>(FALSE);
}

//
// Gets the number of images to upload based on the currently selected radio button
//
int ShareDialog::GetImageUploadCount()
{
    unsigned int selection = ::IsDlgButtonChecked(m_dialogHandle, IDC_RADIO_UPLOAD_SELECTION);

    if (BST_CHECKED == selection)
    {
        int count = 0;

        // Check number of images that are selected
        for (auto image = m_images->begin(); image != m_images->end(); image++)
        {
            // Get selection state
            ThumbnailSelectionState selectionState;
            if (SUCCEEDED((*image)->GetSelectionState(&selectionState)))
            {
                if ((selectionState & SelectionStateSelected) == SelectionStateSelected)
                {
                    count++;
                }
            }
        }

        return count;
    }
    else
    {
        return static_cast<int>(m_images->size());
    }
}

//
// Let's the user know that Flickr authorization is required
//
int ShareDialog::ShowAuthorizationNeededDialog()
{
    int buttonPressed                   = 0;
    TASKDIALOGCONFIG config             = {0};
    const TASKDIALOG_BUTTON buttons[]   = {{ IDOK, L"Authorize..." }};
    config.cbSize                       = sizeof(config);
    config.hwndParent                   = m_dialogHandle;
    config.hInstance                    = nullptr;
    config.dwCommonButtons              = TDCBF_CANCEL_BUTTON;
    config.pszWindowTitle               = MAKEINTRESOURCE(IDS_APP_TITLE);
    config.pszMainInstruction           = MAKEINTRESOURCE(IDS_AUTHORIZE_DIALOG_MESSAGE);
    config.pszContent                   = MAKEINTRESOURCE(IDS_AUTHORIZE_DIALOG_CONTENT);
    config.pszFooter                    = MAKEINTRESOURCE(IDS_AUTHORIZE_DIALOG_FOOTER);
    config.pButtons                     = buttons;
    config.cButtons                     = ARRAYSIZE(buttons);

    TaskDialogIndirect(&config, &buttonPressed, nullptr, nullptr);

    return buttonPressed;
}

//
// Let's the user specify that Flickr authorization is complete
//
int ShareDialog::ShowAuthorizationCompleteDialog()
{
    int buttonPressed                   = 0;
    TASKDIALOGCONFIG config             = {0};
    const TASKDIALOG_BUTTON buttons[]   = {{ IDOK, L"Authorization Complete..." }};
    config.cbSize                       = sizeof(config);
    config.hwndParent                   = m_dialogHandle;
    config.hInstance                    = nullptr;
    config.dwCommonButtons              = TDCBF_CANCEL_BUTTON;
    config.pszWindowTitle               = MAKEINTRESOURCE(IDS_APP_TITLE);
    config.pszMainInstruction           = MAKEINTRESOURCE(IDS_AUTHORIZE_DIALOG_MESSAGE2);
    config.pszContent                   = MAKEINTRESOURCE(IDS_AUTHORIZE_DIALOG_CONTENT2);
    config.pszFooter                    = MAKEINTRESOURCE(IDS_AUTHORIZE_DIALOG_FOOTER2);
    config.pButtons                     = buttons;
    config.cButtons                     = ARRAYSIZE(buttons);

    TaskDialogIndirect(&config, &buttonPressed, nullptr, nullptr);

    return buttonPressed;
}

//
// Called when the user click's the 'Upload' button. This method checks for proper authenication and then
// starts another thread to actually upload the photos.
//
bool ShareDialog::UploadImages()
{
    unsigned long threadId;
    HANDLE threadHandle = nullptr;

    // Make sure there are actual files to upload
    if (!(GetImageUploadCount() > 0))
    {
        ::MessageBox(m_dialogHandle, L"No images to upload...", L"Information", MB_OK);
        return false;
    }

    // Disable all visible controls on this dialog
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_RADIO_UPLOAD_SELECTION), FALSE);
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_RADIO_UPLOAD_FOLDER), FALSE);
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_SERVICE_LABEL), TRUE);
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_SERVICE), FALSE);
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDOK), FALSE);
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDCANCEL), FALSE);

    // Show dialog to ask for authorization
    // Flickr requires authentication directly on it's website
    if (ShowAuthorizationNeededDialog() == IDOK)
    {
        // Go ahead and start authorization process
        FlickrUploader flickrUploader;
        std::wstring frob = flickrUploader.Connect();

        // Wait for user to confirm authorization is complete
        if (ShowAuthorizationCompleteDialog() == IDOK)
        {
            m_flickrToken = flickrUploader.GetToken(frob);

            // Check that we got a valid token. If the user does not complete authorization then
            // the token is empty
            if (!m_flickrToken.empty())
            {
                // This value is checked by the upload thread to determine whether or not to continue uploading
                m_isUploadingActive = true;

                // Create new thread to handle uploading of images to Flickr
                threadHandle = ::CreateThread(nullptr, 0, ImageUploadThreadProc, nullptr, 0, &threadId);
            }
        }
    }

    if (threadHandle == nullptr)
    {
        // Uploading did not start.  Revert the dialog back to it's original state
        ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_RADIO_UPLOAD_SELECTION), TRUE);
        ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_RADIO_UPLOAD_FOLDER), TRUE);
        ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_SERVICE_LABEL), TRUE);
        ::EnableWindow(::GetDlgItem(m_dialogHandle, IDC_SERVICE), TRUE);
        ::EnableWindow(::GetDlgItem(m_dialogHandle, IDOK), TRUE);
        ::EnableWindow(::GetDlgItem(m_dialogHandle, IDCANCEL), TRUE);
    }

    return (threadHandle != nullptr);
}

//
// Thread process that handles uploading of images.
//
unsigned long WINAPI ShareDialog::ImageUploadThreadProc(void* /*threadData*/)
{
    int imageCount = GetImageUploadCount();
    unsigned int uploadSelection = IsDlgButtonChecked(m_dialogHandle, IDC_RADIO_UPLOAD_SELECTION);
    FlickrUploader flickrUploader;
    std::vector<std::wstring> photoIds;

    // Display progress bar and set range
    ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_UPLOAD_TEXT), SW_SHOW);
    ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_UPLOAD_PROGRESS), SW_SHOW);
    ::SendDlgItemMessage(m_dialogHandle, IDC_UPLOAD_PROGRESS, PBM_SETRANGE, 0, MAKELPARAM(0, imageCount));

    // Enable cancel button
    ::EnableWindow(::GetDlgItem(m_dialogHandle, IDCANCEL), TRUE);

    // Upload pictures
    int currentIndex = 0;
    for(auto image = m_images->begin(); image != m_images->end(); image++)
    {
        if (BST_CHECKED == uploadSelection)
        {
            // Get selection state
            ThumbnailSelectionState selectionState;
            if (SUCCEEDED((*image)->GetSelectionState(&selectionState)))
            {
                if ((selectionState & SelectionStateSelected) != SelectionStateSelected)
                {
                    // Image is not selected for upload
                    continue;
                }
            }
        }

        // Retrieve the filename
        ThumbnailInfo info;
        if (SUCCEEDED( (*image)->GetThumbnailInfo(&info) ))
        {
            // Upload image to Flickr
            bool errFound = false;
            std::wstring photoId = flickrUploader.UploadPhotos(m_flickrToken, info.GetFileName().c_str(), &errFound);
            if (errFound)
            {
                std::wstring errorMessage = L"Upload failed for: " + info.GetFileName() + L" Photo Id:" + photoId;
                MessageBox(0, errorMessage.c_str(), L"Flickr Upload Error", MB_ICONSTOP|MB_SETFOREGROUND);
            }
            else
            {
                photoIds.push_back(photoId);
            }
        }

        // Update progress bar
        currentIndex++;
        ::SendDlgItemMessage(m_dialogHandle, IDC_UPLOAD_PROGRESS, PBM_SETPOS, WPARAM(currentIndex), 0);

        if (!m_isUploadingActive)
        {
            // User has clicked cancel or closed the upload dialog. Terminate uploading
            photoIds.clear();
            break;
        }
    }

    if (!photoIds.empty())
    {
        // Create hyperlink to view photos that were just uploaded to Flickr
        std::wstring link(L"<a href=\"http://www.flickr.com/photos/upload/edit/?ids=");
        link += photoIds[0];

        if (photoIds.size() > 1)
        {
            for (size_t id = 1; id < photoIds.size(); id++)
            {
                link += L"," + photoIds[id];
            }
        }

        link += L"\">View Photos</a>";

        // Update hyperlink control with link to recently uploaded photos
        ::SendDlgItemMessage(m_dialogHandle, IDC_VIEW_PHOTOS_LINK, WM_SETTEXT, 0, LPARAM(link.c_str()));

        // Update dialog controls to show upload complete
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_UPLOAD_TEXT), SW_HIDE);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_VIEW_PHOTOS_LINK), SW_SHOW);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_UPLOAD_COMPLETE), SW_SHOW);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_RADIO_UPLOAD_SELECTION), SW_HIDE);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_RADIO_UPLOAD_FOLDER), SW_HIDE);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_SERVICE_LABEL), SW_HIDE);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDC_SERVICE), SW_HIDE);
        ::ShowWindow(::GetDlgItem(m_dialogHandle, IDOK), SW_HIDE);

        // Change caption on cancel button to close
        ::SendDlgItemMessage(m_dialogHandle, IDCANCEL, WM_SETTEXT, 0, (LPARAM)L"Close");
    }

    return 0;
}