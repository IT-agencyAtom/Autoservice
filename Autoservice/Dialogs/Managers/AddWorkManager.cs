﻿using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autoservice.ViewModel.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Dialogs.Managers
{
    public class AddWorkManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        public string Title { get; set; }
        
        private Work _work;

        public Work Work
        {
            get { return _work; }
            set
            {
                if (_work == value)
                    return;

                _work = value;
                RaisePropertyChanged("Work");
            }
        }
        public string Price { get; set; }

        public bool PriceIsEnabled => _isEdit ? UserService.Instance.IsAdmin : true;


        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }


        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Work work)
        {
            _isEdit = true;

            initialize();
            Work = work;

            Title = "Изменить работу";
        }

        public void initializeAdd()
        {
            initialize();
            Work = new Work();
            Title = "Добавить работу";
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
                generalService.UpdateWork(Work);
            else
                generalService.AddWork(Work);
        }

        public override async void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
