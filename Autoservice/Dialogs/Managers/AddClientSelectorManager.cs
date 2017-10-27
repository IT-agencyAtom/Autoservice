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
using System.Windows;

namespace Autoservice.Dialogs.Managers
{
    public class AddClientSelectorManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }
        public bool IsNewClient = false;
        public ObservableCollection<Client> Clients { get; set; }
        public Client Client { get { return _client; }
            set
            {
                _client = value;
                ClientName = _client.Name;
                ClientPhone = _client.Phone.ToString();
                RaisePropertyChanged("Client");
            }
        }

        private Client _client;
        private string _clientName="";
        private string _clientPhone="";
        private bool _clientIsFound = false;
        public RelayCommand Find { get; private set; }

        public string ClientName
        {
            get { return _clientName; }
            set
            {
                if (_clientName == value)
                    return;

                _clientName = value;
                RaisePropertyChanged("ClientName");
            }
        }
        public string ClientPhone
        {
            get { return _clientPhone; }
            set
            {
                if (_clientPhone == value)
                    return;

                _clientPhone = value;
                RaisePropertyChanged("ClientPhone");
            }
        }
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
            Find = new RelayCommand(Filter);         
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
                    Client = addClientManager.Client;
                    _clientIsFound = true;
                    IsNewClient = true;
                    NavigateNext();                    
                    Refresh();
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }
        private void NavigateNext()
        {            
            if (!_clientIsFound)
                return;
            Validate();
            if (Client==null)
                return;
            WasChanged = true;
            OnExit();
        }
        private void Filter()
        {
            List<Client> filterResult = Clients.Where(c => c.Name.ToLower().Contains(ClientName) && c.Phone.ToString().Contains(ClientPhone)).ToList();
            if (filterResult.Count > 1)
            {
                _clientIsFound = false;
                MessageBox.Show("Недостаточно данных для поиска");
            }
            else if (filterResult.Count == 0)
            {
                _clientIsFound = false;
                MessageBox.Show("Клиент не найден");
            }
            else
            {
                MessageBox.Show("Клиент найден");
                _clientIsFound = true;
                Client = filterResult[0];
                IsNewClient = false;                
            }

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
