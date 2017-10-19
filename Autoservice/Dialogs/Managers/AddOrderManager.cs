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
    public class AddOrderManager:PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }

        public string[] Statuses { get; private set; }
        public string[] Methods { get; private set; }

        public string SelectedStatus { get; set; }
        public string SelectedMethod { get; set; }


        private Order _order;

        public Order Order
        {
            get { return _order; }
            set
            {
                if (_order == value)
                    return;

                _order = value;
                RaisePropertyChanged("Order");
            }
        }        
        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(Order order)
        {
            _isEdit = true;

            initialize();

            Order = order;
            SelectedMethod = Order.PaymentMethod.ToString();
            SelectedStatus = Order.Status.ToString();
            Title = "Edit Order";
        }

        public void initializeAdd()
        {
            initialize();

            Order = new Order();

            Title = "Add Order";
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
                        ButtonText = "Cancel"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => SaveHandler(),
                        ButtonIcon = "appbar_disk",
                        ButtonText = "Save"
                    }
                }
            };
            Statuses = Enum.GetNames(typeof(OrderStatus));
            Methods = Enum.GetNames(typeof(PaymentMethod));

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
            var relevantAdsService = Get<IGeneralService>();

            if (_isEdit)
                relevantAdsService.UpdateOrder(Order);
            else
                relevantAdsService.AddOrder(Order);
        }

        public override void Refresh()
        {
            SetIsBusy(false);
        }
    }
}
