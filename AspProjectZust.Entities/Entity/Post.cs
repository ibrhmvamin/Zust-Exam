using AspProjectZust.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspProjectZust.Entities.Entity
{
    public class Post : IEntity
    {
        public int Id { get; set; }
        public string? CustomIdentityUserId { get; set; }
        public string? Images { get; set; }
        public bool? IsImage { get; set; }
        public string? Videos { get; set; }
        public bool? IsVideos { get; set; }
        public ICollection<Friend>? TaggedFriends { get; set; }
        public string? Content { get; set; }
        public DateTime PublishTime { get; set; }
        public int? LikeCount { get; set; } = 0;
        public int? CommentCount { get; set; } = 0;

        public CustomIdentityUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<UserLikedPost>? UserLikedPosts { get; set; }
    }
}
