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
    /// Interaction logic for ClientScreen.xaml
    /// </summary>
    public partial class WarehouseScreen : UserControl
    {
        public WarehouseScreen()
        {
            InitializeComponent();
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            WarehouseManager wm = (WarehouseManager)DataContext;
            if(wm!=null)
                wm.SelectedItem = e.NewValue;
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            WarehouseManager wm = (WarehouseManager)DataContext;
            if (wm == null)
                return;
            if (!(e.Source is TextBlock))
                return;
            var node = e.Data.GetData(typeof(SparePart));
            if(node == null)
                node = e.Data.GetData(typeof(SparePartsFolder));
            var folder = ((TextBlock)e.Source).DataContext as SparePartsFolder;
            if (folder == node)
                return;
            wm.MoveNode((ITreeViewNode)node, folder);
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
                return;
            WarehouseManager wm = (WarehouseManager)DataContext;
            if (wm == null)
                return;
            if (!(e.Source is TextBlock))
                return;        
            DragDrop.DoDragDrop((TextBlock)e.Source,((TextBlock)e.Source).DataContext as ITreeViewNode, DragDropEffects.Move);
        }

        private void helper_Drop(object sender, DragEventArgs e)
        {
            WarehouseManager wm = (WarehouseManager)DataContext;
            if (wm == null)
                return;
            var node = e.Data.GetData(typeof(SparePart));
            if (node == null)
                node = e.Data.GetData(typeof(SparePartsFolder));
            wm.MoveNode((ITreeViewNode)node, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WarehouseManager wm = (WarehouseManager)DataContext;
            if (wm == null)
                return;
            if(!wm.PurchasePriceIsVisible)
                tree.Columns.Remove(purchase);
        }
    }
}
