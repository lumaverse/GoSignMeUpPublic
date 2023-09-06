using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Category;
using Gsmu.Api.Networking;
using System.Web;
using Gsmu.Api.Data.School.User;
using System.Web.Helpers;
using Gsmu.Api.Data.School.Terminology;
using school = Gsmu.Api.Data.School;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Authorization;

namespace Gsmu.Api.Data.School
{
    public class DataLists
    {

        public List<Entities.School> Schools
        {
            get
            {
                int? supervisorrestrictschoolanddistrict = Settings.Instance.GetMasterInfo4().supervisorrestrictschoolanddistrict;
                var list = new List<Entities.School>();
              
                using (var db = new SchoolEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                   db.Configuration.ProxyCreationEnabled = false;

                    if (AuthorizationHelper.CurrentSupervisorUser != null)
                    {
                        if (supervisorrestrictschoolanddistrict!=0)
                        {
                            list = (from sc in db.Schools 
                                    join supc in db.SupervisorSchools on sc.locationid equals supc.SchoolID where supc.SupervisorID == AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID
                                    orderby sc.SortOrder, sc.LOCATION select sc).ToList();
                       }
                       else
                       {
                           list = (from sc in db.Schools orderby sc.SortOrder, sc.LOCATION select sc).ToList();
                       }
                    }
                    else
                    {
                        list = (from sc in db.Schools orderby sc.SortOrder, sc.LOCATION select sc).ToList();
                    }
                }
                return list;
            }
        }

        public List<Entities.Supervisor> Supervisors
        {
            get
            {
                var list = new List<Entities.Supervisor>();
                using (var db = new SchoolEntities())
                {
                    if ((AuthorizationHelper.CurrentSupervisorUser != null) && Settings.Instance.GetMasterInfo3().AssignSup2Stud == 0)
                    {

                        list = (from sc in db.Supervisors where sc.ACTIVE != 0 && sc.LAST!="" && sc.SUPERVISORID == AuthorizationHelper.CurrentSupervisorUser.SUPERVISORID orderby sc.LAST select sc).ToList();
                    }
                    else
                    {


                    list = (from sc in db.Supervisors  where sc.ACTIVE!=0 && sc.LAST!=""  orderby sc.LAST select sc).ToList();
                    }
                }
                return list;
            }
        }

