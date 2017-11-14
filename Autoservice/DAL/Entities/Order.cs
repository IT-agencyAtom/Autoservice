﻿using Autoservice.DAL.Common.Implementation;
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
    public class Order : IEntity
    {
        private Order order;

        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; }

        public DateTime? PreOrderDateTime { get; set; }

        public DateTime StartDate { get; set; }
        public string PersonalNumber => $"АГ-{Number}";
        public string RepairZone { get; set; }
        public List<OrderSparePart> SpareParts { get; set; }
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
        public decimal TotalPrice { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }


        public Order()
        {
            Id = Guid.NewGuid();
            Works = new List<OrderWork>();
            SpareParts = new List<OrderSparePart>();
            Activities = new List<Activity>();
        }

        public static int CompareByPreOrderStartDate(Order o1, Order o2)
        {
            var pTime1 = o1.PreOrderDateTime;
            var pTime2 = o2.PreOrderDateTime;
            if (pTime1 == null && pTime2 != null)
                return 1;
            if (pTime1 != null && pTime2 == null)
                return -1;
            if (pTime1 != null && pTime2 != null)
            {
                return -((DateTime)pTime1).CompareTo((DateTime)pTime2);
            }
            return -o1.StartDate.CompareTo(o2.StartDate);
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
