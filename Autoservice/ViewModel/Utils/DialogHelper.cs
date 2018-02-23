using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autoservice.Dialogs;

namespace Autoservice.ViewModel.Utils
{
    public static class DialogHelper
    {
        public static void ActivateDialog(Type window)
        {
            MetroWindow dialog = null;
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                var currentWindow = Application.Current.Windows[i];
                if (currentWindow != null && currentWindow.GetType() == window)
                {
                    dialog = Application.Current.Windows[i] as AddCarSelectorDialog;
                    break;
                }
            }
            dialog?.Activate();
        }
        public static async Task<string> ShowInputDialog(string title, string message)
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            return await metroWindow.ShowInputAsync(title, message);
        }
    }
}
