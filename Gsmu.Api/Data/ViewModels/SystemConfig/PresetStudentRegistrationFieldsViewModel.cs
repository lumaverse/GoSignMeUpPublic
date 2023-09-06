using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gsmu.Api.Data.ViewModels.SystemConfig
{
    public class PresetStudentRegistrationFieldsViewModel
    {
        public PresetStudentRegistrationFieldsViewModel()
        {
        }

        public bool ? Visible { get; set; }
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public bool CanUpdateFieldLabel { get; set; }
        public bool ? Required { get; set; }
        public int ? SortOrder { get; set; }
        public string Mask { get; set; }
        public bool ? ReadOnly { get; set; }
        public bool ? ConfirmRequired { get; set; }
    }

}
