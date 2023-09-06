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
using Gsmu.Api.Integration.Blackboard.Connector;
using System.Collections.Generic;

namespace Gsmu.Api.Integration.Blackboard
{
    public class BlackboardSso
    { 
        public static void HandleSso(string hash, System.Web.Mvc.ControllerBase controller)
        {
            var config = Configuration.Instance;

            var url = config.BlackboardConnectionUrl + "?hash=" + hash;
            var client = new WebClient();
            var data = client.DownloadString(url);
            var hashResult = HttpUtility.ParseQueryString(data);
            var secure = false;
            string sessionId = null;
            for (var index = 0; index < int.Parse(hashResult["count"]); index++)
            {
                if (hashResult["name" + index.ToString()] == "session_id")
                {
                    sessionId = hashResult["value" + index.ToString()];
                    break;
                } else if (hashResult["name" + index.ToString()] == "s_session_id")
                {
                    secure = true;
                    sessionId = hashResult["value" + index.ToString()];
                    break;
                }
            }
            sessionId = sessionId.Substring(0, 32);
            if (sessionId == null)
            {
                throw new Exception("Session ID not found Blackboard hash!");
            }

            var result = UserConnector.GetSessionUser(sessionId, secure);

            if (result.IsSuccess)
            {
                var role = result["User", "InstitutionRole"];
                var username = result["userName"];

                //pulling BB profile.
                var userResult = UserConnector.SelectUser(username);
                var messages = new List<string>();
                if (role == config.BlackboardInstructorRole)
                {
                    using (var db = new SchoolEntities())
                    {
                        Instructor instructorinfo = (from ins in db.Instructors where ins.USERNAME == username select ins).FirstOrDefault();
                        //if (student == null && config.BlacboardSsoUserIntegrationEnabled && config.BlackboardRealtimeStudentSyncEnabled)
                        if (instructorinfo == null && config.BlacboardSsoUserIntegrationEnabled)
                        {
                            var TurnOnDebugTracingMode = System.Configuration.ConfigurationManager.AppSettings["TurnOnDebugTracingMode"];
                            if (TurnOnDebugTracingMode != null)
                            {
                                if (TurnOnDebugTracingMode.ToLower() == "on")
                                {
                                    Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                                    Audittrail.TableName = "Students";
                                    Audittrail.AuditDate = DateTime.Now;
                                    Audittrail.RoutineName = "BBSSO - InsertBlackboardStudent" + AuthorizationHelper.CurrentUser.LoggedInUserType;
                                    Audittrail.UserName = AuthorizationHelper.CurrentUser.LoggedInUsername;
                                    try
                                    {
                                        Audittrail.AuditAction = "New Username:" + username + " -ROle: " + role;
                                    }
                                    catch { }
                                    Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                                    LogManager.LogSiteActivity(Audittrail);
                                }
                            }
                            instructorinfo = InsertBlackboardInstructor(username, userResult);
                        }
                    }
                    messages = AuthorizationHelper.LoginInstructor(username);
                    Gsmu.Api.Web.ObjectHelper.AddRequestMessages(controller, messages);
                    //throw new Exception("Instructor login is not yet implemented in .NET");
                }
                else
                {
                    //non instructor are treated as student.
                    //insert student into DB first. since comming from BB, it's already authenticated.
                    // search gsmu db for preexisting data if not create new account.
                    using (var db = new SchoolEntities())
                    {
                        Student student = (from s in db.Students where s.USERNAME == username select s).FirstOrDefault();
                        //if (student == null && config.BlacboardSsoUserIntegrationEnabled && config.BlackboardRealtimeStudentSyncEnabled)
                        if (student == null && config.BlacboardSsoUserIntegrationEnabled)
                        {
                            var TurnOnDebugTracingMode = System.Configuration.ConfigurationManager.AppSettings["TurnOnDebugTracingMode"];
                            if (TurnOnDebugTracingMode != null)
                            {
                                if (TurnOnDebugTracingMode.ToLower() == "on")
                                {
                                    Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                                    Audittrail.TableName = "Students";
                                    Audittrail.AuditDate = DateTime.Now;
                                    Audittrail.RoutineName = "BBSSO - InsertBlackboardStudent" + AuthorizationHelper.CurrentUser.LoggedInUserType;
                                    Audittrail.UserName = AuthorizationHelper.CurrentUser.LoggedInUsername;
                                    try
                                    {
                                        Audittrail.AuditAction = "New Username:" + username + " -ROle: " + role;
                                    }
                                    catch { }
                                    Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                                    LogManager.LogSiteActivity(Audittrail);
                                }
                            }
                            student = InsertBlackboardStudent(username, userResult);
                        }
                    }
                    messages = AuthorizationHelper.LoginStudent(username);
                    Gsmu.Api.Web.ObjectHelper.AddRequestMessages(controller, messages);
                }
            }
            else
            {
                /*
                StringBuilder buildResult = new StringBuilder();
                foreach(string key in result.Raw.AllKeys) {
                    string value = result.Raw[key];
                    buildResult.AppendLine(key + ": " + value);
                }
                throw new Exception("SSO failed\n" + buildResult.ToString());
                */
                throw new Exception("SSO failed");
            }
        }

