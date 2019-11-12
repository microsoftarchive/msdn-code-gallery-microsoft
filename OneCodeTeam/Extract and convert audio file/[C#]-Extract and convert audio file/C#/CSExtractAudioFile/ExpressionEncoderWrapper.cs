/********************************** Module Header **********************************\
* Module Name:  ExpressionEncoderWrapper.cs
* Project:      CSExtractAudioFile
* Copyright (c) Microsoft Corporation.
*
* The ExpressionEncoderWrapper class contains helper methods for converting and 
* extracting .MP3/.MP4/.WMA into .MP4/.WMA file format.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Profiles;
using System.Collections.ObjectModel;


namespace CSExtractAudioFile
{
    public class ExpressionEncoderWrapper
    {
        /// <summary>
        /// Extract audio file.
        /// </summary>
        /// <param name="sourceAudioFile">The full path of the source audio file</param>
        /// <param name="outputDirectory">The output directory</param>
        /// <param name="outputAudioType">Output audio format (.mp4/.wma)</param>
        /// <param name="startpoint">
        /// The extracting startpoint in the source audio file in milliseconds
        /// </param>
        /// <param name="endpoint">
        /// The extracting endpoint in the source audio file in milliseconds
        /// </param>
        /// <returns>The full path of the output file name</returns>
        public static string ExtractAudio(string sourceAudioFile, string outputDirectory,
            OutputAudioType outputAudioType, double startpoint, double endpoint)
        {
            using (Job job = new Job())
            {
                MediaItem src = new MediaItem(sourceAudioFile);
                switch (outputAudioType)
                {
                    case OutputAudioType.MP4:
                        src.OutputFormat = new MP4OutputFormat();
                        src.OutputFormat.AudioProfile = new AacAudioProfile();
                        src.OutputFormat.AudioProfile.Codec = AudioCodec.AAC;
                        src.OutputFormat.AudioProfile.BitsPerSample = 24;
                        break;
                    case OutputAudioType.WMA:
                        src.OutputFormat = new WindowsMediaOutputFormat();
                        src.OutputFormat.AudioProfile = new WmaAudioProfile();
                        src.OutputFormat.AudioProfile.Bitrate = new VariableConstrainedBitrate(128, 192);
                        src.OutputFormat.AudioProfile.Codec = AudioCodec.WmaProfessional;
                        src.OutputFormat.AudioProfile.BitsPerSample = 24;
                        break;
                }

                TimeSpan spanStart = TimeSpan.FromMilliseconds(startpoint);
                src.Sources[0].Clips[0].StartTime = spanStart;
                TimeSpan spanEnd = TimeSpan.FromMilliseconds(endpoint);
                src.Sources[0].Clips[0].EndTime = spanEnd;

                job.MediaItems.Add(src);
                job.OutputDirectory = outputDirectory;
                job.Encode();

                return job.MediaItems[0].ActualOutputFileFullPath;
            }
        }
    }

    public enum OutputAudioType
    {
        WMA,
        MP4
    }
}
