//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace PkgDefLanguage
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("pkgdef")]
    [TagType(typeof(TextMarkerTag))]

    internal class BraceMatchingTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                return null;

            //provide highlighting only on the top-level buffer
            if (textView.TextBuffer != buffer)
                return null;

            return new BraceMatchingTagger(textView, buffer) as ITagger<T>;
        }
    }

    internal class BraceMarkerTag : TextMarkerTag
    {
        internal BraceMarkerTag()
            : base("bracehighlight")
        {
        }
    }
    
    /// <summary>
    /// Watches for events that relocate the cursor and sends out TagsChanged when
    /// braces or quotes should be highlighted. GetTags() finds the matching
    /// brace or quote
    /// </summary>
    internal class BraceMatchingTagger : ITagger<TextMarkerTag>
    {
        ITextView View { get; set; }
        ITextBuffer SourceBuffer { get; set; }
        SnapshotPoint? CurrentChar { get; set; }
        private Dictionary<char, char> _braceList;

        internal BraceMatchingTagger(ITextView view, ITextBuffer sourceBuffer)
        {
            //here the keys are the open braces, and the values are the close braces
            _braceList = new Dictionary<char, char>();
            _braceList.Add('[', ']');
            _braceList.Add('{', '}');

            this.View = view;
            this.SourceBuffer = sourceBuffer;
            this.CurrentChar = null;

            this.View.Caret.PositionChanged += CaretPositionChanged;
            this.View.LayoutChanged += ViewLayoutChanged;
        }

        void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot) //make sure that there has really been a change
            {
                UpdateAtCaretPosition(View.Caret.Position);
            }
        }

        void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        /// <summary>
        /// As the caret moves around, ask for new tags
        /// </summary>
        void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            CurrentChar = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

            if (!CurrentChar.HasValue)
                return;

            var tempEvent = TagsChanged;
            if (tempEvent != null)
                tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0,
                    SourceBuffer.CurrentSnapshot.Length)));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged; 

        /// <summary>
        /// Find the matching brace or quote, anchored by the character under the caret
        /// </summary>
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)   //there is no content in the buffer
                yield break;

            //don't do anything if the current SnapshotPoint is not initialized or at the end of the buffer
            if (!CurrentChar.HasValue || CurrentChar.Value.Position >= CurrentChar.Value.Snapshot.Length)
                yield break;

            //hold on to a snapshot of the current character
            SnapshotPoint currentChar = CurrentChar.Value;

            //if the requested snapshot isn't the same as the one the brace is on, translate our spans to the expected snapshot
            if (spans[0].Snapshot != currentChar.Snapshot)
            {
                currentChar = currentChar.TranslateTo(spans[0].Snapshot, PointTrackingMode.Positive);
            }

            //get the current char and the previous char
            char currentText = currentChar.GetChar();
            SnapshotPoint lastChar = currentChar == 0 ? currentChar : currentChar - 1; //if currentChar is 0 (beginning of buffer), don't move it back
            char lastText = lastChar.GetChar();
            SnapshotSpan pairSpan = new SnapshotSpan();

            if (_braceList.ContainsKey(currentText))   //the key is the open brace
            {
                char closeChar;
                _braceList.TryGetValue(currentText, out closeChar);
                if (BraceMatchingTagger.FindMatchingCloseChar(currentChar, currentText, closeChar, out pairSpan) == true)
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(currentChar, 1), new BraceMarkerTag());
                    yield return new TagSpan<TextMarkerTag>(pairSpan, new BraceMarkerTag());
                }
            }
            else if (_braceList.ContainsValue(lastText))    //the value is the close brace, which is the *previous* character 
            {
                var open = from n in _braceList
                           where n.Value.Equals(lastText)
                           select n.Key;
                if (BraceMatchingTagger.FindMatchingOpenChar(lastChar, (char)open.ElementAt<char>(0), lastText, out pairSpan) == true)
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(lastChar, 1), new BraceMarkerTag());
                    yield return new TagSpan<TextMarkerTag>(pairSpan, new BraceMarkerTag());
                }
            }
            else if (currentText == '"')
            {
                if (BraceMatchingTagger.FindMatchingCloseQuote(currentChar, out pairSpan))
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(currentChar, 1), new BraceMarkerTag());
                    yield return new TagSpan<TextMarkerTag>(pairSpan, new BraceMarkerTag());
                }
            }
            else if (lastText == '"')
            {
                if (BraceMatchingTagger.FindMatchingOpenQuote(lastChar, out pairSpan))
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(lastChar, 1), new BraceMarkerTag());
                    yield return new TagSpan<TextMarkerTag>(pairSpan, new BraceMarkerTag());
                }
            }
        }

        private static bool FindMatchingCloseChar(SnapshotPoint startPoint, char open, char close, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            SnapshotSpan enclosingSpan = PkgDefTokenTagger.GetContinuedLines(pairSpan);

            string lineText = enclosingSpan.GetText();

            // start after the open char
            int offset = startPoint.Position - enclosingSpan.Start.Position + 1;

            // walk the rest of the 'line'
            while (offset < lineText.Length)
            {
                char currentChar = lineText[offset];
                if (currentChar == close) // found the close character
                {
                    pairSpan = new SnapshotSpan(startPoint.Snapshot, enclosingSpan.Start.Position + offset, 1);
                    return true;
                }
                offset++;
            }

            return false;
        }

        private static bool FindMatchingOpenChar(SnapshotPoint startPoint, char open, char close, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            SnapshotSpan enclosingSpan = PkgDefTokenTagger.GetContinuedLines(pairSpan);

            string lineText = enclosingSpan.GetText();

            // start to the left of the close char
            int offset = startPoint.Position - enclosingSpan.Start.Position - 1;

            // should not happen, but if the offset is negative, we're done
            if (offset < 0)
            {
                return false;
            }

            // walk to the beginning of the 'line'
            while (offset >= 0)
            {
                char currentChar = lineText[offset];

                if (currentChar == open)
                {
                    pairSpan = new SnapshotSpan(startPoint.Snapshot, enclosingSpan.Start.Position + offset, 1); //we just want the character itself
                    return true;
                }
                offset--;
            }
            return false;
        }

        private static bool FindMatchingOpenQuote(SnapshotPoint startPoint, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            SnapshotSpan enclosingSpan = PkgDefTokenTagger.GetContinuedLines(pairSpan);

            string lineText = enclosingSpan.GetText();

            int posNameStart, posNameEnd, posValueStart, posValueEnd;

            if (FindNameValueQuotes(out posNameStart, out posNameEnd, out posValueStart, out posValueEnd, ref lineText))
            {
                if (startPoint.Position == enclosingSpan.Start + posNameEnd)
                {
                    pairSpan = new SnapshotSpan(startPoint.Snapshot, enclosingSpan.Start.Position + posNameStart, 1);
                    return true;
                }
                if (startPoint.Position == enclosingSpan.Start + posValueEnd)
                {
                    pairSpan = new SnapshotSpan(startPoint.Snapshot, enclosingSpan.Start.Position + posValueStart, 1);
                    return true;
                }
            }
            return false;
        }

        private static bool FindMatchingCloseQuote(SnapshotPoint startPoint, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            SnapshotSpan enclosingSpan = PkgDefTokenTagger.GetContinuedLines(pairSpan);

            string lineText = enclosingSpan.GetText();

            int posNameStart, posNameEnd, posValueStart, posValueEnd;

            if (FindNameValueQuotes(out posNameStart, out posNameEnd, out posValueStart, out posValueEnd, ref lineText))
            {
                if (startPoint.Position == enclosingSpan.Start + posNameStart)
                {
                    pairSpan = new SnapshotSpan(startPoint.Snapshot, enclosingSpan.Start.Position + posNameEnd, 1);
                    return true;
                }
                if (startPoint.Position == enclosingSpan.Start + posValueStart)
                {
                    pairSpan = new SnapshotSpan(startPoint.Snapshot, enclosingSpan.Start.Position + posValueEnd, 1);
                    return true;
                }
            }
            return false;
        }

        private static bool FindNameValueQuotes(out int posNameStart, out int posNameEnd, out int posValueStart, out int posValueEnd, ref string text)
        {
            posNameStart = -1;
            posNameEnd = -1;
            posValueStart = -1;
            posValueEnd = -1;

            Group textAfterWhiteSpace = PkgDefTokenTagger.SkipWhiteSpace(text);
            int posEquals = -1;

            if (textAfterWhiteSpace.Value[0] == '@')
            {
                posEquals = text.IndexOf('=', textAfterWhiteSpace.Index + 1);
            }
            else if (textAfterWhiteSpace.Value[0] == '"')
            {
                posNameStart = textAfterWhiteSpace.Index;
                posNameEnd = text.IndexOf('"', posNameStart + 1);
                if (posNameEnd > posNameStart)
                {
                    posEquals = text.IndexOf('=', posNameEnd + 1);
                }
            }

            if (posEquals > -1)
            {
                Group textAfterEquals = PkgDefTokenTagger.SkipWhiteSpace(text.Substring(posEquals + 1));
                if (textAfterEquals != null)
                {
                    int posValue = posEquals + textAfterEquals.Index + 1;
                    if (textAfterEquals.Value.StartsWith(PkgDefTokenStrings.StringExpandSzPrefix))
                    {
                        posValue += PkgDefTokenStrings.StringExpandSzPrefix.Length - 1; // remove one char pos for "
                    }
                    if (text[posValue] == '"')
                    {
                        int posLastQuote = text.LastIndexOf('"');
                        if (posLastQuote > posValue)
                        {
                            posValueStart = posValue;
                            posValueEnd = posLastQuote;
                        }
                    }
                }
            }
            return (posNameStart > -1) || (posValueStart > -1);
        }
    }
}
