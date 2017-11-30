using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Autoservice.ViewModel.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Car: IEntity
    {
        public Guid Id { get; set; }
        //Производитель
        public string Brand { get; set; }
        //Модель
        public string Model { get; set; }
        public CarType Type { get; set; }

        [NotMapped]
        public string LocalName => ToString();

        [NotMapped]
        public string StringType => Type.ToDescriptionString();

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
            return $"{Brand} {Model}";
        }
    }
}
