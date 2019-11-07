/****************************** Module Header ******************************\
Module Name:  CounterInfo.h
Project:      CppCpuUsage
Copyright (c) Microsoft Corporation.

Structure to hold information about a Counter

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#ifndef PERFORMANCE_COUNTER_H
#define PERFORMANCE_COUNTER_H

#include <Pdh.h>
#include <vector>
#include "Log.h"

using namespace std;

class CounterInfo
{
public:
    PCWSTR counterName;

    PDH_HCOUNTER counter;

    vector<Log> logs;
};

#endif