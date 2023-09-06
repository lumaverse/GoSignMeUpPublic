using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;
using net = System.Net;
using xml = System.Xml;
using lang = Gsmu.Api.Language;
using entities = Gsmu.Api.Data.School.Entities;
using student = Gsmu.Api.Data.School.Student;
using user = Gsmu.Api.Data.School.User;
using log = Gsmu.Api.Logging;
using Gsmu.Api.Integration.Haiku.Responses.Entities;
using school = Gsmu.Api.Data.School;

namespace Gsmu.Api.Integration.Haiku
{
    public static class HaikuImport
    {
        public static entities.Student SynchronizeStudent(int haikuUserId)
        {
            var response = GetUser(haikuUserId);
            return SynchronizeStudent(response.Users.FirstUser);
        }

        public static entities.Student SynchronizeStudent(User user, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();

            var config = Configuration.Instance;
            var stud = (from s in db.Students where s.haiku_user_id == user.Id select s).FirstOrDefault();

            //// orphan handling
            /*
            if (stud == null)
            {
                stud = (from s in db.Students where s.USERNAME == user.Login select s).FirstOrDefault();
            }
            */

            var studentWasNull = (stud == null);
            if (studentWasNull)
            {
                stud = new entities.Student();
                stud.STUDNUM = "haiku-" + user.Id;
            }

            stud.USERNAME = user.Login;
            stud.InActive = !user.Enabled ? 1 : 0;
            stud.haiku_user_id = user.Id;
            stud.haiku_import_id = user.ImportId;

            string[] convertibleFields = new string[] {
                "district", "grade", "school"
            };
            var mapping = config.UserFieldMapping;
            foreach (var haikuKey in mapping.Keys)
            {
                var gsmuKey = mapping[haikuKey];
                if (!config.ReservedGsmuFields.Contains(gsmuKey))
                {
                    var haikuValue = lang.ReflectionHelper.GetPropertyValue(user, haikuKey);
                    if (haikuKey == "Email" && config.UseUnconfirmedEmailWhenEmailIsEmpty &&  string.IsNullOrWhiteSpace(haikuValue as string))
                    {
                        haikuValue = user.UnconfirmedEmail;
                    }
                    if (string.IsNullOrWhiteSpace(haikuValue as string))
                    {
                        haikuValue = null;
                    }
                    if (haikuValue != null && haikuValue.GetType() == typeof(string) && convertibleFields.Contains(gsmuKey.ToLower()))
                    {
                        string originalHaikuValue = haikuValue as string;
                        switch (gsmuKey.ToLower())
                        {
                            case "district":
                                haikuValue = school.District.Queries.GetDistrictIdWithAddIfNonExistent(originalHaikuValue);
                                break;

                            case "grade":
                                haikuValue = school.Grade.Queries.GetGradeIdWithAddIfNonExistent(originalHaikuValue);
                                break;

                            case "school":
                                haikuValue = school.School.Queries.GetSchoolIdWithAddIfNonExistent(originalHaikuValue);
                                break;
                        }

                    }

                    lang.ReflectionHelper.SetPropertyValue(stud, gsmuKey, haikuValue);
                }
            }

            if (studentWasNull)
            {
                student.StudentHelper.RegisterStudent(stud, db, false);
            }

            db.SaveChanges();

            return stud;
        }

