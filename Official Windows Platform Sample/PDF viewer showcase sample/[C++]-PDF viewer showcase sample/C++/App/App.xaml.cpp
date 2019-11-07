//
// App.xaml.cpp
// Implementation of the App class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace PdfShowcase;

using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

/// <summary>
/// Initializes the singleton application object.  This is the first line of authored code
/// executed, and as such is the logical equivalent of main() or WinMain().
/// </summary>
App::App()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when the application is launched normally by the end user.  Other entry points
/// will be used when the application is launched to open a specific file, to display
/// search results, and so forth.
/// </summary>
/// <param name="args">Details about the launch request and process.</param>
void App::OnLaunched(_In_ LaunchActivatedEventArgs^ args)
{
    auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);

    // Do not repeat app initialization when the Window already has content, 
    // just ensure that the window is active 
    if (rootFrame == nullptr)
    {
        // Create a Frame to act as the navigation context and associate it with 
        // a SuspensionManager key 
        rootFrame = ref new Frame();
        if ((rootFrame->Content == nullptr) || (args->Arguments != nullptr))
        {
            // When the navigation stack isn't restored or there are launch arguments 
            // indicating an alternate launch (e.g.: via toast or secondary tile),  
            // navigate to the appropriate page, configuring the new page by passing required  
            // information as a navigation parameter 
            if (!rootFrame->Navigate(TypeName(MainPage::typeid), args->Arguments))
            {
                throw ref new FailureException("Failed to create initial page");
            }
        }
        // Place the frame in the current Window 
        Window::Current->Content = dynamic_cast<UIElement^>(rootFrame->Content);
        // Ensure the current window is active 
        Window::Current->Activate();
    }
    else
    {
        if ((rootFrame->Content == nullptr) || (args->Arguments != nullptr))
        {
            // When the navigation stack isn't restored or there are launch arguments 
            // indicating an alternate launch (e.g.: via toast or secondary tile),  
            // navigate to the appropriate page, configuring the new page by passing required  
            // information as a navigation parameter 
            if (!rootFrame->Navigate(TypeName(MainPage::typeid), args->Arguments))
            {
                throw ref new FailureException("Failed to create initial page");
            }
        }
        // Ensure the current window is active 
        Window::Current->Activate();
    }

    Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
}

void App::OnSuspending(
    _In_ Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // This is a good time to save your application's state in case the process gets terminated.
    auto mainPage = dynamic_cast<MainPage^>(Window::Current->Content);
    if (mainPage != nullptr)
    {
        mainPage->OnSuspending(sender, args);
    }
}
