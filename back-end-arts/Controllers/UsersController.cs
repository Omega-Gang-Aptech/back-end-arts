using back_end_arts.Models;
using back_end_arts.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace back_end_arts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        private IArtsRepository<User> db_User;
        public UsersController(IArtsRepository<User> db_User)
        {
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
        public async Task<ActionResult<User>> UpdateUser([FromBody] User User)
        {
            var data = await db_User.GetById(User.UserId);
            if (data != null)
            {
                data.UserName = User.UserName;
                data.Password = User.Password;
                data.UserFullName = User.UserFullName;
                data.UserEmail = User.UserEmail;
                data.UserPhone = User.UserPhone;
                data.UserGender = User.UserGender;
                data.UserAvatar = User.UserAvatar;
                data.UserAddress = User.UserAddress;
                data.UserRole = User.UserRole;
                data.UpdatedAt = User.UpdatedAt;
                await db_User.Update(data);
                return Ok();
            }
            return NotFound();

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
