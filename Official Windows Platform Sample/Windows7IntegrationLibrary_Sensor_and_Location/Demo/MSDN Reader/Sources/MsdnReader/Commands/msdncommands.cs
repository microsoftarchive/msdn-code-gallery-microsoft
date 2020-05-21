// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SceReader.View;
using MsdnReader;
using System.Windows.Input;
using Microsoft.SceReader.Controls;

namespace MsdnReader
{
    public class MsdnCommands
    {
        public MsdnCommands(ViewManager viewManager)
        {
            _msdnStartSyncCommand = new MsdnStartSyncCommand(viewManager as MsdnViewManager);
            _openImageViewerCommand = new OpenImageViewerCommand(viewManager as MsdnViewManager);
        }

        public MsdnStartSyncCommand MsdnStartSyncCommand
        {
            get
            {
                return _msdnStartSyncCommand;
            }
        }

        public OpenImageViewerCommand OpenImageViewerCommand
        {
            get
            {
                return _openImageViewerCommand;
            }
        }

        private OpenImageViewerCommand _openImageViewerCommand;
        private MsdnStartSyncCommand _msdnStartSyncCommand;
    }

    public class MsdnStartSyncCommand : ViewCommand
    {
        public MsdnStartSyncCommand(MsdnViewManager viewManager) : base(viewManager) { }

        protected override bool CanExecuteInternal(object parameter)
        {
            return (!MsdnServiceProvider.SubscriptionServiceManager.IsServiceUpdateInProgress
            && ViewManager.SyncCommands.StartSyncCommand.CanExecute(parameter));
        }

        protected override void ExecuteInternal(object parameter)
        {
            if (ViewManager.SyncCommands.StartSyncCommand.CanExecute(parameter) &&
                !MsdnServiceProvider.SubscriptionServiceManager.IsServiceUpdateInProgress)
            {
                ViewManager.SyncCommands.StartSyncCommand.Execute(parameter);
            }
        }
    }

    public class OpenImageViewerCommand : ViewCommand
    {
        public OpenImageViewerCommand(MsdnViewManager viewManager) : base(viewManager) { }

        /// <summary>
        /// CanExecute always returns true, since this comand is only attached to StoryImageControls in theory we cna always execute it
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override bool CanExecuteInternal(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Execute ensures that parameter of the right type to get Story/ ImageReference Info
        /// </summary>
        /// <param name="parameter"></param>
        protected override void ExecuteInternal(object parameter)
        {
            // Image viewer expands on the data in StoryImageControl, needs that control as parameter
            StoryImageControl control = parameter as StoryImageControl;
            if (control != null)
            {
                // Have ViewManager open the window
                MsdnViewManager viewManager = this.ViewManager as MsdnViewManager;
                if (viewManager != null)
                {
                    viewManager.OpenImageViewerWindow(control);
                }
            }
        }
    }
}