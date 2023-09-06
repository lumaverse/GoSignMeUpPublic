using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using System.Net;
using System.Web;
using System.Xml;
using Gsmu.Api.Authorization;
using System.Data;
using System.Data.SqlClient;

namespace Gsmu.Api.Integration.Blackboard.Connector
{
    public class UserConnector
    {
        public static BlackboardResponse GetPasswordHash(string password)
        {
            var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "getPasswordHash", "User", new NameValueCollection()
            {
                {"password", password}
            });
            return result;
        }

        public static BlackboardResponse SelectUser(string username)
        {
            var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "select", "User", new NameValueCollection()
            {
                {"userName", username}
            });
            return result;
        }

        public static BlackboardResponse UpdateBBUserAccount(string resetUsername, string resetNewPass, string givenName, string familyName, string uEmail, string isAvailable, int? district, int? school, int? grade )
        {
            NameValueCollection UserObj = new NameValueCollection();
            var bbfield = "";
            var gsmufield = "";
            if (Settings.Instance.GetMasterInfo3().BBStudentIntegrationFields != "" && Settings.Instance.GetMasterInfo3().BBStudentIntegrationFields != null)
            {
                if (!string.IsNullOrEmpty(resetNewPass) && resetNewPass.Length > 7)
                {
                    UserObj.Add("password", resetNewPass);
                }


                foreach(var field in Settings.Instance.GetMasterInfo3().BBStudentIntegrationFields.Split('&'))
                {
                    bbfield = field.Split('=')[0];
                    gsmufield = field.Split('=')[1];
                    if (gsmufield.ToLower() == "last")
                    {
                        if (familyName != "")
                        {
                            UserObj.Add(bbfield, familyName);
                        }
                    }
                    else if (gsmufield.ToLower() == "first")
                    {
                        if (givenName != "")
                        {
                            UserObj.Add(bbfield, givenName);
                        }
                    }
                    else if (gsmufield.ToLower() == "email")
                    {
                        if (uEmail != "")
                        {
                            UserObj.Add(bbfield, uEmail);
                        }
                    }


                    else if (gsmufield.ToLower() == "district")
                    {
                        if (district != 0)
                        {
                            UserObj.Add(bbfield, GetSchoolDistrictGradeLabel(gsmufield, district.Value));
                        }
                    }

                    else if (gsmufield.ToLower() == "school")
                    {
                        if (school != 0)
                        {
                            UserObj.Add(bbfield, GetSchoolDistrictGradeLabel(gsmufield, school.Value));
                        }
                    }

                    else if (gsmufield.ToLower() == "grade")
                    {
                        if (grade != null)
                        {
                            UserObj.Add(bbfield, GetSchoolDistrictGradeLabel(gsmufield, grade.Value));
                        }
                    }

                }

                if (isAvailable != "")
                {
                    UserObj.Add("isAvailable", isAvailable);
                }

                var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "insertUpdate", "User",
                             new NameValueCollection() { { "userName", resetUsername } },
                             UserObj
                 );

                return result;

            }
            else
            {
                if (!string.IsNullOrEmpty(resetNewPass) && resetNewPass.Length > 7)
                {
                    UserObj.Add("password", resetNewPass);
                }
                if (familyName != "")
                {
                    UserObj.Add("familyName", familyName);
                }
                if (familyName != "")
                {
                    UserObj.Add("givenName", givenName);
                }
                if (uEmail != "")
                {
                    UserObj.Add("emailAddress", uEmail);
                }
                if (isAvailable != "")
                {
                    UserObj.Add("isAvailable", isAvailable);
                }

                var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "insertUpdate", "User",
                    new NameValueCollection() { { "userName", resetUsername } },
                    UserObj
                    );
                return result;
            }
        }
        public static BlackboardResponse InsertBBUserAccount(string resetUsername, string resetNewPass, string givenName, string familyName, string uEmail, int? district, int? school, int? grade)
        {
            if (Settings.Instance.GetMasterInfo3().BBStudentIntegrationFields != "" && Settings.Instance.GetMasterInfo3().BBStudentIntegrationFields != null)
            {
                NameValueCollection UserObj = new NameValueCollection();
                var bbfield = "";
                var gsmufield = "";
                foreach (var field in Settings.Instance.GetMasterInfo3().BBStudentIntegrationFields.Split('&'))
                {
                    bbfield = field.Split('=')[0];
                    gsmufield = field.Split('=')[1];
                    if (gsmufield.ToLower() == "last")
                    {
                        if (familyName != "")
                        {
                            UserObj.Add(bbfield, familyName);
                        }
                    }
                    else if (gsmufield.ToLower() == "first")
                    {
                        if (givenName != "")
                        {
                            UserObj.Add(bbfield, givenName);
                        }
                    }
                    else if (gsmufield.ToLower() == "email")
                    {
                        if (uEmail != "")
                        {
                            UserObj.Add(bbfield, uEmail);
                        }
                    }

                    else if (gsmufield.ToLower() == "district")
                    {
                        if (district != 0)
                        {
                            UserObj.Add(bbfield, GetSchoolDistrictGradeLabel(gsmufield, district.Value));
                        }
                    }

                    else if (gsmufield.ToLower() == "school")
                    {
                        if (school != 0)
                        {
                            UserObj.Add(bbfield, GetSchoolDistrictGradeLabel(gsmufield, school.Value));
                        }
                    }

                    else if (gsmufield.ToLower() == "grade")
                    {
                        if (grade != null)
                        {
                            UserObj.Add(bbfield, GetSchoolDistrictGradeLabel(gsmufield,grade.Value));
                        }
                    }

                }


                var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "insertUpdate", "User",
                new NameValueCollection() { { "userName", resetUsername }, { "nodeId", "" } },
                UserObj 
                );
                return result;
            }

            else
            {
                var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "insertUpdate", "User",
                new NameValueCollection() { { "userName", resetUsername }, { "nodeId", "" } },
                new NameValueCollection() { { "userName", resetUsername }, { "password", resetNewPass }, { "familyName", familyName }, { "givenName", givenName }, { "emailAddress", uEmail }, { "isAvailable", "True" }, { "systemRole", "USER" }, { "portalRole", Settings.Instance.GetMasterInfo3().BlackboardPortalRole } }
                );
                return result;
            }
        }

        public static BlackboardResponse SeachUserByFields(string bbfieldname, string username)
        {
            string[] bbUniqueKeyField = new string[2];
            bbUniqueKeyField[0] = "email";
            bbUniqueKeyField[1] = "BatchUidSearch";
            //bbUniqueKeyField[2] = "datasourcekey";

            //string availableFieldname = "userName, emailAddress, batchUid";
            if (bbfieldname != "")
            {
                var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "select", "User", new NameValueCollection()
                {
                    { bbfieldname, username}
                });
                return result;
            }
            else
            {
                //default search
                var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "select", "User", new NameValueCollection()
                {
                    { "userName", username}
                });
                var ObjectBBSearchCount = "";
                //custom field
                // may need to check to see if these fields are public since each BB may customize their availability through B2
                // for now assume they have these 2 fields (email & batch) at all time
                foreach (string arrItem in bbUniqueKeyField)
                {
                    if (!result.IsSuccess)
                    {
                        result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "select", "User", new NameValueCollection()
                        {
                            { arrItem, username}
                        });
                        //IsSuccess here is not really what we are looking for as it is not determine found user or not in Field Username
                        // must look for request.User.count instead
                        // have to loop through the response since not all fields will have the same count path.
                        foreach (String TempKey in result.Raw.AllKeys)
                        {
                            if (TempKey == "response.User.count" && ObjectBBSearchCount =="")
                            {
                                ObjectBBSearchCount = result.Raw[TempKey];
                            }

                        }
                    }
                }
                //override the result object
                if (ObjectBBSearchCount == "0")
                {
                    NameValueCollection result2 = new NameValueCollection();
                    result2.Add("IsSuccess", "false");
                    result2.Add("TargetClass", "User");
                    result2.Add("TargetClassCount", "0");
                    var abc = new BlackboardResponse(result2);
                    result = abc;
                }
                //all false means the account doesnt exist.
                return result;
            }
        }

        public static BlackboardResponse Authenticate(string username, string password)
        {
            var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "authenticate", "User", new NameValueCollection()
            {
                {"userName", username},
                {"password", password}
            });
            return result;
        }

        public static BlackboardResponse GetSessionUser(string sessionId, bool secure)
        {
            var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "getSessionUser", "User", new NameValueCollection()
            {
                {
                    "sessionId", sessionId
                },
                {
                    "secure", secure.ToString()
                }
            });
            return result;
        }

        public static BlackboardResponse GetPortalRoles(string username)
        {
            var result = ServiceInterface.QueryBlackboard(ConnectorEnum.UserConnector, "getPortalRoles", "User", new NameValueCollection()
            {
                {"userName", username}
            });
            return result;
        }

        public static string GetSchoolDistrictGradeValue(string gsmuField, int value)
        {
            string result = "";
            using (var db = new SchoolEntities())
            {
                switch (gsmuField.ToLower())
                {
                    case "district":
                        var district = (from d in db.Districts where d.DISTID ==value select d).FirstOrDefault();
                        if (district != null)
                        {
                            result = district.DISTID.ToString();
                        }
                        else
                        {
                            result = "0";
                        }
                        break;

                    case "school":
                        var school = (from s in db.Schools where s.ID == value select s).FirstOrDefault();
                        if (school != null)
                        {
                            result = school.locationid.ToString();
                        }
                        else
                        {
                            result = "0";
                        }
                        break;

                    case "grade":
                        var grade = (from g in db.Grade_Levels where g.GRADEID==value select g).FirstOrDefault();
                        if (grade != null)
                        {
                            result = grade.GRADEID.ToString();
                        }
                        else
                        {
                            result = "0";
                        }
                        break;
                }
                return result;
            }
        }

        public static string GetSchoolDistrictGradeLabel(string gsmuField, int value)
        {
            string result = "";
            using (var db = new SchoolEntities())
            {
                switch (gsmuField.ToLower())
                {
                    case "district":
                        var district = (from d in db.Districts where d.DISTID == value select d).FirstOrDefault();
                        if (district != null)
                        {
                            result = district.DISTRICT1;
                        }
                        else
                        {
                            result = "0";
                        }
                        break;

                    case "school":
                        var school = (from s in db.Schools where s.locationid == value select s).FirstOrDefault();
                        if (school != null)
                        {
                            result = school.LOCATION;
                        }
                        else
                        {
                            result = "0";
                        }
                        break;

                    case "grade":
                        var grade = (from g in db.Grade_Levels where g.GRADEID == value select g).FirstOrDefault();
                        if (grade != null)
                        {
                            result = grade.GRADE;
                        }
                        else
                        {
                            result = "0";
                        }
                        break;
                }
                return result;
            }
        }



    }
}
