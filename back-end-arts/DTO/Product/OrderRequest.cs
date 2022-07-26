using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.DTO.Product
{
    public class OrderRequest
    {
        public string OrderTypeId { get; set; }
        public int? OrderUserId { get; set; }
        public string OrderAddress { get; set; }
        public string OrderDescription { get; set; }
        public int? OrderStatus { get; set; }
        public int? OrderPaymentMethods { get; set; }
        public int? OrderDeliveryType { get; set; }
    }
}
