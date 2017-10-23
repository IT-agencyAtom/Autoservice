﻿using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.Dialogs;
using Autoservice.Dialogs.Managers;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Autoservice.ViewModel.Utils;

namespace Autoservice.Screens.Managers
{
    class OrderManager : PanelViewModelBase
    {
        private Order _selectedOrder;
        private string _ordersFilterString;
        private ICollectionView _ordersView { get; set; }
        public Order SelectedOrder { get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged("SelectedOrder");
            }
        }
        public ObservableCollection<Order> Orders { get; set; }

        private Order _newOrder;

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
        public RelayCommand MouseDoubleClickCommand { get; set; }

        public OrderManager()
        {
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => AddHandler(),
                        ButtonIcon = "appbar_add",
                        ButtonText = "Добавить"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => EditHandler(),
                        ButtonIcon = "appbar_edit",
                        ButtonText = "Изменить"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => DeleteHandler(),
                        ButtonIcon = "appbar_delete",
                        ButtonText = "Удалить"
                    }
                },
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Обновить"
                    }
                }
            };
            MouseDoubleClickCommand = new RelayCommand(EditHandler);
        }
        private async void AddHandler()
        {
            _newOrder = new Order();
            _newOrder.PersonalNumber = RandomStrings.GetOrderNumber();
            AddClient();
        }
        private async void EditHandler()
        {
            if (SelectedOrder == null)
                return;

            SetIsBusy(true);

            var addManager = new AddOrderManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedOrder));

            var addDialog = new AddOrderDialog(addManager);

            addDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);

                if (addManager.WasChanged)
                {
                    await Task.Run(() => addManager.Save2DB());

                    Refresh();
                }

                SetIsBusy(false);
            };
            addDialog.Show();
        }
        private async void DeleteHandler()
        {
            if (SelectedOrder == null)
                return;

            var deleteDialogSettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                FirstAuxiliaryButtonText = "Отмена"
            };

            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow == null)
                return;

            SetIsBusy(true);

            var result =
                await
                    metroWindow.ShowMessageAsync("Подтвердите удаление заказа",
                        $"Вы уверен что хотите удалить заказ №{SelectedOrder.PersonalNumber}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var relevantAdsService = Get<IGeneralService>();
                relevantAdsService.DeleteOrder(SelectedOrder);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Заказ {SelectedOrder.PersonalNumber} был удалён");
            }

            SetIsBusy(false);
        }

        private async void AddClient()
        {
            SetIsBusy(true);
            var addClientManager = new AddClientManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addClientManager.initializeAdd());
            var addClientDialog = new AddClientDialog(addClientManager);
            addClientDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);
                if (addClientManager.WasChanged)
                {
                    await Task.Run(() => addClientManager.Save2DB());
                    AddCar();
                    Refresh();
                }
                _newOrder.Client = addClientManager.Client;
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }

        private async void AddCar()
        {
            SetIsBusy(true);
            var addCarManager = new AddCarManager() { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addCarManager.initializeAdd());
            var addClientDialog = new AddCarDialog(addCarManager);
            addClientDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);
                if (addCarManager.WasChanged)
                {
                    await Task.Run(() => addCarManager.Save2DB());
                    Refresh();
                    OpenNewOrderData();
                }
                _newOrder.Car = addCarManager.Car;
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }

        private void OpenNewOrderData()
        {
            SelectedOrder = _newOrder;
            EditHandler();
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
