using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using myproject.Models;
using myproject.Security;
using myproject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment webHostingEnvironment;
        private readonly ILogger<HomeController> logger;
        private readonly IDataProtector Protector;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostingEnvironment, 
                                ILogger<HomeController> logger,IDataProtectionProvider dataProtectionProvider,
                                DataProtectionPurposestring dataProtectionPurposestring)
        {
            _employeeRepository = employeeRepository;
            this.webHostingEnvironment = webHostingEnvironment;
            this.logger = logger;
            Protector = dataProtectionProvider.CreateProtector(dataProtectionPurposestring.EmployeeIdRouteValue);
        }
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee().
                Select(e=> 
            {
                e.EncrpytId = Protector.Protect(e.Id.ToString());
                return e;
            });
            return View(model);
        }
        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            int EmployeeId = Convert.ToInt32(Protector.Unprotect(id));
            //throw new Exception("Error in Details view");
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");
            Employee employee = _employeeRepository.GetEmployee(EmployeeId);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", EmployeeId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                //Employee = _employeeRepository.GetEmployee(id??1),
                Employee = employee,
                PageTitle = "Employee Details"
            };        
            return View(homeDetailsViewModel);
        }
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhoto = employee.Photopath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if(model.Photos != null)
                {
                    if(model.ExistingPhoto != null)
                    {
                        string filepath = Path.Combine(webHostingEnvironment.WebRootPath, "images", model.ExistingPhoto);
                        System.IO.File.Delete(filepath);
                    }
                    employee.Photopath = ProcessUploadedFile(model);
                }
                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();

        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string UniqueFileName = null;
            if (model.Photos != null)
            {
                string UploadFolder = Path.Combine(webHostingEnvironment.WebRootPath, "images");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photos.FileName;
                string filepath = Path.Combine(UploadFolder, UniqueFileName);
                using(var filestream = new FileStream(filepath, FileMode.Create))
                {
                    model.Photos.CopyTo(filestream);
                }
                
            }

            return UniqueFileName;
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UniqueFileName = ProcessUploadedFile(model);

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    Photopath = UniqueFileName

                };
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
            
        }
    }
}
