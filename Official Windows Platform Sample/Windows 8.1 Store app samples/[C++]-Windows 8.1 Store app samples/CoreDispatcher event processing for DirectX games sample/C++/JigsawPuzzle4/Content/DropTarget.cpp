//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Content\DropTarget.h"

using namespace JigsawPuzzle;
using namespace Windows::Foundation;

DropTarget::DropTarget(Point location, PuzzlePieceId id) :
    m_bounds(location.X, location.Y, Constants::PuzzlePieceSize, Constants::PuzzlePieceSize),
    m_id(id),
    m_hoverState(false)
{
    m_snappedState.position = Point(0.0f, 0.0f);
    m_snappedState.hovering = false;
}

void DropTarget::Drag(PuzzlePiece* piece, Point location)
{
    m_hoverState = (piece->GetId() == m_id) && m_bounds.Contains(location);
}

bool DropTarget::Drop(PuzzlePiece* piece, Point location)
{
    if ((piece->GetId() == m_id) && m_bounds.Contains(location))
    {
        m_hoverState = false;
        return true;
    }
    return false;
}
