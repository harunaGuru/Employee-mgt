using Microsoft.AspNetCore.Mvc;
using myproject.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller:"Account")]
        [ValidEmailDomain(allowedDomain:"pragimtech.com", ErrorMessage ="Email Domain must be pragimtech.com")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and Confirm Password does not Match")]
        [Display(Name ="Confirm Password" )]
        public string ConfirmPassword { get; set; }
        public string City { get; set; }
    }
}
