using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.CourseRoster
{
    public static class ClassList
    {
        public static string UpdateRosterDetails(int rosterid, string needs, string notes, string invoice, string invoicedate, string recordcount)
        {
            using (var db = new SchoolEntities())
            {
                var _roster = (from roster in db.Course_Rosters where roster.RosterID == rosterid select roster).FirstOrDefault();
                if (_roster != null)
                {
                    _roster.CheckoutComments = needs;
                    _roster.InvoiceDate = DateTime.Parse(String.Format("{0:MM/dd/yyyy}", invoicedate));
                    _roster.InvoiceNumber = invoice;
                    _roster.WaitOrder = Int16.Parse(recordcount.Replace("count=", ""));
                    var _student = (from student in db.Students where student.STUDENTID == _roster.STUDENTID select student).FirstOrDefault();
                    _student.NOTES = notes;
                    db.SaveChanges();
                }
            }
            return "";
        }
        public static Classlist_Result GetClasslist(int? cid, int start, int limit, string filter, string sort, string waiting, int cancel)
        {
            List<Classlist_Object> ObjClasslist = new List<Classlist_Object>();
            Classlist_Object ClassObjct = new Classlist_Object();
            int counter = 0;
            int totalcount = 0;
            string isCanvasSync = "";
            var filterResult = ExtJsDataStoreHelper.ParseFilter(filter);
            var sorterResult = ExtJsDataStoreHelper.ParseSorterUnique(sort);
            var queryState = new QueryState(start, limit)
            {
                OrderByDirection = sorterResult.Value,
                OrderFieldString = sorterResult.Key,
                Filters = filterResult
            };
            using (var db = new SchoolEntities())
            {
                var raw_class = (from cep in db.Course_Rosters where cep.COURSEID == cid && cep.Cancel == 0 && cep.WAITING == 0 select cep);
                if (waiting == "1")
                {
                    raw_class = (from cep in db.Course_Rosters where cep.COURSEID == cid && cep.Cancel == 0 && (cep.WAITING == 1 || cep.WAITING == -1) select cep);

                }
                if (cancel == 1)
                {
                    raw_class = (from cep in db.Course_Rosters where cep.COURSEID == cid && cep.Cancel != 0 select cep);
                }



                if (queryState.Filters != null)
                {
                    if (queryState.Filters.ContainsKey("keyword"))
                    {
                        var keyword = queryState.Filters["keyword"];
                        raw_class = (from cep in db.Course_Rosters join s in db.Students on cep.STUDENTID equals s.STUDENTID where (s.FIRST.Contains(keyword) || s.LAST.Contains(keyword)) && cep.COURSEID == cid && cep.Cancel == 0 && cep.WAITING == 0 select cep);
                    }
                }
                try
                {
                    totalcount = raw_class.ToList().Count();
                }
                catch { }
                if (waiting == "1")
                {
                    raw_class = raw_class.OrderBy(e => e.WaitOrder).ThenBy(e => e.RosterID);
                }
                else
                {
                    raw_class = raw_class.OrderBy(e => e.DATEADDED);
                }
                var model = new GridModel<Course_Roster>(raw_class.Count(), queryState);
                // raw_class = model.Paginate(raw_class);
                try
                {
                    //  totalcount = raw_class.ToList().Count();
                    model.Result = raw_class.ToList();
                }
                catch
                {
                }


                foreach (var c in model.Result)
                {
                    counter = counter + 1 + start;
                    ClassObjct.count = counter;

                    if (waiting == "1")
                    {
                        try
                        {

                            ClassObjct.count = short.Parse(counter.ToString());
                        }
                        catch { }
                    }
                    ClassObjct.rosterId = c.RosterID;
                    ClassObjct.DTAddedDisplay = String.Format("{0:d}", c.DATEADDED) + " " + String.Format("{0:t}", c.TIMEADDED);
                    ClassObjct.DateTimeAdded = Convert.ToDateTime(String.Format("{0:d}", c.DATEADDED) + " " + String.Format("{0:t}", c.TIMEADDED));
                    ClassObjct.DateTimeCancelled = String.Format("{0:d}", c.CancelDate) + " " + String.Format("{0:t}", c.CancelDate);
                    //ClassObjct.DatetimeForSort = c.DATEADDED;
                    ClassObjct.DatetimeForSort = Convert.ToDateTime(String.Format("{0:d}", c.DATEADDED) + " " + String.Format("{0:t}", c.TIMEADDED));
                    //ClassObjct.DateTimeAdded = c.DATEADDED.ToString();
                    var student = (from s in db.Students where s.STUDENTID == c.STUDENTID select s).FirstOrDefault();
                    if (student != null)
                    {
                        if (Gsmu.Api.Integration.Canvas.Configuration.Instance.ExportEnrollmentAfterCheckout && c.canvas_roster_id != null) {
                            if (c.canvas_roster_id > 0)
                            {
                                isCanvasSync = "<img src='/images/icons/gsmu/canvas.png'>";
                            } else
                            {
                                isCanvasSync = "<img src='/images/icons/gsmu/canvasoff.png'>";
                            }
                        }
                        ClassObjct.StudentName = isCanvasSync + " " + student.LAST + ", " + student.FIRST;
                        isCanvasSync = "";
                        ClassObjct.school = (from school in db.Schools where school.locationid == student.SCHOOL select school.LOCATION).FirstOrDefault();
                    }

                    if (c.CourseHoursType == "CH")
                    {
                        ClassObjct.Payhours = Settings.Instance.GetMasterInfo2().CreditHoursName;
                    }
                    else if (c.CourseHoursType == "ISH")
                    {
                        ClassObjct.Payhours = Settings.Instance.GetMasterInfo2().InserviceHoursName;
                    }
                    else if (c.CourseHoursType == "NONE")
                    {
                        ClassObjct.Payhours = "NONE";
                    }
                    else
                    {
                        ClassObjct.Payhours = Settings.Instance.GetMasterInfo2().CreditHoursName + " & " + Settings.Instance.GetMasterInfo2().InserviceHoursName;
                    }

                    ClassObjct.SpecialNeeds = c.CheckoutComments;

                    DateTime temp;
                    DateTime temp2;
                    string d1 = "N/A";
                    string d2 = "N/A";
                    if (DateTime.TryParse(c.ReminderSent.ToString(), out temp))
                    {
                        d1 = temp.ToShortDateString();
                    }
                    if (DateTime.TryParse(c.Reminder2Sent.ToString(), out temp2))
                    {
                        d2 = temp2.ToShortDateString();
                    }

                    ClassObjct.Reminders = "1: " + d1 + "<br/>" + "2: " + d2;
                    ClassObjct.Notes = student.NOTES;
                    ClassObjct.Invoice = c.InvoiceNumber;
                    try
                    {
                        ClassObjct.Invoicedate = c.InvoiceDate.ToString();
                    }
                    catch
                    {
                        ClassObjct.Invoicedate = "";
                    }
                    ClassObjct.cancel = "<input type='checkbox' name='' value='' onclick='Classlist.Cancel(" + c.RosterID + ")'>";
                    ClassObjct.enroll = "<input type='checkbox' name='' value='' onclick='Classlist.Enroll(" + c.RosterID + ")'>";
                    ClassObjct.studentid = c.STUDENTID.Value;

                    ObjClasslist.Add(ClassObjct);
                    ClassObjct = new Classlist_Object();
                    counter = counter - start;
                }

                if (queryState.OrderFieldString != null)
                {
                    switch (queryState.OrderFieldString)
                    {
                        case "StudentName":
                            if (queryState.OrderByDirection == OrderByDirection.Ascending)
                                ObjClasslist = ObjClasslist.OrderBy(o => o.StudentName).ToList();
                            else
                                ObjClasslist = ObjClasslist.OrderByDescending(o => o.StudentName).ToList();
                            break;
                        case "school":
                            if (queryState.OrderByDirection == OrderByDirection.Ascending)
                                ObjClasslist = ObjClasslist.OrderBy(o => o.school).ToList();
                            else
                                ObjClasslist = ObjClasslist.OrderByDescending(o => o.school).ToList();
                            break;
                        case "DateTimeAdded":
                            if (queryState.OrderByDirection == OrderByDirection.Ascending)
                                ObjClasslist = ObjClasslist.OrderBy(o => o.DatetimeForSort).ToList();
                            else
                                ObjClasslist = ObjClasslist.OrderByDescending(o => o.DatetimeForSort).ToList();
                            break;
                        case "DateTimeCancelled":
                            if (queryState.OrderByDirection == OrderByDirection.Ascending)
                                ObjClasslist = ObjClasslist.OrderBy(o => o.DateTimeCancelled).ToList();
                            else
                                ObjClasslist = ObjClasslist.OrderByDescending(o => o.DateTimeCancelled).ToList();
                            break;
                        default:
                            ObjClasslist = ObjClasslist.OrderBy(o => o.StudentName).ToList();
                            break;
                    }
                }

            }
            var objclasslist = ObjClasslist.AsQueryable();
            var ObjModelList = new GridModel<List<Classlist_Object>>(objclasslist.Count(), queryState);
            objclasslist = ObjModelList.Paginate(objclasslist);


            Classlist_Result _Classlist_Result = new Classlist_Result();
            _Classlist_Result.ClassList = objclasslist.ToList();
            _Classlist_Result.PageSize = 10;
            _Classlist_Result.TotalCount = totalcount;
            _Classlist_Result.TotalPages = 2;
            return _Classlist_Result;
        }
        public static void CancelStudent(int courseid, int studentid)
        {
        }

    }

    public class Classlist_Object
    {
        public int count { get; set; }
        public int rosterId { get; set; }
        public int studentid { get; set; }
        public string cancel { get; set; }
        public string enroll { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public string Payhours { get; set; }
        public string StudentName { get; set; }
        public string Organization { get; set; }
        public string Reminders { get; set; }
        public string SpecialNeeds { get; set; }
        public string Notes { get; set; }
        public string Invoice { get; set; }
        public string Invoicedate { get; set; }
        public string ExtraParticipants { get; set; }
        public string school { get; set; }
        public string DateTimeCancelled { get; set; }
        public string DTAddedDisplay { set; get; }
        public DateTime? DatetimeForSort { get; set; }


    }
    public class Classlist_Result
    {
        public List<Classlist_Object> ClassList { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

}
