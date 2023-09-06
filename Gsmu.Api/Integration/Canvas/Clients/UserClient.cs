using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using http = System.Net.Http;

namespace Gsmu.Api.Integration.Canvas.Clients
{
    public class UserClient
    {
        public static Response GetUser(Int64 userId)
        {
            var url = string.Format("/api/v1/users/{0}/profile", userId);
            var response = CanvasClient.GetResponse("GetUser",http.HttpMethod.Get, url);
            return response;
        }

        public static string GetMainEmailFromCommunicationChannel(Int64 userId)
        {
            var response = GetCommunicationChannels(userId);
            string email = (from ch in response.CommunicationChannels where ch.Type == Entities.CommunicationChannelType.email orderby ch.Position ascending select ch.Address).FirstOrDefault();

            return email;
        }

        public static Response GetCommunicationChannels(Int64 userId)
        {
            var url = string.Format("/api/v1/users/{0}/communication_channels", userId);
            var response = CanvasClient.GetResponse("GetCommunicationChannels",http.HttpMethod.Get, url);
            return response;
            
        }

        public static Response GetSupervisorObservee(Int64 userId)
        {
            var url = string.Format("/api/v1/users/{0}/observees", userId);
            var response = CanvasClient.GetResponse("GetSupervisorObservee", http.HttpMethod.Get, url);
            return response;
        }

        public static Response SearchCanvasUserByUserName(string currentusername)
        {
            Response response;
            try
            {
                if (string.IsNullOrWhiteSpace(currentusername))
                {
                    currentusername = "nothing to search";
                }
                var url = string.Format("/api/v1/users/sis_login_id:{0}/profile", currentusername);
                response = CanvasClient.GetResponse("SearchCanvasUserByUserName", http.HttpMethod.Get, url);
            }
            catch
            {
                response = null;
            }
            return response;
        }

        public static Response SearchCanvasUserBySisUserID(string currentuserID)
        {
            Response response;
            try
            {
                if (string.IsNullOrWhiteSpace(currentuserID))
                {
                    currentuserID = "nothing to search";
                }
                var url = string.Format("/api/v1/users/sis_user_id:{0}/profile", currentuserID);
                response = CanvasClient.GetResponse("SearchCanvasUserBySisUserID", http.HttpMethod.Get, url);
            }
            catch
            {
                response = null;
            }
            return response;
        }

        public static Response ListEnrollments(Int64 userId)
        {
            return EnrollmentClient.ListUserEnrollments(userId);
        }

        public static Response InsertObservee(Int64 parentID, Int64 observeeID)
        {
            var url = string.Format("/api/v1/users/{0}/observees/{1}", parentID, observeeID);
            NameValueCollection query = new NameValueCollection();
            //query["root_account_id"] = Configuration.Instance.CanvasAccountId.ToString();
            var response = CanvasClient.GetResponse("InsertObservee_" + observeeID + "_" + parentID, http.HttpMethod.Put, url);
            return response;
        }

        public static Response InsertUser(Entities.User user, string password)
        {
            var url = string.Format("/api/v1/accounts/{0}/users", Configuration.Instance.CanvasAccountId);
            NameValueCollection query = new NameValueCollection();
            query["user[name]"] = user.Name;
            query["user[short_name]"] = user.ShortName;
            query["user[sortable_name]"] = user.SortableName;
            query["user[terms_of_use]"] = "true";
            query["user[skip_registration]"] = "true";
            query["pseudonym[unique_id]"] = user.SisLoginId;
            query["pseudonym[sis_user_id]"] = user.SisUserId;
            query["pseudonym[password]"] = password;
            query["pseudonym[password_confirmation]"] = password;
            query["pseudonym[send_confirmation]"] = "false";
            if (!string.IsNullOrEmpty(Configuration.Instance.canvasAuthProdiverId.ToString()))
            {
                query["pseudonym[authentication_provider_id]"] = Configuration.Instance.canvasAuthProdiverId.ToString();
            }
            query["communication_channel[type]"] = "email";
            query["communication_channel[address]"] = user.PrimaryEmail;
            var response = CanvasClient.GetResponse("InsertUser_0_" + user.SisUserId, http.HttpMethod.Post, url, query);
            return response;
        }

