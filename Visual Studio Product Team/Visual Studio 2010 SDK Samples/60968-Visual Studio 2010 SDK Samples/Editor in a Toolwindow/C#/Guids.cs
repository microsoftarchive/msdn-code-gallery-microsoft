// Guids.cs
// MUST match guids.h
using System;

namespace Samples.EditorToolwindow
{
    static class GuidList
    {
        public const string guidEditorToolwindowPkgString = "08cf7bd4-9cb6-4b32-b795-b932c001b016";
        public const string guidEditorToolwindowCmdSetString = "bf87e998-e9f8-4d3b-bb38-5a660a27ba1d";
        public const string guidToolWindowPersistanceString = "e3165f5f-3dd7-441f-9e5d-0f4233eb2743";

        public static readonly Guid guidEditorToolwindowCmdSet = new Guid(guidEditorToolwindowCmdSetString);
    };
}