        public static entities.Instructor SynchronizeInstructor(User user, entities.SchoolEntities db = null)
        {
            var config = Configuration.Instance;
            var instructor = (from i in db.Instructors where i.haiku_user_id == user.Id select i).FirstOrDefault();

            db = db ?? new entities.SchoolEntities();

            if (instructor == null)
            {
                instructor = new entities.Instructor();
                instructor.PASSWORD = "haiku-" + user.Id;
                db.Instructors.Add(instructor);
            }

            instructor.EMAIL = user.Email;
            if (string.IsNullOrWhiteSpace(instructor.EMAIL) && config.UseUnconfirmedEmailWhenEmailIsEmpty)
            {
                instructor.EMAIL = user.UnconfirmedEmail;
            }
            instructor.USERNAME = user.Login;
            instructor.FIRST = user.FirstName;
            instructor.LAST = user.LastName;
            instructor.haiku_user_id = user.Id;
            instructor.haiku_import_id = user.ImportId;

            db.SaveChanges();

            return instructor;
        }
        // no API available to retrieve the Haiku ID, ImportID 
        // work around solution is to create a request to make new user to get the info in the XML respose.
        // once they have the API ready, we will need to adjust this.
        public static Response syncGoogleAct2HaikutUponAuth(string debugMethod = "get", string debugRequestURL = "test/ping", string debugQuery = null)
        {
            Response response = new Response();
            var currentMethod = http.HttpMethod.Post;

            switch (debugMethod)
            {
                case "get":
                    currentMethod = http.HttpMethod.Get;
                    break;
                case "post":
                    currentMethod = http.HttpMethod.Post;
                    break;
                case "put":
                    currentMethod = http.HttpMethod.Put;
                    break;
                case "delete":
                    currentMethod = http.HttpMethod.Delete;
                    break;
            }
          
            try
            {

                string testString = string.Format(debugRequestURL + debugQuery + "&page=0");
                // response = HaikuClient.GetResponse(RelativeUrl: string.Format(debugRequestURL + debugQuery + "&page=0"), HttpMethod: currentMethod);
                // response = HaikuClient.GetResponse("test/ping/gosignmeup");
                response = HaikuClient.GetResponse(debugRequestURL + debugQuery, currentMethod, true);

            }
            catch (Exception e)
            {
                response = new Response();
                response.Exceptions.Add(e);
            }
            return response;
        }

        public static Response getDebugResponse(string debugMethod = "get", string debugRequestURL = "test/ping", string debugQuery = null)
        {
            var currentPage = 0;
            Response response = new Response();
   
            var currentMethod = http.HttpMethod.Get;

            switch (debugMethod)
            {
                case "get":
                    currentMethod = http.HttpMethod.Get;
                    break;
                case "post":
                    currentMethod = http.HttpMethod.Post;
                    break;
                case "put":
                    currentMethod = http.HttpMethod.Put;
                    break;
                case "delete":
                    currentMethod = http.HttpMethod.Delete;
                    break;
            }

            try
            {
 
                    string testString = string.Format(debugRequestURL + debugQuery + "&page=0");
                   // response = HaikuClient.GetResponse(RelativeUrl: string.Format(debugRequestURL + debugQuery + "&page=0"), HttpMethod: currentMethod);
                   // response = HaikuClient.GetResponse("test/ping/gosignmeup");
                    response = HaikuClient.GetResponse(debugRequestURL + debugQuery, currentMethod);

            }
            catch (Exception e)
            {
                response = new Response();
                response.Exceptions.Add(e);
            }

            return response;

        }

        public static Response ListUsers()
        {
            var currentPage = 0;
            Response response = null;
            Response pagedResponse = null;

            try
            {
                do
                {
                    currentPage++;

                    pagedResponse = HaikuClient.GetResponse(
                        RelativeUrl: string.Format(
                            "user?page={0}", currentPage
                        ),
                        HttpMethod: http.HttpMethod.Get
                    );

                    if (response == null)
                    {
                        response = pagedResponse;
                    }
                    response.Users.AllRecords.AddRange(pagedResponse.Users.UserList);

                } while (currentPage < pagedResponse.Users.PageCount);
            }
            catch (Exception e)
            {
                response = new Response();
                response.Exceptions.Add(e);
            }

            return response;

        }

        public static Response ListClasses()
        {

            var currentPage = 0;
            Response response = null;
            Response pagedResponse = null;
            try
            {
                do
                {
                    currentPage++;

                    pagedResponse = HaikuClient.GetResponse(
                        RelativeUrl: string.Format(
                            "class?page={0}", currentPage
                        ),
                        HttpMethod: http.HttpMethod.Get
                    );

                    if (response == null)
                    {
                        response = pagedResponse;
                    }
                    response.Classes.AllRecords.AddRange(pagedResponse.Classes.ClassList);

                } while (currentPage < pagedResponse.Classes.PageCount);
            }
            catch (Exception e)
            {
                response = new Response();
                response.Exceptions.Add(e);
            }

            return response;

        }

