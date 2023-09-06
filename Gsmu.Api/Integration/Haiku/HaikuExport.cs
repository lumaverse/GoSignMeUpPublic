using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;
using net = System.Net;
using xml = System.Xml;
using lang = Gsmu.Api.Language;
using entities = Gsmu.Api.Data.School.Entities;
using student = Gsmu.Api.Data.School.Student;
using log = Gsmu.Api.Logging;
using school = Gsmu.Api.Data.School;

namespace Gsmu.Api.Integration.Haiku
{
    public static class HaikuExport
    {
        public static Response SynchronizeOrder(string orderNumber, entities.SchoolEntities db = null, Response response = null)
        {
            response = response ?? new Response();
            db = db ?? new entities.SchoolEntities();

            var synchronizedCourses = new List<int>();

            var rosters = (from cr in db.Course_Rosters where cr.OrderNumber == orderNumber && (cr.PaidInFull != 0)  && (cr.Cancel == 0) select cr).ToList();

            foreach (var roster in rosters)
            {
                var student = (from s in db.Students where s.STUDENTID == roster.STUDENTID select s).FirstOrDefault();
                var course = (from c in db.Courses where c.COURSEID == roster.COURSEID select c).FirstOrDefault();
                if (roster.COURSEID.HasValue && student != null && course != null)
                {
                    //check single course settings for haiku integration
                    if (course.disablehaikuintegration.Value != 1)
                    {
                        if (!synchronizedCourses.Contains(course.COURSEID))
                        {
                            var result = SynchronizeCourse(roster.COURSEID.Value);
                            course = result.Item2;
                        }

                        if (course.haiku_course_id.HasValue)
                        {
                            SynchronizeStudent(student, db, response);

                            var deleteRosterResponse = DeleteRoster(course.haiku_course_id.Value, student.haiku_user_id);
                            response.JoinErrors(deleteRosterResponse);
                            response = AddRoster(course.haiku_course_id.Value, GsmuEntityType.Student, student.haiku_user_id, roster, response);
                            
                        }

                    }
               }
            }
            db.SaveChanges();

            return response;
        }

        public static Response DeleteRoster(int haikuCourseId, int haikuUserId, bool disregard404Error = true)
        {
            var response = HaikuClient.GetResponse(
                            string.Format("class/{0}/roster/{1}", haikuCourseId, haikuUserId),
                            http.HttpMethod.Delete,
                            disregard404Error
                        );
            return response;
        }

        public static Response SynchronizeRoster(int courseId, entities.SchoolEntities db = null, Response response = null)
        {
            response = response ?? new Response();

            db = db ?? new entities.SchoolEntities();

            var result = SynchronizeCourse(courseId);
            var instructors = result.Item3;
            var mainHaikuTeacherId = instructors.First().haiku_user_id;
            var course = result.Item2;

            if (course.haiku_course_id.HasValue && Configuration.Instance.disableRosterNormalization == false)
            {
                var haikuRosterResponse = HaikuImport.GetRostersForCourse(course.haiku_course_id.Value);

                foreach (var roster in haikuRosterResponse.Rosters.AllRecords)
                {
                    if (roster.UserId !=0 && roster.UserId != mainHaikuTeacherId)
                    {
                        var deleteRosterResponse = HaikuClient.GetResponse(
                            string.Format("class/{0}/roster/{1}", roster.ClassId, roster.UserId),
                            http.HttpMethod.Delete
                        );
                        response.JoinErrors(deleteRosterResponse);
                    }
                }               
            }

            var rosters = (from cr in db.Course_Rosters where cr.COURSEID == courseId select cr).ToList();

            foreach (var instructor in instructors)
            {
                if (instructor.haiku_user_id != mainHaikuTeacherId)
                {
                    response = AddRoster(course.haiku_course_id.Value, GsmuEntityType.Instructor, instructor.haiku_user_id, null, response);
                }
            }
        

            foreach (var roster in rosters)
            {
                if (!roster.IsCancelled && roster.IsPaid)
                {
                    var student = (from s in db.Students where s.STUDENTID == roster.STUDENTID select s).FirstOrDefault();
                    if (student != null)
                    {
                        SynchronizeStudent(student, db, response);
                        response = AddRoster(course.haiku_course_id.Value, GsmuEntityType.Student, student.haiku_user_id, roster, response);
                    }
                }
            }

            course.haiku_last_integration_date = DateTime.Now;
            db.SaveChanges();

            return response;
        }

        private static Response AddRoster(int haikuClassId, GsmuEntityType importEntityType, int haikuUserId, entities.Course_Roster roster, Response response = null)
        {
            response = response ?? new Response();

            http.HttpMethod method = http.HttpMethod.Post;
            var url = string.Format("class/{0}/roster", haikuClassId);
            var query = new NameValueCollection();
            query["user_id"] = haikuUserId.ToString();

            switch (importEntityType)
            {
                case GsmuEntityType.Instructor:
                    query["role"] = "T";
                    break;

                case GsmuEntityType.Student:
                    query["role"] = "S";
                    break;

                default:
                    throw new Exception(
                        string.Format(
                            "Invalid Haiku Entity type for roster {0}.",
                            Gsmu.Api.Language.EnumHelper.GetEnumName(importEntityType)
                        )
                    );
            }
            var queryString = Gsmu.Api.Language.StringHelper.NameValueCollectionToQueryString(query);
            url = url + "?" + queryString;

            var rosterResponse = HaikuClient.GetResponse(url, method);
            if (roster != null && !rosterResponse.ContainsErrors)
            {
                roster.haiku_roster_id = rosterResponse.Rosters.FirstRoster.RosterId;
            }
            response.JoinErrors(rosterResponse);
            return response;

        }

