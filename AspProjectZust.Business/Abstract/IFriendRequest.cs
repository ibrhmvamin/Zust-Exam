using AspProjectZust.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Business.Abstract
{
    public interface IFriendRequest
    {
        Task<List<Friend>> GetAll();
        //Task<List<CustomIdentityUser>> GetAllByCategory(int categoryId);
        Task Add(Friend user);
        //Task Update(CustomIdentityUser user);
        Task Delete(int id);
        Task<Friend> GetById(int id);
    }
}
