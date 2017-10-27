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
    class WorksManager : PanelViewModelBase
    {
        private string _worksFilterString;
        private ICollectionView _worksView { get; set; }

        public ObservableCollection<Work> Works { get; set; }
        public Work SelectedWork { get; set; }

        public string WorksFilterString
        {
            get { return _worksFilterString; }
            set
            {
                if (_worksFilterString == value)
                    return;

                _worksFilterString = value.ToLower();

                _worksView = CollectionViewSource.GetDefaultView(Works)
                    ;
                _worksView.Filter = WorksFilter;
                _worksView.MoveCurrentToFirst();

                RaisePropertyChanged("WorksFilterString");

            }
        }
        private bool WorksFilter(object item)
        {
            var work = item as Master;
            if (work == null)
                return false;
            if (WorksFilterString != null)
                if (StringFilter(work) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Master work)
        {
            return work.Name.ToLower().Contains(WorksFilterString);
        }
        public RelayCommand MouseDoubleClickCommand { get; set; }

        public WorksManager()
        {
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    /*new PanelButtonManager
                    {
                        OnButtonAction = o => AddHandler(),
                        ButtonIcon = "appbar_add",
                        ButtonText = "Добавить"
                    },*/
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

            var addManager = new AddWorkManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeAdd());

            var addDialog = new AddWorkDialog(addManager);

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
            if (SelectedWork == null)
                return;

            SetIsBusy(true);

            var addManager = new AddWorkManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedWork));

            var addDialog = new AddWorkDialog(addManager);

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
            if (SelectedWork == null)
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
                    metroWindow.ShowMessageAsync("Подтвердите удаление работы",
                        $"Вы уверен что хотите удалить {SelectedWork.Name}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var relevantAdsService = Get<IGeneralService>();
                relevantAdsService.DeleteWork(SelectedWork);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Работа {SelectedWork.Name} была удалена");

                Refresh();
            }

            SetIsBusy(false);
        }

        public async override void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks()));
            RaisePropertyChanged("Works");
            SetIsBusy(false);
        }
    }
}