        public static Response SynchronizeCourses()
        {
            var listClassesResponse = ListClasses();

            using (var db = new entities.SchoolEntities())
            {
                foreach (var course in listClassesResponse.Classes.AllRecords)
                {
                    SynchronizeCourse(course, db, listClassesResponse);
                }
                db.SaveChanges();
            }
            return listClassesResponse;
        }

        public static entities.Course SynchronizeCourse(int haikuClassId)
        {
            var haikuClass = GetClass(haikuClassId);
            var result = SynchronizeCourse(haikuClass.Classes.FirstClass, null, haikuClass);
            return result;
        }

        private static entities.Course SynchronizeCourse(Class haikuClass, entities.SchoolEntities db = null, Response response = null)
        {
            db = db ?? new entities.SchoolEntities();
            db.Configuration.LazyLoadingEnabled = true;
            db.Configuration.ProxyCreationEnabled = false;

            var gsmuCourse = (from c in db.Courses where c.haiku_course_id == haikuClass.Id select c).FirstOrDefault();
            if (gsmuCourse == null)
            {
                gsmuCourse = new entities.Course(true);
                gsmuCourse.COURSENUM = haikuClass.Code;
                gsmuCourse.haiku_integration_date = DateTime.Now;

                //Added to add initial value for non-null fields.
                gsmuCourse.CANCELCOURSE = 0;
                gsmuCourse.InternalClass = 0;
                gsmuCourse.DisplayPrice = 0;
                gsmuCourse.AudienceID = 0;
                gsmuCourse.GradingSystem = 0;
                gsmuCourse.BuybackCourse = 0;
                gsmuCourse.DepartmentNameID = 0;
                gsmuCourse.InserviceHours = 0;
                gsmuCourse.courseoutlineid = 0;
                gsmuCourse.ShowTopCatalog = 0;
                gsmuCourse.ShowBoldCatalog = 0;
                gsmuCourse.AllowCreditRollover = 0;

                gsmuCourse.Requirements = 0;
                gsmuCourse.Duplicated = 0;
                gsmuCourse.OfflineStudentCount = 0;
                gsmuCourse.viewpastcoursesdays = 0;

                gsmuCourse.DISTPRICE = 0;
                gsmuCourse.NODISTPRICE = 0;
                gsmuCourse.LOCATION = "";
                gsmuCourse.MATERIALS = "";
                gsmuCourse.Icons = "~|~~|~~|~";
                gsmuCourse.ContactName = "";
                gsmuCourse.ContactPhone = "";
                gsmuCourse.coursecertificate = 0;
                gsmuCourse.SingleCreditCost = 0;
                gsmuCourse.disable_canvas_integration = 0;
                gsmuCourse.PartialPaymentAmount = "0";
                gsmuCourse.PartialPaymentNon = "0";
                gsmuCourse.PartialPaymentSP = "0";
                gsmuCourse.CustomCreditHours = 0;



                //end
                db.Courses.Add(gsmuCourse);

                haikuClass.GsmuSynchronizationStatus = SynchronizationStatus.Inserted;
            }
            else
            {
                haikuClass.GsmuSynchronizationStatus = SynchronizationStatus.Updated;
            }
            //course.Active;
            gsmuCourse.DESCRIPTION = haikuClass.Description ?? string.Empty;
            gsmuCourse.COURSENAME = haikuClass.Name;
            //gsmuCourse.ShortDescription = course.ShortName;
            gsmuCourse.haiku_course_id = haikuClass.Id;            
            gsmuCourse.haiku_last_integration_date = DateTime.Now;
            gsmuCourse.haiku_import_id = haikuClass.ImportId;

            if (!gsmuCourse.haiku_integration_date.HasValue || gsmuCourse.haiku_integration_date.Value < new DateTime(1991, 1, 1))
            {
                gsmuCourse.haiku_integration_date = DateTime.Now;
            }
            gsmuCourse.haiku_last_result = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            db.SaveChanges();


            if (haikuClass.GsmuSynchronizationStatus == SynchronizationStatus.Inserted)
            {
                var maincategory = new entities.MainCategory();
                maincategory.CourseID = (short)gsmuCourse.COURSEID;
                maincategory.MainCategory1 = Configuration.HaikuCourseCategory;
                maincategory.mcatorder = 0;
                db.MainCategories.Add(maincategory);
                db.SaveChanges();
            }

            return gsmuCourse;
        }

