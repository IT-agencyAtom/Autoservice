using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
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
    public class AddMasterManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }

        private Master _master;

        public Master Master
        {
            get { return _master; }
            set
            {
                if (_master == value)
                    return;

                _master = value;
                RaisePropertyChanged("Master");
            }
        }
        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Master master)
        {
            _isEdit = true;

            initialize();

            Master = master;

            Title = "Изменить мастера";
        }

        public void initializeAdd()
        {
            initialize();

            Master = new Master();

            Title = "Добавить мастера";
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
            var relevantAdsService = Get<IGeneralService>();

            if (_isEdit)
                relevantAdsService.UpdateMaster(Master);
            else
                relevantAdsService.AddMaster(Master);
        }

        public override void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
