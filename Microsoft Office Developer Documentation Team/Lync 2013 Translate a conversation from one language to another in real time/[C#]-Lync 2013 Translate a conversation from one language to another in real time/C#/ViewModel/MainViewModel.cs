/*=====================================================================
  This file is part of the Microsoft Unified Communications Code Samples.

  Copyright (C) 2012 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.IO.IsolatedStorage;

using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{
    /// <summary>
    /// Controls the visual behavior and content of the UI.
    /// </summary>
    public class MainViewModel
    {
        // Creates an instance of IsolatedStorageSettings used to store the preferred languages
        private IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
        private const string TargetLanguageKey = "TargetLanguage";
        private const string SourceLanguageKey = "SourceLanguage";

        public MainViewModel()
        {
            Languages = new ObservableCollection<Language>();
            MessageHistory = new ObservableCollection<MessageLine>();
        }

        /// <summary>
        /// Stores the in-memory history of the conversation.
        /// </summary>
        public ObservableCollection<MessageLine> MessageHistory { get; set; }

        /// <summary>
        /// Stores the list of available languages.
        /// </summary>
        public ObservableCollection<Language> Languages { get; set; }


        /// <summary>
        /// Stores the source language of the translation as selected by the user.
        /// </summary>
        public Language SourceLanguage 
        { 
            get 
            {
                //for the first run, save the default language
                if (!userSettings.Contains(SourceLanguageKey))
                {
                    userSettings[SourceLanguageKey] = Language.DefaultSource;
                }

                //return the stored language
                return userSettings[SourceLanguageKey] as Language;
            }

            set
            {
                //saves the new language
                userSettings[SourceLanguageKey] = value;
                userSettings.Save();
            }
        }

        /// <summary>
        /// Stores the target language of the translation as selected by the user.
        /// </summary>
        public Language TargetLanguage
        {
            get
            {
                //for the first run, save the default language
                if (!userSettings.Contains(TargetLanguageKey))
                {
                    userSettings[TargetLanguageKey] = Language.DefaultTarget;
                }

                //return the stored language
                return userSettings[TargetLanguageKey] as Language;
            }

            set
            {
                //saves the new language
                userSettings[TargetLanguageKey] = value;
                userSettings.Save();
            }
        }
    }
}
