//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Classification;

namespace ToDoGlyphFactory
{
    internal class ToDoTag : IGlyphTag { }

    internal class ToDoTagger : ITagger<ToDoTag>
    {
        
        private const string _searchText = "todo";

        IEnumerable<ITagSpan<ToDoTag>> 
            ITagger<ToDoTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            //todo: implement tagging
            foreach (SnapshotSpan curSpan in spans)
            {
                int loc = curSpan.GetText().ToLower().IndexOf(_searchText);
                if (loc > -1)
                {
                    SnapshotSpan todoSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curSpan.Start + loc, _searchText.Length));
                    yield return new TagSpan<ToDoTag>(todoSpan, new ToDoTag());
                }
            }
            
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add {}
            remove {}
        }
    }
}
