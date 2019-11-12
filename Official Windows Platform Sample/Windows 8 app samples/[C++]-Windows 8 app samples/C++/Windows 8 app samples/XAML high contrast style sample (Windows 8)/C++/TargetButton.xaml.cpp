//
// TargetButton.xaml.cpp
// Implementation of the TargetButton class
//

#include "pch.h"
#include "TargetButton.xaml.h"

using namespace SDKSample::HighContrast;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

TargetButton::TargetButton()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void TargetButton::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter
}

void TargetButton::OnApplyTemplate()
{
           Windows::UI::Xaml::Controls::Button::OnApplyTemplate();

            // Apply correct coloring when in High Contrast

            Windows::UI::ViewManagement::AccessibilitySettings^ accessibilitySettings = ref new Windows::UI::ViewManagement::AccessibilitySettings;
            if (!(accessibilitySettings->HighContrast) /*Off*/)
            {
                // Use default colors

                Background = ref new SolidColorBrush(Windows::UI::Colors::Red);
                BorderBrush = ref new SolidColorBrush(Windows::UI::Colors::Black);

				Circle4->Fill = ref new SolidColorBrush(Windows::UI::Colors::Blue);
                Circle3->Fill = ref new SolidColorBrush(Windows::UI::Colors::Green);
                Circle2->Fill = ref new SolidColorBrush(Windows::UI::Colors::Yellow);
                Circle1->Fill = ref new SolidColorBrush(Windows::UI::Colors::White);

                Circle4->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
				Circle3->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
				Circle2->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
				Circle1->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
            }
            else
            {
                // Use High Contrast Colors

                Windows::UI::ViewManagement::UISettings^ uiSettings = ref new Windows::UI::ViewManagement::UISettings;

				Platform::String^ strBlackScheme = "High Contrast Black";
				Platform::String^ strWhiteScheme = "High Contrast White";

				if(accessibilitySettings->HighContrastScheme->Equals(strBlackScheme))
				{
					 Background = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle4->Fill  = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle3->Fill = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle2->Fill = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle1->Fill = ref new SolidColorBrush(Windows::UI::Colors::Black);

					 BorderBrush = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle4->Stroke  = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle3->Stroke = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle2->Stroke = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle1->Stroke = ref new SolidColorBrush(Windows::UI::Colors::White);
				}
				else if(accessibilitySettings->HighContrastScheme->Equals(strWhiteScheme))
				{
					 Background = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle4->Fill  = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle3->Fill = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle2->Fill = ref new SolidColorBrush(Windows::UI::Colors::White);
					 Circle1->Fill = ref new SolidColorBrush(Windows::UI::Colors::White);

					 BorderBrush = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle4->Stroke  = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle3->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle2->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
					 Circle1->Stroke = ref new SolidColorBrush(Windows::UI::Colors::Black);
				}
				else
				{
					    Background = ref new SolidColorBrush(uiSettings->UIElementColor( Windows::UI::ViewManagement::UIElementType::ButtonFace));
                        BorderBrush = ref new SolidColorBrush(uiSettings->UIElementColor( Windows::UI::ViewManagement::UIElementType::ButtonText));
                        Circle4->Fill = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::Hotlight));
                        Circle3->Fill = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::Hotlight));
                        Circle2->Fill  = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::Hotlight));
                        Circle1->Fill = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::Hotlight));

						Circle4->Stroke = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::HighlightText));
                        Circle3->Stroke = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::HighlightText));
                        Circle2->Stroke  = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::HighlightText));
                        Circle1->Stroke = ref new SolidColorBrush(uiSettings->UIElementColor(Windows::UI::ViewManagement::UIElementType::HighlightText));
				}

            }
        }