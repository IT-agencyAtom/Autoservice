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
using System.Windows.Controls;
using System.Windows.Data;

namespace Autoservice.Screens.Managers
{
    class WarehouseManager : PanelViewModelBase
    {
        private string _sparePartFilterString;
        private object _selectedItem;
        private List<ITreeViewNode> _nodesBuffer;
        private ObservableCollection<SparePart> _spareParts;
        private ICollectionView _sparePartView { get; set; }
        public object SelectedItem { get { return _selectedItem; } set { _selectedItem = value; RaisePropertyChanged("SelectedItem"); } }
        public ObservableCollection<SparePart> SpareParts { get { return _spareParts; } set { _spareParts = value;RaisePropertyChanged("Nodes"); } }
        public List<ITreeViewNode> Nodes { get; set; }

        public string SparePartFilterString
        {
            get { return _sparePartFilterString; }
            set
            {
                if (_sparePartFilterString == value)
                    return;
                _sparePartFilterString = value.ToLower();
                if (_sparePartFilterString == "")
                    Nodes = _nodesBuffer;
                else
                {
                    Nodes = SpareParts.Select(s => s as ITreeViewNode).ToList();      
                    _sparePartView = CollectionViewSource.GetDefaultView(Nodes);
                    _sparePartView.Filter = SparePartFilter;
                    _sparePartView.MoveCurrentToFirst();
                }
                RaisePropertyChanged("SparePartFilterString");
                RaisePropertyChanged("Nodes");


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
            return sparePart.Name.ToLower().Contains(SparePartFilterString) || 
                sparePart.Cargo.ToLower().Contains(SparePartFilterString);


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
                        OnButtonAction = o => AddSparePartHandler(),
                        ButtonIcon = "appbar_add",
                        ButtonText = "Добавить запчасть"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => AddFolderHandler(),
                        ButtonIcon = "appbar_folder_star",
                        ButtonText = "Добавить папку"
                    },                    
                    new PanelButtonManager
                    {
                        OnButtonAction = o => EditHandlerAsync(),
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
            MouseDoubleClickCommand = new RelayCommand(EditHandlerAsync);
            //Refresh();
        }       

        private SparePartsFolder GetParentFolder()
        {

            if (_selectedItem == null)
                return null;
            if (_selectedItem is SparePartsFolder)
                return _selectedItem as SparePartsFolder;
            return (_selectedItem as SparePart).Parent;
        }

        public void MoveNode(ITreeViewNode node,SparePartsFolder newFolder)
        {
            node.Parent = null;
            if (newFolder != null)
                node.ParentId = newFolder.Id;
            else
                node.ParentId = null;
            UpdateNode(node);
            Refresh();
        }

        private async void AddFolderHandler()
        {
            SetIsBusy(true);

            var addManager = new AddFolderManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeAdd(GetParentFolder()));

            var addDialog = new AddFolderDialog(addManager);

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

        private async void AddSparePartHandler()
        {
            SetIsBusy(true);

            var addManager = new AddSparePartManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeAdd(GetParentFolder()));

            var addDialog = new AddSparePartDialog(addManager);

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

        private async void EditHandlerAsync()
        {
            if (_selectedItem == null)
                return;
            if (_selectedItem is SparePartsFolder)
                await EditFolder(_selectedItem as SparePartsFolder);
            else
                await EditSparePart(_selectedItem as SparePart);           
        }

        private async Task EditFolder(SparePartsFolder folder)
        {
            SetIsBusy(true);

            var addManager = new AddFolderManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(folder));
            var addDialog = new AddFolderDialog(addManager);

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
        private async Task EditSparePart(SparePart sparePart)
        {
            SetIsBusy(true);

            var addManager = new AddSparePartManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(sparePart));

            var addDialog = new AddSparePartDialog(addManager);

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

            /*if (SelectedSparePart == null)
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
                    metroWindow.ShowMessageAsync("Подтвердите удаление запчасти",
                        $"Вы уверен что хотите удалить {SelectedSparePart.Name}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var generalService = Get<IGeneralService>();
                generalService.DeleteSparePart(SelectedSparePart);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Работа {SelectedSparePart.Name} была удалена");

                Refresh();
            }

            SetIsBusy(false);*/
        }

        public async override void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            SpareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts()));
            Nodes = new ObservableCollection<ITreeViewNode>(await Task.Run(() => service.GetAllSparePartsFolders())).Where(n => n.Parent == null).ToList();
            Nodes.AddRange(SpareParts.Where(s => s.Parent == null));
            _nodesBuffer = Nodes;
            /**Nodes = new List<ITreeViewNode>
            {
                new SparePartsFolder
                {
                    Name = "Lalala",
                    Children = new List<ITreeViewNode>
                    {
                        new SparePartsFolder
                        {
                            Name = "dadada",
                            Children = new List<ITreeViewNode>{ new SparePart { Name = "Spapa",Number = 23, Cargo="asdasf" } }
                        }
                    }
                },
                new SparePartsFolder
                {
                    Name = "Mamama",
                    Children = new List<ITreeViewNode>{ new SparePart {Name = "dsadas",Number = 23,Cargo = "sa" }, new SparePart { Name = "lhfh", Number = 2143, Cargo = "qte" } }
                },
                new SparePart
                { Name = "Superpuper",Number=1243}
            };*/
            RaisePropertyChanged("SpareParts");
            RaisePropertyChanged("Nodes");
            SetIsBusy(false);
        }

        public async void UpdateNode(ITreeViewNode node)
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            if (node is SparePartsFolder)
                service.UpdateSparePartsFolder((SparePartsFolder)node);
            else
                service.UpdateSparePart((SparePart)node);
            SetIsBusy(false);
        }
    }
    public interface ITreeViewNode
    {
        Guid Id { get; set; }
        string Name { get; set; }
        Guid? ParentId { get; set; }
        SparePartsFolder Parent { get; set; }
    }
}
