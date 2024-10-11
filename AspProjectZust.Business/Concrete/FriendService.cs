using AspProjectZust.Business.Abstract;
using AspProjectZust.DataAccess.Abstract;
using AspProjectZust.DataAccess.Concrete.EFEntityFramework;
using AspProjectZust.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Business.Concrete
{
    public class FriendService : IFriendRequest
    {
        private IFriendDal _friendDal;

        public FriendService(IFriendDal friendDal)
        {
            _friendDal = friendDal;
        }

        public async Task Add(Friend user)
        {
            await _friendDal.Add(user);
        }

        public async Task Delete(int id)
        {
            var user = await _friendDal.Get(u => u.Id == id);

            if (user != null)
            {
                await _friendDal.Delete(user);
            }
        }

        public async Task<List<Friend>> GetAll()
        {
            return await _friendDal.GetList();
        }

        public async Task<Friend> GetById(int id)
        {
            throw new NotImplementedException();
            //return await _friendDal.GetById(id);
        }
    }
}
