using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.DTO.Product
{
    public class ProductRequest
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Category { get; set; }
        public int Price { get; set; }
    }
}
