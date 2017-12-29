using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.Screens.Managers;
using Autoservice.ViewModel.Utils;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Autoservice.Dialogs.Managers
{
    public class SparePartSelectorManager : PanelViewModelBase
    {
        private string _sparePartFilterString;
        private ICollectionView _sparePartView { get; set; }
        private List<ITreeViewNode> _nodesBuffer;
        private ObservableCollection<SparePart> _spareParts;
        public Action OnExit { get; set; }
        
        public string Title { get; set; }

        public List<ITreeViewNode> Nodes { get; set; }
        public List<SparePart> Checked { get; set; }

        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }
        private Order _order;


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
                    Nodes = _spareParts.Select(s => s as ITreeViewNode).ToList();
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
                   sparePart.Cargo != null && sparePart.Cargo.ToLower().Contains(SparePartFilterString);


        }

        public void Initialize(Order order)
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
            _order = order;
            Checked = new List<SparePart>();
            GetNodes();
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
            /*var generalService = Get<IGeneralService>();
            _order.SpareParts = OrderSpareParts.Select(o => new OrderSparePart(o)).ToList();
            generalService.UpdateOrder(_order);*/
        }

        public override void Refresh()
        {
            SetIsBusy(false);            
        }
        public async void GetNodes()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            _spareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts()));
            var folders = new ObservableCollection<SparePartsFolder>(await Task.Run(() => service.GetAllSparePartsFolders()));
            Nodes = new ObservableCollection<ITreeViewNode>(await Task.Run(() => service.GetAllSparePartsFolders())).Where(n => n.Parent == null).ToList();
            Nodes.AddRange(_spareParts.Where(s => s.Parent == null));
            _nodesBuffer = Nodes;
            RaisePropertyChanged("Nodes");
            SetIsBusy(false);
        }
    }   
}
