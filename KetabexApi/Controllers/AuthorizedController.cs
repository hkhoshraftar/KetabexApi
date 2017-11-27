using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using KetabexApi.DataModel;

namespace KetabexApi.Controllers
{
    public class AuthorizedController : BaseApiController
    {
        protected User user;
        protected string apiToken;

        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

           var uid = filterContext.RequestContext.HttpContext.Request.Unvalidated.Headers["uid"];
            apiToken = filterContext.RequestContext.HttpContext.Request.Unvalidated.Headers["token"];
            
            if (uid == null || apiToken == null || uid?.Length < 5 || apiToken?.Length < 20)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotAcceptable,"token or uid not found");
                return;
            }
            else
            {
                var uidMd5 = Encoding.ASCII.GetString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(uid)));
                var keyEntity = db.UserKey.FirstOrDefault(q => q.DeviceId == uidMd5 && q.ApiToken == apiToken && q.Isvalid.Value);
                if (keyEntity == null)
                {
                    filterContext.Result = new HttpNotFoundResult("key not found, please exchange");
                    return;
                }
                else
                {
                    user = keyEntity.User;
                    keyEntity.ExpirationDate = DateTime.Now.AddMonths(6);
                    db.SaveChangesAsync();
                }
            }
          
            db = new KetabexDbEntities();
            base.OnActionExecuting(filterContext);

        }
    }
}