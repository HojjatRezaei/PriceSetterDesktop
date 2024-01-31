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
                var txt = (string)parameter;
                if (txt is not null and "URL")
                {
                    return "آدرس سایت ثبت شده";
                }
                else if (txt is not null and "Data")
                {
                    return "داده ای برای تامین کننده تعریف نشده";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                var txt = (string)parameter;
                if (txt is not null and "URL")
                {
                    return "آدرس سایت مشخص نشده";
                }
                else if (txt is not null and "Data")
                {
                    return "داده ای برای تامین کننده تعریف نشده";
                }
                else
                {
                    return "";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            var txt = (string)parameter;
            if (txt is not null and "URL")
            {
                if (val == "آدرس سایت ثبت شده")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (txt is not null and "XPath")
            {
                if (val == "آدرس منابع ثبت شده")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
    }
}