        public static Response GetRostersForCourse(int courseId)
        {
            var currentPage = 0;
            Response response = null;
            Response pagedResponse = null;

            try
            {
                do
                {
                    currentPage++;

                    pagedResponse = HaikuClient.GetResponse(
                        RelativeUrl: string.Format(
                            "class/{0}/roster?page={1}", courseId, currentPage
                        ),
                        HttpMethod: http.HttpMethod.Get
                    );

                    if (response == null)
                    {
                        response = pagedResponse;
                    }
                    response.Rosters.AllRecords.AddRange(pagedResponse.Rosters.RosterList);

                } while (currentPage < pagedResponse.Rosters.PageCount);
            }
            catch (Exception e)
            {
                response = new Response();
                response.Exceptions.Add(e);
            }

            return response;
        }

        public static Response ListCourseRosterInformation(int? haikuClassId = null)
        {
            Response listClassesResponse;
            if (haikuClassId == null)
            {
                listClassesResponse = ListClasses();
            } else {
                listClassesResponse = GetClass(haikuClassId.Value);
            }
            var response = new Response();
            response.JoinErrors(listClassesResponse);
            foreach (var course in listClassesResponse.Classes.AllRecords)
            {
                response.ClassesByClassId[course.Id] = course;

                var rosterResponse = GetRostersForCourse(course.Id);
                response.JoinErrors(rosterResponse);
                List<Roster> result = rosterResponse.Rosters.AllRecords;

                response.RostersByClassId[course.Id] = result;

                foreach (var roster in result)
                {
                    if (roster.UserId < 1)
                    {
                        response.Exceptions.Add(new Exception(
                            string.Format("Roster {0} for {1} {2} ({3}) looks like is unconfirmed so we cannot import the non existing user/roster.", roster.RosterId, roster.FirstName, roster.LastName, roster.Email)
                        ));
                    } else if (!response.UsersByUserId.ContainsKey(roster.UserId))
                    {
                        var userResponse = GetUser(roster.UserId);

                        if (userResponse.Error == null) {
                            response.UsersByUserId[roster.UserId] = userResponse.Users.FirstUser;
                        }
                        response.JoinErrors(userResponse);
                    }
                }
            }
            return response;
        }

        public static Response GetUser(int haikuUserId)
        {
            var response = new Response();
            try
            {
                response = HaikuClient.GetResponse(
                    RelativeUrl: string.Format(
                        "user/{0}", haikuUserId
                    ),
                    HttpMethod: http.HttpMethod.Get
                );
                response.Users.AllRecords = response.Users.UserList;
            }
            catch (Exception e)
            {
                response.Exceptions.Add(e);
            }
            return response;
        }

        public static Response GetClass(int haikuClassId)
        {
            var response = new Response();
            try
            {
                response = HaikuClient.GetResponse(
                    RelativeUrl: string.Format(
                        "class/{0}", haikuClassId
                    ),
                    HttpMethod: http.HttpMethod.Get
                );
                response.Classes.AllRecords = response.Classes.ClassList;
            }
            catch (Exception e)
            {
                response.Exceptions.Add(e);
            }
            return response;
        }

