using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using haiku = Gsmu.Api.Integration.Haiku;
using web = Gsmu.Api.Web;
using winscp = WinSCP;
using Gsmu.Api.Authorization;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class HaikuController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("settings");
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult InitializeTestSettings()
        {
            var config = haiku.Configuration.Instance;
            config.HaikuAuthenticationEnabled = true;
            config.HaikuUserImportEnabled = true;
            config.EnableExportGoogleUser2Haiku = true;
            config.HaikuUserSynchronizationEnabled = true;
            config.HaikuUrl = "https://gsmu-sandbox.haikulearning.com";
            config.OAuthServiceKey = "hSGPIOF4U7kG819LD8Kw";
            config.OAuthServiceSecret = "cZyLHFCKw4QYGKDQwx8oTHDrL3SbNkNTijhLwWfCWJM";
            config.OAuthRequestToken = "NMPJBJDKK5xqCVqQuZ8og";
            config.OAuthRequestSecret = "Z6I2oMUTT4jUmum2ay38w79vSuHBXFSeiv3oh3FOWY";
            config.EnableCourseGridButtons = true;
            config.EnablePortalWelcomeScreenWidget = true;
            config.EnableRosterCancellationSynchronization = true;
            config.disableRosterNormalization = true;
            config.ExportRosterToHaikuAfterCheckout = true;
            config.ExportUserToHaikuAfterRegistration = true;
            config.UseUnconfirmedEmailWhenEmailIsEmpty = true;

            config.SftpPort = 22;
            config.SftpHost = "sftp1.haikulearning.com";
            config.SftpUsername = "gsmu";
            config.SftpPassword = "Class-Register-2874";
            config.SftpFile = "grade_export/gsmu-sandbox/haiku_subtotal_scores.csv";
            config.SftpSshHostKeyFingerprint = "ssh-rsa 2048 d6:5a:1a:49:a4:31:b9:94:7b:59:1d:5f:67:67:6b:15";

            config.Save();

            return new JsonResult()
            {
                 JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                 Data = haiku.Configuration.Instance
            };
        }

        [HttpPost]
        public ActionResult SaveSettings(string haikuUrl, string oAuthServiceKey, string oAuthServiceSecret, string oAuthRequestToken, string oAuthRequestSecret, bool haikuUserImportEnabled = false, bool EnableExportGoogleUser2Haiku = false, bool haikuUserSynchronizationEnabled = false, bool haikuAuthenticationEnabled = false, bool enableCourseGridButtons = false, bool enablePortalWelcomeScreenWidget = false, bool exportUserToHaikuAfterRegistration = false, bool exportRosterToHaikuAfterCheckout = false, string userFieldMapping = null, bool enableRosterCancellationSynchronization = false, bool disableRosterNormalization = false, bool useUnconfirmedEmailWhenEmailIsEmpty = false, int sftpPort = 22, string sftpHost = null, string sftpUsername = null, string sftpPassword = null, string sftpFile = null, string sftpSshHostKeyFingerprint = null)
        {
            bool success = true;
            string message = string.Empty;
            try
            {
                var config = haiku.Configuration.Instance;
                config.HaikuAuthenticationEnabled = haikuAuthenticationEnabled;
                config.HaikuUserSynchronizationEnabled = haikuUserSynchronizationEnabled;
                config.HaikuUserImportEnabled = haikuUserImportEnabled;
                config.EnableExportGoogleUser2Haiku = EnableExportGoogleUser2Haiku;
                config.HaikuUrl = haikuUrl;
                config.OAuthServiceKey = oAuthServiceKey;
                config.OAuthServiceSecret = oAuthServiceSecret;
                config.OAuthRequestToken = oAuthRequestToken;
                config.OAuthRequestSecret = oAuthRequestSecret;
                config.EnableCourseGridButtons = enableCourseGridButtons;
                config.EnablePortalWelcomeScreenWidget = enablePortalWelcomeScreenWidget;
                config.ExportUserToHaikuAfterRegistration = exportUserToHaikuAfterRegistration;
                config.ExportRosterToHaikuAfterCheckout = exportRosterToHaikuAfterCheckout;
                config.EnableRosterCancellationSynchronization = enableRosterCancellationSynchronization;
                config.disableRosterNormalization = disableRosterNormalization;
                config.UseUnconfirmedEmailWhenEmailIsEmpty = useUnconfirmedEmailWhenEmailIsEmpty;
                config.SftpPort = sftpPort;
                config.SftpHost = sftpHost;
                config.SftpUsername = sftpUsername;
                config.SftpPassword = sftpPassword;
                config.SftpFile = sftpFile;
                config.SftpSshHostKeyFingerprint = sftpSshHostKeyFingerprint;

                var userFieldMappingDictionary = Gsmu.Api.Language.StringHelper.ConvertDashAndEqualSignStringToStringDictionary(userFieldMapping);
                if (userFieldMappingDictionary == null)
                {
                    config.UserFieldMapping.Clear();
                }
                else
                {
                    config.UserFieldMapping = userFieldMappingDictionary;
                }

                config.Save();
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return new JsonResult() {
                Data = new {
                    config = haiku.Configuration.Instance,
                    success = success,
                    message = message
                }
            };
        }

        public ActionResult HaikuDebugRequest(string debugq, string method, string url, string debugquery)
        {
            object result = null;
            //getDebugResponse(string debugMethod = "get", string debugRequestURL = "test/ping", string debugQuery = null)
            result = haiku.HaikuImport.syncGoogleAct2HaikutUponAuth(method, url, debugq);
            //result = haiku.HaikuImport.getDebugResponse(method, url, debugq);
            return new ContentResult()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(result),
                ContentType = "application/json"
            };
        }
        public ActionResult HaikuRequest(string method)
        {
            string json = string.Empty;
            object result = null;
            switch (method.ToLower())
            {
                case "authenticate":
                    result = haiku.HaikuClient.Authenticate(Request["username"], Request["password"]);
                    break;

                case "ping":
                    result = haiku.HaikuClient.Ping();
                    break;

                case "listusers":
                    result = haiku.HaikuImport.ListUsers();
                    break;

                case "throttle":
                    result = haiku.HaikuClient.ThrottleTest();
                    break;

                case "listcourses":
                    result = haiku.HaikuImport.ListClasses();
                    break;

                case "synchronizecourses":
                    result = haiku.HaikuImport.SynchronizeCourses();
                    break;

                case "listcourses4roster":
                    result = haiku.HaikuImport.ListCourseRosterInformation();
                    break;

                case "synchronizerosters":
                    result = haiku.HaikuImport.SynchronizeRoster();
                    break;

                case "exportcourse":
                    result = haiku.HaikuExport.SynchronizeCourse(int.Parse(Request["courseid"]));
                    break;

                case "exportroster":
                    result = haiku.HaikuExport.SynchronizeRoster(int.Parse(Request["courseid"]));
                    break;

                case "importuser":
                    result = haiku.HaikuImport.SynchronizeStudent(int.Parse(Request["userid"]));
                    break;

                case "exportuser":
                    result = haiku.HaikuExport.SynchronizeStudent(int.Parse(Request["userid"]));
                    break;

                case "synchronizeorder":
                    result = haiku.HaikuExport.SynchronizeOrder(Request["ordernumber"]);
                    break;

                default:
                    throw new Exception(
                        string.Format("Invalid Haiku service request: {0}", method)
                    ); 
            }

            return new ContentResult()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(result),
                ContentType = "application/json"
            };
        }

        public ActionResult HaikuCsvImport(HttpPostedFileBase csvFile)
        {
            var success = true;
            var message = "The upload file has to be a name as ending as '.csv'.";
            try
            {
                if (csvFile.FileName.ToLowerInvariant().EndsWith(".csv"))
                {
                    message = "Valid upload. " + csvFile.ContentLength.ToString();
                    message += DoCsvImport(csvFile.InputStream);
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("The upload file has to be a name as ending as '.csv'. There was an error saving the data information: {0}", e.Message);
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = success,
                    message = message
                }
            };
        }

        public ActionResult HaikuSftp()
        {
            var success = true;
            var message = "The download file SFTP from Haiku. ";
            var file = System.Web.HttpContext.Current.Server.MapPath("~/Temp/") + Guid.NewGuid() + ".csv";
            FileStream fileStream = null;

            try
            {
                var config = haiku.Configuration.Instance;

                // WinSCP
                //config.SftpHost
                //config.SftpPort
                //config.SftpUsername
                //config.SftpPassword
                //config.SftpFile
                winscp.SessionOptions sessionOptions = new winscp.SessionOptions
                {
                    Protocol = winscp.Protocol.Sftp,
                    HostName = config.SftpHost,
                    UserName = config.SftpUsername,
                    Password = config.SftpPassword,
                    SshHostKeyFingerprint = config.SftpSshHostKeyFingerprint
                };

                using (winscp.Session session = new winscp.Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    winscp.TransferOptions transferOptions = new winscp.TransferOptions();
                    transferOptions.TransferMode = winscp.TransferMode.Binary;

                    winscp.TransferOperationResult transferResult;

                    //transferResult = session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);
                    transferResult = session.GetFiles(config.SftpFile, file);

                    // Throw on any error
                    transferResult.Check();

                    foreach (winscp.TransferEventArgs transfer in transferResult.Transfers)
                    {
                        message += string.Format(" Upload {0} SFTP to file {1} succeeded.", transfer.FileName, file);
                    }

                    fileStream = new FileStream(file, FileMode.Open);

                    message += " " +  DoCsvImport(fileStream);
                }
            }
            catch (Exception e)
            {
                success = false;
                message = string.Format("The upload file SFTP there was an error saving the data information: {0}. ", e.Message);
            }
            finally
            {
                if (fileStream != null && System.IO.File.Exists(file))
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    message += string.Format(" File upload is deleted {0}.", file);
                    System.IO.File.Delete(file);
                }
            }


            return new JsonResult()
            {
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = new
                {
                    success = success,
                    message = message
                }
            };
        }

        private string DoCsvImport(Stream stream)
        {
            return " " + haiku.Import.HaikuCsvImport.ReportCsvStringFromGrades(stream);
        }
    }
}
