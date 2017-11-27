using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KetabexApi.DataModel;

namespace KetabexApi.Controllers
{
    public class BaseApiController : JsonResultController
    {
        protected KetabexDbEntities db = new KetabexDbEntities();
        
    }
}