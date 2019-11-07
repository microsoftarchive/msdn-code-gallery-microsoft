//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

//
// File type for a given shell item...
//
enum ShellFileType
{
    FileTypeDefault = 0x1,
    FileTypeImage = 0x2,
    FileTypeAudio = 0x4,
    FileTypeVideo = 0x8,
    FileTypeFolder = 0x10
};
