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
using Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas
{
    public class Configuration
    {
        public static readonly string CanvasCourseCategory = "Canvas courses";
        public static readonly string CanvasCourseSubCategory = "Canvas";

        [JsonIgnore]
        public static Configuration Instance
        {
            get
            {
                var instance = ObjectHelper.GetRequestObject<Configuration>(WebContextObject.CanvasConfiguration);
                if (instance == null)
                {
                    var value = Settings.Instance.GetMasterInfo4().CanvasConfiguration;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        instance = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(value);
                    }
                    if (instance == null)
                    {
                        instance = new Configuration();
                    }
                    ObjectHelper.SetRequestObject<Configuration>(WebContextObject.CanvasConfiguration, instance);
                    return instance;
                }
                return instance;
            }
        }


        public Configuration()
        {
            EnableOAuth2Authentication = false;
            enableGSMUMasterAuthentication = false;
            enableTeacherLoginAsStudent = false;
            DisableGSMUAuthIfUserInCanvas = false;
            HideLoginFormIfUserInCanvas = false;

        }

        public string AccessToken
        {
            get;
            set;
        }

        public string CanvasServerUrl
        {
            get;
            set;
        }

        public Uri CanvaseBaseUri
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CanvasServerUrl))
                {
                    return null;
                }
                return new Uri(CanvasServerUrl);
            }
        }

        public bool EnableOAuth2Authentication
        {
            get;
            set;
        }

        public bool enableGSMUMasterAuthentication
        {
            get;
            set;
        }
        public bool enableTeacherLoginAsStudent
        {
            get;
            set;
        }

        public bool DisableGSMUAuthIfUserInCanvas
        {
            get;
            set;
        }

        public bool HideLoginFormIfUserInCanvas
        {
            get;
            set;
        }
        public string CanvasId
        {
            get;
            set;
        }

        public string CanvasKey
        {
            get;
            set;
        }

        public int? CanvasAccountId
        {
            get;
            set;
        }

        public int? canvasAuthProdiverId
        {
            get;
            set;
        }
        
        [JsonIgnore]
        public Entities.Account Account
        {
            get
            {
                if (CanvasAccountId != null)
                {
                    var response = Clients.AccountClient.GetAccount(CanvasAccountId.Value);
                    return response.Account;
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public Entities.AuthenticationProvider CanvasAuthProvider
        {
            get
            {
                if (canvasAuthProdiverId != null)
                {
                    var response = Clients.AccountClient.GetAccountAuthProvider(canvasAuthProdiverId.Value);
                    return response.AuthenticationProvider;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Entities.CourseWorkflowState> IntegratedCourseWorkflowStates
        {
            get
            {
                return new List<Entities.CourseWorkflowState>() {
                    Entities.CourseWorkflowState.available,
                    // temporary remove unpublish
                   // Entities.CourseWorkflowState.unpublished
                };
            }
        }

        public List<Entities.EnrollmentState> IntegratedEnrollmentStates
        {
            get
            {
                return new List<Entities.EnrollmentState>() {
                    Entities.EnrollmentState.active,
                    Entities.EnrollmentState.completed
                };
            }
        }

        public List<Entities.EnrollmenType> IntegratedEnrollmentTypes
        {
            get
            {
                return new List<Entities.EnrollmenType>() {
                    Entities.EnrollmenType.Teacher,
                    Entities.EnrollmenType.Student,
                    Entities.EnrollmenType.StudentEnrollment,
                    Entities.EnrollmenType.TeacherEnrollment
                };
            }
        }

        public Dictionary<Entities.EnrollmenType, GsmuEntityType> CanvasEnrollmentToGsmuEntityMapping
        {
            get
            {
                return new Dictionary<Entities.EnrollmenType, GsmuEntityType>() {
                    {Entities.EnrollmenType.Teacher, GsmuEntityType.Instructor},
                    {Entities.EnrollmenType.Student, GsmuEntityType.Student},
                    {Entities.EnrollmenType.TeacherEnrollment, GsmuEntityType.Instructor},
                    {Entities.EnrollmenType.StudentEnrollment, GsmuEntityType.Student}
                };
            }
        }

        public bool EnableCanvasLtiAuhentication
        {
            get;
            set;
        }

        public bool UserSynchronizationInDashboard
        {
            get;
            set;
        }

        public bool ExportUserAfterRegistration
        {
            get;
            set;
        }

        public bool ExportEnrollmentAfterCheckout
        {
            get;
            set;
        }

        public bool ExportEnrollmentCancellation
        {
            get;
            set;
        }
        public bool ExportEnrollmentNotUpdateCourse
        {
            get;
            set;
        }

        public bool DisableRosterNormalizationOnExport
        {
            get;
            set;
        }

        public bool allowCanvasCourseSubAccountIntegration
        {
            get;
            set;
        }
        
        public bool allowCanvasCourseSectionIntegration
        {
            get;
            set;
        }
        public bool allowSupervisorIntegration
        {
            get;
            set;
        }
        public bool allowSyncSupStudRelationIndEnrollment
        {
            get;
            set;
        }
        public bool allowCourseSectionPerRegistration
        {
            get;
            set;
        }        
        public bool enableCourseGradeIntegration
        {
            get;
            set;
        }
        public string CourseGradeBookFinalField
        {
            get;
            set;
        }
        public string GradeBookPassValue
        {
            get;
            set;
        }
        public string GradeBookPassPercentValue
        {
            get;
            set;
        }
        public string CanvasGradeUpdateAttendance
        {
            get;
            set;
        }
        public string CanvasGradeFinalizeAttendance
        {
            get;
            set;
        }
        public string CanvasUpdateNewGrade
        {
            get;
            set;
        }
        public string CanvasTranscribeRegistration
        {
            get;
            set;
        }
        public string CourseGradeSendCertificate
        {
            get;
            set;
        }
        public string allowCanvasCustomCourseSISID
        {
            get;
            set;
        }

        public string defaultInstructorCourseRole
        {
            get;
            set;
        }
        public bool autoMapCanvasAccount
        {
            get;
            set;
        }

        public string UserCanvasUniqueIdentifierField
        {
            get;
            set;
        }

        public string UserGSMUCanvasUniqueIdentifierField
        {
            get;
            set;
        }
        
        public void Save()
        {
            var value =  Newtonsoft.Json.JsonConvert.SerializeObject(this);
            using (var db = new Gsmu.Api.Data.School.Entities.SchoolEntities())
            {
                foreach (var mi4 in db.masterinfo4)
                {
                    mi4.CanvasConfiguration = value;
                }
                db.SaveChanges();
            }
            Settings.Instance.Refresh();
        }

    }
}
