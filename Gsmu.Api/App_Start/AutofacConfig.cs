using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Autofac;
using Autofac.Integration.WebApi;
using Gsmu.Service.BusinessLogic.Admin.Dashboard;
using Gsmu.Service.Interface.Admin.Dashboard;
using Gsmu.Service.BusinessLogic.Courses;
using Gsmu.Service.Interface.Courses;
using Gsmu.Service.BusinessLogic.Security.Authentication;
using Gsmu.Service.Interface.Security.Authentication;
using Gsmu.Service.BusinessLogic.Global.Settings;
using Gsmu.Service.Interface.Admin.Global;
using Gsmu.Service.Interface.Students;
using Gsmu.Service.BusinessLogic.Students;
using Gsmu.Service.BusinessLogic.Admin.Reports;
using Gsmu.Service.Interface.Admin.Reports;
using Gsmu.Service.Interface.Admin.Portal;
using Gsmu.Service.BusinessLogic.Admin.Portal;

namespace Gsmu.Service.API
{
    public class AutofacConfig
    {
        public static AutofacWebApiDependencyResolver Configure()
        {
            //@TODO : DO A GENERIC HANDLING OF THE REGISTRY TO AVOID MULTIPLE REGISTRATION OF MANAGERS AND INTERFACES
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies());
            //MANAGERS/REPOSITORIES
            #region MANAGERS

            #region ADMIN
            builder.RegisterType<MasterSettingsManager>().As<IMasterSettingsManager>().InstancePerRequest();
            builder.RegisterType<CourseDashBoardManager>().As<ICourseDashboardManager>().InstancePerRequest();
            builder.RegisterType<ReportsManager>().As<IReportsManager>().InstancePerRequest();
            builder.RegisterType<StudentManager>().As<IStudentManager>().InstancePerRequest();
            builder.RegisterType<AttendanceTakingManager>().As<IAttendanceTakingManager>().InstancePerRequest();
            #endregion

            #region PUBLIC
            builder.RegisterType<CourseGridManager>().As<ICourseGrid>().InstancePerRequest();
            builder.RegisterType<CourseDetailsManager>().As<ICourseDetails>().InstancePerRequest();
            builder.RegisterType<StudentRegistrationFieldManager>().As<IStudentRegistrationField>().InstancePerRequest();
            #endregion

            #endregion

            //AUTHENTICATION REGISTRY
            #region AUTH
            builder.RegisterType<AuthenticationManager>().As<IAuthenticationManager>().InstancePerRequest();
            #endregion

            var container = builder.Build();
            return new AutofacWebApiDependencyResolver((IContainer)container);
        }
    }
}