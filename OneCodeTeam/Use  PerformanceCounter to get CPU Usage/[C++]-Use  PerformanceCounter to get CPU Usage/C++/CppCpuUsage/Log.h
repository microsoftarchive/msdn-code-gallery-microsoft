/****************************** Module Header ******************************\
Module Name:  Log.h
Project:      CppCpuUsage
Copyright (c) Microsoft Corporation.

Structure to hold a Log

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#ifndef LOG_H
#define LOG_H

class Log
{
public:
    DWORD time;
    double value;
};

#endif