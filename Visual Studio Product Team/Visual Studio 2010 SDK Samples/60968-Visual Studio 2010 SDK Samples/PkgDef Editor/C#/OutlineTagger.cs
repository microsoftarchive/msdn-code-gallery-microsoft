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
using System.Windows.Threading;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace PkgDefLanguage
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("pkgdef")]

    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate() { return new OutlineTagger(buffer) as ITagger<T>; };
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
        } 
    }

    /// <summary>
    /// Data about one outline region
    /// </summary>
    internal class Region
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartOffset { get; set; }
        public string HoverText { get; set; }
        public string Label { get; set; }
    }

    /// <summary>
    /// Finds the regions in a pkgdef file that are defined by the sections (keys)
    /// </summary>
    internal sealed class OutlineTagger : ITagger<IOutliningRegionTag>
    {
        static string ellipsis = "...";    //the characters that are displayed when the region is collapsed

        static Regex matchKey = new Regex("^\\s*(\\[[^\\]]*)");

        static int MaxHiddenLines = 15;

        ITextBuffer _buffer;

        ITextSnapshot _snapshot;

        List<Region> _regions;

        public OutlineTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _snapshot = buffer.CurrentSnapshot;
            _regions = new List<Region>();
            
            ReparseFile(null, EventArgs.Empty);

            // listen for changes to the buffer, but don't process until the user stops typing
            BufferIdleEventUtil.AddBufferIdleEventListener(_buffer, ReparseFile);
        }

        public void Dispose()
        {
            BufferIdleEventUtil.RemoveBufferIdleEventListener(_buffer, ReparseFile);
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            List<Region> currentRegions = _regions;
            ITextSnapshot currentSnapshot = _snapshot;

            // create a new SnapshotSpan for the entire region encompassed by the span collection
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;

            // return outline tags for any regions that fall within that span
            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    //the region starts at the beginning of the "[", and goes until the *end* of the line that contains the "]".
                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End),
                        new OutliningRegionTag(false, false, region.Label, region.HoverText));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Find all of the outline sections in the snapshot
        /// </summary>
        private static List<Region> ParseOutlineSections(ITextSnapshot newSnapshot)
        {
            List<Region> newRegions = new List<Region>();

            Region currentRegion = null;
            int currentRegionLineCount = 0;
            string lineSep = "";
            SnapshotSpan currentFullSpan = new SnapshotSpan();

            foreach (ITextSnapshotLine line in newSnapshot.Lines)
            {
                if (!currentFullSpan.IsEmpty && currentFullSpan.Contains(line.Start))
                {
                    continue; // already processed, go get next one
                }

                currentFullSpan = PkgDefTokenTagger.GetContinuedLines(new SnapshotSpan(line.Start, line.End));
                int currentFirstLine = currentFullSpan.Start.GetContainingLine().LineNumber;
                int currentEndLine = currentFullSpan.End.GetContainingLine().LineNumber;

                string text = currentFullSpan.GetText();

                MatchCollection matches = matchKey.Matches(text);
                if ((matches.Count == 1) && (matches[0].Groups.Count == 2))
                {
                    // close out the previous Region
                    if (currentRegion != null)
                    {
                        newRegions.Add(currentRegion);
                    }

                    // and start a new one
                    currentRegionLineCount = 0;
                    lineSep = "";
                    currentRegion = new Region()
                    {
                        StartLine = currentEndLine,
                        StartOffset = currentFullSpan.End.GetContainingLine().Length,
                        EndLine = currentEndLine,
                        Label = ellipsis,
                        HoverText = "" // don't include the key in hover text
                    };
                }
                else if ((currentRegion != null) && (text.Trim() != ""))
                {
                    // accumulate non-blank lines for the hover text
                    if (currentRegionLineCount < MaxHiddenLines)
                    {
                        currentRegion.HoverText += lineSep;
                        currentRegion.HoverText += text;
                        currentRegionLineCount += currentEndLine - currentFirstLine + 1;
                        lineSep = line.GetLineBreakText();
                    }
                    currentRegion.EndLine = currentEndLine;
                }
            }
            // close out the final Region
            if (currentRegion != null)
            {
                newRegions.Add(currentRegion);
                currentRegion = null;
            }

            return newRegions;
        }

        /// <summary>
        /// Find all of the outline regions in the document (snapshot) and notify
        /// listeners of any that changed
        /// </summary>
        void ReparseFile(object sender, EventArgs args)
        {
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;

            // get all of the outline regions in the snapshot
            List<Region> newRegions = ParseOutlineSections(snapshot);

            // determine the changed span, and send a changed event with the new spans
            List<SnapshotSpan> oldSpans =
                new List<SnapshotSpan>(_regions.Select(r => AsSnapshotSpan(r, _snapshot)
                    .TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive)));

            List<SnapshotSpan> newSpans =
                new List<SnapshotSpan>(newRegions.Select(r => AsSnapshotSpan(r, snapshot)));

            NormalizedSnapshotSpanCollection oldSpanCollection = new NormalizedSnapshotSpanCollection(oldSpans);
            NormalizedSnapshotSpanCollection newSpanCollection = new NormalizedSnapshotSpanCollection(newSpans);

            NormalizedSnapshotSpanCollection difference = SymmetricDifference(oldSpanCollection, newSpanCollection);

            // save the new baseline
            _snapshot = snapshot;
            _regions = newRegions;

            foreach (var span in difference)
            {
                var temp = TagsChanged;
                if (temp != null)
                    temp(this, new SnapshotSpanEventArgs(span));
            }
        }

        NormalizedSnapshotSpanCollection SymmetricDifference(NormalizedSnapshotSpanCollection first, NormalizedSnapshotSpanCollection second)
        {
            return NormalizedSnapshotSpanCollection.Union(
                NormalizedSnapshotSpanCollection.Difference(first, second),
                NormalizedSnapshotSpanCollection.Difference(second, first));
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine) ? startLine
                 : snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
        }
    }
}
