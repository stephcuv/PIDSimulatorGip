using System.Globalization;
using System.Windows.Data;

namespace PIDSimulatorGip.MVVM
{
   public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine($"[EnumToBooleanConverter] Convert called: Value={value}, Parameter={parameter}");

            if (value == null || parameter == null)
                return false;

            value = value.ToString();
            bool result = value.Equals(parameter);
            Console.WriteLine($"[EnumToBooleanConverter] Returning {result}");
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine($"[EnumToBooleanConverter] ConvertBack called: Value={value}, Parameter={parameter}");

            if (value is bool boolValue && boolValue)
            {
                Console.WriteLine($"[EnumToBooleanConverter] Returning Enum Value: {parameter}");
                return parameter;
            }
            return Binding.DoNothing;
        }
    }
}
