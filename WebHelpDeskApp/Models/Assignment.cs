using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHelpDeskApp.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        // string Feedback { get; set; }
        public string Rate { get; set; }
        //public string Type { get; set; }
        public bool IsArchived { get; set; }

        [ForeignKey("ApplicationUserServiceRequester")]
        public string ServiceRequesterID { get; set; }
        public ApplicationUser ApplicationUserServiceRequester { get; set; }

        [ForeignKey("ApplicationUserAssignee")]
        public string AssigneeID { get; set; }
        public ApplicationUser ApplicationUserAssignee { get; set; }

    }
}
