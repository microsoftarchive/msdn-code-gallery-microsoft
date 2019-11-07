// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Doto.Controls
{
    /// <summary>
    /// Simple user control to allow a data bound image to fade between images when changed
    /// </summary>
    public sealed partial class FadingImage : UserControl
    {
        public FadingImage()
        {
            this.InitializeComponent();
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(FadingImage), new PropertyMetadata(default(ImageSource), SourceChanged));

        private static void SourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FadingImage instance = (FadingImage)sender;
            instance.ImageA.Source = (ImageSource)args.OldValue;
            instance.ImageB.Source = (ImageSource)args.NewValue;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(instance.TransitionMilliseconds));
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, instance.ImageB);
            Storyboard.SetTargetProperty(doubleAnimation, "Opacity");
            sb.Children.Add(doubleAnimation);
            sb.Begin();
        }

        public int TransitionMilliseconds
        {
            get { return (int)GetValue(TransitionMillisecondsProperty); }
            set { SetValue(TransitionMillisecondsProperty, value); }
        }

        public static readonly DependencyProperty TransitionMillisecondsProperty =
            DependencyProperty.Register("TransitionMilliseconds", typeof(int), typeof(FadingImage), new PropertyMetadata(2000));
    }
}