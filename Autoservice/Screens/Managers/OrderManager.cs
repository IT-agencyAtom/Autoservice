using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Autoservice.Screens.Managers
{
    class OrderManager : PanelViewModelBase
    {
        private string _ordersFilterString;
        private ICollectionView _ordersView { get; set; }

        public ObservableCollection<Order> Orders { get; set; }

        public string OrdersFilterString
        {
            get { return _ordersFilterString; }
            set
            {
                if (_ordersFilterString == value)
                    return;

                _ordersFilterString = value.ToLower();

                _ordersView = CollectionViewSource.GetDefaultView(Orders)
                    ;
                _ordersView.Filter = OrdersFilter;
                _ordersView.MoveCurrentToFirst();

                RaisePropertyChanged("OrdersFilterString");

            }
        }
        private bool OrdersFilter(object item)
        {
            var order = item as Order;
            if (order == null)
                return false;
            if (OrdersFilterString != null)
                if (StringFilter(order) == false)
                    return false;            
            return true;
        }
        private bool StringFilter(Order order)
        {
            return order.PersonalNumber.ToLower().Contains(OrdersFilterString) ||
                   order.Client.ToString().ToLower().Contains(OrdersFilterString) ||
                   order.RepairZone.ToLower().Contains(OrdersFilterString) ||
                   order.Car.ToString().ToLower().Contains(OrdersFilterString) ||
                   order.Status.ToString().ToLower().Contains(OrdersFilterString);
        }

        public OrderManager()
        {
            Panel = new PanelManager
            {
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Refresh"
                    }
                },

                RightButtons = new ObservableCollection<PanelButtonManager>
                {

                }
            };            
        }

        public async override void Refresh()
        {
            SetIsBusy(true);

            var service = Get<IGeneralService>();
            Orders = new ObservableCollection<Order>(await Task.Run(() => service.GetAllOrders()));

            RaisePropertyChanged("Orders");

            SetIsBusy(false);
        }
    }
}