        public static Response SynchronizeStudent(int userId)
        {
            using (var db = new entities.SchoolEntities())
            {
                var student = (from s in db.Students where s.STUDENTID == userId select s).FirstOrDefault();
                if (student != null)
                {
                    return SynchronizeStudent(student, db);
                }
                return null;
            }
        }

        public static Response SynchronizeStudent(entities.Student student, entities.SchoolEntities db = null, Response response = null)
        {
            db = db ?? new entities.SchoolEntities();

            http.HttpMethod method = http.HttpMethod.Post;
            string url = "user";

            var query = new NameValueCollection();

            query["import_id"] = Configuration.GetImportId(GsmuEntityType.Student, student.STUDENTID, student.EMAIL);
            if (student.haiku_user_id > 0)
            {
                method = http.HttpMethod.Put;
                url = string.Format("user/{0}", student.haiku_user_id);
                query["import_id"] = student.haiku_import_id;
            }
            //according to Haiku, no password, login=use email username, google_email=gmail must be in the request.
            if (student.STUDNUM.ToLower() != "maintained by haiku" && student.STUDNUM.ToLower() != "maintained by learning" && student.STUDNUM.ToLower() != "google assigned/maintains" && student.STUDNUM.IndexOf("haiku") == -1 && student.STUDNUM.IndexOf("powerschool") == -1 && student.STUDNUM.IndexOf("powerschool learning") == -1)
            {
                query["password"] = student.STUDNUM;
            }
            if (student.google_user == 1)
            {
                query["google_email"] = student.EMAIL;
                string googleUsername = student.EMAIL.Split('@')[0];
                query["login"] = googleUsername;
            }
            else
            {
                query["login"] = student.USERNAME;
            }
            
            query["user_type"] = "S";
            query["enabled"] = (student.InActive == 0).ToString().ToLower();

            var config = Configuration.Instance;
            var mapping = config.UserFieldMapping;
            string[] convertibleFields = new string[] {
                "district", "grade", "school"
            };
            foreach (var haikuKey in mapping.Keys)
            {
                var gsmuKey = mapping[haikuKey];
                if (!config.ReservedHaikuFields.Contains(haikuKey))
                {
                    var gsmuValue = lang.ReflectionHelper.GetPropertyValue(student, gsmuKey);
                    var property = typeof(Responses.Entities.User).GetProperty(haikuKey);
                    var attributes = property.GetCustomAttributes(typeof(System.Xml.Serialization.XmlAttributeAttribute), false);
                    var attribute = attributes[0] as System.Xml.Serialization.XmlAttributeAttribute;
                    var queryName = attribute.AttributeName;

                    if (convertibleFields.Contains(gsmuKey))
                    {
                        switch (gsmuKey.ToLower())
                        {
                            case "district":
                                gsmuValue = school.District.Queries.GetDistrictNameById(gsmuValue as int?);
                                break;

                            case "grade":
                                gsmuValue = school.Grade.Queries.GetGradeNameById(gsmuValue as int?);
                                break;

                            case "school":
                                gsmuValue = school.School.Queries.GetSchoolNameById(gsmuValue as int?);
                                break;
                        }
                    }

                    var requiredFilds = new string[] { "FirstName", "LastName" };
                    if (string.IsNullOrWhiteSpace(gsmuKey) && requiredFilds.Contains(haikuKey))
                    {
                        gsmuValue = "[unknown]";
                    }

                    query[queryName] = gsmuValue == null ? null : gsmuValue.ToString();
                }
            }
            

            var queryString = Gsmu.Api.Language.StringHelper.NameValueCollectionToQueryString(query);

            url = url + "?" + queryString;

            var userResponse = HaikuClient.GetResponse(url, method);
            if (!userResponse.ContainsErrors)
            {
                var haikuUser = userResponse.Users.FirstUser;
                student.haiku_import_id = haikuUser.ImportId;
                student.haiku_user_id = haikuUser.Id;
                db.SaveChanges();
            }
            else if(userResponse.Error != null && userResponse.Error.Code == 404) 
            {
                student.haiku_user_id = 0;
                return SynchronizeStudent(student, db, response);
            }

            if (response == null)
            {
                response = userResponse;
            }
            else
            {
                response.JoinErrors(userResponse);
            }

            return response;
        }

