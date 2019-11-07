using System;
using System.ComponentModel.Design;

namespace Microsoft.Samples.VisualStudio.IronPython.Project
{
    /// <summary>
    /// CommandIDs matching the commands defined symbols in PkgCmd.vsct
    /// </summary>
    public sealed class PythonMenus
    {
        internal static readonly Guid guidIronPythonProjectCmdSet = new Guid(GuidList.guidIronPythonProjectCmdSetString);
        internal static readonly CommandID SetAsMain = new CommandID(guidIronPythonProjectCmdSet, 0x3001);
    }
}

