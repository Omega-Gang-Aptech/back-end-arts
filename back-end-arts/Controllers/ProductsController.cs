using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    //[Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private IArtsRepository<Product> db_product;
        private IArtsRepository<Category> db_category;
        public ProductsController(IWebHostEnvironment env, IArtsRepository<Product> db_product, IArtsRepository<Category> db_category)
        {
            _env = env;
            this.db_product = db_product;
            this.db_category = db_category;
        }


        ///Product
        [HttpGet("Products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                return await db_product.ListAll();
            }
            catch (Exception ex)
            {

                return NotFound(ex.ToString());
            }

        }
        [HttpGet("Product")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            return await db_product.GetById(id);
        }
        //[HttpPost("CreateProduct")]
        //public async Task<ActionResult<Product>> CreateProduct([FromBody] Product Product)
        //{
        //    await db_product.Insert(Product);
        //    return CreatedAtAction(nameof(GetCategories), new { id = Product.ProductId }, Product);
        //}
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct(List<IFormFile> files, [FromForm] string productJson)
        {
            try
            {
                // Config JSON 
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                var productRequest = JsonSerializer.Deserialize<Product>(productJson, options);

                // -Generate Product ID--------------------------------------------------------------------------------- 
                var objCategory = await db_category.GetById(Int32.Parse(productRequest.CategoryId.ToString()));
                var objProductLst = await db_product.ListAll();
                var maxProdCd = objProductLst
                                    .Where(item => item.ProductId.Substring(0, 2) == objCategory.CategoryCode).Max(item => item.ProductId);

                var newProdCd = this.generateProductID(objCategory.CategoryCode.ToString(), maxProdCd);
                // -----------------------------------------------------------------------------------------------------

                // Khoi tao mot product moi
                Product product = null;
                product = new Product()
                {
                    //ProductId = this.initProductID(productRequest.CategoryId.ToString()),
                    ProductId = newProdCd, // ProductId String - not generate
                    ProductName = productRequest.ProductName,
                    ProductPrice = productRequest.ProductPrice,
                    ProductQuantity = productRequest.ProductQuantity,
                    ProductShortDescription = productRequest.ProductShortDescription,
                    ProductLongDescription = productRequest.ProductLongDescription,
                    ProductStatus = productRequest.ProductQuantity,
                    CategoryId = productRequest.CategoryId, // *
                    UpdatedAt = productRequest.UpdatedAt
                };

                // Luu Product xuong DB
                await db_product.Insert(product);

                if (files.Count > 0)
                {
                    var formFile = files[0];
                    if (formFile.Length > 0)
                    {
                        // Sau khi luu Product se co duoc Product Id
                        var filePath = Path.Combine(_env.ContentRootPath, "Images/Products", newProdCd);
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        filePath = Path.Combine(filePath, formFile.FileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await formFile.CopyToAsync(stream);

                        // Cap nhat lai url cua san pham sau luu xong hinh anh
                        product.ProductImage = "Images/Products/" + product.ProductId.ToString() + "/" + formFile.FileName;
                        await db_product.Update(product);
                    }
                }

                var response = new
                {
                    product.ProductId,
                    product.ProductName,
                    product.ProductPrice,
                    product.ProductQuantity,
                    product.ProductImage,
                    product.ProductShortDescription,
                    product.ProductLongDescription,
                    product.ProductStatus,
                    product.CategoryId,
                    product.UpdatedAt,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<Product>> UpdateProduct(List<IFormFile> files, [FromForm] string productJson)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            // Convert JSON string sang Object
            var productRequest = JsonSerializer.Deserialize<Product>(productJson, options);

            try
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    var filePath = Path.Combine(_env.ContentRootPath, "Images/Products/", productRequest.ProductId.ToString());
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    else
                    {
                        Directory.Delete(filePath, true);
                        Directory.CreateDirectory(filePath);
                    }
                    filePath = Path.Combine(filePath, formFile.FileName);
                    // Must be same name 
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await formFile.CopyToAsync(stream);

                    // Cap nhat lai url cua san pham sau luu xong hinh anh
                    productRequest.ProductImage = "Images/Products/" + productRequest.ProductId.ToString() + "/" + formFile.FileName;

                    var data = await db_product.GetById(productRequest.ProductId);
                    DateTime dateTime = DateTime.UtcNow.Date;

                    if (data != null)
                    {
                        data.ProductId = productRequest.ProductId;
                        data.ProductPrice = productRequest.ProductPrice;
                        data.ProductQuantity = productRequest.ProductQuantity;
                        data.ProductImage = productRequest.ProductImage;
                        data.ProductShortDescription = productRequest.ProductShortDescription;
                        data.ProductLongDescription = productRequest.ProductLongDescription;
                        data.ProductStatus = productRequest.ProductStatus;
                        data.CategoryId = productRequest.CategoryId;
                        data.UpdatedAt = dateTime;

                        await db_product.Update(data);
                        return Ok(data);
                    }
                    return Ok(data);

                    //var updatePro = await db_product.Update(productRequest);
                    //return Ok(updatePro);
                }
                else
                {
                    var data = await db_product.GetById(productRequest.ProductId);
                    DateTime dateTime = DateTime.UtcNow.Date;

                    if (data != null)
                    {
                        data.ProductId = productRequest.ProductId;
                        data.ProductPrice = productRequest.ProductPrice;
                        data.ProductQuantity = productRequest.ProductQuantity;
                        data.ProductImage = productRequest.ProductImage;
                        data.ProductShortDescription = productRequest.ProductShortDescription;
                        data.ProductLongDescription = productRequest.ProductLongDescription;
                        data.ProductStatus = productRequest.ProductStatus;
                        data.CategoryId = productRequest.CategoryId;
                        data.UpdatedAt = dateTime;

                        await db_product.Update(data);
                        return Ok(data);
                    }
                    return Ok(data);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("ProductId")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var data = await db_product.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_product.Delete(data);
            return NoContent();
        }

        private string generateProductID(string ProductCode, string ProductId)
        {

            // CategoryCode = ProductCode
            // ProductId = Latest(ProductId)
            // Case 1: Product id null
            string tempPrdId = null;
            if (String.IsNullOrEmpty(ProductId))
            {
                tempPrdId = "00001";
                string ProductIdLatest = ProductCode + tempPrdId;

                return ProductIdLatest;
            }
            else
            {
                // Case 2:  increase ProductId 0100001
                string productNumStr = ProductId.Substring(2);
                string productIdStr = ProductId.Substring(0, 2);
                int productNumInt = Int32.Parse(productNumStr);
                if (productNumInt == 99999) // Case: ProductId overwhelm
                {
                    //throw new Exception("The Order Number is overwhelm. Cannot insert anymore");
                    return null;
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
                return ProductIdLatest;
            }
        }
    }
}
