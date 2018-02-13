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
using System.Windows.Forms;

namespace Autoservice.Dialogs.Managers
{
    public class AddOrderManager : PanelViewModelBase
    {
        private int _selectedMethod=-1;
        private Client _selectedClient;
        private ClientCar _selectedCar;

        public Action OnExit { get; set; }

        public string Title { get; set; }

        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<ClientCar> Cars { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Work> Works { get; private set; }
        public ObservableCollection<SparePart> SpareParts { get; private set; }
        public ObservableCollection<OrderWorkModel> OrderWorks { get; private set; }
        public ObservableCollection<OrderSparePartModel> OrderSpareParts { get; private set; }


        public string[] Methods { get; private set; }
        public string[] Sources { get; private set; }

        public string TotalPriceStr => TotalPrice.ToString("#.##");
        public decimal TotalPrice => (GetWorksSum + GetSparesSum) * GetClientDiscount;

        public decimal GetClientDiscount => SelectedClient == null ? 1 : (1 - (decimal)SelectedClient.Discount / 100);

        public decimal GetWorksSum => OrderWorks.Sum(ow => ow.Price);

        public decimal GetSparesSum => OrderSpareParts.Where(osp => osp.Source != (int) SparePartSource.FromClient)
            .Sum(sp => sp.Number * sp.SparePart.Price);

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
                RaisePropertyChanged("TotalPriceStr");
            }
        }
        public ClientCar SelectedCar { get { return _selectedCar; }
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
            OrderSpareParts = new ObservableCollection<OrderSparePartModel>(order.SpareParts.Select(w => new OrderSparePartModel(w)));
            foreach (var orderSparePart in OrderSpareParts)
                orderSparePart.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };

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
            AddSparePartCommand = new RelayCommand(GetParts);

        }

        private void ChangeSparePartCollection(List<SparePart> _newParts)
        {
            foreach (var part in _newParts)
            {
                var sparePartModel = new OrderSparePartModel(new OrderSparePart { IsNew = true, Number = 1, Order = Order, OrderId = Order.Id, Source = 0, SparePart = part, SparePartId = part.Id });
                sparePartModel.PropertyChanged += (s, e) =>
                {
                    RaisePropertyChanged(e.PropertyName);
                };
                OrderSpareParts.Add(sparePartModel);
            }
            RaisePropertyChanged("OrderSpareParts");
        }

        private async void GetParts()
        {
            SetIsBusy(true);
            var manager = new SparePartSelectorManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => manager.Initialize(Order));
            var addDialog = new SparePartSelectorDialog(manager);
            var _newParts = new List<SparePart>();
            addDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);
                if (manager.WasChanged)
                {
                    _newParts = manager.Checked;
                    ChangeSparePartCollection(_newParts);              
                }
                SetIsBusy(false);
            };
            addDialog.Show();
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
            foreach (var item in OrderSpareParts)
            {
                if (item.Source == 0 && item.IsNew)
                {
                    if (item.Number > item.SparePart.Number)
                    {
                        MessageBox.Show("На складе нет требуемого кол-ва запчастей");
                        return;
                    }
                }
            }
            if (HasErrors)
                return;

            WasChanged = true;

            OnExit();
        }

        public void Save2DB()
        {

            var generalService = Get<IGeneralService>();

            Order.Works = OrderWorks.Select(w => new OrderWork(w)).ToList();
            Order.SpareParts = OrderSpareParts.Select(s => new OrderSparePart(s)).ToList();
            Order.TotalPrice = TotalPrice;
            
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
            Cars = new ObservableCollection<ClientCar>(await Task.Run(() => service.GetAllClientCars()));
            Masters = new ObservableCollection<Master>(await Task.Run(() => service.GetAllMasters()));
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks()));
            SpareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts()));

            if (_isEdit)
            {
                SelectedClient = Clients.First(x => x.Id == Order.Car.ClientId);
                SelectedCar = Cars.First(x => x.Id == Order.ClientCarId);
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
                RaisePropertyChanged("TotalPriceStr");
            }
        }

        public bool IsNew { get; set; }
        public int MasterPercentage { get; set; }

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
            MasterPercentage = work.MasterPercentage;
        }
    }

    public class OrderSparePartModel : ViewModelBase
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public Guid SparePartId { get; set; }

        private SparePart _sparePart;
        public SparePart SparePart {
            get { return _sparePart; }
            set
            {
                if (_sparePart?.Id != value?.Id)
                {
                    _sparePart = value;

                    if (_sparePart != null)
                        Price = _sparePart.Price;
                }
            }
        }

        private int _number;
        public int Number {
            get { return _number; }
            set { _number = value; RaisePropertyChanged("TotalPriceStr"); }
        }

        private int _source;
        public int Source { get { return _source; } set { _source = value; RaisePropertyChanged("TotalPriceStr"); } }
        public string StringSource { get { return _strings[Source]; } set { } }
        public bool IsNew { get; set; }

        private static string[] _strings;

        private decimal? _price;
        public decimal? Price
        {
            get { return _price; }
            set
            {
                _price = value;

                RaisePropertyChanged();
                RaisePropertyChanged("TotalPriceStr");
            }
        }

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
            Price = orderSparePart.SparePart.Price;
            Source = (int)orderSparePart.Source;
            StringSource = _strings[Source];
            IsNew = orderSparePart.IsNew;
        }
    }
}
