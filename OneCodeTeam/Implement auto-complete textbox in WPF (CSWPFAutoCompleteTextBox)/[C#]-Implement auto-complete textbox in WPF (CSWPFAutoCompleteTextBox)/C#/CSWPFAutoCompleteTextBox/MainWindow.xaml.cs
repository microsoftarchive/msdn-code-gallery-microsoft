/****************************** Module Header ******************************\
 Module Name:   MainWindow.xaml.cs
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

using System.Windows;

namespace CSWPFAutoCompleteTextBox
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // set dataSource for AutoComplete TextBox
            ConstructAutoCompletionSource();
        }

        /// <summary>
        /// Constructs the auto completion source.
        /// </summary>
        private void ConstructAutoCompletionSource()
        {
            
            this.textBox.AutoSuggestionList.Add("Hello world");
            this.textBox.AutoSuggestionList.Add("Hey buddy");
            this.textBox.AutoSuggestionList.Add("Halo world");
            this.textBox.AutoSuggestionList.Add("apple");
            this.textBox.AutoSuggestionList.Add("apple tree");
            this.textBox.AutoSuggestionList.Add("appaaaaa");
            this.textBox.AutoSuggestionList.Add("arrange");
            for (int i = 0; i < 100; i++)
            {
                this.textBox.AutoSuggestionList.Add("AA" + i);
            }
        }
    }
}
