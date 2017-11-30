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
    public class AddCarSelectorManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }
        private Client _client;
        public string Title { get; set; }
        public List<ClientCar> Cars { get; set; }
        public ObservableCollection<WorkTemplate> Templates { get; set; }
        public bool PreOrderIsChecked { get; set; }
        public ClientCar Car
        { get { return _car; }
            set
            {
                _car = value; 
                RaisePropertyChanged("Car");
            }
        }
        public WorkTemplate Template
        {
            get { return _template; }
            set
            {
                _template = value;
                RaisePropertyChanged("Template");
            }
        }

        private ClientCar _car;
        private WorkTemplate _template;
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
                        ButtonText = "Добавить ТС"                        
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

            if (Cars.Count == 0)
                AddNewCar();
            else
                Car = Cars.FirstOrDefault();
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
                    Cars.Add(addCarManager.ClientCar);
                    Car = addCarManager.ClientCar;
                    Car.ClientId = _client.Id;
                    RaisePropertyChanged("Cars");
                    var generalService = Get<IGeneralService>();
                    generalService.AddClientCar(Car);

                    //NavigateNext();
                }
                SetIsBusy(false);
            };
            addCarDialog.Show();
        }
        private void NavigateNext()
        {
            if (Car == null)
                return;

            Validate();
            
            WasChanged = true;
            OnExit();
        }       
        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Templates = new ObservableCollection<WorkTemplate>(await Task.Run(() => service.GetAllWorkTemplates()));
            RaisePropertyChanged("Templates");
            SetIsBusy(false);
        }
    }
}
