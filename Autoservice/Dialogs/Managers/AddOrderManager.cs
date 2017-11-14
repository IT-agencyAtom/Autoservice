using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel.Utils;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Autoservice.ViewModel.Utils;

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
        public ObservableCollection<SparePart> SpareParts { get; private set; }
        public ObservableCollection<OrderWorkModel> OrderWorks { get; private set; }
        public ObservableCollection<OrderSparePartModel> OrderSpareParts { get; private set; }
        
        public string[] Methods { get; private set; }
        public string[] Sources { get; private set; }

        public decimal TotalPrice
        {
            get { return OrderWorks.Sum(ow => ow.Price) - (SelectedClient?.Discount * OrderWorks.Sum(ow => ow.Price) / 100).GetValueOrDefault(); }
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
                RaisePropertyChanged();
                RaisePropertyChanged("TotalPrice");
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


        private bool _workAdded = false;
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
        public RelayCommand AddSparePartCommand { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Order order)
        {
            _isEdit = true;

            initialize();

            //var service = Get<IGeneralService>();
            Order = order;
            OrderWorks = new ObservableCollection<OrderWorkModel>(order.Works.Select(w => new OrderWorkModel(w)));
            foreach (var orderWork in OrderWorks)
                orderWork.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            OrderSpareParts = new ObservableCollection<OrderSparePartModel>(order.SpareParts.Select(s => new OrderSparePartModel(s)));
            foreach (var orderSparePart in OrderSpareParts)
            {
                orderSparePart.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            }

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
            Methods = EnumExtender.GetAllDescriptions(typeof(PaymentMethod));
            Sources = EnumExtender.GetAllDescriptions(typeof(SparePartSource));
            AddWorkCommand = new RelayCommand(AddNewWork);
            AddSparePartCommand = new RelayCommand(AddNewSparePart);

        }

        private void AddNewSparePart()
        {
            var newSparePart = new OrderSparePartModel { OrderId = Order.Id, IsNew=true };
            newSparePart.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            OrderSpareParts.Add(newSparePart);
            RaisePropertyChanged("OrderSpareParts");
        }

        private async void AddNewWork()
        {
            var newWork = new OrderWorkModel {OrderId = Order.Id, IsNew = true};
            newWork.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            OrderWorks.Add(newWork);
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
            var generalService = Get<IGeneralService>();

            Order.Works = OrderWorks.Select(w => new OrderWork(w)).ToList();
            Order.TotalPrice = OrderWorks.Sum(ow => ow.Price) - SelectedClient.Discount * OrderWorks.Sum(ow => ow.Price) / 100;

            Order.SpareParts = OrderSpareParts.Select(w => new OrderSparePart(w)).ToList();
            if (_isEdit)
                generalService.UpdateOrder(Order);
            else
                generalService.AddOrder(Order);
        }

        public async override void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Clients = new ObservableCollection<Client>(await Task.Run(() => service.GetAllClients()));
            Cars = new ObservableCollection<Car>(await Task.Run(() => service.GetAllCars()));
            Masters = new ObservableCollection<Master>(await Task.Run(() => service.GetAllMasters()));
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks()));
            SpareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts()));

            if (_isEdit)
            {
                SelectedClient = Clients.First(x => x.Id == Order.Car.ClientId);
                SelectedCar = Cars.First(x => x.Id == Order.CarId);
            }
            RaisePropertyChanged("Clients");
            RaisePropertyChanged("Cars");
            RaisePropertyChanged("Works");
            RaisePropertyChanged("Masters");
            RaisePropertyChanged("SpareParts");
            SetIsBusy(false);
        }
    }

    public class OrderWorkModel : ViewModelBase
    {
        public Guid Id { get; set; }
        public Guid WorkId { get; set; }
        private Work _work { get; set; }

        public Work Work
        {
            get { return _work; }
            set
            {
                if (_work?.Id != value?.Id)
                {
                    _work = value;

                    if(_work != null)
                        Price = _work.Price;
                }
            }
        }
        public Guid MasterId { get; set; }
        public Master Master { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;

                RaisePropertyChanged();
                RaisePropertyChanged("TotalPrice");
            }
        }

        public bool IsNew { get; set; }

        public OrderWorkModel()
        {
            Id = Guid.NewGuid();
            IsNew = false;
        }

        public OrderWorkModel(OrderWork work)
        {
            Id = work.Id;
            WorkId = work.WorkId;
            Work = work.Work;
            MasterId = work.MasterId;
            Master = work.Master;
            OrderId = work.OrderId;
            Order = work.Order;
            Price = work.Price;
            IsNew = work.IsNew;
        }
    }

    public class OrderSparePartModel: ViewModelBase
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public Guid SparePartId { get; set; }
        public SparePart SparePart { get; set; }
        public int Number { get; set; }
        
        public int Source { get; set; }
        public string StringSource { get {return _strings[Source]; } set {} }
        public bool IsNew { get; set; }
        private static string[] _strings;

        static OrderSparePartModel()
        {
            _strings = EnumExtender.GetAllDescriptions(typeof(SparePartSource));          
        }
        public OrderSparePartModel()
        {
            Id = Guid.NewGuid();
            IsNew = false;
        }
        public OrderSparePartModel(OrderSparePart orderSparePart)
        {
            Id = orderSparePart.Id;
            OrderId = orderSparePart.OrderId;
            Order = orderSparePart.Order;
            SparePartId = orderSparePart.SparePartId;
            SparePart = orderSparePart.SparePart;
            Number = orderSparePart.Number;
            Source = (int)orderSparePart.Source;
            StringSource = _strings[Source];
            IsNew = orderSparePart.IsNew;
        }

    }
}
