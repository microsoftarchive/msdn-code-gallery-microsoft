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
using System.Net;
using System.Collections.ObjectModel;
using System.Collections.Generic;

// =======================================================================
// Referencing classes auto-generated from the Bing API service reference
// See the link bellow for more details:
// http://msdn.microsoft.com/en-us/library/ff512388.aspx
// =======================================================================
using Microsoft.Lync.SDK.Samples.ConversationTranslator.BingTranslation;

namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{

    /// <summary>
    /// Called when the list of languages is received from the translation service.
    /// </summary>
    public delegate void LanguagesReceived(IList<Language> languages);

    /// <summary>
    /// Called when a translation result is received from the translation service.
    /// </summary>
    public delegate void TranslationReceived(MessageContext message);

    /// <summary>
    /// Called when there was an issue with the translation.
    /// </summary>
    public delegate void TranslationError(Exception ex, MessageContext context);


    /// <summary>
    /// Uses the Microsoft Translation service via WebServices to request:
    /// * The list of supported languages.
    /// * Language detection of a message.
    /// * Translation of a message.
    /// </summary>
    public class TranslationService
    {
        //An application ID is required by Bing APIs
        //In case you modify this source code, please obtain a new AppID at
        //http://www.bing.com/developer (Get an AppID section)
        private const string AppID = "12A4F25886A9CF0B9105C8B4B2934008A3210EC7";

        //language translation service
        private LanguageService languageService;

        /// <summary>
        /// Occurs when the list of languages is received.
        /// </summary>
        public event LanguagesReceived LanguagesReceived;

        /// <summary>
        /// Occurs when a translation result is received.
        /// </summary>
        public event TranslationReceived TranslationReceived;

        /// <summary>
        /// Occurs when a translation error occurs.
        /// </summary>
        public event TranslationError TranslationError;

        /// <summary>
        /// Creates the translation service. Initialization defferred to Start().
        /// </summary>
        public TranslationService()
        {
        }

        /// <summary>
        /// Starts the translator, which:
        /// 1. Connects to the translation services.
        /// 2. Updates the UI with the available languages.
        /// </summary>
        public void Start()
        {
            //connects to the translation service
            languageService = new LanguageServiceClient();            

            //requests the available language codes
            languageService.BeginGetLanguagesForTranslate(AppID, this.OnGetLanguagesResponse, null);
        }

        /// <summary>
        /// Called back when the language list arrives.
        /// </summary>
        private void OnGetLanguagesResponse(IAsyncResult result)
        {
            //gets the codes
            ObservableCollection<string> languageCodes = languageService.EndGetLanguagesForTranslate(result);

            //requests the language names (and passes the codes as the asyncronous context)
            languageService.BeginGetLanguageNames(AppID, "en", languageCodes, this.OnGetLanguageNamesResponse, languageCodes);
        }

        /// <summary>
        /// Called back when the language name list arrives.
        /// Assembles the array with the Language objects and notifies the UI.
        /// </summary>
        /// <param name="result"></param>
        private void OnGetLanguageNamesResponse(IAsyncResult result)
        {
            //obtains the codes from the asyncronous context
            ObservableCollection<string> languageCodes = (ObservableCollection<string>) result.AsyncState;
            //obtains the names
            ObservableCollection<string> languageNames = languageService.EndGetLanguageNames(result);

            //assembles the language list
            List<Language> languages = new List<Language>();
            for (int i = 0; i < languageCodes.Count; i++)
            {
                languages.Add(new Language(languageCodes[i], languageNames[i]));
            }

            //notifies the UI the languages are ready
            LanguagesReceived(languages);
        }

        /// <summary>
        /// Processes a translation.
        /// </summary>
        /// <param name="context">Context containing the message information.</param>
        public void Translate(MessageContext context)
        {
            //first checks if need to detect the source language
            if (context.Direction == MessageDirection.Incoming && context.IsLanguageDetectionNeeded)
            {
                //detects the the incoming message language
                languageService.BeginDetect(AppID, context.OriginalMessage, this.OnDetectResponse, context);
            }
            else
            {
                //translates directly
                DoTranslate(context);
            }
        }

        /// <summary>
        /// Called back when the source language was detected.
        /// </summary>
        /// <param name="result"></param>
        private void OnDetectResponse(IAsyncResult result)
        {
            //gets context from the asyncronous context
            MessageContext context = (MessageContext) result.AsyncState;

            try
            {
                //reads the response
                context.SourceLanguage = languageService.EndDetect(result);

                //now calls the translation itself
                DoTranslate(context);
            }
            catch (Exception ex)
            {
                TranslationError(ex, context);
            }
        }

        /// <summary>
        /// Sends the message for translation.
        /// </summary>
        /// <param name="context"></param>
        private void DoTranslate(MessageContext context)
        {
            //calls the translation service
            languageService.BeginTranslate(AppID, context.OriginalMessage, context.SourceLanguage, context.TargetLanguage, this.OnTranslateResponse, context);
        }

        /// <summary>
        /// Called when the translation is received.
        /// </summary>
        /// <param name="result"></param>
        private void OnTranslateResponse(IAsyncResult result)
        {
            //gets context from the asyncronous context
            MessageContext context = (MessageContext)result.AsyncState;

            try
            {
                //reads the response
                context.TranslatedMessage = languageService.EndTranslate(result);

                //notifies the UI
                TranslationReceived(context);
            }
            catch (Exception ex)
            {
                TranslationError(ex, context);
            }
        }
    }
}
