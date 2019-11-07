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
using Windows.UI.Core.AnimationMetrics;

namespace AnimationMetrics
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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
        /// Retrieves the specified metrics and displays them in textual form.
        /// </summary>
        /// <param name="effect">The AnimationEffect whose metrics are to be displayed.</param>
        /// <param name="target">The AnimationEffecTarget whose metrics are to be displayed.</param>
        private void DisplayMetrics(AnimationEffect effect, AnimationEffectTarget target)
        {
            var s = new System.Text.StringBuilder();
            AnimationDescription animationDescription = new AnimationDescription(effect, target);
            s.AppendFormat("Stagger delay = {0}ms", animationDescription.StaggerDelay.TotalMilliseconds);
            s.AppendLine();
            s.AppendFormat("Stagger delay factor = {0}", animationDescription.StaggerDelayFactor);
            s.AppendLine();
            s.AppendFormat("Delay limit = {0}ms", animationDescription.DelayLimit.TotalMilliseconds);
            s.AppendLine();
            s.AppendFormat("ZOrder = {0}", animationDescription.ZOrder);
            s.AppendLine();
            s.AppendLine();

            int animationIndex = 0;
            foreach (var animation in animationDescription.Animations)
            {
                s.AppendFormat("Animation #{0}:", ++animationIndex);
                s.AppendLine();

                switch (animation.Type)
                {
                    case PropertyAnimationType.Scale:
                        {
                            ScaleAnimation scale = animation as ScaleAnimation;
                            s.AppendLine("Type = Scale");
                            if (scale.InitialScaleX.HasValue)
                            {
                                s.AppendFormat("InitialScaleX = {0}", scale.InitialScaleX.Value);
                                s.AppendLine();
                            }
                            if (scale.InitialScaleY.HasValue)
                            {
                                s.AppendFormat("InitialScaleY = {0}", scale.InitialScaleY.Value);
                                s.AppendLine();
                            }
                            s.AppendFormat("FinalScaleX = {0}", scale.FinalScaleX);
                            s.AppendLine();
                            s.AppendFormat("FinalScaleY = {0}", scale.FinalScaleY);
                            s.AppendLine();
                            s.AppendFormat("Origin = {0}, {1}", scale.NormalizedOrigin.X, scale.NormalizedOrigin.Y);
                            s.AppendLine();
                        }
                        break;
                    case PropertyAnimationType.Translation:
                        s.AppendLine("Type = Translation");
                        break;
                    case PropertyAnimationType.Opacity:
                        {
                            OpacityAnimation opacity = animation as OpacityAnimation;
                            s.AppendLine("Type = Opacity");
                            if (opacity.InitialOpacity.HasValue)
                            {
                                s.AppendFormat("InitialOpacity = {0}", opacity.InitialOpacity.Value);
                                s.AppendLine();
                            }
                            s.AppendFormat("FinalOpacity = {0}", opacity.FinalOpacity);
                            s.AppendLine();
                        }
                        break;
                }

                s.AppendFormat("Delay = {0}ms", animation.Delay.TotalMilliseconds);
                s.AppendLine();
                s.AppendFormat("Duration = {0}ms", animation.Duration.TotalMilliseconds);
                s.AppendLine();
                s.AppendFormat("Cubic Bezier control points");
                s.AppendLine();
                s.AppendFormat("    X1 = {0}, Y1 = {1}", animation.Control1.X, animation.Control1.Y);
                s.AppendLine();
                s.AppendFormat("    X2 = {0}, Y2 = {1}", animation.Control2.X, animation.Control2.Y);
                s.AppendLine();
                s.AppendLine();
            }

            Metrics.Text = s.ToString();
        }

        /// <summary>
        /// When the selection changes, update the output window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Animations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb != null)
            {
                object selectedListBoxItem = lb.SelectedItem;
                if (selectedListBoxItem == AddToListAdded)
                {
                    DisplayMetrics(AnimationEffect.AddToList, AnimationEffectTarget.Added);
                }
                else if (selectedListBoxItem == AddToListAffected)
                {
                    DisplayMetrics(AnimationEffect.AddToList, AnimationEffectTarget.Affected);
                }
                else if (selectedListBoxItem == EnterPage)
                {
                    DisplayMetrics(AnimationEffect.EnterPage, AnimationEffectTarget.Primary);
                }
            }
        }
    }
}
