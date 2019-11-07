/****************************** Module Header ******************************\
Module Name:  Helper.h
Project:      CppCpuUsage
Copyright (c) Microsoft Corporation.

Declaration of various helper functions

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#ifndef HELPER_H
#define HELPER_H

#include <Windows.h>
#include <vector>

#define SIZE 1024

using namespace std;

DWORD GetProcessorCount();

vector<PCTSTR> GetProcessNames();

vector<PCTSTR> GetValidCounterNames();

#endif