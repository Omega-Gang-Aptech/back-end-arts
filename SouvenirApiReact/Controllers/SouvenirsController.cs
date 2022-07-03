using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SouvenirApiReact.Models;
using SouvenirApiReact.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouvenirApiReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SouvenirsController : ControllerBase
    {
        private readonly ISouvenirRepository _souvenirRepository;
        public SouvenirsController(ISouvenirRepository souvenirRepository)
        {
            _souvenirRepository = souvenirRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<Souvenir>> GetsBooks()
        {
            return await _souvenirRepository.Gets();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Souvenir>> GetBooks(int id)
        {
            return await _souvenirRepository.Get(id);
        }
        [HttpPost]
        public async Task<ActionResult<Souvenir>> PostBook([FromBody] Souvenir souvenir)
        {
            var data = await _souvenirRepository.Create(souvenir);
            return CreatedAtAction(nameof(GetsBooks), new { id = data.SouvenirId }, data);
        }
        [HttpPut]
        public async Task<ActionResult> PutBook(int id, [FromBody] Souvenir souvenir)
        {
            await _souvenirRepository.Update(souvenir);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var bookdelete = await _souvenirRepository.Get(id);
            if (bookdelete == null)
            {
                return NotFound();
            }
            await _souvenirRepository.Delete(id);
            return NoContent();
        }
        [HttpGet("findAll")]
        public async Task<IEnumerable<Souvenir>> findAll(int min, int max)
        {
            return await _souvenirRepository.findAll(min, max);
        }
    }
}
