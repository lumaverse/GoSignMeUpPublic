using Gsmu.Service.API.Extensions;
using Gsmu.Service.API.Models.ResponseModels;
using Gsmu.Service.Interface.Admin.Reports;
using Gsmu.Service.Models.Admin.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Gsmu.Service.API.Controllers.Admin
{
    public class ReportsController : ApiController
    {
        private IReportsManager _reportsManager;

        public ReportsController(IReportsManager reportsManager)
        {
            _reportsManager = reportsManager;
        }

        [HttpGet]
        [Route("Reports/GetPaymentClassListReports")]
        [ResponseType(typeof(ResponseRecordModel<List<ReportsPaymentClassListModel>>))]
        public IHttpActionResult GetPaymentClassListReports(int courseId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _reportsManager.GeneratePaymentClassListReport(courseId);
                return new ResponseRecordModel<List<ReportsPaymentClassListModel>>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Payment ClassList Reports Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("Reports/GetOrderDetailReport")]
        [ResponseType(typeof(ResponseRecordModel<OrderDetailModel>))]
        public IHttpActionResult GetOrderDetailReport(string orderNumber)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _reportsManager.GetOrderDetailReport(orderNumber);
                return new ResponseRecordModel<OrderDetailModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Order Detail Report Success",
                    Data = data
                };
            }));
        }

        [HttpGet]
        [Route("Reports/GetPaymentDetailReport")]
        [ResponseType(typeof(ResponseRecordModel<ReportsPaymentInfoModel>))]
        public IHttpActionResult GetPaymentDetailReport(int rosterId)
        {
            return Ok(this.ConsistentApiHandling(() =>
            {
                var data = _reportsManager.GetPaymentDetailReport(rosterId);
                return new ResponseRecordModel<ReportsPaymentInfoModel>
                {
                    Success = 1,
                    ErrorCode = 0,
                    Message = "Get Payment Detail Report Success",
                    Data = data
                };
            }));
        }
    }
}
