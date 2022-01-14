using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHelpDeskApp.Data;
using WebHelpDeskApp.Models;

namespace WebHelpDeskApp.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // getting the list of assignemnt 
        public async Task<IActionResult> Index()
        {
            var userId = _context.Users.Where(a => a.Email == User.Identity.Name).FirstOrDefault().Id;
            if (User.IsInRole("Employee"))
            {
                // for getting the list of assignemnt which are assigned to a spsific employee
                var notArchived = await _context.Assignments.Where(a => a.AssigneeID == userId && a.IsArchived == false).ToListAsync();
                notArchived.ForEach(a => a.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(a.ServiceRequesterID));
                foreach (var item in notArchived)
                {
                    if (item.AssigneeID != null)
                    {
                        item.ApplicationUserAssignee = _context.ApplicationUsers.Find(item.AssigneeID);
                    }
                }
                
                var Archived = await _context.Assignments.Where(a => a.AssigneeID == userId && a.IsArchived == true).ToListAsync();
                Archived.ForEach(a => a.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(a.ServiceRequesterID));
                foreach (var item in Archived)
                {
                    if (item.AssigneeID != null)
                    {
                        item.ApplicationUserAssignee = _context.ApplicationUsers.Find(item.AssigneeID);
                    }
                }
                
                var tuple = new Tuple<List<Assignment>, List<Assignment>>(notArchived, Archived);
                return View(tuple);
            }

            // for getting the list of assignemnt which are related to a spisific manager department 
            else if (User.IsInRole("Manager"))
            {
                var useremail = User.Identity.Name;
                var userDepartmentId = _context.ApplicationUsers.Where(a => a.Email == useremail).FirstOrDefault().DepartmentID;
                var notArchived = await _context.Assignments.
                    Where(a => a.ApplicationUserServiceRequester.DepartmentID == userDepartmentId && 
                    a.IsArchived == false).ToListAsync();
                notArchived.ForEach(a => a.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(a.ServiceRequesterID));
                foreach (var item in notArchived)
                {
                    if (item.AssigneeID != null)
                    {
                        item.ApplicationUserAssignee = _context.ApplicationUsers.Find(item.AssigneeID);
                    }
                }
                
                var Archived = await _context.Assignments.
                    Where(a => a.ApplicationUserServiceRequester.DepartmentID == userDepartmentId && 
                    a.IsArchived == true).ToListAsync();
                Archived.ForEach(a => a.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(a.ServiceRequesterID));
                foreach (var item in Archived)
                {
                    if (item.AssigneeID != null)
                    {
                        item.ApplicationUserAssignee = _context.ApplicationUsers.Find(item.AssigneeID);
                    }
                }
                
                var tuple = new Tuple<List<Assignment>, List<Assignment>>(notArchived, Archived);
                return View(tuple);
            }
            else

            // for getting list of all assignment for our admin 
            {
                var notArchived = await _context.Assignments.Where(a => a.IsArchived == false).ToListAsync();
                notArchived.ForEach(a => a.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(a.ServiceRequesterID));
                foreach (var item in notArchived)
                {
                    if (item.AssigneeID != null)
                    {
                        item.ApplicationUserAssignee = _context.ApplicationUsers.Find(item.AssigneeID);
                    }
                }
                
                var Archived = await _context.Assignments.Where(a => a.IsArchived == true).ToListAsync();
                Archived.ForEach(a => a.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(a.ServiceRequesterID));
                foreach (var item in Archived)
                {
                    if (item.AssigneeID != null)
                    {
                        item.ApplicationUserAssignee = _context.ApplicationUsers.Find(item.AssigneeID);
                    }
                }
                
                var tuple = new Tuple<List<Assignment>, List<Assignment>>(notArchived, Archived);
                return View(tuple);
            }            
        }

        // It is showing the details of the task
        public async Task<IActionResult> Details(int? id, int? notificationId)
        {
            if (notificationId != null)
            {
                var notification = _context.Notifications.Find(notificationId);
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(m => m.AssignmentId == id);
            assignment.ApplicationUserAssignee = _context.ApplicationUsers.Find(assignment.AssigneeID);
            assignment.ApplicationUserServiceRequester = _context.ApplicationUsers.Find(assignment.ServiceRequesterID);
            if (assignment == null)
            {
                return NotFound();
            }
            var listofComments = _context.Comments.Where(a => a.CommentAssignment == id)
                .OrderByDescending(a => a.CommentDateTime).ToList();
            listofComments.ForEach(a => a.CommentReporter = _context.ApplicationUsers.Find(a.CommentReporter).FullName);
            var submission = _context.TaskSubmissions.Where(a => a.SubmissionAssignment == id).FirstOrDefault();
            var tuple = new Tuple<Assignment, List<Comment>, TaskSubmission>(assignment, listofComments, submission);
            return View(tuple);
        }

        // getting the task information from the view page
        public IActionResult Create()
        {
            var users = (from user in _context.ApplicationUsers
                         join userRole in _context.UserRoles
                         on user.Id equals userRole.UserId
                         join role in _context.Roles
                         on userRole.RoleId equals role.Id
                         where role.Name == "Employee"
                         select user).ToList();
            ViewBag.Employees = new SelectList(users, "Id", "FullName");
            return View();
        }

        // Getting the information from view and saving it on the assignemnt table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssignmentId,Title,Description,Date,Status,Rate,Type,IsArchived,ServiceRequesterID,AssigneeID")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.ServiceRequesterID = _context.Users.Where(a => a.Email == assignment.ServiceRequesterID).FirstOrDefault().Id;
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                var CreateNotification = new Notification
                {
                    NotificationTitle = "A new task (" + assignment.Title + "...) has been added by " +
                    "" + assignment.ApplicationUserServiceRequester.FullName + "",
                    NotificationAssignment = assignment.AssignmentId,
                    DateTime = DateTime.Now
                };
                _context.Notifications.Add(CreateNotification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assignment);
        }

        // Picking the ID of the assignemnt and finding the information of that assignemnt and show you the view 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            var users = (from user in _context.ApplicationUsers
                         join userRole in _context.UserRoles
                         on user.Id equals userRole.UserId
                         join role in _context.Roles
                         on userRole.RoleId equals role.Id
                         where role.Name == "Employee"
                         select user).ToList();
            ViewBag.Employees = new SelectList(users, "Id", "FullName");
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        // After editing the data will come on this method and it will save the updated data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentId,Title,Description,Date,Status,Rate,Type,IsArchived,ServiceRequesterID,AssigneeID")] Assignment assignment)
        {
            if (id != assignment.AssignmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.AssignmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(assignment);
        }

        // Finiding the task by it is ID and return it to view
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(m => m.AssignmentId == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // After clicking of the delete button it will the task from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }

        public string CreateComment(Comment comment)
        {
            if (comment.CommentText == null || comment.CommentText == "")
            {
                return "Please Write Something in Comment First. Thanks!";
            }
            else
            {
                comment.CommentReporter = _context.Users.Where(a => a.Email == User.Identity.Name).FirstOrDefault().Id;
                comment.CommentDateTime = DateTime.Now;
                _context.Comments.Add(comment);
                _context.SaveChanges();
                return "Comment Saved Successfully!";
            }
        }


        public string SubmitTask(TaskSubmission submission)
        {
            submission.DateTime = DateTime.Now;
            _context.TaskSubmissions.Add(submission);
            _context.SaveChanges();
            var assignment = _context.Assignments.Find(submission.SubmissionAssignment);
            assignment.Status = "Submitted";
            _context.Entry(assignment).State = EntityState.Modified;
            _context.SaveChanges();
            return "Task Submitted Successfully!";
        }
    }
}
