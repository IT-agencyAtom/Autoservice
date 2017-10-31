using Autoservice.DAL.Entities;
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
                ChangeRightButtons();
                RaisePropertyChanged("SelectedOrder");
            }
        }
        public ObservableCollection<Order> Orders { get; set; }
        private PanelButtonManager _pbm;
        private PanelButtonManager _inExpectationpbm { get; set; }
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
                   order.Car.Client.Name.ToLower().Contains(OrdersFilterString) ||
                   order.Car.Client.Phone.ToLower().Contains(OrdersFilterString) ||
                   order.Car.ToString().ToLower().Contains(OrdersFilterString);
        }

        public RelayCommand MouseDoubleClickCommand { get; set; }

        public OrderManager()
        {
            _pbm = new PanelButtonManager
            {
                OnButtonAction = o => ChangeActivity(),
                ButtonIcon = "appbar_checkmark_pencil",
                ButtonText = "Change"
            };
            _inExpectationpbm = new PanelButtonManager
            {
                OnButtonAction = o => ChangeActivityExpactaition(),
                ButtonIcon = "appbar_timer",
                ButtonText = "В ожидании запчастей",
                ButtonVisibility = Visibility.Hidden
            };
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
                },
                RightButtons = new ObservableCollection<PanelButtonManager>
                {
                 _inExpectationpbm,
                 _pbm
                }
            };
            MouseDoubleClickCommand = new RelayCommand(EditHandler);
        }

        private void ChangeActivityExpactaition()
        {
            var oldActivity = SelectedOrder.Activities.Last();
            oldActivity.EndTime = DateTime.Now;
            var nextStatus = (oldActivity.Status == ActivityStatus.InOperation) ? ActivityStatus.InExpectationOfSpareParts : ActivityStatus.InOperation; 
            Activity activity = new Activity();
            activity.UniqueString = RandomStrings.GetRandomString(10);
            activity.StartTime = DateTime.Now;
            activity.OrderId = SelectedOrder.Id;
            activity.Status = nextStatus;
            activity.UserId = UserService.Instance.CurrentUser.Id;
            SelectedOrder.Activities.Add(activity);
            SaveActivity2DB(oldActivity, activity);
            ChangeRightButtons();
        }

        private async void ChangeActivity()
        {
            
            var oldActivity = SelectedOrder.Activities.Last();
            oldActivity.EndTime = DateTime.Now;
            var nextStatus = SelectedOrder.Activities.Last().GetNextStatus();
            if (nextStatus == null)
                return;
            Activity activity = new Activity();
            activity.UniqueString = RandomStrings.GetRandomString(10);
            activity.StartTime = DateTime.Now;
            activity.OrderId = SelectedOrder.Id;
            activity.Status = (ActivityStatus)nextStatus;
            activity.UserId = UserService.Instance.CurrentUser.Id;
            SelectedOrder.Activities.Add(activity);
            SaveActivity2DB(oldActivity, activity);
            ChangeRightButtons();

        }
        private void ChangeRightButtons()
        {
            if (_selectedOrder.Activities.Count == 0 || 
                _selectedOrder.Activities == null || 
                _selectedOrder.Activities.LastOrDefault().Status == ActivityStatus.Closed)
                _pbm.ButtonVisibility = Visibility.Hidden;
            else
            {
                _pbm.ButtonVisibility = Visibility.Visible;
                _pbm.ButtonText = _selectedOrder.Activities.LastOrDefault().GetNextStatus().ToDescriptionString();
                if (_selectedOrder.Activities.LastOrDefault().Status == ActivityStatus.InOperation)
                {
                    _inExpectationpbm.ButtonVisibility = Visibility.Visible;
                }
                else
                    _inExpectationpbm.ButtonVisibility = Visibility.Hidden;

            }
        }

        private void AddHandler()
        {
            _newOrder = new Order();
            _newOrder.StartDate = DateTime.Now;
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
            Refresh();
            SetIsBusy(false);
        }

        private Client _client;
        private async void AddClient()
        {
            SetIsBusy(true);
            var addClientManager = new AddClientSelectorManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addClientManager.initialize());
            var addClientDialog = new AddClientSelectorDialog(addClientManager);
            addClientDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addClientManager.WasChanged)
                {
                    _client = addClientManager.Client;
                    AddCar();
                    Refresh();
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }
        private async void AddCar()
        {
            SetIsBusy(true);
            var addCarManager = new AddCarSelectorManager() { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            addCarManager.initialize(_client);
            var addClientDialog = new AddCarSelectorDialog(addCarManager);
            addClientDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addCarManager.WasChanged)
                {
                    _newOrder.Car = addCarManager.Car;
                    _newOrder.CarId = addCarManager.Car.Id;
                    Refresh();
                    Save2DB();
                    SelectedOrder = _newOrder;
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }
        private void Save2DB()
        {
            var relevantAdsService = Get<IGeneralService>();
            relevantAdsService.AddOrder(_newOrder);

            var activity = new Activity
            {
                StartTime = DateTime.Now,
                UniqueString = RandomStrings.GetRandomString(10),
                UserId = UserService.Instance.CurrentUser.Id,
                Status = ActivityStatus.New,
                OrderId = _newOrder.Id
            };
            relevantAdsService.AddActivity(activity);
            Refresh();        
        }            

        public void SaveActivity2DB(Activity oldActivity, Activity newActivity)
        {
            var relevantAdsService = Get<IGeneralService>();            
            relevantAdsService.UpdateOrder(SelectedOrder);
            relevantAdsService.UpdateActivity(oldActivity);
            relevantAdsService.AddActivity(newActivity);
            Refresh();
        }
        public async override void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Orders = new ObservableCollection<Order>(await Task.Run(() => service.GetAllOrders()));
            var Users = new ObservableCollection<User>(await Task.Run(() => service.GetAllUsers()));

            foreach (var order in Orders)
            {
                order.Activities.Sort(Activity.CompareByStatus);
                order.Activities.ForEach(a => a.User = Users.First(u => u.Id == a.UserId));
            }
            _pbm.ButtonText = SelectedOrder?.Activities?.LastOrDefault()?.GetNextStatus()?.ToString() ?? "";
            RaisePropertyChanged("Orders");
            SetIsBusy(false);
        }

        public async Task<Order> GetOrder(Guid id)
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            var order = await Task.Run(() => service.GetOrderById(id));
            RaisePropertyChanged("Orders");
            SetIsBusy(false);
            return order;
        }
    }
}
