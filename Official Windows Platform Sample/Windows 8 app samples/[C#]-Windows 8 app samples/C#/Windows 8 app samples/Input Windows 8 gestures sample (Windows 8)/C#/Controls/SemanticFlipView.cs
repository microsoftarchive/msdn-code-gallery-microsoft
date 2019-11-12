//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml.Controls;

namespace GesturesApp.Controls
{
    /// <summary>
    /// FlipView control that provides a basic implementation of <see cref="ISemanticZoomInformation"/>
    /// and thus can be used with <see cref="SemanticZoom"/>
    /// </summary>
    public class SemanticFlipView : FlipView, ISemanticZoomInformation
    {
        #region ISemanticZoomInformation implementation

        public bool IsActiveView { get; set; }

        public bool IsZoomedInView { get; set; }

        public SemanticZoom SemanticZoomOwner { get; set; }

        public void CompleteViewChangeTo(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
            // Set the selected item to source.Item
            this.SelectedItem = source.Item;

            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        public void StartViewChangeFrom(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
            // Set the destination item to the currently selected item
            destination.Item = this.SelectedItem;
        }

        #region ISemanticZoomInformation methods that do not need an implementation in this sample

        public void CompleteViewChange() { }

        public void CompleteViewChangeFrom(SemanticZoomLocation source, SemanticZoomLocation destination) { }

        public void InitializeViewChange() { }

        public void MakeVisible(SemanticZoomLocation item) { }

        public void StartViewChangeTo(SemanticZoomLocation source, SemanticZoomLocation destination) { }

        #endregion

        #endregion
    }
}
