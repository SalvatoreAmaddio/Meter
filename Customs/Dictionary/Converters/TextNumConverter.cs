using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Testing.Customs.Dictionary.Converters
{
    public class TextNumConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNumber = int.TryParse(value.ToString(), out _);
            if (isNumber) return value;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool isNumber = int.TryParse(value.ToString(), out _);
            if (isNumber) return value;
            else
            {
                MessageBox.Show("ONLY NUMBERS ALLOWED", "INPUT ERROR");
                return 0;
            }
        }
    }
}
