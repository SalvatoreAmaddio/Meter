using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Testing.RecordSource;

namespace Testing.Customs.Dictionary.Converters {

    public class ItemSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            IEnumerable x = (IEnumerable)value;
            var RecordSource = x.Cast<IRecordSource>().ToList();
            return RecordSource.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}