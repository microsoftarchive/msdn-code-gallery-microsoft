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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaStreamSource
{
    /// <summary>
    /// This class acts as a mediator between managed media stream source and low level native media extension.
    /// </summary>
    internal class MMSWinRTPlugin : MSSWinRTExtension.IMediaStreamSourcePlugin
    {
        private MediaStreamSource _parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        public MMSWinRTPlugin(MediaStreamSource source)
        {
            _parent = source;
        }

        /// <summary>
        /// Closes the media
        /// </summary>
        public void CloseMedia()
        {
            _parent.CloseMedia();
            _parent.Unregister();
        }

        /// <summary>
        /// Request for a sample
        /// </summary>
        public void GetSampleAsync(MSSWinRTExtension.MediaStreamType streamType)
        {
            _parent.GetSampleAsync((MediaStreamType) streamType);
        }

        /// <summary>
        /// Opens a media asynchronously
        /// </summary>
        public void OpenMediaAsync()
        {
            _parent.OpenMediaAsync();
        }

        /// <summary>
        /// Reference to a low level service is passed here from the C++ component.
        /// </summary>
        /// <param name="pService"></param>
        public void SetService(MSSWinRTExtension.MediaStreamSourceService pService)
        {
            _parent.SetService(pService);
        }
    }
}
