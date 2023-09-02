using System;
using System.Web.Http;

namespace Gsmu.Service.API.Extensions
{
    public static class ApiControllerExtensions
    {
        public static object ConsistentApiHandling(this ApiController controller, Func<object> getResult)
        {
            try
            {
                return getResult();
            }
            catch (Exception ex)
            {
                return new Models.ResponseModels.ResponseModel
                {
                    ErrorCode = ex.HResult,
                    Message = ex.Message.ToString(),
                    Success = 0
                };
            }
        }
    }
}