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
    class WorkTemplateManager : PanelViewModelBase
    {
        private string _templatesFilterString;

        private ICollectionView _templateView { get; set; }

        public ObservableCollection<WorkTemplate> Templates { get; set; }
        public WorkTemplate SelectedTemplate { get; set; }

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
            SetIsBusy(true);

            var addManager = new AddWorkTemplateManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeAdd());

            var addDialog = new AddWorkTemplateDialog(addManager);

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

        private async void EditHandler()
        {
            if (SelectedTemplate == null)
                return;

            SetIsBusy(true);

            var addManager = new AddWorkTemplateManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedTemplate));

            var addDialog = new AddWorkTemplateDialog(addManager);

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
            if (SelectedTemplate == null)
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
                    metroWindow.ShowMessageAsync("Подтвердите удаление шаблона",
                        $"Вы уверены что хотите удалить {SelectedTemplate.Name}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var generalService = Get<IGeneralService>();
                generalService.DeleteWorkTemplate(SelectedTemplate);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Шаблон {SelectedTemplate.Name} был удалена");

                Refresh();
            }

            SetIsBusy(false);
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