        public static Response SynchronizeInstructor(entities.Instructor instructor, entities.SchoolEntities db = null, Response response = null)
        {
            db = db ?? new entities.SchoolEntities();

            http.HttpMethod method = http.HttpMethod.Post;
            string url = "user";

            var query = new NameValueCollection();

            query["import_id"] = Configuration.GetImportId(GsmuEntityType.Instructor, instructor.INSTRUCTORID, instructor.EMAIL);
            if (instructor.haiku_user_id > 0)
            {
                method = http.HttpMethod.Put;
                url = string.Format("user/{0}", instructor.haiku_user_id);
                query["import_id"] = instructor.haiku_import_id;
            }

            query["first_name"] = instructor.FIRST;
            query["last_name"] = instructor.LAST;
            query["password"] = instructor.PASSWORD;
            query["login"] = instructor.USERNAME;
            query["email"] = instructor.EMAIL;
            query["user_type"] = "T";
            query["enabled"] = (instructor.DISABLED == 0).ToString().ToLower();
            var queryString = Gsmu.Api.Language.StringHelper.NameValueCollectionToQueryString(query);

            url = url + "?" + queryString;

            var userResponse = HaikuClient.GetResponse(url, method);

            if (!userResponse.ContainsErrors)
            {
                var haikuUser = userResponse.Users.FirstUser;
                instructor.haiku_import_id = haikuUser.ImportId;
                instructor.haiku_user_id = haikuUser.Id;
                db.SaveChanges();
            }
            else if (userResponse.Error != null && userResponse.Error.Code == 404)
            {
                instructor.haiku_user_id = 0;
                return SynchronizeInstructor(instructor, db, response);
            }

            if (response == null)
            {
                response = userResponse;
            }
            else
            {
                response.JoinErrors(userResponse);
            }


            return response;
        }

        public static Response SynchronizeCourse(entities.Course course, entities.Instructor instructor, entities.SchoolEntities db = null, Response response = null)
        {
            db = db ?? new entities.SchoolEntities();

            http.HttpMethod method = http.HttpMethod.Post;
            string url = "class";

            var query = new NameValueCollection();

            query["import_id"] = Configuration.GetImportId(GsmuEntityType.Course, course.COURSEID,"");
            if (course.haiku_course_id > 0)
            {
                method = http.HttpMethod.Put;
                url = string.Format("class/{0}", course.haiku_course_id);
                query["import_id"] = course.haiku_import_id;
            }

            query["name"] = course.COURSENAME;
            query["description"] = course.DESCRIPTION;
            query["code"] = course.COURSENUM;

            var date = course.CourseTimes.FirstOrDefault();
            if (date == null || !date.COURSEDATE.HasValue)
            {
                date = new entities.Course_Time()
                {
                    COURSEDATE = DateTime.Now
                };
            }
            query["year"] = date.COURSEDATE.Value.Year.ToString();
            query["teacher_id"] = instructor.haiku_user_id.ToString();
            query["active"] = (!course.IsCancelled).ToString().ToLower();

            var queryString = Gsmu.Api.Language.StringHelper.NameValueCollectionToQueryString(query);

            url = url + "?" + queryString;

            var courseResponse = HaikuClient.GetResponse(url, method);

            if (!courseResponse.ContainsErrors)
            {
                var haikuCourse = courseResponse.Classes.ClassList[0];
                course.haiku_import_id = haikuCourse.ImportId;
                course.haiku_course_id = haikuCourse.Id;
                course.haiku_last_integration_date = DateTime.Now;
                if (!course.haiku_integration_date.HasValue || course.haiku_integration_date.Value < new DateTime(1991, 1, 1))
                {
                    course.haiku_integration_date = DateTime.Now;
                }
            }
            else if (courseResponse.Error != null && courseResponse.Error.Code == 404)
            {
                course.haiku_course_id = 0;
                return SynchronizeCourse(course, instructor, db, response);
            }
            if (response == null)
            {
                response = courseResponse;
            }
            else
            {
                response.JoinErrors(courseResponse);
            }
            course.haiku_last_result = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            db.SaveChanges();


            return response;
        }

        public static Tuple<Response, entities.Course, List<entities.Instructor>> SynchronizeCourse(int courseId)
        {
            Response response = new Response();
            var result = new Tuple<Response, entities.Course, List<entities.Instructor>>(response, null, null);

            using (var db = new entities.SchoolEntities())
            {
                var course = (from c in db.Courses where c.COURSEID == courseId select c).FirstOrDefault();
                var instructors = (from i in db.Instructors where i.INSTRUCTORID == course.INSTRUCTORID || i.INSTRUCTORID == course.INSTRUCTORID2 || i.INSTRUCTORID == course.INSTRUCTORID3 select i).ToList();

                result = new Tuple<Response, entities.Course, List<entities.Instructor>>(response, course, instructors);

                if (instructors.Count == 0)
                {
                    throw new Exception(
                        string.Format("Haiku requires a course to have a teacher. The course in GSMU with ID#{0} does not have an instructor associated so it cannot be imported into Haiku.", courseId)
                    );
                }

                foreach (var instructor in instructors)
                {
                    SynchronizeInstructor(instructor, db, response);
                }

                SynchronizeCourse(course, instructors.First(), db, response);
            }
            return result;
        }


    }
}   
