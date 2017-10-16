using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using ConstaSoft.Core.Controls.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Screens.Managers
{
    class OrderManager : PanelViewModelBase
    {
        public ObservableCollection<Order> Orders { get; set; }
        public OrderManager()
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
            var Clients = new ObservableCollection<Client>(await Task.Run(()=>service.GetAllClients()));
            Orders = new ObservableCollection<Order>(await Task.Run(() => service.GetAllOrders()));

            RaisePropertyChanged("Orders");

            SetIsBusy(false);
        }
    }
}
