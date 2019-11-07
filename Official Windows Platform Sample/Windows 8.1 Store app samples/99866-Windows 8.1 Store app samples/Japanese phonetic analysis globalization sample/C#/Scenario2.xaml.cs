//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.Globalization;

namespace JapanesePhoneticAnalysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// This is the click handler for the 'Start' button.
        /// </summary>
        /// <param name="sender">'Start' button</param>
        /// <param name="e">Event arguments</param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.NotifyUser("Analyzing...", NotifyType.StatusMessage);
            string input = this.InputTextBox.Text;
            bool monoRuby = this.MonoRubyCheckBox.IsChecked != null && this.MonoRubyCheckBox.IsChecked.Value;
            string output = String.Empty;

            // Split the Japanese text input in the text field into multiple words.
            // Based on the second boolean parameter of GetWords(), the unit of segmentation changes.
            // When the second parameter is true, each element in the result corresponds to one or more characters which have an atomic reading.
            // When the second parameter is false, each element in the result corresponds to a single Japanese word.
            IReadOnlyList<JapanesePhoneme> words = JapanesePhoneticAnalyzer.GetWords(input, monoRuby);
            foreach (JapanesePhoneme word in words)
            {
                // IsPhraseStart indicates whether the word is the first word of a segment or not.
                if (output != "" && word.IsPhraseStart)
                {
                    // Output a delimiter before each segment.
                    output += "/";
                }

                // DisplayText property has the display text of the word, which has same characters as the input of GetWords().
                // YomiText property has the reading text of the word, as known as Yomi, which basically consists of Hiragana characters.
                // However, please note that the reading can contains some non-Hiragana characters for some display texts such as emoticons or symbols.
                output += word.DisplayText + "(" + word.YomiText + ")";
            }

            // Display the result.
            this.OutputTextBox.Text = output;
            if (input != "" && output == "")
            {
                // If the result from GetWords() is empty but the input is not empty,
                // it means the given input is too long to analyze.
                MainPage.Current.NotifyUser("Failed to get words from the input text.  The input text is too long to analyze.", NotifyType.ErrorMessage);
            }
            else
            {
                // Otherwise, the analysis has done successfully.
                MainPage.Current.NotifyUser("Got words from the input text successfully.", NotifyType.StatusMessage);
            }
        }

    }
}
