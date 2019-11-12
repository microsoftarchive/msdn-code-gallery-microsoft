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
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace HighContrast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UI : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public UI()
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
            string Buffer;
            Windows.UI.ViewManagement.UISettings UserSettings = new Windows.UI.ViewManagement.UISettings();
            Windows.UI.Color Color;

            Buffer = string.Format("Hand Preference {0}\n", UserSettings.HandPreference == Windows.UI.ViewManagement.HandPreference.LeftHanded ? "left" : "right");
            Buffer += string.Format("Cursor Size {0} x {1}\n", UserSettings.CursorSize.Width, UserSettings.CursorSize.Height);
            Buffer += string.Format("Scrollbar Size {0} x {1}\n", UserSettings.ScrollBarSize.Width, UserSettings.ScrollBarSize.Height);
            Buffer += string.Format("Scrollbar Arrow Size {0} x {1}\n", UserSettings.ScrollBarArrowSize.Width, UserSettings.ScrollBarArrowSize.Height);
            Buffer += string.Format("Scrollbar Thumb Box Size {0} x {1}\n", UserSettings.ScrollBarThumbBoxSize.Width, UserSettings.ScrollBarThumbBoxSize.Height);
            Buffer += string.Format("Message Duration {0}\n", UserSettings.MessageDuration);
            Buffer += string.Format("Animations Enabled {0}\n", UserSettings.AnimationsEnabled ? "true" : "false");
            Buffer += string.Format("Caret Browsing Enabled {0}\n", UserSettings.CaretBrowsingEnabled ? "true" : "false");
            Buffer += string.Format("Caret Blink Rate {0}\n", UserSettings.CaretBlinkRate);
            Buffer += string.Format("Caret Width {0}\n", UserSettings.CaretWidth);
            Buffer += string.Format("Double Click Time {0}\n", UserSettings.DoubleClickTime);
            Buffer += string.Format("Mouse Hover Time {0}\n", UserSettings.MouseHoverTime);

            Buffer += "System Colors: \n";

            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.ActiveCaption);
            Buffer += string.Format("\tActive Caption: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Background);
            Buffer += string.Format("\tBackground: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.ButtonFace);
            Buffer += string.Format("\tButton Face: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.ButtonText);
            Buffer += string.Format("\tButton Text: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.CaptionText);
            Buffer += string.Format("\tCaption Text: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.GrayText);
            Buffer += string.Format("\tGray/Disabled Text: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Highlight);
            Buffer += string.Format("\tHighlight: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.HighlightText);
            Buffer += string.Format("\tHighlighted Text: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Hotlight);
            Buffer += string.Format("\tHotlight: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.InactiveCaption);
            Buffer += string.Format("\tInactive Caption: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.InactiveCaptionText);
            Buffer += string.Format("\tInactive Caption Text: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Window);
            Buffer += string.Format("\tWindow: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);
            Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.WindowText);
            Buffer += string.Format("\tWindow Text: {0}, {1}, {2}\n", Color.R, Color.G, Color.B);

            UIOutputTextBlock.Text = Buffer;
        }
    }
}
