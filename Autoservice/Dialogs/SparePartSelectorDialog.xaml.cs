using Autoservice.DAL.Entities;
using Autoservice.Dialogs.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Autoservice.Dialogs
{
    /// <summary>
    /// Interaction logic for AddWorkDialog.xaml
    /// </summary>
    public partial class SparePartSelectorDialog
    {
        public SparePartSelectorDialog(SparePartSelectorManager sparePartSelectorManager)
        {
            InitializeComponent();

            DataContext = sparePartSelectorManager;
            sparePartSelectorManager.OnExit = Close;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SparePartSelectorManager sm = (SparePartSelectorManager)DataContext;
            if (sm == null)
                return;
            sm.Checked.Add(((CheckBox)e.Source).DataContext as SparePart);
            
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SparePartSelectorManager sm = (SparePartSelectorManager)DataContext;
            if (sm == null)
                return;
            sm.Checked.Remove(((CheckBox)e.Source).DataContext as SparePart);
        }
    }
}
