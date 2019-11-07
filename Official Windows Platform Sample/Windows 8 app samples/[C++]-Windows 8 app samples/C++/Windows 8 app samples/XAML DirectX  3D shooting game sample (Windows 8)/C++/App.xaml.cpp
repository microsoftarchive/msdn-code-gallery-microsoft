//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "App.xaml.h"

using namespace Simple3DGameXaml;

using namespace concurrency;
using namespace DirectX;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Store;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;


//----------------------------------------------------------------------

App::App():
    m_pauseRequested(false),
    m_pressComplete(false),
    m_renderNeeded(false),
    m_visible(true),
    m_haveFocus(true),
    m_updateState(UpdateEngineState::WaitingForResources)
{
    InitializeComponent();

#if defined(_DEBUG)
    UnhandledException += ref new UnhandledExceptionEventHandler([](Object^ /* sender */, UnhandledExceptionEventArgs^ args)
    {
        Platform::String^ error = "Simple3DGameXaml::App::UnhandledException: " + args->Message + "\n";
        OutputDebugStringW(error->Data());
    });
#endif
}

//----------------------------------------------------------------------

App::~App()
{
    CompositionTarget::Rendering::remove(m_onRenderingEventToken);
}

//----------------------------------------------------------------------

void App::OnLaunched(_In_ LaunchActivatedEventArgs^ /* args */)
{
    Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
    Resuming += ref new EventHandler<Object^>(this, &App::OnResuming);

    m_mainPage = ref new MainPage();
    m_mainPage->SetApp(this);

    Window::Current->Content = m_mainPage;
    Window::Current->Activated += ref new WindowActivatedEventHandler(this, &App::OnWindowActivationChanged);
    Window::Current->Activate();

    m_controller = ref new MoveLookController();
    m_renderer = ref new GameRenderer();
    m_game = ref new Simple3DGame();

    auto window = Window::Current->CoreWindow;

    PointerVisualizationSettings^ visualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    visualizationSettings->IsContactFeedbackEnabled = false;
    visualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &App::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &App::OnVisibilityChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &App::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &App::OnDisplayContentsInvalidated);

    m_controller->Initialize(window);
    m_controller->AutoFire(false);

    m_controller->SetMoveRect(
        XMFLOAT2(0.0f, window->Bounds.Height - GameConstants::TouchRectangleSize),
        XMFLOAT2(GameConstants::TouchRectangleSize, window->Bounds.Height)
        );
    m_controller->SetFireRect(
        XMFLOAT2(window->Bounds.Width - GameConstants::TouchRectangleSize, window->Bounds.Height - GameConstants::TouchRectangleSize),
        XMFLOAT2(window->Bounds.Width, window->Bounds.Height)
        );

    m_renderer->Initialize(window, m_mainPage->GetSwapChainBackgroundPanel(), DisplayProperties::LogicalDpi);
    m_renderer->DeviceLost += ref new D3DDeviceEventHandler(this, &App::OnDeviceLost);
    m_renderer->DeviceReset += ref new D3DDeviceEventHandler(this, &App::OnDeviceReset);

    SetGameInfoOverlay(GameInfoOverlayState::Loading);
    SetAction(GameInfoOverlayCommand::None);
    ShowGameInfoOverlay();

    m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
    m_renderNeeded = true;

    create_task([this]()
    {
        // Asynchronously initialize the game class and load the renderer device resources.
        // By doing all this asynchronously, the game gets to its main loop more quickly
        // and in parallel all the necessary resources are loaded on other threads.
        m_game->Initialize(m_controller, m_renderer);

        return m_renderer->CreateGameDeviceResourcesAsync(m_game);

    }).then([this]()
    {
        // The finalize code needs to run in the same thread context
        // as the m_renderer object was created because the D3D device context
        // can ONLY be accessed on a single thread.
        m_renderer->FinalizeCreateGameDeviceResources();

        InitializeLicense();
        InitializeGameState();

        if (m_updateState == UpdateEngineState::WaitingForResources)
        {
            // In the middle of a game so spin up the async task to load the level.
            create_task([this]()
            {
                return m_game->LoadLevelAsync();

            }).then([this]()
            {
                // The m_game object may need to deal with D3D device context work so
                // again the finalize code needs to run in the same thread
                // context as the m_renderer object was created because the D3D
                // device context can ONLY be accessed on a single thread.
                m_game->FinalizeLoadLevel();
                m_game->SetCurrentLevelToSavedState();
                m_updateState = UpdateEngineState::ResourcesLoaded;

            }, task_continuation_context::use_current());
        }
    }, task_continuation_context::use_current());
}

