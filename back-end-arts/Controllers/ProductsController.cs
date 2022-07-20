using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private IArtsRepository<Product> db_Product;
        public ProductsController(IArtsRepository<Product> db_Product)
        {
            this.db_Product = db_Product;
        }


        ///Product
        [HttpGet("Products")]
        public async Task<IEnumerable<Product>> GetCategories()
        {
            return await db_Product.ListAll();
        }
        [HttpGet("Product")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            return await db_Product.GetById(id);
        }
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product Product)
        {

            await db_Product.Insert(Product);
            return CreatedAtAction(nameof(GetCategories), new { id = Product.ProductId }, Product);
        }
        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product Product)
        {
            var data = await db_Product.GetById(Product.ProductId);
            if (data != null)
            {
                data.ProductName = Product.ProductName;
                data.ProductPrice = Product.ProductPrice;
                data.ProductQuantity = Product.ProductQuantity;
                data.ProductImage = Product.ProductImage;
                data.ProductShortDescription = Product.ProductShortDescription;
                data.ProductLongDescription = Product.ProductLongDescription;
                data.ProductStatus = Product.ProductStatus;
                data.UpdatedAt = Product.UpdatedAt;

                await db_Product.Update(data);
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("ProductId")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var data = await db_Product.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_Product.Delete(data);
            return NoContent();
        }
        public static void generateProductID(string ProductCode, string ProductId)
        {
            // Case 1: Product id null
            string tempPrdId = null;
            if (String.IsNullOrEmpty(ProductId))
            {
                tempPrdId = "00001";
                string ProductIdLatest = ProductCode + tempPrdId;
                Console.Write(ProductIdLatest);
                Console.Write("\n");
                Console.ReadLine();
                return;
            }
            else
            {
                // Case 2:  increase ProductId 0100001
                string productNumStr = ProductId.Substring(2);
                string productIdStr = ProductId.Substring(0, 2);
                int productNumInt = Int32.Parse(productNumStr);
                if (productNumInt == 99999) // Case: ProductId overwhelm
                {
                    //Console.Write("The Product Number is overwhelm. Cannot insert anymore");
                    //Console.ReadLine();
                    return;
                }
                else
                {
                    productNumInt++;
                }
                string productNumIntTemp = productNumInt.ToString();
                int productNumIntCnt = productNumIntTemp.Count();
                switch (productNumIntCnt)
                {
                    case 5:
                        tempPrdId = productNumInt.ToString();
                        break;
                    case 4:
                        tempPrdId = "0" + productNumInt;
                        break;
                    case 3:
                        tempPrdId = "00" + productNumInt;
                        break;
                    case 2:
                        tempPrdId = "000" + productNumInt;
                        break;
                    case 1:
                        tempPrdId = "0000" + productNumInt;
                        break;
                    default:
                        break;
                }

                string ProductIdLatest = productIdStr + tempPrdId;
                //Console.Write(ProductIdLatest);
                //Console.Write("\n");
                //Console.ReadLine();

            }
        }
    }
}
