using Gsmu.Service.Models.Admin.CourseDashboard;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.Generic;
using Gsmu.Service.Models.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface.Admin.Dashboard
{
    public interface ICourseDashboardManager
    {
        CourseConfigurationModel GetCourseConfigurationById(int courseId);
        CourseConfigurationModel GetCourseConfiguration();
        List<CourseBasicDetails> GetCourseByFilter();
        List<CourseBasicDetails> GetCourseByFilter(string filter);
        CourseDescriptionModel GetCourseDescriptionById(int courseId);
        List<CourseExpensesModel> GetCourseExpensesById(int courseId);
        CoursePricingMainModel GetCoursePricingById(int courseId);
        CourseRostersMainModel GetCourseRostersById(int courseId);
        List<CourseSurveyModel> GetCourseSurveyById(int courseId);
        List<CourseSurveyResultModel> GetCourseSurveyResultById(int courseId);
        List<Models.Courses.CourseCategoriesModel> GetCourseCategoriesById(int courseId);
        List<CourseInstructorsModel> GetCourseInstructorsById(int courseId);
        List<CourseMaterialsModel> GetCourseMaterialsById(int courseId);
        CourseRosterExtraModel GetCourseRosterExtraById(int courseId);
        List<Models.Courses.CourseDateTimeModel> GetCourseDateAndTimesById(int courseId);
        List<CourseTransciptsModel> GetCourseTransciptsById(int courseId);
        List<CourseChoices> GetCourseChoicesById(int courseId);
        InstructorModel GetInstructorById(int instructorId);
        Models.Courses.CourseModel SaveCourse(CourseDescriptionModel model);
        Models.Courses.CourseDateTimeModel SaveDateTime(Models.Courses.CourseDateTimeModel model);
        void SaveCourseImage();
        void DeleteCourseImage();
        CourseMaterialsModel SaveCourseMaterial(int courseId, int materialId);
        void SaveCourseChoice(int courseId, int choiceId);
        void DeleteCourseChoice(int courseId, int choiceId);
        void SaveCategories(Models.Courses.CourseCategoriesModel categoriesModel);
        void SaveCoursePrice(CoursePricingMainModel priceModel);
        List<FileModel> GetDocumentFilesById(int courseId);
        void SaveFile();
        void DeleteCourseDateWhenOnlineById(int courseId);
        void SaveInstructorBio(int instructorId, string bio);
    }
}
