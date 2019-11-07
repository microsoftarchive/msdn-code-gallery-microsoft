// Copyright (c) Microsoft. All rights reserved.

using ContactPicker;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SDKTemplate
{
     /// <summary>
     /// Partial App class.
     /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Activates the ContactPickerPage.
        /// </summary>
        /// <param name="args">Activation args</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ContactPicker)
            {
                var page = new ContactPickerPage();
                page.Activate((ContactPickerActivatedEventArgs)args);
            }
            else
            {
                base.OnActivated(args);
            }
        }
    }
}