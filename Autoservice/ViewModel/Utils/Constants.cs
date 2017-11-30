using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NLog;

namespace Autoservice.ViewModel.Utils
{
    public static class Constants
    {
        public const string ExitScreenName = "Log Out";

        public static void printException(Logger _logger, Exception ex)
        {
            var errorMessage = "====== > Modules versions < ======= \n\n\n";
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in loadedAssemblies)
            {
                if (assembly.GetName().Name.Equals(Process.GetCurrentProcess().ProcessName))
                    errorMessage += $"=== > Client # Version: {assembly.GetName().Version}, Path: {assembly.Location}\n";
                
            }

            errorMessage += $"\n\n Error: {ex.Message}\n Error stack: {ex.StackTrace}\n Error (inner):";

            while (ex.InnerException != null)
            {
                errorMessage += "\n inner: " + ex.InnerException;
                ex = ex.InnerException;
            }

            _logger.Error(errorMessage);
        }
    }

    public class NullToVisibilityBackConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}