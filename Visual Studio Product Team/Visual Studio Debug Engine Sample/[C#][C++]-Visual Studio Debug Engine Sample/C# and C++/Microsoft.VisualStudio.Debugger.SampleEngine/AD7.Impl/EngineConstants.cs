using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Debugger.SampleEngine
{
    static public class EngineConstants
    {
        /// <summary>
        /// This is the engine GUID of the sample engine. It needs to be changed here and in EngineRegistration.pkgdef
        /// when turning the sample into a real engine.
        /// </summary>
        public const string EngineId = "{D951924A-4999-42a0-9217-1EB5233D1D5A}";
    }
}
