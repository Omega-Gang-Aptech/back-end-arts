using System;
using System.Collections.Generic;

#nullable disable

namespace back_end_arts.Models
{
    public partial class User
    {
        public User()
        {
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public int? UserPhone { get; set; }
        public bool? UserGender { get; set; }
        public string UserAvatar { get; set; }
        public string UserAddress { get; set; }
        public int? UserRole { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Role UserRoleNavigation { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
