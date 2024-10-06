using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByTaskId(int taskId)
        {
            var comments = await _commentRepository.GetCommentsByTaskIdAsync(taskId);

            if (comments == null || !comments.Any())
            {
                return NotFound("No comments found for this task.");
            }

            // Convert to DTO
            var commentDtos = comments.Select(c => new CommentDto
            {
                CommentID = c.CommentID,
                TaskID = c.TaskID,
                UserID = c.UserID,
                Content = c.Content,
                CreatedDate = c.CreatedDate,
                User = new UserDTO // Make sure this resolves properly
                {
                    UserID = c.User.UserID,
                    Name = c.User.Name,
                    Email = c.User.Email
                }
            });

            return Ok(commentDtos);
        }
    

        // GET: api/comments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            // Convert to DTO
            var commentDto = new CommentDto
            {
                CommentID = comment.CommentID,
                TaskID = comment.TaskID,
                UserID = comment.UserID,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate
            };

            return Ok(commentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentDto commentDto)
        {
            if (id != commentDto.CommentID)
            {
                return BadRequest("Comment ID mismatch.");
            }

            // Fetch the existing comment from the repository
            var existingComment = await _commentRepository.GetCommentByIdAsync(id);

            if (existingComment == null)
            {
                return NotFound("Comment not found.");
            }

            // Update the existing comment's fields with the new values from commentDto
            existingComment.Content = commentDto.Content;
            existingComment.CreatedDate = commentDto.CreatedDate; // If the date is intended to be updated
            // Note: TaskID and UserID are usually not changed when updating a comment, but adjust accordingly if needed.

            try
            {
                await _commentRepository.UpdateCommentAsync(existingComment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _commentRepository.GetCommentByIdAsync(id) == null)
                {
                    return NotFound("Comment not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/comments
       [HttpPost]
public async Task<ActionResult<CommentDto>> CreateComment(CommentDto commentDto)
{
    // Validate the payload
    if (commentDto == null || commentDto.TaskID == 0 || commentDto.UserID == 0 || string.IsNullOrEmpty(commentDto.Content))
    {
        return BadRequest("Invalid comment payload.");
    }

    // Convert DTO to entity
    var comment = new Comment
    {
        TaskID = commentDto.TaskID,
        UserID = commentDto.UserID,
        Content = commentDto.Content,
        CreatedDate = DateTime.Now
    };

    // Add the comment using the repository
    await _commentRepository.AddCommentAsync(comment);

    // Prepare the response DTO
    var createdCommentDto = new CommentDto
    {
        CommentID = comment.CommentID,
        TaskID = comment.TaskID,
        UserID = comment.UserID,
        Content = comment.Content,
        CreatedDate = comment.CreatedDate,
        // Optionally include the UserDTO if needed
        User = null // Since we didn't include it, it's null
    };

    return CreatedAtAction(nameof(GetComment), new { id = comment.CommentID }, createdCommentDto);
}

        // DELETE: api/comments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteCommentAsync(id);
            return NoContent();
        }
    }
}
