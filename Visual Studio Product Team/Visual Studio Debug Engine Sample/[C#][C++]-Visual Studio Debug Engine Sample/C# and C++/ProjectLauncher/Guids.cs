// Guids.cs
// MUST match guids.h
using System;

namespace ProjectLauncher
{
    static class GuidList
    {
        public const string guidProjectLauncherPkgString = "933a507d-7abd-4a75-b906-895d8cc0616e";
        public const string guidProjectLauncherCmdSetString = "3ebc3be9-5f46-4b77-a2a3-73cfeb1a8791";

        public static readonly Guid guidProjectLauncherCmdSet = new Guid(guidProjectLauncherCmdSetString);
    };
}