//-----------------------------------------------------------------------
// <copyright file="RenderedData.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      RenderedData.h
//
// Description:
//      RenderedData Class
//      An object of this type will contain a rendered bitmap as well
//      as a wchar_t* string. This makes the data compatible with both
//      bitmap displays, alphanumeric displays, and other devices that
//      can make use of the string data.
//
//-----------------------------------------------------------------------


#pragma once


#include <windows.h>


class CRenderedData
{
public:
    CRenderedData(void);
    ~CRenderedData(void){}

    enum enumDataType
    {
        DataTypeUndefined,
        DataTypeApplication,
        DataTypeNotification,
        DataTypeDefaultBackground
    };

    enumDataType DataType;

    BYTE* pbBitmapData;
    size_t cbBitmapData;

    wchar_t* wszAlphanumericTitle;
    size_t cElementsAlphanumericTitle; // Includes trailing NULL

    wchar_t* wszAlphanumericBody;
    size_t cElementsAlphanumericBody; // Includes trailing NULL

    size_t NumberOfRenderedTextLines; // Number of text lines rendered in the bitmap

    void DeleteData(void); // Calls delete []
};
