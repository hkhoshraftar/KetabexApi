using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using KetabexApi.Controllers;
using KetabexApi.DataModel;
using KetabexApi.Utils;

namespace KetabexApi.Areas.api.Controllers
{
    public class AuthController : AuthorizedController
    {
        [Route("api/user/getauthcode")]
        public JsoniResult GetAuthCode(string phoneNumber)
        {
            //TODO:REMOVE
            Thread.Sleep(2000);

            if (phoneNumber.Length != 11)
            {
                return GR(null,
                    preferedMessage: "شماره تلفن صحیح نمی باشد",
                    status: HttpStatusCode.NotAcceptable);
            }

            var userEntity = db.User.FirstOrDefault(q => q.PhoneNumber == phoneNumber);
            if (userEntity == null)
            {
                userEntity = db.User.First(q => q.Id == user.Id);
                userEntity.PhoneNumber = phoneNumber;
            }

            var authCode = new UserAuth()
            {
                UserId = userEntity.Id,
                Code = new Random().Next(1000, 9999).ToString(),
                GenerateDate = DateTime.Now,
                Resend = 0,
                Inactive = false
            };

            db.UserAuth.Add(authCode);
            db.SaveChangesAsync();

            Util.SendSms(phoneNumber, "کد احراز شما عبارتست از : " + Environment.NewLine + authCode.Code);

            return GR(true);
        }

        [Route("api/user/chekcauthcode")]
        public JsoniResult CheckAuthCode(string phoneNumber, string code)
            {
            //TODO:REMOVE
            Thread.Sleep(2000);

            var entity = user.UserAuth.FirstOrDefault(q =>
                q.Code == code && q.User.PhoneNumber == phoneNumber);

            if (entity == null)
            {
                return GR(false);
            }

            entity.Inactive = true;
            db.SaveChangesAsync();
            return GR(true);
        }

        [Route("api/user/signup")]
        public JsoniResult Signup(string username, string nickname)
        {
            //TODO:REMOVE
            Thread.Sleep(2000);

            if (db.User.Any(q => q.Username == username))
            {
                return GR(false,preferedMessage:"این نام کاربری وجود دارد",status:HttpStatusCode.NotAcceptable);
            }
            var entity = db.User.First(q => q.Id == user.Id);
            entity.Username = username;
            entity.Nickname = nickname;
            db.SaveChangesAsync();
            return GR(true);
        }


    }
}