using Autoservice.DAL.Common.Implementation;
using Autoservice.Dialogs.Managers;
using Autoservice.ViewModel.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class OrderSparePart:IEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public Guid SparePartId { get; set; }
        [ForeignKey("SparePartId")]
        public SparePart SparePart { get; set; }
        public int Number { get; set; }
        public SparePartSource Source { get; set; }

        [NotMapped]
        public string StringSource => Source.ToDescriptionString();
        [NotMapped]
        public bool IsNew { get; set; }


        public OrderSparePart()
        {
            Id = Guid.NewGuid();
        }
        public OrderSparePart(OrderSparePartModel orderSparePartModel)
        {
            Id = orderSparePartModel.Id;
            OrderId = orderSparePartModel.OrderId;
            Order = orderSparePartModel.Order;
            SparePartId = orderSparePartModel.SparePartId;
            SparePart = orderSparePartModel.SparePart;
            Number = orderSparePartModel.Number;
            Source = (SparePartSource) orderSparePartModel.Source;
            IsNew = orderSparePartModel.IsNew;
        }



    }
    public enum SparePartSource
    {
        [Description("Со склада")]
        FromWarehouse,
        [Description("На заказ")]
        Custom,
        [Description("От клиента")]
        FromClient
    }
}
