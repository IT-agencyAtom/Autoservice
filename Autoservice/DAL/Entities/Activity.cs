using Autoservice.DAL.Common.Implementation;
using Autoservice.ViewModel.Utils;
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
        public string UniqueString { get; set; }
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [NotMapped]
        public TimeSpan Time => EndTime - StartTime ?? DateTime.Now - StartTime;
        
        public string StringStatus => ToString();
        public ActivityStatus Status { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        private static string[] _statuses = Enum.GetNames(typeof(ActivityStatus));
        public static int CompareByStatus(Activity a1, Activity a2)
        {
            return a1.Status.CompareTo(a2.Status);
        }

        public Activity()
        {
            Id = Guid.NewGuid();
            UniqueString = RandomStrings.GetRandomString(10);
        }

        public ActivityStatus? GetNextStatus()
        {
            if (_statuses.Length - 1 <= (int)Status)
                return null;
            return (ActivityStatus)((int)Status + 1);

        }
        
    }
    public enum ActivityStatus
    {
        New,InOperation,Closed
    }
}
