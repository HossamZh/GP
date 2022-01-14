using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelpDeskApp.Data;
using WebHelpDeskApp.Models;

namespace WebHelpDeskApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Showing list of employees
        public  async Task<IActionResult> Index()
        {
            //showing all employees to admin
            if (User.IsInRole("Admin"))
            {
                var users = await (from user in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on user.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == "Employee"
                                   select user)
                                 .ToListAsync();
                foreach (var item in users)
                {
                    if (item.DepartmentID != null)
                    {
                        item.Department = _context.Departments.Find(item.DepartmentID);
                    }
                }
                return View(users);
            }
            // Showing list of employee related to manager departments
            if (User.IsInRole("Manager"))
            {
                var useremail = User.Identity.Name;
                var userDepartmentId = _context.ApplicationUsers.Where(a => a.Email == useremail).FirstOrDefault().DepartmentID;

                var users = await (from user in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on user.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == "Employee"
                                   select user)
                                 .ToListAsync();
                users = users.Where(a => a.DepartmentID == userDepartmentId).ToList();
                foreach (var item in users)
                {
                    if (item.DepartmentID != null)
                    {
                        item.Department = _context.Departments.Find(item.DepartmentID);
                    }
                }
                return View(users);
            }
            return View();
        }

        //For picking employee using its id and showing it in view
        public IActionResult UpdateEmployee(string Id)
        {
            ViewBag.Departments = new SelectList(_context.Departments.ToList(), "DepartmentId", "DepartmentName");
            return View(_context.ApplicationUsers.Find(Id));
        }

        //For picking updated data of employee and saving it in database
        [HttpPost]
        public IActionResult UpdateEmployee(ApplicationUser updateduserProfile)
        {
            var userTobeEdit = _context.ApplicationUsers.Find(updateduserProfile.Id);
            userTobeEdit.FullName = updateduserProfile.FullName;
            userTobeEdit.Email = updateduserProfile.Email;
            userTobeEdit.UserName = updateduserProfile.Email;
            userTobeEdit.NormalizedEmail = updateduserProfile.Email.ToUpper();
            userTobeEdit.NormalizedUserName = updateduserProfile.Email.ToUpper();
            userTobeEdit.PhoneNumber = updateduserProfile.PhoneNumber;
            userTobeEdit.Salary = updateduserProfile.Salary;
            userTobeEdit.JobTitle = updateduserProfile.JobTitle;
            userTobeEdit.DateOfBirth = updateduserProfile.DateOfBirth;
            userTobeEdit.DepartmentID = updateduserProfile.DepartmentID;
            _context.Entry(userTobeEdit).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
