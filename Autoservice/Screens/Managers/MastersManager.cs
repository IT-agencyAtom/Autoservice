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
using System.Windows;
using System.Windows.Data;
using Autoservice.Dialogs;
using Autoservice.Dialogs.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Autoservice.Screens.Managers
{
    class MastersManager : PanelViewModelBase
    {
        private string _mastersFilterString;
        private ICollectionView _mastersView { get; set; }

        public ObservableCollection<Master> Masters { get; set; }
        public Master SelectedMaster { get; set; }

        public string MastersFilterString
        {
            get { return _mastersFilterString; }
            set
            {
                if (_mastersFilterString == value)
                    return;

                _mastersFilterString = value.ToLower();

                _mastersView = CollectionViewSource.GetDefaultView(Masters)
                    ;
                _mastersView.Filter = MastersFilter;
                _mastersView.MoveCurrentToFirst();

                RaisePropertyChanged("MastersFilterString");

            }
        }
        private bool MastersFilter(object item)
        {
            var master = item as Master;
            if (master == null)
                return false;
            if (MastersFilterString != null)
                if (StringFilter(master) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Master master)
        {
            return master.Name.ToLower().Contains(MastersFilterString)||
                master.Position.ToLower().Contains(MastersFilterString);
        }
        public RelayCommand MouseDoubleClickCommand { get; set; }

        public MastersManager()
        {
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => AddHandler(),
                        ButtonIcon = "appbar_add",
                        ButtonText = "Add"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => EditHandler(),
                        ButtonIcon = "appbar_edit",
                        ButtonText = "Edit"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => DeleteHandler(),
                        ButtonIcon = "appbar_delete",
                        ButtonText = "Delete"
                    }
                },
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Refresh"
                    }
                }
            };
            MouseDoubleClickCommand = new RelayCommand(EditHandler);
        }
        private async void AddHandler()
        {
            SetIsBusy(true);

            var addManager = new AddMasterManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeAdd());

            var addDialog = new AddMasterDialog(addManager);

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
            if (SelectedMaster == null)
                return;

            SetIsBusy(true);

            var addManager = new AddMasterManager() { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedMaster));

            var addDialog = new AddMasterDialog(addManager);

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
            if (SelectedMaster == null)
                return;

            var deleteDialogSettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancel"
            };

            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow == null)
                return;

            SetIsBusy(true);

            var result =
                await
                    metroWindow.ShowMessageAsync("Confirm Master delete",
                        $"Are you sure to delete Master {SelectedMaster.Name}",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var relevantAdsService = Get<IGeneralService>();
                relevantAdsService.DeleteMaster(SelectedMaster);

                await
                    metroWindow.ShowMessageAsync("Success", $"Master {SelectedMaster.Name} was deleted");

                Refresh();
            }

            SetIsBusy(false);
        }
        public async override void Refresh()
        {
            SetIsBusy(true);

            var service = Get<IGeneralService>();
            Masters = new ObservableCollection<Master>(await Task.Run(() => service.GetAllMasters()));
            RaisePropertyChanged("Masters");

            SetIsBusy(false);
        }
    }
}
