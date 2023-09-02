using Gsmu.Api.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class AttendanceTakingController : Controller
    {
        public ActionResult TakeAttendance(string username, string adminusersessionid, int courseId = 0)
        {
            try
            {
                AuthorizationHelper.ConfigurePortalAdministratorLogin(username, adminusersessionid);
                if (AuthorizationHelper.CurrentAdminUser != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("~/Landing/ErrorPage/");
                }
            }
            catch
            {

                return Redirect("~/Landing/ErrorPage/");
            }
        }
    }
}