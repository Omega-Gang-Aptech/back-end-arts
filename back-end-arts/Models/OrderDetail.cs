using System;
using System.Collections.Generic;

#nullable disable

namespace back_end_arts.Models
{
    public partial class OrderDetail
    {
        public int DetailId { get; set; }
        public string DetailOrderId { get; set; }
        public string DetailProductId { get; set; }
        public string DetailProductImage { get; set; }
        public int? DetailPrice { get; set; }
        public int? DetailQuantity { get; set; }
        public string DetailProductName { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Order DetailOrder { get; set; }
        public virtual Product DetailProduct { get; set; }
    }
}
