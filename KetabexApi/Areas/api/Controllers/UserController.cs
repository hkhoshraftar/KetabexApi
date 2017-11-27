using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KetabexApi.Controllers;
using KetabexApi.Utils;
using Microsoft.Ajax.Utilities;

namespace KetabexApi.Areas.api.Controllers
{
    public class UserController : AuthorizedController
    {
        [Route("api/user/getinfo")]
        public JsoniResult GetUserInfo(string username)
        {
            var userinfo = db.User.FirstOrDefault(q => q.Username == username);
            if (userinfo == null)
            {
                return GR(null, status: HttpStatusCode.NotFound);
            }
            else
            {
                return GR(new
                {
                    userinfo.Avatar,
                    userinfo.Nickname,
                    userinfo.Bio,
                    PostsCount = userinfo.Post.Count,
                    FollowersCount = db.Follow.Count(q => q.FollowingUserId == userinfo.Id),
                    FollowingsCount = db.Follow.Count(q => q.FollowerUserId == userinfo.Id)
                });
            }
        }

        [Route("api/user/followers")]
        public JsoniResult GetUserFollowers(string username)
        {
            var userinfo = db.User.FirstOrDefault(q => q.Username == username);

            if (userinfo == null)
            {
                return GR(null, status: HttpStatusCode.NotFound);
            }
            else
            {
                var folowers = db.Follow.Where(q => q.FollowingUserId == userinfo.Id).ToList().Select(q => new
                {
                    q.User.Avatar,
                    q.User.Nickname,
                    q.User.Bio,
                    q.User.Username,
                });
                return GR(folowers);
            }
        }

        [Route("api/user/followings")]
        public JsoniResult GetUserFollwings(string username)
        {
            var userinfo = db.User.FirstOrDefault(q => q.Username == username);

            if (userinfo == null)
            {
                return GR(null, status: HttpStatusCode.NotFound);
            }
            else
            {
                var folowers = db.Follow.Where(q => q.FollowerUserId == userinfo.Id).ToList().Select(q => new
                {
                    q.User1.Avatar,
                    q.User1.Nickname,
                    q.User1.Bio,
                    q.User1.Username,
                });
                return GR(folowers);
            }
        }


        [Route("api/user/profile")]
        public JsoniResult GetUserProfile()
        {
            return GR(new
            {
                user.Avatar,
                user.Bio,
                user.Nickname,
                user.Username,
                user.PhoneNumber
            });
        }


        [Route("api/user/saveprofile")]
        public JsoniResult SaveUserProfile(string bio, string nickname, string phonenumber)
        {
            try
            {
                var entity = db.User.First(q => q.Id == user.Id);
                entity.Bio = bio;
                entity.Nickname = nickname;
                entity.PhoneNumber = phonenumber;
                db.SaveChangesAsync();

                return GR(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return GR(false, preferedMessage: "لطفا در وارد نمودن اطلاعات دقت کنید",
                status: HttpStatusCode.NotAcceptable);
        }


        [Route("api/user/saveavatar")]
        public JsoniResult SaveUserAvatar(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return GR(false, preferedMessage: "لطفا ارسال تصویر دقت کنید", status: HttpStatusCode.NotAcceptable);
            }
            var fileName = user.Username + "_avatar_" + Util.DateTimeToUnixTimestamp(DateTime.Now) + ".jpg";
            fileName = "/Content/Upload/Avatars/" + fileName;
            file.SaveAs(Server.MapPath("~" + fileName));

            var entity = db.User.First(q => q.Id == user.Id);
            entity.Avatar = fileName;
            db.SaveChangesAsync();

            return GR(fileName);
        }


        [Route("api/user/allposts")]
        public JsoniResult TopPosts(string username, int lastUpdateId = -1, int count = 20)
        {
            var userinfo = db.User.FirstOrDefault(q => q.Username == username);
            if (userinfo == null)
            {
                return GR(null, status: HttpStatusCode.NotFound);
            }
            else
            {
                if (count < 1)
                {
                    return GR(null, status: HttpStatusCode.NotAcceptable);
                }

                count = count > 20 ? 20 : count;

                var posts = lastUpdateId == -1
                    ? db.Post.Where(q => q.UserId == userinfo.Id).OrderByDescending(q => q.PublishDate)
                        .ThenByDescending(q => q.Like.Count).Take(count)
                        .ToList()
                    : db.Post.Where(q => q.UserId == userinfo.Id && q.Id < lastUpdateId)
                        .OrderByDescending(q => q.PublishDate)
                        .ThenByDescending(q => q.Like.Count)
                        .Take(count).ToList();

                return GR(posts.Select(q => new
                    {
                        PostId = q.Id,
                        PublishDate = Util.DateTimeToUnixTimestamp(q.PublishDate),
                        q.UserId,
                        q.BookId,
                        BookTitle = q.Book.Title,
                        q.Description,
                        q.Score,
                        q.Status,
                        LikesCount = q.Like.Count,
                        CommentsCount = q.Comment.Count,
                        q.User.Avatar,
                        q.User.Nickname,
                        q.User.Username,
                        BookCoverUrl = q.Book.CoverUrl
                    })
                );
            }
        }
    }
}