using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel.Utils;
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
        private int _selectedMethod=-1;
        private Client _selectedClient;
        private Car _selectedCar;
        public Action OnExit { get; set; }

        public string Title { get; set; }

        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<Car> Cars { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Work> Works { get; private set; }
        public ObservableCollection<OrderWork> OrderWorks { get; private set; }
        

        public string[] Methods { get; private set; }

        
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

        public RelayCommand AddWorkCommand { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Order order)
        {
            _isEdit = true;

            initialize();

            var service = Get<IGeneralService>();
            Order = service.GetOrderById(order.Id);
            SelectedMethod = (int?)Order.PaymentMethod??-1;
           
            RaisePropertyChanged("SelectedStatus");
            RaisePropertyChanged("SelectedMethod");
            Title = "Изменить заказ";
        }

        public void initializeAdd()
        {
            _isEdit = false;
            initialize();

            Order = new Order();
            Order.Activities.Add(new Activity { StartTime = DateTime.Now, UniqueString = RandomStrings.GetRandomString(10), User = UserService.Instance.CurrentUser, Status = ActivityStatus.New, Order = Order });
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
            Methods = Enum.GetNames(typeof(PaymentMethod));
            AddWorkCommand = new RelayCommand(AddNewWork);

        }

        private async void AddNewWork()
        {
            OrderWorks.Add(new OrderWork {OrderId = Order.Id});

            RaisePropertyChanged("OrderWorks");
            /*SetIsBusy(true);
            var addWorkManager = new AddWorkManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addWorkManager.initializeAdd(Order));
            var addClientDialog = new AddWorkDialog(addWorkManager);
            addClientDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addWorkManager.WasChanged)
                {
                    
                    Refresh();
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();*/
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

            Order.Works = OrderWorks.ToList();

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
            Masters = new ObservableCollection<Master>(await Task.Run(() => service.GetAllMasters()));
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks()));

            OrderWorks = new ObservableCollection<OrderWork>(Order.Works);

            if (_isEdit)
            {
                SelectedClient = Clients.First(x => x.Id == Order.Car.ClientId);
                SelectedCar = Cars.First(x => x.Id == Order.CarId);
            }
            RaisePropertyChanged("Clients");
            RaisePropertyChanged("Cars");
            SetIsBusy(false);
        }
    }
}