        public static Response UpdateUser(Entities.User user, string UserType)
        {
            // to modify login, see login documentation
            var url = string.Format("/api/v1/users/{0}", user.Id);
            NameValueCollection query = new NameValueCollection();
            query["user[name]"] = user.Name;
            query["user[short_name]"] = user.ShortName;
            query["user[sortable_name]"] = user.SortableName;
            query["user[email]"] = user.PrimaryEmail;
            var response = CanvasClient.GetResponse("UpdateUser_" + UserType + "_" + user.Id, http.HttpMethod.Put, url, query);
            //update SIS_user_id
            if (Canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "" && Canvas.Configuration.Instance.UserGSMUCanvasUniqueIdentifierField != "username")
            {
                var lookUpUserLogin = GetUserLoginDetails(Convert.ToInt64(user.Id));
                //string existingCanvasUserLoginId = lookUpUserInCanvas.User.SisUserId.ToString();
                var objCanvasLoginDetail = lookUpUserLogin.CanvasLoginDetails;
                var mainLoginDetail = (from e in objCanvasLoginDetail select e).FirstOrDefault();
                if (mainLoginDetail != null)
                {
                    Int64 UserLoginDetailID = Convert.ToInt64(mainLoginDetail.LD_Id);
                    url = string.Format("/api/v1/accounts/{0}/logins/{1}", Configuration.Instance.CanvasAccountId, UserLoginDetailID);
                    query.Clear();
                    query["login[unique_id]"] = user.SisLoginId;
                    query["login[sis_user_id]"] = user.SisUserId;
                    var response2 = CanvasClient.GetResponse("UpdateUserLogin", http.HttpMethod.Put, url, query);
                }

            }
            return response;
        }
        public static Response UpdateUserPass(Entities.User user)
        {
            try
            {
                var lookUpUserLogin = GetUserLoginDetails(Convert.ToInt64(user.Id));
                var objCanvasLoginDetail = lookUpUserLogin.CanvasLoginDetails;
                var mainLoginDetail = (from e in objCanvasLoginDetail select e).FirstOrDefault();
                if (mainLoginDetail != null)
                {
                    Int64 UserLoginDetailID = Convert.ToInt64(mainLoginDetail.LD_Id);
                    var url = string.Format("/api/v1/accounts/{0}/logins/{1}", Configuration.Instance.CanvasAccountId, UserLoginDetailID);
                    NameValueCollection query = new NameValueCollection();
                    query["login[password]"] = user.cpass;
                    var response = CanvasClient.GetResponse("UpdateUserPass", http.HttpMethod.Put, url, query);
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                var TurnOnDebugTracingMode = System.Configuration.ConfigurationManager.AppSettings["TurnOnDebugTracingMode"];
                if (TurnOnDebugTracingMode != null)
                {
                    if (TurnOnDebugTracingMode.ToLower() == "on")
                    {
                        Gsmu.Api.Data.School.Entities.AuditTrail Audittrail = new Gsmu.Api.Data.School.Entities.AuditTrail();
                        Audittrail.TableName = "User";
                        Audittrail.AuditDate = DateTime.Now;
                        Audittrail.RoutineName = "User - Update user Password" + Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.LoggedInUserType;
                        Audittrail.UserName = Gsmu.Api.Authorization.AuthorizationHelper.CurrentUser.LoggedInUsername;
                        try
                        {
                            Audittrail.AuditAction = "Unable to update Password of account: " + Convert.ToInt64(user.Id);
                        }
                        catch { }
                        Gsmu.Api.Logging.LogManagerDispossable LogManager = new Api.Logging.LogManagerDispossable();
                        LogManager.LogSiteActivity(Audittrail);
                    }
                }
                return null;
            }
            
        }
        public static Response GetUserLoginDetails(Int64 userId)
        {
            var url = string.Format("/api/v1/users/{0}/logins", userId);
            var response = CanvasClient.GetResponse("GetUserLoginDetails", http.HttpMethod.Get, url);
            return response;
        }
    }
}
