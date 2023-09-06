using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Web;
using Gsmu.Api.Language;
using System.Data.SqlClient;

namespace Gsmu.Api.Data
{
    /// <summary>
    /// Provides info for settings.
    /// The settings are cached, call the Refresh() method to update the settings after an update.
    /// If you want to just update one item, you can access the miBySubsite dictioanries internally and clear the appropriate one
    /// and then just get it.
    /// The refresh method is called from the global.asax.cs beginrequest method, to clear the data on every request,
    /// for the lifetime of a request we use the same records, 
    /// we should call refresh if we know the data has been updated, when using the class.
    /// </summary>
    public class Settings
    {
        public static Settings Instance
        {
            get
            {
                var instance = ObjectHelper.GetRequestObject<Settings>(WebContextObject.Settings);
                if (instance == null)
                {
                    ObjectHelper.SetRequestObject<Settings>(WebContextObject.Settings, new Settings());
                    return Instance;
                }
                return instance;
            }
        }

        /// <summary>
        /// Holds the references to the masterinfo by subsite id, if no subsite is specified the subsite id is null
        /// </summary>
        internal Dictionary<int, MasterInfo> miBySubsite = new Dictionary<int, MasterInfo>();

        /// <summary>
        /// Holds the references to the masterinfo2 by subsite id, if no subsite is specified the subsite id is null
        /// </summary>
        internal Dictionary<int, MasterInfo2> miBySubsite2 = new Dictionary<int, MasterInfo2>();

        /// <summary>
        /// Holds the references to the masterinfo3 by subsite id, if no subsite is specified the subsite id is null
        /// </summary>
        internal Dictionary<int, MasterInfo3> miBySubsite3 = new Dictionary<int, MasterInfo3>();

        /// <summary>
        /// Holds the references to the masterinfo3 by subsite id, if no subsite is specified the subsite id is null
        /// </summary>
        internal Dictionary<int, masterinfo4> miBySubsite4 = new Dictionary<int, masterinfo4>();

        /// <summary>
        /// Holds the references to the PDFHeaderFooterInfo by subsite id, if no subsite is specified the subsite id is null
        /// </summary>
        internal Dictionary<int, PDFHeaderFooterInfo> miBySubsitepdf = new Dictionary<int, PDFHeaderFooterInfo>();

        /// <summary>
        /// Clears the masterinfo cache so that next time you want to get it it refreshes.
        /// </summary>
        public void Refresh()
        {
            miBySubsite.Clear();
            miBySubsite2.Clear();
            miBySubsite3.Clear();

            miBySubsitepdf.Clear();
        }

        /// <summary>
        /// Gets the masterinfo data for the appropriate subsite. If subsite id is null it loads the first one.
        /// </summary>
        /// <param name="subsiteId">NULL or the subsite id.</param>
        /// <returns>The masterinfo data</returns>
        public MasterInfo GetMasterInfo(int subsiteId = 0)
        {
            using (var db = new SchoolEntities())
            {
                if (miBySubsite.ContainsKey(subsiteId))
                {
                    return miBySubsite[subsiteId];
                }
                IEnumerable<MasterInfo> item = (from m in db.MasterInfoes orderby m.SubSiteId descending select m).ToList();
                if (subsiteId >= 0)
                {
                    item = item.Where(i => i.SubSiteId == subsiteId);
                }
                var mi = item.First();
                miBySubsite[subsiteId] = mi;
                return GetMasterInfo(subsiteId);
            }
        }

        /// <summary>
        /// Gets the masterinfo2 data for the appropriate subsite. If subsite id is null it loads the first one.
        /// </summary>
        /// <param name="subsiteId">NULL or the subsite id.</param>
        /// <returns>The masterinfo2 data</returns>
        public MasterInfo2 GetMasterInfo2(int subsiteId = 0)
        {
            using (var db = new SchoolEntities())
            {
                if (miBySubsite2.ContainsKey(subsiteId))
                {
                    return miBySubsite2[subsiteId];
                }
                IEnumerable<MasterInfo2> item = from m in db.MasterInfo2 orderby m.SubSiteId descending select m;
                if (subsiteId >= 0) //Added the 'equal to 0' because it always return subsite 4 item whenever I pass a null or 0 value for subsite.
                {
                    item = item.Where(i => i.SubSiteId == subsiteId);
                }
                var mi = item.First();
                miBySubsite2[subsiteId] = mi;
                return GetMasterInfo2(subsiteId);
            }
        }


