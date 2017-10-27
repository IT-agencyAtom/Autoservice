using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Work : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public Guid MasterId { get; set; }
        [ForeignKey("MasterId")]
        public Master Master { get; set; }
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }



        public Work()
        {
            Id = Guid.NewGuid();
        }
    }
}
