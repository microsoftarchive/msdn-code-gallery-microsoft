using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPython.EditorExtensions
{
    /// <summary>
    /// Defines the classification type names
    /// </summary>
	internal class PyClassificationTypes
	{
		internal const string ReadOnlyRegion = "PythonReadOnlyRegion";
		internal const string Comment = "PythonComment";
		internal const string Delimiter = "PythonDelimiter";
		internal const string Operator = "PythonOperator";
		internal const string Keyword = "PythonKeyword";
		internal const string Identifier = "PythonIdentifier";
		internal const string String = "PythonString";
		internal const string Number = "PythonNumber";
		internal const string Unknown = "PythonUnknown";
	}
}
