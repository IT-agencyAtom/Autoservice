using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Activity: IEntity
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Order Order { get; set; }

        [NotMapped]
        public TimeSpan Time => EndTime - StartTime;
        public ActivityStatus Status { get; set; }
        public User User { get; set; }

        public Activity()
        {
            Id = Guid.NewGuid();
        }
    }
    public enum ActivityStatus
    {
        New,InOperation,Closed
    }
}
