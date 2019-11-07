// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// indexedproperty.cs
using System;

public class Document
{
    // Type allowing the document to be viewed like an array of words:
    public class WordCollection
    {
        readonly Document document;  // The containing document

        internal WordCollection(Document d)
        {
           document = d;
        }

        // Helper function -- search character array "text", starting at
        // character "begin", for word number "wordCount." Returns false
        // if there are less than wordCount words. Sets "start" and
        // length" to the position and length of the word within text:
        private bool GetWord(char[] text, int begin, int wordCount, 
                                       out int start, out int length) 
        { 
            int end = text.Length;
            int count = 0;
            int inWord = -1;
            start = length = 0; 

            for (int i = begin; i <= end; ++i) 
            {
                bool isLetter = i < end && Char.IsLetterOrDigit(text[i]);

                if (inWord >= 0) 
                {
                    if (!isLetter) 
                    {
                        if (count++ == wordCount) 
                        {
                            start = inWord;
                            length = i - inWord;
                            return true;
                        }
                        inWord = -1;
                    }
                }
                else 
                {
                    if (isLetter)
                        inWord = i;
                }
            }
            return false;
        }

        // Indexer to get and set words of the containing document:
        public string this[int index] 
        {
            get 
            { 
                int start, length;
                if (GetWord(document.TextArray, 0, index, out start, 
                                                          out length))
                    return new string(document.TextArray, start, length);
                else
                    throw new IndexOutOfRangeException();
            }
            set 
            {
                int start, length;
                if (GetWord(document.TextArray, 0, index, out start, 
                                                         out length)) 
                {
                    // Replace the word at start/length with the 
                    // string "value":
                    if (length == value.Length) 
                    {
                        Array.Copy(value.ToCharArray(), 0, 
                                 document.TextArray, start, length);
                    }
                    else 
                    {
                        char[] newText = 
                            new char[document.TextArray.Length + 
                                           value.Length - length];
                        Array.Copy(document.TextArray, 0, newText, 
                                                        0, start);
                        Array.Copy(value.ToCharArray(), 0, newText, 
                                             start, value.Length);
                        Array.Copy(document.TextArray, start + length,
                                   newText, start + value.Length,
                                  document.TextArray.Length - start
                                                            - length);
                        document.TextArray = newText;
                    }
                }                    
                else
                    throw new IndexOutOfRangeException();
            }
        }

        // Get the count of words in the containing document:
        public int Count 
        {
            get 
            { 
                int count = 0, start = 0, length = 0;
                while (GetWord(document.TextArray, start + length, 0, 
                                              out start, out length))
                    ++count;
                return count; 
            }
        }
    }

    // Type allowing the document to be viewed like an "array" 
    // of characters:
    public class CharacterCollection
    {
        readonly Document document;  // The containing document

        internal CharacterCollection(Document d)
        {
          document = d; 
        }

        // Indexer to get and set characters in the containing document:
        public char this[int index] 
        {
            get 
            { 
                return document.TextArray[index]; 
            }
            set 
            { 
                document.TextArray[index] = value; 
            }
        }

        // Get the count of characters in the containing document:
        public int Count 
        {
            get 
            { 
                return document.TextArray.Length; 
            }
        }
    }

    // Because the types of the fields have indexers, 
    // these fields appear as "indexed properties":
    public WordCollection Words;
    public CharacterCollection Characters;

    private char[] TextArray;  // The text of the document. 

    public Document(string initialText)
    {
        TextArray = initialText.ToCharArray();
        Words = new WordCollection(this);
        Characters = new CharacterCollection(this);
    }

    public string Text 
    {
        get 
        { 
           return new string(TextArray); 
        }
    }
}

class Test
{
    static void Main()
    {
        Document d = new Document(
           "peter piper picked a peck of pickled peppers. How many pickled peppers did peter piper pick?"
        );

        // Change word "peter" to "penelope":
        for (int i = 0; i < d.Words.Count; ++i) 
        {
            if (d.Words[i] == "peter") 
                d.Words[i] = "penelope";
        }

        // Change character "p" to "P"
        for (int i = 0; i < d.Characters.Count; ++i) 
        {
            if (d.Characters[i] == 'p')
                d.Characters[i] = 'P';
        }
        
        Console.WriteLine(d.Text);
    }
}

