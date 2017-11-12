using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
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
        public int IntSource {
            get { return (int)Source; }
            set
            {
                Source = (SparePartSource)value;
            }
        }

        public OrderSparePart()
        {
            Id = Guid.NewGuid();
        }

    }
    public enum SparePartSource
    {
        FromWarehouse,
        Custom,
        FromClient
    }
}
