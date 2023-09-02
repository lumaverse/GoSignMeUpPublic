using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.BusinessLogic.GlobalTools;
using Gsmu.Service.Interface.Courses;
using Gsmu.Service.Models.Admin.Portal;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.GlobalTools.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.Courses
{
    public class CourseGridManager: ICourseGrid
    {
        public List<CourseMainCategoryModel> GetCourseCategoryTree(Gsmu.Service.Models.Enum.CourseActiveState state = Gsmu.Service.Models.Enum.CourseActiveState.All)
        {
            List<CourseMainCategoryModel> CourseMainCategories = new List<CourseMainCategoryModel>();
            using (var db = new SchoolEntities())
            {
                CourseMainCategoryModel CourseMainCategoryModel = new CourseMainCategoryModel();
                CourseSubCategoryModel CourseSubCategoryModel = new CourseSubCategoryModel();
                CourseSubSubCategoryModel CourseSubSubCategoryModel = new Models.Courses.CourseSubSubCategoryModel();
                var courses = (from _courseList in db.CoursesListViews select _courseList);
                var now = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                courses = courses.Where(course => course.coursedate >= now);

                var AllMainCategories = (from maincategories in courses select new GenericNameIdModel { Name = maincategories.MainCategory, Id = maincategories.MainCategoryId }).Distinct();
                var AllSubCategories = (from maincategories in courses select new GenericNameIdModel { Name = "", Id = "" }).Distinct();
                var AllSubSubCategories = (from maincategories in courses select new GenericNameIdModel{ Name= "",Id=""}).Distinct();
                foreach(var mainCategory in AllMainCategories){
                    CourseMainCategoryModel.MainCategoryName = mainCategory.Name;
                    CourseMainCategoryModel.MainCategoryId =mainCategory.Id;
                    
                    CourseMainCategoryModel.SubCategories = new List<CourseSubCategoryModel>();
                    AllSubCategories = (from subcategories in courses where subcategories.MainCategory == mainCategory.Name select new GenericNameIdModel { Name = subcategories.SubCategory, Id = subcategories.SubCategoryId }).Distinct();
                    foreach (var subCategory in AllSubCategories)
                    {
                        CourseSubCategoryModel.SubCategoryName = subCategory.Name;

                        CourseSubCategoryModel.SubCategoryId =subCategory.Id;
                        CourseSubCategoryModel.SubSubCategories = new List<Models.Courses.CourseSubSubCategoryModel>();
                        AllSubSubCategories = (from subsubcategories in courses where subsubcategories.MainCategory == mainCategory.Name && subsubcategories.SubCategory == subCategory.Name select new GenericNameIdModel { Name = subsubcategories.SubSubCategory, Id = subsubcategories.SubSubCategoryId }).Distinct();
                        foreach (var subsubCategory in AllSubSubCategories)
                        {
                            CourseSubSubCategoryModel.SubCategoryName = subsubCategory.Name;

                            CourseSubSubCategoryModel.SubSubCategoryId = subsubCategory.Id;

                            if (subsubCategory.Name != null)
                            {
                                CourseSubCategoryModel.SubSubCategories.Add(CourseSubSubCategoryModel);
                            }
                            CourseSubSubCategoryModel = new CourseSubSubCategoryModel();
                        }
                        if (CourseSubCategoryModel.SubCategoryName != null)
                        {
                            CourseMainCategoryModel.SubCategories.Add(CourseSubCategoryModel);
                            CourseSubCategoryModel = new CourseSubCategoryModel();
                        }

                    }
                    if (CourseMainCategoryModel.MainCategoryName != null)
                    {
                        CourseMainCategories.Add(CourseMainCategoryModel);
                    }
                    CourseMainCategoryModel = new CourseMainCategoryModel();
                }
            }
            return CourseMainCategories;
        }

        public List<CourseGridDetailResultModel> GetCourseList(QueryStateConfig queryState, string text = null, string mainCategory = null, string subCategory = null, string subsubCategory = null, bool subCategoryIsSubSub = false, DateTime? from = null, DateTime? until = null)
        {
            List<CourseGridDetailResultModel> _CourseGridDetailResultModel = new List<CourseGridDetailResultModel>();
            using (var db = new SchoolEntities())
            {
                var courses = (from _courseList in db.CoursesListViews
                               select new Gsmu.Service.Models.Courses.CourseGridDetailResultModel
                               {
                                   CourseBasicDetails = new CourseBasicDetails
                                   {
                                       CourseId = _courseList.COURSEID,
                                       CourseName = _courseList.COURSENAME,
                                       CourseNumber = _courseList.COURSENUM,
                                       Description = _courseList.description
                                   },
                                   DateTime = new EventSessionCourseDateTimeModel
                                   {
                                       CourseDate = _courseList.coursedate,
                                       EndTime = _courseList.FINISHTIME,
                                       StartTime = _courseList.STARTTIME

                                   },
                                   LocationDetailsModel = new LocationDetailsModel
                                   {
                                       Location = _courseList.LOCATION,
                                       LocationURL = _courseList.LOCATIONURL
                                   },
                                   CoursePriceDetails = new CoursePriceDetails
                                   {
                                       price = (_courseList.NODISTPRICE == null) ? 0 : _courseList.NODISTPRICE.Value
                                   },
                                   AccessConfiguration = new AccessConfigurationModel
                                   {
                                       AccessCode = _courseList.courseinternalaccesscode
                                   },
                                   CourseSingleCategoryDetails = new CourseSingleCategoryDetails{
                                       MainCategoryId = _courseList.MainCategoryId,
                                       SubCategoryId = _courseList.SubCategoryId,
                                       SubSubCategoryId = _courseList.SubSubCategoryId
                                   }

                               });
                var now = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                courses = courses.Where(course => course.DateTime.CourseDate >= now);
                if (!string.IsNullOrEmpty(text))
                {
                    courses = courses.Where(course => course.CourseBasicDetails.CourseName.Contains(text)
                       || course.CourseBasicDetails.CourseNumber.Contains(text)
                       || course.CourseBasicDetails.Description.Contains(text)
                       || course.LocationDetailsModel.Location.Contains(text)
                       );

                }
                if(!string.IsNullOrEmpty(mainCategory)){
                    courses = courses.Where(course => course.CourseSingleCategoryDetails.MainCategoryId == mainCategory);
                }
                if (!string.IsNullOrEmpty(subCategory))
                {
                    courses = courses.Where(course => course.CourseSingleCategoryDetails.SubCategoryId == subCategory);
                }
                if (!string.IsNullOrEmpty(subsubCategory))
                {
                    courses = courses.Where(course => course.CourseSingleCategoryDetails.SubSubCategoryId == subsubCategory);
                }

                if (queryState.PageSize == 0)
                {
                    queryState.PageSize = Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().DefaultPaginationCourses.HasValue ? Gsmu.Api.Data.Settings.Instance.GetMasterInfo3().DefaultPaginationCourses.Value : 12;
                }
                var page = queryState.Page;
                var start = (queryState.Page - 1) * queryState.PageSize;
                var limit = queryState.PageSize;
                var pagedcourslist = courses.OrderBy(course=> course.CourseBasicDetails.CourseName).Skip((page - 1) * limit).Take(limit);
                CourseGridDetailResultModel CourseGridDetailResultModel = new CourseGridDetailResultModel();
                foreach (var course in pagedcourslist.ToList())
                {
                    if (course != null)
                    {
                            CourseGridDetailResultModel.CourseBasicDetails = new CourseBasicDetails();
                            CourseGridDetailResultModel.CourseBasicDetails.CourseId = course.CourseBasicDetails.CourseId;
                            CourseGridDetailResultModel.CourseBasicDetails.CourseName = course.CourseBasicDetails.CourseName;
                            CourseGridDetailResultModel.CourseBasicDetails.CourseNumber = course.CourseBasicDetails.CourseNumber;
                            CourseGridDetailResultModel.DateTime = new EventSessionCourseDateTimeModel();
                            CourseGridDetailResultModel.DateTime.CourseDate = course.DateTime.CourseDate;
                            CourseGridDetailResultModel.DateTime.StartTime = course.DateTime.StartTime;
                            CourseGridDetailResultModel.DateTime.EndTime = course.DateTime.EndTime;
                            CourseGridDetailResultModel.CoursePriceDetails = new CoursePriceDetails();
                            CourseGridDetailResultModel.CoursePriceDetails.price = course.CoursePriceDetails.price;
                            CourseGridDetailResultModel.LocationDetailsModel = new LocationDetailsModel();
                            CourseGridDetailResultModel.LocationDetailsModel.Location = course.LocationDetailsModel.Location;
                            CourseGridDetailResultModel.LocationDetailsModel.LocationURL = course.LocationDetailsModel.LocationURL;
                            CourseGridDetailResultModel.AccessConfiguration = new AccessConfigurationModel();
                            CourseGridDetailResultModel.AccessConfiguration.AccessCode = course.AccessConfiguration.AccessCode;
                            _CourseGridDetailResultModel.Add(CourseGridDetailResultModel);
                            CourseGridDetailResultModel = new CourseGridDetailResultModel();
                    }
                }
            }

            return _CourseGridDetailResultModel;
        }

        public ListAdminCanvasSectionCourses GetAdminCanvasSectionCourses(int canvasId)
        {
            ListAdminCanvasSectionCourses AdminCanvasSectionCourses = new ListAdminCanvasSectionCourses();
            AdminCanvasSectionCourses.AdminCanvasSectionCourses = new List<AdminCanvasSectionCourseModel>();
            AdminCanvasSectionCourseModel AdminCanvasSectionCourse = new AdminCanvasSectionCourseModel();
            using (var db = new SchoolEntities())
            {
                var courses = (from course in db.Courses where course.canvas_course_id == canvasId select course).ToList();
                DateTime? startdate= new DateTime();
                if(courses!=null){
                    foreach(var _course in courses){
                        AdminCanvasSectionCourse.CourseId = _course.COURSEID;
                        AdminCanvasSectionCourse.CourseName = _course.COURSENAME;
                        AdminCanvasSectionCourse.Coursenumber = _course.COURSENUM;
                        AdminCanvasSectionCourse.CanvasCourseId = _course.canvas_course_id.Value;
                        startdate = (from _date in db.Course_Times where _date.COURSEID == _course.COURSEID orderby _date.COURSEDATE ascending select _date.COURSEDATE).FirstOrDefault();
                        AdminCanvasSectionCourse.CourseStartDate = startdate; 
                        System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                        dynamic courseconfiguration = JSSerializeObj.Deserialize(_course.CourseConfiguration, typeof(object));
                        try
                        {
                            AdminCanvasSectionCourse.CanvasSectionNAme = courseconfiguration["canvassectionName"]; ;
                            AdminCanvasSectionCourse.CanvasSectionID =int.Parse( courseconfiguration["canvassectionID"]);
                        }
                        catch
                        {
                            AdminCanvasSectionCourse.CanvasSectionNAme = "n/a" ;
                            AdminCanvasSectionCourse.CanvasSectionID = 0;
                        }

                        AdminCanvasSectionCourses.AdminCanvasSectionCourses.Add(AdminCanvasSectionCourse);
                        AdminCanvasSectionCourse = new AdminCanvasSectionCourseModel();
                    }
                }
            }
            AdminCanvasSectionCourses.totalCount = AdminCanvasSectionCourses.AdminCanvasSectionCourses.Count();
            return AdminCanvasSectionCourses;
        }

        //with constring. it wont work in cross site
        public void DeactivateStudents(int studentid)
        {

            using (var db = new SchoolEntities())
            {
                var student = db.Students.Where(s => s.STUDENTID == studentid).SingleOrDefault();
                if (student != null)
                {
                    student.InActive = 1;
                    db.SaveChanges();
                }
            }
        }
        public void ReactivateStudents(int studentid)
        {

            using (var db = new SchoolEntities())
            {
                var student = db.Students.Where(s => s.STUDENTID == studentid).SingleOrDefault();
                if (student != null)
                {
                    student.InActive = 0;
                    db.SaveChanges();
                }
            }
        }


    }
}
