/****************************** Module Header ******************************\
 * Module Name:  MyExtensions.cs
 * Project:      CSUnvsAppHtmlBinding
 * Copyright (c) Microsoft Corporation.
 * 
 * This code sample shows how to bind HTML from a data model to a WebView.
 * For more details, please refer to:
 * http://blogs.msdn.com/b/wsdevsol/archive/2013/09/26/binding-html-to-a-webview-with-attached-properties.aspx
 * 
 * MyExtensions class.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CSUnvsAppHtmlBinding
{
    class MyExtensions
    {
        public static string GetHTML(DependencyObject obj)
        {
            return (string)obj.GetValue(HTMLProperty);
        }

        public static void SetHTML(DependencyObject obj, string value)
        {
            obj.SetValue(HTMLProperty, value);
        }

        // Using a DependencyProperty as the backing store for HTML.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty HTMLProperty =
            DependencyProperty.RegisterAttached("HTML", typeof(string), typeof(MyExtensions), new PropertyMetadata(0,new PropertyChangedCallback(OnHTMLChanged)));

        private static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            WebView wv = d as WebView;
            if (wv != null)
            {
                wv.NavigateToString((string)e.NewValue);
            }
        }

    }

}
