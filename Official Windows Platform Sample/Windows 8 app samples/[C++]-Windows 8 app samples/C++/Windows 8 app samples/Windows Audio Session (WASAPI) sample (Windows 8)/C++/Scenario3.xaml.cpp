//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample;
using namespace SDKSample::WASAPIAudio;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Media;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3() :
    m_IsMFLoaded( false ),
    m_StateChangedEvent( nullptr ),
    m_spCapture( nullptr )
{
    InitializeComponent();

    // Initialize MF
    HRESULT hr = MFStartup( MF_VERSION, MFSTARTUP_LITE );
    if (SUCCEEDED( hr ))
    {
        m_IsMFLoaded = true;
    }
    else
    {
        ThrowIfFailed( hr );
    }

    m_CoreDispatcher = CoreWindow::GetForCurrentThread()->Dispatcher;
    m_Oscilliscope = safe_cast<Windows::UI::Xaml::Shapes::Polyline^>(static_cast<IFrameworkElement^>(this)->FindName("Oscilliscope"));
}

Scenario3::~Scenario3()
{
    if (m_deviceStateChangeToken.Value != 0)
    {
        m_StateChangedEvent->StateChangedEvent -= m_deviceStateChangeToken;

        m_StateChangedEvent = nullptr;
        m_deviceStateChangeToken.Value = 0;
    }

    if (m_plotDataReadyToken.Value != 0)
    {
        PlotDataReadyEvent::PlotDataReady -= m_plotDataReadyToken;
        m_plotDataReadyToken.Value = 0;
    }

    // Uninitialize MF
    if (m_IsMFLoaded)
    {
        MFShutdown();
        m_IsMFLoaded = false;
    }
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

/// <summary>
/// Invoked when about to leave this page.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (nullptr != m_StateChangedEvent)
    {
        DeviceState deviceState = m_StateChangedEvent->GetState();

        if (deviceState == DeviceState::DeviceStateCapturing)
        {
            StopCapture( this, e );
        }
    }
}

#pragma region UI Related Code
void Scenario3::ShowStatusMessage( Platform::String^ str, NotifyType messageType )
{
    m_CoreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler(
        [this, str, messageType]()
    {
        rootPage->NotifyUser( str, messageType );
    }));
}

// Updates transport controls based on current playstate
void Scenario3::UpdateMediaControlUI( DeviceState deviceState )
{
    switch( deviceState )
    {
    case DeviceState::DeviceStateCapturing:
        btnStartCapture->IsEnabled = false;
        btnStopCapture->IsEnabled = true;
        break;

    case DeviceState::DeviceStateStopped:
        btnStartCapture->IsEnabled = true;
        btnStopCapture->IsEnabled = false;
        break;

    case DeviceState::DeviceStateInitialized:
    case DeviceState::DeviceStateStarting:
    case DeviceState::DeviceStateStopping:
    case DeviceState::DeviceStateFlushing:
    case DeviceState::DeviceStateInError:
        btnStartCapture->IsEnabled = false;
        btnStopCapture->IsEnabled = false;
        break;
    }
}
#pragma endregion

#pragma region UI Event Handlers
void Scenario3::btnStartCapture_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ShowStatusMessage( "", NotifyType::StatusMessage );
    InitializeCapture( sender, e );
}

void Scenario3::btnStopCapture_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    StopCapture( sender, e );
}
#pragma endregion

