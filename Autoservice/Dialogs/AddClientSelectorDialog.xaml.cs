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
    /// Interaction logic for AddClientDialog.xaml
    /// </summary>
    public partial class AddClientSelectorDialog
    {
        public AddClientSelectorDialog(AddClientSelectorManager acsm)
        {
            InitializeComponent();
            DataContext = acsm;
            acsm.OnExit = Close;
        }
    }
}
