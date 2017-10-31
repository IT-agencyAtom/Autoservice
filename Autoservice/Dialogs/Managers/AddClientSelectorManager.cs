using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Autoservice.Dialogs.Managers
{
    public class AddClientSelectorManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }
        public Client Client { get; set; }

        private string _clientsFilterString;
        private ICollectionView _clientsView { get; set; }

        public ObservableCollection<Client> Clients { get; set; }

        public string ClientsFilterString
        {
            get { return _clientsFilterString; }
            set
            {
                if (_clientsFilterString == value)
                    return;

                _clientsFilterString = value.ToLower();

                _clientsView = CollectionViewSource.GetDefaultView(Clients)
                    ;
                _clientsView.Filter = ClientsFilter;
                _clientsView.MoveCurrentToFirst();

                RaisePropertyChanged("ClientsFilterString");

            }
        }
        private bool ClientsFilter(object item)
        {
            var client = item as Client;
            if (client == null)
                return false;
            if (ClientsFilterString != null)
                if (StringFilter(client) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Client client)
        {
            return client.Name.ToLower().Contains(ClientsFilterString) || client.Phone.ToString().Contains(ClientsFilterString);
        }

        public RelayCommand SelectClient { get; private set; }
        
        //Комманды
        //public RelayCommand Save { get; private set; }

        // public RelayCommand Cancel { get; private set; }



        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>        

        public void initialize()
        {
            Title = "Выберите клиента";
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => AddNewClient(),
                        ButtonIcon = "appbar_user_add",
                        ButtonText = "Добавить нового"                        
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

            SelectClient = new RelayCommand(SelectClientHandler);
        }

        private void SelectClientHandler()
        {
            if (Client == null)
                return;

            NavigateNext();
        }

        private void CancelHandler()
        {
            OnExit();
        }

        private async void AddNewClient()
        {
            SetIsBusy(true);
            var addClientManager = new AddClientManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addClientManager.initializeAdd());
            var addClientDialog = new AddClientDialog(addClientManager);
            addClientDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addClientManager.WasChanged)
                {
                    var autoService = Get<IGeneralService>();
                    autoService.AddClient(addClientManager.Client);

                    Client = addClientManager.Client;
                    NavigateNext();
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }
        private void NavigateNext()
        {
            if (Client == null)
                return;

            Validate();
            
            WasChanged = true;
            OnExit();
        }
        
        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Clients = new ObservableCollection<Client>(await Task.Run(() => service.GetAllClients()));
            RaisePropertyChanged("Clients");
            SetIsBusy(false);
        }
    }
}
