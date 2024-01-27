namespace PriceSetterDesktop.Libraries.Convertors
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;

    public class BooleanToTextConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            if (val)
            {
                return "آدرس سایت ثبت شده";
            }
            else
            {
                return "آدرس سایت ثبت نشده";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            if(val == "آدرس سایت ثبت شده")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
