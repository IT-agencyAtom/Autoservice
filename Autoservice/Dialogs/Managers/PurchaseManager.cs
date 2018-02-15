using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
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
    public class PurchaseManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }       

        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        public ObservableCollection<SparePart> SpareParts { get; set; }
       
        public void Initialize()
        {
            Panel = new PanelManager
            {
                RightButtons = new ObservableCollection<PanelButtonManager>
                {                    
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => Export(),
                        ButtonIcon = "appbar_printer_text",
                        ButtonText = "Экспорт"
                    }
                }
            };
            Title = "Закупка запчастей";
        }

        private void Export()
        {
            throw new NotImplementedException();
        }

        private void CancelHandler()
        {
            OnExit();
        }
        
        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            SpareParts = new ObservableCollection<SparePart>(await Task.Run(() => service.GetAllSpareParts().Where(s=>s.Deficit>0)));
            RaisePropertyChanged("SpareParts");
            SetIsBusy(false);
        }
    }   
}
