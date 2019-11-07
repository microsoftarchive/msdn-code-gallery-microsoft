using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Security;

using Windows.System.Threading;
using Windows.Foundation;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation.Collections;


namespace MediaStreamSource
{
    // Summary:
    //     This class describes a media sample in enough detail to allow the System.Windows.Controls.MediaElement
    //     and underlying pipeline to render the sample.
    public class MediaStreamSample
    {
        private MSSWinRTExtension.MediaStreamSample _innerSample;

        // Summary:
        //     Instantiates a new instance of the System.Windows.Media.MediaStreamSample
        //     class.
        //
        // Parameters:
        //   mediaStreamDescription:
        //     A description of the stream this sample was pulled from.
        //
        //   stream:
        //     A stream containing the desired media sample. Set to null to report the end
        //     of a stream.
        //
        //   offset:
        //     The offset into the stream where the actual sample data begins.
        //
        //   count:
        //     The number of bytes that comprises the sample data.
        //
        //   timestamp:
        //     The time from the beginning of the media file at which this sample should
        //     be rendered as expressed using 100 nanosecond increments.
        //
        //   attributes:
        //     A collection of pairs describing other attributes of the media sample.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     One or more parameters are invalid.
        public MediaStreamSample(MediaStreamDescription mediaStreamDescription,
                                 Windows.Storage.Streams.IBuffer buffer,// Stream stream,
                                 long offset,
                                 long count,
                                 long timestamp,
                                 IDictionary<MediaSampleAttributeKeys, string> attributes)
        {
            _innerSample = new MSSWinRTExtension.MediaStreamSample(buffer, (int)offset, (int)count, timestamp, 0);
        }

        //
        // Summary:
        //     Instantiates a new instance of the System.Windows.Media.MediaStreamSample
        //     class.
        //
        // Parameters:
        //   mediaStreamDescription:
        //     A description of the stream this sample was pulled from.
        //
        //   stream:
        //     A stream containing the desired media sample. Set to null to report the end
        //     of a stream.
        //
        //   offset:
        //     The offset into the stream where the actual sample data begins.
        //
        //   count:
        //     The number of bytes that comprises the sample data.
        //
        //   timestamp:
        //     The time from the beginning of the media file at which this sample should
        //     be rendered as expressed using 100 nanosecond increments.
        //
        //   duration:
        //     The duration of the sample.
        //
        //   attributes:
        //     A collection of pairs describing other attributes of the media sample.
        public MediaStreamSample(MediaStreamDescription mediaStreamDescription,
                                 Windows.Storage.Streams.IBuffer buffer,//Stream stream,
                                 long offset,
                                 long count,
                                 long timestamp,
                                 long duration,
                                 IDictionary<MediaSampleAttributeKeys, string> attributes)
        {
            _innerSample = new MSSWinRTExtension.MediaStreamSample(buffer, (int)offset, (int)count, timestamp, duration);
        }
               
        //
        // Summary:
        //     Gets the duration of the sample.
        //
        // Returns:
        //     The duration of the sample in 10 nanosecond units.
        public long Duration { get { return _innerSample.Duration; } }
        
        //
        // Summary:
        //     Gets the time at which a sample should be rendered as measured in 100 nanosecond
        //     increments.
        //
        // Returns:
        //     The time at which a sample should be rendered as measured in 100 nanosecond
        //     increments. The default value is 0.
        public long Timestamp { get { return _innerSample.Timestamp; } }

        /// <summary>
        /// Internal sample passed to lower layer.
        /// </summary>
        internal MSSWinRTExtension.MediaStreamSample InnerSample { get { return _innerSample; } }
    }

    // Summary:
    //     A MediaStreamSource delivers media samples directly to the media pipeline
    //     and is most often used to enable the System.Windows.Controls.MediaElement
    //     to use a container format not natively supported by Silverlight.
    public abstract class MediaStreamSource: DependencyObject
    {
        /// <summary>
        /// MediaElements attached property which helps with assigning MediaStreamSource to the control.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
            "Source", typeof(MediaStreamSource), typeof(MediaElement), new Windows.UI.Xaml.PropertyMetadata(null, new PropertyChangedCallback(Source_PropertyChanged)));


