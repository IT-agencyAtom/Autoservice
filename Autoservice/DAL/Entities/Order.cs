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
    public class Order : IEntity
    {
        private Order order;

        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public string PersonalNumber => $"АГ-{Number}";
        
        public string RepairZone { get; set; }
        public List<SparePart> SpareParts { get; set; }
        public Guid CarId { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }
        public List<OrderWork> Works { get; set; }
        public string Notes { get; set; }

        [NotMapped]
        public ActivityStatus? Status => Activities?.LastOrDefault()?.Status;

        [NotMapped]
        public string StringStatus => Status?.ToDescriptionString()??"";

        public List<Activity> Activities { get; set; }
        public int TotalPrice { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }


        public Order()
        {
            Id = Guid.NewGuid();
            Activities = new List<Activity>();
        }
    }   
    public enum PaymentMethod
    {
        Cash,BankCard,BankTransfer
    }
}
