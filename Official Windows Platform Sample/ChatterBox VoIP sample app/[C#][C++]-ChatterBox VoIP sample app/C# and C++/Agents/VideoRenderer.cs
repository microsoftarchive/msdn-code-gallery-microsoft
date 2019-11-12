/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Media;
using PhoneVoIPApp.BackEnd;
using System;
using System.Diagnostics;
using System.Windows;

namespace PhoneVoIPApp.Agents
{
    /// <summary>
    /// A class that renders video from the background process.
    /// Note, the MediaElement that actually displays the video is in the UI process - 
    /// this class receives video from the remote party and writes it to a media streamer.
    /// The media streamer handles connecting the rendered video stream to the media element that
    /// displays it in the UI process.
    /// </summary>
    internal class VideoRenderer : IVideoRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal VideoRenderer()
        {
        }

        #region IVideoRenderer methods

        /// <summary>
        /// Start rendering video.
        /// Note, this method may be called multiple times in a row.
        /// </summary>
        public void Start()
        {
            if (this.isRendering)
                return; // Nothing more to be done

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    Debug.WriteLine("[VideoRenderer::Start] Video rendering setup");
                    StartMediaStreamer();
                    this.isRendering = true;
                }
                catch (Exception err)
                {
                    Debug.WriteLine("[VideoRenderer::Start] " + err.Message);
                }
            });
        }

        private void StartMediaStreamer()
        {
            if (mediaStreamer == null)
            {
                mediaStreamer = MediaStreamerFactory.CreateMediaStreamer(123);
            }

            // Using default resolution of 640x480
            mediaStreamSource = new VideoMediaStreamSource(null, 640, 480);
            mediaStreamer.SetSource(mediaStreamSource);
        }

        /// <summary>
        /// Stop rendering video.
        /// Note, this method may be called multiple times in a row.
        /// </summary>
        public void Stop()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!this.isRendering)
                    return; // Nothing more to be done

                Debug.WriteLine("[VoIP Background Process] Video rendering stopped.");
                mediaStreamSource.Shutdown();                
                mediaStreamSource.Dispose();
                mediaStreamSource = null;
                mediaStreamer.Dispose();
                mediaStreamer = null;

                this.isRendering = false;
            });
        }

        #endregion

        #region Private members

        // Indicates if rendering is already in progress or not
        private bool isRendering;
        private VideoMediaStreamSource mediaStreamSource;
        private MediaStreamer mediaStreamer;

        #endregion
    }
}
