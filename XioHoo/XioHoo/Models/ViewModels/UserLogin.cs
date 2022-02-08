using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseMangement.Models.ViewModels
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter User UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }

        public bool RememberMe { get; set; }


    }
}