//----------------------------------------------------------------------

void App::OnRendering(
    _In_ Object^ /* sender */,
    _In_ Object^ /* args */
    )
{
    Update();
    if (m_updateState == UpdateEngineState::Dynamics || m_renderNeeded)
    {
        m_renderer->Render();
        m_renderNeeded = false;
    }

    if (!m_visible || !m_haveFocus || (m_updateState == UpdateEngineState::Snapped))
    {
        // App is in an inactive state so disable the OnRendering callback
        // This optimizes for power and allows the framework to become more queiecent
        if (m_onRenderingEventToken.Value != 0)
        {
            CompositionTarget::Rendering::remove(m_onRenderingEventToken);
            m_onRenderingEventToken.Value = 0;
        }
    }
}

//--------------------------------------------------------------------------------------

void App::OnWindowSizeChanged(
    _In_ CoreWindow^ window,
    _In_ WindowSizeChangedEventArgs^ /* args */
    )
{
    UpdateViewState();
    m_mainPage->OnWindowSizeChanged();
    m_renderer->UpdateForWindowSizeChange();

    // The location of the Control regions may have changed with the size change so update the controller.
    m_controller->SetMoveRect(
        XMFLOAT2(0.0f, window->Bounds.Height - GameConstants::TouchRectangleSize),
        XMFLOAT2(GameConstants::TouchRectangleSize, window->Bounds.Height)
        );
    m_controller->SetFireRect(
        XMFLOAT2(window->Bounds.Width - GameConstants::TouchRectangleSize, window->Bounds.Height - GameConstants::TouchRectangleSize),
        XMFLOAT2(window->Bounds.Width, window->Bounds.Height)
        );

    m_renderNeeded = true;

    if (m_onRenderingEventToken.Value == 0)
    {
        // Add the OnRendering Callback to make sure that an update happens.
        m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
    }
}

//--------------------------------------------------------------------------------------

void App::OnLogicalDpiChanged(
    _In_ Object^ /* sender */
    )
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
}

//--------------------------------------------------------------------------------------

void App::OnDisplayContentsInvalidated(
    _In_ Object^ /* sender */
    )
{
    m_renderNeeded = true;

    if (m_onRenderingEventToken.Value == 0)
    {
        // Add the OnRendering Callback to make sure that an update happens.
        m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
    }
}

//--------------------------------------------------------------------------------------

void App::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_visible = args->Visible;

    if (m_visible && (m_onRenderingEventToken.Value == 0))
    {
        // App is now visible so set up the event handler to do game processing and rendering
        m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
    }
}

//--------------------------------------------------------------------------------------

void App::InitializeGameState()
{
    // Set up the initial state machine for handling Game playing state
    if (m_game->GameActive() && m_game->LevelActive())
    {
        // The last time the game terminated it was in the middle
        // of a level.
        // We are waiting for the user to continue the game.
        m_updateState = UpdateEngineState::WaitingForResources;
        m_pressResult = PressResultState::ContinueLevel;
        SetGameInfoOverlay(GameInfoOverlayState::Pause);
        SetAction(GameInfoOverlayCommand::PleaseWait);
    }
    else if (!m_game->GameActive() && (m_game->HighScore().totalHits > 0))
    {
        // The last time the game terminated the game had been completed.
        // Show the high score.
        // We are waiting for the user to acknowledge the high score and start a new game.
        // The level resources for the first level will be loaded later.
        m_updateState = UpdateEngineState::WaitingForPress;
        m_pressResult = PressResultState::LoadGame;
        SetGameInfoOverlay(GameInfoOverlayState::GameStats);
        m_controller->WaitForPress();
        SetAction(GameInfoOverlayCommand::TapToContinue);
    }
    else
    {
        // This is either the first time the game has run or
        // the last time the game terminated the level was completed.
        // We are waiting for the user to begin the next level.
        m_updateState = UpdateEngineState::WaitingForResources;
        m_pressResult = PressResultState::PlayLevel;
        SetGameInfoOverlay(GameInfoOverlayState::LevelStart);
        SetAction(GameInfoOverlayCommand::PleaseWait);
    }
    ShowGameInfoOverlay();
}

