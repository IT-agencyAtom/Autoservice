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
    public class AddFolderManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }
        public string Title { get; set; }
        
        private SparePartsFolder _folder;

        public SparePartsFolder Folder
        {
            get { return _folder; }
            set
            {
                if (_folder == value)
                    return;

                _folder = value;
                RaisePropertyChanged("Folder");
            }
        }
        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }


        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(SparePartsFolder folder)
        {
            _isEdit = true;

            initialize();
            Folder = folder;
            Title = "Изменить работу";
        }

        public void initializeAdd(SparePartsFolder parentFolder)
        {
            initialize();
            Folder = new SparePartsFolder();
            if (parentFolder != null)
                Folder.ParentId = parentFolder.Id;
            Title = "Добавить работу";
        }

        private void initialize()
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
            var generalService = Get<IGeneralService>();
            if (_isEdit)
                generalService.UpdateSparePartsFolder(Folder);
            else
                generalService.AddSparePartsFolder(Folder);
        }

        public override async void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