        /// <summary>
        /// Source property has been changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void Source_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MediaStreamSource newSource = (MediaStreamSource)e.NewValue;
                newSource.RegisterWith((MediaElement)obj);
            }
            else
            {
                ((MediaElement)obj).Source = null;
            }
        }

        /// <summary>
        /// Returns values of the attached property - source.
        /// </summary>
        /// <param name="element">Element which property is attached to</param>
        /// <returns></returns>
        public static object GetSource(UIElement element)
        {
            return element.GetValue(SourceProperty);
        }

        /// <summary>
        /// Returns values of the attached property - source.
        /// </summary>
        /// <param name="element">Element which property is attached to</param>
        /// <param name="value">Value of the property</param>
        public static void SetSource(UIElement element, MediaStreamSource value)
        {
            element.SetValue(SourceProperty, value);
        }

        // Summary:
        //     Initializes a new instance of the System.Windows.Media.MediaStreamSource
        //     class.
        protected MediaStreamSource()
        {
        }

        // Summary:
        //     Gets or sets the length of the audio buffer.
        //
        // Returns:
        //     The length of the audio buffer in milliseconds. The value must be between
        //     15 and 1000. The default value is 1000, which is a one second buffer.
        protected internal int AudioBufferLength { get; set; }

        // Summary:
        //     The System.Windows.Controls.MediaElement can call this method when going
        //     through normal shutdown or as a result of an error. This lets the developer
        //     perform any needed cleanup of the System.Windows.Media.MediaStreamSource.
        public abstract void CloseMedia();

        //
        // Summary:
        //     Developers call this method whenever an unrecoverable error has occurred
        //     in the System.Windows.Media.MediaStreamSource. This will cause the System.Windows.Controls.MediaElement.MediaFailed
        //     event to be raised.
        //
        // Parameters:
        //   errorDescription:
        //     A string describing the error.
        public void ErrorOccurred(string errorDescription)
        {
            _service.ErrorOccurred(errorDescription);
        }

        //
        // Summary:
        //     The System.Windows.Controls.MediaElement can call this method to request
        //     information about the System.Windows.Media.MediaStreamSource. Developers
        //     respond to this method by calling System.Windows.Media.MediaStreamSource.ReportGetDiagnosticCompleted(System.Windows.Media.MediaStreamSourceDiagnosticKind,System.Int64).
        //
        // Parameters:
        //   diagnosticKind:
        //     A member of the System.Windows.Media.MediaStreamSourceDiagnosticKind enumeration
        //     describing what type of information is desired.
        public abstract void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind);
        //
        // Summary:
        //     The System.Windows.Controls.MediaElement calls this method to ask the System.Windows.Media.MediaStreamSource
        //     to prepare the next System.Windows.Media.MediaStreamSample of the requested
        //     stream type for the media pipeline. Developers can respond to this method
        //     by calling either System.Windows.Media.MediaStreamSource.ReportGetSampleCompleted(System.Windows.Media.MediaStreamSample)
        //     or System.Windows.Media.MediaStreamSource.ReportGetSampleProgress(System.Double).
        //
        // Parameters:
        //   mediaStreamType:
        //     The description of the stream that the next sample should come from which
        //     will be either System.Windows.Media.MediaStreamType.Audio or System.Windows.Media.MediaStreamType.Video.
        public abstract void GetSampleAsync(MediaStreamType mediaStreamType);

        //
        // Summary:
        //     The System.Windows.Controls.MediaElement calls this method to ask the System.Windows.Media.MediaStreamSource
        //     to open the media.
        public abstract void OpenMediaAsync();

        //
        // Summary:
        //     Developers call this method, in response to System.Windows.Media.MediaStreamSource.GetDiagnosticAsync(System.Windows.Media.MediaStreamSourceDiagnosticKind),
        //     to pass the requested diagnostic information to the System.Windows.Controls.MediaElement.
        //
        // Parameters:
        //   diagnosticKind:
        //     The type of diagnostic that you are returning a value for.
        //
        //   diagnosticValue:
        //     The value associated with the diagnostics that was returned.
        protected void ReportGetDiagnosticCompleted(MediaStreamSourceDiagnosticKind diagnosticKind, long diagnosticValue)
        {
            // _service.ReportGetDiagnosticCompleted(diagnosticKind, diagnosticValue);
        }

        //
        // Summary:
        //     Developers call this method in response to System.Windows.Media.MediaStreamSource.GetSampleAsync(System.Windows.Media.MediaStreamType)
        //     to give the System.Windows.Controls.MediaElement the next media sample to
        //     be rendered, or to report the end of a stream.
        //
        // Parameters:
        //   mediaStreamSample:
        //     The description of the media stream that this sample came from. Passing nullSystem.Windows.Media.MediaStreamSample
        //     object indicates that the stream has ended.
        protected void ReportGetSampleCompleted(MediaStreamSample mediaStreamSample)
        {
            _service.ReportGetSampleCompleted(mediaStreamSample.InnerSample);
        }

        //
        // Summary:
        //     Developers call this method in response to System.Windows.Media.MediaStreamSource.GetSampleAsync(System.Windows.Media.MediaStreamType)
        //     to inform the System.Windows.Controls.MediaElement that it will not return
        //     a sample right now, because the System.Windows.Media.MediaStreamSource needs
        //     to refill its buffers, and to allow the System.Windows.Controls.MediaElement
        //     to transition to the System.Windows.Media.MediaElementState.Buffering state.
        //
        // Parameters:
        //   bufferingProgress:
        //     A value between 0 and 1 indicating what percentage of the buffer is filled.
        protected void ReportGetSampleProgress(double bufferingProgress)
        {
            //_service.ReportGetSampleProgress(bufferingProgress);
        }

        //
        // Summary:
        //     Developers call this method in response to System.Windows.Media.MediaStreamSource.OpenMediaAsync()
        //     to inform the System.Windows.Controls.MediaElement that the System.Windows.Media.MediaStreamSource
        //     has been opened and to supply information about the streams it contains.
        //
        // Parameters:
        //   mediaStreamAttributes:
        //     A collection of attributes describing features of the entire media source.
        //     Currently supported attributes are listed in the System.Windows.Media.MediaSourceAttributesKeys.
        //
        //   availableMediaStreams:
        //     A description of each audio and video stream contained within the content.
        protected void ReportOpenMediaCompleted(IPropertySet mediaStreamAttributes, IEnumerable<IPropertySet> availableMediaStreams)
        {
            _service.ReportOpenMediaCompleted(mediaStreamAttributes, availableMediaStreams.ElementAt(0));
        }

        //
        // Summary:
        //     Developers call this method in response to System.Windows.Media.MediaStreamSource.SeekAsync(System.Int64)
        //     to inform the System.Windows.Controls.MediaElement that the System.Windows.Media.MediaStreamSource
        //     has finished the requested position change and that future calls to System.Windows.Media.MediaStreamSource.ReportGetSampleCompleted(System.Windows.Media.MediaStreamSample)
        //     will return samples from that point in the media.
        //
        // Parameters:
        //   timeSeekedTo:
        //     The time as represented by 100 nanosecond increments where the actual seek
        //     took place. This is typically measured from the beginning of the media file.
        protected void ReportSeekCompleted(long timeSeekedTo)
        {
            //_service.ReportSeekCompleted(timeSeekedTo);
        }

        //
        // Summary:
        //     Developers call this method in response to System.Windows.Media.MediaStreamSource.SwitchMediaStreamAsync(System.Windows.Media.MediaStreamDescription)
        //     to inform the System.Windows.Controls.MediaElement that the System.Windows.Media.MediaStreamSource
        //     has completed the requested stream switch and that samples returned will
        //     now be from the requested stream instead of the original stream. Note, that
        //     this is meant for the multiple audio stream case, for example language tracks,
        //     and not the adaptive streaming case.
        //
        // Parameters:
        //   mediaStreamDescription:
        //     The description of the stream actually switched to. This should be the same
        //     stream as what was requested by System.Windows.Media.MediaStreamSource.SwitchMediaStreamAsync(System.Windows.Media.MediaStreamDescription).
        protected void ReportSwitchMediaStreamCompleted(MediaStreamDescription mediaStreamDescription)
        {
            //_service.ReportSwitchMediaStreamCompleted(mediaStreamDescription);
        }
        //
        // Summary:
        //     The System.Windows.Controls.MediaElement calls this method to ask the System.Windows.Media.MediaStreamSource
        //     to seek to the nearest randomly accessible point before the specified time.
        //     Developers respond to this method by calling System.Windows.Media.MediaStreamSource.ReportSeekCompleted(System.Int64)
        //     and by ensuring future calls to System.Windows.Media.MediaStreamSource.ReportGetSampleCompleted(System.Windows.Media.MediaStreamSample)
        //     will return samples from that point in the media.
        //
        // Parameters:
        //   seekToTime:
        //     The time as represented by 100 nanosecond increments to seek to. This is
        //     typically measured from the beginning of the media file.
        public abstract void SeekAsync(long seekToTime);
        
        //
        // Summary:
        //     Called when a stream switch is requested on the System.Windows.Controls.MediaElement.
        //
        // Parameters:
        //   mediaStreamDescription:
        //     The stream switched to.
        public abstract void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription);

        private MediaExtensionManager _extensionManager;
        private MSSWinRTExtension.MediaStreamSourceService _service;

        /// <summary>
        /// Registers element for current media stream source
        /// </summary>
        /// <param name="element"></param>
        private void RegisterWith(MediaElement element)
        {
            if (_extensionManager != null)
            {
                throw new InvalidOperationException("MediaStreamSource is already registered with a media element.");
            }

            _extensionManager = new MediaExtensionManager();
            var config = new Windows.Foundation.Collections.PropertySet();

            // Registration of the scheme handler is global, so we need to get unique scheme so our 
            // plugin will be used only for our this one instance of media element.
            string uri = "samplemss-" + element.GetHashCode() + ":";
            config.Add("plugin", new MMSWinRTPlugin(this));
            _extensionManager.RegisterSchemeHandler("MSSWinRTExtension.MediaStreamSchemeHandler", uri, config);

            element.Source = new Uri(uri);
        }

        internal void SetService(MSSWinRTExtension.MediaStreamSourceService pService)
        {
            _service = pService;
        }

        internal void Unregister()
        {
            _extensionManager = null;
            _service = null;
        }
    }

    // Summary:
    //     Describes the type of diagnostic information used by the media.
    public enum MediaStreamSourceDiagnosticKind
    {
        // Summary:
        //     Represents a download buffer in milliseconds.
        BufferLevelInMilliseconds = 1,
        //
        // Summary:
        //     Represents a download buffer in bytes.
        BufferLevelInBytes = 2,
    }

    // Summary:
    //     Enumeration that specifies the type of stream.
    public enum MediaStreamType
    {
        // Summary:
        //     The stream is an audio stream.
        Audio = 0,
        //
        // Summary:
        //     The stream is a video stream.
        Video = 1,
        //
        // Summary:
        //     The stream is a script stream. Note: Currently script commands are not supported
        //     in System.Windows.Media.MediaStreamSource.
        Script = 2,
    }

    // Summary:
    //     This class describes a media stream in enough detail to initialize the System.Windows.Controls.MediaElement
    //     and the underlying media pipeline.
    public class MediaStreamDescription
    {
        // Summary:
        //     Initializes a new instance of the System.Windows.Media.MediaStreamDescription
        //     class with the specified values.
        //
        // Parameters:
        //   type:
        //     A value from the enumeration, which is either audio or video.
        //
        //   mediaStreamAttributes:
        //     A collection of pairs describing other attributes of the overall Media Stream.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The specified type is not audio or video.
        public MediaStreamDescription(MediaStreamType type, IDictionary<MediaStreamAttributeKeys, string> mediaStreamAttributes)
        {
            c_MediaStreamAttributes = mediaStreamAttributes;
            c_MediaStreamType = type;
            return;
        }
        MediaStreamType c_MediaStreamType = MediaStreamType.Audio;
        IDictionary<MediaStreamAttributeKeys, string> c_MediaStreamAttributes;

        // Summary:
        //     Gets a collection of attributes about the media stream. This collection only
        //     contains attributes that are used to initialize the media pipeline and the
        //     System.Windows.Controls.MediaElement.
        //
        // Returns:
        //     A collection of attributes about the media stream. The default value is Empty
        //     IDictionary<MediaStreamAttributeKeys, string>.
        public IDictionary<MediaStreamAttributeKeys, string> MediaAttributes
        {
            get
            {
                return c_MediaStreamAttributes;
            }
        }
        //
        // Summary:
        //     Gets the stream ID of the stream being described.
        //
        // Returns:
        //     The stream ID of the stream being described.
        public int StreamId
        {
            get
            {
                return 0;
            }
        }
        //
        // Summary:
        //     Gets the type of stream that is being described which is either System.Windows.Media.MediaStreamType.Audio
        //     or System.Windows.Media.MediaStreamType.Video.
        //
        // Returns:
        //     The type of stream that is being described which is either System.Windows.Media.MediaStreamType.Audio
        //     or System.Windows.Media.MediaStreamType.Video. The default value is System.Windows.Media.MediaStreamType.Video.
        public MediaStreamType Type
        {
            get
            {
                return c_MediaStreamType;
            }
        }
    }

    // Summary:
    //     This enumeration is used in a dictionary of attributes for media streams.
    public enum MediaStreamAttributeKeys
    {
        // Summary:
        //     Codec data that the pipeline needs to initialize and render correctly. For
        //     video, this is other header information. For audio, this is the base16-encoded
        //     WaveFormatEx structure. For more information on CodecPrivateDate, see Implementing
        //     MediaStream Sources and Media in Silverlight for Windows Phone.
        CodecPrivateData = 0,
        //
        // Summary:
        //     Data needed to instantiate a video codec. This is the four-character value
        //     also known as a FourCC. For more information on VideoFourCC CodecPrivateDate,
        //     see Implementing MediaStream Sources and Media in Silverlight for Windows
        //     Phone.
        VideoFourCC = 1,
        //
        // Summary:
        //     The maximum width of reported video frames for this stream and the default
        //     width to render them at.
        Width = 2,
        //
        // Summary:
        //     The maximum height of reported video frames for this stream and the default
        //     width to render them at.
        Height = 3,
    }
    // Summary:
    //     This enumeration is used in a dictionary of attributes for media samples.
    public enum MediaSampleAttributeKeys
    {
        // Summary:
        //     For video samples, the presence of this attribute indicates the sample is
        //     a is a keyframe.   Silverlight for Windows Phone For audio samples, the presence
        //     of this attribute indicates a discontinuity (Silverlight for Windows Phone
        //     only).
        KeyFrameFlag = 0,
        //
        // Summary:
        //     Provides data about the media sample that is needed to decrypt it.
        DRMInitializationVector = 1,
        //
        // Summary:
        //     The width of the video frame. If this attribute is not specified, the width
        //     of the sample is assumed to be the width value defined on the System.Windows.Media.MediaStreamAttributeKeys
        //     passed into to the System.Windows.Media.MediaStreamSource.ReportOpenMediaCompleted(System.Collections.Generic.IDictionary<System.Windows.Media.MediaSourceAttributesKeys,System.String>,System.Collections.Generic.IEnumerable<System.Windows.Media.MediaStreamDescription>)
        //     method.
        FrameWidth = 2,
        //
        // Summary:
        //     The height of the video frame. If this attribute is not specified, the width
        //     of the sample is assumed to be the height value defined on the System.Windows.Media.MediaStreamAttributeKeys
        //     passed into to the System.Windows.Media.MediaStreamSource.ReportOpenMediaCompleted(System.Collections.Generic.IDictionary<System.Windows.Media.MediaSourceAttributesKeys,System.String>,System.Collections.Generic.IEnumerable<System.Windows.Media.MediaStreamDescription>)
        //     method.
        FrameHeight = 3,
        //
        // Summary:
        //     Provides data about which portions of a media sample are encrypted. This
        //     value is a B64 encoded array of one or more 32bit pairs. The first value
        //     in a pair is the length of encrypted data, the second is the length of clear
        //     (non-encrypted) data. If supplied, the array values must total to the length
        //     of the ntire sample. If not supplied, any encryption is assume to cover the
        //     entire sample. the length of encrypted data and clear data (not encrypted
        //     data) in an H264 media sample.This attribute is supported for H.264 on both
        //     Windows Phone 7 and Silverlight 4.
        DRMSubSampleMapping = 4,
    }
    // Summary:
    //     Describes the media source.
    public enum MediaSourceAttributesKeys
    {
        // Summary:
        //     A Boolean value that describes whether this source can seek.
        CanSeek = 0,
        //
        // Summary:
        //     The length of playback time of this source as an integer in 100-nanosecond
        //     increments.
        Duration = 1,
        //
        // Summary:
        //     DRM data that the pipeline needs to initialize and decrypt correctly. This
        //     is the DRM header represented as a string.
        DRMHeader = 2,
    }
}
