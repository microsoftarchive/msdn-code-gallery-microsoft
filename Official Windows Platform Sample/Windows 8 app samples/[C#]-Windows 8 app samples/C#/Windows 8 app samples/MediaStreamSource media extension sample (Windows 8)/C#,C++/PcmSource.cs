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

using Windows.Foundation.Collections;
using Windows.Storage.Streams;

namespace MediaStreamSource
{
    /// <summary>
    /// A simple media source generating audio wave
    /// </summary>
    public class PcmSource : MediaStreamSource
    {
        byte[] _pSample;
        const uint SampleRate = 44100;
        const uint Channels = 1;
        const uint BitsPerSample = 16;
        const long SampleDuration = 10000000;

        long nextTimestamp;

        /// <summary>
        /// Contructor
        /// </summary>
        public PcmSource()
        {
            Frequency = 440;
        }

        public override void CloseMedia()
        {
            _pSample = null;
        }

        public override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        public override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            Task.Run(() =>
            {
                MediaStreamSample sample;
                try
                {
                    IBuffer winrtbuf = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBuffer.Create(_pSample, 0, _pSample.Length, _pSample.Length);
                    sample = new MediaStreamSample(null, winrtbuf, 0, _pSample.Length, nextTimestamp, SampleDuration, null);
                    nextTimestamp += SampleDuration;
                }
                catch (Exception e)
                {
                    ErrorOccurred(e.Message);
                    return;
                }

                ReportGetSampleCompleted(sample);
            });
        }

        public override void OpenMediaAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    CreateSample();
                }
                catch (Exception e)
                {
                    ErrorOccurred(e.Message);
                    return;
                }

                PropertySet sourceAttr = new Windows.Foundation.Collections.PropertySet();
                PropertySet streamAttr = new Windows.Foundation.Collections.PropertySet();

                streamAttr.Add("SampleRate", SampleRate);
                streamAttr.Add("Channels", Channels);
                streamAttr.Add("BitsPerSample", BitsPerSample);

                ReportOpenMediaCompleted(
                    sourceAttr,
                    new IPropertySet[] { streamAttr });
            });
        }

        public override void SeekAsync(long seekToTime)
        {
            throw new NotImplementedException();
        }

        public override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a audio sample
        /// </summary>
        private void CreateSample()
        {
            const uint size = SampleRate * Channels * BitsPerSample / 8;
            _pSample = new byte[size];
            double period = (double)SampleRate / Frequency;

            for (uint i = 0; i < size; i += 2)
            {
                double val = Math.IEEERemainder(i, period);

                val = 2 * Math.PI * val;
                short res = (short)(Math.Sin(val) * (double)short.MaxValue);

                _pSample[i] = (byte)(res & 0x00ff);
                _pSample[i] = (byte)((res >> 8) & 0x00ff);
            }
        }

        public double Frequency
        {
            get;
            set;
        }
    }
}
