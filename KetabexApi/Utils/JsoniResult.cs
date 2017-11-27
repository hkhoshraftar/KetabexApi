using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KetabexApi.Utils
{
    public class JsoniResult : ContentResult
    {
        private string _content;
        private HttpStatusCode _statusCode;
        private string _statusDescription;

        public JsoniResult(object data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var serializer = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _content = JsonConvert.SerializeObject(data, serializer);
            _statusCode = statusCode;
            _statusDescription = message;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentEncoding = Encoding.UTF8;
         
            response.StatusCode = (int)_statusCode;
            if (_statusDescription != null)
            {
                response.StatusDescription = _statusDescription;
            }
            if (_content != null)
            {
                response.ContentType = "application/json";
               response.Write(_content);

            }
        }
    }
}