#include "pch.h"
#include "ContinuationManager.h"

using namespace SDKSample;

using namespace Platform;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml;

/// <summary>
/// Sets the ContinuationArgs for this instance. Using default Frame of current Window
/// Should be called by the main activation handling code in App.xaml.cs
/// </summary>
/// <param name="args">The activation args</param>
void ContinuationManager::Continue(IContinuationActivatedEventArgs^ args)
{
    Continue(args, dynamic_cast<Frame^>(Window::Current->Content));
}

/// <summary>
/// Sets the ContinuationArgs for this instance. Should be called by the main activation
/// handling code in App.xaml.cs
/// </summary>
/// <param name="args">The activation args</param>
/// <param name="rootFrame">The frame control that contains the current page</param>
void ContinuationManager::Continue(IContinuationActivatedEventArgs^ args, Frame^ rootFrame)
{
    this->args = args;
    this->handled = false;

    GUID result;
    if (SUCCEEDED(CoCreateGuid(&result)))
    {
        this->id = Platform::Guid(result);
    }


    if (rootFrame == nullptr)
        return;

    switch (args->Kind)
    {
    case ActivationKind::PickFileContinuation:
        {
            auto fileOpenPickerPage = dynamic_cast<IFileOpenPickerContinuable^>(rootFrame->Content);
            if (fileOpenPickerPage != nullptr)
            {
                fileOpenPickerPage->ContinueFileOpenPicker(dynamic_cast<FileOpenPickerContinuationEventArgs^>(args));
            }
        }
        break;
    case ActivationKind::PickSaveFileContinuation:
        {
            auto fileSavePickerPage = dynamic_cast<IFileSavePickerContinuable^>(rootFrame->Content);
            if (fileSavePickerPage != nullptr)
            {
                fileSavePickerPage->ContinueFileSavePicker(dynamic_cast<FileSavePickerContinuationEventArgs^>(args));
            }
        }
        break;
    case ActivationKind::PickFolderContinuation:
        {
            auto folderPickerPage = dynamic_cast<IFolderPickerContinuable^>(rootFrame->Content);
            if (folderPickerPage != nullptr)
            {
                folderPickerPage->ContinueFolderPicker(dynamic_cast<FolderPickerContinuationEventArgs^>(args));
            }
        }
        break;
    case ActivationKind::WebAuthenticationBrokerContinuation:
        {
            auto webAuthenticationPage = dynamic_cast<IWebAuthenticationContinuable^>(args);
            if (webAuthenticationPage != nullptr)
            {
                webAuthenticationPage->ContinueWebAuthentication(dynamic_cast<WebAuthenticationBrokerContinuationEventArgs^>(args));
            }
        }
        break;
    }
}

/// <summary>
/// Marks the contination data as 'stale', meaning that it is probably no longer of
/// any use. Called when the app is suspended (to ensure future activations don't appear
/// to be for the same continuation) and whenever the continuation data is retrieved 
/// (so that it isn't retrieved on subsequent navigations)
/// </summary>
void ContinuationManager::MarkAsStale()
{
    this->handled = true;
}

/// <summary>
/// Retrieves the continuation args, if they have not already been retrieved, and 
/// prevents further retrieval via this property (to avoid accidentla double-usage)
/// </summary>
IContinuationActivatedEventArgs^ ContinuationManager::GetContinuationArgs(bool includeStaleArgs)
{
    if (!includeStaleArgs && handled)
        return nullptr;
    else
    {
        MarkAsStale();
        return args;
    }
}