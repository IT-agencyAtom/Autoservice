using System;
using Autoservice.DAL.Common.Implementation;

namespace Autoservice.DAL.Entities
{
    /// <summary>
    /// User database entity.</summary>
    public class User : IEntity
    {
        public User()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// User role.</summary>
        public string Role { get; set; }
    }
}
