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
    public class PostService : IPostService
    {
        private IPostDal _postDal;

        public PostService(IPostDal postDal)
        {
            _postDal = postDal;
        }

        public async Task Add(Post user)
        {
            await _postDal.Add(user);
        }

        public async Task Delete(int id)
        {
            var user = await _postDal.Get(u => u.Id == id);

            if (user != null)
            {
                await _postDal.Delete(user);
            }
        }

        public async Task<List<Post>> GetAll()
        {
            return await _postDal.GetList();
        }

        public async Task<Post> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
