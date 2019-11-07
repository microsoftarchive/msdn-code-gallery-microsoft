// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Contents:    Tells a flow layout where text is allowed to flow.
//
//----------------------------------------------------------------------------
#include "pch.h"
#include "TextAnalysis.h"
#include "FlowSource.h"

void FlowLayoutSource::Reset()
{
    m_currentV = 0;
    m_currentU = 0;
}

void FlowLayoutSource::SetSize(
    float width,
    float height
    )
{
    m_width  = width;
    m_height = height;
}

bool FlowLayoutSource::GetNextRect(
    float fontHeight,
    ReadingDirection readingDirection,
    _Out_ RectF* nextRect
    )
{
    RectF& rect = *nextRect;

    // Set defaults.
    RectF zeroRect = {};
    rect = zeroRect;

    if (m_height <= 0 || m_width <= 0)
    {
        return 1; // Do nothing if empty.
    }

    bool const isVertical           = (readingDirection & ReadingDirectionPrimaryAxis) != 0;
    bool const isReversedPrimary    = (readingDirection & ReadingDirectionPrimaryProgression) != 0;
    bool const isReversedSecondary  = (readingDirection & ReadingDirectionSecondaryProgression) != 0;

    float const uSize     = isVertical ? m_height : m_width;
    float const vSize     = isVertical ? m_width  : m_height;
    float const uSizeHalf = uSize / 2;
    float const vSizeHalf = vSize / 2;

    if (m_currentV >= vSize)
    {
        return 1; // Crop any further lines.
    }

    // Initially set to entire size.
    rect.left   = 0;
    rect.top    = 0;
    rect.right  = uSize;
    rect.bottom = vSize;

    // Advance to the next row/column.
    float u = m_currentU;
    float v = m_currentV;

    m_currentV += fontHeight;

    float minSizeHalf = min(uSizeHalf, vSizeHalf);
    float adjustedV = (v + fontHeight/2) - vSizeHalf;

    // Determine x from y using circle formula d^2 = (x^2 + y^2).
    float d2    = (minSizeHalf * minSizeHalf) - (adjustedV * adjustedV);
    u           = (d2 > 0) ? sqrt(d2) : -1;
    rect.top    = v;
    rect.bottom = v + fontHeight;
    rect.left   = uSizeHalf - u;
    rect.right  = uSizeHalf + u;

    // Reorient rect according to reading direction.
    if (isReversedPrimary)
    {
        std::swap(rect.left, rect.right);
        rect.left  = uSize - rect.left;
        rect.right = uSize - rect.right;
    }
    if (isReversedSecondary)
    {
        std::swap(rect.top, rect.bottom);
        rect.top    = vSize - rect.top;
        rect.bottom = vSize - rect.bottom;
    }
    if (isVertical)
    {
        std::swap(rect.left, rect.top);
        std::swap(rect.right, rect.bottom);
    }

    return 1;
}