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
using System.Windows.Data;

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

        public MastersManager()
        {
            Panel = new PanelManager
            {
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Refresh"
                    }
                },

                RightButtons = new ObservableCollection<PanelButtonManager>
                {

                }
            };
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
