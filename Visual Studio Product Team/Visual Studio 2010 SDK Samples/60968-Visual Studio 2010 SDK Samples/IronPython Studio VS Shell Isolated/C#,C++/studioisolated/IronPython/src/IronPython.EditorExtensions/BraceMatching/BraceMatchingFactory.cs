using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Adornments;

namespace IronPython.EditorExtensions
{
    /// <summary>
    /// Adds the brace matching support when the view is created
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
	[ContentType(PyContentTypeDefinition.ContentType)]
	[TextViewRole(PredefinedTextViewRoles.Document)]
    internal class BraceMatchingFactory : IWpfTextViewCreationListener
    {
        [Import(typeof(ITextMarkerProviderFactory))]
        ITextMarkerProviderFactory TextMakerProviderFactory { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            // Create the brace matching presenter
            textView.Properties.GetOrCreateSingletonProperty<BraceMatchingPresenter>(() =>
                new BraceMatchingPresenter(textView, TextMakerProviderFactory)
                );            
        }
    }
}