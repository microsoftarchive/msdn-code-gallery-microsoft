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

        inline Windows::Foundation::Point GetPosition()
        {
            return Windows::Foundation::Point(m_bounds.X, m_bounds.Y);
        }

        inline PuzzlePieceId GetId()
        {
            return m_id;
        }

        inline bool IsHovering()
        {
            return m_hoverState;
        }

    private:
        Windows::Foundation::Rect m_bounds;
        PuzzlePieceId m_id;
        bool m_hoverState;
    };
}