//--------------------------------------------------------------------------------------

void App::Update()
{
    m_controller->Update();

    switch (m_updateState)
    {
    case UpdateEngineState::ResourcesLoaded:
        switch (m_pressResult)
        {
        case PressResultState::LoadGame:
            SetGameInfoOverlay(GameInfoOverlayState::GameStats);
            break;

        case PressResultState::PlayLevel:
            SetGameInfoOverlay(GameInfoOverlayState::LevelStart);
            break;

        case PressResultState::ContinueLevel:
            SetGameInfoOverlay(GameInfoOverlayState::Pause);
            break;
        }
        m_updateState = UpdateEngineState::WaitingForPress;
        SetAction(GameInfoOverlayCommand::TapToContinue);
        m_controller->WaitForPress();
        ShowGameInfoOverlay();
        m_renderNeeded = true;
        break;

    case UpdateEngineState::WaitingForPress:
        if (m_controller->IsPressComplete() || m_pressComplete)
        {
            m_pressComplete = false;

            switch (m_pressResult)
            {
            case PressResultState::LoadGame:
                m_updateState = UpdateEngineState::WaitingForResources;
                m_pressResult = PressResultState::PlayLevel;
                m_controller->Active(false);
                m_game->LoadGame();
                SetAction(GameInfoOverlayCommand::PleaseWait);
                SetGameInfoOverlay(GameInfoOverlayState::LevelStart);
                ShowGameInfoOverlay();
                m_renderNeeded = true;

                m_game->LoadLevelAsync().then([this]()
                {
                    m_game->FinalizeLoadLevel();
                    m_updateState = UpdateEngineState::ResourcesLoaded;
                    m_renderNeeded = true;
                }, task_continuation_context::use_current());
                break;

            case PressResultState::PlayLevel:
                m_updateState = UpdateEngineState::Dynamics;
                HideGameInfoOverlay();
                m_controller->Active(true);
                m_game->StartLevel();
                break;

            case PressResultState::ContinueLevel:
                m_updateState = UpdateEngineState::Dynamics;
                HideGameInfoOverlay();
                m_controller->Active(true);
                m_game->ContinueGame();
                break;
            }
        }
        break;

    case UpdateEngineState::Dynamics:
        if (m_controller->IsPauseRequested() || m_pauseRequested)
        {
            m_pauseRequested = false;

            m_game->PauseGame();
            SetGameInfoOverlay(GameInfoOverlayState::Pause);
            SetAction(GameInfoOverlayCommand::TapToContinue);
            m_updateState = UpdateEngineState::WaitingForPress;
            m_pressResult = PressResultState::ContinueLevel;
            ShowGameInfoOverlay();
            m_renderNeeded = true;
        }
        else
        {
            GameState runState = m_game->RunGame();
            switch (runState)
            {
            case GameState::TimeExpired:
                SetAction(GameInfoOverlayCommand::TapToContinue);
                SetGameInfoOverlay(GameInfoOverlayState::GameOverExpired);
                ShowGameInfoOverlay();
                m_updateState = UpdateEngineState::WaitingForPress;
                m_pressResult = PressResultState::LoadGame;
                m_renderNeeded = true;
                break;

            case GameState::LevelComplete:
                SetAction(GameInfoOverlayCommand::PleaseWait);
                SetGameInfoOverlay(GameInfoOverlayState::LevelStart);
                ShowGameInfoOverlay();
                m_updateState = UpdateEngineState::WaitingForResources;
                m_pressResult = PressResultState::PlayLevel;
                m_renderNeeded = true;

                m_game->LoadLevelAsync().then([this]()
                {
                    m_game->FinalizeLoadLevel();
                    m_updateState = UpdateEngineState::ResourcesLoaded;
                    m_renderNeeded = true;

                }, task_continuation_context::use_current());
                break;

            case GameState::GameComplete:
                SetAction(GameInfoOverlayCommand::TapToContinue);
                SetGameInfoOverlay(GameInfoOverlayState::GameOverCompleted);
                ShowGameInfoOverlay();
                m_updateState  = UpdateEngineState::WaitingForPress;
                m_pressResult = PressResultState::LoadGame;
                m_renderNeeded = true;
                break;
            }
        }

        if (m_updateState == UpdateEngineState::WaitingForPress)
        {
            // Transitioning state, so enable waiting for the press event
            m_controller->WaitForPress();
        }
        if (m_updateState == UpdateEngineState::WaitingForResources)
        {
            // Transitioning state, so shut down the input controller until resources are loaded
            m_controller->Active(false);
        }
        break;
    }
}

