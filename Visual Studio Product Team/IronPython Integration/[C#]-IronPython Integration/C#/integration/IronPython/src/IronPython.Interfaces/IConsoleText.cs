using System;

namespace Microsoft.Samples.VisualStudio.IronPython.Interfaces
{
    /// <summary>
    /// Interface used to provide a way for other object to access the text
    /// inside the console window.
    /// </summary>
    public interface IConsoleText
    {
        /// <summary>
        /// Returns the text inside a line in the console up to a specific colums.
        /// The skipReadOnly flag is used to specify if the text that is inside the
        /// read-only region should be skipped from the return value.
        /// </summary>
        string TextOfLine(int line, int endColumn, bool skipReadOnly);
    }

}
