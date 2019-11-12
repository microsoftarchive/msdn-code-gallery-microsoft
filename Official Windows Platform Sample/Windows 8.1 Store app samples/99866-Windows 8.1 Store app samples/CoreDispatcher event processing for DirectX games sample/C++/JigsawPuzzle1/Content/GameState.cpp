//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Content\GameState.h"

using namespace JigsawPuzzle;
using namespace Windows::Foundation;

GameState::GameState() :
    m_dragging(false),
#ifdef MEASURE_LATENCY
    m_toggleBackground(false),
#endif
    m_stateChanged(true)
{
    PuzzlePieceId ids[] = { TopLeft, TopRight, BottomLeft, BottomRight };
    bool idsChosen[] = { false, false, false, false };
    Point startingLocation = { Constants::PuzzlePieceMargin, Constants::PuzzlePieceMargin };

    srand(static_cast<UINT>(GetTickCount64()));
    UINT idIndex;
    for (UINT n = 0; n < Constants::PuzzlePieceCount; n++)
    {
        do
        {
            idIndex = rand() % Constants::PuzzlePieceCount;

        } while (idsChosen[idIndex]);
        idsChosen[idIndex] = true;

        m_pieces[n] = std::shared_ptr<PuzzlePiece>(new PuzzlePiece(startingLocation, ids[idIndex]));

        Point dropTargetLocation = Point(Constants::PuzzleBoardMargin + Constants::PuzzlePieceSize * (n % 2), Constants::PuzzleBoardMargin + Constants::PuzzlePieceSize * (n / 2));
        m_dropTargets[n] = std::shared_ptr<DropTarget>(new DropTarget(dropTargetLocation, ids[n]));

        startingLocation.X += Constants::PuzzlePieceMargin + Constants::PuzzlePieceSize;
    }
}

void GameState::OnPointerPressed(Point location)
{
    std::shared_ptr<PuzzlePiece> result = nullptr;

    for (std::shared_ptr<PuzzlePiece> p : m_pieces)
    {
        if (result != nullptr)
        {
            p->Unpick();
        }
        else if (p->Pick(location))
        {
            result = p;
            m_dragging = true;
        }
    }

    if (result == nullptr && m_selectedPiece != nullptr)
    {
        // Piece was deselected either because it's being placed on the game board,
        // or because the user clicked somewhere else. In either case the game will
        // need to be redrawn.
        m_stateChanged = true;

        for (std::shared_ptr<DropTarget> target : m_dropTargets)
        {
            if (target->Drop(m_selectedPiece.get(), location))
            {
                // If we're pointing to a drop spot, drop the piece at that location.
                m_selectedPiece->DropAndSnapTo(target->GetPosition());
                m_selectedPiece = nullptr;
                return;
            }
        }
        m_selectedPiece->Unpick();
    }

    m_selectedPiece = result;
}

void GameState::OnPointerReleased(Point location)
{
    if (m_selectedPiece != nullptr)
    {
        m_dragging = false;
        for (std::shared_ptr<DropTarget> target : m_dropTargets)
        {
            if (target->Drop(m_selectedPiece.get(), location))
            {
                // If we're hovering over a drop spot, snap to that location.
                m_selectedPiece->DropAndSnapTo(target->GetPosition());
                m_stateChanged = true;
                return;
            }
        }

        m_selectedPiece->Drop(location);
        m_stateChanged = true;
    }
}

void GameState::OnPointerMoved(Point location)
{
    if (m_selectedPiece != nullptr && m_dragging)
    {
        m_selectedPiece->Drag(location);
        m_stateChanged = true;

        for (std::shared_ptr<DropTarget> target : m_dropTargets)
        {
            target->Drag(m_selectedPiece.get(), location);
        }
    }
}