//--------------------------------------------------------------------------------------

void App::OnWindowActivationChanged(
    _In_ Platform::Object^ /* sender */,
    _In_ Windows::UI::Core::WindowActivatedEventArgs^ args
    )
{
    if (args->WindowActivationState == CoreWindowActivationState::Deactivated)
    {
        m_haveFocus = false;

        switch (m_updateState)
        {
        case UpdateEngineState::Dynamics:
            // From Dynamic mode, when coming out of Deactivated rather than going directly back into game play
            // go to the paused state waiting for user input to continue
            m_updateStateNext = UpdateEngineState::WaitingForPress;
            m_pressResult = PressResultState::ContinueLevel;
            SetGameInfoOverlay(GameInfoOverlayState::Pause);
            ShowGameInfoOverlay();
            m_game->PauseGame();
            m_updateState = UpdateEngineState::Deactivated;
            SetAction(GameInfoOverlayCommand::None);
            m_renderNeeded = true;
            break;

        case UpdateEngineState::WaitingForResources:
        case UpdateEngineState::WaitingForPress:
            m_updateStateNext = m_updateState;
            m_updateState = UpdateEngineState::Deactivated;
            SetAction(GameInfoOverlayCommand::None);
            ShowGameInfoOverlay();
            m_renderNeeded = true;
            break;
        }
        // Don't have focus so shutdown input processing
        m_controller->Active(false);
    }
    else if (args->WindowActivationState == CoreWindowActivationState::CodeActivated
        || args->WindowActivationState == CoreWindowActivationState::PointerActivated)
    {
        m_haveFocus = true;

        if (m_updateState == UpdateEngineState::Deactivated)
        {
            m_updateState = m_updateStateNext;

            if (m_updateState == UpdateEngineState::WaitingForPress)
            {
                SetAction(GameInfoOverlayCommand::TapToContinue);
                m_controller->WaitForPress();
            }
            else if (m_updateStateNext == UpdateEngineState::WaitingForResources)
            {
                SetAction(GameInfoOverlayCommand::PleaseWait);
            }

            // App is now "active" so set up the event handler to do game processing and rendering
            if (m_onRenderingEventToken.Value == 0)
            {
                m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
            }
        }
    }
}

//--------------------------------------------------------------------------------------

void App::OnSuspending(
    _In_ Platform::Object^ /* sender */,
    _In_ SuspendingEventArgs^ args
    )
{
    // Save application state.
    switch (m_updateState)
    {
    case UpdateEngineState::Dynamics:
        // Game is in the active game play state, Stop Game Timer and Pause play and save state
        SetAction(GameInfoOverlayCommand::None);
        SetGameInfoOverlay(GameInfoOverlayState::Pause);
        m_updateStateNext = UpdateEngineState::WaitingForPress;
        m_pressResult = PressResultState::ContinueLevel;
        m_game->PauseGame();
        break;

    case UpdateEngineState::WaitingForResources:
    case UpdateEngineState::WaitingForPress:
        m_updateStateNext = m_updateState;
        break;

    default:
        // any other state don't save as next state as they are trancient states and have already set m_updateStateNext
        break;
    }
    m_updateState = UpdateEngineState::Suspended;

    m_controller->Active(false);
    m_game->OnSuspending();
}

//--------------------------------------------------------------------------------------

void App::OnResuming(
    _In_ Platform::Object^ /* sender */,
    _In_ Platform::Object^ /* args */
    )
{
    if (m_haveFocus)
    {
        m_updateState = m_updateStateNext;
    }
    else
    {
        m_updateState = UpdateEngineState::Deactivated;
    }

    if (m_updateState == UpdateEngineState::WaitingForPress)
    {
        SetAction(GameInfoOverlayCommand::TapToContinue);
        m_controller->WaitForPress();
    }
    m_game->OnResuming();
    ShowGameInfoOverlay();
    m_renderNeeded = true;
}

