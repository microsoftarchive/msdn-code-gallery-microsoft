using System;
using Windows.UI;
using Windows.UI.Core.AnimationMetrics;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace KeyboardEventsSampleCS
{
    internal enum ResizeType
    {
        NoResize = 0,
        ResizeFromShow = 1,
        ResizeFromHide = 2
    }

    public sealed partial class KeyboardPage : Page
    {
        private double displacement = 0;
        private double viewSize = 0;
        private double bottomOfList = 0;
        private bool resized = false;
        private InputPaneHelper inputPaneHelper;

        private ResizeType shouldResize = ResizeType.NoResize;

        public KeyboardPage()
        {
            InitializeComponent();

            // Each scrollable area should be large enough to demonstrate scrolling
            double listHeight = Window.Current.Bounds.Height * 2;
            LeftList.Height = listHeight;
            MiddleList.Height = listHeight;

            // InputPaneHelper is a custom class that allows keyboard event listeners to
            // be attached to individual elements
            inputPaneHelper = new InputPaneHelper();
            inputPaneHelper.SubscribeToKeyboard(true);
            inputPaneHelper.AddShowingHandler(CustomHandlingBox, new InputPaneShowingHandler(CustomKeyboardHandler));
            inputPaneHelper.SetHidingHandler(new InputPaneHidingHandler(InputPaneHiding));
        }

        private void CloseView_Click(object sender, RoutedEventArgs e)
        { 
            this.Frame.GoBack(); 
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            inputPaneHelper.SubscribeToKeyboard(false);
            inputPaneHelper.RemoveShowingHandler(CustomHandlingBox);
            inputPaneHelper.SetHidingHandler(null);
        }

        private void CustomKeyboardHandler(object sender, InputPaneVisibilityEventArgs e)
        {
            // This function animates the middle scroll area up, then resizes the rest of
            // the viewport. The order of operations is important to ensure that the user
            // doesn't see a blank spot appear behind the keyboard
            viewSize = e.OccludedRect.Y;

            // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
            // will move on its own. You should make sure the input element doesn't get occluded by the bar
            displacement = -e.OccludedRect.Height;
            bottomOfList = MiddleScroller.VerticalOffset + MiddleScroller.ActualHeight;

            // Be careful with this property. Once it has been set, the framework will
            // do nothing to help you keep the focused element in view.
            e.EnsuredFocusedElementInView = true;

            ShowingMoveSpline.Value = displacement;
            MoveMiddleOnShowing.Begin();
        }

        private void ShowAnimationComplete(object sender, object e)
        {
            // Once the animation completes, the app is resized
            shouldResize = ResizeType.ResizeFromShow;
            Container.SetValue(Grid.HeightProperty, viewSize);
            MiddleTranslate.Y = 0;
        }

        private void InputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            if (displacement != 0.0)
            {
                MoveMiddleOnShowing.Stop();

                // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
                // will move on its own. You should make sure the input element doesn't get occluded by the bar
                bottomOfList = MiddleScroller.VerticalOffset + MiddleScroller.ActualHeight;

                // If the middle area has actually completed resize, then we want to ignore
                // the default system behavior
                if (resized)
                {
                    // Be careful with this property. Once it has been set, the framework will not change
                    // any layouts in response to the keyboard coming up
                    e.EnsuredFocusedElementInView = true;
                }

                // If the container has already been resized, it should be sized back to the right size
                // Otherwise, there's no need to change the height
                if (Double.IsNaN((double) Container.GetValue(Grid.HeightProperty)))
                {
                    MoveMiddleOnHiding.Begin();
                }
                else
                {
                    shouldResize = ResizeType.ResizeFromHide;
                    Container.ClearValue(Grid.HeightProperty);
                }
            }
        }

        private void MiddleScroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Scrolling should occur after the scrollable element has been resized to ensure
            // that the items the user was looking at remain in view
            if (shouldResize == ResizeType.ResizeFromShow)
            {
                resized = true;
                shouldResize = ResizeType.NoResize;
		MiddleScroller.ChangeView(null, bottomOfList - MiddleScroller.ActualHeight, null, true);
            }
            else if (shouldResize == ResizeType.ResizeFromHide)
            {
                shouldResize = ResizeType.NoResize;
                MiddleTranslate.Y = displacement;
		MiddleScroller.ChangeView(null, bottomOfList - MiddleScroller.ActualHeight, null, true);

                displacement = 0;
                resized = false;

                MoveMiddleOnHiding.Begin();
            }
        }
    }
}
