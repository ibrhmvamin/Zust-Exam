using AspProjectZust.Business.Abstract;
using AspProjectZust.DataAccess.Abstract;
using AspProjectZust.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Business.Concrete
{
    public class UserService : IUserService
    {
        private IUserDal _userDal;

        public UserService(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public async Task Add(CustomIdentityUser user)
        {
            await _userDal.Add(user);
        }

        public async Task Delete(int id)
        {
            var user = await _userDal.Get(u => u.Id == id.ToString());

            if (user != null)
            {
                await _userDal.Delete(user);
            }
        }

        public async Task<List<CustomIdentityUser>> GetAll()
        {
            return await _userDal.GetList();
        }

        public Task<List<CustomIdentityUser>> GetAllByCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomIdentityUser> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(CustomIdentityUser user)
        {
            await _userDal.Update(user);
        }
    }
}
