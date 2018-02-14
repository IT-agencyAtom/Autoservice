using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel.Utils;
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
    public class AddClientManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }

        private Client _client;

        public bool DiscountIsEnabled => _isEdit ? UserService.Instance.IsAdmin : true;

        public Client Client
        {
            get { return _client; }
            set
            {
                if (_client == value)
                    return;

                _client = value;
                RaisePropertyChanged("Client");
            }
        }        
        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Client client)
        {
            _isEdit = true;

            initialize();

            Client = client;

            Title = "Изменить клиента";
        }

        public void initializeAdd()
        {
            initialize();

            Client = new Client();

            Title = "Добавить клиента";
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
                generalService.UpdateClient(Client);
            else
                generalService.AddClient(Client);
        }

        public override void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
