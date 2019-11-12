//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// GameInfoOverlay:
// This class maintains a D2D1 Bitmap surface to provide information to the player
// on the state of the game when the game is not in active play.
// This class has several different status modes:
//     GameLoading - this displays a title and a series of dots to show progress.
//     GameStats - this displays a title and the Max level achieved, the hit and shot counts.
//     GameOver - this displays a title and the game status.
//     Pause - this displays only a title - 'Paused'.
// In addition to the status modes, there is a region at the bottom that displays what the
// player is expected to do next.  This can be one of four things:
//     None - the action is blank.  This is usually set when the game does not currently have focus.
//     TapToContinue - the game is waiting for the player to provide input that they are ready to
//         proceed.
//     PleaseWait - the game is actively doing some background processing (like loading a level).
//     PlayAgain - the game has completed and is waiting for the player to indicate they are ready
//         to play another round of the game.

namespace GameInfoOverlayConstant
{
    static const float Width    = 750.0f;
    static const float Height   = 380.0f;
};

enum class GameInfoOverlayCommand
{
    None,
    TapToContinue,
    PleaseWait,
    PlayAgain,
};

ref class GameInfoOverlay
{
internal:
    GameInfoOverlay();

    void CreateDeviceIndependentResources(_In_ IDWriteFactory* dwriteFactory);
    void CreateDeviceResources(_In_ ID2D1DeviceContext*  d2dContext);
    void CreateDpiDependentResources(float dpi);

    void SetGameLoading(uint32 dots);
    void SetGameStats(int maxLevel, int hitCount, int shotCount);
    void SetGameOver(bool win, int maxLevel, int hitCount, int shotCount, int highScore);
    void SetLevelStart(int level, Platform::String^ objective, float timeLimit, float bonusTime);
    void SetPause();
    void SetAction(GameInfoOverlayCommand action);
    void HideGameInfoOverlay() { m_visible = false; };
    void ShowGameInfoOverlay() { m_visible = true; };
    bool Visible() { return m_visible; };
    ID2D1Bitmap1* Bitmap() { return m_levelBitmap.Get(); }

private:
    float                                           m_dpi;
    bool                                            m_visible;

    Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
    Microsoft::WRL::ComPtr<IDWriteFactory>          m_dwriteFactory;

    Microsoft::WRL::ComPtr<ID2D1Bitmap1>            m_levelBitmap;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_textFormatTitle;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_textFormatBody;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_textBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_backgroundBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_actionBrush;
};
