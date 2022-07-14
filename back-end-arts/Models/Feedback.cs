using System;
using System.Collections.Generic;

#nullable disable

namespace back_end_arts.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int? FeedbackUserId { get; set; }
        public string FeedbackProductId { get; set; }
        public string Comment { get; set; }
        public int? Rating { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Product FeedbackProduct { get; set; }
        public virtual User FeedbackUser { get; set; }
    }
}
