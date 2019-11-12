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
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Input
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        //Scenario specific constants
        const uint SUPPORTEDCONTACTS = 5;
        const double STROKETHICKNESS = 5;
        uint numActiveContacts;
        Dictionary<uint, Point?> contacts;

        public Scenario1()
        {
            this.InitializeComponent();
            numActiveContacts = 0;
            contacts = new Dictionary<uint, Point?>((int)SUPPORTEDCONTACTS);
            Scenario1OutputRoot.PointerPressed += new PointerEventHandler(Scenario1OutputRoot_PointerPressed);
            Scenario1OutputRoot.PointerMoved += new PointerEventHandler(Scenario1OutputRoot_PointerMoved);
            Scenario1OutputRoot.PointerReleased += new PointerEventHandler(Scenario1OutputRoot_PointerReleased);
            Scenario1OutputRoot.PointerExited += new PointerEventHandler(Scenario1OutputRoot_PointerReleased);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void Scenario1OutputRoot_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            uint ptrId = e.GetCurrentPoint(sender as FrameworkElement).PointerId;
            if (e.GetCurrentPoint(Scenario1OutputRoot).Properties.PointerUpdateKind != Windows.UI.Input.PointerUpdateKind.Other)
            {
                this.buttonPress.Text = e.GetCurrentPoint(Scenario1OutputRoot).Properties.PointerUpdateKind.ToString();
            }
            if (contacts.ContainsKey(ptrId))
            {
                contacts[ptrId] = null;
                contacts.Remove(ptrId);
                --numActiveContacts;
            }
            e.Handled = true;
        }

        void Scenario1OutputRoot_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(Scenario1OutputRoot);
            uint ptrId = pt.PointerId;
            if (pt.Properties.PointerUpdateKind != Windows.UI.Input.PointerUpdateKind.Other)
            {
                this.buttonPress.Text = pt.Properties.PointerUpdateKind.ToString();
            }
            if (contacts.ContainsKey(ptrId) && contacts[ptrId].HasValue)
            {
                Point currentContact = pt.Position;
                Point previousContact = contacts[ptrId].Value;
                if (Distance(currentContact, previousContact) > 4)
                {
                    Line l = new Line()
                    {
                        X1 = previousContact.X,
                        Y1 = previousContact.Y,
                        X2 = currentContact.X,
                        Y2 = currentContact.Y,
                        StrokeThickness = STROKETHICKNESS,
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeEndLineCap = PenLineCap.Round
                    };

                    contacts[ptrId] = currentContact;
                    ((System.Collections.Generic.IList<UIElement>)Scenario1OutputRoot.Children).Add(l);
                }
            }

            e.Handled = true;
        }

        private double Distance(Point currentContact, Point previousContact)
        {
            return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) + Math.Pow(currentContact.Y - previousContact.Y, 2));
        }

        void Scenario1OutputRoot_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (numActiveContacts >= SUPPORTEDCONTACTS)
            {
                // cannot support more contacts
                return;
            }
            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(Scenario1OutputRoot);
            if (pt.Properties.PointerUpdateKind != Windows.UI.Input.PointerUpdateKind.Other)
            {
                this.buttonPress.Text = pt.Properties.PointerUpdateKind.ToString();
            }
            contacts[pt.PointerId] = pt.Position;
            e.Handled = true;
            ++numActiveContacts;
        }

        void Scenario1Reset(object sender, RoutedEventArgs e)
        {
            Scenario1Reset();
        }

        public void Scenario1Reset()
        {
            ((System.Collections.Generic.IList<UIElement>)Scenario1OutputRoot.Children).Clear();
            numActiveContacts = 0;
            if (contacts != null)
            {
                contacts.Clear();
            }
        }
    }
}
