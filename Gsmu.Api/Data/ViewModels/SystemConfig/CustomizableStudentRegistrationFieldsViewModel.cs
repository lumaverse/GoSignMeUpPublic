using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.SystemConfig
{
    public class CustomizableStudentRegistrationFieldsViewModel
    {
        public CustomizableStudentRegistrationFieldsViewModel()
        {
        }

        public string DBField { get; set; }
        public string FieldLabel { get; set; }
        public bool ? Required { get; set; }
        public int ? SortOrder { get; set; }
        public string Mask { get; set; }
        public bool ? ReadOnly { get; set; }
        public bool ? ShowInMultipleEnrollment { get; set; }
    }
}