        public static Tuple<bool, BlackboardResponse> AuthenticateUser(string username, string password)
        {
            var authenticated = false;

            var getPasswordHashResult = UserConnector.GetPasswordHash(password);
            var userResult = UserConnector.SelectUser(username);

            if (getPasswordHashResult["requireAuthentication"] == "true")
            {
                var authenticateResult = UserConnector.Authenticate(username, password);
                authenticated = authenticateResult["authenticated"] == "true";
            }
            else
            {
                authenticated = getPasswordHashResult["password"] == userResult["User", "Password"];
            }

            return new Tuple<bool, BlackboardResponse>(authenticated, userResult);
        }


        /*
         * Clone of v3/blackboard/library.asp function GSMUInsertBBStudent

BB CONFIGS TO LOAD INTO .NET

- BBStudentIntegrationFields masterinfo3
- membership_status masterinfo
= DefaultNewStudentMemberType masterinfo2

BB FUNCTIONS TO COPY INTO .NET
BBGetStudentFieldId
getnewmemberdistemployeevalue
BBStudentInActiveValue
BBGetUserPortalRoles

' THIS IS USED FROM SSO AS WELL, MAKE SURE IF YOU CHANGE THAT CHANGES ARE APPROPPRIATE FOR THAT AS WELL
Function GSMUInsertBBStudent(byref config, byref msg)
	''1 - Institution Role
	''2 - Secondary Institution Role
	''3 - System Role
	''4 - Secondary System Role
	''5 - Default Primary Institution Role associate with district
	''6 - Default Secondary Institution Role associate with district
	''7 - Default System Role associate with district
	dim sql, sql_fields, sql_values, comma, had_distemployee
	dim i, gsmufield, bbfield_value

	dim fields, bbfields
	set fields = config("BBStudentIntegrationFields")
	bbfields = fields.keys

	sql = "INSERT INTO Students ("
	sql_fields = ""
	sql_values = ""
	comma = ""
	had_distemployee = false
	GSMUInsertBBStudent = msg.getResponseTargetClassProperty("Password")
	StudentBBUsername = msg.getResponseTargetClassProperty("UserName")
	BBPrimaryInstitutionRole = msg.getResponseTargetClassProperty("InstitutionRole")

	for i = lbound(bbfields) to ubound(bbfields)
		bbfield = bbfields(i)
		bbfield = ucase(left(bbfield, 1)) & mid(bbfield, 2)

		gsmufield = fields(bbfields(i))

		bbfield_value = BBGetStudentFieldId(config, gsmufield, bbfield, msg)
		'bbfield_value = msg.getResponseTargetClassProperty(bbfield)

		sql_fields = sql_fields & comma & gsmufield
		sql_values = sql_values & comma & "'" & dbprepvalue(bbfield_value) & "'"
		comma = ", "

		if lcase(gsmufield) = "distemployee" then
			had_distemployee = true
		end if
	next
	if not had_distemployee then
		sql_fields = sql_fields & comma & "distemployee"
		if config("membership_status") = 0 then
			sql_values = sql_values & comma & "0"
		else
			'sql_values = sql_values & comma & config("DefaultNewStudentMemberType")
			sql_values = sql_values & comma & getnewmemberdistemployeevalue(config("DefaultNewStudentMemberType"))
		end if
	end if

	sql_fields = sql_fields & comma & "date_imported_from_bb"
	sql_values = sql_values & comma & "getdate()"

	sql_fields = sql_fields & comma & "date_modified"
	sql_values = sql_values & comma & "getdate()"

	sql_fields = sql_fields & comma & "BBPrimaryInstitutionRole"
	sql_values = sql_values & comma & "'" & BBPrimaryInstitutionRole & "'"

	sql_fields = sql_fields & comma & "inactive"
	sql_values = sql_values & comma & BBStudentInActiveValue(msg)

	sql = sql & sql_fields & ") VALUES (" & sql_values & ")"
	sql = sqlscrub(sql)

	dim db
	set db = config("db")
	db.ExecQuery(sql)

	sql = " select top 1 * from students where username='" & dbprepvalue(StudentBBUsername) & "' order by studentid desc"
	set rsNewStudent = db.queryresultset(sql)
	if not rsNewStudent.eof then
		newStudentID = rsNewStudent("studentid")
		set myrRoles = BBGetUserPortalRoles(config,StudentBBUsername )
		Do While Not myrRoles.EOF
		      sSQL = "Insert into [BlackboardUserRoles] (roletype,studentid,BBRoleID,BBRoleName,exclusion,dateadded) "
		      sSQL = sSQL & " VALUES (1," & newStudentID & ",'" & myrRoles.Item("RoleID") & "','" & myrRoles.Item("RoleName") & "',0,getdate())"
		      set db = config("db")
		      db.ExecQuery(sSQL)
		      myrRoles.MoveNext
		Loop
	end if
End Function

         */
        public static Instructor InsertBlackboardInstructor(string username, BlackboardResponse blackboardStudentDataResponse)
        {
            var config = Configuration.Instance;
            using (var db = new SchoolEntities())
            using (var connection = Connections.GetSchoolConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    var command = connection.CreateCommand();

                    var sql = "INSERT INTO Instructors (";
                    var sqlFields = string.Empty;
                    var sqlValues = string.Empty;
                    var comma = string.Empty;
                    var hadDistemployee = false;
                    var fields = config.BlackboardStudentIntegrationFields;
                    var fieldKeys = fields.Keys;
                    var bbPrimaryInstitutionRole = blackboardStudentDataResponse["User", "InstitutionRole"];

                    foreach (string bbField in fields.Keys)
                    {
                        var gsmuField = fields[bbField];
                        if (gsmuField.ToLower() == "studnum")
                        {
                            gsmuField = "password";
                        }
                        if (gsmuField.ToLower() == "grade")
                        {
                            gsmuField = "gradelevel";
                        }                       
                        string gsmuFieldValue = GetGsmuUserFieldValue(gsmuField, bbField, blackboardStudentDataResponse);

                        string gsmuParameterName = "@" + gsmuField;
                        sqlFields += comma + gsmuField;
                        sqlValues += comma + gsmuParameterName;
                        command.Parameters.Add(
                            new SqlParameter(gsmuParameterName, gsmuFieldValue)
                        );
                        comma = ", ";

                        if (gsmuField.ToLower() == "distemployee")
                        {
                            hadDistemployee = true;
                        }
                    }

                    if (!hadDistemployee)
                    {
                        sqlFields += comma + "distemployee";
                        if (Settings.Instance.GetMasterInfo().Membership_Status == 0)
                        {
                            sqlValues += comma + "0";
                        }
                        else
                        {
                            sqlValues += comma + StudentHelper.GetNewMemberDistemployeeValue(Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType);
                        }
                    }
                    sqlFields += comma + "date_bb_integrated";
                    sqlValues += comma + "getdate()";

                    sqlFields += comma + "date_modified";
                    sqlValues += comma + "getdate()";

                    sqlFields += comma + "InstructorRegField10";
                    sqlValues += comma + "@BBPrimaryInstitutionRole";
                    command.Parameters.Add(
                        new SqlParameter("@BBPrimaryInstitutionRole", bbPrimaryInstitutionRole)
                    );

                    sqlFields += comma + "DISABLED";
                    sqlValues += comma;
                    sqlValues += blackboardStudentDataResponse["User.IsAvailable"] == "true" ? "0" : "1";

                    sql += sqlFields + ") VALUES (" + sqlValues + ")";

                    command.Transaction = transaction;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    var InstructorIdCommand = connection.CreateCommand();
                    InstructorIdCommand.Transaction = transaction;
                    InstructorIdCommand.CommandText = "SELECT instructorid FROM Instructors WHERE username = @username";
                    InstructorIdCommand.Parameters.Add(
                        new SqlParameter("@username", username)
                    );
                    var InstructorId = (int)InstructorIdCommand.ExecuteScalar();

                    var roleQueryResult = UserConnector.GetPortalRoles(username);

                    for (var index = 0; index < roleQueryResult.TargetClassCount; index++)
                    {
                        var roleId = roleQueryResult[index, "RoleID"];
                        var roleName = roleQueryResult[index, "RoleName"];

                        var roleCommand = connection.CreateCommand();
                        roleCommand.Transaction = transaction;
                        roleCommand.CommandText = "Insert into [BlackboardUserRoles] (roletype, studentid, BBRoleID, BBRoleName, exclusion, dateadded) VALUES (1, @studentId, @roleId, @RoleName,0, getdate())";
                        roleCommand.Parameters.Add(
                            new SqlParameter("@studentId", InstructorId)
                        );
                        roleCommand.Parameters.Add(
                            new SqlParameter("@roleId", roleId)
                        );
                        roleCommand.Parameters.Add(
                            new SqlParameter("@RoleName", roleName)
                        );
                        roleCommand.ExecuteNonQuery();
                    }


                    transaction.Commit();

                    var instructorInfo = (from s in db.Instructors where s.USERNAME == username select s).FirstOrDefault();
                    return instructorInfo;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }

            }
        }
        public static Student InsertBlackboardStudent(string username, BlackboardResponse blackboardStudentDataResponse)
        {
            var config = Configuration.Instance;
            using (var db = new SchoolEntities())
            using (var connection = Connections.GetSchoolConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    var command = connection.CreateCommand();

                    var sql = "INSERT INTO Students (";
                    var sqlFields = string.Empty;
                    var sqlValues = string.Empty;
                    var comma = string.Empty;
                    var hadDistemployee = false;
                    var fields = config.BlackboardStudentIntegrationFields;
                    var fieldKeys = fields.Keys;
                    var bbPrimaryInstitutionRole = blackboardStudentDataResponse["User", "InstitutionRole"];

                    foreach (string bbField in fields.Keys)
                    {
                        var gsmuField = fields[bbField];
                        string gsmuFieldValue = GetGsmuUserFieldValue(gsmuField, bbField, blackboardStudentDataResponse);

                        string gsmuParameterName = "@" + gsmuField;
                        sqlFields += comma + gsmuField;
                        sqlValues += comma + gsmuParameterName;
                        command.Parameters.Add(
                            new SqlParameter(gsmuParameterName, gsmuFieldValue)
                        );
                        comma = ", ";

                        if (gsmuField.ToLower() == "distemployee")
                        {
                            hadDistemployee = true;
                        }
                    }

                    if (!hadDistemployee)
                    {
                        sqlFields += comma + "distemployee";
                        if (Settings.Instance.GetMasterInfo().Membership_Status == 0)
                        {
                            sqlValues += comma + "0";
                        }
                        else
                        {
                            sqlValues += comma + StudentHelper.GetNewMemberDistemployeeValue(Settings.Instance.GetMasterInfo2().DefaultNewStudentMemberType);
                        }
                    }
                    sqlFields += comma + "dateadded";
                    sqlValues += comma + "getdate()";

                    sqlFields += comma + "date_imported_from_bb";
                    sqlValues += comma + "getdate()";

                    sqlFields += comma + "date_modified";
                    sqlValues += comma + "getdate()";

                    sqlFields += comma + "BBPrimaryInstitutionRole";
                    sqlValues += comma + "@BBPrimaryInstitutionRole";
                    command.Parameters.Add(
                        new SqlParameter("@BBPrimaryInstitutionRole", bbPrimaryInstitutionRole)
                    );

                    sqlFields += comma + "inactive";
                    sqlValues += comma;
                    sqlValues += blackboardStudentDataResponse["User.IsAvailable"] == "true" ? "0" : "1";

                    sql += sqlFields + ") VALUES (" + sqlValues + ")";

                    command.Transaction = transaction;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    var studentIdCommand = connection.CreateCommand();
                    studentIdCommand.Transaction = transaction;
                    studentIdCommand.CommandText = "SELECT studentid FROM students WHERE username = @username";
                    studentIdCommand.Parameters.Add(
                        new SqlParameter("@username", username)
                    );
                    var studentId = (int)studentIdCommand.ExecuteScalar();

                    var roleQueryResult = UserConnector.GetPortalRoles(username);

                    for (var index = 0; index < roleQueryResult.TargetClassCount; index++)
                    {
                        var roleId = roleQueryResult[index, "RoleID"];
                        var roleName = roleQueryResult[index, "RoleName"];

                        var roleCommand = connection.CreateCommand();
                        roleCommand.Transaction = transaction;
                        roleCommand.CommandText = "Insert into [BlackboardUserRoles] (roletype, studentid, BBRoleID, BBRoleName, exclusion, dateadded) VALUES (1, @studentId, @roleId, @RoleName,0, getdate())";
                        roleCommand.Parameters.Add(
                            new SqlParameter("@studentId", studentId)
                        );
                        roleCommand.Parameters.Add(
                            new SqlParameter("@roleId", roleId)
                        );
                        roleCommand.Parameters.Add(
                            new SqlParameter("@RoleName", roleName)
                        );
                        roleCommand.ExecuteNonQuery();
                    }


                    transaction.Commit();

                    var student = (from s in db.Students where s.USERNAME == username select s).FirstOrDefault();
                    return student;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        /*
 
        function BBGetStudentFieldId(byref config, gsmufield, bbfield, byref msg)
            dim value
            gsmufield = lcase(gsmufield)
            value = msg.getResponseTargetClassProperty(bbfield)
            dim original_value
            original_value = value
            dim sql
            sql = ""

            select case gsmufield
                case "district"
                    sql = "SELECT distid  FROM Districts WHERE district LIKE '" & dbprepvalue(value) & "'"

                case "school"
                    sql = "SELECT locationid FROM Schools WHERE location LIKE '" & dbprepvalue(value) & "'"

                case "grade"
                    sql = "SELECT gradeid FROM [Grade Levels] WHERE grade LIKE '" & dbprepvalue(value) & "'"

            end select

            dim db, rs
            if sql <> "" then

                if trim(value ) = "" then
                    BBGetStudentFieldId = 0
                    Exit Function
                end if

                set db = config("db")
                set rs = db.QueryResultSet(sql)
                if rs.eof then
                    bblog.log_error "FAILED Field lookup: BB [" & bbfield & "=" & original_value & "] is GSMU [" & gsmufield & "=" & value & "]"
                    value = ""
                else
                    value = rs(0)
                    bblog.log_debug "Field lookup: BB [" & bbfield & "=" & original_value & "] is GSMU [" & gsmufield & "=" & value & "]"
                end if
            end if

            BBGetStudentFieldId = value
        end function 
 
         */
        public static string GetGsmuUserFieldValue(string gsmuField, string bbField, BlackboardResponse userDataResponse)
        {
            System.Globalization.TextInfo ti = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            var config = Configuration.Instance;

            using (var db = new SchoolEntities())
            {
                var bbValue = userDataResponse["User", ti.ToTitleCase(bbField)];
                var dbBbValue = bbValue;
                if (!string.IsNullOrEmpty(dbBbValue))
                {
                    dbBbValue = dbBbValue.ToLower().Trim();
                }
                string result = bbValue;

                switch (gsmuField.ToLower())
                {
                    case "district":
                        var district = (from d in db.Districts where d.DISTRICT1.Equals(dbBbValue, StringComparison.OrdinalIgnoreCase) select d).FirstOrDefault();
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
                        var school = (from s in db.Schools where s.LOCATION.Equals(dbBbValue, StringComparison.OrdinalIgnoreCase) select s).FirstOrDefault();
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
                        var grade = (from g in db.Grade_Levels where g.GRADE.Equals(dbBbValue, StringComparison.OrdinalIgnoreCase) select g).FirstOrDefault();
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

    }
}
