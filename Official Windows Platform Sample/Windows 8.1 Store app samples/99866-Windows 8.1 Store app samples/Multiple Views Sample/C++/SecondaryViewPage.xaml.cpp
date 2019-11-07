//
// SecondaryViewPage.xaml.cpp
// Implementation of the SecondaryViewPage class
// This page is shown in secondary views created by this app.
// See Scenario 1 for details on how to create a secondary view
//

#include "pch.h"
#include "SecondaryViewPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MultipleViews;

using namespace Concurrency;
using namespace Platform;
using namespace SecondaryViewsHelpers;
using namespace Windows::Foundation;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::Core::AnimationMetrics;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Animation;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ViewManagement;

const int ANIMATION_TRANSLATION_START = 100;
const int ANIMATION_TRANSLATION_END = 0;
const int ANIMATION_OPACITY_START = 0;
const int ANIMATION_OPACITY_END = 1;

const wchar_t* EMPTY_TITLE = L"<title cleared>";

SecondaryViewPage::SecondaryViewPage()
{
    InitializeComponent();
    enterAnimation = CreateEnterAnimation();
}

void SecondaryViewPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    thisViewControl = dynamic_cast<ViewLifetimeControl^>(e->Parameter);
    mainDispatcher = dynamic_cast<App^>(App::Current)->MainDispatcher;
    mainViewId = dynamic_cast<App^>(App::Current)->MainViewId;
    
    // When this view is finally release, clean up state
    releasedToken = thisViewControl->Released += ref new ViewReleasedHandler(this, &SecondaryViewPage::View_Released);
}

void SecondaryViewPage::HandleProtocolLaunch(Uri^ uri)
{
    // This code should only get executed if DisableShowingMainViewOnActivation
    // has been called. See Scenario 2 for details
    ProtocolText->Visibility = Windows::UI::Xaml::Visibility::Visible;
    ProtocolText->Text = uri->AbsoluteUri;
}

void SecondaryViewPage::SwitchAndAnimate(int fromViewId)
{   
    // This continues the flow from Scenario 3
    thisViewControl->StartViewInUse();

    // Before switching, make this view match the outgoing window
    // (go to a blank background)
    TimeSpan start;
    start.Duration = 0;
    enterAnimation->Begin();
    enterAnimation->Pause();
    enterAnimation->Seek(start);
    
    // Bring this view onto screen. Since the two view are drawing
    // the same visual, the user will not be able to perceive the switch
    create_task(ApplicationViewSwitcher::SwitchAsync(
        ApplicationView::GetForCurrentView()->Id,
        fromViewId,
        ApplicationViewSwitchingOptions::SkipAnimation)).then([this] ()
    {
        // Now that this window is on screen, animate in its contents
        enterAnimation->Begin();
        thisViewControl->StopViewInUse();
    });
}

void SecondaryViewPage::SetTitle_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Set a title for the window. This title is visible
    // in system switchers
    SetTitle(TitleBox->Text);
}

void SecondaryViewPage::ClearTitle_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Clear the title by setting it to blank
    SetTitle("");
    TitleBox->Text = "";
}

void SecondaryViewPage::SetTitle(String^ newTitle)
{
    auto thisViewId = ApplicationView::GetForCurrentView()->Id;
    ApplicationView::GetForCurrentView()->Title = newTitle;
    thisViewControl->StartViewInUse();

    // The title contained in the ViewLifetimeControl object is bound to
    // UI elements on the main thread. So, updating the title
    // in this object must be done on the main thread
    create_task(mainDispatcher->RunAsync(CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this, newTitle, thisViewId] ()
    {
        // Setting the title on ApplicationView to blank will clear the title in
        // the system switcher. It would be good to still have a title in the app's UI.
        String^ uiTitle = newTitle != "" ? newTitle : ref new String(EMPTY_TITLE);
        dynamic_cast<App^>(App::Current)->UpdateTitle(uiTitle, thisViewId);
        thisViewControl->StopViewInUse(); 
    })));
}

void SecondaryViewPage::View_Released(Object^ sender, Object^ e)
{
    dynamic_cast<ViewLifetimeControl^>(sender)->Released -= releasedToken;
    
    auto thisDispatcher = Window::Current->Dispatcher;

    // The ViewLifetimeControl object is bound to UI elements on the main thread
    // So, the object must be removed from that thread
    create_task(mainDispatcher->RunAsync(CoreDispatcherPriority::Normal, 
        ref new DispatchedHandler([this, thisDispatcher] ()
    {
        auto views = dynamic_cast<App^>(App::Current)->SecondaryViews;
        for (unsigned int i = 0; i < views->Size; i++)
        {
            auto viewData = views->GetAt(i);
            if (thisViewControl->Id == viewData->Id)
            {
                views->RemoveAt(i);
                thisDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([] ()
                {
                    // It's important to make sure no work is scheduled on this thread
                    // after it starts to close (no data binding changes, no changes to
                    // XAML, creating new objects in destructors, etc.) since
                    // that will throw exceptions
                    Window::Current->Close();    
                }));
                return;
            }
        }

        throw ref new Exception(HRESULT_FROM_WIN32(ERROR_NOT_FOUND), "ViewID not found in collection");
    })));
}

