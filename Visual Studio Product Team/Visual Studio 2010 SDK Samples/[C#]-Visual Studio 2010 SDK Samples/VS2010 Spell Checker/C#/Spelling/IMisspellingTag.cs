using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.VisualStudio.Language.Spellchecker
{
    interface IMisspellingTag : ITag
    {
        IEnumerable<string> Suggestions { get; }
    }
}
