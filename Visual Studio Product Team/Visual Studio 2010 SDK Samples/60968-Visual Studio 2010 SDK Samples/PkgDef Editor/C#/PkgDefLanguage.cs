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
using System.Globalization;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace PkgDefLanguage
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("pkgdef")]
    [TagType(typeof(PkgDefTokenTag))]
    internal sealed class PkgDefTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate() { return new PkgDefTokenTagger(buffer) as ITagger<T>; };
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
        }
    }

    public class PkgDefTokenTag : ITag 
    {
        public PkgDefLanguageTokens type { get; private set; }

        public PkgDefTokenTag(PkgDefLanguageTokens type)
        {
            this.type = type;
        }
    }

    public class PkgDefErrorTag : PkgDefTokenTag
    {
        public string message { get; private set; }

        public PkgDefErrorTag(string message)
            : base(PkgDefLanguageTokens.Error)
        {
            this.message = message;
        }
    }

    /// <summary>
    /// PkgDefTokenSpan records a result of parsing within a string. It's not converted to an ITagSpan
    /// until the last minute, to make it easier to adjust
    /// </summary>
    internal struct PkgDefTokenSpan
    {
        private PkgDefLanguageTokens _tokenType;
        private Span _span;
        private string _errorText;

        public PkgDefTokenSpan(Span span, PkgDefLanguageTokens type) { _tokenType = type; _span = span; _errorText = ""; }
        public PkgDefTokenSpan(Span span, string errorText = "") { _tokenType = PkgDefLanguageTokens.Error; _span = span; _errorText = errorText; }
        public PkgDefTokenSpan(Span span, PkgDefLanguageTokens type, string errorText = "") { _tokenType = type; _span = span; _errorText = errorText; }

        /// <summary>
        /// re-align a span to an offset within an outer string
        /// </summary>
        public PkgDefTokenSpan OffsetFrom(int offset)
        {
            return new PkgDefTokenSpan(new Span(offset + _span.Start, _span.Length), _tokenType, _errorText);
        }

        /// <summary>
        /// convert to the form being returned by GetTags()
        /// </summary>
        public ITagSpan<PkgDefTokenTag> AsTagSpan(SnapshotPoint start)
        {
            SnapshotSpan snapshot = new SnapshotSpan(start + _span.Start, _span.Length);
            if (_errorText == "")
            {
                return new TagSpan<PkgDefTokenTag>(snapshot, new PkgDefTokenTag(_tokenType));
            }
            return new TagSpan<PkgDefTokenTag>(snapshot, new PkgDefErrorTag(_errorText));
        }
    }

    /// <summary>
    /// PkgDefTokenTagger is the core parser for the pkgdef 'language'. It handles continuation lines
    /// and finds both language elements and errors within the given SnapshotSpan(s)
    /// </summary>
    internal sealed class PkgDefTokenTagger : ITagger<PkgDefTokenTag>
    {
        ITextBuffer _buffer;

        static Regex _skipWhiteSpace = new Regex("\\s*(.*)");

        static private Dictionary<string, string> _validTokens = new Dictionary<string, string>()
        {
            { "$RootKey$", "Registry path to application root" },
            { "$HivelessRootKey$", "Registry path to application root (minus HKEY_CURRENT_USER)" },
            { "$RootFolder$", "File path to the root of the application installation" },
            { "$ShellFolder$", "File path to the root of the Visual Studio installation" },
            { "$ApplicationExtensionsFolder$", "File path to the default location for application extensions" },
            { "$UserExtensionsRootFolder$", "File path to the root for user installed extensions" },
            { "$BaseInstallDir$", "File path to the root of the Visual Studio installation" },
            { "$MyDocuments$", "File path to the user's Documents folder" },
            { "$WinDir$", "File path to the Windows installation" },
            { "$System$", "File path to the System32 folder of the Windows installation" },
            { "$PackageFolder$", "File path to the folder containing the pkgdef file being loaded" },
            { "$ProgramFiles$", "File path to the parent folder for applications" },
            { "$CommonFiles$", "File path to the common files folder for the application" },
            { "$AppName$", "Name of the application (only defined for Visual Studio Shell (Isolated) applications)" },
            { "$AppDataLocalFolder$", "File path to the application-specific sub-folder of the user's local application data" },
            { "$Initialization$", "Initialization section of the master pkgdef file" }
        };

        internal PkgDefTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;

            _buffer.Changed += new EventHandler<TextContentChangedEventArgs>(_buffer_Changed);
        }

        /// <summary>
        /// When the buffer changes, check to see if any of the edits were in a paragraph with multi-line tokens.
        /// If so, we need to send out a classification changed event for those paragraphs.
        /// </summary>
        void _buffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            var temp = this.TagsChanged;
            if (temp != null)
            {
                foreach (var change in e.Changes)
                {
                    Span oldLineSpan = GetContinuedLinesSpan(new SnapshotSpan(e.Before, change.OldSpan));
                    Span newLineSpan = GetContinuedLinesSpan(new SnapshotSpan(e.After, change.NewSpan));

                    // single lines are 0 length spans (start and end on same line)
                    // if either the old or new span was more than one line, we need to process the larger range
                    if ((oldLineSpan.Length > 0) || (newLineSpan.Length > 0))
                    {
                        SnapshotPoint startPoint = e.After.GetLineFromLineNumber(Math.Min(oldLineSpan.Start, newLineSpan.Start)).Start;
                        SnapshotPoint endPoint = e.After.GetLineFromLineNumber(Math.Min(e.After.LineCount - 1, Math.Max(oldLineSpan.End, newLineSpan.End))).End;
                        SnapshotSpan expandedSpan = new SnapshotSpan(startPoint, endPoint);

                        temp(this, new SnapshotSpanEventArgs(expandedSpan));
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        
        static public Dictionary<string, string> ValidTokens { get { return _validTokens; } }

        /// <summary>
        /// Parse the given span(s) and return all the tags that intersect the specified spans.
        /// pkgdef is a line-oriented format, so we process the span a line at a time, grouping
        /// lines with continuation characters together as a single virtual line, and then parse
        /// each line based on the first non-blank character
        /// </summary>
        /// <param name="spans">ordered collection of non-overlapping spans</param>
        /// <returns>unordered enumeration of tags</returns>
        public IEnumerable<ITagSpan<PkgDefTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // GetIntersectingLineSpans enumerates the virtual lines in the span collection
            foreach (SnapshotSpan curSpan in GetIntersectingLineSpans(spans))
            {
                // convert to a string (minus any backslashes and line separators)
                string parseableText = CleanContinuationLines(curSpan);

                Group textAfterWhiteSpace = SkipWhiteSpace(parseableText);
                if (textAfterWhiteSpace != null)
                {
                    switch (textAfterWhiteSpace.Value[0])
                    {
                        case '/':
                            if ((textAfterWhiteSpace.Length > 1) && (textAfterWhiteSpace.Value[1] == '/'))
                                goto case ';';
                            goto default;

                        case ';':
                            yield return new TagSpan<PkgDefTokenTag>(curSpan, new PkgDefTokenTag(PkgDefLanguageTokens.Comment));
                            break;

                        case '[':
                            foreach (PkgDefTokenSpan span in ParseKey(textAfterWhiteSpace.Index, ref parseableText))
                            {
                                yield return span.AsTagSpan(curSpan.Start);
                            }
                            break;

                        case '@':
                        case '"':
                            foreach (PkgDefTokenSpan span in ParseNameValue(textAfterWhiteSpace.Index, ref parseableText))
                            {
                                yield return span.AsTagSpan(curSpan.Start);
                            }
                            break;

                        default:
                            yield return new TagSpan<PkgDefTokenTag>(curSpan, new PkgDefErrorTag("Name not in quotes and not '@'"));
                            break;
                    }
                }
            }
        }

        #region Parser methods (private)

        /// <summary>
        /// Parse a line that looks like a key or section
        /// </summary>
        /// <param name="posOpeningBracket">position of the opening bracket (skipping the white space)</param>
        /// <param name="text">entire virtual line to parse</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> ParseKey(int posOpeningBracket, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();
            
            // find the closing bracket - note this matches the behavior of CPkgDefFileReader, which does not allow embedded closing brackets
            int posClosingBracket = text.IndexOf(']', posOpeningBracket + 1);

            if ((posClosingBracket < 0) && (text.Length > posOpeningBracket + 1))
            {
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstring(posOpeningBracket + 1, text.Length - 1), "Section header: missing closing bracket"));
            }

            if ((posClosingBracket > 0) && (posClosingBracket < text.Length - 1))
            {
                string tail = text.Substring(posClosingBracket + 1);
                if (tail.Trim().Length > 0)
                {
                    tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstring(posClosingBracket + 1, text.Length - 1), "Section header: additional characters after closing bracket"));
                }
            }
            if (posClosingBracket - 1 > 0)
            {
                tokens.AddRange(GetStringSpans(PkgDefLanguageTokens.Key, posOpeningBracket + 1, posClosingBracket - 1, ref text));
            }
            return tokens;
        }

        /// <summary>
        /// Parse a line that looks like a name=value
        /// </summary>
        /// <param name="posValueName">position of the opening quote or '@'</param>
        /// <param name="text">entire virtual line to parse</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> ParseNameValue(int posValueName, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            int posEquals = -1;

            if (text[posValueName] == '@')
            {
                if (text.Length > 1)
                    posEquals = text.IndexOf('=', posValueName + 1);
            }
            else
            {
                int posCloseQuote = text.IndexOf('"', posValueName + 1);
                if (posCloseQuote > posValueName)
                {
                    posEquals = text.IndexOf('=', posCloseQuote + 1);
                }
            }
            if (posEquals < posValueName)
            {
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(0, text.Length), "Name value: no equals sign"));
            }
            else
            {
                tokens.AddRange(ParseName(posValueName, posEquals, ref text));

                tokens.AddRange(ParseValue(posEquals, ref text));
            }

            return tokens;
        }

        /// <summary>
        /// Find GUIDs and string-substitution tokens within a string and return tagged spans for all
        /// </summary>
        /// <param name="tokenType">type of the original token for the span</param>
        /// <param name="startPos">first character index</param>
        /// <param name="endPos">last character index</param>
        /// <param name="text">entire virtual line</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> GetStringSpans(PkgDefLanguageTokens tokenType, int startPos, int endPos, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();
            if (endPos > startPos)
            {
                string textToScan = text.Substring(startPos, endPos - startPos + 1);

                foreach (PkgDefTokenSpan span in GetGuidTags(ref textToScan))
                    tokens.Add(span.OffsetFrom(startPos));

                foreach (PkgDefTokenSpan span in GetTokenTags(ref textToScan))
                    tokens.Add(span.OffsetFrom(startPos));

                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstring(startPos, endPos), tokenType));
            }
            return tokens;
        }

        /// <summary>
        /// Find any GUIDs embedded in the string
        /// </summary>
        /// <param name="text">string to analyze</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> GetGuidTags(ref string text)
        {
            Regex matchGUID = new Regex("(\\{([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\})");

            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();
            foreach (Match match in matchGUID.Matches(text))
            {
                //Try and add the capture groups, if there are any
                for (int j = 1; j < match.Groups.Count; j++)
                {
                    tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(match.Groups[j].Index, match.Groups[j].Length), PkgDefLanguageTokens.Guid));
                }
            }
            return tokens;
        }

        /// <summary>
        /// Find any string-substitution tokens embedded in the string, identifying those that have
        /// the right format, but do not match the list of valid tokens
        /// </summary>
        /// <param name="text">string to analyze</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> GetTokenTags(ref string text)
        {
            Regex matchToken = new Regex("(\\$.+?\\$)");
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            foreach (Match match in matchToken.Matches(text))
            {
                //Try and add the capture groups, if there are any
                for (int j = 1; j < match.Groups.Count; j++)
                {
                    // validate token against list
                    Span tokenSpan = GetSpanOfSubstringLength(match.Groups[j].Index, match.Groups[j].Length);
                    tokens.Add(new PkgDefTokenSpan(tokenSpan, PkgDefLanguageTokens.Token));

                    if (!ValidTokens.Keys.Contains(match.Groups[j].Value))
                    {
                        tokens.Add(new PkgDefTokenSpan(tokenSpan, "Unrecognized string substitution"));
                    }
                }
            }
            return tokens;
        }

        /// <summary>
        /// Parse a value name, which must be a single @ or within quotes
        /// </summary>
        /// <param name="posValueName">position where the name starts</param>
        /// <param name="posEquals">position of equals</param>
        /// <param name="text">entire virtual line to parse</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> ParseName(int posValueName, int posEquals, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            string name = text.Substring(0, posEquals).Trim();

            if (name != "@")
            {
                if (name[0] == '"' && name[name.Length - 1] == '"')
                {
                    tokens.AddRange(GetStringSpans(PkgDefLanguageTokens.ValueName, posValueName + 1, posValueName + name.Length - 2, ref text));
                }
                else
                {
                    tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValueName, name.Length), "Name not in quotes and not '@'"));
                }
            }
            return tokens;
        }

        /// <summary>
        /// Parse the value to the right of an equals sign by determining its type
        /// and then parsing that type
        /// </summary>
        /// <param name="posEquals">position of the equals sign</param>
        /// <param name="text">entire virtual line</param>
        /// <returns>unordered enumeration of tokens</returns>
        private IEnumerable<PkgDefTokenSpan> ParseValue(int posEquals, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            Group textAfterEquals = SkipWhiteSpace(text.Substring(posEquals + 1));

            if (textAfterEquals != null)
            {
                int posValue = posEquals + textAfterEquals.Index + 1;
                switch (textAfterEquals.Value[0])
                {
                    case 'e':
                        if (textAfterEquals.Value.StartsWith(PkgDefTokenStrings.StringExpandSzPrefix))
                        {
                            tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValue, 2), PkgDefLanguageTokens.ValueType));
                            posValue += PkgDefTokenStrings.StringExpandSzPrefix.Length - 1;  // skip over prefix, but subtract out the "
                            goto case '"';
                        }
                        goto default;

                    case '"':
                        tokens.AddRange(ParseStringValue(posValue, ref text));
                        break;

                    case 'h':
                        tokens.AddRange(ParseHexValue(posValue, ref text));
                        break;

                    case 'd':
                    case 'q':
                        tokens.AddRange(ParseNumericValue(posValue, ref text));
                        break;

                    default:
                        tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posEquals + 1, text.Length - posEquals - 1), "Value format not recognized"));
                        break;
                }
            }
            else
            {
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posEquals + 1, text.Length - posEquals - 1), "No value given"));
                }
            return tokens;
        }

        /// <summary>
        /// Parse a simple numeric value
        /// </summary>
        private IEnumerable<PkgDefTokenSpan> ParseNumericValue(int posValue, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();
            
            string valueString = text.Substring(posValue);

            if (valueString.StartsWith(PkgDefTokenStrings.BinaryDWordPrefix) ||
                valueString.StartsWith(PkgDefTokenStrings.BinaryQWordPrefix))
            {
                int posColon = valueString.IndexOf(':');
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValue, posColon), PkgDefLanguageTokens.ValueType));
                Span numberSpan = GetSpanOfSubstringLength(posValue + posColon + 1, text.Length - (posValue + posColon + 1));
                bool isValidNumber = false;
                if (numberSpan.Length > 0)
                {
                    string numberString = text.Substring(numberSpan.Start);

                    //NOTE: HexSpecifier is actually NOT desired
                    //if (!numberSpan.GetText().StartsWith(PkgDefTokenStrings.HexSpecifier))
                    //{
                    //    yield return new TagSpan<PkgDefTokenTag>(numberSpan, new PkgDefErrorTag("Missing hex specifier ('0x')"));
                    //}

                    if (valueString.StartsWith(PkgDefTokenStrings.BinaryDWordPrefix))
                    {
                        int result;
                        isValidNumber = Int32.TryParse(numberString,
                                           System.Globalization.NumberStyles.AllowHexSpecifier,
                                           CultureInfo.InvariantCulture.NumberFormat, out result);
                    }
                    else
                    {
                        Int64 result;
                        isValidNumber = Int64.TryParse(numberString,
                                           System.Globalization.NumberStyles.AllowHexSpecifier,
                                           CultureInfo.InvariantCulture.NumberFormat, out result);
                    }
                }
                if (isValidNumber)
                {
                    tokens.Add(new PkgDefTokenSpan(numberSpan, PkgDefLanguageTokens.ValueBinary));
                }
                else
                {
                    tokens.Add(new PkgDefTokenSpan(numberSpan, "Invalid number format"));
                }
            }
            else
            {
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValue, text.Length - posValue), "Unrecognized data type"));
            }
            return tokens;
        }

        /// <summary>
        /// Parse a hex value, which is actually a list of hex bytes
        /// </summary>
        private IEnumerable<PkgDefTokenSpan> ParseHexValue(int posValue, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            string valueString = text.Substring(posValue);

            int posColon = valueString.IndexOf(':');
            if (posColon > 2)
            {
                string typeName = valueString.Substring(0, posColon + 1);
                if ((typeName == PkgDefTokenStrings.HexDWordBinaryPrefix) ||
                    (typeName == PkgDefTokenStrings.HexExpandSzBinaryPrefix) ||
                    (typeName == PkgDefTokenStrings.HexMultiSzPrefix1) ||
                    (typeName == PkgDefTokenStrings.HexMultiSzPrefix2) ||
                    (typeName == PkgDefTokenStrings.HexBinaryPrefix))
                {
                    tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValue, typeName.Length - 1), PkgDefLanguageTokens.ValueType));
                    tokens.AddRange(GetHexTags(posValue + typeName.Length, ref text));
                }
            }
            else
            {
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValue, text.Length - posValue), "Unrecognized data type"));
            }
            return tokens;
        }

        /// <summary>
        /// Parse the comma-separated list of hex bytes
        /// </summary>
        private IEnumerable<PkgDefTokenSpan> GetHexTags(int startPos, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            int posHex = 0;
            string hexString = text.Substring(startPos);

            while (posHex < hexString.Length)
            {
                // find next hex byte (skips over commas, white space, and line continuations)
                while ((posHex < hexString.Length) && (!Uri.IsHexDigit(hexString[posHex])))
                {
                    posHex++;
                }
                if (posHex == hexString.Length)
                {
                    break; // at the end
                }
                if (posHex + 1 >= hexString.Length || (!Uri.IsHexDigit(hexString[posHex + 1])))
                {
                    // expected another hex char
                    tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(startPos + posHex, 1), "Expected another hex digit"));
                    break;
                }

                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(startPos + posHex, 2), PkgDefLanguageTokens.ValueBinary));

                posHex += 2; // skip past this pair
            }
            return tokens;
        }

        /// <summary>
        /// Parse a simple string value
        /// </summary>
        private IEnumerable<PkgDefTokenSpan> ParseStringValue(int posValue, ref string text)
        {
            List<PkgDefTokenSpan> tokens = new List<PkgDefTokenSpan>();

            int posCloseQuote = text.LastIndexOf('"');

            if (posCloseQuote <= posValue)
            {
                tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posValue, text.Length - posValue), "No closing quote found"));
            }
            else
            {
                if (posValue + posCloseQuote - 1 > 0)
                {
                    tokens.AddRange(GetStringSpans(PkgDefLanguageTokens.ValueString, posValue + 1, posCloseQuote - 1, ref text));
                }

                // check for extra stuff
                string stuff = text.Substring(posCloseQuote + 1).TrimEnd();
                if (stuff.Length > 0)
                {
                    tokens.Add(new PkgDefTokenSpan(GetSpanOfSubstringLength(posCloseQuote + 1, stuff.Length), "Extra characters found after value"));
                }
            }
            return tokens;
        }

        #endregion

        #region Helper methods (public)

        /// <summary>
        /// Expands a given span of lines (usually one) to a virtual line that accounts for
        /// continuation characters
        /// </summary>
        static public Span GetContinuedLinesSpan(SnapshotPoint start, SnapshotPoint end)
        {
            ITextSnapshot snapshot = start.Snapshot;

            int startLineNumber = start.GetContainingLine().LineNumber;
            int endLineNumber = (end <= start.GetContainingLine().EndIncludingLineBreak) ? startLineNumber : snapshot.GetLineNumberFromPosition(end);

            // Find the first line
            bool lineContinuedUp = (startLineNumber > 0) && snapshot.GetLineFromLineNumber(startLineNumber - 1).GetText().EndsWith("\\");
            while (lineContinuedUp && (startLineNumber > 0))
            {
                startLineNumber--;
                lineContinuedUp = (startLineNumber > 0) && snapshot.GetLineFromLineNumber(startLineNumber - 1).GetText().EndsWith("\\");
            }

            // Find the last line
            while ((endLineNumber < snapshot.LineCount - 1) && (snapshot.GetLineFromLineNumber(endLineNumber).GetText().EndsWith("\\")))
            {
                endLineNumber++;
            }

            return new Span(startLineNumber, endLineNumber - startLineNumber);
        }

        static public Span GetContinuedLinesSpan(SnapshotSpan span)
        {
            return GetContinuedLinesSpan(span.Start, span.End);
        }

        static public SnapshotSpan GetContinuedLines(ITextSnapshotLine line)
        {
            ITextSnapshot snapshot = line.Snapshot;

            Span lineSpan = GetContinuedLinesSpan(line.Start, line.End);

            // Generate a snapshot that spans all lines
            SnapshotPoint startPoint = snapshot.GetLineFromLineNumber(lineSpan.Start).Start;
            SnapshotPoint endPoint = snapshot.GetLineFromLineNumber(lineSpan.End).End;

            return new SnapshotSpan(startPoint, endPoint);
        }

        static public SnapshotSpan GetContinuedLines(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;

            Span lineSpan = GetContinuedLinesSpan(span);

            // Generate a snapshot that spans all lines
            SnapshotPoint startPoint = snapshot.GetLineFromLineNumber(lineSpan.Start).Start;
            SnapshotPoint endPoint = snapshot.GetLineFromLineNumber(lineSpan.End).End;

            return new SnapshotSpan(startPoint, endPoint);
        }

        /// <summary>
        /// Determine if the given paragraph of text contains any multi-line tokens.
        /// </summary>
        public static bool ContainsContinuationLines(string text)
        {
            return text.Contains("\\\r\n");
        }

        #endregion

        #region Helper methods (private)

        /// <summary>
        /// Get all sets of lines that intersect a <see cref="NormalizedSnapshotSpanCollection" />.
        /// </summary>
        /// <param name="spans">The normalized set of spans.</param>
        /// <returns>An enumeration of text snapshot spans.</returns>
        internal static IEnumerable<SnapshotSpan> GetIntersectingLineSpans(NormalizedSnapshotSpanCollection spans)
        {
            SnapshotSpan lastSpan = new SnapshotSpan();

            foreach (SnapshotSpan span in spans)
            {
                if (!lastSpan.IsEmpty && lastSpan.End >= span.End)
                    continue;

                // If we haven't yet returned the first set of lines for this span, do so
                if (lastSpan.IsEmpty || lastSpan.End <= span.Start)
                {
                    lastSpan = GetContinuedLines(span.Start.GetContainingLine());
                    yield return lastSpan;
                }

                // Cycle through the rest of the lines
                while (lastSpan.End < span.End && lastSpan.End.GetContainingLine().LineNumber < lastSpan.Snapshot.LineCount)
                {
                    lastSpan = GetContinuedLines(lastSpan.Snapshot.GetLineFromLineNumber(lastSpan.End.GetContainingLine().LineNumber + 1));
                    yield return lastSpan;
                }
            }
        }

        internal static Group SkipWhiteSpace(string sourceLine)
        {
            if (sourceLine.TrimEnd().Length == 0)
            {
                return null;
            }
            GroupCollection matches = _skipWhiteSpace.Match(sourceLine).Groups;
            if (matches.Count > 1)
            {
                return matches[1];
            }
            return null;
        }

        private Span GetSpanOfSubstring(int offset, int end)
        {
            return new Span(offset, end - offset + 1);
        }

        private Span GetSpanOfSubstringLength(int offset, int length)
        {
            return new Span(offset, length);
        }

        private SnapshotSpan GetSpanOfSubstring(SnapshotPoint start, int offset, int end)
        {
            return new SnapshotSpan(start + offset, start + end + 1);
        }

        private SnapshotSpan GetSpanOfSubstringLength(SnapshotPoint start, int offset, int length)
        {
            return new SnapshotSpan(start + offset, start + offset + length);
        }

        /// <summary>
        /// replaces the continuation character and the following line separator with plain white space
        /// without changing the length of the span
        /// </summary>
        private string CleanContinuationLines(SnapshotSpan lastFullSpan)
        {
            return lastFullSpan.GetText().Replace("\\\r\n", "   ");
        }
        #endregion

    }
}
