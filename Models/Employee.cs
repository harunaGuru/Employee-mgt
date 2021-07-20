using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncrpytId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage ="Name cannot Exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        public Dept? Department { get; set; }
        [Required]
        //[RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]\.[a-zA-Z0-9-.]+$", ErrorMessage ="Invalid Email Format")]
        [EmailAddress]
        [Display(Name ="Office Email")]
        public string Email { get; set; }
        public string Photopath { get; set; }
    }
}
