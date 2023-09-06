using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data
{
    /// <summary>
    /// Gets the data context classes for the specific databases and also provides sql connections should u want to use sql.
    /// </summary>
    public static class Connections
    {
        /// <summary>
        /// School DB sql connection.
        /// </summary>
        public static System.Data.SqlClient.SqlConnection GetSchoolConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["School"].ConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        /// <summary>
        /// Client tracking DB sql connection.
        /// </summary>
        public static System.Data.SqlClient.SqlConnection GetClientTrackingConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ClientTracking"].ConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        /// <summary>
        /// Events DB sql connection.
        /// </summary>
        public static System.Data.SqlClient.SqlConnection GetEventsConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Events"].ConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }


        /// <summary>
        /// Project log DB sql connection.
        /// </summary>
        public static System.Data.SqlClient.SqlConnection GetProjectLogConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ProjectLog"].ConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        /// <summary>
        /// Room mmanagement log DB sql connection.
        /// </summary>
        public static System.Data.SqlClient.SqlConnection GetRoomManagementConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RoomManagement"].ConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }


        /// <summary>
        /// Survey evaluations  DB sql connection.
        /// </summary>
        public static System.Data.SqlClient.SqlConnection GetSurveyConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Survey"].ConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

    }
}
