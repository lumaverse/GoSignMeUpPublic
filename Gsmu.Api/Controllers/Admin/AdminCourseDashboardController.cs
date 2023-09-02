using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.Models.Admin.CourseDashboard;
using Gsmu.Service.Interface.Admin.Dashboard;
using Gsmu.Service.BusinessLogic.Admin.Dashboard;
using Gsmu.Service.API.Extensions;
using Gsmu.Service.API.Security;
using Gsmu.Service.Models.School;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.Generic;

namespace Gsmu.Service.API.Controllers.Admin
{
    //[Authorize] -- uncomment this and this will ask for a token - uses OWIN implementation
    [GSMUCustomAuthorize] // -- use custom implementation
    public class AdminCourseDashboardController : ApiController
    {
        private ICourseDashboardManager _courseManager;

        public AdminCourseDashboardController(ICourseDashboardManager courseManager) {
            _courseManager = courseManager;
        }

        [HttpPost]
        [Route("AdminCourseDash/GetCourseByFilter")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseBasicDetails>>))]
        public IHttpActionResult GetCourseByFilter()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseByFilter();
                return new ResponseRecordModel<List<CourseBasicDetails>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Description Success",
                    results = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseByFilter")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseBasicDetails>>))]
        public IHttpActionResult GetCourseByFilter(string filter)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseByFilter(filter);
                return new ResponseRecordModel<List<CourseBasicDetails>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Description Success",
                    results = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseDescriptionById")]
        [ResponseType(typeof(ResponseRecordModel<CourseDescriptionModel>))]
        public IHttpActionResult GetCourseDescriptionById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseDescriptionById(courseId);
                return new ResponseRecordModel<CourseDescriptionModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Description Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseRostersById")]
        [ResponseType(typeof(ResponseRecordModel<CourseRostersMainModel>))]
        public IHttpActionResult GetCourseRostersById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseRostersById(courseId);
                return new ResponseRecordModel<CourseRostersMainModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Rosters Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseExpensesById")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseExpensesModel>>))]
        public IHttpActionResult GetCourseExpensesById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseExpensesById(courseId);
                return new ResponseRecordModel<List<CourseExpensesModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Expense Success",
                    Data = data
                };
            }));

        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseDateAndTimesById")]
        [ResponseType(typeof(ResponseRecordModel<List<Service.Models.Courses.CourseDateTimeModel>>))]
        public IHttpActionResult GetCourseDateAndTimesById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseDateAndTimesById(courseId);
                return new ResponseRecordModel<List<Service.Models.Courses.CourseDateTimeModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Date And Times Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseInstructorsById")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseInstructorsModel>>))]
        public IHttpActionResult GetCourseInstructorsById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseInstructorsById(courseId);
                return new ResponseRecordModel<List<CourseInstructorsModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Instructor Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseMaterialsById")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseMaterialsModel>>))]
        public IHttpActionResult GetCourseMaterialsById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseMaterialsById(courseId);
                return new ResponseRecordModel<List<CourseMaterialsModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Materials Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseConfiguration")]
        [ResponseType(typeof(ResponseRecordModel<CourseConfigurationModel>))]
        public IHttpActionResult GetCourseConfiguration()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseConfiguration();
                return new ResponseRecordModel<CourseConfigurationModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Configuration Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseCategoriesById")]
        [ResponseType(typeof(ResponseRecordModel<List<Service.Models.Courses.CourseCategoriesModel>>))]
        public IHttpActionResult GetCourseCategoriesById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseCategoriesById(courseId);
                return new ResponseRecordModel<List<Service.Models.Courses.CourseCategoriesModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Categories Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCoursePricingById")]
        [ResponseType(typeof(ResponseRecordModel<CoursePricingMainModel>))]
        public IHttpActionResult GetCoursePricingById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCoursePricingById(courseId);
                return new ResponseRecordModel<CoursePricingMainModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Prices Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseChoiceById")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseChoices>>))]
        public IHttpActionResult GetCourseChoiceById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseChoicesById(courseId);
                return new ResponseRecordModel<List<CourseChoices>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Choices Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetCourseTransciptsById")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseTransciptsModel>>))]
        public IHttpActionResult GetCourseTransciptsById([FromUri] int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetCourseTransciptsById(courseId);
                return new ResponseRecordModel<List<CourseTransciptsModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Transcripts Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetInstructorById")]
        [ResponseType(typeof(ResponseRecordModel<InstructorModel>))]
        public IHttpActionResult GetInstructorById([FromUri] int instructorId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetInstructorById(instructorId);
                return new ResponseRecordModel<InstructorModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Instructor Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("AdminCourseDash/GetDocumentFilesById")]
        [ResponseType(typeof(ResponseRecordModel<List<FileModel>>))]
        public IHttpActionResult GetDocumentFilesById(int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.GetDocumentFilesById(courseId);
                return new ResponseRecordModel<List<FileModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Price Success",
                    Data = data
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveCourse")]
        [ResponseType(typeof(ResponseRecordModel<Gsmu.Service.Models.Courses.CourseModel>))]
        public IHttpActionResult SaveCourse([FromBody] CourseDescriptionModel model)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.SaveCourse(model);
                return new ResponseRecordModel<Gsmu.Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveDateTime")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseDateTimeModel>))]
        public IHttpActionResult SaveDateTime(Service.Models.Courses.CourseDateTimeModel model)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseManager.SaveDateTime(model);
                return new ResponseRecordModel<Service.Models.Courses.CourseDateTimeModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course DateTime Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveCourseImage")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseModel>))]
        public IHttpActionResult SaveCourseImage()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                 _courseManager.SaveCourseImage();
                return new ResponseRecordModel<Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Image Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/DeleteCourseImage")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseDateTimeModel>))]
        public IHttpActionResult DeleteCourseImage()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.DeleteCourseImage();
                return new ResponseRecordModel<Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Delete Course Image Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveMaterials")]
        [ResponseType(typeof(ResponseRecordModel<CourseMaterialsModel>))]
        public IHttpActionResult SaveMaterials(int courseId, int materialId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.SaveCourseMaterial(courseId, materialId);
                return new ResponseRecordModel<CourseMaterialsModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Material Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveCourseChoice")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseModel>))]
        public IHttpActionResult SaveCourseChoice(int courseId, int choiceId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.SaveCourseChoice(courseId, choiceId);
                return new ResponseRecordModel<Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Choice Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/DeleteCourseChoice")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseModel>))]
        public IHttpActionResult DeleteCourseChoice(int courseId, int choiceId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.DeleteCourseChoice(courseId, choiceId);
                return new ResponseRecordModel<Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Delete Course Choice Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveCategories")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseCategoriesModel>))]
        public IHttpActionResult SaveCategories(Service.Models.Courses.CourseCategoriesModel model)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.SaveCategories(model);
                return new ResponseRecordModel<Service.Models.Courses.CourseCategoriesModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Categories Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveCoursePrice")]
        [ResponseType(typeof(ResponseRecordModel<CoursePricingMainModel>))]
        public IHttpActionResult SaveCoursePrice(CoursePricingMainModel model)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.SaveCoursePrice(model);
                return new ResponseRecordModel<Service.Models.Courses.PriceOptionDetails>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course Price Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveFile")]
        [ResponseType(typeof(ResponseRecordModel<Service.Models.Courses.CourseModel>))]
        public IHttpActionResult SaveFile()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.SaveFile();
                return new ResponseRecordModel<Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Course File Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/DeleteCourseDateWhenOnlineById")]
        public IHttpActionResult DeleteCourseDateWhenOnlineById(int courseid) {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.DeleteCourseDateWhenOnlineById(courseid);
                return new ResponseRecordModel<Service.Models.Courses.CourseDateTimeModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Deleting Course Dates When Online Success"
                };
            }));
        }

        [HttpPost]
        [Route("AdminCourseDash/SaveInstructorBio")]
        public IHttpActionResult SaveInstructorBio(int instructorId, string bio)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                _courseManager.SaveInstructorBio(instructorId, bio);
                return new ResponseRecordModel<Service.Models.Courses.CourseModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Save Instructor Bio Success"
                };
            }));
        }
    }
}