//--------------------------------------------------------------------------------------

void App::OnDeviceLost()
{
    if (m_updateState == UpdateEngineState::Dynamics)
    {
        // Game is in the active game play state, Stop Game Timer and Pause play and save state
        m_game->PauseGame();
    }
    m_renderNeeded = false;
}

//--------------------------------------------------------------------------------------

void App::OnDeviceReset()
{

    SetAction(GameInfoOverlayCommand::PleaseWait);
    SetGameInfoOverlay(GameInfoOverlayState::Loading);
    m_updateState = UpdateEngineState::WaitingForResources;
    m_renderNeeded = true;

    if (m_onRenderingEventToken.Value == 0)
    {
        // Add the OnRendering Callback to make sure that an update happens.
        m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
    }

    create_task([this]()
    {
        return m_renderer->CreateGameDeviceResourcesAsync(m_game);

    }).then([this]()
    {
        // The finalize code needs to run in the same thread context
        // as the m_renderer object was created because the D3D device context
        // can ONLY be accessed on a single thread.
        m_renderer->FinalizeCreateGameDeviceResources();

        // Reset the main state machine based on the current game state now
        // that the device resources have been restored.
        InitializeGameState();

        if (m_updateState == UpdateEngineState::WaitingForResources)
        {
            // In the middle of a game so spin up the async task to load the level.
            return m_game->LoadLevelAsync().then([this]()
            {
                // The m_game object may need to deal with D3D device context work so
                // again the finalize code needs to run in the same thread
                // context as the m_renderer object was created because the D3D
                // device context can ONLY be accessed on a single thread.
                m_game->FinalizeLoadLevel();
                m_game->SetCurrentLevelToSavedState();
                m_updateState = UpdateEngineState::ResourcesLoaded;

            }, task_continuation_context::use_current());
        }
        else
        {
            // The game is not in the middle of a level so there aren't any level
            // resources to load.  Creating a no-op task so that in both cases
            // the same continuation logic is used.
            return create_task([]()
            {
            });
        }

    }, task_continuation_context::use_current()).then([this]()
    {
        // Since Device lost is an unexpected event, the app visual state
        // may be snapped or not have focus.  Put the state machine
        // into the correct state to reflect these cases.
        if (ApplicationView::Value == ApplicationViewState::Snapped)
        {
            m_updateStateNext = m_updateState;
            m_updateState = UpdateEngineState::Snapped;
            m_controller->Active(false);
            HideGameInfoOverlay();
            SetSnapped();
            m_renderNeeded = true;
        }
        else if (!m_haveFocus)
        {
            m_updateStateNext = m_updateState;
            m_updateState = UpdateEngineState::Deactivated;
            m_controller->Active(false);
            SetAction(GameInfoOverlayCommand::None);
            m_renderNeeded = true;
        }

        if (m_onRenderingEventToken.Value == 0)
        {
            // Add the OnRendering Callback to make sure that an update happens.
            m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
        }

    }, task_continuation_context::use_current());
}

//--------------------------------------------------------------------------------------

