using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gsmu.Service.BusinessLogic.Courses;
using Gsmu.Service.Interface.Courses;
using System.Web.Http.Description;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.API.Extensions;
using System.Net.Http;
using System.Web.Http;
using Gsmu.Service.Models.GlobalTools.Grids;
using Gsmu.Service.Models.Admin.Portal;
namespace Gsmu.Service.API.Controllers
{

    public class PublicCourseGridController : ApiController
    {
                private ICourseGrid _courseGrid;

       public PublicCourseGridController(ICourseGrid courseGrid){
            _courseGrid = courseGrid;
        }
        [HttpGet]
        [Route("PublicCourseGrid/GetCategoryTree")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseMainCategoryModel>>))]
        public IHttpActionResult GetCategoryTree()
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _courseGrid.GetCourseCategoryTree(Gsmu.Service.Models.Enum.CourseActiveState.All);
                return new ResponseRecordModel<List<CourseMainCategoryModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course Category Tree Success",
                    Data = data
                };
            }));


        }
        [Route("PublicCourseGrid/GetCourseList")]
        [ResponseType(typeof(ResponseRecordModel<List<CourseGridDetailResultModel>>))]
        public IHttpActionResult GetCourseList(
            int page = 1,
            int? pageSize = 0,
            string Keyword = null,
            string MainCategory = null,
            string SubCategory = null,
            string SubSubCategory=null
            )
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                QueryStateConfig queryState = new QueryStateConfig
                {
                    Page = page,
                    PageSize = pageSize.Value,
                };
                var data = _courseGrid.GetCourseList(queryState, Keyword,MainCategory,SubCategory,SubSubCategory);
                return new ResponseRecordModel<List<CourseGridDetailResultModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Course List Success",
                    Data = data
                };
            }));


        }
    }
}