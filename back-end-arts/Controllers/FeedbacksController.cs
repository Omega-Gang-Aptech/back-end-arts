using back_end_arts.Models;
using back_end_arts.Repository;
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
    public class FeedbacksController : ControllerBase
    {
        private IArtsRepository<Feedback> db_feedback;
        public FeedbacksController(IArtsRepository<Feedback> db_feedback)
        {
            this.db_feedback = db_feedback;
        }


        ///Feedback
        [HttpGet("Categories")]
        public async Task<IEnumerable<Feedback>> GetCategories()
        {
            return await db_feedback.ListAll();
        }
        [HttpGet("Feedback")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
            return await db_feedback.GetById(id);
        }
        [HttpPost("CreateFeedback")]
        public async Task<ActionResult<Feedback>> CreateFeedback([FromBody] Feedback Feedback)
        {

            await db_feedback.Insert(Feedback);
            return CreatedAtAction(nameof(GetCategories), new { id = Feedback.FeedbackId }, Feedback);
        }
        [HttpPut("UpdateFeedback")]
        public async Task<ActionResult<Feedback>> UpdateFeedback([FromBody] Feedback Feedback)
        {
            var data = await db_feedback.GetById(Feedback.FeedbackId);
            if (data != null)
            {
                data.FeedbackUserId = Feedback.FeedbackUserId;
                data.FeedbackProductId = Feedback.FeedbackProductId;
                data.Comment = Feedback.Comment;
                data.Rating = Feedback.Rating;
                data.UpdatedAt = Feedback.UpdatedAt;
                await db_feedback.Update(data);
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("FeedbackId")]
        public async Task<ActionResult<Feedback>> DeleteFeedback(int id)
        {
            var data = await db_feedback.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            await db_feedback.Delete(data);
            return NoContent();
        }
    }
}
