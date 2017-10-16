using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Order : IEntity
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public string PersonalNumber { get; set; }
        public Guid ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }
        public string RepairZone { get; set; }
        public List<SparePart> SpareParts { get; set; }
        public List<Work> Works { get; set; }
        public Guid CarId { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }
        public List<Master> Masters { get; set; }
        public OrderStatus Status { get; set; }
        public List<Activity> Activities { get; set; }


        public Order()
        {
            Id = Guid.NewGuid();
        }

    }
    public enum OrderStatus
    {
        New,InWork,Closed
    }
}
