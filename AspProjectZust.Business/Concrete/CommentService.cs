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
    public class CommentService : ICommentService
    {
        private ICommentDal _commentDal;

        public CommentService(ICommentDal commentDal)
        {
            _commentDal = commentDal;
        }

        public async Task Add(Comment user)
        {
            await _commentDal.Add(user);
        }

        public async Task Delete(int id)
        {
            var user = await _commentDal.Get(u => u.Id == id);

            if (user != null)
            {
                await _commentDal.Delete(user);
            }
        }

        public async Task<List<Comment>> GetAll()
        {
            return await _commentDal.GetList();
        }

        public async Task<Comment> GetById(int id)
        {
            throw new NotImplementedException();
            //return await _commentDal.GetById(id);
        }
    }
}
