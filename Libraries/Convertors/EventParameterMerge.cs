namespace PriceSetterDesktop.Libraries.Convertors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class EventParameterMerge : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is object[] castedParam)
            {
                List<object> objectList = [value];
                castedParam.ToList().ForEach(objectList.Add);
                return objectList.ToArray().Clone();
            }
            else
            {
                object[] pars = [value, parameter];
                return pars;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
