using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.SystemConfig
{
    public class StudentRegistrationFieldsViewModel
    {
        public PresetStudentRegistrationFieldsViewModel ProfileStudRegFieldsViewModel {get; set;}
        public CustomizableStudentRegistrationFieldsViewModel CustomtudRegFieldsViewModel { get; set; }

        public List<PresetStudentRegistrationFieldsViewModel> ProfileStudRegFieldsListViewModel { get; set; }
        public List<CustomizableStudentRegistrationFieldsViewModel> CustomtudRegFieldsListViewModel { get; set; }
    }
}
