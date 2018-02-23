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
using Autoservice.ViewModel.Utils;

namespace Autoservice.Dialogs.Managers
{
    public class AddCarSelectorManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        private Client _client;
        public string Title { get; set; }
        public List<ClientCar> Cars { get; set; }
        public ObservableCollection<WorkTemplate> Templates { get; set; }
        private WorkTemplate _selectedTemplate;
        public WorkTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate == value)
                    return;
                _selectedTemplate = value;
                RaisePropertyChanged("SelectedTemplate");
                RefreshWorks();                
            }
        }
        public List<WorkModel> Works { get; set; }

        public bool PreOrderIsChecked { get; set; }
        public ClientCar Car
        {
            get { return _car; }
            set
            {
                _car = value;
                RaisePropertyChanged("Car");
            }
        }
        private ClientCar _car;

        //Комманды

        public RelayCommand AddWorkCommand { get; private set; }
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
                        OnButtonAction = (obj) => AddNewTemplate(),
                        ButtonIcon = "appbar_list_add",
                        ButtonText = "Добавить шаблон"
                    },
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
            Car = Cars.FirstOrDefault();
            RefreshWorks();
            AddWorkCommand = new RelayCommand(AddNewWork);
        }

        private void AddNewWork()
        {
            var generalService = Get<IGeneralService>();
            Work work = new Work
            {
                Name = "Новая работа",
                Price = 0
            };
            generalService.AddWork(work);
            if (SelectedTemplate != null)
            {
                WorkTemplateWork tWork = new WorkTemplateWork();
                tWork.TemplateId = SelectedTemplate.Id;
                tWork.WorkId = work.Id;
                generalService.AddWorkTemplateWork(tWork);
                SelectedTemplate.Works.Add(new WorkTemplateWork(SelectedTemplate, new WorkModel(work)));
            }
            RefreshWorks();
        }

        private async void AddNewTemplate()
        {
            WorkTemplate workTemplate = new WorkTemplate();
            workTemplate.Name = await DialogHelper.ShowInputDialog("Имя шаблона","Укажите имя создаваемого шаблона работ");
            DialogHelper.ActivateDialog(typeof(AddCarSelectorDialog));
            if(workTemplate.Name==null)
                return;
            Templates.Add(workTemplate);
            RaisePropertyChanged("Templates");
            SelectedTemplate = workTemplate;
            var generalService = Get<IGeneralService>();
            generalService.AddWorkTemplate(SelectedTemplate);
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
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks())).Select(w => new WorkModel(w)).ToList();
            if (Works != null)
            {
                foreach (var work in Works)
                {
                    Works.First(w => w.Id == work.Id).IsChecked = false;
                }
            }
            RaisePropertyChanged("Templates");
            SetIsBusy(false);
        }
        private async void RefreshWorks()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks())).Select(w => new WorkModel(w)).ToList();
            if (Works != null)
            {
                foreach (var work in Works)                
                    work.IsChecked = false;                
                if (SelectedTemplate != null)
                {
                    foreach (var work in _selectedTemplate.Works)
                    {
                        Works.FirstOrDefault(w => w.Id == work.WorkId).IsChecked = true;
                    }
                }
            }
            Works = Works.OrderByDescending(w => w.IsChecked).ThenByDescending(w => w.Name).ToList();
            SetIsBusy(false);
            RaisePropertyChanged("Works");
        }
    }
}
