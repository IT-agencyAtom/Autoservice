using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservice.Dialogs.Managers;

namespace Autoservice.DAL.Entities
{
    public class OrderWork : IEntity
    {
        [NotMapped]
        public Action OnChangeWork { get; set; }
        public Guid Id { get; set; }
        public Guid WorkId { get; set; }

        [ForeignKey("WorkId")]
        public Work Work { get; set; }
        public Guid MasterId { get; set; }
        [ForeignKey("MasterId")]
        public Master Master { get; set; }
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public decimal Price { get; set; }

        [NotMapped]
        public bool IsNew { get; set; }

        public OrderWork()
        {
            Id = Guid.NewGuid();
            IsNew = false;
        }

        public OrderWork(OrderWorkModel work)
        {
            Id = work.Id;
            WorkId = work.WorkId;
            Work = work.Work;
            MasterId = work.MasterId;
            Master = work.Master;
            OrderId = work.OrderId;
            Order = work.Order;
            Price = work.Price;
            IsNew = work.IsNew;
        }
    }
}
