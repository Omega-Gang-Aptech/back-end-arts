using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IArtsRepository<Order> db_order;
        public OrdersController(IArtsRepository<Order> db_order)
        {
            this.db_order = db_order;
        }


        ///Order
        [HttpGet("Categories")]
        public async Task<IEnumerable<Order>> GetCategories()
        {
            return await db_order.ListAll();
        }
        [HttpGet("Order")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            return await db_order.GetById(id);
        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order Order)
        {

            await db_order.Insert(Order);
            return CreatedAtAction(nameof(GetCategories), new { id = Order.OrderId }, Order);
        }
        [HttpPut("UpdateOrder")]
        public async Task<ActionResult<Order>> UpdateOrder([FromBody] Order Order)
        {
            var data = await db_order.GetById(Order.OrderId);
            if (data != null)
            {
                data.OrderUserId = Order.OrderUserId;
                data.OrderAddress = Order.OrderAddress;
                data.OrderDescription = Order.OrderDescription;
                data.OrderCreateDate = Order.OrderCreateDate;
                data.OrderStatus = Order.OrderStatus;
                data.OrderPaymentMethods = Order.OrderPaymentMethods;
                data.OrderDeliveryType = Order.OrderDeliveryType;
                data.UpdatedAt = Order.UpdatedAt;
                await db_order.Update(data);
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("OrderId")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var data = await db_order.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_order.Delete(data);
            return NoContent();
        }
    }
}
