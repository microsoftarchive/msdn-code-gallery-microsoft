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


#include "RenderedData.h"


CRenderedData::CRenderedData(void) :
DataType(DataTypeUndefined),
pbBitmapData(NULL),
cbBitmapData(0),
wszAlphanumericTitle(NULL),
cElementsAlphanumericTitle(0),
wszAlphanumericBody(NULL),
cElementsAlphanumericBody(0),
NumberOfRenderedTextLines(0)
{
}


void CRenderedData::DeleteData(void)
{
    DataType = DataTypeUndefined;

    if (NULL != pbBitmapData)
    {
        delete [] pbBitmapData;
        pbBitmapData = NULL;
    }
    cbBitmapData = 0;

    if (NULL != wszAlphanumericTitle)
    {
        delete [] wszAlphanumericTitle;
        wszAlphanumericTitle = NULL;
    }
    cElementsAlphanumericTitle = 0;

    if (NULL != wszAlphanumericBody)
    {
        delete [] wszAlphanumericBody;
        wszAlphanumericBody = NULL;
    }
    cElementsAlphanumericBody = 0;

    NumberOfRenderedTextLines = 0;

    return;
}
