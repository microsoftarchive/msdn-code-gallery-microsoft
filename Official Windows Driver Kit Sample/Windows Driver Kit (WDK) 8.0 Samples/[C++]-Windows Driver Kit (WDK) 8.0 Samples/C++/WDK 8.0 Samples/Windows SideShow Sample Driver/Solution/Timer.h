//-----------------------------------------------------------------------
// <copyright file="Timer.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Timer.h
//
// Description:
//      Timer Class
//
//-----------------------------------------------------------------------


#include <windows.h>


// This thread will wake once a second to trigger the Data Manager
// The timer thread will have its own loop which will mainly be a Sleep(x);


// Singleton object
class CTimer
{
public:
    CTimer(void);
    ~CTimer(void);

    HRESULT CallDataManagerTimerMaintenance(void);
};
