// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

void ApplyCartoonEffectPPL(byte* sourcePixels, 
                           unsigned int width, 
                           unsigned int height, 
                           unsigned int neighborWindow, 
                           unsigned int phases, 
                           unsigned int size, 
                           concurrency::cancellation_token token);