using Gsmu.Service.Models.Admin.CourseDashboard;
using Gsmu.Service.Models.Admin.Global;
using Gsmu.Service.Models.Generic;
using Gsmu.Service.Models.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Admin.Global
{
    public interface IMasterSettingsManager
    {
        MasterSettingsModel GetMasterSettings();
        List<AudienceModel> GetAudiences();
        List<IconsModel> GetIcons();
        List<DeparmentModel> GetDepartments();
        List<CourseGroupingModel> GetCourseColorGrouping();
        List<CustomCertificateModel> GetCustomCertificateModel();
        List<PricingOptionsModel> GetAllPricingOptions();
        List<DropdownModel> GetMainCategories();
        List<DropdownModel> GetSubCategories();
        List<DropdownModel> GetSubSubCategories();
        List<CourseChoices> GetCourseChoices();
        List<DropdownModel> GetSurveys();
        List<DropdownModel> GetLocations();
        List<DropdownModel> GetRooms();
        List<DropdownModel> GetRoomDirections();
        List<DropdownModel> GetCountries();
        List<DropdownModel> GetInstructors();
        List<DropdownModel> GetMaterials();
    }
}
