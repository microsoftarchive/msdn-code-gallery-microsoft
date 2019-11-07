// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.Samples.VisualStudio.IDE.WinformsControlsInstaller

{
    static class GuidList
    {
        public const string guidWinformsControlsInstallerPkgString = "bbdf2b9d-b9bc-4d0c-8480-f46c68806fe2";
        public const string guidWinformsControlsInstallerCmdSetString = "58a3c677-9903-43ba-b991-df7093aaf841";

        public static readonly Guid guidWinformsControlsInstallerCmdSet = new Guid(guidWinformsControlsInstallerCmdSetString);
    };
}