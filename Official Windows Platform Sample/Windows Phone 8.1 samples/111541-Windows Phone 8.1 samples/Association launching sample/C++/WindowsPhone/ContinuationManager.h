#pragma once

#include <windows.h>

using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Xaml::Controls;

namespace SDKSample
{
    /// <summary>
    /// ContinuationManager is used to detect if the most recent activation was due
    /// to a continuation such as the FileOpenPicker or WebAuthenticationBroker
    /// </summary>
    ref class ContinuationManager sealed
    {
    private:
        IContinuationActivatedEventArgs^ args;
        bool handled;
        Platform::Guid id;

    public:
        void Continue(IContinuationActivatedEventArgs^ args);
        void Continue(IContinuationActivatedEventArgs^ args, Frame^ rootFrame);
        void MarkAsStale();
        IContinuationActivatedEventArgs^ GetContinuationArgs(bool includeStaleArgs);
        property Platform::Guid Id
        {
            Platform::Guid get()
            {
                return id;
            }
        }
        property IContinuationActivatedEventArgs^ ContinuationArgs
        {
            IContinuationActivatedEventArgs^ get()
            {
                if (handled)
                    return nullptr;
                MarkAsStale();
                return args;
            }
        }
    };

    /// <summary>
    /// Implement this interface if your page invokes the file open picker
    /// API.
    /// </summary>
    public interface class IFileOpenPickerContinuable
    {
        /// <summary>
        /// This method is invoked when the file open picker returns picked
        /// files
        /// </summary>
        /// <param name="args">Activated event args object that contains returned files from file open picker</param>
        void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args);
    };

    /// <summary>
    /// Implement this interface if your page invokes the file save picker
    /// API
    /// </summary>
    public interface class IFileSavePickerContinuable
    {
        /// <summary>
        /// This method is invoked when the file save picker returns saved
        /// files
        /// </summary>
        /// <param name="args">Activated event args object that contains returned file from file save picker</param>
        void ContinueFileSavePicker(FileSavePickerContinuationEventArgs^ args);
    };

    /// <summary>
    /// Implement this interface if your page invokes the folder picker API
    /// </summary>
    public interface class IFolderPickerContinuable
    {
        /// <summary>
        /// This method is invoked when the folder picker returns the picked
        /// folder
        /// </summary>
        /// <param name="args">Activated event args object that contains returned folder from folder picker</param>
        void ContinueFolderPicker(FolderPickerContinuationEventArgs^ args);
    };

    /// <summary>
    /// Implement this interface if your page invokes the web authentication
    /// broker
    /// </summary>
    public interface class IWebAuthenticationContinuable
    {
        /// <summary>
        /// This method is invoked when the web authentication broker returns
        /// with the authentication result
        /// </summary>
        /// <param name="args">Activated event args object that contains returned authentication token</param>
        void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs^ args);
    };
}