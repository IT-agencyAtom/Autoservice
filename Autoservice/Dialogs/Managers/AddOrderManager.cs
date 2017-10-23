using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Dialogs.Managers
{
    public class AddOrderManager : PanelViewModelBase
    {
        private int _selectedStatus=-1;
        private int _selectedMethod=-1;
        private Client _selectedClient;
        private Car _selectedCar;
        public Action OnExit { get; set; }

        public string Title { get; set; }

        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<Car> Cars { get; private set; }

        public string[] Statuses { get; private set; }
        public string[] Methods { get; private set; }

        public int SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                Order.Status = (OrderStatus)value;
                RaisePropertyChanged("SelectedStatus");
            }
        }
        public int SelectedMethod {
            get { return _selectedMethod; }
            set
            {
                _selectedMethod = value;
                Order.PaymentMethod = (PaymentMethod)value;
                RaisePropertyChanged("SelectedMethod");
            }
        }

        public Client SelectedClient { get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                Order.Client = value;
                RaisePropertyChanged("SelectedClient");
            }
        }
        public Car SelectedCar { get { return _selectedCar; }
            set
            {
                _selectedCar = value;
                Order.Car = value;
                RaisePropertyChanged("SelectedCar");
            }
        }



        private Order _order;

        public Order Order
        {
            get { return _order; }
            set
            {
                if (_order == value)
                    return;

                _order = value;
                RaisePropertyChanged("Order");
            }
        }

        public bool IsNew => !_isEdit;
        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Order order)
        {
            _isEdit = true;

            initialize();

            Order = order;
            SelectedMethod = (int?)Order.PaymentMethod??-1;
            SelectedStatus = (int)Order.Status;
           
            RaisePropertyChanged("SelectedStatus");
            RaisePropertyChanged("SelectedMethod");
            Title = "Изменить заказ";
        }

        public void initializeAdd()
        {
            _isEdit = false;
            initialize();

            Order = new Order();

            Title = "Добавить заказ";
        }

        private void initialize()
        {
            Panel = new PanelManager
            {
                RightButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => CancelHandler(),
                        ButtonIcon = "appbar_undo",
                        ButtonText = "Отмена"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => SaveHandler(),
                        ButtonIcon = "appbar_disk",
                        ButtonText = "Сохранить"
                    }
                }
            };
            Statuses = Enum.GetNames(typeof(OrderStatus));
            Methods = Enum.GetNames(typeof(PaymentMethod));

        }
        private void CancelHandler()
        {
            OnExit();
        }

        private void SaveHandler()
        {
            Validate();
            if (HasErrors)
                return;

            WasChanged = true;

            OnExit();
        }

        public void Save2DB()
        {
            var relevantAdsService = Get<IGeneralService>();

            if (_isEdit)
                relevantAdsService.UpdateOrder(Order);
            else
                relevantAdsService.AddOrder(Order);
        }

        public async override void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Clients = new ObservableCollection<Client>(await Task.Run(() => service.GetAllClients()));
            Cars = new ObservableCollection<Car>(await Task.Run(() => service.GetAllCars()));
            if (_isEdit)
            {
                SelectedClient = Clients.First(x => x.Id == Order.Client.Id);
                SelectedCar = Cars.First(x => x.Id == Order.Car.Id);
            }
            RaisePropertyChanged("Clients");
            RaisePropertyChanged("Cars");
            SetIsBusy(false);
        }
    }
}
