using AspProjectZust.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Business.Abstract
{
    public interface IChatService
    {
        Task<List<Chat>> GetAll();
        //Task<List<CustomIdentityUser>> GetAllByCategory(int categoryId);
        Task Add(Chat user);
        //Task Update(CustomIdentityUser user);
        Task Delete(int id);
        Task<Chat> GetById(int id);
    }
}
