using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHelpDeskApp.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentReporter { get; set; }
        public string CommentText { get; set; }
        public DateTime? CommentDateTime { get; set; }

        [ForeignKey("AssignmentCommentAssignment")]
        public int CommentAssignment { get; set; }
        public Assignment AssignmentCommentAssignment { get; set; }
    }
}