void SecondaryViewPage::ProtocolLaunch_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Used with Scenario 2
    thisViewControl->StartViewInUse();
    create_task(Launcher::LaunchUriAsync(ref new Uri("multiple-view-sample://basiclaunch/"))).then([this] (bool launched)
    {
        thisViewControl->StopViewInUse();
    });
}

void SecondaryViewPage::GoToMain_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Switch to the main view without explicitly requesting
    // that this view be hidden
    thisViewControl->StartViewInUse();
    create_task(ApplicationViewSwitcher::SwitchAsync(mainViewId)).then([this] ()
    {
        thisViewControl->StopViewInUse();
    });
}

void SecondaryViewPage::HideView_Click(Object^ sender, RoutedEventArgs^ e)
{
    thisViewControl->StartViewInUse();
    create_task(ApplicationViewSwitcher::SwitchAsync(
        mainViewId,
        ApplicationView::GetForCurrentView()->Id,
        ApplicationViewSwitchingOptions::ConsolidateViews
    )).then([this] ()
    {
        thisViewControl->StopViewInUse();
    });
}

Storyboard^ SecondaryViewPage::CreateEnterAnimation()
{
    auto enterAnimation = ref new Storyboard();
    Storyboard::SetTarget(enterAnimation, LayoutRoot);

    auto ad = ref new AnimationDescription(AnimationEffect::EnterPage, AnimationEffectTarget::Primary);
    for (unsigned int i = 0; i < LayoutRoot->Children->Size; i++)
    {
        // Add a render transform to the existing one just for animations
        auto element = LayoutRoot->Children->GetAt(i);
        auto tg = ref new TransformGroup();
        tg->Children->Append(ref new TranslateTransform());
        tg->Children->Append(element->RenderTransform);
        element->RenderTransform = tg;

        // Calculate the stagger for each animation. Note that this has a max       
        long long delayMs = (long long) min(ad->StaggerDelay.Duration * i * ad->StaggerDelayFactor, ad->DelayLimit.Duration);
        TimeSpan delay;
        delay.Duration = delayMs;

        for (auto description : ad->Animations)
        {
            auto animation = ref new DoubleAnimationUsingKeyFrames();

            // Start the animation at the right offset
            auto startSpline = ref new SplineDoubleKeyFrame();
            TimeSpan startKeyTime;
            startKeyTime.Duration = 0;
            startSpline->KeyTime = startKeyTime;
            Storyboard::SetTarget(animation, element);

            // Hold at that offset until the stagger delay is hit
            auto middleSpline = ref new SplineDoubleKeyFrame();
            middleSpline->KeyTime = delay;

            // Animation from delayed time to last time
            auto endSpline = ref new SplineDoubleKeyFrame();
            auto endSplineKey = ref new KeySpline();
            endSplineKey->ControlPoint1 = description->Control1;
            endSplineKey->ControlPoint2 = description->Control2;
            endSpline->KeySpline = endSplineKey;
            TimeSpan endKeyTime;
            endKeyTime.Duration = description->Duration.Duration + delay.Duration;
            endSpline->KeyTime = endKeyTime;

            // Do the translation
            if (description->Type == PropertyAnimationType::Translation)
            {
                startSpline->Value = ANIMATION_TRANSLATION_START;
                middleSpline->Value = ANIMATION_TRANSLATION_START;
                endSpline->Value = ANIMATION_TRANSLATION_END;

                Storyboard::SetTargetProperty(animation, "(UIElement.RenderTransform).(TransformGroup.Children)[0].X");
            }
            // Opacity
            else if (description->Type == PropertyAnimationType::Opacity)
            {
                startSpline->Value = ANIMATION_OPACITY_START;
                middleSpline->Value = ANIMATION_OPACITY_START;
                endSpline->Value = ANIMATION_OPACITY_END;

                Storyboard::SetTargetProperty(animation, "Opacity");
            }
            else
            {
                throw ref new Exception(E_INVALIDARG, "Encountered an unexpected animation type.");
            }

            // Put the final animation together
            animation->KeyFrames->Append(startSpline);
            animation->KeyFrames->Append(middleSpline);
            animation->KeyFrames->Append(endSpline);
            enterAnimation->Children->Append(animation);
        }
    }

    return enterAnimation;
}