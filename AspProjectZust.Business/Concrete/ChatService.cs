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
    public class ChatService : IChatService
    {
        private IChatDal _chatDal;

        public ChatService(IChatDal chatDal)
        {
            _chatDal = chatDal;
        }

        public async Task Add(Chat user)
        {
            await _chatDal.Add(user);
        }

        public async Task Delete(int id)
        {
            var user = await _chatDal.Get(u => u.id == id);

            if (user != null)
            {
                await _chatDal.Delete(user);
            }
        }

        public async Task<List<Chat>> GetAll()
        {
            return await _chatDal.GetList();
        }

        public async Task<Chat> GetById(int id)
        {
            throw new NotImplementedException();
            //return await GetById(id);
        }
    }
}
