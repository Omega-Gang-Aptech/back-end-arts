using back_end_arts.DTO.Product;
using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace back_end_arts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrdersController : ControllerBase
    {
        private IArtsRepository<Order> db_order;
        public OrdersController(IArtsRepository<OrderDetail> db_orderdetail,IArtsRepository<Order> db_order)
        {
            this.db_order = db_order;
        }

        ///Order
        [HttpGet("Orders")]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await db_order.ListAll();
        }
        [HttpGet("Order")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            return await db_order.GetById(id);
        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderRequest Order)
        {
            var objOrderLst = await db_order.ListAll();
            var maxOrderCd = objOrderLst
                                .Where(item => item.OrderId.Substring(0, 8) == Order.OrderTypeId).Max(item => item.OrderId);

            var newOrderCd = this.generateOrderID(Order.OrderTypeId.ToString(), maxOrderCd);
            DateTime dateTime = DateTime.UtcNow.Date;
            Order order = null;
            order = new Order()
            {
                OrderId = newOrderCd, 
                OrderUserId = Order.OrderUserId,
                OrderAddress = Order.OrderAddress,
                OrderDescription = Order.OrderDescription,
                OrderCreateDate = dateTime,
                OrderStatus = Order.OrderStatus,
                OrderTotal = Order.OrderTotal,
                OrderPaymentMethods = Order.OrderPaymentMethods,
                OrderDeliveryType = Order.OrderDeliveryType,
                UpdatedAt = dateTime
            };
            await db_order.Insert(order);
            return Ok(order);
            //return CreatedAtAction(nameof(GetOrders), new { id = order.OrderId }, Order);
        }
        [HttpPost("UpdateOrder")]
        public async Task<ActionResult<Order>> UpdateOrder([FromBody] Order Order)
        {
            var data = await db_order.GetById(Order.OrderId);
            DateTime dateTime = DateTime.UtcNow.Date;
            if (data != null)
            {
                data.OrderUserId = Order.OrderUserId;
                data.OrderAddress = Order.OrderAddress;
                data.OrderDescription = Order.OrderDescription;
                data.OrderStatus = Order.OrderStatus;
                data.OrderTotal = Order.OrderTotal;
                data.OrderPaymentMethods = Order.OrderPaymentMethods;
                data.OrderDeliveryType = Order.OrderDeliveryType;
                data.UpdatedAt = dateTime;
                await db_order.Update(data);
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("OrderId")]
        public async Task<ActionResult<Order>> DeleteOrder(string id)
        {
            var data = await db_order.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_order.Delete(data);
            return NoContent();
        }

        private string generateOrderID(string OrderTypeId, string OrderNumber)
        {
            string tempOrderNumber = null;
            if (String.IsNullOrEmpty(OrderNumber)) // OrderNumber non exist
            {
                tempOrderNumber = "00000001";
                string OrderIdLatest = OrderTypeId + tempOrderNumber;
                
                return OrderIdLatest;
            }
            else
            {
                // 1010000112345678
                // Increate Order Number
                string orderNumberStr = OrderNumber.Substring(8);
                string typeDeliProdIdStr = OrderNumber.Substring(0, 8);
                int orderNumberInt = Int32.Parse(orderNumberStr);
                if (orderNumberInt == 99999999) // Case: Order Number overwhelm
                {
                    //Console.Write("The Order Number is overwhelm. Cannot insert anymore");
                    //Console.ReadLine();
                    return null;
                }
                else
                {
                    orderNumberInt++;
                }
                string orderNumIntTemp = orderNumberInt.ToString();
                int orderNumIntCnt = orderNumIntTemp.Count();
                switch (orderNumIntCnt)
                {
                    case 8:
                        orderNumberStr = orderNumberInt.ToString();
                        break;
                    case 7:
                        orderNumberStr = "0" + orderNumberInt;
                        break;
                    case 6:
                        orderNumberStr = "00" + orderNumberInt;
                        break;
                    case 5:
                        orderNumberStr = "000" + orderNumberInt;
                        break;
                    case 4:
                        orderNumberStr = "0000" + orderNumberInt;
                        break;
                    case 3:
                        orderNumberStr = "00000" + orderNumberInt;
                        break;
                    case 2:
                        orderNumberStr = "000000" + orderNumberInt;
                        break;
                    case 1:
                        orderNumberStr = "0000000" + orderNumberInt;
                        break;
                    default:
                        break;
                }

                string OrderIdLatest = typeDeliProdIdStr + orderNumberStr;
                return OrderIdLatest;
            }
        }
    }
}
