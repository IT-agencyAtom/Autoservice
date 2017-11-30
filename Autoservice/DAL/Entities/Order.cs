using Autoservice.DAL.Common.Implementation;
using Autoservice.ViewModel.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Autoservice.DAL.Entities
{
    public class Order : IEntity
    {
        private Order order;

        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime? PreOrderDateTime { get; set; }
        public string PersonalNumber => $"АГ-{Number}";
        public string RepairZone { get; set; }
        public List<OrderSparePart> SpareParts { get; set; }
        public Guid ClientCarId { get; set; }
        [ForeignKey("ClientCarId")]
        public ClientCar Car { get; set; }
        public List<OrderWork> Works { get; set; }
        public string Notes { get; set; }

        [NotMapped]
        public ActivityStatus? Status => Activities?.LastOrDefault()?.Status;

        [NotMapped]
        public string StringStatus => Status?.ToDescriptionString()??"";

        [NotMapped]
        public Visibility PreOrderVisibility => PreOrderDateTime == null ? Visibility.Collapsed : Visibility.Visible;

        public List<Activity> Activities { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }


        public Order()
        {
            Id = Guid.NewGuid();
            Works = new List<OrderWork>();
            SpareParts = new List<OrderSparePart>();
            Activities = new List<Activity>();
            PaymentMethod = Entities.PaymentMethod.Cash;
        }

        public static int CompareByPreOrderStartDate(Order o1, Order o2)
        {
            var pTime1 = o1.PreOrderDateTime>DateTime.Now?o1.PreOrderDateTime:null;
            var pTime2 = o2.PreOrderDateTime>DateTime.Now?o2.PreOrderDateTime : null;
            if(pTime1==null&&pTime2==null)
                return -o1.StartDate.CompareTo(o2.StartDate);
            if (pTime1 == null && pTime2 != null)
                return 1;
            if (pTime1 != null && pTime2 == null)
                return -1;              
            return -((DateTime)pTime1).CompareTo((DateTime)pTime2);
           
        }
    }   
    public enum PaymentMethod
    {
        [Description("Наличные")]
        Cash,
        [Description("Банковская карта")]
        BankCard,
        [Description("Банковский перевод")]
        BankTransfer
    }
}