        /// <summary>
        /// Gets the masterinfo3 data for the appropriate subsite. If subsite id is null it loads the first one.
        /// </summary>
        /// <param name="subsiteId">NULL or the subsite id.</param>
        /// <returns>The masterinfo3 data</returns>
        public MasterInfo3 GetMasterInfo3(int subsiteId = 0)
        {
            using (var db = new SchoolEntities())
            {
                if (miBySubsite3.ContainsKey(subsiteId))
                {
                    return miBySubsite3[subsiteId];
                }
                IEnumerable<MasterInfo3> item = from m in db.MasterInfo3 orderby m.SubSiteId descending select m;
                if (subsiteId >= 0)
                {
                    item = item.Where(i => i.SubSiteId == subsiteId);
                }
                var mi = item.First();
                miBySubsite3[subsiteId] = mi;
                return GetMasterInfo3(subsiteId);
            }
        }

        /// <summary>
        /// Gets the masterinfo4 data for the appropriate subsite. If subsite id is null it loads the first one.
        /// </summary>
        /// <param name="subsiteId">NULL or the subsite id.</param>
        /// <returns>The masterinfo3 data</returns>
        public masterinfo4 GetMasterInfo4(int subsiteId = 0)
        {
            using (var db = new SchoolEntities())
            {
                if (miBySubsite4.ContainsKey(subsiteId))
                {
                    return miBySubsite4[subsiteId];
                }
                IEnumerable<masterinfo4> item = from m in db.masterinfo4 orderby m.subsiteid descending select m;
                if (subsiteId > 0)
                {
                    item = Enumerable.Where(from m in db.masterinfo4 orderby m.subsiteid descending select m, i => i.subsiteid == subsiteId);
                }
                var mi = Enumerable.First(from m in db.masterinfo4 orderby m.subsiteid descending select m);
                this.miBySubsite4[subsiteId] = mi;
                return GetMasterInfo4(subsiteId);
            }
        }

        /// <summary>
        /// Gets the PDFHeaderFooterInfo data for the appropriate subsite. If subsite id is null it loads the first one.
        /// </summary>
        /// <param name="subsiteId">NULL or the subsite id.</param>
        /// <returns>The PDFHeaderFooterInfo data</returns>
        public PDFHeaderFooterInfo GetPDFHeaderFooterInfo(int subsiteId = 0)
        {
            using (var db = new SchoolEntities())
            {
                if (miBySubsitepdf.ContainsKey(subsiteId))
                {
                    return miBySubsitepdf[subsiteId];
                }
                IEnumerable<PDFHeaderFooterInfo> item = from m in db.PDFHeaderFooterInfoes orderby m.SubSiteId descending select m;
                if (subsiteId >= 0)
                {
                    item = item.Where(i => i.SubSiteId == subsiteId);
                }
                var mi = item.First();
                this.miBySubsitepdf[subsiteId] = mi;
                return GetPDFHeaderFooterInfo(subsiteId);
            }
        }

        /// <summary>
        /// Gets the Field Label by Fieldname and Tablename.
        /// </summary>
        /// <param name="tablename">NULL = Students</param>
        /// <param name="fieldname">must not Null</param>
        /// <returns>FieldLabel</returns>
        public FieldSpec GetFieldSpecs(string fieldname = null, string tablename = "Students")
        {
            using (var db = new SchoolEntities())
            {
                try
                {
                    IEnumerable<FieldSpec> item = from fs in db.FieldSpecs where fs.FieldName == fieldname && fs.TableName == tablename select fs;
                    var mi = item.First();
                    return mi;
                }
                catch
                {
                    var itemz = new FieldSpec();
                    return itemz;
                }
            }
        }

