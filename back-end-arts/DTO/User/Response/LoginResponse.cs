using back_end_arts.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.DTO.User.Response
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
       

        public string RequestToken { get; set; }

        public IEnumerable<string> UserRoles { get; set; }
    }
}
