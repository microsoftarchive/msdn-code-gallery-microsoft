using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;

namespace IronPython.EditorExtensions
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(PyContentTypeDefinition.ContentType)]
    [ContentType(PyContentTypeDefinition.ConsoleContentType)]
    internal class PyClassificationProvider : IClassifierProvider
    {
        [Import]
        IClassificationTypeRegistryService classificationRegistryService { get; set; }

        IClassifier IClassifierProvider.GetClassifier(ITextBuffer textBuffer)
        {
            // Creates the python classifier
            return new PyClassifier(textBuffer, classificationRegistryService);
        }
    }
}