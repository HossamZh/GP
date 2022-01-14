using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHelpDeskApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public double Salary { get; set; }

        [Column(TypeName ="date")]
        public DateTime? DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public int? DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}
