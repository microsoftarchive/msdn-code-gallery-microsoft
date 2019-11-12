using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CSUWPAppCommandBindInDT.Converter
{
    class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "Male" : "Female";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string sex = value as string;

            if (sex == "Female")
                return false;
            return true;
        }
    }
}
