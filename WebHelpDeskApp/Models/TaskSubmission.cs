using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHelpDeskApp.Models
{
    public class TaskSubmission
    {
        [Key]
        public int Id { get; set; }
        public string SubmissionName { get; set; }
        public string SubmissionFile { get; set; }
        public DateTime? DateTime { get; set; }

        [ForeignKey("AssignmentSubmissionAssignment")]
        public int SubmissionAssignment { get; set; }
        public Assignment AssignmentSubmissionAssignment { get; set; }
    }
}
