using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Public.Controllers
{
    public class CertificateController:Controller
    {
        public ActionResult GetCertificateRequirements(int CertifiacteId)
        {
            var result = new JsonResult();
            Gsmu.Api.Data.School.Certificate.UserCertificate certifactefunction = new Api.Data.School.Certificate.UserCertificate();
            result.Data = certifactefunction.StudentCertifiacteDetails(CertifiacteId);
            return result;
        }
    }
}