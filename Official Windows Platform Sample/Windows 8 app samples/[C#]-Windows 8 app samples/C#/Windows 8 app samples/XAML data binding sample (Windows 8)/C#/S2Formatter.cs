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
using Windows.UI.Xaml.Data;

namespace DataBinding
{
    // This value converter is used in Scenario 2. For more information on Value Converters, see http://go.microsoft.com/fwlink/?LinkId=254639#data_conversions
    public class S2Formatter : IValueConverter
    {
        //Convert the slider value into Grades
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            int _value;
            string _grade = string.Empty;
            //try parsing the value to int
            if (Int32.TryParse(value.ToString(), out _value))
            {
                if (_value < 50)
                    _grade = "F";
                else if (_value < 60)
                    _grade = "D";
                else if (_value < 70)
                    _grade = "C";
                else if (_value < 80)
                    _grade = "B";
                else if (_value < 90)
                    _grade = "A";
                else if (_value < 100)
                    _grade = "A+";
                else if (_value == 100)
                    _grade = "SUPER STAR!";
            }

            return _grade;
        }

        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
