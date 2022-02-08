using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public bool UserStatus { get; set; }
        public string RoleName { get; set; }
        public DateTime DOB { get; set; }
        public string PlainPassword { get; set; }
        public DateTime Joiningdate { get; set; }
        public List<UsersSurvey> UsersSurveys { get; set; }
    }
}
