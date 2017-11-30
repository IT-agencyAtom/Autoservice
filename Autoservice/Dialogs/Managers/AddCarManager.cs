using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel.Utils;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Autoservice.Dialogs.Managers
{
    public class AddCarManager : PanelViewModelBase
    {
        private int _selectedType=-1;

        public Action OnExit { get; set; }

        public string Title { get; set; }

        private ClientCar _clientCar;

        public ClientCar ClientCar
        {
            get { return _clientCar; }
            set
            {
                if (_clientCar == value)
                    return;

                _clientCar = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Car");
            }
        }

        public Car Car
        {
            get { return ClientCar.Car; }
            set
            {
                if (ClientCar.Car == value)
                    return;

                ClientCar.Car = value;
                RaisePropertyChanged();
            }
        }

        /*public string[] Types { get;private set; }

        public int SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                Car.Type = (Car.CarType)value;
                RaisePropertyChanged("SelectedType");
            }
        }*/

        public ObservableCollection<string> Brands { get; set; }

        private string _brand;
        public string Brand
        {
            get { return _brand; }
            set
            {
                if (_brand == value)
                    return;

                if (value != null)
                {
                    _brand = value.ToLower();

                    _carsView = CollectionViewSource.GetDefaultView(Cars);
                    _carsView.Filter = CarsFilter;
                    _carsView.MoveCurrentToFirst();
                }

                RaisePropertyChanged();

            }
        }

        private bool CarsFilter(object item)
        {
            var car = item as Car;
            if (car == null)
                return false;
            if (Brand != null)
                if (StringFilter(car) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Car car)
        {
            return car.Brand.ToLower().Contains(Brand);
        }

        private ICollectionView _carsView { get; set; }
        public ObservableCollection<Car> Cars { get; set; }

        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(ClientCar car)
        {
            _isEdit = true;
            initialize();
            ClientCar = car;
            //SelectedType = (int)Car.Car.Type;
            Title = "Изменить машину";
        }

        public void initializeAdd()
        {
            initialize();

            ClientCar = new ClientCar();
            
            Title = "Добавить машину";
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
            //Types = EnumExtender.GetAllDescriptions(typeof(Car.CarType));
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

            if (_isEdit)
                generalService.UpdateClientCar(ClientCar);
            else
                generalService.AddClientCar(ClientCar);
        }

        public override void Refresh()
        {
            var generalService = Get<IGeneralService>();

            Cars = new ObservableCollection<Car>(generalService.GetAllCars());
            Brands = new ObservableCollection<string>(Cars.Select(c => c.Brand).Distinct());

            if(ClientCar.Car != null)
                Brand = ClientCar.Car.Brand;

            RaisePropertyChanged("Cars");
            RaisePropertyChanged("Brands");

            SetIsBusy(false);
        }
    }
}

