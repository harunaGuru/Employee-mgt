using Microsoft.AspNetCore.Http;
using myproject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot Exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        public Dept? Department { get; set; }
        [Required]
        //[RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]\.[a-zA-Z0-9-.]+$", ErrorMessage ="Invalid Email Format")]
        [EmailAddress]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        public IFormFile Photos { get; set; }
       




    }
}
