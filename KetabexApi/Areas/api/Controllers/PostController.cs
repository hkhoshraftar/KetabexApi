using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KetabexApi.Controllers;
using KetabexApi.DataModel;
using KetabexApi.Utils;

namespace KetabexApi.Areas.api.Controllers
{
    public class PostController : AuthorizedController
    {
        [Route("api/posts/followings")]
        public JsoniResult PostsOfFollowings(int lastUpdateId = -1, int count = 20)
        {
            if (count < 1)
            {
                return GR(null, status: HttpStatusCode.NotAcceptable);
            }

            count = count > 20 ? 20 : count;
            var followings = db.Follow.Where(q => q.FollowerUserId == user.Id).Select(q => q.FollowingUserId).ToList();

            var posts = lastUpdateId == -1
                ? db.Post.Where(q => followings.Contains(q.UserId)).OrderByDescending(q => q.PublishDate).Take(count)
                    .ToList()
                : db.Post.Where(q => followings.Contains(q.UserId) && q.Id < lastUpdateId)
                    .OrderByDescending(q => q.PublishDate)
                    .Take(count).ToList();

            return GR(posts.Select(q => new
                {
                     q.Id,
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

        [Route("api/posts/latest")]
        public JsoniResult LatestPosts(int lastUpdateId = -1, int count = 20)
        {
            if (count < 1)
            {
                return GR(null, status: HttpStatusCode.NotAcceptable);
            }

            count = count > 20 ? 20 : count;

            var posts = lastUpdateId == -1
                ? db.Post.OrderByDescending(q => q.PublishDate).Take(count).ToList()
                : db.Post.Where(q => q.Id < lastUpdateId).OrderByDescending(q => q.PublishDate)
                    .Take(count).ToList();

            return GR(posts.Select(q => new
                {
                q.Id,
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

        [Route("api/posts/top")]
        public JsoniResult TopPosts(int lastUpdateId = -1, int count = 20)
        {
            if (count < 1)
            {
                return GR(null, status: HttpStatusCode.NotAcceptable);
            }

            count = count > 20 ? 20 : count;

            var posts = lastUpdateId == -1
                ? db.Post.OrderByDescending(q => q.PublishDate).ThenByDescending(q=>q.Like.Count).Take(count).ToList()
                : db.Post.Where(q => q.Id < lastUpdateId).OrderByDescending(q => q.PublishDate).ThenByDescending(q => q.Like.Count)
                    .Take(count).ToList();

            return GR(posts.Select(q => new
                {
                q.Id,
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

        [Route("api/posts/like")]
        public JsoniResult LikePost(int postId)
        {
            var entity = db.Like.FirstOrDefault(q => q.PostId == postId && q.UserId == user.Id);
            if (entity == null)
            {
                entity = new Like()
                {
                    Date =  DateTime.Now,
                    Liked = true,
                    PostId = postId,
                    UserId = user.Id
                };
                db.Like.Add(entity);
            }
            else
            {
                entity.Liked = !entity.Liked;
            }
            db.SaveChangesAsync();
            return GR(entity.Liked);

        }

        [Route("api/posts/comment")]
        public JsoniResult CommentPost(int postId,string text,int? replyto = null)
        {
            var entity = new Comment()
            {
                 Date = DateTime.Now,
                 PostId =  postId,
                 ReplyTo =  replyto,
                 Text = text,
                 UserId = user.Id
            };
            db.Comment.Add(entity);
            db.SaveChangesAsync();
            return GR(true);
        }
        [Route("api/posts/deletecomment")]
        public JsoniResult DeleteComment(int commentId)
        {
            if (db.Comment.Any(q => q.ReplyTo == commentId))
            {
                return GR(false,preferedMessage:"این کامنت دارای پاسخ می باشد و قادر به حذف آن نیستید");
            }
            var entity = db.Comment.FirstOrDefault(q => q.Id == commentId && q.UserId == user.Id);
            if (entity == null)
            {
                return GR(false, status: HttpStatusCode.NotFound);
            }
            else
            {
                db.Comment.Remove(entity);
                db.SaveChangesAsync();
                return GR(true);
            }
            
        }


        [Route("api/posts/savenew")]
        public JsoniResult NewPost(string bookTitle, string bookAuthor, int status,string description,byte score)
        {
            var bookEntity = db.Book.FirstOrDefault(q => q.Title == bookTitle && q.Author == bookAuthor);
            if (bookEntity == null)
            {
                bookEntity = new Book()
                {
                    Author = bookAuthor,
                    Title =  bookTitle
                };
                db.Book.Add(bookEntity);
            }

            var postEntity  = new Post()
            {
                Book = bookEntity,
                Description = description,
                PublishDate = DateTime.Now,
                Score = score,
                Status = status,
                UserId =  user.Id
            };

            db.Post.Add(postEntity);
            db.SaveChangesAsync();

            return GR(true);
        }


    }
}