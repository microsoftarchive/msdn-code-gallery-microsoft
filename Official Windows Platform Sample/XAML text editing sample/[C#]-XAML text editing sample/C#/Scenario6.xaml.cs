//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Text;
using Windows.UI;
using Windows.Storage;
using Windows.Storage.Streams;
using SDKTemplate;
using System;
using System.Collections.Generic;

namespace TextEditing
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        List<ITextRange> m_highlightedWords = null;

        public Scenario6()
        {
            this.InitializeComponent();

            m_highlightedWords = new List<ITextRange>();
        }

        private void BoldButtonClick(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void ItalicButtonClick(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void FontColorButtonClick(object sender, RoutedEventArgs e)
        {
            fontColorPopup.IsOpen = true;
            fontColorButton.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private void FindBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            string textToFind = findBox.Text;

            if (textToFind != null)
                FindAndHighlightText(textToFind);
        }

        private void FontColorButtonLostFocus(object sender, RoutedEventArgs e)
        {
            fontColorPopup.IsOpen = false;
        }

        private void FindBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ClearAllHighlightedWords();
        }

        private void FindBoxGotFocus(object sender, RoutedEventArgs e)
        {
            string textToFind = findBox.Text;

            if (textToFind != null)
                FindAndHighlightText(textToFind);
        }

        private void ClearAllHighlightedWords()
        {
            ITextCharacterFormat charFormat;
            for (int i = 0; i < m_highlightedWords.Count; i++)
            {
                charFormat = m_highlightedWords[i].CharacterFormat;
                charFormat.BackgroundColor = Colors.Transparent;
                m_highlightedWords[i].CharacterFormat = charFormat;
            }

            m_highlightedWords.Clear();
        }

        private void FindAndHighlightText(string textToFind)
        {
            ClearAllHighlightedWords();

            ITextRange searchRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            searchRange.Move(0, 0);

            bool textFound = true;
            do
            {
                if (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) < 1)
                    textFound = false;
                else
                {
                    m_highlightedWords.Add(searchRange.GetClone());

                    ITextCharacterFormat charFormatting = searchRange.CharacterFormat;
                    charFormatting.BackgroundColor = Colors.Yellow;
                    searchRange.CharacterFormat = charFormatting;
                }
            } while (textFound);
        }

        private void ColorButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedColor = (Button)sender;

            ITextCharacterFormat charFormatting = editor.Document.Selection.CharacterFormat;
            switch (clickedColor.Name)
            {
                case "black":
                    {
                        charFormatting.ForegroundColor = Colors.Black;
                        break;
                    }

                case "gray":
                    {
                        charFormatting.ForegroundColor = Colors.Gray;
                        break;
                    }

                case "darkgreen":
                    {
                        charFormatting.ForegroundColor = Colors.DarkGreen;
                        break;
                    }

                case "green":
                    {
                        charFormatting.ForegroundColor = Colors.Green;
                        break;
                    }

                case "blue":
                    {
                        charFormatting.ForegroundColor = Colors.Blue;
                        break;
                    }

                default:
                    {
                        charFormatting.ForegroundColor = Colors.Black;
                        break;
                    }
            }
            editor.Document.Selection.CharacterFormat = charFormatting;

            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            fontColorPopup.IsOpen = false;
        }

        private async void LoadContentAsync()
        {
            StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("lorem.rtf");
            IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.Read);
            editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
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
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("You clicked the " + b.Name + " button", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Other' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Other_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("You clicked the " + b.Name + " button", NotifyType.StatusMessage);
            }
        }

    }
}
