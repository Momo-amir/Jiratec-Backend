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
            try
            {
                var comments = await _commentRepository.GetCommentsByTaskIdAsync(taskId);

                if (comments == null || !comments.Any())
                {
                    return NotFound("No comments found for this task.");
                }

                var commentDtos = comments.Select(c => new CommentDto
                {
                    CommentID = c.CommentID,
                    TaskID = c.TaskID,
                    UserID = c.UserID,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    User = new UserDTO
                    {
                        UserID = c.User.UserID,
                        Name = c.User.Name,
                        Email = c.User.Email
                    }
                });

                return Ok(commentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentDto commentDto)
        {
            if (id != commentDto.CommentID)
            {
                return BadRequest("Comment ID mismatch.");
            }

            try
            {
                var existingComment = await _commentRepository.GetCommentByIdAsync(id);

                if (existingComment == null)
                {
                    return NotFound("Comment not found.");
                }

                existingComment.Content = commentDto.Content;
                existingComment.CreatedDate = commentDto.CreatedDate;

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CommentDto commentDto)
        {
            if (commentDto == null || commentDto.TaskID == 0 || commentDto.UserID == 0 || string.IsNullOrEmpty(commentDto.Content))
            {
                return BadRequest("Invalid comment payload.");
            }

            try
            {
                var comment = new Comment
                {
                    TaskID = commentDto.TaskID,
                    UserID = commentDto.UserID,
                    Content = commentDto.Content,
                    CreatedDate = DateTime.Now
                };

                await _commentRepository.AddCommentAsync(comment);

                var createdCommentDto = new CommentDto
                {
                    CommentID = comment.CommentID,
                    TaskID = comment.TaskID,
                    UserID = comment.UserID,
                    Content = comment.Content,
                    CreatedDate = comment.CreatedDate,
                    User = null
                };

                return CreatedAtAction(nameof(GetComment), new { id = comment.CommentID }, createdCommentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                await _commentRepository.DeleteCommentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}