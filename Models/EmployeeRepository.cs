using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _Employeelist;

        public EmployeeRepository()
        {
            _Employeelist = new List<Employee>
            {
                new Employee()
                {
                    Id= 1,
                    Name = "Mary",
                    Department = Dept.HR,
                    Email = "Mary@gmail.com"
                },
                new Employee()
                {
                     Id= 2,
                    Name = "John",
                    Department = Dept.IT,
                    Email = "John@gmail.com"
                },
                new Employee()
                {
                     Id= 3,
                    Name = "Sam",
                    Department = Dept.IT,
                    Email = "Sam@gmail.com"
                }

            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _Employeelist.Max(x => x.Id) + 1;
            _Employeelist.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = _Employeelist.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                _Employeelist.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _Employeelist;
        }

        public Employee GetEmployee(int Id)
        {
            return _Employeelist.FirstOrDefault(x => x.Id == Id);
        }

        public Employee Update(Employee employeechanges)
        {
            Employee employee = _Employeelist.FirstOrDefault(e => e.Id == employeechanges.Id);
            if(employee != null)
            {
                employee.Name = employeechanges.Name;
                employee.Department = employeechanges.Department;
                employee.Email = employeechanges.Email;
            }
            return employeechanges;
        }
    }
}
