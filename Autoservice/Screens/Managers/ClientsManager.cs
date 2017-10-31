using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.Dialogs;
using Autoservice.Dialogs.Managers;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            return client.Name.ToLower().Contains(ClientsFilterString)||client.Phone.ToString().Contains(ClientsFilterString);
        }

        public RelayCommand MouseDoubleClickCommand { get; set; }

        public ClientsManager()
        {
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {                    
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

        private async void EditHandler()
        {
            if (SelectedClient == null)
                return;

            SetIsBusy(true);

            var addManager = new AddClientManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedClient));

            var addDialog = new AddClientDialog(addManager);

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
            if (SelectedClient == null)
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
                    metroWindow.ShowMessageAsync("Подтвердите удаление клиента",
                        $"Вы уверен что хотите удалить {SelectedClient.Name}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var relevantAdsService = Get<IGeneralService>();
                relevantAdsService.DeleteClient(SelectedClient);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Клиент {SelectedClient.Name} был удалён");
            }
            Refresh();
            SetIsBusy(false);
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
