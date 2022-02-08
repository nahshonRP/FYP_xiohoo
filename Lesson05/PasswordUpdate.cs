using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson05.Models
{
    public class PasswordUpdate
    {
        [Required(ErrorMessage = "invalid/no input!")]
        [DataType(DataType.Password)]
        [Remote("VerifyCurrentPassword", "Account", ErrorMessage = "password doesn't match!")]
        public String CurrentPassword { get; set; }

        [Required(ErrorMessage = "invalid/no input!")]
        [DataType(DataType.Password)]
        [Remote("VerifyNewPassword", "Account", ErrorMessage = "new password is invalid!")]
        public String NewPassword { get; set; }

        [Required(ErrorMessage = "invalid/no input!")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "password isn't confirmed1")]
        public String ConfirmPassword { get; set; }
    }
}
//19047572 Konada Obadiah Nahshon