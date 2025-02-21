using System;
using System.Globalization;
using System.Windows.Data;

namespace PIDSimulatorGip.MVVM
{
    public class LogConverter : IValueConverter
    {
        private const double MinValue = 1;    // Minimum logarithmic scale value
        private const double MaxValue = 100;  // Maximum logarithmic scale value
        private const double OutputMin = 0;   // Output range minimum
        private const double OutputMax = 2.5;   // Output range maximum

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double linearValue = (double)value; // Expecting input from 0 to 1

            // Convert linear (0-1) to logarithmic scale (MinValue to MaxValue)
            double logMin = Math.Log10(MinValue);
            double logMax = Math.Log10(MaxValue);
            double logValue = logMin + (logMax - logMin) * linearValue; // Interpolate log scale

            // Convert logarithmic scale (MinValue - MaxValue) to output range (0-5)
            double actualValue = Math.Pow(10, logValue); // Convert log back to normal scale
            return OutputMin + (OutputMax - OutputMin) * ((actualValue - MinValue) / (MaxValue - MinValue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double logValue = (double)value; // Input expected in range OutputMin to OutputMax

            // Normalize logValue to 0-1 range
            double normalizedValue = (logValue - OutputMin) / (OutputMax - OutputMin);

            // Convert back using logarithm (OutputMin - OutputMax → MinValue - MaxValue)
            double logMin = Math.Log10(MinValue);
            double logMax = Math.Log10(MaxValue);
            double actualLog = MinValue * Math.Pow(10, (logMin + normalizedValue * (logMax - logMin)));

            return (Math.Log10(actualLog) - logMin) / (logMax - logMin); // Return normalized 0-1 value
        }
    }
}
