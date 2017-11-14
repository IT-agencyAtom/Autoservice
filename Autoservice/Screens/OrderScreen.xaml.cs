using Autoservice.DAL.Entities;
using Autoservice.Screens.Managers;
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

namespace Autoservice.Screens
{
    /// <summary>
    /// Interaction logic for OrderScreen.xaml
    /// </summary>
    public partial class OrderScreen : UserControl
    {
        public OrderScreen()
        {
            InitializeComponent();
        }

        private SolidColorBrush redColor = new SolidColorBrush(Colors.SandyBrown);
        private SolidColorBrush whiteColor = new SolidColorBrush(Colors.Gray);
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Order order = (Order)e.Row.DataContext;
            if (order.PreOrderDateTime != null)
                e.Row.Background = redColor;
            else
                e.Row.Background = whiteColor;
        }
    }
}
