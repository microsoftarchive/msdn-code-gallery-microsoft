/****************************** Module Header ******************************\
 Module Name:   AutoCompleteTextBox.cs
 Project:       CSWPFAutoCompleteTextBox
 Copyright (c) Microsoft Corporation.
 
 This example demonstrates how to achieve AutoComplete TextBox in WPF Application.
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CSWPFAutoCompleteTextBox.UserControls
{

    /// <summary>
    /// Achieve AutoComplete TextBox or ComboBox
    /// </summary>
    public class AutoCompleteTextBox : ComboBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteTextBox"/> class.
        /// </summary>
        public AutoCompleteTextBox()
        {
            //load and apply style to the ComboBox.
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("/" + this.GetType().Assembly.GetName().Name + 
                ";component/UserControls/AutoCompleteComboBoxStyle.xaml",
                 UriKind.Relative);
            this.Resources = rd;
            //disable default Text Search Function
            this.IsTextSearchEnabled = false;
        }
 
        /// <summary>
        /// Override OnApplyTemplate method 
        /// Get TextBox control out of Combobox control, and hook up TextChanged event.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //get the textbox control in the ComboBox control
            TextBox textBox = this.Template.FindName("PART_EditableTextBox", this) as TextBox;
            if (textBox != null)
            {
                //disable Autoword selection of the TextBox
                textBox.AutoWordSelection = false;
                //handle TextChanged event to dynamically add Combobox items.
                textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
            }
        }

        //The autosuggestionlist source.
        private ObservableCollection<string> autoSuggestionList = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the auto suggestion list.
        /// </summary>
        /// <value>The auto suggestion list.</value>
        public ObservableCollection<string> AutoSuggestionList
        {
            get { return autoSuggestionList; }
            set { autoSuggestionList = value; }
        }

      
        /// <summary>
        /// main logic to generate auto suggestion list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> 
        /// instance containing the event data.</param>
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.AutoWordSelection = false;
            // if the word in the textbox is selected, then don't change item collection
            if ((textBox.SelectionStart != 0 || textBox.Text.Length==0))
            {
                this.Items.Clear();
                //add new filtered items according the current TextBox input
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    foreach (string s in this.autoSuggestionList)
                    {
                        if (s.StartsWith(textBox.Text, StringComparison.InvariantCultureIgnoreCase))
                        {

                            string unboldpart = s.Substring(textBox.Text.Length);
                            string boldpart = s.Substring(0, textBox.Text.Length);
                            //construct AutoCompleteEntry and add to the ComboBox
                            AutoCompleteEntry entry = new AutoCompleteEntry(s, boldpart, unboldpart);
                            this.Items.Add(entry);
                        }
                    }
                }
            }
            // open or close dropdown of the ComboBox according to whether there are items in the 
            // fitlered result.
            this.IsDropDownOpen = this.HasItems;

            //avoid auto selection
            textBox.Focus();
            textBox.SelectionStart = textBox.Text.Length;

        }
    }

    /// <summary>
    /// Extended ComboBox Item
    /// </summary>
    public class AutoCompleteEntry : ComboBoxItem
    {
        private TextBlock tbEntry;

        //text of the item
        private string text;

        /// <summary>
        /// Contrutor of AutoCompleteEntry class
        /// </summary>
        /// <param name="text">All the Text of the item </param>
        /// <param name="bold">The already entered part of the Text</param>
        /// <param name="unbold">The remained part of the Text</param>
        public AutoCompleteEntry(string text, string bold, string unbold)
        {
            this.text = text;
            tbEntry = new TextBlock();
            //highlight the current input Text
            tbEntry.Inlines.Add(new Run { Text = bold, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.RoyalBlue) });
            tbEntry.Inlines.Add(new Run { Text = unbold });
            this.Content = tbEntry;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }
    }
}
