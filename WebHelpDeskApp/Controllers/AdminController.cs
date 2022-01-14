using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelpDeskApp.Data;

namespace WebHelpDeskApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult AdminDashboardData()
        {
            if (User.IsInRole("Admin"))
            {
                var managers = (from user in _context.ApplicationUsers
                                join userRole in _context.UserRoles
                                on user.Id equals userRole.UserId
                                join role in _context.Roles
                                on userRole.RoleId equals role.Id
                                where role.Name == "Manager"
                                select user).Count();
                var employees = (from user in _context.ApplicationUsers
                                 join userRole in _context.UserRoles
                                 on user.Id equals userRole.UserId
                                 join role in _context.Roles
                                 on userRole.RoleId equals role.Id
                                 where role.Name == "Employee"
                                 select user).Count();
                var totalTasks = _context.Assignments.Count();
                var archivedTasks = _context.Assignments.Where(a => a.IsArchived == true).Count();
                var TodayAddedTasks = _context.Assignments.Where(a => a.Date == DateTime.Today).Count();
                var data = string.Format("{0}::{1}::{2}::{3}::{4}", managers, employees,
                    totalTasks, archivedTasks, TodayAddedTasks);
                return Ok(data);
            }
            else if(User.IsInRole("Manager"))
            {
                int? userDepartmentId = _context.ApplicationUsers.
                    Where(a => a.Email == User.Identity.Name).FirstOrDefault()?.DepartmentID;
                var employees = (from user in _context.ApplicationUsers.Where(a => a.DepartmentID == userDepartmentId)
                                 join userRole in _context.UserRoles
                                 on user.Id equals userRole.UserId
                                 join role in _context.Roles
                                 on userRole.RoleId equals role.Id
                                 where role.Name == "Employee"
                                 select user).Count();
                var totalTasks = _context.Assignments
                    .Where(a => a.ApplicationUserServiceRequester.DepartmentID == userDepartmentId).Count();
                var archivedTasks = _context.Assignments
                    .Where(a => a.ApplicationUserServiceRequester.DepartmentID == userDepartmentId && a.IsArchived == true).Count();
                var TodayAddedTasks = _context.Assignments
                    .Where(a => a.ApplicationUserServiceRequester.DepartmentID == userDepartmentId && a.Date == DateTime.Today).Count();
                var data = string.Format("{0}::{1}::{2}::{3}", employees, totalTasks, archivedTasks, TodayAddedTasks);
                return Ok(data);
            }
            else
            {
                string userid = _context.Users.Where(a => a.Email == User.Identity.Name).FirstOrDefault()?.Id;
                var mytotalTasksList = _context.Assignments.Where(a => a.AssigneeID == userid).ToList();
                var mytotalTasks = mytotalTasksList.Count();
                var myopenTasks = mytotalTasksList.Where(a => a.Status != "Close").Count();
                var myclosedTasks = mytotalTasksList.Where(a => a.Status == "Close").Count();
                var data = string.Format("{0}::{1}::{2}", mytotalTasks, myopenTasks, myclosedTasks);
                return Ok(data);
            }            
        }


        public IActionResult TasksChartData()
        {
            var openTasks = _context.Assignments.Where(a => a.Status == "Open").Count();
            var closeTasks = _context.Assignments.Where(a => a.Status == "Close").Count();
            var pendingTasks = _context.Assignments.Where(a => a.Status == "Pending").Count();
            var rejectTasks = _context.Assignments.Where(a => a.Status == "Reject").Count();
            var result = string.Format("{0}::{1}::{2}::{3}", openTasks, closeTasks, pendingTasks, rejectTasks);
            return Ok(result);
        }

        [HttpPost]
        public void DeleteUserbyAdmin(string userid)
        {
            var user = _context.Users.Find(userid);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
