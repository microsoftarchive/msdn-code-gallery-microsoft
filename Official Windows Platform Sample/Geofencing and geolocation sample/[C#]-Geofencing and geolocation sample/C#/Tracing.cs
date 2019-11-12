//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Geofencing4SqSample
{
    public enum TraceLevel
    {
        Error = 0,
        Warn,
        Info,
        Debug
    }

    // Simple logging class that updates a scrolling text box
    sealed class Logger
    {
        private static TextBox _textBox;
        private static ScrollViewer _textBoxScrollViewer;
        private static Logger _log;

        public static void Initialize(TextBox textBox, ScrollViewer textBoxScrollViwer)
        {
            Debug.Assert(_log == null);
            _log = new Logger(textBox, textBoxScrollViwer);
        }

        public Logger(TextBox textBox, ScrollViewer textBoxScrollViwer)
        {
            Logger._textBox = textBox;
            Logger._textBoxScrollViewer = textBoxScrollViwer;
        }

        public static string FormatException(Exception ex)
        {
            return ex.ToString() + "\n" + ex.StackTrace;
        }

        public static string FormatLatLong(double latitude, double longitude)
        {
            return String.Format("({0:F6}, {1:F6})", latitude, longitude);
        }

        // Note: this returns async void instead of async Task for convenience
        // and because this is fire-and-forget
        public static async void Trace(TraceLevel level, string message)
        {
            if (null != _textBox)
            {
                await _textBox.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var newFormatedLine = string.Format("{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff}\t[{1}]\t{2}\n", DateTime.Now, level, message);
                    _textBox.Text += newFormatedLine;
                    _textBoxScrollViewer.ChangeView(null, _textBoxScrollViewer.ScrollableHeight, null); // Scroll to the bottom
                });
            }
        }
    }
}
