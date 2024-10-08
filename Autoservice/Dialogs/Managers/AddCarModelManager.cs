﻿using Autoservice.DAL.Entities;
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
    public class AddCarModelManager : PanelViewModelBase
    {
        private int _selectedType=-1;

        public Action OnExit { get; set; }

        public string Title { get; set; }

        private Car _car;

        public Car Car
        {
            get { return _car; }
            set
            {
                if (_car == value)
                    return;
                _car = value;
                RaisePropertyChanged("Car");
            }
        }        

        public string[] Types { get;private set; }

        public int SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                Car.Type = (Car.CarType)value;
                RaisePropertyChanged("SelectedType");
           }
        }

        public ObservableCollection<string> Brands { get; set; }


        public string Brand { get; set; }

        

        private ICollectionView _carsView { get; set; }
        public ObservableCollection<Car> Cars { get; set; }

        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Car car)
        {
            _isEdit = true;
            initialize();
            Car = car;
            SelectedType = (int)Car.Type;
            Title = "Изменить машину";
        }

        public void initializeAdd()
        {
            initialize();

            Car = new Car();
            
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
            Types = EnumExtender.GetAllDescriptions(typeof(Car.CarType));
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
            Car.Brand = Brand;
            if (_isEdit)
                generalService.UpdateCar(Car);
            else
                generalService.AddCar(Car);
        }

        public override void Refresh()
        {
            var generalService = Get<IGeneralService>();

            Cars = new ObservableCollection<Car>(generalService.GetAllCars());
            Brands = new ObservableCollection<string>(Cars.Select(c => c.Brand).Distinct());            

            RaisePropertyChanged("Cars");
            RaisePropertyChanged("Brands");

            SetIsBusy(false);
        }
    }
}

