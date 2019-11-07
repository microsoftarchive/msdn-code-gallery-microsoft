//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// App:
// This class is the main App class required for XAML Metro style apps.
// It it called on Launch activation and maintains the overall state of the game.
// The App class drives and maintains a state machine for the game.  It can be in
// one of seven major stated defined by the UpdateEngineState enum class.
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
// App creates and maintains references to three major objects used for the game:
//     MoveLookController (m_controller) - this object handles all the game specific user input and
//         aggregates touch, mouse/keyboard and Xbox controller input into a unified input control.
//     Simple3DGame (m_game) - this object handles all the game specific logic and game dynamics.
//     GameRenderer (m_renderer) - This object handles all the graphics rendering for the game.
//
// App registers for all the necessary Metro style events to maintain and control all state transitions.
// The App class dynamically registers and unregisters for the CompositionTarget::Rendering event based on whether
// rendering is needed or not.  This event will occur on each vertical retrace to give a steady set of frames for
// smooth scene rendering.

#if defined(_DEBUG)
#define DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION 1
#endif

#define USE_STORE_SIMULATOR 1

#include "MainPage.xaml.h"
#include "Simple3DGame.h"
#ifdef USE_STORE_SIMULATOR
#include "PersistentState.h"
#endif
#include "App.g.h"

namespace Simple3DGameXaml
{
    private enum class UpdateEngineState
    {
        WaitingForResources,
        ResourcesLoaded,
        WaitingForPress,
        Dynamics,
        Snapped,
        Suspended,
        Deactivated,
    };

    private enum class PressResultState
    {
        LoadGame,
        PlayLevel,
        ContinueLevel,
    };

    private enum class GameInfoOverlayState
    {
        Loading,
        GameStats,
        GameOverExpired,
        GameOverCompleted,
        LevelStart,
        Pause,
        Snapped,
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class App sealed
    {
    public:
        App();

        virtual void OnLaunched(
            _In_ Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ args
            ) override;

        void PauseRequested() { if (m_updateState == UpdateEngineState::Dynamics) m_pauseRequested = true; };
        void PressComplete()  { if (m_updateState == UpdateEngineState::WaitingForPress) m_pressComplete = true; };
        void ResetGame();
        void SetBackground(unsigned int background);
        void CycleBackground();

#ifdef USE_STORE_SIMULATOR
        void ResetLicense();
#endif

    private:
        ~App();

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
            _In_ Platform::Object^ sender,
            _In_ Windows::UI::Core::WindowActivatedEventArgs^ args
            );

        void OnWindowSizeChanged(
            _In_ Windows::UI::Core::CoreWindow^ sender,
            _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
            );

        void OnLogicalDpiChanged(
            _In_ Platform::Object^ sender
            );

        void OnDisplayContentsInvalidated(
            _In_ Platform::Object^ sender
            );

        void OnVisibilityChanged(
            _In_ Windows::UI::Core::CoreWindow^ sender,
            _In_ Windows::UI::Core::VisibilityChangedEventArgs^ args
            );

        void OnRendering(
            _In_ Object^ sender,
            _In_ Object^ args
            );

        void OnLicenseChanged();
        void InitializeLicense();
        void InitializeLicenseCore();

        void InitializeGameState();
        void OnDeviceLost();
        void OnDeviceReset();
        void Update();
        void SetGameInfoOverlay(GameInfoOverlayState state);
        void SetAction (GameInfoOverlayCommand command);
        void ShowGameInfoOverlay();
        void HideGameInfoOverlay();
        void SetSnapped();
        void HideSnapped();

        Windows::Foundation::EventRegistrationToken         m_onRenderingEventToken;
        bool                                                m_pauseRequested;
        bool                                                m_pressComplete;
        bool                                                m_renderNeeded;
        bool                                                m_haveFocus;
        bool                                                m_visible;

        MainPage^                                           m_mainPage;
        MoveLookController^                                 m_controller;
        GameRenderer^                                       m_renderer;
        Simple3DGame^                                       m_game;

        UpdateEngineState                                   m_updateState;
        UpdateEngineState                                   m_updateStateNext;
        PressResultState                                    m_pressResult;
        GameInfoOverlayState                                m_gameInfoOverlayState;
        Windows::ApplicationModel::Store::LicenseInformation^ m_licenseInformation;
        Windows::ApplicationModel::Store::ListingInformation^ m_listingInformation;
#ifdef USE_STORE_SIMULATOR
        PersistentState^                                    m_licenseState;
        bool                                                m_isTrial;
#endif
    };
}


