using Autoservice.Dialogs.Managers;
using Autoservice.ViewModel.Utils;
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
    public partial class AddOrderDialog
    {
        public AddOrderDialog(AddOrderManager aom)
        {
            InitializeComponent();

            DataContext = aom;
            aom.OnExit = Close;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var aom = (AddOrderManager)DataContext;
            if (aom == null)
                return;
            if(!UserService.Instance.IsAdmin)
                works.Columns.Remove(percentColumn);
        }
    }
}