        public string SchoolGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Schools.Where(d => d.locationid == Convert.ToInt32(vlu)).First().LOCATION; }
            catch { }
            return selecteditem;
        }


        public List<Grade_Level> GradeLevels
        {
            get
            {
                var list = new List<Grade_Level>();
                using (var db = new SchoolEntities())
                {
                    list = (from gl in db.Grade_Levels orderby gl.SortOrder, gl.GRADE select gl).ToList();
                }
                return list;
            }
        }

        public List<Grade_LevelForFilter> GradeLevelsForFilter
        {
            get
            {
                var list = new List<Grade_LevelForFilter>();
                using (var db = new SchoolEntities())
                {
                    list = (from gr in db.Grade_Levels
                            join sg in db.SchoolsGradeLevelsRelateds on gr.GRADEID equals sg.GradeId
                            join sc in db.Schools on sg.SchoolsId equals sc.locationid
                            orderby gr.SortOrder, gr.GRADE
                            select new Grade_LevelForFilter()
                            {
                                GRADEID = gr.GRADEID,
                                GRADE = gr.GRADE,
                                SCHOOLID = sc.locationid,
                                GradeSortOrder = gr.SortOrder
                            }
                    ).ToList();
                }
                return list;
            }
        }

        public string GradeLevelGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = GradeLevels.Where(d => d.GRADEID == Convert.ToInt32(vlu)).First().GRADE; }
            catch { }
            return selecteditem;
        }


        public List<entities.District> Districts
        {
            get
            {
                var list = new List<entities.District>();
                using (var db = new SchoolEntities())
                {
                    int? supervisorrestrictschoolanddistrict = Settings.Instance.GetMasterInfo4().supervisorrestrictschoolanddistrict;

                    if (AuthorizationHelper.CurrentSupervisorUser != null)
                    {
                        if (supervisorrestrictschoolanddistrict != 0)
                        {
                            list = (from ds in db.Districts where ds.DISTID == AuthorizationHelper.CurrentSupervisorUser.DISTRICT orderby ds.SortOrder, ds.DISTRICT1 select ds).ToList();
                        }
                        else
                        {
                            list = (from ds in db.Districts orderby ds.SortOrder, ds.DISTRICT1 select ds).ToList();
                        }
                    }
                    else
                    {
                        list = (from ds in db.Districts orderby ds.SortOrder, ds.DISTRICT1 select ds).ToList();
                    }
                }
                return list;
            }
        }

        public string DistrictGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Districts.Where(d => d.DISTID == Convert.ToInt32(vlu)).First().DISTRICT1; }
            catch {}
            return selecteditem;
        }

        public List<Distemployee> Distemployees
        {
            get
            {
                var dlist = new List<Distemployee>();
                dlist.Add(new Distemployee { vlu = "-1", txt = "Member" });
                dlist.Add(new Distemployee { vlu = "0", txt = "Non Member" });
                dlist.Add(new Distemployee { vlu = "3", txt = "Special" });
                return dlist;
            }
        }

        public string DistemployeeGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Distemployees.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }


        public List<Gender> Genders
        {
            get
            {
                var dlist = new List<Gender>();
                dlist.Add(new Gender { vlu = "1", txt = "1(Male)" });
                dlist.Add(new Gender { vlu = "2", txt = "2(Female)" });
                return dlist;
            }
        }

        public string GenderGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Genders.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }


        public List<YesNo> YesNos
        {
            get
            {
                var dlist = new List<YesNo>();
                dlist.Add(new YesNo { vlu = "Yes", txt = "Yes" });
                dlist.Add(new YesNo { vlu = "No", txt = "No" });
                return dlist;
            }
        }
 
        public string YesNoGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = YesNos.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }


        public List<Ethnicity1> Ethnicities1
        {
            get
            {
                var dlist = new List<Ethnicity1>();
                dlist.Add(new Ethnicity1 { vlu = "01", txt = "White/Not Hispanic origin" });
                dlist.Add(new Ethnicity1 { vlu = "02", txt = "Black/Not Hispanic origin" });
                dlist.Add(new Ethnicity1 { vlu = "03", txt = "Hispanic" });
                dlist.Add(new Ethnicity1 { vlu = "04", txt = "Asian or Pacific Islander" });
                dlist.Add(new Ethnicity1 { vlu = "05", txt = "American Indian/Alaskan" });
                return dlist;
            }
        }

        public string Ethnicity1GetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Ethnicities1.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }


        public List<Ethnicity2> Ethnicities2
        {
            get
            {
                var dlist = new List<Ethnicity2>();
                dlist.Add(new Ethnicity2 { vlu = "E1", txt = "Hispanic / Latino" });
                dlist.Add(new Ethnicity2 { vlu = "E2", txt = "Not Hispanic / Latino" });
                return dlist;
            }
        }

        public string Ethnicity2GetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Ethnicities2.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }

        public string GetSupervisorsText(string vlu)
        {
            string selecteditem = "";
            int supervisorid = 0;
            foreach (var sup in vlu.Split(','))
            {

                try { supervisorid = int.Parse(sup);
                selecteditem =selecteditem +  Supervisors.Where(d => d.SUPERVISORID == supervisorid).First().FIRST + " " + Supervisors.Where(d => d.SUPERVISORID == supervisorid).First().LAST +",";
                }

                catch { }
            }
            if (AuthorizationHelper.CurrentSupervisorUser != null)
            {
                selecteditem  =  AuthorizationHelper.CurrentSupervisorUser.FIRST + " " + AuthorizationHelper.CurrentSupervisorUser.LAST + ",";
            }
            return selecteditem;
        }

        public List<Race> Races
        {
            get
            {
                var dlist = new List<Race>();
                dlist.Add(new Race { vlu = "R1", txt = "American Indian or Alaskan Native" });
                dlist.Add(new Race { vlu = "R2", txt = "Asian" });
                dlist.Add(new Race { vlu = "R3", txt = "Black or African American" });
                dlist.Add(new Race { vlu = "R4", txt = "Native Hawaiian or Other Pacific Islander" });
                dlist.Add(new Race { vlu = "R5", txt = "White" });
                return dlist;
            }
        }

        public string RaceGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Races.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }


        public List<State> States
        {
            get
            {
                var dlist = new List<State>();

                dlist.Add(new State { txt = "Not Applicable", vlu = "N/A" });
                dlist.Add(new State { txt = "ALABAMA", vlu = "AL" });
                dlist.Add(new State { txt = "ALASKA", vlu = "AK" });
                dlist.Add(new State { txt = "AMERICAN SAMOA", vlu = "AS" });
                dlist.Add(new State { txt = "ARIZONA ", vlu = "AZ" });
                dlist.Add(new State { txt = "ARKANSAS", vlu = "AR" });
                dlist.Add(new State { txt = "CALIFORNIA ", vlu = "CA" });
                dlist.Add(new State { txt = "COLORADO ", vlu = "CO" });
                dlist.Add(new State { txt = "CONNECTICUT", vlu = "CT" });
                dlist.Add(new State { txt = "DELAWARE", vlu = "DE" });
                dlist.Add(new State { txt = "DISTRICT OF COLUMBIA", vlu = "DC" });
                dlist.Add(new State { txt = "FEDERATED STATES OF MICRONESIA", vlu = "FM" });
                dlist.Add(new State { txt = "FLORIDA", vlu = "FL" });
                dlist.Add(new State { txt = "GEORGIA", vlu = "GA" });
                dlist.Add(new State { txt = "GUAM ", vlu = "GU" });
                dlist.Add(new State { txt = "HAWAII", vlu = "HI" });
                dlist.Add(new State { txt = "IDAHO", vlu = "ID" });
                dlist.Add(new State { txt = "ILLINOIS", vlu = "IL" });
                dlist.Add(new State { txt = "INDIANA", vlu = "IN" });
                dlist.Add(new State { txt = "IOWA", vlu = "IA" });
                dlist.Add(new State { txt = "KANSAS", vlu = "KS" });
                dlist.Add(new State { txt = "KENTUCKY", vlu = "KY" });
                dlist.Add(new State { txt = "LOUISIANA", vlu = "LA" });
                dlist.Add(new State { txt = "MAINE", vlu = "ME" });
                dlist.Add(new State { txt = "MARSHALL ISLANDS", vlu = "MH" });
                dlist.Add(new State { txt = "MARYLAND", vlu = "MD" });
                dlist.Add(new State { txt = "MASSACHUSETTS", vlu = "MA" });
                dlist.Add(new State { txt = "MICHIGAN", vlu = "MI" });
                dlist.Add(new State { txt = "MINNESOTA", vlu = "MN" });
                dlist.Add(new State { txt = "MISSISSIPPI", vlu = "MS" });
                dlist.Add(new State { txt = "MISSOURI", vlu = "MO" });
                dlist.Add(new State { txt = "MONTANA", vlu = "MT" });
                dlist.Add(new State { txt = "NEBRASKA", vlu = "NE" });
                dlist.Add(new State { txt = "NEVADA", vlu = "NV" });
                dlist.Add(new State { txt = "NEW HAMPSHIRE", vlu = "NH" });
                dlist.Add(new State { txt = "NEW JERSEY", vlu = "NJ" });
                dlist.Add(new State { txt = "NEW MEXICO", vlu = "NM" });
                dlist.Add(new State { txt = "NEW YORK", vlu = "NY" });
                dlist.Add(new State { txt = "NORTH CAROLINA", vlu = "NC" });
                dlist.Add(new State { txt = "NORTH DAKOTA", vlu = "ND" });
                dlist.Add(new State { txt = "NORTHERN MARIANA ISLANDS", vlu = "MP" });
                dlist.Add(new State { txt = "OHIO", vlu = "OH" });
                dlist.Add(new State { txt = "OKLAHOMA", vlu = "OK" });
                dlist.Add(new State { txt = "OREGON", vlu = "OR" });
                dlist.Add(new State { txt = "PALAU", vlu = "PW" });
                dlist.Add(new State { txt = "PENNSYLVANIA", vlu = "PA" });
                dlist.Add(new State { txt = "PUERTO RICO", vlu = "PR" });
                dlist.Add(new State { txt = "RHODE ISLAND", vlu = "RI" });
                dlist.Add(new State { txt = "SOUTH CAROLINA", vlu = "SC" });
                dlist.Add(new State { txt = "SOUTH DAKOTA", vlu = "SD" });
                dlist.Add(new State { txt = "TENNESSEE", vlu = "TN" });
                dlist.Add(new State { txt = "TEXAS", vlu = "TX" });
                dlist.Add(new State { txt = "UTAH", vlu = "UT" });
                dlist.Add(new State { txt = "VERMONT", vlu = "VT" });
                dlist.Add(new State { txt = "VIRGIN ISLANDS", vlu = "VI" });
                dlist.Add(new State { txt = "VIRGINIA ", vlu = "VA" });
                dlist.Add(new State { txt = "WASHINGTON", vlu = "WA" });
                dlist.Add(new State { txt = "WEST VIRGINIA", vlu = "WV" });
                dlist.Add(new State { txt = "WISCONSIN", vlu = "WI" });
                dlist.Add(new State { txt = "WYOMING", vlu = "WY" });

                return dlist;
            }
        }

        public string StateGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = States.Where(d => d.vlu == vlu).First().txt; }
            catch { }
            return selecteditem;
        }



        public List<Country> Countries
        {
            get
            {
                var list = new List<Country>();
                using (var db = new SchoolEntities())
                {
                    list = (from cntry in db.Countries where cntry.disabled == 0 orderby cntry.countryorder select cntry).ToList();
                }
                return list;
            }
        }

        public string CountryGetTxt(string vlu)
        {
            string selecteditem = vlu;
            try { selecteditem = Countries.Where(d => d.countrycode == vlu).First().countryname; }
            catch { }
            return selecteditem;
        }


        public List<FieldSpec> FieldSpecs
        {
            get
            {
                var list = new List<FieldSpec>();
                using (var db = new SchoolEntities())
                {
                    //list = (from fs in db.FieldSpecs select fs).ToList();
                    list = (from fs in db.FieldSpecs orderby fs.FieldDisplaySortOrder select fs).ToList();
                }
                return list;
            }
        }

        public List<FieldMask> FieldMasks
        {
            get
            {
                var list = new List<FieldMask>();
                using (var db = new SchoolEntities())
                {
                    list = (from fm in db.FieldMasks select fm).ToList();
                }
                return list;
            }
        }

        public string CheckReqMissingFields(int userid = 0, string usergroup = "ST")
        {
            String ReqVal = "";
            List<UserRegFieldSpecs> dlists = AllStudentUserFields;
            string STForceResetPassAdminCreated = System.Configuration.ConfigurationManager.AppSettings["STForceResetPassAdminCreated"];

            UserWidget userwidgets = new UserWidget();
            if(usergroup=="IT"){
                userwidgets = UserWidgets("InstructorsDashViewEdit");
            }else{
                userwidgets = UserWidgets("StudentsDashViewEdit");
            }

            foreach (var item in userwidgets.WidgetItems)
            {
                string wgdtFieldName = item.FieldName;
                UserRegFieldSpecs fld = new UserRegFieldSpecs();
                try { 
                    fld = dlists.Where(f => f.FieldName == item.FieldName).First();
                    if (fld.BoolFieldRequired)
                    {

                        String vlu = GetFieldValue(userid, item.FieldName, "Students");
                        if (item.FieldName.ToLower() == "password" || item.FieldName.ToLower() == "studnum")
                        {
                            if (Settings.Instance.GetMasterInfo2().LDAPOn == 1 || Settings.Instance.GetMasterInfo2().LDAPOn == 2)
                            {
                                vlu = "LDAP Assigned/Maintains";
                            }
                            if(STForceResetPassAdminCreated == "true")
                            {
                                string ResetPassword = GetFieldValue(userid, "ResetPassword", "Students");
                                if(ResetPassword == "1")
                                {
                                    ReqVal = ReqVal + item.FieldName + ":" + item.WidgetID + "|";
                                }
                            }

                        }                        
                        if (String.IsNullOrWhiteSpace(vlu))
                        {
                        ReqVal = ReqVal + item.FieldName + ":" + item.WidgetID + "|";
                        }

                        if ((fld.FieldName == "district") && (vlu =="0"))
                        {
                            ReqVal = ReqVal + item.FieldName + ":" + item.WidgetID + "|";
                        }
                        if ((fld.FieldName == "school") && (vlu == "0"))
                        {
                            ReqVal = ReqVal + item.FieldName + ":" + item.WidgetID + "|";
                        }
                        if ((fld.FieldName == "grade") && (vlu == "0"))
                        {
                            ReqVal = ReqVal + item.FieldName + ":" + item.WidgetID + "|";
                        }
                        //ReqVal = ReqVal + item.FieldName + ":" + vlu + ">" + item.WidgetID + "|";
                    }
                }
                catch { }
            }

            if (String.IsNullOrWhiteSpace(ReqVal))
            {
                ReqVal = "NoMissingReqFields";
            }
            return ReqVal;
        }

         
        public UserWidget UserWidgets(string adminmode)
        {
                string resultui = new UserModel().InitializeUserWidgetSettings("NONE");

                if (resultui == "initialized")
                {
                    Settings.Instance.Refresh();
                }

                string json = "[]";
                if(adminmode == "StudentsDashViewEdit"){
                    json = Settings.Instance.GetMasterInfo4().StudentsDashViewEdit;
                }else if(adminmode == "StudentsDashAddnew"){
                    json = Settings.Instance.GetMasterInfo4().StudentsDashAddnew;
                }else if(adminmode == "StudentsDashAdmin"){
                    json = Settings.Instance.GetMasterInfo4().StudentsDashAdmin;

                }else if(adminmode == "InstructorsDashViewEdit"){
                    json = Settings.Instance.GetMasterInfo4().InstructorsDashViewEdit;
                }else if(adminmode == "InstructorsDashAddnew"){
                    json = Settings.Instance.GetMasterInfo4().InstructorsDashAddnew;

                }else if(adminmode == "SupervisorDashAddnew"){
                    json = GetDefaultSupervisorDashAddnew();
                }else if (adminmode == "SupervisorsDashAdmin"){
                    json = GetDefaultSupervisorDashAddnew();
                }

                //throw new System.Web.HttpException(500, "json:" + json + " adminmode:" + adminmode + " resultui:" + resultui);

                UserWidget list = Json.Decode<UserWidget>(json);
                return list;
        }


        public String GetDefaultSupervisorDashAddnew()
        {
            UserWidget uw = new UserWidget();
            var widgetitemlist = new List<WidgetItemList>();
            var widgetinfo = new List<WidgetInfo>();
            var widgetcolumn = new List<WidgetColumn>();

            DataLists dlists = new DataLists();

            //column
            widgetcolumn.Add(new WidgetColumn() { ID = 1, ColFlex = 1, WidthPer = 60, DispSort = 1 });

            //widget
            widgetinfo.Add(new WidgetInfo() { ID = 1, ColID = 1, DispSort = 1, Title = TerminologyHelper.Instance.GetTermCapital(TermsEnum.Supervisor) + " Information", WithProfileImage = false });

            //fields
            /*
            widgetitemlist.Add(new WidgetItemList() { FieldName = "first",                      ID = 1, DispSort = 1, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "last",                       ID = 2, DispSort = 2, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "username",                   ID = 3, DispSort = 3, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "password",                   ID = 4, DispSort = 4, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "title",                      ID = 5, DispSort = 5, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "address",                    ID = 6, DispSort = 6, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "city",                       ID = 7, DispSort = 7, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "state",                      ID = 8, DispSort = 8, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "zip",                        ID = 9, DispSort = 9, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "phone",                  ID = 10, DispSort = 10, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "fax",                        ID = 11, DispSort = 11, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "supervisornum",              ID = 12, DispSort = 12, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "email",                      ID = 13, DispSort = 13, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "additionalemailaddresses",   ID = 14, DispSort = 14, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "notify",                     ID = 15, DispSort = 15, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "advanceoptionsstr",          ID = 16, DispSort = 16, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "district",                 ID = 17, DispSort = 17, WidgetID = 1 });
            widgetitemlist.Add(new WidgetItemList() { FieldName = "school",                   ID = 18, DispSort = 18, WidgetID = 1 });
            */
            ///****** WARNING
            /////for now use Parents field to house custom fields
            ///****** WARNING
            try
            {
                var fs = FieldSpecs.Where(f => f.TableName == "Parents" && (f.FieldVisible == 1 || f.FieldVisible == -1) && f.FieldLabel != "").ToList();
                int id = 0;
                foreach (var item in fs)
                {

                    id = id + 1;
                    //list.Add(new UserRegFieldSpecs() { FieldName = item.FieldName, FieldLabel = item.FieldLabel, BoolFieldRequired = (item.FieldRequired == 1 || item.FieldRequired == -1) ? true : false, FieldVisible = (item.FieldVisible == 1 || item.FieldVisible == -1) ? true : false });
                    widgetitemlist.Add(new WidgetItemList() { FieldName = item.FieldName, ID = id, DispSort = item.FieldDisplaySortOrder, WidgetID = 1 });
                }
                widgetitemlist.Add(new WidgetItemList() { FieldName = "district", ID = id + 1, DispSort = 17, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "school", ID = id + 2, DispSort = 18, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "notify", ID = id + 3, DispSort = 15, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "advanceoptionsstr", ID = id + 4, DispSort = 16, WidgetID = 1 });
            }
            catch
            {
                widgetitemlist.Add(new WidgetItemList() { FieldName = "first", ID = 1, DispSort = 1, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "last", ID = 2, DispSort = 2, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "username", ID = 3, DispSort = 3, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "password", ID = 4, DispSort = 4, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "title", ID = 5, DispSort = 5, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "address", ID = 6, DispSort = 6, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "city", ID = 7, DispSort = 7, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "state", ID = 8, DispSort = 8, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "zip", ID = 9, DispSort = 9, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "phone", ID = 10, DispSort = 10, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "fax", ID = 11, DispSort = 11, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "supervisornum", ID = 12, DispSort = 12, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "email", ID = 13, DispSort = 13, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "additionalemailaddresses", ID = 14, DispSort = 14, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "notify", ID = 15, DispSort = 15, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "advanceoptionsstr", ID = 16, DispSort = 16, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "district", ID = 17, DispSort = 17, WidgetID = 1 });
                widgetitemlist.Add(new WidgetItemList() { FieldName = "school", ID = 18, DispSort = 18, WidgetID = 1 });
            }
            uw.Column = widgetcolumn;
            uw.Widgets = widgetinfo;
            uw.WidgetItems = widgetitemlist;

            return Json.Encode(uw);

        }

        public List<UserRegFieldSpecs> AllStudentUserFields
        {
            get
            {
                int countEmptyCustomfieldLbl = 0;
                var list = new List<UserRegFieldSpecs>();

                string fieldgrp = "affiliaton";
                list.Add(new UserRegFieldSpecs() { FieldName = "district", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "school", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "grade", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "supervisor", FieldGrp = fieldgrp });

                fieldgrp = "specialfield";
                list.Add(new UserRegFieldSpecs() { FieldName = "profileimage", FieldGrp = fieldgrp });

                fieldgrp = "presetfield";
                list.Add(new UserRegFieldSpecs() { FieldName = "first", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "last", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "email", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "address", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "city", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "state", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "country", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "zip", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "homephone", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "workphone", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "fax", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "studnum", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "username", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "ss", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "distemployee", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "additionalemail", FieldGrp = fieldgrp });

                list.Add(new UserRegFieldSpecs() { FieldName = "dateadded", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "date_modified", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "date_bb_integrated", FieldGrp = fieldgrp });

                fieldgrp = "customfield";
                for (int i = 1; i <= 20; i++)
                {
                    list.Add(new UserRegFieldSpecs() { FieldName = "studregfield" + i, FieldGrp = fieldgrp });
                }

                fieldgrp = "readonlyfield";
                for (int i = 1; i <= 4; i++)
                {
                    list.Add(new UserRegFieldSpecs() { FieldName = "readonlystudregfield" + i, FieldGrp = fieldgrp });
                }

                fieldgrp = "hiddenfield";
                for (int i = 1; i <= 4; i++)
                {
                    list.Add(new UserRegFieldSpecs() { FieldName = "hiddenstudregfield" + i, FieldGrp = fieldgrp });
                }

                foreach (var item in list)
                {
                    string MIfldRequired = "";
                    FieldSpec fs = new FieldSpec();
                    FieldMask fm = new FieldMask();

                    try { fs = FieldSpecs.Where(f => f.FieldName == item.FieldName && f.TableName == "Students").First(); }
                    catch { }

                    try { fm = FieldMasks.Where(f => f.FieldName == item.FieldName && f.TableName == "Students").First(); }
                    catch { }

                    //default, try to get all prop from FieldSpecs and FieldMask
                    item.FieldLabel = fs.FieldLabel;
                    item.FieldRequired = fs.FieldRequired;
                    item.FieldReadOnly = (fs.FieldReadOnly== 1 ? true : false);
                    item.FieldCustomList = fs.FieldListValue;
                    item.FieldListType = fs.FieldListType;
                    item.FieldListValue = fs.FieldListValue;
                    item.MaskTxt = item.FieldName;
                    item.FieldVisible = true;
                    item.FieldForceSelection = false;
                    item.ConfirmRequired = (fs.ConfirmRequired == 1 ? true : false); ;

                    if (item.FieldGrp == "presetfield")
                    {

                        MIfldRequired = GetFieldInfo("ReqStudent" + item.FieldName.ToUpper());
                        item.BoolFieldRequired = (MIfldRequired == "-1" || MIfldRequired == "1"  ? true : false);
                       
                        item.MaskNum = fm.DefaultMaskNumber;

                        if (item.FieldName == "state" && item.MaskNum == 1)
                        { item.MaskTxt = "2LetterStateAbbrev"; item.FieldStore = "storeState"; }

                        if (item.FieldName == "homephone" && item.MaskNum == 1)
                        { item.MaskTxt = "(###) ###-####"; }

                        if (item.FieldName == "workphone" && item.MaskNum == 1)
                        { item.MaskTxt = "(###) ###-####"; }

                        if (item.FieldName == "fax" && item.MaskNum == 1)
                        { item.MaskTxt = "(###) ###-####"; }

                        if (item.FieldName == "ss" && item.MaskNum == 1)
                        { item.MaskTxt = "###-##-####"; }

                        if (item.FieldName == "ss" && item.MaskNum == 2)
                        { item.MaskTxt = "####"; }

                        if (item.FieldName == "address")
                        { item.FieldLabel = Settings.Instance.GetMasterInfo2().PublicAddressLabel; }

                        if (item.FieldName == "country")
                        { item.FieldStore = "storeCountry"; }

                        if (item.FieldName == "distemployee")
                        { item.FieldStore = "storeDistemployee"; }

                        if (item.FieldName == "studnum")
                        { item.MaskTxt = "password"; }

                        if (item.FieldName == "username" && item.MaskNum == 97)
                        { item.MaskTxt = "emailusername"; item.BoolFieldRequired = false; }

                    }
                    else if (item.FieldGrp == "customfield")
                    {
                        item.TblFieldName = item.FieldName;
                        item.FieldLabel = GetFieldInfo(item.FieldName + "Name");
                        MIfldRequired = GetFieldInfo(item.FieldName + "Required");
                        item.BoolFieldRequired = (MIfldRequired == "-1" || MIfldRequired == "1" ? true : false);

                        item.MaskNum = fm.DefaultMaskNumber; ;

                        if (item.MaskNum == 0)
                        { item.MaskTxt = "No Mask"; item.FieldStore = ""; }

                        else if (item.MaskNum == 1)
                        { item.MaskTxt = "YYYY/MM/DD"; item.FieldStore = ""; }

                        else if (item.MaskNum == 2)
                        { item.MaskTxt = "MM/DD/YYYY"; item.FieldStore = ""; }

                        else if (item.MaskNum == 20)
                        { item.MaskTxt = "Gender"; item.FieldStore = "storeGender"; }

                        else if (item.MaskNum == 21)
                        { item.MaskTxt = "Ethnicity 1"; item.FieldStore = "storeEthnicity1"; }

                        else if (item.MaskNum == 22)
                        { item.MaskTxt = "Department"; item.FieldStore = ""; }

                        else if (item.MaskNum == 23)
                        { item.MaskTxt = "Yes/No"; item.FieldStore = "storeYesNo"; }

                        else if (item.MaskNum == 24)
                        { item.MaskTxt = "Ethnicity 2"; item.FieldStore = "storeEthnicity2"; }

                        else if (item.MaskNum == 25)
                        { item.MaskTxt = "Race"; item.FieldStore = ""; }

                        else if (item.MaskNum == 26 && item.FieldListType == 0)
                        { item.MaskTxt = "SelectionCheckbox"; item.FieldStore = ""; }

                        else if (item.MaskNum == 26 && item.FieldListType == 1)
                        { item.MaskTxt = "SelectionListSingleSelect"; item.FieldStore = ""; }

                        else if (item.MaskNum == 26 && item.FieldListType == 2)
                        { item.MaskTxt = "SelectionListMultiSelect"; item.FieldStore = ""; }

                        else
                        { item.MaskTxt = "No Mask"; }

                        countEmptyCustomfieldLbl += string.IsNullOrEmpty(item.FieldLabel) ? 1 : 0;

                        //show only 5 max empty customfield not showing all 20 if no labels - sept 19 2016
                        //show all fields ticket# 27575 july 10 2018
                        //item.FieldVisible = countEmptyCustomfieldLbl > 5 ? false : true;

                    }
                    else if (item.FieldGrp == "readonlyfield")
                    {
                        item.TblFieldName = item.FieldName;
                        item.FieldLabel = GetFieldInfo(item.FieldName + "Name");
                        MIfldRequired = GetFieldInfo(item.FieldName + "Required");
                        item.BoolFieldRequired = false;

                        item.MaskNum = 0;
                        item.MaskTxt = "readonly";
                        countEmptyCustomfieldLbl += string.IsNullOrEmpty(item.FieldLabel) ? 1 : 0;
                        item.FieldVisible = true;


                    }
                    else if (item.FieldGrp == "hiddenfield")
                    {
                        item.TblFieldName = item.FieldName;
                        item.FieldLabel = GetFieldInfo(item.FieldName + "Name");
                        MIfldRequired = GetFieldInfo(item.FieldName + "Required");
                        item.BoolFieldRequired = false;

                        item.MaskNum = 0;
                        item.MaskTxt = "hidden";
                        countEmptyCustomfieldLbl += string.IsNullOrEmpty(item.FieldLabel) ? 1 : 0;
                        item.FieldVisible = true;

                    }
                    else if (item.FieldGrp == "affiliaton")
                    {
                        MIfldRequired = GetFieldInfo("ReqStudent" + item.FieldName.ToUpper());
                        item.BoolFieldRequired = (MIfldRequired == "-1" || MIfldRequired == "1" || MIfldRequired =="2" ? true : false);

                        if (item.FieldName == "district")
                        {
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field3Name;
                            item.FieldForceSelection = true;
                            item.FieldStore = "storedistrictfield";
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubStud == 1)
                            {
                                item.MaskTxt = "districtfiltered";
                            }
                        }
                        else if (item.FieldName == "school")
                        {
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field2Name;
                            item.FieldForceSelection = true;
                            item.BoolFieldRequiredAll = (MIfldRequired == "2" ? true : false);
                            item.FieldStore = "storeschoolfield";
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubStud == 1)
                            {
                                item.MaskTxt = "schoolfiltered";
                            }
                        }
                        else if (item.FieldName == "grade")
                        {
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field1Name;
                            item.FieldForceSelection = true;
                            item.FieldStore = "gradelevelfield";
                            //added count to condition, so that all grade levels are displayed if there are no school to grade level relations setup.
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubStud == 1 && GradeLevelsForFilter.Count > 0)
                            {
                                item.FieldStore = "gradelevelfieldForFilter";
                            }
                        }
                        else if (item.FieldName == "supervisor")
                        {

                            item.FieldLabel = Gsmu.Api.Data.School.Terminology.TerminologyHelper.Instance.GetTermCapitalize(TermsEnum.Supervisor);
                            item.FieldForceSelection = true;
                            item.BoolFieldRequiredAll = false;
                            item.FieldStore = "supervisorslevelfield";
                            item.FieldRequired = fs.FieldRequired;
                            if(fs.FieldRequired ==0){
                                item.BoolFieldRequired = false;

                            }
                            else{
                                item.BoolFieldRequired = true;
                            }
                            item.MaskTxt = "supervisor";
                            item.StudentSupervisorFieldType = "multiselect";
                            int? SupervisorAutoSearchFilter = Settings.Instance.GetMasterInfo3().AssignSup2StudVisible;
                            int? AssignSup2Stud = Settings.Instance.GetMasterInfo3().AssignSup2Stud;
                            if (SupervisorAutoSearchFilter!=null)
                            {
                                if (SupervisorAutoSearchFilter ==2)
                                  {
                                      item.StudentSupervisorFieldType = "tagfield";
                                  }
                                else if (SupervisorAutoSearchFilter == 3)
                                {
                                    item.StudentSupervisorFieldType = "combobox";
                                }
                                else 
                                {
                                    item.StudentSupervisorFieldType = "multiselect";
                                }
                                
                               }
                            
                        }

                    }

                }

                return list;
            }

        }


        public List<UserRegFieldSpecs> AllInstructorUserFields
        {
            get
            {
                var list = new List<UserRegFieldSpecs>();


                string fieldgrp = "affiliaton";
                list.Add(new UserRegFieldSpecs() { FieldName = "district", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "school", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "grade", TblFieldName = "GRADELEVEL", FieldGrp = fieldgrp });

                fieldgrp = "specialfield";
                list.Add(new UserRegFieldSpecs() { FieldName = "profileimage", FieldGrp = fieldgrp });

                fieldgrp = "presetfield";
                list.Add(new UserRegFieldSpecs() { FieldName = "username", FieldLabel = "User Name", BoolFieldRequired = true, FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "first", FieldLabel = "First Name", BoolFieldRequired = true, FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "last", FieldLabel = "Last Name", BoolFieldRequired = true, FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "password", FieldLabel = "Password", BoolFieldRequired = true, FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "instructornum"
                    , FieldLabel = TerminologyHelper.Instance.GetTermCapital(TermsEnum.Instructor) + " Number", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "email", FieldLabel = "E-Mail", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "address", FieldLabel = "Address", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "city", FieldLabel = "City", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "state", FieldLabel = "State", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "zip", FieldLabel = "Zip", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "homephone", FieldLabel = "Home Phone", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "workphone", FieldLabel = "Work Phone", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "fax", FieldLabel = "Fax", FieldGrp = fieldgrp });

                list.Add(new UserRegFieldSpecs() { FieldName = "years", FieldLabel = "Years Teaching Experience", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "disabled", FieldLabel="Temporarily Disable", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "bio", FieldLabel = "Biography", FieldGrp = fieldgrp });
                list.Add(new UserRegFieldSpecs() { FieldName = "AdminNotes", FieldLabel = "Admin Notes", FieldGrp = fieldgrp });
                
                //NOT ACTIVATED FIELDS
                //list.Add(new UserRegFieldSpecs() { FieldName = "CLASSIFIED", FieldGrp = fieldgrp });
                //list.Add(new UserRegFieldSpecs() { FieldName = "CERTIFIED", FieldGrp = fieldgrp });
                //list.Add(new UserRegFieldSpecs() { FieldName = "OTHER", FieldGrp = fieldgrp });
                //list.Add(new UserRegFieldSpecs() { FieldName = "CONTENT", FieldGrp = fieldgrp });
                //list.Add(new UserRegFieldSpecs() { FieldName = "EXPERIENCE", FieldGrp = fieldgrp });

                fieldgrp = "customfield";
                for (int i = 1; i <= 10; i++)
                {
                    list.Add(new UserRegFieldSpecs() { FieldName = "InstructorRegField" + i, FieldGrp = fieldgrp });
                }


                foreach (var item in list)
                {
                    string MIfldRequired = "NA";
                    FieldSpec fs = new FieldSpec();
                    FieldMask fm = new FieldMask();

                    item.MaskTxt = item.FieldName;
                    item.FieldVisible = true;
                    item.FieldForceSelection = false;

                    if (item.FieldGrp == "presetfield")
                    {

                        try {
                            fs = FieldSpecs.Where(f => (f.FieldName.ToLower() == item.FieldLabel.ToLower() || f.FieldName.ToLower() == item.FieldName.ToLower()) && f.TableName == "Instructors").First();
                            item.BoolFieldRequired = ((fs.FieldRequired == -1 || fs.FieldRequired == 1) ? true : false);
                            item.FieldReadOnly = ((fs.FieldReadOnly == -1 || fs.FieldReadOnly == 1) ? true : false);
                        }
                        catch { }
                        MIfldRequired = GetFieldInfo("ReqStudent" + item.FieldName.ToUpper());

                        if (item.FieldName == "state")
                        { item.MaskTxt = "2LetterStateAbbrev"; item.FieldStore = "storeState"; }

                        if (item.FieldName == "homephone")
                        { item.MaskTxt = "(###) ###-####"; }

                        if (item.FieldName == "workphone")
                        { item.MaskTxt = "(###) ###-####"; }

                        if (item.FieldName == "address")
                        { item.FieldLabel = Settings.Instance.GetMasterInfo2().PublicAddressLabel; }

                        if (item.FieldName == "country")
                        { item.FieldStore = "storeCountry"; }

                        if (item.FieldName == "password")
                        { item.MaskTxt = "password"; }

                        if (item.FieldName == "username")
                        { item.MaskTxt = "usernameIT"; }

                        if (item.FieldName == "disabled")
                        { item.MaskTxt = "singlecheckbox"; }

                        if (item.FieldName == "bio")
                        { item.MaskTxt = "html"; }

                        if (item.FieldName == "AdminNotes")
                        { item.MaskTxt = "html"; }


                    }
                    else if (item.FieldGrp == "customfield")
                    {

                        try { fs = FieldSpecs.Where(f => f.FieldName.ToLower() == item.FieldName.ToLower() && f.TableName == "Instructors").First(); }
                        catch { }

                        item.FieldRequired = fs.FieldRequired;
                        item.FieldReadOnly = ((fs.FieldReadOnly==-1 ||fs.FieldReadOnly==1) ? true : false);
                        item.FieldLabel = fs.FieldLabel;
                        item.FieldVisible = (string.IsNullOrEmpty(item.FieldLabel) ? false : true);

                        MIfldRequired = item.FieldRequired.ToString();

                        if (fs.YesNo == -1 || fs.YesNo == 1)
                        { item.MaskTxt = "Yes/No"; item.FieldStore = "storeYesNo"; }


                    }
                    else if (item.FieldGrp == "affiliaton")
                    {
                        MIfldRequired = "1";
                        if (item.FieldName == "district")
                        {
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field3Name;
                            item.FieldForceSelection = true;
                            item.FieldStore = "storedistrictfield";
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubInst == 1)
                            {
                                item.MaskTxt = "districtfiltered";
                            }
                        }
                        else if (item.FieldName == "school")
                        {
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field2Name;
                            item.FieldForceSelection = true;
                            item.FieldStore = "storeschoolfield";
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubInst == 1)
                            {
                                item.MaskTxt = "schoolfiltered";
                            }
                        }
                        else if (item.FieldName == "grade")
                        {
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field1Name;
                            item.FieldForceSelection = true;
                            item.FieldStore = "gradelevelfield";
                            //added count to condition, so that all grade levels are displayed if there are no school to grade level relations setup.
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubInst == 1 && GradeLevelsForFilter.Count > 0)
                            {
                                item.FieldStore = "gradelevelfieldForFilter";
                            }
                        }
                    }

                    item.BoolFieldRequired = (MIfldRequired == "-1" || MIfldRequired == "1" ? true : false);
                    item.AllowBlankBool = (MIfldRequired == "-1" || MIfldRequired == "1" ? false : true);
                }

                return list;
            }

        }



        public List<UserRegFieldSpecs> AllSupervisorUserFields
        {
            get
            {
                var list = new List<UserRegFieldSpecs>();

                /*
                list.Add(new UserRegFieldSpecs() { FieldName = "first", FieldLabel = "First Name",      BoolFieldRequired = true });
                list.Add(new UserRegFieldSpecs() { FieldName = "last", FieldLabel = "Last Name",        BoolFieldRequired = true });
                list.Add(new UserRegFieldSpecs() { FieldName = "username", FieldLabel = "UserName", MaskTxt = "usernameSP", BoolFieldRequired = true });
                list.Add(new UserRegFieldSpecs() { FieldName = "password", FieldLabel = "Password for Log In",  BoolFieldRequired = true });
                list.Add(new UserRegFieldSpecs() { FieldName = "title", FieldLabel = "Title"            });
                list.Add(new UserRegFieldSpecs() { FieldName = "address", FieldLabel = "Street Address" });
                list.Add(new UserRegFieldSpecs() { FieldName = "city", FieldLabel = "City"              });
                list.Add(new UserRegFieldSpecs() { FieldName = "state", FieldLabel = "State"            });
                list.Add(new UserRegFieldSpecs() { FieldName = "zip", FieldLabel = "Zip"                });
                list.Add(new UserRegFieldSpecs() { FieldName = "phone", FieldLabel = "Phone"        });
                list.Add(new UserRegFieldSpecs() { FieldName = "fax", FieldLabel = "Fax"                });
                list.Add(new UserRegFieldSpecs() { FieldName = "supervisornum", FieldLabel = "Supervisor Number"});
                list.Add(new UserRegFieldSpecs() { FieldName = "email", FieldLabel = "E-Mail Address", BoolFieldRequired = true, MaskTxt = "emailSP" });
                list.Add(new UserRegFieldSpecs() { FieldName = "additionalemailaddresses", FieldLabel = "Additional E-Mail Addresses", MaskTxt = "textareafield" });
                list.Add(new UserRegFieldSpecs() { FieldName = "notify", FieldLabel = "E-mail me when my students", MaskTxt="notifySP" });
                list.Add(new UserRegFieldSpecs() { FieldName = "advanceoptionsstr", FieldLabel = "Students enrollment/edit enabled", MaskTxt = "advanceoptions", FieldReadOnly = true });
                list.Add(new UserRegFieldSpecs() { FieldName = "district", FieldLabel = "hidden district", MaskTxt = "hidden", DefaultValue = "0" });
                list.Add(new UserRegFieldSpecs() { FieldName = "school", FieldLabel = "hidden school", MaskTxt = "hidden", DefaultValue = "0" });
                */
                list.Add(new UserRegFieldSpecs() { FieldName = "district", FieldLabel = "hidden district", DefaultValue = "0" });
                list.Add(new UserRegFieldSpecs() { FieldName = "school", FieldLabel = "hidden school", MaskTxt = "hidden", DefaultValue = "0" });
                list.Add(new UserRegFieldSpecs() { FieldName = "notify", FieldLabel = "E-mail me when my students", MaskTxt = "notifySP" });
                list.Add(new UserRegFieldSpecs() { FieldName = "advanceoptionsstr", FieldLabel = "Students enrollment/edit enabled", MaskTxt = "advanceoptions", FieldReadOnly = true, DefaultValue = "3" });

                try
                {
                    var fs = FieldSpecs.Where(f => f.TableName == "Parents").ToList();
                    int gotDistrictDropdown = 0;
                    foreach (var item in fs)
                    {
                        FieldMask fm = new FieldMask();
                        string usernamemask = "";
                        try { fm = FieldMasks.Where(f => f.FieldName == item.FieldName && f.TableName == "Parents").FirstOrDefault(); }
                        catch { }
                        if (fm != null)
                        {
                            if (item.FieldName.ToLower() == "username" && fm.DefaultMaskNumber == 97)
                            { usernamemask = "emailusername"; item.FieldVisible = 0; }
                        }
                        if (item.FieldName.ToLower() == "password")
                        {
                            usernamemask = "password";
                        }
                        if (item.FieldName.ToLower() == "parentleveloneid")
                        {
                            if (item.FieldVisible == -1)
                            {
                                gotDistrictDropdown = 1;
                            }
                        }
                        list.Add(new UserRegFieldSpecs() { FieldName = item.FieldName, FieldLabel = item.FieldLabel, BoolFieldRequired = (item.FieldRequired == 1 || item.FieldRequired == -1) ? true : false, FieldVisible = (item.FieldVisible == 1 || item.FieldVisible == -1) ? true : false, MaskTxt = usernamemask });
                    }
                    foreach (var item in list)
                    {
                        if (item.FieldName == "district" && Settings.Instance.GetMasterInfo().Field3Name != "" && Settings.Instance.GetMasterInfo().Field3Name != "n/a" && gotDistrictDropdown !=0)
                        {
                            item.MaskTxt = item.FieldName;
                            item.FieldLabel = Settings.Instance.GetMasterInfo().Field3Name;
                            item.FieldForceSelection = true;
                            item.FieldStore = "storedistrictfield";
                            if (Settings.Instance.GetMasterInfo3().FilterDistSchoolPubInst == 1)
                            {
                                item.MaskTxt = "districtfiltered";
                            }
                            item.BoolFieldRequired = true;
                        }
                        else if (item.FieldName == "district" && gotDistrictDropdown ==0)
                        {
                            var removeDistrict = list.SingleOrDefault(r => r.FieldName == "district");
                            if (removeDistrict != null)
                                list.Remove(removeDistrict);
                        }
                        if (item.FieldName.ToLower() == "parentleveloneid")
                        {
                            item.FieldVisible = false;
                        }
                    }
                    //list.Add(new UserRegFieldSpecs() { FieldName = "district", FieldLabel = "hidden district", MaskTxt = "hidden", DefaultValue = "0" });
                    //list.Add(new UserRegFieldSpecs() { FieldName = "school", FieldLabel = "hidden school", MaskTxt = "hidden", DefaultValue = "0" });
                    //list.Add(new UserRegFieldSpecs() { FieldName = "notify", FieldLabel = "E-mail me when my students", MaskTxt = "notifySP" });
                    //list.Add(new UserRegFieldSpecs() { FieldName = "advanceoptionsstr", FieldLabel = "Students enrollment/edit enabled", MaskTxt = "advanceoptions", FieldReadOnly = true });
                }
                catch
                {
                    list.Add(new UserRegFieldSpecs() { FieldName = "first", FieldLabel = "First Name", BoolFieldRequired = true });
                    list.Add(new UserRegFieldSpecs() { FieldName = "last", FieldLabel = "Last Name", BoolFieldRequired = true });
                    list.Add(new UserRegFieldSpecs() { FieldName = "username", FieldLabel = "UserName", MaskTxt = "usernameSP", BoolFieldRequired = true });
                    list.Add(new UserRegFieldSpecs() { FieldName = "password", FieldLabel = "Password for Log In", BoolFieldRequired = true });
                    list.Add(new UserRegFieldSpecs() { FieldName = "title", FieldLabel = "Title" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "address", FieldLabel = "Street Address" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "city", FieldLabel = "City" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "state", FieldLabel = "State" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "zip", FieldLabel = "Zip" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "phone", FieldLabel = "Phone" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "fax", FieldLabel = "Fax" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "supervisornum", FieldLabel = "Supervisor Number" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "email", FieldLabel = "E-Mail Address", BoolFieldRequired = true, MaskTxt = "emailSP" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "additionalemailaddresses", FieldLabel = "Additional E-Mail Addresses", MaskTxt = "textareafield" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "notify", FieldLabel = "E-mail me when my students", MaskTxt = "notifySP" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "advanceoptionsstr", FieldLabel = "Students enrollment/edit enabled", MaskTxt = "advanceoptions", FieldReadOnly = true });
                    list.Add(new UserRegFieldSpecs() { FieldName = "district", FieldLabel = "hidden district", MaskTxt = "hidden", DefaultValue = "0" });
                    list.Add(new UserRegFieldSpecs() { FieldName = "school", FieldLabel = "hidden school", MaskTxt = "hidden", DefaultValue = "0" });
                }
                return list;
            }

        }

        public List<UserRegFields> DemographicFields
        {
            get
            {
                var list = new List<UserRegFields>();
                using (var db = new SchoolEntities())
                {
                    list = (from fs in db.FieldSpecs where fs.TableName.Equals("Students")
                            
                            select new UserRegFields()
                            {
                                FieldLabel = fs.FieldLabel==null?"":fs.FieldLabel ,
                                FieldName = fs.FieldName == null ? "" : fs.FieldName,
                                FieldSpecsId = fs.FieldSpecsId,
                                FieldRequired = fs.FieldRequired,
                                FieldCustomList = fs.FieldListValue == null ? "" : fs.FieldListValue,
                                FieldListType = fs.FieldListType,
                                GroupWidget = fs.GroupWidget == null ? "" : fs.GroupWidget,

                            }
                            ).ToList();


                    //remove not included in demographic fields
                    list.RemoveAll(sd => sd.FieldName.Equals("first"));
                    list.RemoveAll(sd => sd.FieldName.Equals("last"));
                    list.RemoveAll(sd => sd.FieldName.Equals("email"));
                    list.RemoveAll(sd => sd.FieldName.Equals("studnum"));
                    list.RemoveAll(sd => sd.FieldName.Equals("username"));

                   




                    foreach (var item in list)
                    {
                        string MIfldLabel = "NA";
                        string MIfldRequired = "NA";
                        string MIfldVisible = "NA";
                        int DefaultMaskNumber = 0;


                        //CUSTOMIZABLE Student Registration Fields
                        if (item.FieldName.Contains("studregfield"))
                        {
                            MIfldLabel = GetFieldInfo(item.FieldName + "Name");
                            MIfldRequired = GetFieldInfo(item.FieldName + "Required");
                            item.FieldGrp = "customfield";
                            DefaultMaskNumber = GetMaskNum(item.FieldName);
                            item.MaskNum = DefaultMaskNumber;
                            item.DefaultMaskNumber = DefaultMaskNumber;

                            if (DefaultMaskNumber == 0)
                            { item.MaskTxt = "No Mask"; }

                            else if (DefaultMaskNumber == 1)
                            { item.MaskTxt = "YYYY/MM/DD"; }

                            else if (DefaultMaskNumber == 2)
                            { item.MaskTxt = "MM/DD/YYYY"; }

                            else if (DefaultMaskNumber == 20)
                            { item.MaskTxt = "Gender"; }

                            else if (DefaultMaskNumber == 21)
                            { item.MaskTxt = "Ethnicity 1"; }

                            else if (DefaultMaskNumber == 22)
                            { item.MaskTxt = "Department"; }

                            else if (DefaultMaskNumber == 23)
                            { item.MaskTxt = "Yes/No"; }

                            else if (DefaultMaskNumber == 24)
                            { item.MaskTxt = "Ethnicity 2"; }

                            else if (DefaultMaskNumber == 25)
                            { item.MaskTxt = "Race"; }

                            else if (DefaultMaskNumber == 26 && item.FieldListType == 0)
                            { item.MaskTxt = "SelectionCheckbox"; }

                            else if (DefaultMaskNumber == 26 && item.FieldListType == 1)
                            { item.MaskTxt = "SelectionListSingleSelect"; }

                            else if (DefaultMaskNumber == 26 && item.FieldListType == 2)
                            { item.MaskTxt = "SelectionListMultiSelect"; }

                            else
                            { item.MaskTxt = "Unidentified Mask"; }


                        //PRESET Student Registration Fields
                        }else{
                            MIfldLabel = item.FieldLabel;
                            MIfldRequired = GetFieldInfo("ReqStudent" + item.FieldName.ToUpper());
                            MIfldVisible = GetFieldInfo("VisibleStudent" + item.FieldName.ToUpper());
                            MIfldLabel = (MIfldVisible == "0" ? "" : MIfldLabel);
                            item.FieldGrp = "presetfield";
                            DefaultMaskNumber = GetMaskNum(item.FieldName);
                            item.MaskNum = DefaultMaskNumber;
                            item.DefaultMaskNumber = DefaultMaskNumber;

                            item.MaskTxt = item.FieldName;

                            if (item.FieldName == "state" && item.MaskNum == 1)
                            { item.MaskTxt = "2LetterStateAbbrev"; }

                            if (item.FieldName == "homephone" && item.MaskNum == 1)
                            { item.MaskTxt = "(###) ###-####"; }

                            if (item.FieldName == "workphone" && item.MaskNum == 1)
                            { item.MaskTxt = "(###) ###-####"; }

                            if (item.FieldName == "ss" && item.MaskNum == 1)
                            { item.MaskTxt = "###-##-####"; }

                            if (item.FieldName == "ss" && item.MaskNum == 2)
                            { item.MaskTxt = "####"; }


                        }



                        item.FieldLabel = (item.FieldName == "address" ? Settings.Instance.GetMasterInfo2().PublicAddressLabel : MIfldLabel);
                        try
                        {
                            item.FieldRequired = (MIfldRequired == "NA" ? item.FieldRequired : int.Parse(MIfldRequired));
                        }
                        catch
                        {
                            item.FieldRequired = 0;
                        }
                        item.BoolFieldRequired = (MIfldRequired == "-1" || MIfldRequired == "1" ? true : false);
                        item.AllowBlankBool = (MIfldRequired == "-1" || MIfldRequired == "1" ? false : true);
                        
                    }

                    //remove other not to show / visible = 0 
                    //list.RemoveAll(sd => string.IsNullOrEmpty(sd.FieldLabel));

                }
                return list;
            }
        }


        public object GetField(object obj, string fieldName)
        {
            var t = obj.GetType();
            var field = t.GetField(fieldName);
            return field.GetValue(obj);
        }

         private string GetFieldInfo(string fldname)
        {
            try
            {
                var mainConnection = Connections.GetSchoolConnection();
                mainConnection.Open();

                string dtring = "";
                string query = "SELECT Top 1 " + fldname + " FROM MasterInfo Order by SubSiteId desc";

                var mainCommand = mainConnection.CreateCommand();
                mainCommand.CommandText = query;

                using (var Reader = mainCommand.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        dtring = Reader.GetValue(0).ToString();
                    }
                }
                mainConnection.Close();
                return dtring;
            }
            catch
            {
                return "NA";
            }
        }

         private int GetMaskNum(string fldname)
         {
             try
             {
                 using (var db = new SchoolEntities())
                 {
                     return (from fm in db.FieldMasks
                             where (fm.TableName.Equals("Students") && fm.FieldName.Equals(fldname))
                             select fm.DefaultMaskNumber).First();
                 }
             }
             catch
             {
                 return 0;
             }
         }


         public string GetFieldValue(int userid, string fieldname = null, string tablename = "Students")
         {
             string query = "SELECT Top 1 [" + fieldname + "] FROM [" + tablename + "] WHERE STUDENTID=" + userid;
             if (tablename == "Instructors")
             {
                 query = "SELECT Top 1 [" + fieldname + "] FROM [" + tablename + "] WHERE INSTRUCTORID=" + userid;
             }

             try
             {
                 var mainConnection = Connections.GetSchoolConnection();
                 mainConnection.Open();

                 string dtring = "";

                 var mainCommand = mainConnection.CreateCommand();
                 mainCommand.CommandText = query;

                 using (var Reader = mainCommand.ExecuteReader())
                 {
                     while (Reader.Read())
                     {
                         dtring = Reader.GetValue(0).ToString();
                     }
                 }
                 mainConnection.Close();
                 return dtring;
             }
             catch
             {
                 return "Invalid SQL: " + query;
             }
         }

        

    }
}
