using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Students
{
    public class StudentRegistrationActiveFieldsModel
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public string FieldType { get; set; }
        public int FieldMask { get; set; }
        public int FieldDisplaySortOrder { get; set; }
        public int IsRequired { get; set; }
        public int IsReadOnly { get; set; }
        public int? IsRequiredConfirmation { get; set; }
    }

    public class StudentRegistrationActiveWidgetsModel
    {
        public int WidgetId { get; set; }
        public string Title { get; set; }
        public int WidgetSortOrder { get; set; }
        public List<StudentRegistrationActiveFieldsModel> StudentRegistrationActiveFieldsModel = new List<StudentRegistrationActiveFieldsModel>();
    }
}
