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
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Language.Spellchecker
{
    public class SpellingEventArgs : EventArgs
    {
        public SpellingEventArgs(string word)
        {
            this.Word = word;
        }

        /// <summary>
        /// Word placed in the Dictionary.
        /// </summary>
        public string Word { get; private set; }
    }

    public interface ISpellingDictionaryService
    {
        /// <summary>
        /// Add the given word to the dictionary, so that it will no longer show up as an
        /// incorrect spelling.
        /// </summary>
        /// <param name="word">The word to add to the dictionary.</param>
        void AddWordToDictionary(string word);

        /// <summary>
        /// Check the ignore dictionary for the given word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        bool IsWordInDictionary(string word);

        /// <summary>
        /// Raised when a new word is added to the dictionary, with the word
        /// that was added.
        /// </summary>
        event EventHandler<SpellingEventArgs> DictionaryUpdated;
    }
}
