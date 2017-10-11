using System.Collections.Generic;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Services
{
    /// <summary>
    /// Database application service interface.</summary>
    public interface IRelevantAdsService
    {       
        /// <summary>
        /// Update user.</summary>
        void UpdateUser(User user);
        /// <summary>
        /// Add user.</summary>
        void AddUser(User user);
        /// <summary>
        /// Delete user.</summary>
        void DeleteUser(User user);
        /// <summary>
        /// Get all users.</summary>
        List<User> GetAllUsers();       
    }
}
