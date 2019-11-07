using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

using EnvDTE;
using DteProject = EnvDTE.Project;

namespace Microsoft.Samples.VisualStudio.CodeDomCodeModel {
    public static class PythonCodeModelFactory {
        public static FileCodeModel CreateFileCodeModel(ProjectItem item, CodeDomProvider provider, string fileName) {
            if (null == item) {
                throw new ArgumentNullException("item");
            }
            return CreateFileCodeModel(item.DTE, item, provider, fileName);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "0#dte")]
        public static FileCodeModel CreateFileCodeModel(DTE dte, ProjectItem item, CodeDomProvider provider, string fileName) {
            if (null == item) {
                throw new ArgumentNullException("item");
            }
            return new CodeDomFileCodeModel(dte, item, provider, fileName);
        }

        public static CodeModel CreateProjectCodeModel(DteProject project)
        {
            return new PythonProjectCodeModel(project);
        }
    }
}
