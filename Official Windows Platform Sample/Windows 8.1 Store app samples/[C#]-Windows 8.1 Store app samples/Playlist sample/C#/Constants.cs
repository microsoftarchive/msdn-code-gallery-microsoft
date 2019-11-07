//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System;
using PlaylistSample;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Media.Playlists;
using Windows.UI.ViewManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Playlists sample";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Create a playlist", ClassType = typeof(Create) },
            new Scenario() { Title = "Display a playlist", ClassType = typeof(Display) },
            new Scenario() { Title = "Add items to a playlist", ClassType = typeof(Add) },
            new Scenario() { Title = "Remove an item from a playlist", ClassType = typeof(Remove) },
            new Scenario() { Title = "Clear a playlist", ClassType = typeof(Clear) }
        };

        /// <summary>
        /// Playlist used by all sample scenarios
        /// </summary>
        public static Playlist playlist = null;

        /// <summary>
        /// Audio file extensions.
        /// </summary>
        public static readonly string[] audioExtensions = new string[] { ".wma", ".mp3", ".mp2", ".aac", ".adt", ".adts", ".m4a" };

        /// <summary>
        /// Playlist file extensions.
        /// </summary>
        public static readonly string[] playlistExtensions = new string[] { ".m3u", ".wpl", ".zpl" };

        /// <summary>
        /// Creates a FileOpenPicker for the specified extensions. 
        /// </summary>
        /// <param name="extensions">File extensions to pick.</param>
        /// <returns>FileOpenPicker instance.</returns>
        public static FileOpenPicker CreateFilePicker(string[] extensions)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;

            foreach (string extension in extensions)
            {
                picker.FileTypeFilter.Add(extension);
            }

            return picker;
        }

        /// <summary>
        /// Picks and loads a playlist.
        /// </summary>
        async public static Task<Playlist> PickPlaylistAsync()
        {
            FileOpenPicker picker = CreateFilePicker(MainPage.playlistExtensions);
            StorageFile file = await picker.PickSingleFileAsync();

            Playlist playlist = null;
            if (file != null)
            {
                return await Playlist.LoadAsync(file);
            }

            return playlist;
        }
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