void App::UpdateViewState()
{
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        switch (m_updateState)
        {
        case UpdateEngineState::Dynamics:
            // From Dynamic mode, when coming out of SNAPPED layout rather than going directly back into game play
            // go to the paused state waiting for user input to continue
            m_updateStateNext = UpdateEngineState::WaitingForPress;
            m_pressResult = PressResultState::ContinueLevel;
            SetGameInfoOverlay(GameInfoOverlayState::Pause);
            SetAction(GameInfoOverlayCommand::TapToContinue);
            m_game->PauseGame();
            break;

        case UpdateEngineState::WaitingForResources:
        case UpdateEngineState::WaitingForPress:
            // Avoid corrupting the m_updateStateNext on a transition from Snapped -> Snapped.
            // Otherwise just cache the current state and return to it when leaving SNAPPED layout

            m_updateStateNext = m_updateState;
            break;

        default:
            break;
        }

        m_updateState = UpdateEngineState::Snapped;
        m_controller->Active(false);
        HideGameInfoOverlay();
        SetSnapped();
        m_renderNeeded = true;
    }
    else if (ApplicationView::Value == ApplicationViewState::Filled ||
        ApplicationView::Value == ApplicationViewState::FullScreenLandscape ||
        ApplicationView::Value == ApplicationViewState::FullScreenPortrait)
    {
        if (m_updateState == UpdateEngineState::Snapped)
        {
            HideSnapped();
            ShowGameInfoOverlay();
            m_renderNeeded = true;

            if (m_haveFocus)
            {
                if (m_updateStateNext == UpdateEngineState::WaitingForPress)
                {
                    SetAction(GameInfoOverlayCommand::TapToContinue);
                    m_controller->WaitForPress();
                }
                else if (m_updateStateNext == UpdateEngineState::WaitingForResources)
                {
                    SetAction(GameInfoOverlayCommand::PleaseWait);
                }

                m_updateState = m_updateStateNext;
            }
            else
            {
                m_updateState = UpdateEngineState::Deactivated;
                SetAction(GameInfoOverlayCommand::None);
            }
        }
        else if (m_updateState == UpdateEngineState::Dynamics)
        {
            // In Dynamic mode, when a resize event happens, go to the paused state waiting for user input to continue.
            m_pressResult = PressResultState::ContinueLevel;
            SetGameInfoOverlay(GameInfoOverlayState::Pause);
            m_game->PauseGame();
            if (m_haveFocus)
            {
                m_updateState = UpdateEngineState::WaitingForPress;
                SetAction(GameInfoOverlayCommand::TapToContinue);
                m_controller->WaitForPress();
            }
            else
            {
                m_updateState = UpdateEngineState::Deactivated;
                SetAction(GameInfoOverlayCommand::None);
            }
            ShowGameInfoOverlay();
            m_renderNeeded = true;
        }

        if (m_haveFocus && m_onRenderingEventToken.Value == 0)
        {
            // App is now "active" so set up the event handler to do game processing and rendering
            m_onRenderingEventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &App::OnRendering));
        }
    }
}

//--------------------------------------------------------------------------------------

void App::SetGameInfoOverlay(GameInfoOverlayState state)
{
    m_gameInfoOverlayState = state;
    switch (state)
    {
    case GameInfoOverlayState::Loading:
        m_mainPage->SetGameLoading();
        break;

    case GameInfoOverlayState::GameStats:
        m_mainPage->SetGameStats(
            m_game->HighScore().levelCompleted + 1,
            m_game->HighScore().totalHits,
            m_game->HighScore().totalShots
            );
        break;

    case GameInfoOverlayState::LevelStart:
        m_mainPage->SetLevelStart(
            m_game->LevelCompleted() + 1,
            m_game->CurrentLevel()->Objective(),
            m_game->CurrentLevel()->TimeLimit(),
            m_game->BonusTime()
            );
        break;

    case GameInfoOverlayState::GameOverCompleted:
        m_mainPage->SetGameOver(
            true,
            m_game->LevelCompleted() + 1,
            m_game->TotalHits(),
            m_game->TotalShots(),
            m_game->HighScore().totalHits
            );
        break;

    case GameInfoOverlayState::GameOverExpired:
        m_mainPage->SetGameOver(
            false,
            m_game->LevelCompleted(),
            m_game->TotalHits(),
            m_game->TotalShots(),
            m_game->HighScore().totalHits
            );
        break;

    case GameInfoOverlayState::Pause:
        m_mainPage->SetPause(
            m_game->LevelCompleted() + 1,
            m_game->TotalHits(),
            m_game->TotalShots(),
            m_game->TimeRemaining()
            );
        break;
    }
}

//--------------------------------------------------------------------------------------

void App::SetAction(GameInfoOverlayCommand command)
{
    m_mainPage->SetAction(command);
}

//--------------------------------------------------------------------------------------

void App::ShowGameInfoOverlay()
{
    m_mainPage->ShowGameInfoOverlay();
}

//--------------------------------------------------------------------------------------

void App::HideGameInfoOverlay()
{
    m_mainPage->HideGameInfoOverlay();
}

//--------------------------------------------------------------------------------------

void App::SetSnapped()
{
    m_mainPage->SetSnapped();
}

//--------------------------------------------------------------------------------------

void App::HideSnapped()
{
    m_mainPage->HideSnapped();
}

