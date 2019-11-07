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

﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Language.Spellchecker
{
    /// <summary>
    /// Spell checking provider based on WPF spell checker
    /// </summary>
    [Export(typeof(ISpellingDictionaryService))]
    internal class SpellingDictionaryService : ISpellingDictionaryService
    {
        #region Private data
        private SortedSet<string> _ignoreWords = new SortedSet<string>();
        private string _ignoreWordsFile;
        #endregion

        #region Cached span
        class CachedSpan
        {
            public CachedSpan(int start, int length, IEnumerable<string> suggestions)
            {
                Start = start;
                Length = length;
                Suggestions = suggestions;
            }

            public int Start { get; private set; }
            public int Length { get; private set; }
            public IEnumerable<string> Suggestions { get; private set; }
        }

        class CachedSpanList : List<CachedSpan>
        {
            public CachedSpanList()
            {
                HitCount = 0;
            }
            public uint HitCount { get; set; }
        }

        class SpanCache
        {
            ulong m_cleanupCount = 0;
            Dictionary<string, CachedSpanList> m_spans = new Dictionary<string, CachedSpanList>();
            public SpanCache()
            {
                MaxCacheCount = 10000;
                CleanupRate = MaxCacheCount * 10;
            }

            public bool TryGetValue(string key, out CachedSpanList value)
            {
                bool result = m_spans.TryGetValue(key, out value);
                if (result) value.HitCount++;
                return result;
            }


            public CachedSpanList this[string key]
            {
                get
                {
                    CachedSpanList value = m_spans[key];
                    value.HitCount++;
                    return value;
                }
                set
                {
                    m_spans[key] = value;
                    CleanupCache();
                }
            }

            public void Clear()
            {
                m_spans.Clear();
            }

            public uint MaxCacheCount { get; set; }
            public uint CleanupRate { get; set; }

            private void CleanupCache()
            {
                if (++m_cleanupCount > CleanupRate)
                {
                    m_cleanupCount = 0;
                    if (m_spans.Count > MaxCacheCount)
                    {
                        uint removedStep = ((uint)(m_spans.Count)) / (MaxCacheCount / 2);
                        uint index = 0;
                        foreach (var item in m_spans)
                        {
                            if (item.Value.HitCount < 2 || index % removedStep != 0)
                            {
                                m_spans.Remove(item.Key);
                            }
                            index++;
                            removedStep = (removedStep + 1);
                        }
                    }

                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for SpellingDictionaryService
        /// </summary>
        public SpellingDictionaryService()
        {
            string localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\VisualStudio\10.0\SpellChecker");
            if (!Directory.Exists(localFolder))
            {
                Directory.CreateDirectory(localFolder);
            }
            _ignoreWordsFile = Path.Combine(localFolder, "Dictionary.txt");

            LoadIgnoreDictionary();
        }
        #endregion

        #region ISpellingDictionaryService

        /// <summary>
        /// Adds given word to the dictionary.
        /// </summary>
        /// <param name="word">The word to add to the dictionary.</param>
        public void AddWordToDictionary(string word)
        {
            if (!string.IsNullOrEmpty(word) && !_ignoreWords.Contains(word))
            {
                lock (_ignoreWords)
                    _ignoreWords.Add(word);

                // Add this word to the dictionary file.
                using (StreamWriter writer = new StreamWriter(_ignoreWordsFile, true))
                {
                    writer.WriteLine(word);
                }

                // Notify listeners.
                RaiseSpellingChangedEvent(word);
            }
        }

        public bool IsWordInDictionary(string word)
        {
            lock (_ignoreWords)
                return _ignoreWords.Contains(word);
        }

        public event EventHandler<SpellingEventArgs> DictionaryUpdated;

        #endregion

        #region Helpers

        void RaiseSpellingChangedEvent(string word)
        {
            var temp = DictionaryUpdated;
            if (temp != null)
                DictionaryUpdated(this, new SpellingEventArgs(word));
        }

        private void LoadIgnoreDictionary()
        {
            if (File.Exists(_ignoreWordsFile))
            {
                _ignoreWords.Clear();
                using (StreamReader reader = new StreamReader(_ignoreWordsFile))
                {
                    string word;
                    while (!string.IsNullOrEmpty((word = reader.ReadLine())))
                    {
                        _ignoreWords.Add(word);
                    }
                }
            }
        }
        #endregion
    }
}
