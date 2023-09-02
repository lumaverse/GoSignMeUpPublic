using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Data;
using System.Web.Helpers;

using student = Gsmu.Api.Data.School.Student;
using web = Gsmu.Api.Web;
using json = Newtonsoft.Json;
using models = Gsmu.Api.Data.ViewModels.UserFields;
using school = Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.User;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class UserSettingsController : Controller
    {
        //
        // GET: /Admin/UserSettings/
        public ActionResult Index()
        {
            return View();
        }


        public EmailRestriction GetEmailRestrictions()
        {
             var json = Settings.Instance.GetMasterInfo().RequiredDomain;
            //string json = Settings.Instance.GetMasterInfo().EmailCancelBody;

            EmailRestriction jsonObj = null;
            try
            {
                jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailRestriction>(json);
                var i = 0;
                foreach (var item in jsonObj.Data)
                {
                    i += 1;
                    item.id = i;
                }
                jsonObj.Count = i;
            }
            catch
            {
                jsonObj = new EmailRestriction() { Count = 0, OnOff = 0, EmailNotification = "This email is not listed in the allowed email postfix.", Data = null };
            }


            return jsonObj;
        }

        public string getEmailRestrictionStr()
        {
            EmailRestriction jsonObj = GetEmailRestrictions();
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            return output;
        }

        public ActionResult setEmailRestriction(EmailRestriction er)
        {
            EmailRestriction jsonObj = GetEmailRestrictions();
            jsonObj.OnOff = er.OnOff;
            jsonObj.ShowList = er.ShowList;
            jsonObj.EmailNotification = er.EmailNotification;
            return Content(SaveEmailRestriction(jsonObj), "application/json");
        }

        public ActionResult getEmailRestrictionParam()
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            EmailRestriction jsonObj = GetEmailRestrictions();

            result.Data = new
            {
                success = true,
                OnOff = jsonObj.OnOff,
                EmailNotification = jsonObj.EmailNotification,
                ShowList = jsonObj.ShowList
            };
            return result;
        }

        public string SaveEmailRestriction(EmailRestriction jsonObj)
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            Gsmu.Api.Data.Settings.Instance.SetMasterinfoValue(0, "RequiredDomain", output);
            //Gsmu.Api.Data.Settings.Instance.SetMasterinfoValue(0, "EmailCancelBody", output);
            return output;
        }
        /* *** move off to Public User controller
        public ActionResult CheckEmailRestriction(string domain)
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            EmailRestriction jsonObj = GetEmailRestrictions();

            var Whitelist = new List<EmailRestrictionData>();
            var Blacklist = new List<EmailRestrictionData>();

            var validemail = false;

            Whitelist = (from n in jsonObj.Data
                         where n.grp.Contains("whitelist")
                         select n).ToList();

            Blacklist = (from n in jsonObj.Data
                         where n.grp.Contains("blacklist")
                         select n).ToList();

            int CntWhitelist = Whitelist.Count();

            int CntBlacklist = Blacklist.Count();

            int CntValidWhitelist = (from n in Whitelist
                                     where n.email.Contains(domain)
                                     select n).Count();

            int CntInvalidBlacklist = (from n in Blacklist
                                     where n.email.Contains(domain)
                                     select n).Count();

            validemail = false;
            
            if (CntWhitelist == 0 && CntBlacklist == 0)
            {
                validemail = true;
            }

            if (CntWhitelist > 0 && CntBlacklist == 0 && CntValidWhitelist > 0)
            {
                validemail = true;
            }

            if (CntWhitelist == 0 && CntBlacklist > 0 && CntInvalidBlacklist == 0)
            {
                validemail = true;
            }

            if (CntWhitelist > 0 && CntBlacklist > 0 && CntValidWhitelist > 0 && CntInvalidBlacklist == 0)
            {
                validemail = true;
            }

            result.Data = new
            {
                success = true,
                OnOff = jsonObj.OnOff,
                valid = validemail,
                EmailNotification = jsonObj.EmailNotification,
                Whitelist = Whitelist,
                Blacklist = Blacklist,
                CntWhitelist = CntWhitelist,
                CntBlacklist = CntBlacklist,
                CntValidWhitelist = CntValidWhitelist,
                CntInvalidBlacklist = CntInvalidBlacklist
            };
            return result;
        }
        */

        public ActionResult ReadEmailRestrictions()
        {
            EmailRestriction jsonObj = GetEmailRestrictions();
            return Content(SaveEmailRestriction(jsonObj), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateEmailRestrictions(int id, string newemail)
        {

            EmailRestriction jsonObj = GetEmailRestrictions();

            foreach (var item in jsonObj.Data)
            {
                if (item.id.ToString() == id.ToString())
                {
                    item.email = newemail;
                }
            }
            return Content(SaveEmailRestriction(jsonObj), "application/json");
        }

        public ActionResult CreateEmailRestrictions(string grpvlu, string vlu)
        {
            EmailRestriction jsonObj = GetEmailRestrictions();
            jsonObj.Data.Add(new EmailRestrictionData() { id = jsonObj.Data.Count + 1, email = vlu, grp = grpvlu });
            return Content(SaveEmailRestriction(jsonObj), "application/json");
        }

        [HttpPost]
        public ActionResult DestroyEmailRestrictions(int id)
        {
            var i = 0;
            EmailRestriction jsonObj = GetEmailRestrictions();

            jsonObj.Data.RemoveAll(x => x.id == id);
            jsonObj.Count = jsonObj.Count - 1;

            return Content(SaveEmailRestriction(jsonObj), "application/json");
        }

    }


}