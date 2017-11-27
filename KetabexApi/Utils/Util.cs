using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MD.PersianDateTime;

namespace KetabexApi.Utils
{
    public class Util
    {
        public static JsoniResult GR(object result, Controller context, string message = "", HttpStatusCode  status =  HttpStatusCode.OK)
        {
            var data = new
            {
                status,
                message,
                date = (long)DateTimeToUnixTimestamp(DateTime.Now),
                result
            };
            return new JsoniResult(data, message,status);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local)).TotalSeconds;
        }

        public static void SendSms(string to, string message)
        {
            Task.Run(() =>
            {
   try
            {
                Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi("5A304857524D2F69444773616C764E706476346A65413D3D");
                api.Send("10000055005505", to, message);
            }
            catch (Kavenegar.Exceptions.ApiException ex)
            {
                Console.Write("Message : " + ex.Message);
            }
            catch (Kavenegar.Exceptions.HttpException ex)
            {
                Console.Write("Message : " + ex.Message);
            }
            });
         
        }
    }
}