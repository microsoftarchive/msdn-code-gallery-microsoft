//
// App.xaml.h
// Declaration of the App class.
//

#pragma once

#include "App.g.h"
#include "App.xaml.h"

namespace PdfShowcase
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    ref class App sealed
    {
    public:
        App();
        virtual void OnLaunched(_In_ Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ args) override;
    private:
        void OnSuspending(
            _In_ Platform::Object^ sender,
            _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
            );
    };
}
