namespace PriceSetterDesktop.Libraries.Convertors
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BooleanToTextConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            if (val)
            {
                var txt = (string)parameter;
                return txt is not null and "URL"
                    ? "آدرس سایت ثبت شده"
                    : txt is not null and "Data" ? "داده ای برای تامین کننده تعریف نشده" : (object)"";
            }
            else
            {
                var txt = (string)parameter;
                return txt is not null and "URL"
                    ? "آدرس سایت مشخص نشده"
                    : txt is not null and "Data" ? "داده ای برای تامین کننده تعریف نشده" : (object)"";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            var txt = (string)parameter;
            return txt is not null and "URL"
                ? val == "آدرس سایت ثبت شده" ? true : (object)false
                : txt is not null and "XPath" ? val == "آدرس منابع ثبت شده" ? true : (object)false : false;
        }
    }
}