// Event callback from WASAPI capture for changes in device state
void Scenario3::OnDeviceStateChange( Object^ sender, DeviceStateChangedEventArgs^ e )
{
    String^ strMessage = "";
    
    // Get the current time for messages
    auto t = Windows::Globalization::DateTimeFormatting::DateTimeFormatter::LongTime;
    Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();  
    calendar->SetToNow(); 

    // Update Control Buttons
    m_CoreDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler(
        [this, e]()
    {
        UpdateMediaControlUI( e->State );
    }));

    // Handle state specific messages
    switch( e->State )
    {
    case DeviceState::DeviceStateInitialized:
        m_spCapture->StartCaptureAsync();
        break;

    case DeviceState::DeviceStateCapturing:
        strMessage = "Capture Started @" + t->Format( calendar->GetDateTime() );
        ShowStatusMessage( strMessage, NotifyType::StatusMessage );
        break;

    case DeviceState::DeviceStateDiscontinutity:
        {
            m_DiscontinuityCount++;
            
            // Should always have a discontinuity when starting the capture, so will disregard it
            if (m_DiscontinuityCount > 1)
            {
                strMessage = "DISCONTINUITY DETECTED: " + t->Format( calendar->GetDateTime() ) + " (Count = " + (m_DiscontinuityCount - 1) + ")";
                ShowStatusMessage( strMessage, NotifyType::StatusMessage );
            }
        }
        break;

    case DeviceState::DeviceStateFlushing:
        PlotDataReadyEvent::PlotDataReady -= m_plotDataReadyToken;
        m_plotDataReadyToken.Value = 0;

        ShowStatusMessage( "Finalizing WAV Header.  This may take a few minutes...", NotifyType::StatusMessage );

        m_CoreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler(
            [this]()
        {
            m_Oscilliscope->Points->Clear();

            Windows::Foundation::Point p;
            p.X = OSC_START_X;
            p.Y = OSC_START_Y;

            m_Oscilliscope->Points->Append( p );
                    
            p.X = OSC_X_LENGTH;
            p.Y = OSC_START_Y;

            m_Oscilliscope->Points->Append( p );
        }));
        break;

    case DeviceState::DeviceStateStopped:
        // For the stopped state, completely tear down the audio device
        m_spCapture = nullptr;

        if (m_deviceStateChangeToken.Value != 0)
        {
            m_StateChangedEvent->StateChangedEvent -= m_deviceStateChangeToken;
            m_StateChangedEvent = nullptr;
            m_deviceStateChangeToken.Value = 0;
        }

        ShowStatusMessage( "Capture Stopped", NotifyType::StatusMessage );
        break;

    case DeviceState::DeviceStateInError:
        HRESULT hr = e->hr;
        
        if (m_deviceStateChangeToken.Value != 0)
        {
            m_StateChangedEvent->StateChangedEvent -= m_deviceStateChangeToken;
            m_StateChangedEvent = nullptr;
            m_deviceStateChangeToken.Value = 0;
        }

        m_spCapture = nullptr;

        wchar_t hrVal[11];
        swprintf_s( hrVal, 11, L"0x%08x\0", hr );
        String^ strHRVal = ref new String( hrVal );

        // Specifically handle a couple of known errors
        switch( hr )
        {
        case AUDCLNT_E_RESOURCES_INVALIDATED:
            strMessage = "ERROR: Endpoint Lost Access To Resources (" + strHRVal + ")";
            ShowStatusMessage( strMessage, NotifyType::ErrorMessage );
            break;

        case E_ACCESSDENIED:
            strMessage = "ERROR: Access Denied (" + strHRVal + ").  Check 'Settings->Permissions' for access to Microphone.";
            ShowStatusMessage( strMessage, NotifyType::ErrorMessage );
            break;

        default:
            strMessage = "ERROR: " + strHRVal + " has occurred.";
            ShowStatusMessage( strMessage, NotifyType::ErrorMessage );
        }
    }
}

// Event callback when visualization data is ready to be plotted
void Scenario3::OnPlotDataReady( Object^ sender, PlotDataReadyEventArgs^ e )
{
    Platform::Array<int,1>^ Points = e->Points;
    UINT32 Size = e->Size;

    m_CoreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler(
        [this, Points, Size]()
    {   
        // The last point value gives the maximum and minimum values in the data
        // Get absolute value of the minimum
        float y_max = static_cast<float>( abs( Points[Size - 1] ) );

        // Scale the incoming point list so that it fills our scope
        float x_inc = static_cast<float>( OSC_X_LENGTH ) / static_cast<float>( Size - 2 );

        UINT32 existingPoints = m_Oscilliscope->Points->Size;

        for (UINT32 i = 0; i < Size - 1; i++)
        {
            Point p;

            // Fixup the x and y coordinates and set it back into the collection
            p.X = OSC_START_X + i * x_inc;
            p.Y = OSC_START_Y - ( Points[i] * OSC_TOTAL_HEIGHT ) / ( 2.0F * y_max );

            if (i < existingPoints)
            {
                m_Oscilliscope->Points->SetAt(i, p);
            }
            else
            {
                m_Oscilliscope->Points->Append(p);
            }
        }
    }));
}

//
//  InitializeCapture()
//
void Scenario3::InitializeCapture( Platform::Object^ sender, Object^ e )
{
    HRESULT hr = S_OK;

    if (m_spCapture)
    {
        m_spCapture = nullptr;
    }

    // Create a new WASAPI capture instance
    m_spCapture = Make<WASAPICapture>();
        
    if (nullptr == m_spCapture)
    {
        OnDeviceStateChange( this, ref new DeviceStateChangedEventArgs( DeviceState::DeviceStateInError, E_OUTOFMEMORY ));
        return;
    }

    // Get a pointer to the device event interface
    m_StateChangedEvent = m_spCapture->GetDeviceStateEvent();

    if (nullptr == m_StateChangedEvent)
    {
        OnDeviceStateChange( this, ref new DeviceStateChangedEventArgs( DeviceState::DeviceStateInError, E_FAIL ));
        return;
    }

    // Register for events
    m_deviceStateChangeToken = m_StateChangedEvent->StateChangedEvent += ref new DeviceStateChangedHandler( this, &Scenario3::OnDeviceStateChange );
    m_plotDataReadyToken = PlotDataReadyEvent::PlotDataReady += ref new PlotDataReadyHandler( this, &Scenario3::OnPlotDataReady );

    // Reset discontinuity counter
    m_DiscontinuityCount = 0;

    // Perform the initialization
    m_spCapture->InitializeAudioDeviceAsync();
}

//
//  StopCapture()
//
void Scenario3::StopCapture( Platform::Object^ sender, Object^ e )
{
    if (m_spCapture)
    {
        // Set the event to stop playback
        m_spCapture->StopCaptureAsync();
    }
}

