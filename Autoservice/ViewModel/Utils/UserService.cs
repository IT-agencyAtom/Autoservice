using System;
using Autoservice.DAL.Entities;

namespace Autoservice.ViewModel.Utils
{
    public sealed class UserService
    {
        private static volatile UserService _instance;
        private static readonly object SyncRoot = new Object();

        //public RolePermissionSet Permissions { get; private set; }

        public User CurrentUser { get; set; }

        public bool IsAdmin => true; //CurrentUser.Role?.Name == "Admin";

        public Master DefaultMaster { get; set; }

        /*public void LoadUser(User user)
        {
            _user = user;

            Permissions = new RolePermissionSet(_user.Role.PermissionsForRoles);
        }*/

        // ReSharper disable InconsistentNaming
        public static UserService Instance
        {
            // ReSharper restore InconsistentNaming
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new UserService();
                    }
                }

                return _instance;
            }
        }
    }
}
