//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        //
        //  Open a single file picker [with fileTypeFilter].
        //  And then, call media.SetSource(picked file).
        //  If the file is successfully opened, VideoMediaOpened() will be called and call media.Play().
        //
        public async void PickSingleFileAndSet(string[] fileTypeFilters, params MediaElement[] mediaElements)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                String firstFilter = fileTypeFilters[0];
                Uri uri;
                if (firstFilter.Equals(".mpg", StringComparison.OrdinalIgnoreCase))
                {
                    uri = new Uri("ms-appx:///videos/video.mpg");
                }
                else if (firstFilter.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    uri = new Uri("http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4");
                }
                else
                {
                    throw new ArgumentException();
                }

                for (uint i = 0; i < mediaElements.Length; ++i)
                {
                    MediaElement media = mediaElements[i];
                    media.Stop();
                    media.Source = uri;
                }
            });
        }
    }
}
