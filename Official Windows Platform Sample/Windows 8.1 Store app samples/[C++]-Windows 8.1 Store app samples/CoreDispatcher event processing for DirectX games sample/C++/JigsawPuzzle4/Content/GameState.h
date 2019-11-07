//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Common\StepTimer.h"
#include "Content\DropTarget.h"
#include "Content\PuzzlePiece.h"

namespace JigsawPuzzle
{
    class GameState
    {
    public:
        GameState();

        void OnPointerPressed(Windows::Foundation::Point location);
        void OnPointerReleased(Windows::Foundation::Point location);
        void OnPointerMoved(Windows::Foundation::Point location);

        // 4. Take a snapshot of the current state for the render thread to consume.
        // This prevents the render thread from consuming state that is out of sync and
        // removes the need to block the input thread while rendering.
        void SnapState();

        std::shared_ptr<PuzzlePiece> GetPuzzlePiece(UINT index)
        {
            if (index < Constants::PuzzlePieceCount)
            {
                return m_pieces[index];
            }
            return nullptr;
        }

        std::shared_ptr<DropTarget> GetDropTarget(UINT index)
        {
            if (index < Constants::PuzzlePieceCount)
            {
                return m_dropTargets[index];
            }
            return nullptr;
        }

        // Report the total time elapsed since the game began.
        double GetTotalTime()
        {
            return m_snappedState.totalTime;
        }

        // Report the state of the puzzle.
        bool IsPuzzleSolved()
        {
            return m_snappedState.solved;
        }

#ifdef MEASURE_LATENCY
        // The easiest way to measure input latency with a high speed camera is to draw
        // a large area with a different color.  These APIs allow the game renderer to
        // know when to change the background color for subsequent frames.

        void ToggleBackgroundColor()
        {
            m_toggleBackground = !m_toggleBackground;
        }

        bool BackgroundToggled()
        {
            return m_toggleBackground;
        }
#endif

    private:
        // Update the state of the game.
        void Update();

        // Check to see if the puzzle has been solved.
        void CheckSolutionStatus();

        std::shared_ptr<PuzzlePiece> m_pieces[Constants::PuzzlePieceCount];
        std::shared_ptr<DropTarget> m_dropTargets[Constants::PuzzlePieceCount];
        std::shared_ptr<PuzzlePiece> m_selectedPiece;
        bool m_dragging;
        bool m_animating;

        // Keep track of the total time elapsed while the game is in play.
        DX::StepTimer m_timer;
        bool m_solved;

        // 4. Support for multi-threaded access to game state.
        Concurrency::critical_section m_criticalSection;
        struct
        {
            double totalTime;
            bool solved;
        } m_snappedState;

#ifdef MEASURE_LATENCY
        bool m_toggleBackground;
#endif
    };
}
