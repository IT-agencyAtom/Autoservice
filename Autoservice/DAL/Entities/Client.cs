using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Client: IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set;}

        public Client()
        {
            Id = Guid.NewGuid();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
