using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHelpDeskApp.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string NotificationTitle { get; set; }
        public DateTime? DateTime { get; set; }

        [ForeignKey("Assignment_NotificationAssignment")]
        public int NotificationAssignment { get; set; }
        public Assignment Assignment_NotificationAssignment { get; set; }

    }
}
