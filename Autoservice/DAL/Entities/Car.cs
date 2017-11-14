using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Car: IEntity
    {
        public Guid Id { get; set; }        
        public string RegistrationNumber { get; set;}
        public string Brand { get; set; }
        public string Model { get; set; }
        public CarType Type { get; set; }
        public int Mileage { get; set; }

        public Guid ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [NotMapped]
        public string LocalName { get { return ToString(); } set { } }

        public Car()
        {
            Id = Guid.NewGuid();
        }
        
        public enum CarType
        {
            [Description("Автомобиль")]
            Automobile,
            [Description("Мотоцикл")]
            Motocycle,
            [Description("Спецтехника")] 
            SpecialTransport
        }
        public override string ToString()
        {
            return string.Format("{0} {1} [{2}]",Brand,Model,RegistrationNumber);
        }
    }
}
