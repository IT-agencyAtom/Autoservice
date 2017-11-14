using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Dialogs.Managers
{
    public class AddWorkTemplateManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        public string Title { get; set; }
        
        private WorkTemplate _workTemplate;
        public List<WorkModel> Works { get; set; }
        public WorkTemplate WorkTemplate
        {
            get { return _workTemplate; }
            set
            {
                if (_workTemplate == value)
                    return;

                _workTemplate = value;
                RaisePropertyChanged("WorkTemplate");
            }
        }
        public string Price { get; set; }


        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }
       
        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(WorkTemplate workTemplate)
        {
            _isEdit = true;

            initialize();
            WorkTemplate = workTemplate;

            Title = "Изменить шаблон";
        }

        public void initializeAdd()
        {
            initialize();
            WorkTemplate = new WorkTemplate();
            Title = "Добавить шаблон";
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
            Works = new List<WorkModel>();
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

            WorkTemplate.Works = Works.Where(s=>s.IsChecked==true).Select(w => new Work(w)).ToList();

            if (_isEdit)
                generalService.UpdateWorkTemplate(WorkTemplate);
            else
                generalService.AddWorkTemplate(WorkTemplate);
        }

        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            var allWorks = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks())).ToList();
            Works.AddRange(allWorks.Select(w => new WorkModel(w)));
            foreach (var work in WorkTemplate.Works)
            {
                Works.First(w => w.Id == work.Id).IsChecked = true;
            }
            RaisePropertyChanged("Works");
            SetIsBusy(false);
        }
    }
    public class WorkModel : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsChecked { get; set; }     
        
        public WorkModel()
        {
            Id = Guid.NewGuid();
            IsChecked = false;
        }
        public WorkModel(Work work)
        {
            Id = work.Id;
            Name = work.Name;
            Price = work.Price;
            IsChecked = false;
        }

    }
}
