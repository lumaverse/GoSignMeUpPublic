using Gsmu.Api.Data.School.User;
using Gsmu.Service.Models.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Students
{
    public interface IStudentRegistrationField
    {
        List<StudentRegistrationActiveWidgetsModel> GetStudentRegistrationWidgets();
        List<StudentRegistrationActiveFieldsModel> GetStudentRegistrationFields(int widgetId, List<WidgetItemList> FieldList);
    }
}
