//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Content\PuzzlePiece.h"

namespace JigsawPuzzle
{
    class DropTarget
    {
    public:
        DropTarget(Windows::Foundation::Point location, PuzzlePieceId id);

        void Drag(PuzzlePiece* piece, Windows::Foundation::Point location);
        bool Drop(PuzzlePiece* piece, Windows::Foundation::Point location);

        // 4. Take a snapshot of the current state for the render thread to consume.
        void SnapState()
        {
            m_snappedState.position = Windows::Foundation::Point(m_bounds.X, m_bounds.Y);
            m_snappedState.hovering = m_hoverState;
        }

        inline Windows::Foundation::Point GetPosition()
        {
            return m_snappedState.position;
        }

        inline PuzzlePieceId GetId()
        {
            return m_id;
        }

        inline bool IsHovering()
        {
            return m_snappedState.hovering;
        }

    private:
        Windows::Foundation::Rect m_bounds;
        PuzzlePieceId m_id;
        bool m_hoverState;

        // 4. Support for multi-threaded access to game state.
        struct
        {
            Windows::Foundation::Point position;
            bool hovering;
        } m_snappedState;
    };
}
