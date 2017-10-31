using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class OrderWork : IEntity
    {
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

        public OrderWork()
        {
            Id = Guid.NewGuid();
        }
    }
}
