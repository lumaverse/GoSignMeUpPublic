using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using Gsmu.Api.Data.ViewModels.Grid;
using Gsmu.Api.Data.Calendar;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.Events.Entities;
using System.Linq.Dynamic;
namespace Gsmu.Api.Data.School.CourseRoster
{
    public static class Queries
    {
        private static string GetTimeZone()
        {
            if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 1)
            {
               return "(MST)"; // Mountain (1)
            }
            else if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 2)
            {
                return "(CST)"; // Central (2)
            }
            else if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 3)
            {
                return "(EST)"; // Eastern (3)
            }
            else if (Settings.Instance.GetMasterInfo3().system_timezone_hour == 0)
            {
               return "(PST)"; // Pacific (0)
            }
            else
            {
                return  ""; // other value means timezone is not in used. (99). If need more timezone then just add the number accordingly.
            }
        }
        public static List<LayoutOrderModel> GetLayoutOrders(string keyword)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = from cr in db.Course_Rosters
                            join s in db.Students on cr.STUDENTID equals s.STUDENTID
                            group new { cr, s } by new { cr.OrderNumber, cr.DATEADDED, s.FIRST, s.LAST } into g
                            select new LayoutOrderModel()
                            {
                                OrderNumber = g.Key.OrderNumber,
                                OrderDate = g.Key.DATEADDED,
                                StudentFirst = g.Key.FIRST,
                                StudentLast = g.Key.LAST
                            };
                if (keyword != null)
                {
                    query = query.Where(lom => lom.OrderNumber.Contains(keyword) || lom.StudentFirst.Contains(keyword) || lom.StudentLast.Contains(keyword));
                }
                query = query.OrderByDescending(lom => lom.OrderDate);
                query = query.Take(100);
                return query.ToList();
            }
        }
        public static List<string> GetAllMainCategories()
        {
            using (var db = new entities.SchoolEntities())
            {
                List<MainCategory> category = (from c in db.MainCategories
                                         join ct in db.Course_Times on c.CourseID
                                             equals ct.COURSEID
                                         join cc in db.Courses on c.CourseID equals cc.COURSEID
                                         where c.MainCategory1 != "~ZZZZZZ~Membership" && ct.COURSEDATE >= (System.Data.Entity.DbFunctions.AddDays(DateTime.Today, ((cc.viewpastcoursesdays == null ? 0 : -cc.viewpastcoursesdays))))
                                         orderby c.mcatorder,c.MainCategory1 select c).Distinct().ToList();

                var categories = category.OrderBy(a => a.mcatorder).ThenBy(a => a.MainCategory1);
                var orderedcateg = categories.Select(a => a.MainCategory1).Distinct().ToList() ;
                return orderedcateg;
            }


        }

        public static List<string> GetAllSubCategories(string MainCategory)
        {
            using (var db = new entities.SchoolEntities())
            {
                List<string> category = (from c in db.MainCategories
                                         join ct in db.Course_Times on c.CourseID
                                             equals ct.COURSEID
                                         join cc in db.Courses on c.CourseID equals cc.COURSEID
                                         join sub in db.SubCategories on c.MainCategoryID equals sub.MainCategoryID
                                         where c.MainCategory1 != "~ZZZZZZ~Membership" && c.MainCategory1==MainCategory && ct.COURSEDATE >= (System.Data.Entity.DbFunctions.AddDays(DateTime.Today, ((cc.viewpastcoursesdays == null ? 0 : -cc.viewpastcoursesdays))))
                                         select sub.SubCategory1).Distinct().ToList();

                return category;
            }


        }

        public static List<string> GetRosterByCouponCode(string couponcode)
        {
            using (var db = new entities.SchoolEntities())
            {
                List<string> rosterids = (from c in db.Course_Rosters where c.CouponCode==couponcode || c.SingleRosterDiscountCoupon == couponcode select c.CouponCode).ToList();
                return rosterids;
            }


        }

        public static List<string> GetRosterByCouponCode(string couponcode,int studentid)
        {
            using (var db = new entities.SchoolEntities())
            {
                List<string> rosterids = (from c in db.Course_Rosters where (c.CouponCode == couponcode || c.SingleRosterDiscountCoupon==couponcode) && c.STUDENTID == studentid select c.CouponCode).ToList();
                return rosterids;
            }


        }

        public static List<CalendarModel> GetCalendarEntries()
        {
            List<CalendarModel> dataHolder = new List<CalendarModel>();
            try
            {
                int counter = 0;
                int? cid = 0;
                string onlinesingleDate = string.Empty;
                int sequenceid = 0;
                using (var db = new entities.SchoolEntities())
                {

                    CalendarModel cmodel = new CalendarModel();
                    List<CalendarModel> query = new List<CalendarModel>();
                    //var course = (from  c in db.Courses
                    //              //join ct in db.Course_Times on c.COURSEID equals ct.COURSEID
                    //              where c.CANCELCOURSE == 0 && c.InternalClass == 0 && c.OnlineCourse !=0 
                    //              orderby c.COURSEID
                    //              select c).ToList();

                    //foreach (var id in course)
                    //{
                    IEnumerable<CalendarModel> cmodelList = (from ct in db.Course_Times
                                                            //where ct.COURSEID == id.COURSEID
                                                            join c in db.Courses on ct.COURSEID equals c.COURSEID
                                                            where c.CANCELCOURSE == 0 && c.InternalClass == 0
                                                            orderby ct.COURSEDATE ascending
                                                            select new CalendarModel()
                                                            {
                                                                id = c.COURSEID,
                                                                cid = 3,
                                                                title = c.COURSENAME,
                                                                startdate = ct.COURSEDATE.Value,
                                                                enddate = ct.COURSEDATE.Value,
                                                                loc = c.LOCATION + "," + c.STREET + ", " + c.CITY + ", " + c.STATE + " " + c.ZIP,
                                                                note = c.DESCRIPTION,
                                                                starttime = ct.STARTTIME,
                                                                endtime = ct.FINISHTIME.Value,
                                                                OnlineCourse = c.OnlineCourse,
                                                                StartEndTimeDisplay = c.StartEndTimeDisplay,
                                                                EventInternalClass = (from eic in db.Courses where eic.COURSEID == c.eventid select eic.InternalClass).FirstOrDefault(),
                                                                ctype = c.coursetype
                                                            }).ToList();
                    foreach (var m in cmodelList)
                    {
                        if (m.EventInternalClass == 0)
                        {
                            query.Add(m);
                        }
                    }

                    // };
                    if (query != null)
                    {
                        foreach (var dta in query)
                        {
                            if (dta != null)
                            {
                                if ((dta.startdate >= DateTime.Now.AddDays(-365)))
                                {
                                    sequenceid = sequenceid + 1;
                                    string startdatetime = dta.startdate.Value.ToShortDateString() + " " + dta.starttime.Value.ToShortTimeString();
                                    string enddatetime = dta.startdate.Value.ToShortDateString() + " " + dta.endtime.Value.ToShortTimeString();
                                    cmodel.id = sequenceid;
                                    cmodel.cid = dta.id;
                                    cmodel.ctype = dta.ctype;
                                    cmodel.title = GetTimeZone() + " " + dta.title;
                                   
                                    cmodel.OnlineCourse = dta.OnlineCourse;
                                    cmodel.StartEndTimeDisplay = dta.StartEndTimeDisplay;
                                    // take out timezone for now and will revisit this later
                                    if (Settings.Instance.GetMasterInfo3().system_timezone_hour != 999)
                                    {
                                        cmodel.startdate = DateTime.Parse(startdatetime);
                                        cmodel.enddate = DateTime.Parse(enddatetime);
                                    }
                                    else
                                    {
                                        cmodel.startdate = DateTime.Parse(startdatetime);
                                        cmodel.enddate = DateTime.Parse(enddatetime);
                                    }

                                    cmodel.loc = dta.loc.Replace(", ,", "");
                                    //cmodel.note = dta.note;
                                    cmodel.starttime = dta.starttime;
                                    cmodel.endtime = dta.endtime;
                                    //cmodel.rem = dta.id.Value.ToString();
                                    cmodel.stringstartdate = cmodel.startdate.ToString();
                                    cmodel.stringenddate = cmodel.enddate.ToString();
                                    cmodel.enddate = dta.enddate;
                                    if (dta.OnlineCourse != 0)
                                    {
                                        cmodel.stringenddate = cmodel.startdate.ToString();
                                        cmodel.enddate = dta.startdate;
                                        cmodel.endtime = cmodel.starttime;
                                        // THIS WILL REMOVE THE 2nd DATE TO APPEND ON THE CALENDAR
                                        // LOAD THE SECOND DATE ON THE TOOLTIP INSTEAD BY cmodel.OnlineDateStartEnd
                                        var data = (from cd in db.Course_Times where cd.COURSEID == dta.id select cd).OrderBy(ct => ct.COURSEDATE).ToList();

                                        if (data.Count() > 0)
                                        {
                                            startdatetime = data.First().COURSEDATE.Value.ToShortDateString() + " " + data.First().STARTTIME.Value.ToShortTimeString();
                                            enddatetime = data.First().COURSEDATE.Value.ToShortDateString() + " " + data.First().FINISHTIME.Value.ToShortTimeString();

                                            cmodel.starttime = data.First().STARTTIME.Value;
                                            cmodel.endtime = data.First().FINISHTIME.Value;
                                            cmodel.stringstartdate = startdatetime;
                                            cmodel.stringenddate = enddatetime;
                                            cmodel.enddate = data.First().COURSEDATE.Value;
                                        }

                                        foreach (var dates in data)
                                        {
                                            //IF ONLINE COURSE, LOAD THE DATES IN 1 VARIABLE TO BE USED ON THE TOOLTIP
                                            string start_date = DateTime.Parse(dates.COURSEDATE.Value.ToShortDateString() + " " + dates.STARTTIME.Value.ToShortTimeString()).ToString();
                                            onlinesingleDate += start_date + "<br />";
                                            if (cmodel.ctype == 1)
                                            {
                                               
                                                onlinesingleDate = cmodel.stringstartdate;
                                 
                                            }
                                        }
                                        if (cmodel.ctype == 1)
                                        {
                                            cmodel.StartEndTimeDisplay =DateTime.Parse( onlinesingleDate).ToShortTimeString();

                                        }
                                        cmodel.OnlineDateStartEnd = onlinesingleDate != "" ? onlinesingleDate : cmodel.stringstartdate;
                                        if (data.Count() == 1 || counter == 0)
                                        {
                                            if (data.Count() == 1)
                                            {
                                                counter = 0;
                                            }
                                            else
                                            {
                                                counter++;
                                            }
                                            if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype == 1).Count() <= 0)
                                            dataHolder.Add(cmodel);
                                        }
                                        else
                                        {
                                            counter = 0;
                                        }
                                    }
                                    else
                                    {
                                        cmodel.OnlineDateStartEnd = cmodel.stringstartdate;
                                        if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype == 1).Count() <= 0)
                                            dataHolder.Add(cmodel);
                                    }
                                }
                            }
                            cmodel = new CalendarModel();
                            onlinesingleDate = "";
                        }
                    }

                    IEnumerable<@event> independentEvents = Gsmu.Api.Data.Events.Queries.GetAllEvents();
                    if (independentEvents != null)
                    {
                        foreach (@event sevent in independentEvents)
                        {
                            cmodel = new CalendarModel();
                            sequenceid = sequenceid + 1;
                            string startdatetime = sevent.DateStart;
                            string enddatetime = sevent.DateEnd;
                            cmodel.id = sequenceid;
                            cmodel.cid = sevent.eventid;
                            cmodel.title = GetTimeZone() + " " + sevent.title + "&nbsp;&nbsp;";
                            // take out timezone for now and will revisit this later
                            if (Settings.Instance.GetMasterInfo3().system_timezone_hour != 999)
                            {
                                cmodel.startdate = DateTime.Parse(startdatetime);
                                cmodel.enddate = DateTime.Parse(enddatetime);
                            }
                            else
                            {
                                cmodel.startdate = DateTime.Parse(startdatetime);
                                cmodel.enddate = DateTime.Parse(enddatetime);
                            }

                            cmodel.starttime = DateTime.Parse(startdatetime);
                            cmodel.endtime = DateTime.Parse(enddatetime);
                            cmodel.stringstartdate = startdatetime;
                            if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype == 1).Count() <= 0)
                            dataHolder.Add(cmodel);

                            for (int x = 1; x < (DateTime.Parse(enddatetime) - DateTime.Parse(startdatetime)).TotalDays; x++)
                            {
                                cmodel = new CalendarModel();
                                sequenceid = sequenceid + x;
                                cmodel.id = sequenceid;
                                cmodel.cid = sevent.eventid;
                                cmodel.title = GetTimeZone() + " " + sevent.title + "&nbsp;&nbsp;";
                                cmodel.enddate = DateTime.Parse(enddatetime);
                                cmodel.startdate = DateTime.Parse(startdatetime).AddDays(x);
                                cmodel.starttime = DateTime.Parse(startdatetime);
                                cmodel.endtime = DateTime.Parse(enddatetime);
                                cmodel.stringstartdate = cmodel.startdate.ToString();
                                if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype ==1).Count() <= 0)
                                dataHolder.Add(cmodel);
                            }


                        }
                    }
                    return dataHolder.Distinct().ToList();
                }
            }catch(Exception ex)
            {
                return dataHolder.Distinct().ToList();
            }
        }

        public static List<CalendarModel> GetCalendarEntries(string maincategory,string subcategory)
        {
            int counter = 0;
            string onlinesingleDate = string.Empty;
            List<CalendarModel> dataHolder = new List<CalendarModel>();
            using (var db = new entities.SchoolEntities())
            {

                CalendarModel cmodel = new CalendarModel();
                List<CalendarModel> query = new List<CalendarModel>();
                
                var course = (from 
                               c in db.Courses 
                              join mc in db.MainCategories on c.COURSEID equals mc.CourseID
                              where  c.CANCELCOURSE == 0 && mc.MainCategory1 == maincategory && c.InternalClass==0
                              select c).ToList();

                if (subcategory != "")
                {
                    course = (from
                               c in db.Courses
                              join mc in db.MainCategories on c.COURSEID equals mc.CourseID
                              join sub in db.SubCategories on mc.MainCategoryID equals sub.MainCategoryID 
                              where c.CANCELCOURSE == 0 && mc.MainCategory1 == maincategory && c.InternalClass == 0 && sub.SubCategory1 ==subcategory
                              select c).ToList();
                }

                foreach (var id in course.Distinct())
                {
                    IEnumerable<CalendarModel> cmodelList = (from ct in db.Course_Times
                                                             where ct.COURSEID == id.COURSEID
                                                             select new CalendarModel()
                                                             {

                                                                 id = ct.COURSEID,
                                                                 cid = 3,
                                                                 title = id.COURSENAME,
                                                                 startdate = ct.COURSEDATE.Value,
                                                                 enddate = ct.COURSEDATE.Value,
                                                                 loc = id.LOCATION + ", " + id.STREET+ ", "+id.CITY+", "+id.STATE+" "+id.ZIP,
                                                                 note = id.DESCRIPTION,
                                                                 starttime = ct.STARTTIME.Value,
                                                                 endtime = ct.FINISHTIME.Value,
                                                                 OnlineCourse = id.OnlineCourse,
                                                                 EventInternalClass = (from eic in db.Courses where eic.COURSEID == id.eventid select eic.InternalClass).FirstOrDefault()


                                                             }).ToList().OrderBy(ct => ct.startdate);

                    foreach (var m in cmodelList)
                    {

                        
                        if (m.EventInternalClass == 0) 
                        {
                            query.Add(m);

                        }
                    }
                };
                if (query != null)
                {
                    int sequenceid = 0;
                    foreach (var dta in query)
                    {
                        if (dta != null)
                        {
                            sequenceid = sequenceid + 1;
                            string startdatetime = dta.startdate.Value.ToShortDateString() + " " + dta.starttime.Value.ToShortTimeString();
                            string enddatetime = dta.startdate.Value.ToShortDateString() + " " + dta.endtime.Value.ToShortTimeString();
                            string title =  dta.title;
                            cmodel.id = sequenceid;
                            cmodel.cid = dta.id;
                            cmodel.title = GetTimeZone() + " " + title;
                            cmodel.startdate = DateTime.Parse(startdatetime);
                            cmodel.enddate = DateTime.Parse(enddatetime);
                            cmodel.loc = dta.loc.Replace(", ,","");
                            cmodel.note = dta.note;
                            cmodel.OnlineCourse = dta.OnlineCourse;
                            // take out timezone for now and will revisit this later
                            if (Settings.Instance.GetMasterInfo3().system_timezone_hour != 999)
                            {
                                //cmodel.startdate = DateTime.Parse(startdatetime).AddHours(Settings.Instance.GetMasterInfo3().system_timezone_hour.Value);
                                //cmodel.enddate = DateTime.Parse(enddatetime).AddHours(Settings.Instance.GetMasterInfo3().system_timezone_hour.Value);
                                cmodel.startdate = DateTime.Parse(startdatetime);
                                cmodel.enddate = DateTime.Parse(enddatetime);
                            }
                            else
                            {
                                cmodel.startdate = DateTime.Parse(startdatetime);
                                cmodel.enddate = DateTime.Parse(enddatetime);
                            }
                            cmodel.rem = dta.id.Value.ToString();
                            cmodel.stringstartdate = cmodel.startdate.ToString();
                            cmodel.enddate = dta.enddate;
                            if (dta.OnlineCourse != 0)
                            {
                                // THIS WILL REMOVE THE 2nd DATE TO APPEND ON THE CALENDAR
                                // LOAD THE SECOND DATE ON THE TOOLTIP INSTEAD BY cmodel.OnlineDateStartEnd
                                var data = (from cd in db.Course_Times where cd.COURSEID == dta.id select cd).OrderBy(ct => ct.COURSEDATE);
                                foreach (var dates in data)
                                {
                                    //IF ONLINE COURSE, LOAD THE DATES IN 1 VARIABLE TO BE USED ON THE TOOLTIP
                                    string start_date = DateTime.Parse(dates.COURSEDATE.Value.ToShortDateString() + " " + dates.STARTTIME.Value.ToShortTimeString()).ToString();
                                    onlinesingleDate += start_date + "<br />";
                                }
                                if (cmodel.ctype == 1)
                                {
                                    onlinesingleDate = cmodel.stringstartdate;
                                    cmodel.StartEndTimeDisplay = cmodel.stringstartdate.ToString();
                                    
                                }
                               
                                cmodel.OnlineDateStartEnd = onlinesingleDate != "" ? onlinesingleDate : cmodel.stringstartdate;
                                if (counter == 0)
                                {
                                    if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype == 1).Count() <= 0) 
                                        dataHolder.Add(cmodel);
                                    counter++;
                                }
                                else
                                {
                                    counter = 0;
                                }
                            }
                            else
                            {
                                cmodel.OnlineDateStartEnd = cmodel.stringstartdate;
                                if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && cmodel.startdate != cmodel.enddate)
                                dataHolder.Add(cmodel);
                            }
                        }
                        cmodel = new CalendarModel();
                        onlinesingleDate = "";
                    }
                }
                return dataHolder.Distinct().ToList();
            }
        }
        public static List<CalendarModel> GetCalendarEntries(int studentid)
        {
            int counter = 0;
            string onlinesingleDate = string.Empty;
            List<CalendarModel> dataHolder = new List<CalendarModel>();
            using (var db = new entities.SchoolEntities())
            {

                CalendarModel cmodel = new CalendarModel();
                List<CalendarModel> query = new List<CalendarModel>();
                var course = (from cr in db.Course_Rosters
                              join c in db.Courses on cr.COURSEID equals c.COURSEID
                              // join ct in db.Course_Times on cr.COURSEID equals ct.COURSEID
                              where cr.STUDENTID == studentid && cr.Cancel == 0 && c.InternalClass ==0
                              select c).ToList();

                foreach (var id in course)
                {
                    IEnumerable<CalendarModel> cmodelList = (from ct in db.Course_Times
                                                             where ct.COURSEID == id.COURSEID
                                                             select new CalendarModel()
                                                             {
                                                                 id = ct.COURSEID,
                                                                 cid = 3,
                                                                 title = id.COURSENAME,
                                                                 startdate = ct.COURSEDATE.Value,
                                                                 enddate = ct.COURSEDATE.Value,
                                                                 loc = id.LOCATION + "," + id.STREET + ", " + id.CITY + ", " + id.STATE + " " + id.ZIP,
                                                                 note = id.DESCRIPTION,
                                                                 starttime = ct.STARTTIME,
                                                                 endtime = ct.FINISHTIME.Value,
                                                                 OnlineCourse = id.OnlineCourse,
                                                                 EventInternalClass = (from eic in db.Courses where eic.COURSEID == id.eventid select eic.InternalClass).FirstOrDefault()



                                                             }).ToList().OrderBy(ct => ct.startdate);
                    foreach (var m in cmodelList)
                    {
                        if (m.EventInternalClass == 0)
                        {
                            query.Add(m);
                        }
                    }
                };
                if (query != null)
                {
                    int sequenceid = 0;
                    foreach (var dta in query.Distinct())
                    {
                        if (dta != null)
                        {
                            sequenceid = sequenceid + 1;
                            string startdatetime = dta.startdate.Value.ToShortDateString() + " " + dta.starttime.Value.ToShortTimeString();
                            string enddatetime = dta.startdate.Value.ToShortDateString() + " " + dta.endtime.Value.ToShortTimeString();
                            string title =  dta.title;
                            cmodel.id = sequenceid;
                            cmodel.cid = dta.id;
                            cmodel.title = GetTimeZone() + " " + title;

                            // take out timezone for now and will revisit this later
                                cmodel.startdate = DateTime.Parse(startdatetime);
                                cmodel.enddate = DateTime.Parse(enddatetime);


                            cmodel.loc = dta.loc.Replace(", ,", "");
                            cmodel.note = dta.note;
                            cmodel.starttime = dta.starttime;
                            cmodel.endtime = dta.endtime;
                            cmodel.rem = dta.id.Value.ToString();
                            cmodel.stringstartdate = cmodel.startdate.ToString();
                            cmodel.OnlineCourse = dta.OnlineCourse;
                            cmodel.enddate = dta.enddate;
                            if (dta.OnlineCourse != 0)
                            {
                                // THIS WILL REMOVE THE 2nd DATE TO APPEND ON THE CALENDAR
                                // LOAD THE SECOND DATE ON THE TOOLTIP INSTEAD BY cmodel.OnlineDateStartEnd
                                var data = (from cd in db.Course_Times where cd.COURSEID == dta.id select cd).OrderBy(ct => ct.COURSEDATE);
                                foreach (var dates in data)
                                {
                                    //IF ONLINE COURSE, LOAD THE DATES IN 1 VARIABLE TO BE USED ON THE TOOLTIP
                                    string start_date = DateTime.Parse(dates.COURSEDATE.Value.ToShortDateString() + " " + dates.STARTTIME.Value.ToShortTimeString()).ToString();
                                    onlinesingleDate += start_date + "<br />";
                                }
                                if (cmodel.ctype == 1)
                                {
                                    onlinesingleDate = cmodel.stringstartdate;
                                    cmodel.StartEndTimeDisplay = cmodel.stringstartdate.ToString();
                                  
                                }
                                cmodel.OnlineDateStartEnd = onlinesingleDate != "" ? onlinesingleDate : cmodel.stringstartdate;

                                if (counter == 0)
                                {
                                    if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype == 1).Count() <= 0) 
                                        dataHolder.Add(cmodel);
                                    counter++;
                                }
                                else
                                {
                                    counter = 0;
                                }
                            }
                            else
                            {
                                cmodel.OnlineDateStartEnd = cmodel.stringstartdate;
                                if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && cmodel.startdate != cmodel.enddate)
                                dataHolder.Add(cmodel);
                            }
                        }
                        cmodel = new CalendarModel();
                        onlinesingleDate = "";
                    }
                }


                return dataHolder;
            }
        }

        public static List<CalendarModel> GetCalendarEntries(int studentid,string maincategory)
        {
            int counter = 0;
            string onlinesingleDate = string.Empty;
            List<CalendarModel> dataHolder = new List<CalendarModel>();
            using (var db = new entities.SchoolEntities())
            {

                CalendarModel cmodel = new CalendarModel();
                List<CalendarModel> query = new List<CalendarModel>();
                var course = (from cr in db.Course_Rosters
                              join c in db.Courses on cr.COURSEID equals c.COURSEID
                               join mc in db.MainCategories on cr.COURSEID equals mc.CourseID
                              where cr.STUDENTID == studentid && cr.Cancel == 0 && mc.MainCategory1==maincategory && c.InternalClass == 0
                              select c).ToList();

                foreach (var id in course)
                {
                    IEnumerable<CalendarModel> cmodelList = (from ct in db.Course_Times
                                                             where ct.COURSEID == id.COURSEID
                                                             select new CalendarModel()
                                                             {
                                                                 id = ct.COURSEID,
                                                                 cid = 3,
                                                                 title = id.COURSENAME,
                                                                 startdate = ct.COURSEDATE.Value,
                                                                 enddate = ct.COURSEDATE.Value,
                                                                 loc = id.LOCATION + "," + id.STREET + ", " + id.CITY + ", " + id.STATE + " " + id.ZIP,
                                                                 note = id.DESCRIPTION,
                                                                 starttime = ct.STARTTIME,
                                                                 endtime = ct.FINISHTIME.Value,
                                                                 OnlineCourse = id.OnlineCourse,
                                                                 EventInternalClass = (from eic in db.Courses where eic.COURSEID == id.eventid select eic.InternalClass).FirstOrDefault()


                                                             }).ToList().OrderBy(ct => ct.startdate);
                    foreach (var m in cmodelList)
                    {
                        if (m.EventInternalClass == 0)
                        {
                            query.Add(m);
                        }
                    }
                };
                if (query != null)
                {
                    int sequenceid = 0;
                    foreach (var dta in query)
                    {
                        if (dta != null)
                        {
                            sequenceid = sequenceid + 1;
                            string startdatetime = dta.startdate.Value.ToShortDateString() + " " + dta.starttime.Value.ToShortTimeString();
                            string enddatetime = dta.startdate.Value.ToShortDateString() + " " + dta.endtime.Value.ToShortTimeString();
                            string title =  dta.title;
                            cmodel.id = sequenceid;
                            cmodel.cid = dta.id;
                            cmodel.title = GetTimeZone() + " " + title;
                            cmodel.startdate = DateTime.Parse(startdatetime);
                            cmodel.enddate = DateTime.Parse(enddatetime);
                            cmodel.loc = dta.loc.Replace(", ,", "");
                            cmodel.note = dta.note;
                            cmodel.OnlineCourse = dta.OnlineCourse;
                            // take out timezone for now and will revisit this later
                                cmodel.startdate = DateTime.Parse(startdatetime);
                                cmodel.enddate = DateTime.Parse(enddatetime);
                           
                            cmodel.rem = dta.id.Value.ToString();
                            cmodel.stringstartdate = cmodel.startdate.ToString();
                            cmodel.OnlineDateStartEnd = onlinesingleDate != "" ? onlinesingleDate : cmodel.stringstartdate;
                            cmodel.enddate = dta.enddate;
                            if (dta.OnlineCourse != 0)
                            {
                                // THIS WILL REMOVE THE 2nd DATE TO APPEND ON THE CALENDAR
                                // LOAD THE SECOND DATE ON THE TOOLTIP INSTEAD BY cmodel.OnlineDateStartEnd
                                var data = (from cd in db.Course_Times where cd.COURSEID == dta.id select cd).OrderBy(ct => ct.COURSEDATE);
                                foreach (var dates in data)
                                {
                                    //IF ONLINE COURSE, LOAD THE DATES IN 1 VARIABLE TO BE USED ON THE TOOLTIP
                                    string start_date = DateTime.Parse(dates.COURSEDATE.Value.ToShortDateString() + " " + dates.STARTTIME.Value.ToShortTimeString()).ToString();
                                    onlinesingleDate += start_date + "<br />";
                                }
                                if (cmodel.ctype == 1)
                                {
                                    onlinesingleDate = cmodel.stringstartdate;
                                    cmodel.StartEndTimeDisplay = cmodel.stringstartdate.ToString();
                                }
                                cmodel.OnlineDateStartEnd = onlinesingleDate != "" ? onlinesingleDate : cmodel.stringstartdate;
                                if (counter == 0)
                                {
                                    if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && dataHolder.Where(a => a.cid == cmodel.cid && a.ctype == 1).Count() <= 0)
                                    dataHolder.Add(cmodel);
                                     counter++;
                                }
                                else
                                {
                                    counter = 0;
                                }
                            }
                            else
                            {
                                cmodel.OnlineDateStartEnd = cmodel.stringstartdate;
                                if (dataHolder.Where(a => a.cid == cmodel.cid && a.stringstartdate == cmodel.stringstartdate).Count() <= 0 && cmodel.startdate != cmodel.enddate)
                                dataHolder.Add(cmodel);
                            }
                        }
                        cmodel = new CalendarModel();
                        onlinesingleDate = "";
                    }
                }
                return dataHolder.Distinct().ToList();
            }
        }

    }
}
