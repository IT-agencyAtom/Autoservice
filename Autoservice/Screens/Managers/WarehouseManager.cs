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
    class WarehouseManager : PanelViewModelBase
    {
        private string _sparePartFilterString;
        private ICollectionView _sparePartView { get; set; }

        public ObservableCollection<SparePart> SpareParts { get; set; }
        public SparePart SelectedSparePart { get; set; }

        public string SparePartFilterString
        {
            get { return _sparePartFilterString; }
            set
            {
                if (_sparePartFilterString == value)
                    return;

                _sparePartFilterString = value.ToLower();

                _sparePartView = CollectionViewSource.GetDefaultView(SpareParts)
                    ;
                _sparePartView.Filter = SparePartFilter;
                _sparePartView.MoveCurrentToFirst();

                RaisePropertyChanged("SparePartFilterString");

            }
        }
        private bool SparePartFilter(object item)
        {
            var sparePart = item as SparePart;
            if (sparePart == null)
                return false;
            if (SparePartFilterString != null)
                if (StringFilter(sparePart) == false)
                    return false;
            return true;
        }
        private bool StringFilter(SparePart sparePart)
        {
            return sparePart.Name.ToLower().Contains(SparePartFilterString);
        }
        public RelayCommand MouseDoubleClickCommand { get; set; }

        public WarehouseManager()
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
            SpareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts()));
            RaisePropertyChanged("SpareParts");
            SetIsBusy(false);
        }
    }
}
