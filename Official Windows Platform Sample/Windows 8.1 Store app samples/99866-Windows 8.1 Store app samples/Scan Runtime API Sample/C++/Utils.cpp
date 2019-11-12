//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "Utils.h"
#include "MainPage.xaml.h"
#include <sstream> 

using namespace SDKSample;

using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace concurrency;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Controls;

/// <summary>
/// Returns a Bitmap image for a give random access stream 
/// </summary>
/// <param name="stream">Random access stream for which the preview bitmap need to be generated</param>
/// <return Type="BitmapImage">Bitmap for the given stream</return>
BitmapImage^ Utils::GetImageFromFile(_In_ IRandomAccessStream^ stream)
{
    BitmapImage^ bitmap = ref new BitmapImage();

    bitmap->SetSource(stream);
    return bitmap;
}

/// <summary>
/// Set the image source that is given to a bitmap file that is generated from the given storage file
/// </summary>
/// <param name="file">Storage file used to generate a bitmap image that is assigned as source to the given image object</param>
/// <param name="img">Image for which the source needs to be set to the generated a bitmap from given storage file</param>
void Utils::SetImageSourceFromFile(_In_ StorageFile^ file, _Inout_ Image^ img)
{
    create_task(file->OpenAsync(FileAccessMode::Read)).then([file, img](IRandomAccessStream^ stream) 
    {
        BitmapImage ^bitmap = Utils::GetImageFromFile(stream);	

        if ((bitmap->PixelHeight > img->Height) || (bitmap->PixelWidth > img->Width))
        {
            img->Stretch = Windows::UI::Xaml::Media::Stretch::Uniform;
        }
        else
        {
            img->Stretch = Windows::UI::Xaml::Media::Stretch::None;
        }
        img->Source = bitmap;
    });
}

/// <summary>
/// Set the image source that is given to a bitmap file that is generated from the given random access stream
/// </summary>
/// <param name="stream">Random access stream used to generate a bitmap image that is assigned as source to the given image object</param>
/// <param name="img">Image for which the source needs to be set to the generated bitmap from given stream</param>
void Utils::SetImageSourceFromStream(_In_ IRandomAccessStream^ stream, _Inout_ Image^ img)
{
    BitmapImage^ bitmap = Utils::GetImageFromFile(stream);	
    

    if ((bitmap->PixelHeight >= img->ActualHeight) || (bitmap->PixelWidth >= img->ActualWidth))
    {
        img->Stretch = Windows::UI::Xaml::Media::Stretch::Uniform;
    }
    else
    {
        img->Stretch = Windows::UI::Xaml::Media::Stretch::None;
    }
    img->Source = bitmap;
}

/// <summary>
/// Display the image of the first scanned file and output the corresponding message
/// </summary>
/// <param name="FileStorageList">List of storage files create by the Scan Runtime API for all the scanned images given by the scanner</param>
/// <param name="img">Image for which the source need to be set to the generated bitmap from first storage file</param>
void Utils::DisplayImageAndScanCompleteMessage(_In_ IVectorView<StorageFile ^>^ FileStorageList, _Inout_ Image ^img)
{
    StorageFile^ file = FileStorageList->GetAt(0);						
    Utils::SetImageSourceFromFile(file, img);		
    if (FileStorageList->Size > 1)
    {
        MainPage::Current->NotifyUser("Scanning is complete. Below is the first of the scanned images. \n" +
            "All the scanned files have been saved to local My Pictures folder." , NotifyType::StatusMessage);
    }
    else
    {
        MainPage::Current->NotifyUser("Scanning is complete. Below is the scanned image.\n"+
                        "Scanned file has been saved to local My Pictures folder with file name: " + file->Name, NotifyType::StatusMessage);	
    }
}

/// <summary>
/// Add the file names of the scanned files to the list of file names that will be displayed to the user
/// </summary>
/// <param name="FileStorageList">List of storage files create by the Scan Runtime API for all the scanned images given by the scanner</param>
/// <param name="Data">ModelDataContext object which contains the current data Context </param>
void Utils::UpdateFileListData(_In_ Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile ^>^ FileStorageList, _Inout_ Common::ModelDataContext ^DataContext)
{
    // populate list with the names of the files that are scanned			
    for(unsigned int i = 0; i < FileStorageList->Size; i++)
    {
        DataContext->AddToFileList(FileStorageList->GetAt(i));
    }
}

/// <summary>
/// Displays an error message when an exception is thrown during the scenario and set ScenarioRunning of the Data context to false
/// </summary>
void Utils::OnScenarioException(_In_ Platform::Exception^ e, _Inout_ Common::ModelDataContext ^Data)
{
    DisplayExceptionErrorMessage(e);
    Data->ScenarioRunning = false;
}

/// <summary>
/// Displays an error message when an exception is thrown during the scenario
/// </summary>
void Utils::DisplayExceptionErrorMessage(_In_ Platform::Exception^ e)
{
    std::wstringstream message;
    message << L"An exception occurred while executing the scenario. Exception HRESULT: 0x" << std::hex << e->HResult;
    Platform::String ^errorMessage = ref new Platform::String(message.str().c_str());
    MainPage::Current->NotifyUser(errorMessage, NotifyType::ErrorMessage);
}

/// <summary>
/// Displays cancellation message when user cancels scanning
/// </summary>
void Utils::DisplayScanCancelationMessage()
{
    MainPage::Current->NotifyUser("Scanning has been cancelled. Files that have been scanned so far would be saved to the local My Pictures folder." , NotifyType::StatusMessage);
}