//--------------------------------------------------------------------------------------

void App::ResetGame()
{
    m_updateState = UpdateEngineState::WaitingForResources;
    m_pressResult = PressResultState::PlayLevel;
    m_controller->Active(false);
    m_game->LoadGame();
    SetAction(GameInfoOverlayCommand::PleaseWait);
    SetGameInfoOverlay(GameInfoOverlayState::LevelStart);
    ShowGameInfoOverlay();
    m_renderNeeded = true;

    m_game->LoadLevelAsync().then([this]()
    {
        m_game->FinalizeLoadLevel();
        m_updateState = UpdateEngineState::ResourcesLoaded;
        m_renderNeeded = true;

    }, task_continuation_context::use_current());
}

//--------------------------------------------------------------------------------------

void App::InitializeLicense()
{
#ifdef USE_STORE_SIMULATOR
    m_licenseState = ref new PersistentState();
    m_licenseState->Initialize(ApplicationData::Current->LocalSettings->Values, "CurrentAppSimulator");
    m_isTrial = m_licenseState->LoadBool(":isTrial", true);

    Platform::String^ license;
    if (this->m_isTrial)
    {
        license = "TrialLicense.xml";
    }
    else
    {
        license = "FullLicense.xml";
    }
    task<StorageFile^> fileTask(Package::Current->InstalledLocation->GetFileAsync(license));
    fileTask.then([=](StorageFile^ sourceFile)
    {
        return create_task(CurrentAppSimulator::ReloadSimulatorAsync(sourceFile));
    }).then([this]()
    {
            this->InitializeLicenseCore();
    });
#else
    this->InitializeLicenseCore();
#endif
}

//--------------------------------------------------------------------------------------

void App::InitializeLicenseCore()
{
#ifdef USE_STORE_SIMULATOR
    this->m_licenseInformation = CurrentAppSimulator::LicenseInformation;
    task<ListingInformation^> listingTask(CurrentAppSimulator::LoadListingInformationAsync());
#else
    m_licenseInformation = CurrentApp::LicenseInformation;
    task<ListingInformation^> listingTask(CurrentApp::LoadListingInformationAsync());
#endif

    this->m_licenseInformation->LicenseChanged += ref new LicenseChangedEventHandler(this, &App::OnLicenseChanged);
    this->m_mainPage->SetProductItems(nullptr, m_licenseInformation);
    this->OnLicenseChanged();

    listingTask.then([=](ListingInformation^ listing)
    {
        this->m_listingInformation = listing;
        this->OnLicenseChanged();
    });
}

//--------------------------------------------------------------------------------------

#ifdef USE_STORE_SIMULATOR
void App::ResetLicense()
{
    task<StorageFile^> fileTask(Package::Current->InstalledLocation->GetFileAsync("TrialLicense.xml"));
    fileTask.then([=](StorageFile^ sourceFile)
    {
        CurrentAppSimulator::ReloadSimulatorAsync(sourceFile);
    });
}
#endif

//--------------------------------------------------------------------------------------

void App::OnLicenseChanged()
{
#ifdef USE_STORE_SIMULATOR
    m_isTrial = (m_licenseInformation->IsActive && m_licenseInformation->IsTrial);
    m_licenseState->SaveBool(":isTrial", m_isTrial);
#endif
    m_mainPage->LicenseChanged(m_listingInformation, m_licenseInformation);
    m_game->UpdateGameConfig(m_licenseInformation);

    if (m_licenseInformation->IsActive)
    {
        if (m_licenseInformation->IsTrial)
        {
            m_renderer->Hud()->SetLicenseInfo("Trial Version");
        }
        else
        {
            m_renderer->Hud()->SetLicenseInfo("Full Version");
        }
    }
    else
    {
        m_renderer->Hud()->SetLicenseInfo("License Inactive - defaulting to Trial Version");
    }

    m_renderNeeded = true;
}

//--------------------------------------------------------------------------------------

void App::SetBackground(unsigned int background)
{
    m_game->SetBackground(background);
    m_renderNeeded = true;
}

//--------------------------------------------------------------------------------------

void App::CycleBackground()
{
    m_game->CycleBackground();
    m_renderNeeded = true;
}

//--------------------------------------------------------------------------------------

