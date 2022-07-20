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
    public class CategoriesController : ControllerBase
    {
        private IArtsRepository<Category> db_category;
        public CategoriesController(IArtsRepository<Category> db_category)
        {
            this.db_category = db_category;
        }


        ///Category
        ///
        [HttpGet("Categories")]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await db_category.ListAll();
        }
        [HttpGet("Category")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            return await db_category.GetById(id);
        }
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {

            await db_category.Insert(category);
            return CreatedAtAction(nameof(GetCategories), new { id = category.CategoryId }, category);
        }
        [HttpPut("UpdateCategory")]
        public async Task<ActionResult<Category>> UpdateCategory([FromBody] Category category)
        {
            var data = await db_category.GetById(category.CategoryId);
            if (data != null)
            {
                data.CategoryName = category.CategoryName;
                data.CategoryCode = category.CategoryCode;
                data.UpdatedAt = category.UpdatedAt;
                await db_category.Update(data);
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("CategoryId")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var data = await db_category.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_category.Delete(data);
            return NoContent();
        }
    }
}

