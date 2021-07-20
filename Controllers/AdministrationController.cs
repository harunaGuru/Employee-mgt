using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using myproject.Models;
using myproject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace myproject.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Authorize(Policy = "AdminRolePolicy")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ILogger<AdministrationController> logger;

        // public UserManager<ApplicationUser> UserManager { get; }

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            UserManager = userManager;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string UserId)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {UserId} cannot be found";
                return View("NotFound");
            }
            var Existingclaims = await UserManager.GetClaimsAsync(user);
            var model = new UserClaimViewModel
            {
                UserId = UserId
            };
            foreach (Claim claim in ClaimStore.Allclaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (Existingclaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }
                model.claims.Add(userClaim);

            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {model.UserId} cannot be found";
                return View("NotFound");
            }
            var claims = await UserManager.GetClaimsAsync(user);
            var result = await UserManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot remove user existing claims");
                return View(model);
            }
            result = await UserManager.AddClaimsAsync(user, model.claims.Select(c=> new Claim(c.ClaimType, c.IsSelected? "true" : "false"))); 
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser", new { id = model.UserId});
        }
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userid)
        {
            ViewBag.userid = userid;
            var user = await UserManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {userid} cannot be found";
                return View("NotFound");
            }
            var model = new List<ManageUserRolesViewModel>();
           ///var role = await roleManager.Roles.ToListAsync();
            //role.OrderBy(r => r.Name)
            foreach (var roles in await roleManager.Roles.ToListAsync())
            {
                var manageUserRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = roles.Id,
                    RoleName = roles.Name
                };
                if (await UserManager.IsInRoleAsync(user, roles.Name))
                {
                    manageUserRolesViewModel.IsSelected = true;
                }
                else
                {
                    manageUserRolesViewModel.IsSelected = false;
                }
                model.Add(manageUserRolesViewModel);
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<ManageUserRolesViewModel> model, string userid)
        {
            var user = await UserManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {userid} cannot be found";
                return View("NotFound");
            }
            var roles = await UserManager.GetRolesAsync(user);
            var result = await UserManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot remove user existing role");
                return View(model);
            }
            result = await UserManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("EditUser", new { id = userid });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("listuser");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("listuser");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "DeleteOptionPolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    //throw new Exception("test exception");
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("listrole");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("listrole");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role{ex}");
                    ViewBag.Title = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in the role. " +
                        $"If you want to delete the role, please remove the users from the role " +
                        $"then try to delete";
                    return View("Error");
                }
            }
        }
        [HttpGet]
        public IActionResult ListUser()
        {
            var user = UserManager.Users;
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await UserManager.FindByIdAsync(Id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {Id} cannot be found";
                return View("NotFound");
            }
            var userRole = await UserManager.GetRolesAsync(user);
            var userClaims =await UserManager.GetClaimsAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Roles = userRole.ToList(),
                Claims = userClaims.Select(c => c.Type + ":" + c.Value).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUser");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult identityResult = await roleManager.CreateAsync(identityRole);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("ListRole", "Administration");
                }
                foreach(IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
       
        [HttpGet]
        public IActionResult ListRole()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = "Role with id = {id} cannot be found";
                return View("Not Found");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach(var user in UserManager.Users.ToList())
            {
               if(await UserManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
              
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role with id = {id} cannot be found";
                return View("Not Found");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("listrole");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleid)
        {
            ViewBag.roleid = roleid;
            var role = await roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role with id = {id} cannot be found";
                return View("Not Found");
            }
            var model = new List<UserRoleViewModel>();
            foreach(var user in UserManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if(await UserManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleid)
        {
            var role = await roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role with id = {id} cannot be found";
                return View("Not Found");
            }
            for(int i = 0; i < model.Count; i++)
            {
                var user = await UserManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if(model[i].IsSelected && !( await UserManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await UserManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await UserManager.IsInRoleAsync(user, role.Name))
                {
                    result = await UserManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { id = roleid });
                }
            }
            return RedirectToAction("EditRole", new { id = roleid });

        }

    }

}
