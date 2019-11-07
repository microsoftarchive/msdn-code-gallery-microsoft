// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicMediaPlayback.Common;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SDKTemplateCS
{
    public enum TooltipTextFormat
    {
        Seconds,
        Minutes,
        Hours
    }

    #region Media Position Binding Helper object

    public class MediaPositionBindingHelper : BindableBase
    {
        public MediaElement ReferenceMediaElement { set; get; }

        private double _mediapositionslidermaxvalue;
        public double MediaPositionSliderMaximum
        {
            set
            {
                this.SetProperty(ref _mediapositionslidermaxvalue , value);
            }
            get { return _mediapositionslidermaxvalue; }
        }
    }

    #endregion

    #region StorageFile Wrapper

    public class BindableStorageFile : BindableBase
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                this.SetProperty(ref _name, value);
            }
        }

        TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                this.SetProperty<TimeSpan>(ref _duration, value);
            }
        }

        StorageFile _file;
        public StorageFile File
        {
            get { return _file; }
            set
            {
                this.SetProperty<StorageFile>(ref _file, value);
            }
        }
    }

    #endregion

    #region Converters

    // Converts double to Timespan and Timespan to double for Media Position 
    // slider binding.
    public class MediaPositionTimeSpanToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan refval = (TimeSpan)value;

            if (refval.TotalSeconds == 0.0)
                return -1.0;

            if (MediaDurationToSliderValues.StepFrequencyValue == -1)
            {
                return 0;
            }

            return refval.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new TimeSpan(0, 0, (int)((double)value));
        }
    }

    // Uses NaturalDuration to set the StepFrequency on a Slider control
    // for Media Position slider binding.
    public class MediaDurationToSliderValues : IValueConverter
    {
        public static double StepFrequencyValue = -1;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Duration)
            {
                TimeSpan refval = ((Duration)value).TimeSpan;

                if (StepFrequencyValue == -1)
                {
                    if (refval.TotalSeconds == 0)
                    {
                        // This happens because of NaturalDuration not being bound before MediaOpened is fired.
                        return -1;
                    }

                    double absvalue = (int)Math.Round(refval.TotalSeconds, MidpointRounding.AwayFromZero);

                    StepFrequencyValue = (int)(Math.Round(absvalue / 100));

                    if (refval.TotalMinutes >= 10 && refval.TotalMinutes < 30)
                    {
                        StepFrequencyValue = 10;
                    }
                    else if (refval.TotalMinutes >= 30 && refval.TotalMinutes < 60)
                    {
                        StepFrequencyValue = 30;
                    }
                    else if (refval.TotalHours >= 1)
                    {
                        StepFrequencyValue = 60;
                    }

                    if (StepFrequencyValue == 0) StepFrequencyValue += 1;

                    if (StepFrequencyValue == 1)
                    {
                        StepFrequencyValue = absvalue / 100;
                    }
                }

                if (refval.Minutes == 0) MediaPositionTooltipTextValue.TooltipTextFormat = SDKTemplateCS.TooltipTextFormat.Seconds;
                else if (refval.Hours == 0) MediaPositionTooltipTextValue.TooltipTextFormat = SDKTemplateCS.TooltipTextFormat.Minutes;
                else MediaPositionTooltipTextValue.TooltipTextFormat = SDKTemplateCS.TooltipTextFormat.Hours;

                return StepFrequencyValue;
            }

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Not implemented as there is no two-binding used for Slider StepFrequency 
            // in this scenario.
            return 0.0;
        }
    }

    // Converts Timespan values to helpful text values for ToolTip rendering on 
    // the Media Position slider.
    public class MediaPositionTooltipTextValue : IValueConverter
    {
        public static TooltipTextFormat TooltipTextFormat;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan refval = TimeSpan.FromSeconds((double)value);
            if (MediaDurationToSliderValues.StepFrequencyValue == -1)
            {
                return 0;
            }

            string tooltiptext = string.Empty;
            if (TooltipTextFormat == SDKTemplateCS.TooltipTextFormat.Seconds)
            {
                tooltiptext = string.Format(" {0:00} s", refval.Seconds);
            }
            else if (TooltipTextFormat == SDKTemplateCS.TooltipTextFormat.Minutes)
            {
                tooltiptext = string.Format(" {0:00} : {1:00} m", refval.Minutes, refval.Seconds);
            }
            else
            {
                tooltiptext = string.Format(" {0:00} : {1:00} : {2:00} h ", refval.Hours, refval.Minutes, refval.Seconds);
            }

            return tooltiptext;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
    #endregion
}
