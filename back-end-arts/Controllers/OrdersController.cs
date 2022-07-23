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
        [HttpGet("Orders")]
        public async Task<IEnumerable<Order>> GetCategories()
        {
            return await db_order.ListAll();
        }
        [HttpGet("Order")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            return await db_order.GetById(id);
        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder([FromForm] string orderJson)
        {
            try
            {
                // Config JSON 
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                var orderRequest = JsonSerializer.Deserialize<OrderRequest>(orderJson, options);

                // -Generate Order ID--------------------------------------------------------------------------------- 
                
                var objOrderLst = await db_order.ListAll();
                var maxOrderCd = objOrderLst
                                    .Where(item => item.OrderId.Substring(0, 8) == orderRequest.OrderTypeId).Max(item => item.OrderId);

                var newOrderCd = this.generateOrderID(orderRequest.OrderTypeId.ToString(), maxOrderCd);
                // -----------------------------------------------------------------------------------------------------

                // Khoi tao mot product moi
                Order order = null;
                order = new Order()
                {
                    //ProductId = this.initProductID(productRequest.CategoryId.ToString()),
                    OrderId = newOrderCd, // ProductId String - not generate
                    OrderUserId = orderRequest.OrderUserId,
                    OrderAddress = orderRequest.OrderAddress,
                    OrderDescription = orderRequest.OrderDescription,
                    OrderCreateDate = orderRequest.OrderCreateDate,
                    OrderStatus = orderRequest.OrderStatus,
                    OrderPaymentMethods = orderRequest.OrderPaymentMethods,
                    OrderDeliveryType = orderRequest.OrderDeliveryType, // *
                    UpdatedAt = orderRequest.UpdatedAt
                };

                // Luu Product xuong DB
                await db_order.Insert(order);

                var response = new
                {
                    newOrderCd,
                    //orderRequest.OrderUserId,
                    //orderRequest.OrderAddress,
                    //orderRequest.OrderDescription,
                    //orderRequest.OrderCreateDate,
                    //orderRequest.OrderStatus,
                    //orderRequest.OrderPaymentMethods,
                    //orderRequest.OrderDeliveryType, 
                    //orderRequest.UpdatedAt
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
            //await db_order.Insert(Order);
            //return CreatedAtAction(nameof(GetCategories), new { id = Order.OrderId }, Order);
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
