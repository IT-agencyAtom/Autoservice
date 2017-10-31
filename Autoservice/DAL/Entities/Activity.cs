using Autoservice.DAL.Common.Implementation;
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
        
        public string StringStatus => Status.ToDescriptionString();
        public ActivityStatus Status { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        private static string[] _statuses = Enum.GetNames(typeof(ActivityStatus));
        public static int CompareByStatus(Activity a1, Activity a2)
        {
            return a1.StartTime.CompareTo(a2.StartTime);
        }

        public Activity()
        {
            Id = Guid.NewGuid();
            UniqueString = RandomStrings.GetRandomString(10);
        }

        public ActivityStatus? GetNextStatus()
        {
            if (Status == ActivityStatus.InOperation)
                return ActivityStatus.Closed;
            if (Status == ActivityStatus.InExpectationOfSpareParts)
                return ActivityStatus.InOperation;
            if (_statuses.Length - 1 <= (int)Status)
                return null;
            return (ActivityStatus)((int)Status + 1);

        }
        
    }
    public enum ActivityStatus
    {
        [Description("Новый")]
        New,
        [Description("В работе")]
        InOperation,
        [Description("В ожидании запчастей")]
        InExpectationOfSpareParts,
        [Description("Закрыт")]
        Closed
    }
}
