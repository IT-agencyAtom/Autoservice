using Autoservice.DAL.Common.Implementation;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autoservice.DAL.Entities
{
    public class ClientCar : IEntity
    {
        public Guid Id { get; set; }        
        public string RegistrationNumber { get; set;}
        public int Mileage { get; set; }

        public Guid CarId { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public Guid ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [NotMapped]
        public string LocalName { get { return ToString(); } set { } }

        public ClientCar()
        {
            Id = Guid.NewGuid();
        }
        
        public override string ToString()
        {
            return $"{Car?.ToString()} [{RegistrationNumber}]";
        }
    }
}
