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
    public class AddSparePartManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }

        private SparePart _sparePart;

        public SparePart SparePart
        {
            get { return _sparePart; }
            set
            {
                if (_sparePart == value)
                    return;

                _sparePart = value;
                RaisePropertyChanged("sparePart");
            }
        }
        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(SparePart sparePart)
        {
            _isEdit = true;

            initialize();

            SparePart = sparePart;

            Title = "Изменить запчасть";
        }

        public void initializeAdd()
        {
            initialize();

            SparePart = new SparePart();

            Title = "Добавить запчасть";
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
                generalService.UpdateSparePart(SparePart);
            else
                generalService.AddSparePart(SparePart);
        }

        public override void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
