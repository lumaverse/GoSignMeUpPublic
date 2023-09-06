using Gsmu.Api.Data.School.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using school = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.CourseSettings
{
    public class CouponsModel
    {

        public class CouponsOptions
        {
            public bool HideCoupons { get; set; }
            public bool AllowCouponPerOrder { get; set; }
        }

        public class CouponsReportModel
        {
            public int CouponId { get; set; }
            public string CouponCode { get; set; }
            public double? CouponPercentAmount { get; set; }
            public double? CouponDollarAmount { get; set; }
            public DateTime? CouponStartDate { get; set; }
            public DateTime? CouponExpireDate { get; set; }
            public int? CouponCourseId { get; set; }
            public int? CouponOneTimeUse { get; set; }
            public int? CouponMaterialsDiscounted { get; set; }
            public double? CouponPerCourseAmount { get; set; }
            public string additionalcourseid { get; set; }
            public int? CouponAutomatic { get; set; }
            public int? CouponOrderCourses { get; set; }
            public int? NoOfCouponAvailable { get; set; }
            public int? LimitCouponUsageinSystem { get; set; }
            public bool IsValidForShoppingCart { get; set; }
        }

        public class CouponsCourses
        {
            public int CourseId { get; set; }
            public string CourseNum { get; set; }
            public string CourseName { get; set; }
            public DateTime? CourseDate { get; set; }
        }

        public class CoursesCoupons
        {
            public int CouponId { get; set; }
            public string CouponCode { get; set; }
            public DateTime? CouponStartDate { get; set; }
            public DateTime? CouponExpireDate { get; set; }
            public string Additionalcourseid { get; set; }
        }

        public static CouponsOptions GetCouponOptions()
        {
            var cOptions = new CouponsOptions();
            cOptions.HideCoupons = Convert.ToBoolean(Settings.Instance.GetMasterInfo3().HideCoupons);
            cOptions.AllowCouponPerOrder = Convert.ToBoolean(Settings.Instance.GetMasterInfo4().AllowCouponPerOrder);

            return cOptions;
        }

        public static JsonResult UpdateCouponOptions(HttpRequestBase Request)
        {
            var param = GetParamList(Request);

            var hideCoupons = param["hidecouponfields"][0] == "true" ? 1 : 0;
            var allowCouponPerOrder = param["stackcouponpercourse"][0] == "true" ? 1 : 0;

            using (var db = new SchoolEntities())
            {
                db.masterinfo4.Where(x => 0 == x.subsiteid).ToList().ForEach(a => a.AllowCouponPerOrder = allowCouponPerOrder);
                db.SaveChanges();

                db.MasterInfo3.Where(x => 0 == x.SubSiteId).ToList().ForEach(a => a.HideCoupons = hideCoupons);
                db.SaveChanges();
            }

            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    message = "Successfully updated!"
                }
            };
        }

        public static JsonResult UpdateCoupon(HttpRequestBase Request)
        {

            var param = GetParamList(Request);

            var list = JsonConvert.DeserializeObject<List<CouponsCourses>>(Request.Form.Get("courseList"));

            var actionType = param["actionType"][0];
            var couponId = Convert.ToInt32(param["couponId"][0]);
            var couponCode = param["couponCode"][0];
            var startDate = Convert.ToDateTime(param["startDate"][0]);
            var endDate = Convert.ToDateTime(param["endDate"][0]);
            var percentOffAmount = float.Parse(param["percentOffAmount"][0]);
            var dollarOffAmount = float.Parse(param["dollarOffAmount"][0]);
            var maximumOrderDiscount = Convert.ToInt32(param["maximumOrderDiscount"][0]);
            var oneTimeUseperStudent = Convert.ToInt32(param["oneTimeUseperStudent"][0]);
            var limitNumberUsage = Convert.ToInt32(param["limitNumberUsage"][0]);
            var noOfCouponAvailable = Convert.ToInt32(param["x"][0]);
            var appliesTowardsMaterials = Convert.ToInt32(param["appliesTowardsMaterials"][0]);
            var coursesInTheOrder = Convert.ToInt32(param["coursesInTheOrder"][0]);
            var automaticCoupon = Convert.ToInt32(param["automaticCoupon"][0]);
            var additionalCourseId = string.Empty;

            try
            {
                foreach (var course in list)
                {
                    additionalCourseId += course.CourseId + ",";
                }

                using (var db = new SchoolEntities())
                {
                    if (actionType == "add")
                    {
                        couponId = Coupon.NextId;

                        var newCoupon = new Coupon()
                        {
                            CouponId = couponId,
                            CouponCode = couponCode,
                            CouponStartDate = startDate,
                            CouponExpireDate = endDate,
                            CouponPercentAmount = percentOffAmount,
                            CouponDollarAmount = dollarOffAmount,
                            CouponPerCourseAmount = maximumOrderDiscount,
                            CouponMaterialsDiscounted = appliesTowardsMaterials,
                            CouponOneTimeUse = oneTimeUseperStudent,
                            LimitCouponUsageinSystem = limitNumberUsage,
                            NoOfCouponAvailable = noOfCouponAvailable,
                            CouponAutomatic = automaticCoupon,
                            additionalcourseid = additionalCourseId,
                        };

                        AddCoupon(newCoupon);

                    }
                    else
                    {
                        Coupon objCoupon = db.Coupons.Single(coupon => coupon.CouponId == couponId);
                        objCoupon.CouponCode = couponCode;
                        objCoupon.CouponStartDate = startDate;
                        objCoupon.CouponExpireDate = endDate;
                        objCoupon.CouponPercentAmount = percentOffAmount;
                        objCoupon.CouponDollarAmount = dollarOffAmount;
                        objCoupon.CouponPerCourseAmount = maximumOrderDiscount;
                        objCoupon.CouponMaterialsDiscounted = appliesTowardsMaterials;
                        objCoupon.CouponOneTimeUse = oneTimeUseperStudent;
                        objCoupon.LimitCouponUsageinSystem = limitNumberUsage;
                        objCoupon.NoOfCouponAvailable = noOfCouponAvailable;
                        objCoupon.CouponOrderCourses = coursesInTheOrder;
                        objCoupon.CouponAutomatic = automaticCoupon;
                        objCoupon.additionalcourseid = additionalCourseId;

                        db.SaveChanges();
                    }
                }

                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Successfully updated!"
                    }
                };

            }
            catch (Exception ex)
            {

                //Re-insert the coupon if identity is off
                if (ex.Message.Contains("IDENTITY_INSERT is set to OFF"))
                {
                    try
                    {
                        using (var db = new SchoolEntities())
                        {
                            var newCoupon = new Coupon()
                            {
                                CouponCode = couponCode,
                                CouponStartDate = startDate,
                                CouponExpireDate = endDate,
                                CouponPercentAmount = percentOffAmount,
                                CouponDollarAmount = dollarOffAmount,
                                CouponPerCourseAmount = maximumOrderDiscount,
                                CouponMaterialsDiscounted = appliesTowardsMaterials,
                                CouponOneTimeUse = oneTimeUseperStudent,
                                LimitCouponUsageinSystem = limitNumberUsage,
                                NoOfCouponAvailable = noOfCouponAvailable,
                                CouponAutomatic = automaticCoupon,
                                additionalcourseid = additionalCourseId,
                            };

                            db.Coupons.Add(newCoupon);
                            db.SaveChanges();
                        }

                        return new JsonResult
                        {
                            Data = new
                            {
                                success = true,
                                message = "Successfully updated!"
                            }
                        };

                    }
                    catch (Exception er)
                    {
                        return new JsonResult
                        {
                            Data = new
                            {
                                success = false,
                                message = er.Message
                            }
                        };
                    }
                }
                else
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            success = false,
                            message = ex.Message
                        }
                    };
                }
            }
        }

        public static JsonResult AssignCouponToCourse(HttpRequestBase Request)
        {
            try
            {
                var param = GetParamList(Request);

                var couponIdist = JsonConvert.DeserializeObject<List<int>>(Request.Form.Get("couponList"));
                var courseId = param["courseId"][0];

                using (var db = new SchoolEntities())
                {
                    foreach (var coupon in couponIdist)
                    {
                        Coupon objCoupon = db.Coupons.Single(c => c.CouponId == coupon);
                        objCoupon.additionalcourseid = objCoupon.additionalcourseid.TrimEnd(',') + "," + courseId;

                        db.SaveChanges();
                    }
                }

                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Successfully updated!"
                    }
                };

            }
            catch (Exception)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "Fail to update!"
                    }
                };
            }
        }

        public static JsonResult DeleteAssignCouponToCourse(HttpRequestBase Request)
        {
            try
            {
                var param = GetParamList(Request);

                var couponIdist = JsonConvert.DeserializeObject<List<int>>(Request.Form.Get("couponList"));
                var courseId = param["courseId"][0];

                using (var db = new SchoolEntities())
                {
                    foreach (var coupon in couponIdist)
                    {
                        Coupon objCoupon = db.Coupons.Single(c => c.CouponId == coupon);
                        objCoupon.additionalcourseid = objCoupon.additionalcourseid.Replace("," + courseId, string.Empty).Replace(courseId, string.Empty);

                        db.SaveChanges();
                    }
                }

                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Successfully updated!"
                    }
                };

            }
            catch (Exception)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "Fail to update!"
                    }
                };
            }
        }

        public static JsonResult DeleteCoupon(HttpRequestBase Request)
        {
            var param = GetParamList(Request);

            var couponId = Convert.ToInt32(param["couponId"][0]);

            using (var db = new SchoolEntities())
            {
                var coupon = (from c in db.Coupons
                              where c.CouponId == couponId
                              select c).First();

                db.Coupons.Remove(coupon);
                db.SaveChanges();
            }

            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    message = "Successfully deleted!"
                }
            };
        }

        public static List<CouponsReportModel> GenerateCouponReport(HttpRequestBase Request, bool isExport)
        {
            ////Pending to use Request/isExport
            try
            {

                QueryState queryState = BuildRequestQuery(Request);

                var couponCode = string.Empty;
                var dateFrom = string.Empty;
                var dateTo = string.Empty;

                if (queryState.Filters != null)
                {
                    //filter Coupon Code

                    if (queryState.Filters.ContainsKey("coupon-code"))
                    {
                        couponCode = queryState.Filters["coupon-code"].ToLower();
                    }

                    //filter  date-from
                    if (queryState.Filters.ContainsKey("date-from"))
                    {
                        dateFrom = queryState.Filters["date-from"];
                    }
                    else
                    {
                        dateFrom = DateTime.Now.AddDays(-90).Date.ToString();
                    }
                    //filter date-to
                    if (queryState.Filters.ContainsKey("date-to"))
                    {
                        dateTo = queryState.Filters["date-to"];
                    }
                    else
                    {
                        dateTo = DateTime.Now.AddDays(90).ToString();
                    }

                }
                else
                {
                    dateFrom = DateTime.Now.AddDays(-90).Date.ToString();
                    dateTo = DateTime.Now.AddDays(90).ToString();
                }

                var couponReportData = new List<CouponsReportModel>();

                using (var db = new school.SchoolEntities())
                {
                    DateTime fromDate = Convert.ToDateTime(dateFrom);
                    DateTime toDate = Convert.ToDateTime(dateTo);

                    couponReportData = (from coupon in db.Coupons
                                        where (DbFunctions.TruncateTime(coupon.CouponStartDate.Value) >= fromDate &&
                                                 DbFunctions.TruncateTime(coupon.CouponExpireDate.Value) <= toDate) &&
                                                  (coupon.CouponCode.ToLower().StartsWith(couponCode) || coupon.CouponCode.ToLower().EndsWith(couponCode))
                                        select new CouponsReportModel()
                                        {
                                            CouponId = coupon.CouponId,
                                            CouponCode = coupon.CouponCode,
                                            CouponPercentAmount = coupon.CouponPercentAmount,
                                            CouponDollarAmount = coupon.CouponDollarAmount,
                                            CouponStartDate = coupon.CouponStartDate,
                                            CouponExpireDate = coupon.CouponExpireDate,
                                            CouponCourseId = coupon.CouponCourseId,
                                            CouponOneTimeUse = coupon.CouponOneTimeUse,
                                            CouponMaterialsDiscounted = coupon.CouponMaterialsDiscounted,
                                            CouponPerCourseAmount = coupon.CouponPerCourseAmount,
                                            additionalcourseid = coupon.additionalcourseid,
                                            CouponAutomatic = coupon.CouponAutomatic,
                                            CouponOrderCourses = coupon.CouponOrderCourses,
                                            NoOfCouponAvailable = coupon.NoOfCouponAvailable,
                                            LimitCouponUsageinSystem = coupon.LimitCouponUsageinSystem
                                        }).ToList();
                }

                return couponReportData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<CoursesCoupons> GetCourseAssignedCouponList(HttpRequestBase Request)
        {

            try
            {

                var param = GetParamList(Request);

                var courseId = param["courseId"][0];

                var coursesCoupons = new List<CoursesCoupons>();

                using (var db = new school.SchoolEntities())
                {

                    coursesCoupons = (from c in
                                          ((from c in db.Coupons
                                            select new CoursesCoupons()
                                            {
                                                CouponId = c.CouponId,
                                                CouponCode = c.CouponCode,
                                                CouponStartDate = c.CouponStartDate,
                                                CouponExpireDate = c.CouponExpireDate,
                                                Additionalcourseid = c.additionalcourseid
                                            }).ToList())
                                      where Array.IndexOf(c.Additionalcourseid.Split(','), courseId) > -1
                                      select c).ToList();
                }

                return coursesCoupons;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<CoursesCoupons> GetCouponList(HttpRequestBase Request)
        {

            try
            {
                QueryState queryState = BuildRequestQuery(Request);
                var param = GetParamList(Request);
                var couponCode = string.Empty;
                var courseId = param["courseId"][0];

                if (queryState.Filters != null)
                {
                    if (queryState.Filters.ContainsKey("couponCode"))
                    {
                        couponCode = queryState.Filters["couponCode"].ToLower();
                    }
                }

                var coursesCoupons = new List<CoursesCoupons>();

                using (var db = new school.SchoolEntities())
                {

                    coursesCoupons = (from c in
                                          ((from c in db.Coupons
                                            select new CoursesCoupons()
                                            {
                                                CouponId = c.CouponId,
                                                CouponCode = c.CouponCode,
                                                CouponStartDate = c.CouponStartDate,
                                                CouponExpireDate = c.CouponExpireDate,
                                                Additionalcourseid = c.additionalcourseid
                                            }).ToList())
                                      where Array.IndexOf(c.Additionalcourseid.Split(','), courseId) < 0
                                      && (c.CouponCode.ToLower().StartsWith(couponCode) || c.CouponCode.ToLower().EndsWith(couponCode))
                                      select c).ToList();
                }

                return coursesCoupons;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<CouponsCourses> GenerateCouponCourseList(HttpRequestBase Request)
        {

            try
            {

                QueryState queryState = BuildRequestQuery(Request);

                var courseName = string.Empty;
                var couponCode = string.Empty;
                var dateFrom = string.Empty;
                var dateTo = string.Empty;

                if (queryState.Filters != null)
                {
                    if (queryState.Filters.ContainsKey("course-name"))
                    {
                        courseName = queryState.Filters["course-name"].ToLower();
                    }

                    if (queryState.Filters.ContainsKey("coupon-code"))
                    {
                        couponCode = queryState.Filters["coupon-code"].ToLower();
                    }
                }

                var couponCourseData = new List<CouponsCourses>();

                using (var db = new school.SchoolEntities())
                {

                    couponCourseData = (from c in
                                            ((from c in db.Courses
                                              join ct in db.Course_Times on c.COURSEID equals ct.COURSEID
                                              where (c.COURSENAME.ToLower().StartsWith(courseName) || c.COURSENAME.ToLower().EndsWith(courseName))
                                              && c.CANCELCOURSE == 0 && c.eventid == 0 //&& DbFunctions.TruncateTime(ct.COURSEDATE) >= Convert.ToDateTime("05-23-2017")
                                              select new CouponsCourses()
                                              {
                                                  CourseId = c.COURSEID,
                                                  CourseNum = c.COURSENUM,
                                                  CourseName = c.COURSENAME,
                                                  CourseDate = ct.COURSEDATE
                                              }).ToList())
                                        where c.CourseDate >= DateTime.Now
                                        select c).Distinct().Take(50).ToList();
                }

                return couponCourseData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<CouponsCourses> GenerateAssignedCouponCourseList(HttpRequestBase Request)
        {

            try
            {

                QueryState queryState = BuildRequestQuery(Request);

                var couponCode = string.Empty;
                var dateFrom = string.Empty;
                var dateTo = string.Empty;

                if (queryState.Filters != null)
                {
                    if (queryState.Filters.ContainsKey("coupon-code"))
                    {
                        couponCode = queryState.Filters["coupon-code"].ToLower();
                    }
                }

                var couponCourseData = new List<CouponsCourses>();

                using (var db = new school.SchoolEntities())
                {
                    string[] couponCourseList = (from c in db.Coupons
                                                 where c.CouponCode == couponCode
                                                 select c.additionalcourseid).FirstOrDefault().Split(',');

                    couponCourseData = (from c in
                                            (from course in db.Courses
                                             join ct in db.Course_Times on course.COURSEID equals ct.COURSEID
                                             where couponCourseList.Contains(course.COURSEID.ToString())
                                             select new { course, ct })
                                        group c by
                                            new { c.course.COURSEID, c.course.COURSENUM, c.course.COURSENAME } into a
                                        select new CouponsCourses()
                                        {
                                            CourseId = a.Key.COURSEID,
                                            CourseNum = a.Key.COURSENUM,
                                            CourseName = a.Key.COURSENAME,
                                            CourseDate = a.Min(k => k.ct.COURSEDATE)
                                        }).Take(50).ToList();
                }

                return couponCourseData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, string[]> GetParamList(HttpRequestBase Request)
        {
            NameValueCollection paramItems = Request.Params;

            Dictionary<string, string[]> paramList = new Dictionary<string, string[]>();

            for (int i = 0; i <= paramItems.Count - 1; i++)
            {
                paramList.Add(paramItems.GetKey(i), paramItems.GetValues(i));
            }

            return paramList;
        }

        public static QueryState BuildRequestQuery(HttpRequestBase Request)
        {
            var jresult = new JsonResult();
            var filter = @"[{'property':'keyword','value':''}]";

            if (Request.QueryString["filter"] != null)
            {
                filter = Request.QueryString["filter"];
            }

            var start = 0;
            if (Request.QueryString["start"] != null)
            {
                start = int.Parse(Request.QueryString["start"]);
            }
            var limit = 50;
            if (Request.QueryString["limit"] != null)
            {
                limit = int.Parse(Request.QueryString["limit"]);
            }
            var page = 1;
            if (Request.QueryString["page"] != null)
            {
                page = int.Parse(Request.QueryString["page"]);
            }

            jresult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
            var queryState = new QueryState(start, limit)
            {
                Filters = filterResult,
                Page = page
            };

            return queryState;
        }

        public static void AddCoupon(Coupon c)
        {
            using (var db = Connections.GetSchoolConnection())
            {
                db.Open();
                var query = "INSERT INTO Coupons" +
                            "(CouponId, CouponCode, CouponPercentAmount, CouponDollarAmount, CouponStartDate, CouponExpireDate, CouponCourseId, CouponOneTimeUse, CouponMaterialsDiscounted, CouponPerCourseAmount, additionalcourseid, CouponAutomatic, CouponOrderCourses, NoOfCouponAvailable, LimitCouponUsageinSystem)" +
                            "VALUES" +
                            "('" + replaceField(c.CouponId.ToString()) + "'," +
                            "'" + replaceField(c.CouponCode.ToString()) + "'," +
                            "'" + replaceField(c.CouponPercentAmount.ToString()) + "'," +
                            "'" + replaceField(c.CouponDollarAmount.ToString()) + "'," +
                            "'" + replaceField(c.CouponStartDate.ToString()) + "'," +
                            "'" + replaceField(c.CouponExpireDate.ToString()) + "'," +
                            "'" + replaceField(c.CouponCourseId.ToString()) + "'," +
                            "'" + replaceField(c.CouponOneTimeUse.ToString()) + "'," +
                            "'" + replaceField(c.CouponMaterialsDiscounted.ToString()) + "'," +
                            "'" + replaceField(c.CouponPerCourseAmount.ToString()) + "'," +
                            "'" + replaceField(c.additionalcourseid.ToString()) + "'," +
                            "'" + replaceField(c.CouponAutomatic.ToString()) + "'," +
                            "'" + replaceField(c.CouponOrderCourses.ToString()) + "'," +
                            "'" + replaceField(c.NoOfCouponAvailable.ToString()) + "'," +
                            "'" + replaceField(c.LimitCouponUsageinSystem.ToString()) + "')";
                var cmd = db.CreateCommand();
                cmd.CommandText = query;
                var result = cmd.ExecuteScalar();
                db.Close();
            }
        }

        private static string replaceField(string val)
        {
            return val.Replace("'", "''");
        }
    }
}
