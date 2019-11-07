//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// DirectXApp:
// This class implements IFrameworkView, the main class required for Metro style apps.
// It controls the main event processing loop and maintains the overall state of the game.
// The DirectXApp class drives and maintains a state machine for the game.  It can be in
// one of seven major states defined by the UpdateEngineState enum class.
// These are:
//     WaitingForResources - the game has requested the game object or the renderer object
//         to load resources asynchronously.
//     ResourcesLoaded - the asynchronous loading of resources has been completed.  This
//         is a transient state.
//     WaitingForPress - the game is waiting for the player to indicate they are ready to proceed.
//         There are three possible actions from this state.  This is controlled by m_pressResult.
//         The possible outcomes are:
//             LoadGame - The player is ready to start a new game and has acknowledged the status
//                 information provided about the previous state.
//             PlayLevel - The player is ready to play the next level.  The level has already been
//                 loaded so active game play will start.
//             ContinueLevel - The player is ready to continue playing the current level.  Part of the
//                 current level has already been played.
//     Dynamics - the game is active play mode.
//     Snapped - the game is currently in the snapped position on the screen.  It is not possible to
//         play the game in snapped mode.
//     Suspended - the game was suspended by PLM.
//     Deactivated - the game has lost focus.
//
// DirectXApp creates and maintains references to three major objects used for the game:
//     MoveLookController (m_controller) - this object handles all the game specific user input and
//         aggregates touch, mouse/keyboard and Xbox controller input into a unified input control.
//     Simple3DGame (m_game) - this object handles all the game specific logic and game dynamics.
//     GameRenderer (m_renderer) - This object handles all the graphics rendering for the game.
//
// DirectXApp registers for all the necessary Metro style events to maintain and control all state transitions.

#include "Simple3DGame.h"

enum class UpdateEngineState
{
    WaitingForResources,
    ResourcesLoaded,
    WaitingForPress,
    Dynamics,
    Snapped,
    Suspended,
    Deactivated,
};

enum class PressResultState
{
    LoadGame,
    PlayLevel,
    ContinueLevel,
};

enum class GameInfoOverlayState
{
    Loading,
    GameStats,
    GameOverExpired,
    GameOverCompleted,
    LevelStart,
    Pause,
};

ref class DirectXApp : public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    DirectXApp();

public:
    // IFrameworkView Methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    void InitializeGameState();
    void OnDeviceLost();
    void OnDeviceReset();

    // Event Handlers
    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
        );

    void OnResuming(
        _In_ Platform::Object^ sender,
        _In_ Platform::Object^ args
        );

    void UpdateViewState();

    void OnWindowActivationChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowActivatedEventArgs^ args
        );

    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
        );

    void OnLogicalDpiChanged(
        _In_ Platform::Object^ sender
        );

    void OnDisplayContentsInvalidated(
        _In_ Platform::Object^ sender
        );

    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView,
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args
        );

    void OnVisibilityChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::VisibilityChangedEventArgs^ args
        );

    void Update();
    void SetGameInfoOverlay(GameInfoOverlayState state);
    void SetAction (GameInfoOverlayCommand command);
    void ShowGameInfoOverlay();
    void HideGameInfoOverlay();
    void SetSnapped();
    void HideSnapped();

    bool                                                m_windowClosed;
    bool                                                m_renderNeeded;
    bool                                                m_haveFocus;
    bool                                                m_visible;

    MoveLookController^                                 m_controller;
    GameRenderer^                                       m_renderer;
    Simple3DGame^                                       m_game;

    UpdateEngineState                                   m_updateState;
    UpdateEngineState                                   m_updateStateNext;
    PressResultState                                    m_pressResult;
    GameInfoOverlayState                                m_gameInfoOverlayState;
    GameInfoOverlayCommand                              m_gameInfoOverlayCommand;
    uint32                                              m_loadingCount;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};

