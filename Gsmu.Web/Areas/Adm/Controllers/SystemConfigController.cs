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
using models = Gsmu.Api.Data.ViewModels;
using school = Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.User;

using Gsmu.Api.Data.ViewModels.SystemConfig;
using Gsmu.Api.Data.School.Entities;

using Gsmu.Api.BLL;
using Gsmu.Api.BLL.SystemConfig;

using Newtonsoft.Json;
namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class SystemConfigController : Controller
    {
        PublicOptionsManager publicOptionsManager;
        public ActionResult Index() 
        {
            return View();
        }
        public ActionResult PublicOptions()
        {
            string extJSMode = Request.QueryString["extjs"];
            ViewBag.ExtJSMode = extJSMode;
            publicOptionsManager = new PublicOptionsManager();
            return View(publicOptionsManager.GetPublicOptions());
        }
        public ActionResult SavePublicOptions(PublicOptionsViewModel publicOptionsModel) 
        {
            var JsonResult = new JsonResult();
            try {
                publicOptionsManager = new PublicOptionsManager();
                publicOptionsManager.SavePublicOptionsToDB(publicOptionsModel);
                JsonResult.Data = new
                {
                    success = true,
                    error = string.Empty
                };
                return JsonResult;
            }catch(Exception e)
            {
                JsonResult.Data = new
                {
                    success = false,
                    error = e.Message
                };
                return JsonResult;
            }
            
        }

        public ActionResult StudentRegistration() 
        {
            string FieldName1 = Settings.Instance.GetMasterInfo().Field1Name; //district
            string FieldName2 = Settings.Instance.GetMasterInfo().Field2Name; //school
            string FieldName3 = Settings.Instance.GetMasterInfo().Field3Name; //grade
            ViewBag.ShibbolethEnabled = Settings.Instance.GetMasterInfo4().shibboleth_sso_enabled;
            ViewBag.FielName1 = FieldName1;
            ViewBag.FielName2 = FieldName2;
            ViewBag.FielName3 = FieldName3;

            using (var db = new SchoolEntities()) 
            {
                var masterinfo = db.MasterInfoes.FirstOrDefault();
                StudentRegistrationFieldsViewModel studentRegistrationField = new StudentRegistrationFieldsViewModel();
                List<PresetStudentRegistrationFieldsViewModel> presetStudentRegistrationFields = new List<PresetStudentRegistrationFieldsViewModel>();
                List<CustomizableStudentRegistrationFieldsViewModel> customStudentRegistrationFields = new List<CustomizableStudentRegistrationFieldsViewModel>();

                var fieldSpecs = db.FieldSpecs.Where(fs => fs.TableName == "Students" && !fs.FieldName.ToLower().Contains("studregfield")).ToList();
                foreach(var fieldSpec in fieldSpecs)
                {
                    presetStudentRegistrationFields.Add(new PresetStudentRegistrationFieldsViewModel() { 
                        Visible = true,
                        CanUpdateFieldLabel = true,
                        FieldName = fieldSpec.FieldName,
                        FieldLabel = fieldSpec.FieldLabel,
                        SortOrder = SortOrder (db, fieldSpec.FieldName),
                        Mask = Mask(db, fieldSpec.FieldName)
                    });
                }
                presetStudentRegistrationFields = presetStudentRegistrationFields.OrderBy(p => p.SortOrder).ToList();

                presetStudentRegistrationFields.Insert(0, new PresetStudentRegistrationFieldsViewModel()
                {
                    Visible = masterinfo.VisibleStudentDISTRICT != 0 ? true : false,
                    FieldName = "district",
                    CanUpdateFieldLabel = false,
                    FieldLabel = masterinfo.Field1Name,
                    Required = masterinfo.ReqStudentDISTRICT != 0 ? true : false,
                    SortOrder = db.StudentRegSortOrders.Where(s => s.FieldName.ToLower().Equals("district")).FirstOrDefault().SortOrder,
                    Mask = "N / A"
                });
                presetStudentRegistrationFields.Insert(1, new PresetStudentRegistrationFieldsViewModel()
                {
                    Visible = masterinfo.VisibleStudentSCHOOL != 0 ? true : false,
                    FieldName = "school",
                    CanUpdateFieldLabel = false,
                    FieldLabel = masterinfo.Field2Name,
                    Required = masterinfo.ReqStudentSCHOOL != 0 ? true : false,
                    SortOrder = db.StudentRegSortOrders.Where(s => s.FieldName.ToLower().Equals("school")).FirstOrDefault().SortOrder,
                    Mask = "N / A"
                });
                presetStudentRegistrationFields.Insert(2,new PresetStudentRegistrationFieldsViewModel()
                {
                    Visible = masterinfo.VisibleStudentGRADE != 0 ? true : false,
                    FieldName = "grade",
                    CanUpdateFieldLabel = false,
                    FieldLabel = masterinfo.Field3Name,
                    Required = masterinfo.ReqStudentGRADE != 0 ? true : false,
                    SortOrder = db.StudentRegSortOrders.Where(s => s.FieldName.ToLower().Equals("grade")).FirstOrDefault().SortOrder,
                    Mask = "N / A"
                });

                studentRegistrationField.ProfileStudRegFieldsListViewModel = presetStudentRegistrationFields;

                for (int index = 1; index <= 5;  index++)
                {
                    customStudentRegistrationFields.Add(new CustomizableStudentRegistrationFieldsViewModel()
                    {
                        DBField = "StudRegField" + index + "Name",
                        FieldLabel = "StudRegField" + index + "Name",
                        Required = false,
                        SortOrder = SortOrder(db, "StudRegField" + index + "Name"),
                        Mask = Mask(db, "StudRegField" + index + "Name"),
                        ReadOnly = false,
                        ShowInMultipleEnrollment = false
                    });
                }

                studentRegistrationField.CustomtudRegFieldsListViewModel = customStudentRegistrationFields;
                return View(studentRegistrationField);
            };

            
        }

        private string Mask(SchoolEntities context, string fieldName) 
        {
            var mask = context.FieldMasks.Where(s => s.FieldName.ToLower().Equals(fieldName.ToLower())).FirstOrDefault();
            return mask == null ? "N / A" : mask.Mask1;
        }

        private int SortOrder(SchoolEntities context ,string fieldName) 
        {
            var sortOrder = context.StudentRegSortOrders.Where(s => s.FieldName.ToLower().Equals(fieldName.ToLower())).FirstOrDefault();
            return sortOrder == null ? 0 : sortOrder.SortOrder;
        }
    }
}