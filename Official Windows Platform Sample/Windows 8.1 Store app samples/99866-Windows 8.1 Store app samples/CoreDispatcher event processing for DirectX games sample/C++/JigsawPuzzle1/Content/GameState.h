//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

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

        // Notifies the app that the game state has changed and needs to be
        // re-rendered to reflect that change to the user.
        bool StateChanged()
        {
            return m_stateChanged;
        }

        // Allows the render loop to signal that it has rendered the pending
        // state changes and will wait for another change before rendering again.
        void Validate()
        {
            m_stateChanged = false;
        }

#ifdef MEASURE_LATENCY
        // The easiest way to measure input latency with a high speed camera is to draw
        // a large area with a different color.  These APIs allow the game renderer to
        // know when to change the background color for the next frame.

        void ToggleBackgroundColor()
        {
            m_stateChanged = true;
            m_toggleBackground = !m_toggleBackground;
        }

        bool BackgroundToggled()
        {
            return m_toggleBackground;
        }
#endif

    private:
        std::shared_ptr<PuzzlePiece> m_pieces[Constants::PuzzlePieceCount];
        std::shared_ptr<DropTarget> m_dropTargets[Constants::PuzzlePieceCount];
        std::shared_ptr<PuzzlePiece> m_selectedPiece;
        bool m_dragging;
        bool m_stateChanged;

#ifdef MEASURE_LATENCY
        bool m_toggleBackground;
#endif
    };
}
