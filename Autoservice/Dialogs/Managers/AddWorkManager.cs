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
    public class AddWorkManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        private Master _selectedMaster;
        public string Title { get; set; }
        public ObservableCollection<Master> Masters { get; set; }
        public Master SelectedMaster { get { return _selectedMaster; } set { _selectedMaster = value;Work.Master = value; } }
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

        private Order _order;
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
            SelectedMaster = work.Master;
            Work = work;

            Title = "Изменить работу";
        }

        public void initializeAdd()
        {
            initialize();
            Work = new Work();
            Title = "Добавить работу";
        }

        public void initializeAdd(Order order)
        {
            initialize();
            Work = new Work();
            Title = "Добавить работу";
            _order = order;
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
            Work.MasterId = SelectedMaster.Id;
            Work.OrderId = _order.Id;
            if (_isEdit)
                relevantAdsService.UpdateWork(Work);
            else
                relevantAdsService.AddWork(Work);
        }

        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Masters = new ObservableCollection<Master>(await Task.Run(() => service.GetAllMasters()));
            SetIsBusy(false);
        }
    }
}
