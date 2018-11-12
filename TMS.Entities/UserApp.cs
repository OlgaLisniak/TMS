﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TMS.Entities
{
    public class UserApp : IdentityUser
    {
        public UserApp()
        {
            TaskModerator_Users = new List<TaskModerator_User>();
            TaskViewer_Users = new List<TaskViewer_User>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<TaskModerator_User> TaskModerator_Users { get; set; }
        public ICollection<TaskViewer_User> TaskViewer_Users { get; set; }

    }
}