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
    m_animating(false),
    m_solved(false)
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

    m_timer.ResetElapsedTime();
}

void GameState::OnPointerPressed(Point location)
{
    // 3. Disallow picking a new puzzle piece when animating or when the puzzle has been solved.
    if (m_animating || m_solved)
    {
        return;
    }

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
        for (std::shared_ptr<DropTarget> target : m_dropTargets)
        {
            if (target->Drop(m_selectedPiece.get(), location))
            {
                // If we're pointing to a drop spot, animate the piece to that location.
                m_selectedPiece->AnimateTo(target->GetPosition());
                m_animating = true;
                return;
            }
        }
        m_selectedPiece->Unpick();
    }

    m_selectedPiece = result;
}

void GameState::OnPointerReleased(Point location)
{
    // Disallow dropping the piece while it is animating.
    if (m_selectedPiece != nullptr && !m_animating)
    {
        m_dragging = false;
        for (std::shared_ptr<DropTarget> target : m_dropTargets)
        {
            if (target->Drop(m_selectedPiece.get(), location))
            {
                // If we're hovering over a drop spot, snap to that location.
                m_selectedPiece->DropAndSnapTo(target->GetPosition());
                return;
            }
        }

        m_selectedPiece->Drop(location);
    }
}

void GameState::OnPointerMoved(Point location)
{
    // Disallow dragging the piece while it is animating.
    if (m_selectedPiece != nullptr && m_dragging && !m_animating)
    {
        m_selectedPiece->Drag(location);

        for (std::shared_ptr<DropTarget> target : m_dropTargets)
        {
            target->Drag(m_selectedPiece.get(), location);
        }
    }
}

// 3. Update the timer and the location of the animating puzzle piece, if applicable.
void GameState::Update() 
{
    if (!m_solved)
    {
        CheckSolutionStatus();
    }

    if (m_solved)
    {
        m_selectedPiece = nullptr;
        return;
    }

    m_timer.Tick([](){});
    
    if (m_animating)
    {
        m_animating = m_selectedPiece->Update();
        if (!m_animating)
        {
            m_selectedPiece = nullptr;
        }
    }
}

// 3. Determine if the puzzle has been solved.
void GameState::CheckSolutionStatus()
{
    bool solved = true;
    for (std::shared_ptr<PuzzlePiece> p : m_pieces)
    {
        if (!p->IsSnapped())
        {
            solved = false;
            break;
        }
    }

    m_solved = solved;
}
