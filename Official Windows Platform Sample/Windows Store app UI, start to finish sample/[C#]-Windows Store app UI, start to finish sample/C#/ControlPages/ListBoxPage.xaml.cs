//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListBoxPage : ControlPage
    {
        public ListBoxPage()
        {
            this.InitializeComponent();
            
            List<FontFamily> fonts = new List<FontFamily>();
            fonts.Add(new FontFamily("Arial"));
            fonts.Add(new FontFamily("Comic Sans MS"));
            fonts.Add(new FontFamily("Courier New"));
            fonts.Add(new FontFamily("Segoe UI"));
            fonts.Add(new FontFamily("Times New Roman"));

            ListBox2.DataContext = fonts;
            ListBox2.SelectedIndex = 3;
        }

        private void ColorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorName = e.AddedItems[0].ToString();
            switch (colorName)
            {
                case "Yellow":
                    Control1Output.Fill = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
                case "Green":
                    Control1Output.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case "Blue":
                    Control1Output.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
                    break;
                case "Red":
                    Control1Output.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }
        }

        private void FontListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Control2Output.FontFamily = (FontFamily)e.AddedItems[0];
        }
    }
}
