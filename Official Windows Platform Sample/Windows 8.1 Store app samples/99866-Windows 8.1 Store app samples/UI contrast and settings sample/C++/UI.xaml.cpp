//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// UI.xaml.cpp
// Implementation of the UI class
//

#include "pch.h"
#include "UI.xaml.h"

using namespace SDKSample::HighContrast;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::UI::ViewManagement;


UI::UI()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UI::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::HighContrast::UI::UIGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		UISettings^ pUserSettings = ref new UISettings();
		Platform::String^ Buffer;
        Windows::UI::Color Color;
        
        Buffer =  "Hand Preference " + (pUserSettings->HandPreference == HandPreference::RightHanded ? "right" : "left") + "\n";
        Buffer += "Cursor Size "  + pUserSettings->CursorSize.Width.ToString() + " x " + pUserSettings->CursorSize.Height.ToString() + "\n";
        Buffer += "Scrollbar Size "  + pUserSettings->ScrollBarSize.Width.ToString() + " x " + pUserSettings->ScrollBarSize.Height.ToString() + "\n";
        Buffer += "Scrollbar Arrow Size "   + pUserSettings->ScrollBarArrowSize.Width.ToString() + " x " + pUserSettings->ScrollBarArrowSize.Height.ToString() + "\n";
        Buffer += "Scrollbar Thumb Box Size " + pUserSettings->ScrollBarThumbBoxSize.Width.ToString() + " x " + pUserSettings->ScrollBarThumbBoxSize.Height.ToString() + "\n";
        Buffer += "Message Duration "  + pUserSettings->MessageDuration.ToString() + "\n";
        Buffer += "Animations Enabled "  + (pUserSettings->AnimationsEnabled ? "true" : "false") + "\n"; 
        Buffer += "Caret Browsing Enabled "  + (pUserSettings->CaretBrowsingEnabled ? "true" : "false") + "\n";
        Buffer += "Caret Blink Rate "  + pUserSettings->CaretBlinkRate.ToString() + "\n";
        Buffer += "Caret Width " + pUserSettings->CaretWidth.ToString() + "\n";
        Buffer += "Double Click Time " + pUserSettings->DoubleClickTime.ToString() + "\n";
        Buffer += "Mouse Hover Time " + pUserSettings->MouseHoverTime.ToString() + "\n";

        Buffer += "System Colors: \n";

        Color = pUserSettings->UIElementColor(UIElementType::ActiveCaption);
        Buffer += "Active Caption: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::Background);
        Buffer += "Background: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::ButtonFace);
        Buffer += "Button Face: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::ButtonText);
        Buffer += "Button Text: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::CaptionText);
        Buffer += "Caption Text: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::GrayText);
        Buffer += "Gray Text: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::Highlight);
        Buffer += "Highlight: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::HighlightText);
        Buffer += "Highlight Text: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::Hotlight);
        Buffer += "Hotlight: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::InactiveCaption);
        Buffer += "Inactive Caption: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::InactiveCaptionText);
        Buffer += "Inactive Caption Text: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::Window);
        Buffer += "Window: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";
        Color = pUserSettings->UIElementColor(UIElementType::WindowText);
        Buffer += "Window Text: " + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + "\n";

		UIOutputTextBlock->Text = Buffer;
    }
}
