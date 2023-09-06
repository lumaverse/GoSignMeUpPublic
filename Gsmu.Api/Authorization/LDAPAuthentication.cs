using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;
using System.Net;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using entities = Gsmu.Api.Data.School.Entities;
using System.Data.SqlClient;
using System.DirectoryServices;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.User;
using System.Globalization;
using System.DirectoryServices.Protocols;
using System.Security.Cryptography.X509Certificates;

namespace Gsmu.Api.Authorization
{
    public class LDAPAuthentication
    {
        public static bool ServerCallback(LdapConnection connection, X509Certificate certificate)
        {
            return true;
        }
        private static String GetDn(LdapConnection ldap, String userName, String BaseDNN, String DNFilter)
        {
            var request = new SearchRequest(BaseDNN, string.Format(CultureInfo.InvariantCulture, DNFilter, userName), System.DirectoryServices.Protocols.SearchScope.Subtree);
            var response = (SearchResponse)ldap.SendRequest(request);
            if (response.Entries.Count == 1)
            {
                return response.Entries[0].DistinguishedName;
            }
            return null;
        }
        public static Student LdapConnProcess(Student student)
        {
            string LdapDistName = Settings.Instance.GetMasterInfo2().LDAPContext;
            string[] strArray = new string[10]
                  {
                    Settings.Instance.GetMasterInfo2().LDAPContext,
                    Settings.Instance.GetMasterInfo2().LDAPContext2,
                    Settings.Instance.GetMasterInfo3().LDAPContext3,
                    Settings.Instance.GetMasterInfo3().LDAPContext4,
                    Settings.Instance.GetMasterInfo3().LDAPContext5,
                    Settings.Instance.GetMasterInfo3().LDAPContext6,
                    Settings.Instance.GetMasterInfo3().LDAPContext7,
                    Settings.Instance.GetMasterInfo3().LDAPContext8,
                    Settings.Instance.GetMasterInfo3().LDAPContext9,
                    Settings.Instance.GetMasterInfo3().LDAPContext10
                  };
            string LdapUserNameField = Settings.Instance.GetMasterInfo2().LDAPUserNameField;
            int LDAPStudentDataIn = Settings.Instance.GetMasterInfo2().LDAPStudentDataIn; //only 1 & 11 for now

            try
            {
                if (Settings.Instance.GetMasterInfo3().LdapAuthType == 2 || Settings.Instance.GetMasterInfo3().LdapAuthType == 3)
                {
                    // create an LdapSessionOptions object to configure session 
                    LdapConnection AuthConnection = GetConnection();
                    LdapConnection ServiceConnection = null;

                    // settings on the connection to bind first
                    if (Settings.Instance.GetMasterInfo2().LDAPUseServiceAccount == 0)
                    {
                        AuthConnection.AuthType = AuthType.Anonymous;
                        AuthConnection.SessionOptions.ProtocolVersion = 3;
                        AuthConnection.Timeout = new TimeSpan(0, 0, 10, 30, 0); ;
                        AuthConnection.Bind();
                    }
                    else
                    {
                        ServiceConnection = GetConnection();

                        var SAusername = Settings.Instance.GetMasterInfo2().LDAPServiceAccountUsername;
                        if (Settings.Instance.GetMasterInfo2().LDAPServiceAccountContext != "")
                        {
                            //if using context, username must be in the node hiearchy. ie: cn=langn
                            SAusername = SAusername + "," + Settings.Instance.GetMasterInfo2().LDAPServiceAccountContext;
                        }
                        //SAusername = "CN=gosignmeupldap,OU=SERVICEACCOUNTS,OU=AUXILIARY SERVICES,DC=waketech,DC=edu";
                        var SAuserpass = Settings.Instance.GetMasterInfo2().LDAPServiceAccountPassword;
                        ServiceConnection.Credential = new NetworkCredential(SAusername, SAuserpass);
                        ServiceConnection.AuthType = AuthType.Basic;
                        ServiceConnection.Bind();
                    }
                    //search for user
                    //getDN (ldapconnection, username, DN, filter)
                    //var dn = GetDn(connection, uid, distinguishedName, "uid={0}");
                    //"ldap.grps.org" "389" "false" "cn=piechockic@grps.org,o=GRPS" "password" "o=GRPS" "(cn=piechockic@grps.org)" "cn mail sn givenName physicalDeliveryOfficeName employeeID"
                    //string host, int port, bool ssl, string uid, string pwd, string distinguishedName, string ldapSearchFilter, string[] attributeList, bool debug
                    if (LdapUserNameField == "") { LdapUserNameField = "sAMAccountName"; }

                    string ldapSearchFilter = "(" + LdapUserNameField + "=" + student.USERNAME + ")";
                    string LDAPCustomSearchObjFilter = Settings.Instance.GetMasterInfo2().LDAPCustomSearchObj;
                    //search criteria
                    LDAPCustomSearchObjFilter = string.IsNullOrEmpty(LDAPCustomSearchObjFilter) ? "&(objectClass=*)" : LDAPCustomSearchObjFilter;
                    var dn = GetDn(ServiceConnection ?? AuthConnection, student.USERNAME, LdapDistName, "(" + LDAPCustomSearchObjFilter + ldapSearchFilter + ")");

                    //var dn = GetDn(AuthConnection, student.USERNAME, LdapDistName, "(&(objectclass=inetOrgPerson)" + ldapSearchFilter + ")");
                    if (dn == null)
                    {
                        for (int index = 1; index < strArray.Length; ++index)
                        {
                            if (strArray[index] != "")
                            {
                                dn = LDAPAuthentication.GetDn(ServiceConnection ?? AuthConnection, student.USERNAME, strArray[index], "(" + LDAPCustomSearchObjFilter + ldapSearchFilter + ")");
                                if (dn != null)
                                {
                                    LdapDistName = strArray[index];
                                    break;
                                }
                            }
                        }
                    }
                    if (dn == null)
                    {
                        dn = LdapUserNameField + "=" + student.USERNAME + "," + LdapDistName;
                    }
                    //binding actual account
                    AuthConnection.AuthType = AuthType.Basic;
                    NetworkCredential credential = new NetworkCredential(dn, student.PASSWORD);
                    AuthConnection.Credential = credential;

                    if (ServiceConnection != null)
                    {
                        AuthConnection.Bind();
                    }

                    //find attribute
                    string attributeFields = "";
                    switch (LDAPStudentDataIn)
                    {
                        case 1:
                            string LdapCustomFieldName1 = Settings.Instance.GetMasterInfo2().LdapCustomFieldName1;
                            LdapCustomFieldName1 = LdapCustomFieldName1.Replace("|", " ");
                            attributeFields = LdapUserNameField + " mail sn givenName physicalDeliveryOfficeName";
                            if (LdapCustomFieldName1 != "")
                            {
                                attributeFields = attributeFields + " " + LdapCustomFieldName1;
                            }
                            break;
                        //customize for ithaca
                        case 10:
                            attributeFields = LdapUserNameField + " CN Mail sn givenName Status TelephoneNumber OU ou0 ou1";
                            break;
                        case 11:
                            string LdapCustomFieldName11 = Settings.Instance.GetMasterInfo2().LdapCustomFieldName11;
                            attributeFields = LdapUserNameField + " givenName sn mail telephoneNumber company department description title physicalDeliveryOfficeName";
                            if (LdapCustomFieldName11 != "")
                            {
                                attributeFields = attributeFields + " " + LdapCustomFieldName11;
                            }
                            break;
                    }
                    IList<string> attributeList = attributeFields.Split(' ').Reverse().ToList<string>();

                    // cast the returned directory response as a SearchResponse object
                    SearchResponse searchResponse = null;
                    if (ServiceConnection != null)
                    {
                        SearchRequest searchRequest = new SearchRequest(LdapDistName, ldapSearchFilter, System.DirectoryServices.Protocols.SearchScope.Subtree, attributeFields.Split(' '));
                        searchResponse = (SearchResponse)ServiceConnection.SendRequest(searchRequest);
                    }
                    else
                    {
                        SearchRequest searchRequest = new SearchRequest(dn, ldapSearchFilter, System.DirectoryServices.Protocols.SearchScope.Base, attributeFields.Split(' '));
                        searchResponse = (SearchResponse)AuthConnection.SendRequest(searchRequest);
                    }
                    int countResp = searchResponse.Entries.Count;

                    //parsing the data
                    if (searchResponse != null && dn != null)
                    {
                        try
                        {
                            switch (LDAPStudentDataIn)
                            {
                                case 1:
                                    student = LDAPSetOption1(student, searchResponse, attributeFields.Split(' '));
                                    break;
                                //customize for ithaca
                                case 10:
                                    student = LDAPSetOption10(student, searchResponse, attributeFields.Split(' '));
                                    break;
                                case 11:
                                    student = LDAPSetOption11(student, searchResponse, attributeFields.Split(' '));
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            student.USERNAME = student.USERNAME;
                        }
                    }

                }
                else
                {
                    //normal AD connection - no initial binding
                    string server = "";
                    if (LdapUserNameField == "") { LdapUserNameField = "sAMAccountName"; }

                    if ((Settings.Instance.GetMasterInfo3().LdapAuthSecureType == 0) && (Settings.Instance.GetMasterInfo3().LDAPport != 389))
                    {
                        server = "LDAPS://" + Settings.Instance.GetMasterInfo2().LDAPServer + "/" + Settings.Instance.GetMasterInfo2().LDAPContext;
                    }
                    else
                    {
                        server = "LDAP://" + Settings.Instance.GetMasterInfo2().LDAPServer + "/" + Settings.Instance.GetMasterInfo2().LDAPContext;
                    }
                    DirectoryEntry entry = new DirectoryEntry(server, student.USERNAME, student.PASSWORD, AuthenticationTypes.Secure);
                    object theObject = entry.NativeObject;

                    DirectorySearcher theSearcher = new DirectorySearcher(entry);
                    theSearcher.Filter = "(" + LdapUserNameField + "=" + student.USERNAME + ")";
                    //                    theSearcher.PropertiesToLoad.Add("cn");

                    if (Settings.Instance.GetMasterInfo2().LDAPStudentDataIn == 1)
                    {
                        theSearcher.PropertiesToLoad.Add(LdapUserNameField);
                        theSearcher.PropertiesToLoad.Add("givenName");
                        theSearcher.PropertiesToLoad.Add("sn");
                        theSearcher.PropertiesToLoad.Add("mail");
                        theSearcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                        //if (Settings.Instance.GetMasterInfo2().LdapCustomFieldName1.ToString() != "")
                        //{
                        //    theSearcher.PropertiesToLoad.Add(Settings.Instance.GetMasterInfo2().LdapCustomFieldName1.ToString());
                        //}
                        string LdapCustomFieldName1 = Settings.Instance.GetMasterInfo2().LdapCustomFieldName1;
                        if (LdapCustomFieldName1.Contains("|"))
                        {
                            string[] tempAttFields = LdapCustomFieldName1.Split('|');
                            foreach (string tempfield in tempAttFields)
                            {
                                theSearcher.PropertiesToLoad.Add(tempfield);
                            }
                        }
                        else if (!string.IsNullOrEmpty(LdapCustomFieldName1)) 
                        {
                            theSearcher.PropertiesToLoad.Add(Settings.Instance.GetMasterInfo2().LdapCustomFieldName1.ToString());
                        }

                    }
                    else if (Settings.Instance.GetMasterInfo2().LDAPStudentDataIn == 11)
                    {
                        theSearcher.PropertiesToLoad.Add(LdapUserNameField);
                        theSearcher.PropertiesToLoad.Add("givenName");
                        theSearcher.PropertiesToLoad.Add("sn");
                        theSearcher.PropertiesToLoad.Add("mail");
                        theSearcher.PropertiesToLoad.Add("telephoneNumber");
                        theSearcher.PropertiesToLoad.Add("company");
                        theSearcher.PropertiesToLoad.Add("department");
                        theSearcher.PropertiesToLoad.Add("title");
                        theSearcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                        if (Settings.Instance.GetMasterInfo2().LdapCustomFieldName11.ToString() != "")
                        {
                            theSearcher.PropertiesToLoad.Add(Settings.Instance.GetMasterInfo2().LdapCustomFieldName11.ToString());
                        }

                    }
                    SearchResult theResult = theSearcher.FindOne();
                    if (theResult != null)
                    {
                        try
                        {
                            if (Settings.Instance.GetMasterInfo2().LDAPStudentDataIn == 1)
                            {
                                student = LDAPSetOption1(student, theResult);
                            }

                            else if (Settings.Instance.GetMasterInfo2().LDAPStudentDataIn == 11)
                            {
                                student = LDAPSetOption11(student, theResult);
                            }
                        }
                        catch (Exception)
                        {
                            student.USERNAME = student.USERNAME;
                            student.FIRST = Convert.ToString(theResult.Properties["CN"][0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to retrieve user data. Try again.");
                    }

                }
            }
            catch (Exception e)
            {
                //All LDAP failed, now try GSMU base authentication
                //var LDAPErrorMsg = e.Message;
                if (Settings.Instance.GetMasterInfo3().AllowLoginEvenIfLDAPFails == 1)
                {
                    student = new Student();
                    student = null;
                }
                else
                {
                    throw new Exception(e.Message);
                }

            }
            if (student != null)
            {
                if (!UserDashQueries.ProcessCheckStudentUsernameExist(student.USERNAME) && Settings.Instance.GetMasterInfo2().LDAPOn !=2)
                {
                    UserInfo uinfo = new UserInfo();
                    uinfo.usergroup = "ST";
                    uinfo.usergroupAbv = "ST";
                    uinfo.username = student.USERNAME;
                    uinfo.password = student.STUDNUM;
                    //uinfo.HiddenStudRegField4 = student.PASSWORD;
                    uinfo.first = student.FIRST;
                    uinfo.last = student.LAST;
                    uinfo.email = student.EMAIL; uinfo.email = student.EMAIL;
                    if (student.DISTRICT != null) { uinfo.district = student.DISTRICT; } else { uinfo.district = 0; }
                    if (student.SCHOOL != null) { uinfo.school = student.SCHOOL; } else { uinfo.school = 0; }
                    uinfo.grade = student.GRADE;
                    uinfo.homephone = student.HOMEPHONE;
                    uinfo.workphone = student.WORKPHONE;
                    uinfo.StudRegField1 = student.StudRegField1;
                    uinfo.StudRegField2 = student.StudRegField2;
                    uinfo.StudRegField3 = student.StudRegField3;
                    uinfo.StudRegField4 = student.StudRegField4;
                    uinfo.StudRegField5 = student.StudRegField5;
                    uinfo.StudRegField6 = student.StudRegField6;
                    uinfo.StudRegField7 = student.StudRegField7;
                    uinfo.StudRegField8 = student.StudRegField8;
                    uinfo.StudRegField9 = student.StudRegField9;
                    uinfo.StudRegField10 = student.StudRegField10;
                    uinfo.userid = 0;
                    UserInfo resultui = new UserModel().SumbitUserInfo(uinfo, uinfo.usergroupAbv);
                }
                else if (Settings.Instance.GetMasterInfo2().LDAPOn == 2)
                {
                    UserInfo uinfo = new UserInfo();
                    uinfo.usergroup = "ST";
                    uinfo.usergroupAbv = "ST";
                    uinfo.username = student.USERNAME;
                    uinfo.password = student.STUDNUM;
                    //uinfo.HiddenStudRegField4 = student.PASSWORD;
                    uinfo.first = student.FIRST;
                    uinfo.last = student.LAST;
                    uinfo.email = student.EMAIL; uinfo.email = student.EMAIL;
                    if (student.DISTRICT != null) { uinfo.district = student.DISTRICT; } else { uinfo.district = 0; }
                    if (student.SCHOOL != null) { uinfo.school = student.SCHOOL; } else { uinfo.school = 0; }
                    uinfo.grade = student.GRADE;
                    if (!string.IsNullOrEmpty(student.HOMEPHONE)) { uinfo.homephone = student.HOMEPHONE;  }
                    if (!string.IsNullOrEmpty(student.WORKPHONE)) { uinfo.workphone = student.WORKPHONE; }
                    if (!string.IsNullOrEmpty(student.StudRegField1)) { uinfo.StudRegField1 = student.StudRegField1; }
                    if (!string.IsNullOrEmpty(student.StudRegField2)) { uinfo.StudRegField2 = student.StudRegField2; }
                    if (!string.IsNullOrEmpty(student.StudRegField3)) { uinfo.StudRegField3 = student.StudRegField3; }
                    if (!string.IsNullOrEmpty(student.StudRegField4)) { uinfo.StudRegField4 = student.StudRegField4; }
                    if (!string.IsNullOrEmpty(student.StudRegField5)) { uinfo.StudRegField5 = student.StudRegField5; }
                    if (!string.IsNullOrEmpty(student.StudRegField6)) { uinfo.StudRegField6 = student.StudRegField6; }
                    if (!string.IsNullOrEmpty(student.StudRegField7)) { uinfo.StudRegField7 = student.StudRegField7; }
                    if (!string.IsNullOrEmpty(student.StudRegField8)) { uinfo.StudRegField8 = student.StudRegField8; }
                    if (!string.IsNullOrEmpty(student.StudRegField9)) { uinfo.StudRegField9 = student.StudRegField9; }
                    if (!string.IsNullOrEmpty(student.StudRegField10)) { uinfo.StudRegField10 = student.StudRegField10; }
                    var tempDBCont = new SchoolEntities();
                    uinfo.userid = (from selstud in tempDBCont.Students where selstud.USERNAME == student.USERNAME select selstud.STUDENTID).FirstOrDefault();                    
                    UserInfo resultui = new UserModel().SumbitUserInfo(uinfo, uinfo.usergroupAbv);
                }
            }
            return student;
        }

        private static LdapConnection GetConnection()
        {
            string LdapHostServer = Settings.Instance.GetMasterInfo2().LDAPServer;
            int LdapHostPort = 0;
            LdapHostPort = (int)Settings.Instance.GetMasterInfo3().LDAPport;
            LdapConnection connection = new LdapConnection(new LdapDirectoryIdentifier(LdapHostServer, LdapHostPort));
            if (Settings.Instance.GetMasterInfo3().LdapAuthSecureType == 0 && LdapHostPort != 389)
            {
                connection.SessionOptions.SecureSocketLayer = true;
                connection.SessionOptions.ProtocolVersion = 3;
                connection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(ServerCallback);
            }
            return connection;
        }
        private static Student LDAPSetOption1(Student student, SearchResponse theResponse, string[] uAttributes)
        {
            try
            {
                var attrNameValue = "";
                int passwordMaskNum = Settings.Instance.GetFieldMasks("studnum", "Students").DefaultMaskNumber;
                foreach (SearchResultEntry entry in theResponse.Entries)
                {
                    foreach (var attr in uAttributes)
                    {
                        // match data with field
                        try
                        {
                            for (var i = 0; i < entry.Attributes[attr].Count; i++)
                            {
                                attrNameValue = entry.Attributes[attr][i].ToString();
                                if (attrNameValue != null && i < 1)
                                {
                                    switch (attr.ToLower())
                                    {
                                        case "cn":
                                        case "samaccountname":
                                            student.USERNAME = attrNameValue;
                                            //if (passwordMaskNum == 98)
                                            //{
                                            student.STUDNUM = "LDAP Assigned/Maintains";
                                            //}                                        
                                            break;
                                        case "mail":
                                            student.EMAIL = attrNameValue;
                                            break;
                                        case "physicaldeliveryofficename":
                                            //assign to school dropdown.
                                            if (attrNameValue != "")
                                            {
                                                List<entities.School> mySchool = Gsmu.Api.Data.School.School.Queries.GetSchoolByName(attrNameValue);
                                                if (mySchool.Count != 0)
                                                {
                                                    student.SCHOOL = mySchool.First().locationid;
                                                }
                                                else
                                                {
                                                    entities.School newSchool = new entities.School();
                                                    newSchool.LOCATION = attrNameValue;
                                                    using (var db = new SchoolEntities())
                                                    {
                                                        db.Schools.Add(newSchool);
                                                        db.SaveChanges();
                                                        try
                                                        {
                                                            student.SCHOOL = newSchool.locationid;
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                student.SCHOOL = 0;
                                            }
                                            break;
                                        case "givenname":
                                            student.FIRST = attrNameValue;
                                            break;
                                        case "sn":
                                            student.LAST = attrNameValue;
                                            break;
                                        default:
                                            // handling client custom attribute with GSMU custom field
                                            string CustomADFieldString = Settings.Instance.GetMasterInfo2().LdapCustomFieldName1.ToLower();
                                            string CustomGSMUFieldString = Settings.Instance.GetMasterInfo2().LDAPOption1CustomField1.ToLower();
                                            int countFields = CustomADFieldString.Count(f => f == '|');

                                            if (countFields > 0)
                                            {
                                                string[] arrADField = CustomADFieldString.Split('|');
                                                string[] arrGSMUField = CustomGSMUFieldString.Split('|');
                                                for (int idxFieldCount = 0; idxFieldCount < arrADField.Length; idxFieldCount++)
                                                {
                                                    if (attr.ToLower() == arrADField[idxFieldCount])
                                                    {
                                                        setValueToCustomField(student, attrNameValue, arrGSMUField[idxFieldCount]);
                                                    }
                                                }
                                            }
                                            else if (countFields == 0)
                                            {

                                                if (attr.ToLower() == Settings.Instance.GetMasterInfo2().LdapCustomFieldName1.ToLower())
                                                {
                                                    setValueToCustomField(student, attrNameValue, Settings.Instance.GetMasterInfo2().LDAPOption1CustomField1.ToLower());
                                                }
                                            } // countfield==0
                                            break;
                                    }
                                }
                                attrNameValue = "";
                            } //loop entry.Attributes
                        }
                        catch (Exception)
                        {

                        }
                    } // loop uAttributes
                }
                student.AuthFromLDAP = 1;
            }
            catch (Exception)
            {

            }
            return student;
        }
        private static void setValueToCustomField(Student student, string ADFieldValue, string GSMUfieldNum)
        {
            switch (GSMUfieldNum)
            {
                case "1":
                    string LdapCustomFieldName11 = Settings.Instance.GetMasterInfo2().LdapCustomFieldName11;
                    student.StudRegField1 = Convert.ToString(ADFieldValue);
                    break;
                case "2":
                    student.StudRegField2 = Convert.ToString(ADFieldValue);
                    break;
                case "3":
                    student.StudRegField3 = Convert.ToString(ADFieldValue);
                    break;
                case "4":
                    student.StudRegField4 = Convert.ToString(ADFieldValue);
                    break;
                case "5":
                    student.StudRegField5 = Convert.ToString(ADFieldValue);
                    break;
                case "6":
                    student.StudRegField6 = Convert.ToString(ADFieldValue);
                    break;
                case "7":
                    student.StudRegField7 = Convert.ToString(ADFieldValue);
                    break;
                case "8":
                    student.StudRegField8 = Convert.ToString(ADFieldValue);
                    break;
                case "9":
                    student.StudRegField9 = Convert.ToString(ADFieldValue);
                    break;
                case "10":
                    student.StudRegField10 = Convert.ToString(ADFieldValue);
                    break;
                case "11":
                    student.EMAIL = Convert.ToString(ADFieldValue);
                    break;
                case "12":
                    student.HOMEPHONE = Convert.ToString(ADFieldValue);
                    break;
                case "13":
                    student.WORKPHONE = Convert.ToString(ADFieldValue);
                    break;
                case "14":
                    //district
                    if (ADFieldValue != "")
                    {
                        List<entities.District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(ADFieldValue);
                        if (myDistrict.Count != 0)
                        {
                            student.DISTRICT = myDistrict.First().DISTID;
                        }
                        else
                        {
                            entities.District newDistrict = new entities.District();
                            newDistrict.DISTRICT1 = ADFieldValue;
                            using (var db = new SchoolEntities())
                            {
                                db.Districts.Add(newDistrict);
                                db.SaveChanges();
                                try
                                {
                                    student.DISTRICT = newDistrict.DISTID;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    break;
                case "15":
                    //school
                    if (ADFieldValue != "")
                    {
                        List<entities.School> mySchool = Gsmu.Api.Data.School.School.Queries.GetSchoolByName(ADFieldValue);
                        if (mySchool.Count != 0)
                        {
                            student.SCHOOL = mySchool.First().locationid;
                        }
                        else
                        {
                            entities.School newSchool = new entities.School();
                            newSchool.LOCATION = ADFieldValue;
                            using (var db = new SchoolEntities())
                            {
                                db.Schools.Add(newSchool);
                                db.SaveChanges();
                                try
                                {
                                    student.SCHOOL = newSchool.locationid;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else
                    {
                        student.SCHOOL = 0;
                    }
                    break;
                case "16":
                    //grade level
                    if (ADFieldValue != "")
                    {
                        List<entities.Grade_Level> myGradeLevel = Gsmu.Api.Data.School.Grade.Queries.GetGradeByName(ADFieldValue);
                        if (myGradeLevel.Count != 0)
                        {
                            student.GRADE = myGradeLevel.First().GRADEID;
                        }
                        else
                        {
                            entities.Grade_Level newGradeLevel = new entities.Grade_Level();
                            newGradeLevel.GRADE = ADFieldValue;
                            using (var db = new SchoolEntities())
                            {
                                db.Grade_Levels.Add(newGradeLevel);
                                db.SaveChanges();
                                try
                                {
                                    student.GRADE = newGradeLevel.GRADEID;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }          
        }
        /// <summary>
        /// this option is for Ithaca only. all other LDAP client should use option #1 or 11
        /// </summary>        
        /// <returns></returns>
        private static Student LDAPSetOption10(Student student, SearchResponse theResponse, string[] uAttributes)
        {
            try
            {
                var attrNameValue = "";
                int passwordMaskNum = Settings.Instance.GetFieldMasks("studnum", "Students").DefaultMaskNumber;
                foreach (SearchResultEntry entry in theResponse.Entries)
                {
                    foreach (var attr in uAttributes)
                    {
                        // match data with field                         
                        for (var i = 0; i < entry.Attributes[attr].Count; i++)
                        {
                            attrNameValue = entry.Attributes[attr][i].ToString();
                            if (attrNameValue != null && i < 1)
                            {
                                switch (attr.ToLower())
                                {
                                    case "uid":
                                        student.USERNAME = attrNameValue;
                                        //if (passwordMaskNum == 98)
                                        //{
                                        student.STUDNUM = "LDAP Assigned/Maintains";
                                        //}   
                                        break;
                                    case "cn":
                                        //student.USERNAME = entry.Attributes[attr][i].ToString();
                                        break;
                                    case "mail":
                                        student.EMAIL = attrNameValue;
                                        break;
                                    case "status":
                                        //assign to school dropdown.
                                        if (attrNameValue != "")
                                        {
                                            List<entities.School> mySchool = Gsmu.Api.Data.School.School.Queries.GetSchoolByName(attrNameValue);
                                            if (mySchool.Count != 0)
                                            {
                                                student.SCHOOL = mySchool.First().locationid;
                                            }
                                            else
                                            {
                                                entities.School newSchool = new entities.School();
                                                newSchool.LOCATION = attrNameValue;
                                                using (var db = new SchoolEntities())
                                                {
                                                    db.Schools.Add(newSchool);
                                                    db.SaveChanges();
                                                    try
                                                    {
                                                        student.SCHOOL = newSchool.locationid;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            student.SCHOOL = 0;
                                        }
                                        break;
                                    case "telephonenumber":
                                        student.HOMEPHONE = attrNameValue;
                                        break;
                                    case "ou":
                                        //assign to District dropdown.
                                        if (attrNameValue != "")
                                        {
                                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(attrNameValue);
                                            if (myDistrict.Count != 0)
                                            {
                                                student.DISTRICT = myDistrict.First().DISTID;
                                            }
                                            else
                                            {
                                                District newDistrict = new District();
                                                newDistrict.DISTRICT1 = attrNameValue;
                                                using (var db = new SchoolEntities())
                                                {
                                                    db.Districts.Add(newDistrict);
                                                    db.SaveChanges();
                                                    try
                                                    {
                                                        student.DISTRICT = newDistrict.DISTID;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            student.DISTRICT = 0;
                                        }
                                        break;
                                    case "ou0":
                                        student.StudRegField1 = attrNameValue;
                                        break;
                                    case "ou1":
                                        student.StudRegField2 = attrNameValue;
                                        break;
                                    case "givenname":
                                        student.FIRST = attrNameValue;
                                        break;
                                    case "sn":
                                        student.LAST = attrNameValue;
                                        break;
                                }
                            }
                            attrNameValue = "";
                        }

                    }

                }
                student.AuthFromLDAP = 1;
            }
            catch (Exception)
            {

            }
            return student;
        }

        private static Student LDAPSetOption11(Student student, SearchResponse theResponse, string[] uAttributes)
        {
            try
            {
                student.AuthFromLDAP = 1;
            }
            catch (Exception)
            {

            }
            return student;
        }
        //below options are for straight search no initial binding. - Lang
        private static Student LDAPSetOption1(Student student, SearchResult theResult)
        {
            string result = "";
            try
            {
                int passwordMaskNum = Settings.Instance.GetFieldMasks("studnum", "Students").DefaultMaskNumber;
                string LdapUserNameField = Settings.Instance.GetMasterInfo2().LDAPUserNameField;
                if (LdapUserNameField == "") { LdapUserNameField = "sAMAccountName"; }
                if (theResult.Properties[LdapUserNameField][0] != null)
                {
                    Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                    Audittrail.TableName = LdapUserNameField;
                    Audittrail.AuditDate = DateTime.Now;
                    Audittrail.RoutineName = "LDAP Log Username:" + theResult.Properties[LdapUserNameField][0];
                    Audittrail.UserName = AuthorizationHelper.CurrentUser.LoggedInUsername;

                    student.USERNAME = Convert.ToString(theResult.Properties[LdapUserNameField][0]);
                    //if (passwordMaskNum == 98)
                    //{
                    student.STUDNUM = "LDAP Assigned/Maintains";
                    //}   
                }
                if (theResult.Properties.Contains("givenName"))
                {
                    student.FIRST = Convert.ToString(theResult.Properties["givenName"][0]);
                }
                if (theResult.Properties.Contains("sn"))
                {
                    student.LAST = Convert.ToString(theResult.Properties["sn"][0]);
                }
                if (theResult.Properties.Contains("mail"))
                {
                    student.EMAIL = Convert.ToString(theResult.Properties["mail"][0]);
                }



                // handling client custom attribute with GSMU custom field
                string arrADField = Settings.Instance.GetMasterInfo2().LdapCustomFieldName1.ToLower();
                string CustomGSMUFieldString = Settings.Instance.GetMasterInfo2().LDAPOption1CustomField1.ToLower();
                int countFields = arrADField.Count(f => f == '|');

                if (countFields > 0)
                {
                    string[] tempADFields = arrADField.Split('|');
                    string[] arrGSMUField = CustomGSMUFieldString.Split('|');

                    for (int idx = 0; idx < tempADFields.Length; idx++)
                    {
                        if (theResult.Properties.Contains(tempADFields[idx]))
                        {
                            setValueToCustomField(student, Convert.ToString(theResult.Properties[tempADFields[idx]][0]), arrGSMUField[idx]);
                        }
                    }
                }
                else
                {
                    if (theResult.Properties.Contains(arrADField))
                    {
                        setValueToCustomField(student, Convert.ToString(theResult.Properties[arrADField][0]), CustomGSMUFieldString);
                    }
                }
                student.AuthFromLDAP = 1;
            }
            catch (Exception myException)
            {
                result = myException.Message + " <br /> Inner Exception:" + myException.InnerException;
            }
            return student;
        }

        private static Student LDAPSetOption10(Student student, SearchResult theResult)
        {
            try
            {
                student.AuthFromLDAP = 1;
            }
            catch (Exception)
            {
            }
            return student;
        }

        private static Student LDAPSetOption11(Student student, SearchResult theResult)
        {
            try
            {
                int passwordMaskNum = Settings.Instance.GetFieldMasks("studnum", "Students").DefaultMaskNumber;
                if (theResult.Properties["sAMAccountName"][0] != null)
                {
                    student.USERNAME = Convert.ToString(theResult.Properties["sAMAccountName"][0]);
                    //if (passwordMaskNum == 98) // not yet implement due to problem with other function yet to implement.
                    //{
                    student.STUDNUM = "LDAP Assigned/Maintains";
                    //}   
                }
                if (theResult.Properties["givenName"].Count > 0 && theResult.Properties["givenName"][0] != null)
                {
                    student.FIRST = Convert.ToString(theResult.Properties["givenName"][0]);
                }
                if (theResult.Properties["sn"].Count > 0 && theResult.Properties["sn"][0] != null)
                {
                    student.LAST = Convert.ToString(theResult.Properties["sn"][0]);
                }
                if (theResult.Properties["mail"].Count > 0 && theResult.Properties["mail"][0] != null)
                {
                    student.EMAIL = Convert.ToString(theResult.Properties["mail"][0]);
                }
                if (theResult.Properties["telephoneNumber"].Count > 0 && theResult.Properties["telephoneNumber"][0] != null)
                {
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 1)
                    {
                        student.StudRegField1 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 2)
                    {
                        student.StudRegField2 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 3)
                    {
                        student.StudRegField3 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 4)
                    {
                        student.StudRegField4 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 5)
                    {
                        student.StudRegField5 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 6)
                    {
                        student.StudRegField6 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 7)
                    {
                        student.StudRegField7 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 8)
                    {
                        student.StudRegField8 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 9)
                    {
                        student.StudRegField9 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 10)
                    {
                        student.StudRegField10 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 11)
                    {
                        student.EMAIL = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 12)
                    {
                        student.HOMEPHONE = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 13)
                    {
                        student.WORKPHONE = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 14)
                    {
                        if (Convert.ToString(theResult.Properties["telephoneNumber"][0]) != "")
                        {
                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(Convert.ToString(theResult.Properties["telephoneNumber"][0]));
                            if (myDistrict.Count != 0)
                            {
                                student.DISTRICT = myDistrict.First().DISTID;
                            }
                            else
                            {
                                District newDistrict = new District();
                                newDistrict.DISTRICT1 = Convert.ToString(theResult.Properties["telephoneNumber"][0]);
                                using (var db = new SchoolEntities())
                                {
                                    db.Districts.Add(newDistrict);
                                    db.SaveChanges();
                                    try
                                    {
                                        student.DISTRICT = newDistrict.DISTID;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                    }
                }
                if (theResult.Properties["company"].Count > 0 && theResult.Properties["company"][0] != null)
                {
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 1)
                    {
                        student.StudRegField1 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 2)
                    {
                        student.StudRegField2 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 3)
                    {
                        student.StudRegField3 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 4)
                    {
                        student.StudRegField4 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 5)
                    {
                        student.StudRegField5 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 6)
                    {
                        student.StudRegField6 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 7)
                    {
                        student.StudRegField7 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 8)
                    {
                        student.StudRegField8 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 9)
                    {
                        student.StudRegField9 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 10)
                    {
                        student.StudRegField10 = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 11)
                    {
                        student.EMAIL = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 12)
                    {
                        student.HOMEPHONE = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 13)
                    {
                        student.WORKPHONE = Convert.ToString(theResult.Properties["company"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 14)
                    {
                        if (Convert.ToString(theResult.Properties["company"][0]) != "")
                        {
                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(Convert.ToString(theResult.Properties["company"][0]));
                            if (myDistrict.Count != 0)
                            {
                                student.DISTRICT = myDistrict.First().DISTID;
                            }
                            else
                            {
                                District newDistrict = new District();
                                newDistrict.DISTRICT1 = Convert.ToString(theResult.Properties["company"][0]);
                                using (var db = new SchoolEntities())
                                {
                                    db.Districts.Add(newDistrict);
                                    db.SaveChanges();
                                    try
                                    {
                                        student.DISTRICT = newDistrict.DISTID;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                    }

                }
                if (theResult.Properties["department"].Count > 0 && theResult.Properties["department"][0] != null)
                {
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 1)
                    {
                        student.StudRegField1 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 2)
                    {
                        student.StudRegField2 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 3)
                    {
                        student.StudRegField3 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 4)
                    {
                        student.StudRegField4 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 5)
                    {
                        student.StudRegField5 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 6)
                    {
                        student.StudRegField6 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 7)
                    {
                        student.StudRegField7 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 8)
                    {
                        student.StudRegField8 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 9)
                    {
                        student.StudRegField9 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 10)
                    {
                        student.StudRegField10 = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 11)
                    {
                        student.EMAIL = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 12)
                    {
                        student.HOMEPHONE = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 13)
                    {
                        student.WORKPHONE = Convert.ToString(theResult.Properties["department"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 14)
                    {
                        if (Convert.ToString(theResult.Properties["department"][0]) != "")
                        {
                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(Convert.ToString(theResult.Properties["department"][0]));
                            if (myDistrict.Count != 0)
                            {
                                student.DISTRICT = myDistrict.First().DISTID;
                            }
                            else
                            {
                                District newDistrict = new District();
                                newDistrict.DISTRICT1 = Convert.ToString(theResult.Properties["department"][0]);
                                using (var db = new SchoolEntities())
                                {
                                    db.Districts.Add(newDistrict);
                                    db.SaveChanges();
                                    try
                                    {
                                        student.DISTRICT = newDistrict.DISTID;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                    }
                }
                if (theResult.Properties["description"].Count > 0 && theResult.Properties["description"][0] != null)
                {
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 1)
                    {
                        student.StudRegField1 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 2)
                    {
                        student.StudRegField2 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 3)
                    {
                        student.StudRegField3 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 4)
                    {
                        student.StudRegField4 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 5)
                    {
                        student.StudRegField5 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 6)
                    {
                        student.StudRegField6 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 7)
                    {
                        student.StudRegField7 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 8)
                    {
                        student.StudRegField8 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 9)
                    {
                        student.StudRegField9 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 10)
                    {
                        student.StudRegField10 = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 11)
                    {
                        student.EMAIL = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 12)
                    {
                        student.HOMEPHONE = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 13)
                    {
                        student.WORKPHONE = Convert.ToString(theResult.Properties["description"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 14)
                    {
                        if (Convert.ToString(theResult.Properties["description"][0]) != "")
                        {
                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(Convert.ToString(theResult.Properties["description"][0]));
                            if (myDistrict.Count != 0)
                            {
                                student.DISTRICT = myDistrict.First().DISTID;
                            }
                            else
                            {
                                District newDistrict = new District();
                                newDistrict.DISTRICT1 = Convert.ToString(theResult.Properties["description"][0]);
                                using (var db = new SchoolEntities())
                                {
                                    db.Districts.Add(newDistrict);
                                    db.SaveChanges();
                                    try
                                    {
                                        student.DISTRICT = newDistrict.DISTID;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                    }
                }
                if (theResult.Properties["title"].Count > 0 && theResult.Properties["title"][0] != null)
                {
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 1)
                    {
                        student.StudRegField1 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 2)
                    {
                        student.StudRegField2 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 3)
                    {
                        student.StudRegField3 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 4)
                    {
                        student.StudRegField4 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 5)
                    {
                        student.StudRegField5 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 6)
                    {
                        student.StudRegField6 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 7)
                    {
                        student.StudRegField7 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 8)
                    {
                        student.StudRegField8 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 9)
                    {
                        student.StudRegField9 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 10)
                    {
                        student.StudRegField10 = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 11)
                    {
                        student.EMAIL = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 12)
                    {
                        student.HOMEPHONE = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 13)
                    {
                        student.WORKPHONE = Convert.ToString(theResult.Properties["title"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 14)
                    {
                        if (Convert.ToString(theResult.Properties["title"][0]) != "")
                        {
                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(Convert.ToString(theResult.Properties["title"][0]));
                            if (myDistrict.Count != 0)
                            {
                                student.DISTRICT = myDistrict.First().DISTID;
                            }
                            else
                            {
                                District newDistrict = new District();
                                newDistrict.DISTRICT1 = Convert.ToString(theResult.Properties["title"][0]);
                                using (var db = new SchoolEntities())
                                {
                                    db.Districts.Add(newDistrict);
                                    db.SaveChanges();
                                    try
                                    {
                                        student.DISTRICT = newDistrict.DISTID;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                    }
                }

                if (theResult.Properties["physicalDeliveryOfficeName"].Count > 0 && theResult.Properties["physicalDeliveryOfficeName"][0] != null)
                {
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 1)
                    {
                        student.StudRegField1 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 2)
                    {
                        student.StudRegField2 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 3)
                    {
                        student.StudRegField3 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 4)
                    {
                        student.StudRegField4 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 5)
                    {
                        student.StudRegField5 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 6)
                    {
                        student.StudRegField6 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 7)
                    {
                        student.StudRegField7 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 8)
                    {
                        student.StudRegField8 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 9)
                    {
                        student.StudRegField9 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 10)
                    {
                        student.StudRegField10 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 11)
                    {
                        student.EMAIL = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 12)
                    {
                        student.HOMEPHONE = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 13)
                    {
                        student.WORKPHONE = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                    }
                    if (Settings.Instance.GetMasterInfo2().LDAPOption11CustomField1 == 14)
                    {
                        if (Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]) != "")
                        {
                            List<District> myDistrict = Gsmu.Api.Data.School.District.Queries.GetDistrictByName(Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]));
                            if (myDistrict.Count != 0)
                            {
                                student.DISTRICT = myDistrict.First().DISTID;
                            }
                            else
                            {
                                District newDistrict = new District();
                                newDistrict.DISTRICT1 = Convert.ToString(theResult.Properties["physicalDeliveryOfficeName"][0]);
                                using (var db = new SchoolEntities())
                                {
                                    db.Districts.Add(newDistrict);
                                    db.SaveChanges();
                                    try
                                    {
                                        student.DISTRICT = newDistrict.DISTID;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                    }
                }
                student.AuthFromLDAP = 1;
            }
            catch (Exception)
            {
            }
            return student;
        }


    }
}
