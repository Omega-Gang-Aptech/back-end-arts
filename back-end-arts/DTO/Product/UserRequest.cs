using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.DTO.Product
{
    public class UserRequest
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public bool? UserGender { get; set; }
        public string UserAvatar { get; set; }
        public string UserAddress { get; set; }
        public int? UserRole { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
