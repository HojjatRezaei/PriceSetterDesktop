namespace PriceSetterDesktop.Libraries.Types.Enum.ViewConvertor
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal class ContainerTypeConverter : IValueConverter
    {
        //To View
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Array result = Enum.GetValues(typeof(ContainerType));
            for (int x = 0; x < result.Length; x++)
            {
                var extractedValue = result.GetValue(x);
                if (extractedValue == null) continue;
                if (extractedValue.Equals(value))
                {
                    return x;
                }
            }
            return -1;
        }
        //To Model
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Array result = Enum.GetValues(typeof(ContainerType));
            try
            {
                return result.GetValue((int)value) ?? string.Empty;
            }
            catch (Exception)
            {
                return "";
            }

        }
    }
}
