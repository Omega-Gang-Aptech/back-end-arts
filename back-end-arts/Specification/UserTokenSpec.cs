using Ardalis.Specification;
using back_end_arts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Specification
{
    public class UserTokenSpec : Specification<User>, ISingleResultSpecification
    {
        public UserTokenSpec(string userName)
        {
            Query.Where(item => item.UserName == userName);
        }
    }
}
