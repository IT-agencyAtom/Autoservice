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
    class ClientsManager : PanelViewModelBase
    {
        private string _clientsFilterString;
        private ICollectionView _clientsView { get; set; }

        public ObservableCollection<Client> Clients { get; set; }
        public Client SelectedClient { get; set; }

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
            return client.Name.ToLower().Contains(ClientsFilterString);
        }

        public ClientsManager()
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
            Clients = new ObservableCollection<Client>(await Task.Run(() => service.GetAllClients()));
            RaisePropertyChanged("Clients");

            SetIsBusy(false);
        }
    }
}
