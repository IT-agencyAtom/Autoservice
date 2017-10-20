﻿using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autoservice.DAL.Entities.Car;

namespace Autoservice.Dialogs.Managers
{
    public class AddCarManager : PanelViewModelBase
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
            Title = "Edit Car";
        }

        public void initializeAdd()
        {
            initialize();

            Car = new Car();
            
            Title = "Add Car";
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
                        ButtonText = "Cancel"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => SaveHandler(),
                        ButtonIcon = "appbar_disk",
                        ButtonText = "Save"
                    }
                }
            };
            Types = Enum.GetNames(typeof(CarType));
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
                relevantAdsService.UpdateCar(Car);
            else
                relevantAdsService.AddCar(Car);
        }

        public override void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
