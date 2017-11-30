using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Dialogs.Managers
{
    public class AddPreEntryManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        public string Title { get; set; }      
        
        public DateTime SelectedDate { get; set; }

        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }


        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
      
        public void Initialize()
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
                        ButtonText = "Оформить запись"
                    }
                }
            };
        }

        private void SkipHandler()
        {
            throw new NotImplementedException();
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
            
        }

        public override async void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
