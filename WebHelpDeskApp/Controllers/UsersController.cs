using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelpDeskApp.Data;
using WebHelpDeskApp.Models;

namespace WebHelpDeskApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //for showing all the users in one view "all users page" 
        public async Task<IActionResult> Index()
        {

            //this is for manager
            var managers = await(from user in _context.ApplicationUsers
                              join userRole in _context.UserRoles
                              on user.Id equals userRole.UserId
                              join role in _context.Roles
                              on userRole.RoleId equals role.Id
                              where role.Name == "Manager"
                              select user)
                                 .ToListAsync();

            //this is for employee
            var employees = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == "Employee"
                                  select user)
                                 .ToListAsync();
            var tuple = new Tuple<List<ApplicationUser>, List<ApplicationUser>>(managers, employees);
            return View(tuple);
        }
    }
}
