using Autoservice.DAL.Common.Implementation;
using Autoservice.Dialogs.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class WorkTemplateWork : IEntity
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public WorkTemplate Template { get; set; }
        public Guid WorkId { get; set; }
        [ForeignKey("WorkId")]
        public Work Work { get; set; }
        [NotMapped]
        public string Name => Work.Name;

        public WorkTemplateWork()
        {
            Id = Guid.NewGuid();
        }
        public WorkTemplateWork(WorkTemplate template, WorkModel model)
        {
            Id = Guid.NewGuid();
            TemplateId = template.Id;
            WorkId = model.Id;
        }
    }
}
