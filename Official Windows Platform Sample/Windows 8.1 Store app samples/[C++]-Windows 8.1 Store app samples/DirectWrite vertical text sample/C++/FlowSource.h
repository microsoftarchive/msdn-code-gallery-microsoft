// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Contents:    Tells a flow layout where text is allowed to flow.

#pragma once

enum ReadingDirection;

class DECLSPEC_UUID("84600D90-8F09-480f-9275-D71125FCD0C3") FlowLayoutSource:
    public ComBase<QiListSelf<FlowLayoutSource, QiList<IUnknown>>>
{
public:
    struct RectF
    {
        float left;
        float top;
        float right;
        float bottom;
    };

    FlowLayoutSource():
        m_width(300),
        m_height(300),
        m_currentU(),
        m_currentV()
    {
        Reset();
    }

    void Reset();
    void SetSize(float width, float height);
    bool GetNextRect(float fontHeight, ReadingDirection readingDirection, _Out_ RectF* nextRect);

protected:
    float m_width;
    float m_height;
    float m_currentU;
    float m_currentV;
};
