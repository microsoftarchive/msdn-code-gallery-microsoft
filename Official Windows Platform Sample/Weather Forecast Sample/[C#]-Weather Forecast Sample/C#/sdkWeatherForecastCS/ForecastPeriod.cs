/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.ComponentModel;

namespace sdkWeatherForecastCS
{

            /// <summary>
    /// Class for holding the forecast for a particular time period
    /// </summary>
    public class ForecastPeriod : INotifyPropertyChanged
    {
        #region member variables
        private string timeName;
        private int temperature;
        private int chancePrecipitation;
        private string weatherType;
        private string textForecast;
        private string conditionIcon;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public ForecastPeriod()
        {
        }


        public string TimeName
        {
            get
            {
                return timeName;
            }
            set
            {
                if (value != timeName)
                {
                    this.timeName = value;
                    NotifyPropertyChanged("TimeName");
                }
            }
        }

        public int Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                if (value != temperature)
                {
                    this.temperature = value;
                    NotifyPropertyChanged("Temperature");
                }
            }
        }


        public int ChancePrecipitation
        {
            get
            {
                return chancePrecipitation;
            }
            set
            {
                if (value != chancePrecipitation)
                {
                    this.chancePrecipitation = value;
                    NotifyPropertyChanged("ChancePrecipitation");
                }
            }
        }

        public string WeatherType
        {
            get
            {
                return weatherType;
            }
            set
            {
                if (value != weatherType)
                {
                    this.weatherType = value;
                    NotifyPropertyChanged("WeatherType");
                }
            }
        }

        public string TextForecast
        {
            get
            {
                return textForecast;
            }
            set
            {
                if (value != textForecast)
                {
                    this.textForecast = value;
                    NotifyPropertyChanged("TextForecast");
                }
            }
        }

        public string ConditionIcon
        {
            get
            {
                return conditionIcon;
            }
            set
            {
                if (value != conditionIcon)
                {
                    this.conditionIcon = value;
                    NotifyPropertyChanged("ConditionIcon");
                }
            }
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
