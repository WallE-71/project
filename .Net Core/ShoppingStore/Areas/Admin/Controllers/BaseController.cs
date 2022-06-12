using System;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Attributes;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [Area(AreaConstants.adminArea)]
    public class BaseController : Controller
    {
        public const string ErrorMessage = "خطایی رخ داده.";
        public const string OperationSuccess = "عملیات با موفقیت انجام شد.";
        public const string DeleteSuccess = "حذف اطلاعات با موفقیت انجام شد.";
        public const string InsertSuccess = "درج اطلاعات با موفقیت انجام شد.";
        public const string EditSuccess = "ویرایش اطلاعات با موفقیت انجام شد.";
        public const string DeleteGroupSuccess = "حذف گروهی اطلاعات با موفقیت انجام شد.";

        public IActionResult Notification()
        {
            return Content(TempData["notification"].ToString());
        }

        [HttpGet, AjaxOnly]
        public IActionResult DeleteGroup()
        {
            return PartialView("_DeleteGroup");
        }

        [NonAction]
        public bool CheckOffline()
        {
            try
            {
                var ping = new Ping();
                var callingUrl = Request.GetTypedHeaders().Referer;
                //var result = ping.Send(callingUrl.Host, 120, Encoding.ASCII.GetBytes("samplestring"));
                var result = ping.Send("www.google.com");
                if (result.Status == IPStatus.Success)
                    return false;
            }
            catch (Exception)
            {
                return true;
            }
            return true;
        }
    }
}
