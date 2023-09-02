using Gsmu.Service.API.Extensions;
using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.API.Security;
using Gsmu.Service.Interface.Admin.Global;
using Gsmu.Service.Models.Admin.CourseDashboard;
using Gsmu.Service.Models.Admin.Global;
using Gsmu.Service.Models.Generic;
using Gsmu.Service.Models.School;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace Gsmu.Service.API.Controllers
{
    [GSMUCustomAuthorize]
    public class MasterSettingsController : ApiController
    {
        private IMasterSettingsManager _masterSettingsManager;

        public MasterSettingsController(IMasterSettingsManager masterSettingsManager)
        {
            _masterSettingsManager = masterSettingsManager;
        }

        [HttpGet]
        [Route("MasterSettings/GetMasterSettings")]
        [ResponseType(typeof(ResponseRecordModel<MasterSettingsModel>))]
        public IHttpActionResult GetMasterSettings()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetMasterSettings();
                return new ResponseRecordModel<MasterSettingsModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Master Setting Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetAudiences")]
        [ResponseType(typeof(ResponseRecordModel<List<AudienceModel>>))]
        public IHttpActionResult GetAudiences()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetAudiences();
                return new ResponseRecordModel<List<AudienceModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Audiences Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetIcons")]
        [ResponseType(typeof(ResponseRecordModel<List<IconsModel>>))]
        public IHttpActionResult GetIcons()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetIcons();
                return new ResponseRecordModel<List<IconsModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Icons Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetDepartments")]
        [ResponseType(typeof(ResponseRecordModel<List<DeparmentModel>>))]
        public IHttpActionResult GetDepartments()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetDepartments();
                return new ResponseRecordModel<List<DeparmentModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Departments Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetCourseColorGrouping")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseGroupingModel>>))]
        public IHttpActionResult GetCourseColorGrouping()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetCourseColorGrouping();
                return new ResponseRecordModel<List<CourseGroupingModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Color Grouping Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetCustomCertificates")]
        [ResponseType(typeof(ResponseRecordModel<List<CustomCertificateModel>>))]
        public IHttpActionResult GetCustomCertificates()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetCustomCertificateModel();
                return new ResponseRecordModel<List<CustomCertificateModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Custom Certificates Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetAllPricingOptions")]
        [ResponseType(typeof(ResponseRecordModel<List<PricingOptionsModel>>))]
        public IHttpActionResult GetAllPricingOptions()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetAllPricingOptions();
                return new ResponseRecordModel<List<PricingOptionsModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Pricing Options Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetMainCategories")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetMainCategories()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetMainCategories();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Main Categories Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetSubCategories")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetSubCategories()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetSubCategories();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Sub Categories Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetSubSubCategories")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetSubSubCategories()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetSubSubCategories();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Sub Sub Categories Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/CourseChoices")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseChoices>>))]
        public IHttpActionResult GetCourseChoices()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetCourseChoices();
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
        [Route("MasterSettings/GetSurveys")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetSurveys()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetSurveys();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Surveys Success",
                    Data = data
                };
            }));
        }
        [HttpGet]
        [Route("MasterSettings/GetLocations")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetLocations()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetLocations();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Locations Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetRooms")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetRooms()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetRooms();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Rooms Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetRoomDirections")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetRoomDirections()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetRoomDirections();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Room Directions Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetCountries")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetCountries()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetCountries();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Countries Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetInstructors")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetInstructors()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetInstructors();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Instructors Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("MasterSettings/GetMaterials")]
        [ResponseType(typeof(ResponseRecordModel<List<DropdownModel>>))]
        public IHttpActionResult GetMaterials()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _masterSettingsManager.GetMaterials();
                return new ResponseRecordModel<List<DropdownModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Materials Success",
                    Data = data
                };
            }));
        }
    }
}
