using back_end_arts.DTO.Product;
using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace back_end_arts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        Byte[] originalBytes;
        Byte[] encodedBytes;
        public MD5 md5;
        string EncodePassword(string password)
        {
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes);
        }
        private readonly IWebHostEnvironment _env;
        private IArtsRepository<User> db_User;
        public UsersController(IWebHostEnvironment env, IArtsRepository<User> db_User)
        {
            _env = env;
            this.db_User = db_User;
        }


        ///User
        [HttpGet("Users")]
        public async Task<IEnumerable<User>> GetCategories()
        {
            return await db_User.ListAll();
        }
        [HttpGet("User")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await db_User.GetById(id);
        }
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User User)
        {
            User.Password = EncodePassword(User.Password);
            await db_User.Insert(User);
            return CreatedAtAction(nameof(GetCategories), new { id = User.UserId }, User);
        }
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<User>> UpdateUser(List<IFormFile> files, [FromForm] string userJson)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            // Convert JSON string sang Object
            var userRequest = JsonSerializer.Deserialize<User>(userJson, options);

            try
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    //if (formFile.Length > 0)
                    var filePath = Path.Combine(_env.ContentRootPath, "Images/Users/", userRequest.UserId.ToString());
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
                    userRequest.UserAvatar = "Images/Users/" + userRequest.UserId.ToString() + "/" + formFile.FileName;

                    var data = await db_User.GetById(userRequest.UserId);
                    if (data != null)
                    {
                        data.UserFullName = userRequest.UserFullName;
                        data.UserEmail = userRequest.UserEmail;
                        data.UserPhone = userRequest.UserPhone;
                        data.UserGender = userRequest.UserGender;
                        data.UserAvatar = userRequest.UserAvatar;
                        data.UserAddress = userRequest.UserAddress;
                        data.UserRole = userRequest.UserRole;
                        data.UpdatedAt = userRequest.UpdatedAt;
                        await db_User.Update(data);
                        return Ok(data);
                    }
                    return Ok(data);
                }
                else
                {
                    var data = await db_User.GetById(userRequest.UserId);
                    if (data != null)
                    {
                        data.UserFullName = userRequest.UserFullName;
                        data.UserEmail = userRequest.UserEmail;
                        data.UserPhone = userRequest.UserPhone;
                        data.UserGender = userRequest.UserGender;
                        data.UserAvatar = userRequest.UserAvatar;
                        data.UserAddress = userRequest.UserAddress;
                        data.UserRole = userRequest.UserRole;
                        data.UpdatedAt = userRequest.UpdatedAt;
                        await db_User.Update(data);
                        return Ok(data);
                    }
                    return Ok(data);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //var data = await db_User.GetById(User.UserId);
            //if (data != null)
            //{
            //    data.UserName = User.UserName;
            //    data.Password = User.Password;
            //    data.UserFullName = User.UserFullName;
            //    data.UserEmail = User.UserEmail;
            //    data.UserPhone = User.UserPhone;
            //    data.UserGender = User.UserGender;
            //    data.UserAvatar = User.UserAvatar;
            //    data.UserAddress = User.UserAddress;
            //    data.UserRole = User.UserRole;
            //    data.UpdatedAt = User.UpdatedAt;
            //    await db_User.Update(data);
            //    return Ok();
            //}
            //return NotFound();

        }
        [HttpDelete("UserId")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var data = await db_User.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_User.Delete(data);
            return NoContent();
        }
    }
}
