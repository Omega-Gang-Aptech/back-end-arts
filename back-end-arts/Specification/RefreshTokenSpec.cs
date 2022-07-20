using Ardalis.Specification;
using back_end_arts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Specification
{
    public class RefreshTokenSpec : Specification<RefreshToken>, ISingleResultSpecification
    {
        public RefreshTokenSpec(string token)
        {
            Query.Where(item => item.Token == token);
        }
    }
}
