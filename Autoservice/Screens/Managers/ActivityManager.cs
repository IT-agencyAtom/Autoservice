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
    class ActivityManager : PanelViewModelBase
    {
        private string _activityFilterString;
        private ICollectionView _activityView { get; set; }

        public ObservableCollection<Activity> Activities { get; set; }
        public Activity SelectedActivity { get; set; }

        public string ActivityFilterString
        {
            get { return _activityFilterString; }
            set
            {
                if (_activityFilterString == value)
                    return;

                _activityFilterString = value.ToLower();

                _activityView = CollectionViewSource.GetDefaultView(Activities)
                    ;
                _activityView.Filter = ActivityFilter;
                _activityView.MoveCurrentToFirst();

                RaisePropertyChanged("ActivityFilterString");

            }
        }
        private bool ActivityFilter(object item)
        {
            var activity = item as Activity;
            if (activity == null)
                return false;
            if (ActivityFilterString != null)
                if (StringFilter(activity) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Activity activity)
        {
            return activity.Status.ToString().ToLower().Contains(ActivityFilterString) ||
                activity.StartTime.ToString().ToLower().Contains(ActivityFilterString);
        }

        public ActivityManager()
        {
            Panel = new PanelManager
            {
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Обновить"
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
            Activities = new ObservableCollection<Activity>(await Task.Run(() => service.GetAllActivities()));
            RaisePropertyChanged("Activities");

            SetIsBusy(false);
        }
    }
}
