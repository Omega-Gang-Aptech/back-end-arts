using System;
using System.Collections.Generic;

#nullable disable

namespace back_end_arts.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string OrderId { get; set; }
        public int? OrderUserId { get; set; }
        public string OrderAddress { get; set; }
        public string OrderDescription { get; set; }
        public DateTime? OrderCreateDate { get; set; }
        public int? OrderStatus { get; set; }
        public int? OrderPaymentMethods { get; set; }
        public int? OrderDeliveryType { get; set; }
        public double? OrderTotal { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User OrderUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
