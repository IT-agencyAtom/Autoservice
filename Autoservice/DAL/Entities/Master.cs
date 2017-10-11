using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Master: IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set;}

        public Master()
        {
            Id = Guid.NewGuid();
        }
    }
}
