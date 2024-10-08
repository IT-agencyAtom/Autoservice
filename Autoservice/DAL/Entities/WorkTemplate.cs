﻿using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class WorkTemplate : IEntity
    {
        public Guid Id {get; set;}
        public string Name { get; set; }
        public List<WorkTemplateWork> Works { get; set; }
        public string WorksString => WorksToSting(); 

        public WorkTemplate()
        {
            Id = Guid.NewGuid();
            Works = new List<WorkTemplateWork>();
        }

        private string WorksToSting()
        {
            string result = "";
            foreach (var work in Works)
            {
                result += work.Name + '\n';
            }
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
