using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Models
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpiredInHours { get; set; }
    }
}
