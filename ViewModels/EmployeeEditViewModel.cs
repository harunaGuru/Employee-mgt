using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.ViewModels
{
    public class EmployeeEditViewModel : EmployeeCreateViewModel
    {
        public int Id { get; set; }
        public string ExistingPhoto { get; set; }
    }
}
