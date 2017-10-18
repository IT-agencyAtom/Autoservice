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
    class WorksManager : PanelViewModelBase
    {
        private string _worksFilterString;
        private ICollectionView _worksView { get; set; }

        public ObservableCollection<Work> Works { get; set; }
        public Master SelectedWork { get; set; }

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

        public WorksManager()
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
            Works = new ObservableCollection<Work>(await Task.Run(() => service.GetAllWorks()));
            RaisePropertyChanged("Works");
            SetIsBusy(false);
        }
    }
}
