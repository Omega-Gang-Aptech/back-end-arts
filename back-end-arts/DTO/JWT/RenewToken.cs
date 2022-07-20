using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.DTO.JWT
{
    public class RenewToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
