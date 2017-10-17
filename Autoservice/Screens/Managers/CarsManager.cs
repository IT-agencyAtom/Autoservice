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
    class CarsManager : PanelViewModelBase
    {
        private string _carsFilterString;
        private ICollectionView _carsView { get; set; }

        public ObservableCollection<Car> Cars { get; set; }

        public string CarsFilterString
        {
            get { return _carsFilterString; }
            set
            {
                if (_carsFilterString == value)
                    return;

                _carsFilterString = value.ToLower();

                _carsView = CollectionViewSource.GetDefaultView(Cars)
                    ;
                _carsView.Filter = CarsFilter;
                _carsView.MoveCurrentToFirst();

                RaisePropertyChanged("CarsFilterString");

            }
        }
        private bool CarsFilter(object item)
        {
            var car = item as Car;
            if (car == null)
                return false;
            if (CarsFilterString != null)
                if (StringFilter(car) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Car car)
        {
            return car.RegistrationNumber.ToLower().Contains(CarsFilterString) ||
                   car.Brand.ToString().ToLower().Contains(CarsFilterString) ||
                   car.Model.ToLower().Contains(CarsFilterString) ||
                   car.Type.ToString().ToLower().Contains(CarsFilterString);
        }

        public CarsManager()
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
            Cars = new ObservableCollection<Car>(await Task.Run(() => service.GetAllCars()));
            var Clients = new ObservableCollection<Client>(await Task.Run(() =>service.GetAllClients()));
            RaisePropertyChanged("Cars");

            SetIsBusy(false);
        }
    }
}
