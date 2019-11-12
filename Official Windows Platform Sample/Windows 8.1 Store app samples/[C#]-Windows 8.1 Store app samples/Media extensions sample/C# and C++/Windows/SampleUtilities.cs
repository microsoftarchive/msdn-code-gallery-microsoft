//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using Windows.Storage;
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
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            foreach (string filter in fileTypeFilters)
            {
                picker.FileTypeFilter.Add(filter);
            }
            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);

                    for (int i = 0; i < mediaElements.Length; ++i)
                    {
                        MediaElement me = mediaElements[i];
                        me.Stop();
                        if (i + 1 < mediaElements.Length)
                        {
                            me.SetSource(stream.CloneStream(), file.ContentType);
                        }
                        else
                        {
                            me.SetSource(stream, file.ContentType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotifyUser("Cannot open video file - error: " + ex.Message, NotifyType.ErrorMessage);
                }
            }
        }
    }
}
