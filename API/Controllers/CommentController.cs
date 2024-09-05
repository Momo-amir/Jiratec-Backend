using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
 private readonly AppDbContext _context;

  public CommentsController(AppDbContext context)
  {
   _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
  {
   return await _context.Comments.ToListAsync();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Comment>> GetComment(int id)
  {
   var comment = await _context.Comments.FindAsync(id);

   if (comment == null)
   {
    return NotFound();
   }

   return comment;
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateComment(int id, Comment comment)
  {
   if (id != comment.CommentID)
   {
    return BadRequest();
   }

   _context.Entry(comment).State = EntityState.Modified;

   try
   {
    await _context.SaveChangesAsync();
   }
   catch (DbUpdateConcurrencyException)
   {
    if (!_context.Comments.Any(e => e.CommentID == id))
    {
                  
     return NotFound();
     
    }
   
   }

   return NoContent();

  }

  [HttpPost]
  public async Task<ActionResult<Comment>> CreateComment(Comment comment)
  {
   _context.Comments.Add(comment);
   await _context.SaveChangesAsync();

   return CreatedAtAction(nameof(GetComment), new { id = comment.CommentID }, comment);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteComment(int id)
  {
   var comment = await _context.Comments.FindAsync(id);
   if (comment == null)
   {
    return NotFound();
   }

   _context.Comments.Remove(comment);
   await _context.SaveChangesAsync();
   
   return NoContent();
   
  }
 
}