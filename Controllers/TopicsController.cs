using GITA.Data;
using GITA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GITA.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TopicsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var topic = new Topic
            {
                Title = model.Title,
                Content = model.Content,
                UserId = user.Id,
                State = "pending",
                Status = "active"
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Topic created successfully!", TopicId = topic.Id });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/state")]
        public async Task<IActionResult> UpdateTopicState(int id, [FromBody] UpdateTopicStateModel model)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            if (model.State != "show" && model.State != "hide")
            {
                return BadRequest(new { Message = "Invalid state value. Allowed values are 'show' or 'hide'." });
            }

            topic.State = model.State;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Topic state updated successfully!" });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTopicStatus(int id, [FromBody] UpdateTopicStatusModel model)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            if (model.Status != "active" && model.Status != "inactive")
            {
                return BadRequest(new { Message = "Invalid status value. Allowed values are 'active' or 'inactive'." });
            }

            topic.Status = model.Status;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Topic status updated successfully!" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllTopics()
        {
            var topics = await _context.Topics
                .Include(t => t.User)
                .Include(t => t.Comments)
                .Select(t => new TopicInfoModel
                {
                    Title = t.Title,
                    CommentCount = t.Comments.Count,
                    CreatedDate = t.CreatedDate,
                    AuthorFirstName = t.User.FirstName,
                    AuthorLastName = t.User.LastName,
                    State = t.State,
                    Status = t.Status
                })
                .ToListAsync();

            return Ok(topics);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicTopics()
        {
            var topics = await _context.Topics
                .Include(t => t.User)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .Where(t => t.State == "show" && t.Status == "active")
                .Select(t => new TopicWithCommentsModel
                {
                    Title = t.Title,
                    Content = t.Content,
                    CreatedDate = t.CreatedDate,
                    AuthorFirstName = t.User.FirstName,
                    AuthorLastName = t.User.LastName,
                    State = t.State,
                    Status = t.Status,
                    Comments = t.Comments.Select(c => new CommentModel
                    {
                        Content = c.Content,
                        CreatedDate = c.CreatedDate,
                        AuthorFirstName = c.User.FirstName,
                        AuthorLastName = c.User.LastName
                    }).ToList()
                })
                .ToListAsync();

            return Ok(topics);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTopicsLogedUsers()
        {
            var topics = await _context.Topics
                .Include(t => t.User)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .Select(t => new TopicWithCommentsModel
                {
                    //Id = t.Id,
                    Title = t.Title,
                    Content = t.Content,
                    CreatedDate = t.CreatedDate,
                    AuthorFirstName = t.User.FirstName,
                    AuthorLastName = t.User.LastName,
                    State = t.State,
                    Status = t.Status,
                    Comments = t.Comments.Select(c => new CommentModel
                    {
                        //Id = c.Id,
                        Content = c.Content,
                        CreatedDate = c.CreatedDate,
                        AuthorFirstName = c.User.FirstName,
                        AuthorLastName = c.User.LastName
                    }).ToList()
                })
                .ToListAsync();

            return Ok(topics);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] CreateTopicModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null || topic.UserId != user.Id)
            {
                return NotFound();
            }

            if (topic.Status == "inactive")
            {
                return BadRequest(new { Message = "Cannot update an inactive topic." });
            }

            topic.Title = model.Title;
            topic.Content = model.Content;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Topic updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null || topic.UserId != user.Id)
            {
                return NotFound();
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Topic deleted successfully!" });
        }

        [Authorize]
        [HttpPost("{topicId}/comments")]
        public async Task<IActionResult> CreateComment(int topicId, [FromBody] CreateCommentModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                return NotFound();
            }

            if (topic.Status == "inactive")
            {
                return BadRequest(new { Message = "Cannot add comments to an inactive topic." });
            }

            var comment = new Comment
            {
                Content = model.Content,
                TopicId = topicId,
                UserId = user.Id
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment created successfully!", CommentId = comment.Id });
        }

        [Authorize]
        [HttpPut("comments/{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateCommentModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != user.Id)
            {
                return NotFound();
            }

            if (comment.Topic.Status == "inactive")
            {
                return BadRequest(new { Message = "Cannot update a comment on an inactive topic." });
            }

            comment.Content = model.Content;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment updated successfully!" });
        }

        [Authorize]
        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != user.Id)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment deleted successfully!" });
        }
    }

}

