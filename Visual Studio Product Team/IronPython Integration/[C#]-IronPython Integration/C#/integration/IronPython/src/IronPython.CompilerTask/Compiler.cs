using System;
using System.Collections.Generic;
using System.Text;
using CoreIronPython = IronPython;

namespace Microsoft.Samples.VisualStudio.IronPython.CompilerTasks
{
	/// <summary>
	/// The main purpose of this class is to associate the PythonCompiler
	/// class with the ICompiler interface.
	/// </summary>
	public class Compiler : CoreIronPython.Hosting.PythonCompiler, ICompiler
	{
		public Compiler(IList<string> sourcesFiles, string OutputAssembly)
			: base(sourcesFiles, OutputAssembly)
		{
		}

        public Compiler(IList<string> sourcesFiles, string OutputAssembly, CoreIronPython.Hosting.CompilerSink compilerSink)
			: base(sourcesFiles, OutputAssembly, compilerSink)
		{
		}
	}
}
