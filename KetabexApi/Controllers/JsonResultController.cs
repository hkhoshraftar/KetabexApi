using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KetabexApi.Utils;

namespace KetabexApi.Controllers
{
    public class JsonResultController : Controller
    {
        // GET: JsonResult

        public JsoniResult GR(object result, string message = "",string preferedMessage = "", HttpStatusCode status = HttpStatusCode.OK)
        {
            var data = new
            {
                status,
                message,
                preferedMessage ,
                date = (long)Util.DateTimeToUnixTimestamp(DateTime.Now),
                result
            };
            return new JsoniResult(data, message,status);
        }
    }
}