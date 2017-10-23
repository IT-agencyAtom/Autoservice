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
using System.Windows;
using System.Windows.Data;
using Autoservice.Dialogs;
using Autoservice.Dialogs.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Autoservice.Screens.Managers
{
    class CarsManager : PanelViewModelBase
    {
        private string _carsFilterString;
        private ICollectionView _carsView { get; set; }

        public Car SelectedCar { get; set; }

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
        public RelayCommand MouseDoubleClickCommand { get; set; }

        public CarsManager()
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
            SetIsBusy(true);

            var addManager = new AddCarManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeAdd());

            var addDialog = new AddCarDialog(addManager);

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

        private async void EditHandler()
        {
            if (SelectedCar == null)
                return;

            SetIsBusy(true);

            var addManager = new AddCarManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedCar));

            var addDialog = new AddCarDialog(addManager);

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
            if (SelectedCar == null)
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
                    metroWindow.ShowMessageAsync("Подтвердите удаление машины",
                        $"Вы уверен что хотите удалить {SelectedCar.LocalName}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var relevantAdsService = Get<IGeneralService>();
                relevantAdsService.DeleteCar(SelectedCar);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Машина {SelectedCar.LocalName} была удалена");

                Refresh();
            }

            SetIsBusy(false);
        }

        public async override void Refresh()
        {
            SetIsBusy(true);

            var service = Get<IGeneralService>();
            Cars = new ObservableCollection<Car>(await Task.Run(() => service.GetAllCars()));
            RaisePropertyChanged("Cars");
            SetIsBusy(false);
        }
    }
}
