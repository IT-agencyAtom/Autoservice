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
using System.Windows.Data;

namespace Autoservice.Screens.Managers
{
    class WorkTemplateManager : PanelViewModelBase
    {
        private string _templatesFilterString;

        private ICollectionView _templateView { get; set; }

        public ObservableCollection<WorkTemplate> Templates { get; set; }
        public Work SelectedTemplate { get; set; }

        public string TemplateFilterString
        {
            get { return _templatesFilterString; }
            set
            {
                if (_templatesFilterString== value)
                    return;

                _templatesFilterString = value.ToLower();

                _templateView = CollectionViewSource.GetDefaultView(Templates);
                _templateView.Filter = TemplatesFilter;
                _templateView.MoveCurrentToFirst();

                RaisePropertyChanged("WorksFilterString");

            }
        }
        private bool TemplatesFilter(object item)
        {
            var work = item as WorkTemplate;
            if (work == null)
                return false;
            if (TemplateFilterString != null)
                if (StringFilter(work) == false)
                    return false;
            return true;
        }
        private bool StringFilter(WorkTemplate work)
        {
            return work.Name.ToLower().Contains(TemplateFilterString);
        }
        public RelayCommand MouseDoubleClickCommand { get; set; }

        public WorkTemplateManager()
        {
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => AddHandler(),
                        ButtonIcon = "appbar_add",
                        ButtonText = "Добавить"
                    },
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

        private async void AddHandler()
        {
        }

        private async void EditHandler()
        {
        }

        private async void DeleteHandler()
        {
        }

        public async override void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Templates = new ObservableCollection<WorkTemplate>(await Task.Run(() => service.GetAllWorkTemplates()));
            RaisePropertyChanged("Templates");
            SetIsBusy(false);
        }
    }
}
