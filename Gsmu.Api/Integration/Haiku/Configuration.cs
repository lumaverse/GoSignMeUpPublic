using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Gsmu.Api.Data;
using Gsmu.Api.Language;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Web;

namespace Gsmu.Api.Integration.Haiku
{
    public class Configuration
    {
        public static readonly string ImportIdPrefix = "GSMU";
        public static readonly string HaikuCourseCategory = "Haiku courses";

        public static string GetImportId(GsmuEntityType type, object id, string customTextID)
        {
            string HaikuUseEmailAsImportID = System.Configuration.ConfigurationManager.AppSettings["HaikuUseEmailAsImportID"];
            //if (!string.IsNullOrEmpty(HaikuUseEmailAsImportID) && bool.Parse(HaikuUseEmailAsImportID) == true)
            if ((EnumHelper.GetEnumName(type).ToUpper() == "STUDENT" || EnumHelper.GetEnumName(type).ToUpper() == "INSTRUCTOR") && !string.IsNullOrEmpty(HaikuUseEmailAsImportID) && bool.Parse(HaikuUseEmailAsImportID) == true)
            {
                return string.Format("{0}", customTextID.ToLower());
            }
            return string.Format("{0}-{1}-{2}", ImportIdPrefix, EnumHelper.GetEnumName(type).ToUpper(), id);
        }

        public static Configuration Instance
        {
            get
            {
                var instance = ObjectHelper.GetRequestObject<Configuration>(WebContextObject.HaikuConfiguration);
                if (instance == null)
                {
                    var value = Settings.Instance.GetMasterInfo4().HaikuConfiguration;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        instance = Json.Decode<Configuration>(value);
                    }
                    if (instance == null)
                    {
                        instance = new Configuration();
                    }

                    ObjectHelper.SetRequestObject<Configuration>(WebContextObject.HaikuConfiguration, instance);
                    return instance;
                }
                return instance;
            }
        }

        public Configuration()
        {
            UserFieldMapping = DefaultUserFieldMapping;
            UseUnconfirmedEmailWhenEmailIsEmpty = true;

            SftpPort = 22;
            SftpHost = "sftp1.haikulearning.com";
            SftpUsername = "gsmu";
            SftpPassword = "Class-Register-2874";
            SftpFile = "grade_export/gsmu-sandbox/haiku_subtotal_scores.csv";
            SftpSshHostKeyFingerprint = "ssh-rsa 2048 d6:5a:1a:49:a4:31:b9:94:7b:59:1d:5f:67:67:6b:15";
        }

        public void Save()
        {
            var value = Json.Encode(this);
            using (var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
            {
                foreach (var mi4 in db.masterinfo4)
                {
                    mi4.HaikuConfiguration = value;
                }
                db.SaveChanges();
            }
            Settings.Instance.Refresh();
        }

        public bool HaikuAuthenticationEnabled
        {
            get;
            set;
        }

        public bool HaikuUserImportEnabled
        {
            get;
            set;
        }

        public bool EnableExportGoogleUser2Haiku
        {
            get;
            set;
        }

        public bool HaikuUserSynchronizationEnabled
        {
            get;
            set;
        }

        public string HaikuUrl
        {
            get;
            set;
        }

        public string OAuthServiceKey {
            get;
            set;
        }

        public string OAuthServiceSecret {
            get;
            set;
        }

        public string OAuthRequestToken {
            get;
            set;
        }

        public string OAuthRequestSecret {
            get;
            set;
        }

        public bool EnableCourseGridButtons
        {
            get;
            set;
        }

        public bool EnablePortalWelcomeScreenWidget
        {
            get;
            set;
        }

        public bool UseUnconfirmedEmailWhenEmailIsEmpty
        {
            get;
            set;
        }

        public bool ExportUserToHaikuAfterRegistration
        {
            get;
            set;
        }

        public bool ExportRosterToHaikuAfterCheckout
        {
            get;
            set;
        }

        public bool EnableRosterCancellationSynchronization
        {
            get;
            set;
        }

        public bool disableRosterNormalization
        {
            get;
            set;
        }
        
        public Dictionary<string, string> UserFieldMapping
        {
            get;
            set;
        }

        public int SftpPort
        {
            get;
            set;
        }

        public string SftpHost
        {
            get;
            set;
        }

        public string SftpUsername
        {
            get;
            set;
        }

        public string SftpPassword
        {
            get;
            set;
        }

        public string SftpFile 
        {
            get;
            set;
        }

        public string SftpSshHostKeyFingerprint
        {
            get;
            set;
        }


        /// <summary>
        /// Key: Haiku, Value: GSMU
        /// </summary>
        public Dictionary<string, string> ReservedUserFieldMapping
        {
            get
            {
                return new Dictionary<string, string>() {
                    { "Login", "USERNAME"},
                    { "Password", "STUDNUM"},
                    { "Enabled", "InActive"},
                    { "Id", "haiku_user_id"},
                    { "ImportId", "haiku_import_id"}
                };
            }
        }

        public string[] ReservedGsmuFields
        {
            get
            {
                return new string[] {
                   "USERNAME", "STUDNUM", "InActive", "haiku_user_id", "haiku_import_id"
                };
            }
        }

        public string[] ReservedHaikuFields
        {
            get
            {
                return new string[] {
                   "Login", "Password", "Enabled", "Id", "ImportId", "UserTypeString"
                };
            }
        }

        public string[] RequiredHaikuUserFields
        {
            get
            {
                return new string[] {
                   "FirstName", "LastName"
                };
            }
        }

        /// <summary>
        /// Key: Haiku, Value: GSMU
        /// </summary>
        public Dictionary<string, string> DefaultUserFieldMapping
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "Email", "EMAIL"},
                    { "FirstName", "FIRST"},
                    { "LastName", "LAST"}
                };
            }
        }

        public List<ClassFieldDescriptor> HaikuUserEntityFields
        {
            get
            {
                return ReflectionHelper.GetFieldsByAttribue(typeof(Responses.Entities.User), typeof(System.Xml.Serialization.XmlAttributeAttribute));
            }
        }

        public List<DatabaseSchemaColumnModel> GsmuStudentTableColumns
        {
            get
            {
                return Gsmu.Api.Data.DatabaseSchemaHelper.GetSchoolDatabaseTableColumnNames("Students");
            }
        }

        public Dictionary<string, string> GsmuStudentTableFieldMapLowercase
        {
            get
            {
                var masterinfo = Settings.Instance.GetMasterInfo();
                var gradeField = masterinfo.Field1Name;
                var schoolField = masterinfo.Field2Name;
                var districtField = masterinfo.Field3Name;

                var gsmuStudentTableFieldMapLowercase = new Dictionary<string, string>();

                using (var db = new SchoolEntities())
                {
                    gsmuStudentTableFieldMapLowercase =
                        (from f in db.FieldSpecs where f.TableName.ToLower() == "students" select f)
                        .ToDictionary(o => o.FieldName.ToLower(), o => o.FieldLabel)
                    ;
                    gsmuStudentTableFieldMapLowercase["grade"] = gradeField;
                    gsmuStudentTableFieldMapLowercase["school"] = schoolField;
                    gsmuStudentTableFieldMapLowercase["district"] = districtField;

                }

                return gsmuStudentTableFieldMapLowercase;
            }
        }

    }
}
