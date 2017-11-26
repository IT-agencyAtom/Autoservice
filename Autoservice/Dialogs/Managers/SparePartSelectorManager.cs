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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Dialogs.Managers
{
    public class SparePartSelectorManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        
        public string Title { get; set; }

        public List<ITreeViewNode> Nodes { get; set; }
        public List<SparePart> Checked { get; set; }

        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }
        private Order _order;

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
            var service = Get<IGeneralService>();
            var spareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts()));
            Nodes = new ObservableCollection<ITreeViewNode>(await Task.Run(() => service.GetAllSparePartsFolders())).Where(n => n.Parent == null).ToList();
            Nodes.AddRange(spareParts.Where(s => s.Parent == null));            
            RaisePropertyChanged("Nodes");            
        }
    }   
}