        /// <summary>
        /// Gets the Field Masks by Fieldname and Tablename.
        /// </summary>
        /// <param name="tablename">NULL = Students</param>
        /// <param name="fieldname">must not Null</param>
        /// <returns>FieldLabel</returns>
        public FieldMask GetFieldMasks(string fieldname = null, string tablename = "Students")
        {
            using (var db = new SchoolEntities())
            {
                try
                {
                    IEnumerable<FieldMask> item = from fm in db.FieldMasks where fm.FieldName == fieldname && fm.TableName == tablename select fm;
                    var mi = item.First();
                    return mi;
                }
                catch
                {
                    var itemz = new FieldMask();
                    return itemz;
                }
            }
        }

        public static bool HideLocationName
        {
            get
            {
                return GetVbScriptBoolValue(Instance.GetMasterInfo2().HideLocationName);
            }
        }

        public string GetPubDateFormat()
        {
            string PubDateFormat = "M/d/yyyy";
            int? usePubDateFormat = Settings.Instance.GetMasterInfo2().usePubDateFormat;
            switch (usePubDateFormat)
            {
                case 1:
                    PubDateFormat = "ddd, MMMM d, yyyy";
                    break;
                case 2:
                    PubDateFormat = "d/M/yyyy";
                    break;
            }
            return PubDateFormat;
        }

        public string GetJsPubDateFormat()
        {
            string jsPubDateFormat = "m/d/Y";
            int? usePubDateFormat = Settings.Instance.GetMasterInfo2().usePubDateFormat;
            switch (usePubDateFormat)
            {
                case 1:
                    jsPubDateFormat = "D M d, Y";
                    break;
                case 2:
                    jsPubDateFormat = "d/m/Y";
                    break;
            }
            return jsPubDateFormat;
        }

        public static bool GetVbScriptBoolValue(int? value)
        {
            if (value == null)
            {
                return false;
            }
            return value == 1 || value == -1;
        }

        public static string GetStringOrDefault(string value, string defaultvalue) {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultvalue;
            }
            else
            {
                return value;
            }
        }

        public void SetMasterinfoValue(int masterinfoId, string key, string value)
        {
            using (var db = Connections.GetSchoolConnection())
            {
                key = StringHelper.KeepAlphaNumberUnderscoreAndDash(key);
                db.Open();
                var query = string.Format("UPDATE masterinfo" + (masterinfoId > 1 ? masterinfoId.ToString() : "") + " SET {0} = @value", key);
                var cmd = db.CreateCommand();
                cmd.CommandText = query;
                cmd.Parameters.Add(
                    new SqlParameter("@value", value)
                );
                var result = cmd.ExecuteNonQuery();
            }
            this.Refresh();
        }

        //only usable if the edbmx did not properly updated
        public string GetFieldValueFromDbByQuery(string fieldName, string tableName, string where = "")
        {
            string query = "SELECT " + fieldName + " FROM " + tableName + " " + where + ";";
            using (var connection = Connections.GetSchoolConnection())
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.Connection = connection;
                cmd.CommandText = query;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var index = reader.GetOrdinal(fieldName);
                        return reader.GetValue(index).ToString();
                    }
                    reader.Close();
                }

                connection.Close();
            }
            return string.Empty;
        }

        public void SaveChanges()
        {
            using (var db = new SchoolEntities())
            {
                foreach (var mi in this.miBySubsite.Values)
                {
                    db.MasterInfoes.Attach(mi);
                    db.Entry(mi).State = System.Data.Entity.EntityState.Modified;
                }
                foreach (var mi2 in this.miBySubsite2.Values)
                {
                    db.MasterInfo2.Attach(mi2);
                    db.Entry(mi2).State = System.Data.Entity.EntityState.Modified;
                }
                foreach (var mi3 in this.miBySubsite3.Values)
                {
                    db.MasterInfo3.Attach(mi3);
                    db.Entry(mi3).State = System.Data.Entity.EntityState.Modified;
                }
                foreach (var mi4 in this.miBySubsite4.Values)
                {
                    db.masterinfo4.Attach(mi4);
                    db.Entry(mi4).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
    }
}
