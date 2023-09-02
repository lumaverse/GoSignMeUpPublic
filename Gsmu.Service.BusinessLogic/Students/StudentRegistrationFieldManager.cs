using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.User;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.Interface.Students;
using Gsmu.Service.Models.Constants;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Gsmu.Service.BusinessLogic.Students
{
    public class StudentRegistrationFieldManager : IStudentRegistrationField
    {
        private ISchoolEntities _db; // no need to use using(db) - the ISchoolEntities already inherited IDisposable
        private string connString = ConfigParserManager.ConnectionStringReader(ConfigParserManager.ConfigSourcePath(), ConfigSettingConstant.schoolEntitiesKey);

        public StudentRegistrationFieldManager()
        {
            _db = new SchoolEntities(connString);
        }

        public List<StudentRegistrationActiveWidgetsModel> GetStudentRegistrationWidgets()
        {
            List<StudentRegistrationActiveWidgetsModel> StudentRegistrationActiveWidgetsModels = new List<StudentRegistrationActiveWidgetsModel>();
            StudentRegistrationActiveWidgetsModel StudentRegistrationActiveWidgetsModel = new StudentRegistrationActiveWidgetsModel();

            var masterInfo4 = _db.masterinfo4.FirstOrDefault();
            string StudentsDashAddnew ="[]";
            StudentsDashAddnew = masterInfo4.StudentsDashAddnew;

            UserWidget userwidgets = Json.Decode<UserWidget>(StudentsDashAddnew);
            foreach (var item in userwidgets.Widgets)
            {
                StudentRegistrationActiveWidgetsModel.Title = item.Title;
                StudentRegistrationActiveWidgetsModel.WidgetId = item.ID;
                StudentRegistrationActiveWidgetsModel.WidgetSortOrder = item.DispSort;
                StudentRegistrationActiveWidgetsModel.StudentRegistrationActiveFieldsModel = new List<StudentRegistrationActiveFieldsModel>();
                StudentRegistrationActiveWidgetsModel.StudentRegistrationActiveFieldsModel = GetStudentRegistrationFields(item.ID,userwidgets.WidgetItems);


                StudentRegistrationActiveWidgetsModels.Add(StudentRegistrationActiveWidgetsModel);
                StudentRegistrationActiveWidgetsModel = new StudentRegistrationActiveWidgetsModel();
                
            }
            return StudentRegistrationActiveWidgetsModels;
        }

        public List<StudentRegistrationActiveFieldsModel> GetStudentRegistrationFields(int widgetId, List<WidgetItemList> FieldList)
        {
            List<StudentRegistrationActiveFieldsModel> StudentRegistrationActiveFieldsModels = new List<StudentRegistrationActiveFieldsModel>();
            StudentRegistrationActiveFieldsModel StudentRegistrationActiveFieldsModel = new StudentRegistrationActiveFieldsModel();
            foreach (var field in FieldList.Where(field => field.WidgetID == widgetId))
            {
                FieldSpec FieldSpec = new FieldSpec();
                FieldMask FieldMask = new FieldMask();
                StudentRegistrationActiveFieldsModel.FieldName = field.FieldName;
                StudentRegistrationActiveFieldsModel.FieldId = field.ID;
                StudentRegistrationActiveFieldsModel.FieldDisplaySortOrder = field.DispSort;
                FieldSpec = _db.FieldSpecs.Where(f => f.FieldName == field.FieldName && f.TableName == "Students").FirstOrDefault();
                FieldMask = _db.FieldMasks.Where(f => f.FieldName == field.FieldName && f.TableName == "Students").FirstOrDefault();
                if (FieldMask != null)
                {
                    StudentRegistrationActiveFieldsModel.FieldMask = FieldMask.DefaultMaskNumber;
                }
                if (FieldSpec != null)
                {
                    StudentRegistrationActiveFieldsModel.IsRequired = FieldSpec.FieldRequired;
                    StudentRegistrationActiveFieldsModel.IsReadOnly = FieldSpec.FieldReadOnly;
                    StudentRegistrationActiveFieldsModel.IsRequiredConfirmation = FieldSpec.ConfirmRequired;
                    StudentRegistrationActiveFieldsModel.FieldLabel = FieldSpec.FieldLabel;

                }

                if (field.FieldName.ToLower() == "district" || field.FieldName.ToLower() == "school" || field.FieldName.ToLower() == "grade" || field.FieldName.ToLower() == "country" || StudentRegistrationActiveFieldsModel.FieldMask ==21 || StudentRegistrationActiveFieldsModel.FieldMask==24 || StudentRegistrationActiveFieldsModel.FieldMask==20 || StudentRegistrationActiveFieldsModel.FieldMask==23)
                {
                    StudentRegistrationActiveFieldsModel.FieldType = "dropdown";

                }
                else if (StudentRegistrationActiveFieldsModel.FieldMask == 25 || StudentRegistrationActiveFieldsModel.FieldMask == 26)
                {
                    StudentRegistrationActiveFieldsModel.FieldType = "checkbox";
                }
                else
                {
                    StudentRegistrationActiveFieldsModel.FieldType = "textbox";
                }
                StudentRegistrationActiveFieldsModels.Add(StudentRegistrationActiveFieldsModel);
                StudentRegistrationActiveFieldsModel = new StudentRegistrationActiveFieldsModel();
            }
            return StudentRegistrationActiveFieldsModels;
        }
    }
}
