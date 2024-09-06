using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using DAL.Models;

namespace Repository.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment?>> GetAllCommentsAsync();
        Task<Comment?> GetCommentByIdAsync(int id);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int id);
    }
}