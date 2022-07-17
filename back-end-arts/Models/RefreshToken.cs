using System;
using System.Collections.Generic;

#nullable disable

namespace back_end_arts.Models
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual User User { get; set; }
    }
}
