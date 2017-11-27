using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using KetabexApi.Controllers;
using KetabexApi.DataModel;
using KetabexApi.Utils;

namespace KetabexApi.Areas.api.Controllers
{
    public class ExchangeController : BaseApiController
    {

        [Route("api/user/exchange")]
        public JsoniResult Exchange(string uid)
        {
            //TODO:REMOVE
            Thread.Sleep(2000);

            if (uid.Length < 5)
                return new JsoniResult(null,statusCode:HttpStatusCode.NotAcceptable);

            var uidMd5 = Encoding.ASCII.GetString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(uid)));
            var entity = db.UserKey.FirstOrDefault(q => q.DeviceId == uidMd5 && q.Isvalid.Value);
            if (entity == null)
            {
                var userEntity = new User()
                {
                    Inactive = false
                };

                db.User.Add(userEntity);

                entity = new UserKey()
                {
                    User = userEntity,
                    CreationDate = DateTime.Now,
                    DeviceId = uidMd5,
                    ExpirationDate = DateTime.Now.AddMonths(6),
                    ApiToken = Guid.NewGuid().ToString()
                };

                db.UserKey.Add(entity);
                db.SaveChangesAsync();
            }
            return GR(new
            {
                isSignedUp = entity?.User?.Issigned?? false,
                token = entity.ApiToken
            });
        }

        [Route("api/connect")]
        public JsoniResult Connect()
        {
            return GR(true);
        }
    }
}