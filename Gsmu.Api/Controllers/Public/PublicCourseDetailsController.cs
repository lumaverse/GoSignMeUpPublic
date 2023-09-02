using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Gsmu.Service.Interface.Courses;
using System.Web.Http.Description;
using Gsmu.Service.API.Extensions;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.API.Models.ResponseModels;

namespace Gsmu.Service.API.Controllers.Public
{
    public class PublicCourseDetailsController: ApiController
    {
        private ICourseDetails _CourseDetails;

        public PublicCourseDetailsController(ICourseDetails CourseDetails)
          {
              _CourseDetails = CourseDetails;
        }
        [HttpGet]
        [Route("PublicCourseDetails/GetCourseFullDetailsById")]
        [ResponseType(typeof(ResponseRecordModel<object>))]
        public IHttpActionResult GetCourseFullDetailsById(int courseId)
        {
            var CourseDetails =_CourseDetails.GetCourseDetailsById(courseId);
            string Message = "Get Course Details Success.";
            if (CourseDetails.CourseBasicDetails == null)
            {
                Message = "Course is not Accessible.";
            }
            var ScrubbedResult=  new System.Web.Mvc.JsonResult()
            {
                Data = CourseDetails,
            };
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = ScrubbedResult;
                return new ResponseRecordModel<object>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = Message,
                    Data = data.Data
                };
            }));


        }
        [HttpGet]
        [Route("PublicCourseDetails/GetEventCourseFullDetailsById")]
        [ResponseType(typeof(ResponseRecordModel<object>))]
        public IHttpActionResult GetEventCourseFullDetailsById(int eventId)
        {
            var EventDetails = _CourseDetails.GetEventCourseFullDetailsById(eventId);
            string Message = "Get Course Details Success.";
            var ScrubbedResult = new System.Web.Mvc.JsonResult()
            {
                Data = EventDetails,
            };
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = ScrubbedResult;
                return new ResponseRecordModel<object>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = Message,
                    Data = data.Data
                };
            }));


        }
    }
}