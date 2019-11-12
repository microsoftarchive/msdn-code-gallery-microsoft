/****************************** Module Header ******************************\
Module Name:  Main.cpp
Project:      CppCpuUsage
Copyright (c) Microsoft Corporation.

Main Entry point of Application

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#include <Windows.h>
#include <gdiplus.h>
#include <process.h>
#include <stdio.h>
#include "Query.h"
#include "Helper.h"

const int CENTER_X = 55;
const int CENTER_Y = 580;
const int SCALE_X = 7;
const int SCALE_Y = 5;

int diff_x = 0;
HWND hwnd;
static HINSTANCE g_hInst;
static HWND g_hAddButton;
static HWND g_hRefreshButton;
static HWND g_hCounterComboBox;

Query query;

#define IDC_BUTTON_ADD 101
#define IDC_COMBOBOX_COUNTER 102
#define IDC_BUTTON_REFRESH 103

bool fContinueLog = true;

using namespace Gdiplus;

VOID OnPaint(HDC hdc);

unsigned __stdcall KeepLogging( void* )
{
    while(fContinueLog)
    {
        if(query.vciSelectedCounters.size()>0)
        {
            query.Record();
            diff_x = query.CleanOldRecord();
            InvalidateRect(hwnd, NULL, TRUE); 
        }
    }
    return 0;
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

void PlotLine(Graphics& g, Pen& p, float x1, float y1, float x2, float y2, bool fDiff)
{
    if(fDiff)
    {
        x1 = x1 - diff_x;
        x2 = x2 - diff_x;
    }
    x1 = CENTER_X + x1 * SCALE_X;
    x2 = CENTER_X + x2 * SCALE_X;
    y1 = CENTER_Y - y1 * SCALE_Y;
    y2 = CENTER_Y - y2 * SCALE_Y;

    if((x1>=0&&x2>=0)||(!fDiff))
    {
        g.DrawLine(&p,x1,y1,x2,y2);
    }
}

void PlotString(Graphics& g, float x1, float y1, PCWSTR text, int len)
{
    x1 = CENTER_X + x1 * SCALE_X;
    y1 = CENTER_Y - y1 * SCALE_Y;

    PointF origin(x1,y1);
    SolidBrush blackBrush(Color(255, 0, 0, 0));
    Font myFont(L"Arial", 16);

    wchar_t* wa = new wchar_t[len]; 
    //mbstowcs(wa,text,len); 

    g.DrawString(
        text,
        len,
        &myFont,
        origin,
        &blackBrush);
}

VOID OnPaint(HDC hdc)
{
    Graphics graphics(hdc);
    Pen      pen(Color(255, 0, 0, 0));   
    graphics.DrawRectangle(&pen,10,50,770,550);

    wchar_t  * buffer = new wchar_t[4];

    // AXIS   
    PlotLine(graphics,pen,0.0,0.0,0.0,100.0,false);
    PlotLine(graphics,pen,0.0,0.0,100.0,0.0,false);
    for(float i = 10.0; i<=100.0; i+=10.0 )
    {
        PlotLine(graphics,pen,0.0,i,-1,i,false);		
        swprintf(buffer,L"%d",(int)i);				 
        PlotString(graphics,(i==100.0?-6.0f:-5.0f),i,buffer,3);
    }


    for(auto it = query.vciSelectedCounters.begin();
        it < query.vciSelectedCounters.end();
        it++)
    {
        double prevTime = -1;
        double prevValue = -1;
        for(auto it2 = it->logs.begin(); it2 < it->logs.end(); it2++ )
        {
            if(prevTime != -1 && prevValue != -1)
            {
                PlotLine(graphics,pen,(float)prevTime,(float)prevValue,(float)it2->time,(float)it2->value,true);
            }

            prevTime = it2->time;
            prevValue = it2->value;
        }
    }

    buffer = NULL;
}

wchar_t  ListItem[256];

void RefreshCounterList()
{
    SendMessage(g_hCounterComboBox,(UINT) CB_RESETCONTENT ,(WPARAM) 0, (LPARAM)(0)); 

    vector<PCTSTR> validCounterNames = GetValidCounterNames();

    for(auto it = validCounterNames.begin(); it < validCounterNames.end(); it++ )
    {
        SendMessage(g_hCounterComboBox,(UINT) CB_ADDSTRING,(WPARAM) 0, (LPARAM)(*it)); 
    }	
    SendMessage(g_hCounterComboBox, CB_SETCURSEL, (WPARAM)0, (LPARAM)0);

    int ItemIndex = 0;            
    (wchar_t) SendMessage((HWND) g_hCounterComboBox, (UINT) CB_GETLBTEXT, 
        (WPARAM) ItemIndex, (LPARAM) ListItem); 
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    HDC          hdc;
    PAINTSTRUCT  ps;
    int ItemIndex;

    switch(uMsg)
    {
    case WM_COMMAND:
        if(((HWND)lParam) && (HIWORD(wParam) == BN_CLICKED))
        {
            int iMID;
            iMID = LOWORD(wParam);
            switch(iMID)
            {
            case IDC_BUTTON_ADD:		
                {					
                    query.AddCounterInfo(ListItem);
                    break;
                }
            case IDC_BUTTON_REFRESH:		
                {								
                    RefreshCounterList();
                    break;
                }
            default:				
                break;
            }
        }

        if(HIWORD(wParam) == CBN_SELCHANGE)
        { 
            ItemIndex = SendMessage((HWND) lParam, (UINT) CB_GETCURSEL, 
                (WPARAM) 0, (LPARAM) 0);            
            (wchar_t) SendMessage((HWND) lParam, (UINT) CB_GETLBTEXT, 
                (WPARAM) ItemIndex, (LPARAM) ListItem);                                 
        }

        return 0;

    case WM_PAINT:
        hdc = BeginPaint(hwnd, &ps);
        OnPaint(hdc);
        EndPaint(hwnd, &ps);
        return 0;

    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    case WM_GETMINMAXINFO:
    case WM_SIZING:
        return 0;
    case WM_CREATE:
        {

            g_hAddButton = CreateWindowEx(0,            /* more or ''extended'' styles */
                L"BUTTON",                         /* GUI ''class'' to create */
                L"Add",							/* GUI caption */
                WS_CHILD|WS_VISIBLE|BS_DEFPUSHBUTTON,   /* control styles separated by | */
                640,                                    /* LEFT POSITION (Position from left) */
                10,                                     /* TOP POSITION  (Position from Top) */
                100,                                    /* WIDTH OF CONTROL */
                30,                                     /* HEIGHT OF CONTROL */
                hwnd,                                   /* Parent window handle */
                (HMENU)IDC_BUTTON_ADD,                  /* control''s ID for WM_COMMAND */
                g_hInst,                                /* application instance */
                NULL);

            g_hRefreshButton = CreateWindowEx(0,        /* more or ''extended'' styles */
                L"BUTTON",                         /* GUI ''class'' to create */
                L"Refresh",                        /* GUI caption */
                WS_CHILD|WS_VISIBLE|BS_DEFPUSHBUTTON,   /* control styles separated by | */
                520,                                    /* LEFT POSITION (Position from left) */
                10,                                     /* TOP POSITION  (Position from Top) */
                100,                                    /* WIDTH OF CONTROL */
                30,                                     /* HEIGHT OF CONTROL */
                hwnd,                                   /* Parent window handle */
                (HMENU)IDC_BUTTON_REFRESH,              /* control''s ID for WM_COMMAND */
                g_hInst,                                /* application instance */
                NULL);

            g_hCounterComboBox = CreateWindowEx(0,              /* more or ''extended'' styles */
                L"ComboBox",								/* GUI ''class'' to create */
                NULL,											/* GUI caption */
                CBS_DROPDOWN|WS_VSCROLL | WS_CHILD | WS_VISIBLE,  /* control styles separated by | */
                10,													/* LEFT POSITION (Position from left) */
                10,                                     /* TOP POSITION  (Position from Top) */
                500,                                    /* WIDTH OF CONTROL */
                400,                                     /* HEIGHT OF CONTROL */
                hwnd,                                   /* Parent window handle */
                (HMENU)IDC_COMBOBOX_COUNTER,                        /* control''s ID for WM_COMMAND */
                g_hInst,                                /* application instance */
                NULL);

            RefreshCounterList();
        }
    }

    return DefWindowProc(hwnd, uMsg, wParam, lParam);	
}

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE, PWSTR , int nCmdShow)
{
    HANDLE hThread;
    unsigned threadID;	
    GdiplusStartupInput gdiplusStartupInput;
    ULONG_PTR           gdiplusToken;
    GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

    query.Init();

    WNDCLASS wc = { };

    wc.lpfnWndProc = WindowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"CppCpuUsage";
    wc.hbrBackground  = (HBRUSH)GetStockObject(WHITE_BRUSH);

    g_hInst = hInstance;

    RegisterClass(&wc);

    hwnd = CreateWindowEx(
        0,
        L"CppCpuUsage",
        L"CppCpuUsage",		
        WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        5, 5, 800, 640,
        NULL,
        NULL,
        hInstance,
        NULL);

    if( hwnd == NULL )
    {
        return 0;
    }

    ShowWindow(hwnd, nCmdShow);

    hThread = (HANDLE)_beginthreadex( NULL, 0, &KeepLogging, NULL, 0, &threadID );

    MSG msg = { };

    while(GetMessage(&msg,NULL,0,0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    GdiplusShutdown(gdiplusToken);
    return 0;
}