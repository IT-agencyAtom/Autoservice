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
using System.Windows;

namespace Autoservice.Dialogs.Managers
{
    public class AddCarSelectorManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }
        private Client _client;
        public string Title { get; set; }
        public bool IsNewCar = false;
        public List<Car> Cars { get; set; }
        public Car SelectedCar { get { return _car; }
            set
            {
                _car = value;
                IsNewCar = false;    
                RaisePropertyChanged("Car");
            }
        }

        private Car _car;      
      
        //Комманды
        //public RelayCommand Save { get; private set; }

        // public RelayCommand Cancel { get; private set; }



        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>        

        public void initialize(Client client)
        {
            Title = "Выберите машину";
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => AddNewCar(),
                        ButtonIcon = "appbar_user_add",
                        ButtonText = "Добавить новую"                        
                    }
                },

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
                        OnButtonAction = (obj) => NavigateNext(),
                        ButtonIcon = "appbar_navigate_next",
                        ButtonText = "Выбрать"
                    }
                }
            };
            Cars = client.Cars;
            _client = client;
        }
        private void CancelHandler()
        {
            OnExit();
        }

        private async void AddNewCar()
        {
            SetIsBusy(true);
            var addCarManager = new AddCarManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addCarManager.initializeAdd());
            var addCarDialog = new AddCarDialog(addCarManager);
            addCarDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addCarManager.WasChanged)
                {                    
                    SelectedCar = addCarManager.Car;
                    SelectedCar.ClientId = _client.Id;
                    IsNewCar = true;                  
                    NavigateNext();                    
                    Refresh();
                }
                SetIsBusy(false);
            };
            addCarDialog.Show();
        }
        private void NavigateNext()
        {          
            Validate();
            if (SelectedCar==null)
                return;
            WasChanged = true;
            OnExit();
        }       
        public override async void Refresh()
        {
            SetIsBusy(false);
        }
    }
}