using AspProjectZust.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Business.Abstract
{
    public interface ICommentService
    {
        Task<List<Comment>> GetAll();
        //Task<List<CustomIdentityUser>> GetAllByCategory(int categoryId);
        Task Add(Comment user);
        //Task Update(CustomIdentityUser user);
        Task Delete(int id);
        Task<Comment> GetById(int id);
    }
}
