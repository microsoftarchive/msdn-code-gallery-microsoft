//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "WindowLayoutChildInterface.h"

using namespace Hilo::Direct2DHelpers;
using namespace Hilo::WindowApiHelpers;

HRESULT WindowLayout::SetMainWindow(__in_opt IWindow* mainWindow)
{
    m_applicationWindow = mainWindow;
    return S_OK;
}

HRESULT WindowLayout::GetMainWindow(IWindow** mainWindow)
{
    return AssignToOutputPointer(mainWindow, m_applicationWindow);
}

HRESULT WindowLayout::GetChildWindowCount(unsigned int* count)
{
    // Check for valid out parameter
    assert(count);

    (*count) = static_cast<unsigned int>(m_childWindows.size());

    return S_OK;
}

HRESULT WindowLayout::GetChildWindow(unsigned int index, IWindow** childWindow)
{
    // Check for valid out parameter
    assert(childWindow);
    *childWindow = nullptr;

    // Check for valid index value
    if (index >= m_childWindows.size())
    {
        return E_INVALIDARG;
    }

    return AssignToOutputPointer(childWindow, m_childWindows[index]);
}

HRESULT WindowLayout::GetChildWindowLayoutHeight(unsigned int index, unsigned int* height)
{
    // Check for valid out parameter
    assert(height);

    // Check for valid index value
    if (index >= m_childWindows.size())
    {
        return E_INVALIDARG;
    }

    if (index == 0)
    {
        if (m_isSlideShow)
        {
            *height = 0;
        }
        else
        {
            *height = m_carouselPaneHeight;
        }
    }

    return S_OK;
}

HRESULT WindowLayout::SetChildWindowLayoutHeight(unsigned int /*index*/, unsigned int height)
{
    m_carouselPaneHeight = height;
    return S_OK;
}

WindowLayout::WindowLayout() : m_isSlideShow(false)
{
}

WindowLayout::~WindowLayout()
{
}

HRESULT WindowLayout::SwitchDisplayMode(bool mode)
{
    // Update slide show mode
    m_isSlideShow = mode;

    // Update window layout
    return UpdateLayout();
}

HRESULT WindowLayout::UpdateLayout()
{
    RECT clientRect;

    HRESULT hr = m_applicationWindow->GetClientRect(&clientRect);
    if (SUCCEEDED(hr))
    {
        // Tracks remaining width and height
        unsigned int width = clientRect.right - clientRect.left;
        unsigned int height = clientRect.bottom - clientRect.top;

        // Tracks current position for window placement
        unsigned int xPos = 0;
        unsigned int yPos = 0;

        if (m_isSlideShow)
        {
            // Hide the carousel pane during slide-show mode
            hr = m_childWindows[0]->SetSize(0, 0);
        }
        else
        {
            // Update carousel pane position
            hr = m_childWindows[0]->SetPosition(xPos, yPos);
            if (SUCCEEDED(hr))
            {
                // Update carousel pane position
                hr = m_childWindows[0]->SetSize(width, m_carouselPaneHeight);
            }

            // Update current y-position
            yPos = min(yPos + m_carouselPaneHeight, height);
        }

        if (SUCCEEDED(hr))
        {
            // Update media pane position
            hr = m_childWindows[1]->SetPosition(xPos, yPos);
        }
        if (SUCCEEDED(hr))
        {
            // Update media pane size
            hr = m_childWindows[1]->SetSize(width, height - yPos);
        }
    }

    return hr;
}

HRESULT WindowLayout::InsertChildWindow(IWindow* childWindow, __out unsigned int* index)
{
    assert(index);

    // Add child window to vector of child windows
    m_childWindows.push_back(childWindow);
    *index = static_cast<unsigned int>(m_childWindows.size() - 1);

    ComPtr<IWindowMessageHandler> messageHandler;
    ComPtr<IWindowLayoutChild> windowLayoutChild;

    HRESULT hr = childWindow->GetMessageHandler(&messageHandler);
    if (SUCCEEDED(hr))
    {
        hr = messageHandler.QueryInterface(&windowLayoutChild);
    }

    if (SUCCEEDED(hr))
    {
        // Set the window layout information for this child window
        hr = windowLayoutChild->SetWindowLayout(this);
    }

    return hr;
}

HRESULT WindowLayout::Finalize()
{
    HRESULT hr = S_OK;

    // Call each child window's 'Finalize' method. This allows each child window to remove any COM object dependencies
    // and stops the asynchronous loader threads.
    for (auto iter = m_childWindows.begin(); iter < m_childWindows.end(); iter++)
    {
        // Retrieve the message handler
        ComPtr<IWindowMessageHandler> handler;
        hr = (*iter)->GetMessageHandler(&handler);

        if (FAILED(hr))
        {
            continue;
        }

        ComPtr<IWindowLayoutChild> child;
        hr = handler.QueryInterface(&child);

        if (FAILED(hr))
        {
            continue;
        }

        hr = child->Finalize();
    }

    return hr;
}
