using AspProjectZust.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Business.Abstract
{
    public interface IPostService
    {
        Task<List<Post>> GetAll();
        //Task<List<CustomIdentityUser>> GetAllByCategory(int categoryId);
        Task Add(Post user);
        //Task Update(Post user);
        Task Delete(int id);
        Task<Post> GetById(int id);
    }
}
