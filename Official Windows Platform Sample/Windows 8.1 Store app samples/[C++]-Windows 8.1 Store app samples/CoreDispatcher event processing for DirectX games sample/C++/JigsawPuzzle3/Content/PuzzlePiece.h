//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace JigsawPuzzle
{
    enum PuzzlePieceId
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Invalid
    };

    class PuzzlePiece
    {
    public:
        PuzzlePiece(Windows::Foundation::Point location, PuzzlePieceId id);

        bool Pick(Windows::Foundation::Point location);
        void Unpick();
        void Drag(Windows::Foundation::Point location);
        void Drop(Windows::Foundation::Point location);
        void DropAndSnapTo(Windows::Foundation::Point location);

        // Methods to enable and drive animations.
        void AnimateTo(Windows::Foundation::Point location);
        bool Update();

        inline Windows::Foundation::Point GetPosition()
        {
            return Windows::Foundation::Point(m_bounds.X - m_pickOffset.X, m_bounds.Y - m_pickOffset.Y);
        }

        inline PuzzlePieceId GetId()
        {
            return m_id;
        }

        inline bool IsSelected()
        {
            return m_selected;
        }

        inline bool IsMoving()
        {
            return m_moving || m_animationStep.X != 0.0f || m_animationStep.Y != 0.0f;
        }

        inline bool IsSnapped()
        {
            return m_snapped;
        }

    private:
        void Drop();

        Windows::Foundation::Rect m_bounds;
        Windows::Foundation::Point m_pickOffset;
        PuzzlePieceId m_id;
        bool m_selected;
        bool m_moving;
        bool m_snapped;

        // Track the final destination of the animation and its pace.
        Windows::Foundation::Point m_destination;
        Windows::Foundation::Point m_animationStep;
    };
}
