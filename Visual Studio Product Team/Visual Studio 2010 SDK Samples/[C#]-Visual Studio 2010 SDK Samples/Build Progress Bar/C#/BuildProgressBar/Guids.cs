// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.BuildProgressBar
{
    static class GuidList
    {
        public const string guidProgressBarPkgString = "646a5020-a33d-4509-864e-9dc6bf7206f0";
        public const string guidProgressBarCmdSetString = "413e9fca-f16d-4883-86c1-9d6f6e8af3dd";
        public const string guidToolWindowPersistanceString = "1bcb49dc-47f9-4eba-8d7d-b2baefe89076";

        public static readonly Guid guidProgressBarCmdSet = new Guid(guidProgressBarCmdSetString);
    };
}