using Gsmu.Api.Data.School.Course;
using Gsmu.Service.Models.Admin.Portal;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.GlobalTools.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Courses
{
    public interface ICourseGrid
    {
        List<CourseMainCategoryModel> GetCourseCategoryTree(Gsmu.Service.Models.Enum.CourseActiveState  state = Gsmu.Service.Models.Enum.CourseActiveState.All);
        List<CourseGridDetailResultModel> GetCourseList(QueryStateConfig queryState, string text = null, string mainCategory = null, string subCategory = null, string subsubcattext = null, bool subCategoryIsSubSub = false, DateTime? from = null, DateTime? until = null);
        ListAdminCanvasSectionCourses GetAdminCanvasSectionCourses(int canvasId);

        void DeactivateStudents(int studentid);
        void ReactivateStudents(int studentid);
    }
}
