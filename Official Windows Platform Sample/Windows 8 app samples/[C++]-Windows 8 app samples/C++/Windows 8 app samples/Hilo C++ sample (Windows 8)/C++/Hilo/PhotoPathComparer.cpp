// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "PhotoPathComparer.h"
#include "IPhoto.h"

using namespace Hilo;

bool PhotoPathComparer::operator()(IPhoto^ left, IPhoto^ right) const
{
    if (nullptr == left && nullptr == right)
    {
        return true;
    }
    if (nullptr == left || nullptr == right)
    {
        return false;
    }
    bool equal = left->Path == right->Path;
    return equal;
}