using AspProjectZust.Business.Abstract;
using AspProjectZust.Entities.Entity;
using AspProjectZust.WebUI.Helpers;
using AspProjectZust.WebUI.Hubs;
using AspProjectZust.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace AspProjectZust.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private UserManager<CustomIdentityUser> _userManager;
        private readonly IUserService _userService;
        private IWebHostEnvironment _webHost;
        private CustomIdentityDbContext _dbContext;
        private readonly SignInManager<CustomIdentityUser> _signInManager;

        public HomeController(UserManager<CustomIdentityUser> userManager, IUserService userService, CustomIdentityDbContext dbContext, IWebHostEnvironment webHost, SignInManager<CustomIdentityUser> signInManager)
        {
            _userManager = userManager;
            _userService = userService;
            _dbContext = dbContext;
            _webHost = webHost;
            _signInManager = signInManager;
        }

        public IActionResult Favorite()
        {
            return View();
        }


        public IActionResult Friends()
        {
            return View();
        }

        public IActionResult LiveChat()
        {
            return View();
        }

        [HttpPost(Name = "AddMessage")]
        public async Task<IActionResult> AddMessage(MessageViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.SenderId == model.SenderId && c.ReceiverId == model.ReceiverId
                || c.SenderId == model.ReceiverId && c.ReceiverId == model.SenderId);
                if (chat != null)
                {
                    var message = new Message
                    {
                        ChatId = chat.id,
                        Content = model.Message,
                        WriteTime = DateTime.Now,
                        DateTimeString = DateTime.Now.ToShortTimeString(),
                        HasSeen = false,
                        IsImage = false,
                        ReceiverId = model.ReceiverId,
                        SenderId = user.Id,
                    };

                    FriendRequest request = null;

                    if (user.Id != model.ReceiverId)
                    {
                        message.ReceiverId = model.ReceiverId;
                        message.SenderId = user.Id;

                        request = new FriendRequest
                        {
                            Content = model.Message,
                            Status = "Message",
                            SenderId = user.Id,
                            Sender = user,
                            ReceiverId = model.ReceiverId,
                        };
                    }
                    else
                    {
                        message.SenderId = user.Id;
                        message.ReceiverId = model.SenderId;
                        request = new FriendRequest
                        {
                            Content = model.Message,
                            Status = "Message",
                            SenderId = user.Id,
                            Sender = user,
                            ReceiverId = model.SenderId,
                            RequestTime = DateTime.Now.ToShortDateString() + "\t\t" + DateTime.Now.ToShortTimeString(),
                        };
                    }

                    await _dbContext.FriendRequests.AddAsync(request);
                    await _dbContext.Messages.AddAsync(message);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        public async Task<IActionResult> AllUsers()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var allUsers = await _dbContext.Users.Where(i => i.Id != user.Id).ToListAsync();
            return Ok(allUsers);
        }

        public async Task<IActionResult> PostLike(int postId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var allUsers = await _dbContext.Users.ToListAsync();
            var post = await _dbContext.Posts.Include(nameof(Post.User)).FirstOrDefaultAsync(p => p.Id == postId);

            post.User.LikeCount += 1;
            post.LikeCount += 1;

            var userLikePost = new UserLikedPost
            {
                Post = post,
                PostId = post.Id,
                UserId = user.Id,
                User = user,
            };

            await _dbContext.UserLikedPosts.AddAsync(userLikePost);

            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
            return Ok(allUsers);
        }

        public async Task<IActionResult> PostDisLike(int postId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var allUsers = await _dbContext.Users.ToListAsync();
            var post = await _dbContext.Posts.Include(nameof(Post.User)).FirstOrDefaultAsync(p => p.Id == postId);
            var userLikedPost = await _dbContext.UserLikedPosts.FirstOrDefaultAsync(f => f.UserId == user.Id && f.PostId == post.Id);

            post.User.LikeCount -= 1;
            post.LikeCount -= 1;

            _dbContext.UserLikedPosts.Remove(userLikedPost);

            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
            return Ok(allUsers);
        }
        
        public async Task<IActionResult> AddPostComment(int postId, string commentAddedUserId, string coment)
        {
            var allUsers = await _dbContext.Users.ToListAsync();
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            var commentUser = await _dbContext.Users.FirstOrDefaultAsync(f => f.Id == commentAddedUserId.ToString());
            post.CommentCount += 1;
            var comment = new Comment
            {
                CustomIdentityUserId = commentAddedUserId,
                LikeCount = 0,
                Post = post,
                PostId = postId,
                User = commentUser,
                WriteTime = DateTime.Now,
                Content = coment
            };

            var notification = new FriendRequest
            {
                ReceiverId = post.CustomIdentityUserId,
                RequestTime = DateTime.Now.ToShortDateString() + "\t\t" + DateTime.Now.ToShortTimeString(),
                SenderId = commentAddedUserId,
                Status = "Notification",
                Content = "Posted A Comment On Your Status",
                Sender = commentUser
            };

            _dbContext.Posts.Update(post);
            await _dbContext.FriendRequests.AddAsync(notification);

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return Ok(allUsers);
        }

        public async Task<IActionResult> Messages(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var chat = await _dbContext.Chats.Include(nameof(Chat.Receiver)).FirstOrDefaultAsync(c => c.SenderId == user.Id && c.ReceiverId == id || c.ReceiverId == user.Id && c.SenderId == id);
            var receiver = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            var messageNotification = await _dbContext.FriendRequests.Where(f => f.ReceiverId == user.Id && f.SenderId == receiver.Id && f.Status == "Message").ToListAsync();

            if (messageNotification != null)
            {
                _dbContext.FriendRequests.RemoveRange(messageNotification);
                await _dbContext.SaveChangesAsync();
            }

            if (chat == null)
            {
                chat = new Chat
                {
                    Messages = new List<Message>(),
                    ReceiverId = id,
                    SenderId = user.Id,
                };

                await _dbContext.Chats.AddAsync(chat);
                await _dbContext.SaveChangesAsync();
            }

            if (chat.ReceiverId == user.Id)
            {
                chat.Receiver = _dbContext.Users.FirstOrDefault(u => u.Id == chat.SenderId);
            }

            List<Message> messages = new List<Message>();
            if (chat != null)
            {
                messages = await _dbContext.Messages.Where(c => c.ChatId == chat.id).OrderBy(d => d.WriteTime).ToListAsync();
            }

            chat.Messages = messages;

            var model = new ChatViewModel
            {
                CurrentChat = chat,
                Sender = user,
                CurrentUserId = user.Id,
            };

            return View(model);
        }


        public async Task<IActionResult> GetAllMessages(string receiverId, string senderId)
        {
            var chat = await _dbContext.Chats.Include(nameof(Chat.Receiver)).FirstOrDefaultAsync(c => c.SenderId == receiverId && c.ReceiverId == senderId || c.ReceiverId == receiverId && c.SenderId == senderId);

            var receiver = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == senderId);
            var sender = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == receiverId);
            var messages = await _dbContext.Messages.Where(m => m.ReceiverId == receiverId && m.SenderId == senderId || m.SenderId == receiverId && m.ReceiverId == senderId).OrderBy(d => d.WriteTime).ToListAsync();
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (chat.ReceiverId == user.Id)
            {
                chat.Receiver = _dbContext.Users.FirstOrDefault(u => u.Id == chat.SenderId);
            }

            chat.Messages = messages;



            return Ok(new { CurrenUserId = user.Id, ReceiverName = chat.Receiver.UserName, SenderName = user.UserName, Chat = chat, ReceiverImageUrl = chat.Receiver.ImageUrl, SenderImageUrl = user.ImageUrl });
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(UserInfoViewModel userInfo = null)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (userInfo != null)
            {
                var helper = new FileStorageService(_webHost);
                if (userInfo.File != null)
                {
                    userInfo.ImageUrl = await helper.SaveFile(userInfo.File);
                    user.ImageUrl = userInfo.ImageUrl;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            ViewBag.User = user;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> NewsFeed()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = user;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var allUsers = await _dbContext.Users.ToListAsync();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var post = await _dbContext.Posts.FirstOrDefaultAsync(f => f.Id == postId);
            var messages = await _dbContext.Comments.Where(g => g.PostId == post.Id).ToListAsync();

            var d = user.LikeCount - post.LikeCount;
            user.LikeCount = (int)d;

            if (post != null)
            {
                _dbContext.Users.Update(user);
                _dbContext.Comments.RemoveRange(messages);
                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }

            return Ok(allUsers);
        }



        [HttpPost(Name = "NewsFeed")]
        public async Task<IActionResult> NewsFeed(PostAddedViewModel post)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (post.TagFriends != null)
            {
                var tagFriends = post.TagFriends.Split('@');
                var myFriends = await _dbContext.Friends.Where(f => f.YourFriendId == user.Id).ToListAsync();
                var data = await _dbContext.Users.Where(i => i.Id != user.Id).ToListAsync();
                var friends = new List<CustomIdentityUser>();
                foreach (var item in data)
                {
                    var friend = myFriends.FirstOrDefault(f => f.OwnId == item.Id);
                    if (friend != null)
                    {
                        var fri = await _dbContext.Users.FirstOrDefaultAsync(f => f.Id == friend.OwnId || f.Id == friend.YourFriendId && f.Id != user.Id);
                        for (int i = 0; i < tagFriends.Length; i++)
                        {
                            if (fri != null)
                            {
                                if (tagFriends[i].ToLower() == fri.UserName.ToLower())
                                {
                                    friends.Add(fri);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (post.Content != null && post.Content.Trim() != String.Empty || post.Image != null || post.VideoUrl != null && post.VideoUrl != String.Empty)
            {
                var helper = new FileStorageService(_webHost);
                if (post.Image != null)
                {
                    post.ImageUrl = await helper.SaveFile(post.Image);
                }


                var newPost = new Post
                {
                    Content = post.Content,
                    CustomIdentityUserId = user.Id,
                    Images = post.ImageUrl,
                    PublishTime = DateTime.Now,
                    User = user,
                    Videos = post.VideoUrl,
                };

                if (post.VideoUrl != null)
                {
                    var split = post.VideoUrl.Split('/');
                    var d = split[split.Length - 1].Split('=');
                    newPost.Videos = d[1];
                }
                await _dbContext.Posts.AddAsync(newPost);
                await _dbContext.SaveChangesAsync();
            }

            ViewBag.User = user;
            return View();
        }

        public async Task<IActionResult> EditCoverMyProfile(UserInfoViewModel userInfo)
        {
            var helper = new FileStorageService(_webHost);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (userInfo.File != null)
            {
                userInfo.ImageUrl = await helper.SaveFile(userInfo.File);
                user.EditCoverImageUrl = userInfo.ImageUrl;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            ViewBag.User = user;
            return RedirectToAction("MyProfile", "Home");
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public async Task<IActionResult> AlreadySent(string id)
        {
            try
            {
                var senderUser = await _userManager.GetUserAsync(HttpContext.User);
                var receiverUser = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);

                var request = await _dbContext.FriendRequests.FirstOrDefaultAsync(f => f.SenderId == senderUser.Id && f.ReceiverId == receiverUser.Id && f.Status == "Request");

                _dbContext.FriendRequests.Remove(request);
                var sendRequest = new FriendRequest
                {
                    Content = $"He withdrew the friendly situation he had sent",
                    SenderId = senderUser.Id,
                    Sender = senderUser,
                    ReceiverId = id,
                    Status = "Notification",
                    RequestTime = DateTime.Now.ToShortDateString() + "\t\t" + DateTime.Now.ToShortTimeString(),
                };

                await _dbContext.FriendRequests.AddAsync(sendRequest);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> GetAllUsers()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var data = await _dbContext.Users.Where(i => i.Id != user.Id).ToListAsync();
            var myRequests = _dbContext.FriendRequests.Where(s => s.SenderId == user.Id);
            var myFriends = _dbContext.Friends.Where(f => f.OwnId == user.Id || f.YourFriendId == user.Id);

            foreach (var item in data)
            {
                var request = myRequests.FirstOrDefault(r => r.ReceiverId == item.Id && r.Status == "Request");
                if (request != null)
                {
                    item.HasRequestPending = true;
                }
                var friend = myFriends.FirstOrDefault(f => f.OwnId == item.Id || f.YourFriendId == item.Id);
                if (friend != null)
                {
                    item.IsFriend = true;
                }
            }

            return Ok(data);
        }


        public async Task<IActionResult> SendFollow(string id)
        {
            var senderUser = await _userManager.GetUserAsync(HttpContext.User);
            var receiverUser = _userManager.Users.FirstOrDefault(i => i.Id == id);
            if (receiverUser != null)
            {
                var request = new FriendRequest
                {
                    Content = $"Sent You A Friend Request",
                    SenderId = senderUser.Id,
                    Sender = senderUser,
                    ReceiverId = id,
                    Status = "Request",
                    RequestTime = DateTime.Now.ToShortDateString() + "\t\t" + DateTime.Now.ToShortTimeString(),
                };
                _dbContext.FriendRequests.Add(request);
                await _userManager.UpdateAsync(receiverUser);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        public async Task<IActionResult> GetPosts()
        {
            var allUsers = await _dbContext.Users.ToListAsync();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var myFriends = await _dbContext.Friends.Where(f => f.YourFriendId == user.Id).ToListAsync();
            var data = await _dbContext.Users.Where(i => i.Id != user.Id).ToListAsync();
            var friendPosts = new List<Post>();
            foreach (var item in data)
            {
                var friend = myFriends.FirstOrDefault(f => f.OwnId == item.Id);
                if (friend != null)
                {
                    var fri = await _dbContext.Users.FirstOrDefaultAsync(f => f.Id == friend.OwnId || f.Id == friend.YourFriendId && f.Id != user.Id);
                    var post = await _dbContext.Posts.Where(p => p.CustomIdentityUserId == fri.Id).ToListAsync();
                    for (int i = 0; i < post.Count(); i++)
                    {
                        var comments = await _dbContext.Comments.Include(nameof(Comment.Post)).Where(f => f.PostId == post[i].Id).ToListAsync();
                        var userLiked = await _dbContext.UserLikedPosts.FirstOrDefaultAsync(f => f.UserId == user.Id && f.PostId == post[i].Id);
                        post[i].Comments = comments;
                        if (post[i].Images != null)
                        {
                            post[i].IsImage = true;
                        }
                        else
                        {
                            post[i].IsImage = false;
                        }

                        if (userLiked != null)
                        {
                            post[i].User.IsUserLikedPost = true;
                        }
                        else
                        {
                            post[i].User.IsUserLikedPost = false;
                        }

                    }
                    friendPosts.AddRange(post);
                }
            }
            var myPosts = await _dbContext.Posts.Where(p => p.CustomIdentityUserId == user.Id).ToListAsync();
            for (int i = 0; i < myPosts.Count(); i++)
            {
                var comments = await _dbContext.Comments.Include(nameof(Comment.Post)).Where(f => f.PostId == myPosts[i].Id).ToListAsync();
                var userLiked2 = await _dbContext.UserLikedPosts.FirstOrDefaultAsync(f => f.UserId == user.Id && f.PostId == myPosts[i].Id);
                myPosts[i].Comments = comments;
                if (myPosts[i].Images != null)
                {
                    myPosts[i].IsImage = true;
                }
                else
                {
                    myPosts[i].IsImage = false;
                }

                if (userLiked2 != null)
                {
                    myPosts[i].User.IsUserLikedPost = true;
                }
                else
                {
                    myPosts[i].User.IsUserLikedPost = false;
                }
            }
            friendPosts.AddRange(myPosts);
            return Ok(new { friendPost = friendPosts, Current = user, allUser = allUsers });
        }

        public async Task<IActionResult> UnFollowCall(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var friendUser = _userManager.Users.FirstOrDefault(i => i.Id == id);

            var friend = _dbContext.Friends.Where(f => f.OwnId == id && f.YourFriendId == user.Id || f.OwnId == user.Id && f.YourFriendId == id);


            user.FollowersCount -= 1;
            user.FollowingCount -= 1;

            friendUser.FollowersCount -= 1;
            friendUser.FollowingCount -= 1;

            var request = new FriendRequest
            {
                Content = $"He unfriended you",
                SenderId = user.Id,
                Sender = user,
                ReceiverId = id,
                Status = "Notification",
                RequestTime = DateTime.Now.ToShortDateString() + "\t\t" + DateTime.Now.ToShortTimeString(),
            };

            await _userManager.UpdateAsync(user);
            await _userManager.UpdateAsync(friendUser);

            await _dbContext.FriendRequests.AddAsync(request);


            _dbContext.Friends.RemoveRange(friend);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests(string filter = "")
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            List<FriendRequest> requests = null;
            filter = filter.Trim();
            if (filter == "")
            {
                requests = await _dbContext.FriendRequests.Include(nameof(FriendRequest.Sender)).Where(r => r.ReceiverId == current.Id).OrderByDescending(r => r.RequestTime).ToListAsync();
            }
            else
            {
                requests = await _dbContext.FriendRequests.Include(nameof(FriendRequest.Sender)).Where(r => r.ReceiverId == current.Id && r.Sender.UserName.Contains(filter) && r.Status == "Message").OrderByDescending(r => r.RequestTime).ToListAsync();
            }
            return Ok(requests);
        }

        public async Task<IActionResult> CurrentUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return Ok(user);
        }
        public async Task<IActionResult> DeleteRequest(int requestId, string senderId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var sender = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == senderId);

            var request = await _dbContext.FriendRequests.FirstOrDefaultAsync(r => r.Id == requestId);

            var newRequest = new FriendRequest
            {
                ReceiverId = sender.Id,
                Sender = user,
                SenderId = user.Id,
                Status = "Notification",
                Content = "Rejected the offer of friendship",
                RequestTime = DateTime.Now.ToShortDateString() + "\t\t" + DateTime.Now.ToShortTimeString(),
            };

            await _dbContext.FriendRequests.AddAsync(newRequest);


            _dbContext.FriendRequests.Remove(request);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> DeleteNotification()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var requests = await _dbContext.FriendRequests.Where(u => u.ReceiverId == user.Id && u.Status == "Notification").ToListAsync();
            if (requests != null)
            {
                _dbContext.FriendRequests.RemoveRange(requests);
                await _dbContext.SaveChangesAsync();
                return Ok(user.Id);
            }
            return BadRequest();
        }

        public async Task<IActionResult> DeleteMessageNotification()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var requests = await _dbContext.FriendRequests.Where(u => u.ReceiverId == user.Id && u.Status == "Message").ToListAsync();
            if (requests != null)
            {
                _dbContext.FriendRequests.RemoveRange(requests);
                await _dbContext.SaveChangesAsync();
                return Ok(user.Id);
            }
            return BadRequest();
        }

        public async Task<IActionResult> NotificationGeneralFormOfInformation(int requestId)
        {
            var requests = await _dbContext.FriendRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (requests != null)
            {
                _dbContext.FriendRequests.Remove(requests);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        public async Task<IActionResult> UserMessage(string id)
        {
            var friend = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            var sender = await _userManager.GetUserAsync(HttpContext.User);

            var chat = await _dbContext.Chats.Include(nameof(Chat.Receiver)).FirstOrDefaultAsync(c => c.SenderId == sender.Id && c.ReceiverId == id || c.ReceiverId == sender.Id && c.SenderId == id);

            if (chat == null)
            {
                chat = new Chat
                {
                    Messages = new List<Message>(),
                    ReceiverId = id,
                    SenderId = sender.Id,
                };

                await _dbContext.Chats.AddAsync(chat);
                await _dbContext.SaveChangesAsync();
            }

            if (chat.ReceiverId == sender.Id)
            {
                chat.Receiver = _dbContext.Users.FirstOrDefault(u => u.Id == chat.SenderId);
            }

            List<Message> messages = new List<Message>();
            if (chat != null)
            {
                messages = await _dbContext.Messages.Where(c => c.ChatId == chat.id).OrderBy(d => d.WriteTime).ToListAsync();
            }

            chat.Messages = messages;


            return Ok(new { CurrenUserId = sender.Id, ReceiverUserId = friend.Id, ReceiverName = chat.Receiver.UserName, SenderName = sender.UserName, Chat = chat, ReceiverImageUrl = chat.Receiver.ImageUrl, SenderImageUrl = sender.ImageUrl });
        }


        public async Task<IActionResult> ConfirmRequest(string senderId, int requestId)
        {
            var receiver = await _userManager.GetUserAsync(HttpContext.User);
            var sender = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == senderId);


            var receiverFriend = new Friend
            {
                OwnId = receiver.Id,
                YourFriendId = sender.Id,
            };

            var senderFriend = new Friend
            {
                OwnId = sender.Id,
                YourFriendId = receiver.Id,
            };

            _dbContext.Friends.Add(senderFriend);
            _dbContext.Friends.Add(receiverFriend);

            receiver.FollowersCount += 1;
            receiver.FollowingCount += 1;

            sender.FollowersCount += 1;
            sender.FollowingCount += 1;

            var request = await _dbContext.FriendRequests.FirstOrDefaultAsync(f => f.Id == requestId);

            _dbContext.FriendRequests.Remove(request);

            await _userManager.UpdateAsync(receiver);
            await _userManager.UpdateAsync(sender);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}

