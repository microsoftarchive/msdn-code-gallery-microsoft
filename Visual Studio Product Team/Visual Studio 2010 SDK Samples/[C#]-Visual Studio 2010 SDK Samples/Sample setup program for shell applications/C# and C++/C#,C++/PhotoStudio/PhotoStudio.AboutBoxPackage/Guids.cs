//******************************************************************
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// This code is licensed under the Visual Studio SDK license terms.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//******************************************************************
// Guids.cs
// MUST match guids.h
using System;

namespace PhotoStudio.AboutBoxPackage
{
    static class GuidList
    {
        public const string guidAboutBoxPackagePkgString = "c3999640-0406-40cf-ba15-541ff8cf0c08";
        public const string guidAboutBoxPackageCmdSetString = "15fdc884-a51c-4a8a-8003-8eb5c2f0d4fc";

        public static readonly Guid guidAboutBoxPackageCmdSet = new Guid(guidAboutBoxPackageCmdSetString);
    };
}