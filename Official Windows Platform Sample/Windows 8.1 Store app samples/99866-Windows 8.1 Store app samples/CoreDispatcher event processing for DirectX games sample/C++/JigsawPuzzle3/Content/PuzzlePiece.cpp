//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Content\PuzzlePiece.h"

using namespace JigsawPuzzle;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;

PuzzlePiece::PuzzlePiece(Point location, PuzzlePieceId id) :
    m_bounds(location.X, location.Y, Constants::PuzzlePieceSize, Constants::PuzzlePieceSize),
    m_pickOffset(0.0f, 0.0f),
    m_id(id),
    m_selected(false),
    m_moving(false),
    m_snapped(false),
    m_destination(0.0f, 0.0f),
    m_animationStep(0.0f, 0.0f)
{
}

bool PuzzlePiece::Pick(Point location)
{
    m_selected = m_bounds.Contains(location);
    if (m_selected)
    {
        m_pickOffset = Point(location.X - m_bounds.X, location.Y - m_bounds.Y);
        Drag(location);
    }
    return m_selected;
}

void PuzzlePiece::Unpick()
{
    m_selected = false;
}

void PuzzlePiece::Drag(Point location)
{
    m_bounds.X = location.X;
    m_bounds.Y = location.Y;
    m_moving = true;
    m_snapped = false;
}

void PuzzlePiece::Drop(Point location)
{
    Drag(Point(location.X - m_pickOffset.X, location.Y - m_pickOffset.Y));
    Drop();
}

void PuzzlePiece::DropAndSnapTo(Point location)
{
    Drag(location);
    Drop();
    Unpick();
    m_snapped = true;
}

void PuzzlePiece::Drop()
{
    m_moving = false;
    m_pickOffset = Point(0.0f, 0.0f);
}

// Animation support for puzzle pieces.

void PuzzlePiece::AnimateTo(Point location)
{
    m_destination = location;

    Point offset = Point(m_destination.X - m_bounds.X, m_destination.Y - m_bounds.Y);
    m_animationStep = Point(offset.X / Constants::PuzzlePieceAnimationFrames, offset.Y / Constants::PuzzlePieceAnimationFrames);
}

bool PuzzlePiece::Update()
{
    if (m_bounds.X == m_destination.X && m_bounds.Y == m_destination.Y)
    {
        m_animationStep = Point(0.0f, 0.0f);
        m_destination = Point(0.0f, 0.0f);
        return false;
    }

    m_bounds.X += m_animationStep.X;
    m_bounds.Y += m_animationStep.Y;

    if (abs(m_bounds.X - m_destination.X) < 0.01f && abs(m_bounds.Y - m_destination.Y) < 0.01f)
    {
        DropAndSnapTo(m_destination);
    }

    return true;
}
