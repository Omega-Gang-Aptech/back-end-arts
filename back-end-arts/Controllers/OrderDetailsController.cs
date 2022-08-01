using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace back_end_arts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    public class OrderDetailsController : ControllerBase
    {
        private IArtsRepository<OrderDetail> db_orderdetail;
        public OrderDetailsController(IArtsRepository<OrderDetail> db_orderdetail)
        {
            this.db_orderdetail = db_orderdetail;
        }


        ///OrderDetail
        [HttpGet("OrderDetails")]
        public async Task<IEnumerable<OrderDetail>> GetOrderDetails()
        {
            return await db_orderdetail.ListAll();
        }
        [HttpGet("OrderDetail")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            return await db_orderdetail.GetById(id);
        }
        [HttpPost("CreateOrderDetail")]
        public async Task<ActionResult<OrderDetail>> CreateOrderDetail([FromBody] OrderDetail OrderDetail)
        {

            await db_orderdetail.Insert(OrderDetail);
            return CreatedAtAction(nameof(GetOrderDetails), new { id = OrderDetail.DetailId }, OrderDetail);
        }
        [HttpPost("UpdateOrderDetail")]
        public async Task<ActionResult<OrderDetail>> UpdateOrderDetail([FromBody] OrderDetail OrderDetail)
        {
            var data = await db_orderdetail.GetById(OrderDetail.DetailId);
            if (data != null)
            {
                data.DetailOrderId = OrderDetail.DetailOrderId;
                data.DetailProductId = OrderDetail.DetailProductId;
                data.DetailPrice = OrderDetail.DetailPrice;
                data.DetailQuantity = OrderDetail.DetailQuantity;
                data.DetailProductName = OrderDetail.DetailProductName;
                data.UpdatedAt = OrderDetail.UpdatedAt;
                await db_orderdetail.Update(data);
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("OrderDetailId")]
        public async Task<ActionResult<OrderDetail>> DeleteOrderDetail(int id)
        {
            var data = await db_orderdetail.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_orderdetail.Delete(data);
            return NoContent();
        }
    }
}
