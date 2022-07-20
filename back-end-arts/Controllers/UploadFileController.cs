using back_end_arts.DTO.Product;
using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace back_end_arts.Controllers
{
    public class UploadFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IArtsRepository<Product> _repository;

        public UploadFileController(IWebHostEnvironment env, IArtsRepository<Product> repository)
        {
            _env = env;
            _repository = repository;
        }

        //[Authorize]
        [HttpPost]
        [Route("UploadFlie")]
        [SwaggerOperation(
            Summary = "UploadFlie",
            Description = "UploadFlie",
            OperationId = "UploadFlie",
            Tags = new[] { "UploadFlie" })]
        public async Task<ActionResult<HttpResponseMessage>> HandleAsync(List<IFormFile> files, [FromForm] string productJson)
        {

            try
            {

                // Config JSON 
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                var productRequest = JsonSerializer.Deserialize<ProductRequest>(productJson, options);


                // Khoi tao mot product moi
                Product product = null;


                if (files.Count > 0)
                {
                    var formFile = files[0];
                    if (formFile.Length > 0)
                    {

                        product = new Product()
                        {
                            ProductId = productRequest.ProductId, // ProductId String - not generate
                            ProductName = productRequest.ProductName,
                            CategoryId = productRequest.CategoryId,
                            ProductPrice = productRequest.ProductPrice,
                        };

                        // Luu Product xuong BD

                        await _repository.Insert(product);
                        // Sau khi luu Product se co duoc Product Id
                        var filePath = Path.Combine(_env.ContentRootPath, "Images", product.ProductId.ToString());
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        filePath = Path.Combine(filePath, formFile.FileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await formFile.CopyToAsync(stream);

                        // Cap nhat lai url cua san pham sau luu xong hinh anh
                        product.ProductImage = "Images/" + product.ProductId.ToString() + "/" + formFile.FileName;
                        await _repository.Update(product);


                    }
                }
                else
                {
                    return BadRequest();
                }



                var response = new
                {
                    product.ProductId,
                    product.ProductName,
                    product.ProductPrice,
                    product.Category,
                    product.ProductImage,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private ActionResult<HttpResponseMessage> BadRequest()
        {
            throw new NotImplementedException();
        }
    }
}