        public static Response SynchronizeRoster(int? haikuClassId = null)
        {
            var rosterInformationData = ListCourseRosterInformation(haikuClassId);

            using (var db = new entities.SchoolEntities())
            {
                foreach (var pair in rosterInformationData.ClassesByClassId)
                {
                    var haikuCourseId = pair.Key;
                    var haikuCourse = pair.Value;
                    var gsmuCourse = SynchronizeCourse(haikuCourse, db, rosterInformationData);
                    db.SaveChanges();

                    gsmuCourse.INSTRUCTORID = null;
                    gsmuCourse.INSTRUCTORID2 = null;
                    gsmuCourse.INSTRUCTORID3 = null;

                    var haikuRosters = rosterInformationData.RostersByClassId[haikuCourseId];
                    foreach (var roster in haikuRosters)
                    {
                        if (rosterInformationData.UsersByUserId.ContainsKey(roster.UserId))
                        {
                            var haikuRosterUser = rosterInformationData.UsersByUserId[roster.UserId];
                            if (roster.Role == UserType.Teacher)
                            {
                                var gsmuInstructor = SynchronizeInstructor(haikuRosterUser, db);
                                db.SaveChanges();

                                SynchronizeInstructors(gsmuCourse, gsmuInstructor, rosterInformationData);
                                db.SaveChanges();
                            }
                            else if (roster.Role == UserType.Student)
                            {
                                var gsmuStudent = SynchronizeStudent(haikuRosterUser, db);
                                db.SaveChanges();

                                var gsmuRoster = SynchronizeRoster(roster, haikuRosterUser, gsmuCourse, gsmuStudent, db);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            rosterInformationData.Exceptions.Add(new Exception(string.Format("User in roster missing! Haiku roster Id: {0}, Haiku user Id: {1}, Haiku course Id: {2}", roster.RosterId, roster.UserId, roster.ClassId)));
                        }
                    }
                    SynchronizeOrphanedRosters(haikuRosters, gsmuCourse, db);
                    db.SaveChanges();
                }

                db.SaveChanges();
            }
            return rosterInformationData;
        }

        private static void SynchronizeOrphanedRosters(List<Roster> haikuRosters, entities.Course gsmuCourse, entities.SchoolEntities db)
        {
            var gsmuRosters = (from cr in db.Course_Rosters where cr.COURSEID == gsmuCourse.COURSEID select cr);
            foreach (var roster in gsmuRosters)
            {
                if (roster.haiku_roster_id.HasValue && roster.haiku_roster_id > 0)
                {
                    var haikuId = roster.haiku_roster_id.Value;
                    var haikuRoster = (from hr in haikuRosters where hr.RosterId == haikuId select hr).FirstOrDefault();
                    if (haikuRoster == null)
                    {
                        roster.CancelRoster();
                    }
                }
            }
        }

        private static void SynchronizeInstructors(entities.Course gsmuCourse, entities.Instructor gsmuInstructor, Response data)
        {
            if (gsmuCourse.INSTRUCTORID == null && gsmuCourse.INSTRUCTORID != gsmuInstructor.INSTRUCTORID)
            {
                gsmuCourse.INSTRUCTORID = gsmuInstructor.INSTRUCTORID;
            }
            else if (gsmuCourse.INSTRUCTORID2 == null && gsmuCourse.INSTRUCTORID2 != gsmuInstructor.INSTRUCTORID)
            {
                gsmuCourse.INSTRUCTORID2 = gsmuInstructor.INSTRUCTORID;
            }
            else if (gsmuCourse.INSTRUCTORID3 == null && gsmuCourse.INSTRUCTORID3 != gsmuInstructor.INSTRUCTORID)
            {
                gsmuCourse.INSTRUCTORID3 = gsmuInstructor.INSTRUCTORID;
            }
            else
            {
                data.Exceptions.Add(new Exception(
                    string.Format("Could not add instructor {0} to course {1}, three instructor limit reached.", gsmuInstructor.INSTRUCTORID, gsmuCourse.COURSEID)
                ));
            }
        }

        public static entities.Course_Roster SynchronizeRoster(Roster haikuRoster, User haikuRosterUser, entities.Course gsmuCourse, entities.Student gsmuStudent, entities.SchoolEntities db = null)
        {
            db = db ?? new entities.SchoolEntities();

            var gsmuRoster = (from r in db.Course_Rosters where r.haiku_roster_id == haikuRoster.RosterId select r).FirstOrDefault();

            if (gsmuRoster == null)
            {
                gsmuRoster = new entities.Course_Roster(true);
                db.Course_Rosters.Add(gsmuRoster);
            }
            gsmuRoster.COURSEID = gsmuCourse.COURSEID;
            gsmuRoster.STUDENTID = gsmuStudent.STUDENTID;
            gsmuRoster.haiku_roster_id = haikuRoster.RosterId;
            gsmuRoster.ActivateRoster();

            db.SaveChanges();

            return gsmuRoster;
        }
    }
}
