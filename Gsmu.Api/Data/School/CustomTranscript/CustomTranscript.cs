using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gsmu.Api.Data.School.CustomTranscriptModel
{
    public class CustomTranscriptCourseFields
    {
        public string id
        {
            get;
            set;
        }
        public int sort
        {
            get;
            set;
        }

        public string textDisplay
        {
            get;
            set;
        }
    }
    public class CustomTranscriptModel
    {
        public CustomTranscriptModel(int transcriptid =0)
        {
            using (var db = new SchoolEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;
                var transcript= new customtranscript();
                CustomTranscriptList = (from c in db.customtranscripts.AsNoTracking() select c).ToList();

                if (transcriptid != 0)
                {
                    transcript = (from c in db.customtranscripts.AsNoTracking() where c.customtranid == transcriptid select c).FirstOrDefault();
                }

                else
                {
                    transcript = (from c in db.customtranscripts.AsNoTracking()  select c).First();

                }
                if (transcript != null)
                {
                    Init(db, transcript);
                }
            }
        }
        public void SaveChanges(customtranscript updatedTranscript)
        {
            var Context = new SchoolEntities();
            customtranscript customTranscript = Context.customtranscripts.First(cr => cr.customtranid == updatedTranscript.customtranid);
            customTranscript.transtitle = updatedTranscript.transtitle;
            customTranscript.transwidth = updatedTranscript.transwidth;
            customTranscript.transheight = updatedTranscript.transheight;
            customTranscript.boundY = updatedTranscript.boundY - 40;
            customTranscript.boundX = updatedTranscript.boundX-40;
            customTranscript.isdefaultselected = updatedTranscript.isdefaultselected;

            
            customTranscript.useHeader1 = updatedTranscript.useHeader1;
            customTranscript.fontsizeHeader1 = updatedTranscript.fontsizeHeader1;
            customTranscript.locheader1 = updatedTranscript.locheader1;
            customTranscript.OptionalHeaderText = updatedTranscript.OptionalHeaderText;

            customTranscript.useInfo1 = updatedTranscript.useInfo1;
            customTranscript.fontsizeInfo1 = updatedTranscript.fontsizeInfo1;
            customTranscript.locinfo1 = updatedTranscript.locinfo1;
            customTranscript.textInfo1 = updatedTranscript.textInfo1;

            customTranscript.fontsizename1 = updatedTranscript.fontsizename1;
            customTranscript.locname1 = updatedTranscript.locname1;
            customTranscript.alignsname1 = updatedTranscript.alignsname1;


            customTranscript.useAddress1 = updatedTranscript.useAddress1;
            customTranscript.locaddress1 = updatedTranscript.locaddress1;
            customTranscript.fontsizeAddress1 = updatedTranscript.fontsizeAddress1;


            customTranscript.useStudField1 = updatedTranscript.useStudField1;
            customTranscript.fontsizeStudfield1 = updatedTranscript.fontsizeStudfield1;
            customTranscript.alignstudfield1 = updatedTranscript.alignstudfield1;
            customTranscript.locStudField1 = updatedTranscript.locStudField1;
            customTranscript.selectedstudfield1 = updatedTranscript.selectedstudfield1;

            customTranscript.useStudField2 = updatedTranscript.useStudField2;
            customTranscript.fontsizeStudfield2 = updatedTranscript.fontsizeStudfield2;
            customTranscript.alignstudfield2 = updatedTranscript.alignstudfield2;
            customTranscript.locStudField2 = updatedTranscript.locStudField2;
            customTranscript.selectedstudfield2 = updatedTranscript.selectedstudfield2;

            customTranscript.useStudField3 = updatedTranscript.useStudField3;
            customTranscript.fontsizeStudfield3 = updatedTranscript.fontsizeStudfield3;
            customTranscript.alignstudfield3 = updatedTranscript.alignstudfield3;
            customTranscript.locStudField3 = updatedTranscript.locStudField3;
            customTranscript.selectedstudfield3 = updatedTranscript.selectedstudfield3;

            customTranscript.useDatePrint = updatedTranscript.useDatePrint;
            customTranscript.fontsizeDateprint = updatedTranscript.fontsizeDateprint;
            customTranscript.locDatePrint = updatedTranscript.locDatePrint;
            customTranscript.aligndateprinted = updatedTranscript.aligndateprinted;
            customTranscript.coursefieldjsonsettings = updatedTranscript.selectedcoursefieldsettings;

            customTranscript.locCoursename = updatedTranscript.locCoursename;
            customTranscript.loccourseloc = updatedTranscript.locCoursename;

            customTranscript.coursegridconfiguration = updatedTranscript.coursegridconfiguration;
            Context.SaveChanges();
        }

        public void SaveBackgroundImage(customtranscript updatedTranscript)
        {
            var Context = new SchoolEntities();
            customtranscript customTranscript = Context.customtranscripts.First(cr => cr.customtranid == updatedTranscript.customtranid);
            customTranscript.backgroundimage = updatedTranscript.backgroundimage;
            Context.SaveChanges();
        }
        private void Init(SchoolEntities db, Entities.customtranscript c)
        {
            transcriptid = c.customtranid;
            BackgroundImage = c.backgroundimage;
            PdfTitle = c.transtitle;
            PdfHeight = c.transheight.Value.ToString();
            PdfWidth = c.transwidth.Value.ToString();
            PdfMarginBottom = c.marginBottom.Value.ToString();
            PdfMarginLeft = c.marginLeft.Value.ToString();
            PdfMarginRight = c.marginRight.Value.ToString() ;
            PdfMarginTop = c.marginTop.Value.ToString();
            BoundX = c.boundX.Value.ToString();
            BoundY = c.boundY.Value.ToString();
            useHeader =SetVisibility( c.useHeader1.Value);
            HeaderFont = c.fontsizeHeader1.Value.ToString() +"px";
            OptionalHeaderText = c.OptionalHeaderText;
            HeaderAlign = SetAlign(0);
            HeaderX = (c.locheader1.Split(',')[0]) +"px";
            HeaderY = (c.locheader1.Split(',')[1]) + "px";
            HeaderW = (c.locheader1.Split(',')[2]) + "px";
            HeaderH = (c.locheader1.Split(',')[3]) + "px";


            useOptionalText = SetVisibility(c.useInfo1.Value);
            OptionalTextValue = c.textInfo1;
            OptionalTextFont  = c.fontsizeInfo1.Value.ToString() +"px";
            OptionalTextAlign = SetAlign(0);
            OptionalTextX = (c.locinfo1.Split(',')[0]) + "px";
            OptionalTextY = (c.locinfo1.Split(',')[1]) + "px";
            OptionalTextW = (c.locinfo1.Split(',')[2]) + "px";
            OptionalTextH = (c.locinfo1.Split(',')[3]) + "px";


            StudentNameAlign = SetAlign(c.alignsname1.Value);
            StudentNameFont = c.fontsizename1.Value.ToString() +"px";
            StudentNameX = (c.locname1.Split(',')[0]) + "px";
            StudentNameY = (c.locname1.Split(',')[1]) + "px";
            StudentNameW = (c.locname1.Split(',')[2]) + "px";
            StudentNameH = (c.locname1.Split(',')[3]) + "px";


            useStudentAddress = SetVisibility(c.useAddress1.Value);
            StudentAddressAlign = SetAlign(c.alignsname1.Value);
            StudentAddressFont = c.fontsizeAddress1.Value.ToString() + "px";
            StudentAddressX = (c.locaddress1.Split(',')[0]) + "px";
            StudentAddressY = (c.locaddress1.Split(',')[1]) + "px";
            StudentAddressW = (c.locaddress1.Split(',')[2]) + "px";
            StudentAddressH = (c.locaddress1.Split(',')[3]) + "px";


            useCustomField1 = SetVisibility(c.useStudField1.Value);
            useCustomField2 = SetVisibility(c.useStudField2.Value);
            useCustomField3 = SetVisibility(c.useStudField3.Value);
            CustomField1Align = SetAlign(c.alignstudfield1.Value);
            CustomField2Align = SetAlign(c.alignstudfield2.Value);
            CustomField3Align = SetAlign(c.alignstudfield3.Value);
            CustomField1Font = c.fontsizeStudfield1.Value.ToString() + "px";
            CustomField2Font = c.fontsizeStudfield2.Value.ToString() + "px";
            CustomField3Font = c.fontsizeStudfield3.Value.ToString() + "px";
            CustomField1X = (c.locStudField1.Split(',')[0]) + "px";
            CustomField1Y = (c.locStudField1.Split(',')[1]) + "px";
            CustomField1W = (c.locStudField1.Split(',')[2]) + "px";
            CustomField1H = (c.locStudField1.Split(',')[3]) + "px";
            CustomField2X = (c.locStudField2.Split(',')[0]) + "px";
            CustomField2Y = (c.locStudField2.Split(',')[1]) + "px";
            CustomField2W = (c.locStudField2.Split(',')[2]) + "px";
            CustomField2H = (c.locStudField2.Split(',')[3]) + "px";
            CustomField3X = (c.locStudField3.Split(',')[0]) + "px";
            CustomField3Y = (c.locStudField3.Split(',')[1]) + "px";
            CustomField3W = (c.locStudField3.Split(',')[2]) + "px";
            CustomField3H = (c.locStudField3.Split(',')[3]) + "px";
            CustomField1Selected = "Custom Field 1";
            CustomField2Selected = "Custom Field 2";
            CustomField3Selected = "Custom Field 3";
            if (c.selectedstudfield1 != "")
            {
                CustomField1Selected = c.selectedstudfield1;
            }
            if (c.selectedstudfield2 != "")
            {
                CustomField2Selected = c.selectedstudfield2;
            }
            if (c.selectedstudfield3 != "")
            {
                CustomField3Selected = c.selectedstudfield3;
            }

            //Date Print

            useDatePrint = SetVisibility(c.useDatePrint.Value);
            try
            {
                DatePrinttAlign = SetAlign(c.aligndateprinted.Value);
            }
            catch
            {
                DatePrinttAlign = SetAlign(0);
            }
            DatePrintFont = c.fontsizeDateprint.Value.ToString() + "px";
            DatePrintX = (c.locDatePrint.Split(',')[0]) + "px";
            DatePrintY = (c.locDatePrint.Split(',')[1]) + "px";
            DatePrintW = (c.locDatePrint.Split(',')[2]) + "px";
            DatePrintH = (c.locDatePrint.Split(',')[3]) + "px";



            CourseGridX = (c.locCoursename.Split(',')[0]) + "px";
            CourseGridY = (c.locCoursename.Split(',')[1]) + "px";
            CourseGridW = (c.locCoursename.Split(',')[2]) + "px";
            CourseGridH = (c.locCoursename.Split(',')[3]) + "px";

            if (c.coursegridconfiguration == "" || c.coursegridconfiguration == null)
            {
                CourseGridDefaultFieldSorting = "startdate";
            }
            else
            {
                CourseGridDefaultFieldSorting = GetGridDefaultFieldSorting(c.coursegridconfiguration);
            }
            ProcessSelectedFields(c.coursefieldjsonsettings);
            DefaultTranscript = c.isdefaultselected.Value;
            SelectedUploadeLogo = 0;

        }
        public string GetGridDefaultFieldSorting(string jsonfields)
        {
            try
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic settingsconfig = j.Deserialize(jsonfields, typeof(object));

                string sort = settingsconfig["sort"];
                return sort;
            }
            catch
            {
                return "startdate";
            }
        }
        public void ProcessSelectedFields(string jsonfields)
        {
            if (jsonfields.Trim() == "")
            {
                jsonfields = DefaultSelectedField();
            }
            CustomTranscriptCourseFields fields = new CustomTranscriptCourseFields();
            ListSelectedFields = new List<CustomTranscriptCourseFields>();
            ListAvailableFields = new List<CustomTranscriptCourseFields>();
            JavaScriptSerializer j = new JavaScriptSerializer();
            dynamic settingsconfig = j.Deserialize(jsonfields, typeof(object));
            foreach (var field in settingsconfig["selectedfield"])
            {
                fields = new CustomTranscriptCourseFields();
                fields.id = field["id"];
                fields.sort = field["sort"];
                fields.textDisplay = SetCourseHeaderDisplay(field["id"]);
                if (fields.textDisplay != "")
                {
                    ListSelectedFields.Add(fields);
                }
            }

            foreach (var field in settingsconfig["availablefield"])
            {
                fields = new CustomTranscriptCourseFields();
                fields.id = field["id"];
                fields.sort = field["sort"];
                fields.textDisplay = SetCourseHeaderDisplay(field["id"]);
                if (fields.textDisplay != "")
                {
                    ListAvailableFields.Add(fields);
                }
            }

            if((from a in ListAvailableFields where a.id =="courseicon" select a).Count()<1 && (from a in ListSelectedFields where a.id =="courseicon" select a).Count()<1)
            {
                fields = new CustomTranscriptCourseFields();
                fields.id = "courseicon";
                fields.sort = 9;
                fields.textDisplay = SetCourseHeaderDisplay("courseicon");
                if (fields.textDisplay != "")
                {
                    ListAvailableFields.Add(fields);
                }
            }

        }

        public string SetCourseHeaderDisplay(string value)
        {
            if (value == "credithours")
            {
                return Settings.Instance.GetMasterInfo2().CreditHoursName;
            }
            if (value == "hours")
            {
                return "*" + Settings.Instance.GetMasterInfo2().CreditHoursName;
            }
            else if (value == "InService")
            {
                return Settings.Instance.GetMasterInfo2().InserviceHoursName;
            }
            else if (value == "customcredit")
            {
                return Settings.Instance.GetMasterInfo2().CustomCreditTypeName;
            }
            else if (value == "sbceu")
            {
                return Settings.Instance.GetMasterInfo2().CEUCreditLabel;
            }
            else if (value == "Graduate")
            {
                return "Graduate";
            }


            else if (value == "och1")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1;
            }
            else if (value == "och2")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2;
            }
            else if (value == "och3")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3;
            }
            else if (value == "och4")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4;
            }
            else if (value == "och5")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5;
            }
            else if (value == "och6")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6;
            }
            else if (value == "och7")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7;
            }
            else if (value == "och8")
            {
                return Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8;
            }
            else if (value == "startdate")
            {
                return "Start Date";
            }
            else if (value == "enddate")
            {
                return "End Date";
            }
            else if (value == "coursenum")
            {
                return "Course No.";
            }
            else if (value == "coursename")
            {
                return "Course Name";
            }
            else if (value == "customfield1")
            {
                return Settings.Instance.GetMasterInfo2().CustomCourseFieldLabel1;
            }
            else if (value == "customfield2")
            {
                return Settings.Instance.GetMasterInfo2().CustomCourseFieldLabel2;
            }
            else if (value == "customfield3")
            {
                return Settings.Instance.GetMasterInfo2().CustomCourseFieldLabel3;
            }
            else if (value == "customfield4")
            {
                return Settings.Instance.GetMasterInfo2().CustomCourseFieldLabel4;
            }
            else if (value == "customfield5")
            {
                return Settings.Instance.GetMasterInfo2().CustomCourseFieldLabel5;
            }
            else if (value == "courseicon")
            {
                return "Course Icon(s)";
            }
            else
            {
                return value;
            }

        }
        public string DefaultSelectedField()
        {
            return @"{"+
                "\"selectedfield\":[{\"id\":\"coursename\",\"sort\":0},{\"id\":\"coursenum\",\"sort\":1},"+
                                 "{\"id\":\"startdate\",\"sort\":2},{\"id\":\"enddate\"  ,\"sort\":3}"+
                                 "],"+
                "\"availablefield\":[{\"id\":\"hours\",\"sort\":4},{\"id\":\"credithours\",\"sort\":5},{\"id\":\"InService\",\"sort\":6}," +
                             "{\"id\":\"customcredit\",\"sort\":7},{\"id\":\"Graduate\"  ,\"sort\":8}," +
                             "{\"id\":\"sbceu\",\"sort\":9},{\"id\":\"och1\"  ,\"sort\":10}," +
                             "{\"id\":\"och2\",\"sort\":9},{\"id\":\"och3\"  ,\"sort\":10}," +
                             "{\"id\":\"och4\",\"sort\":9},{\"id\":\"och5\"  ,\"sort\":10}," +
                             "{\"id\":\"och6\",\"sort\":9},{\"id\":\"och7\"  ,\"sort\":10}," +
                             "{\"id\":\"och8\",\"sort\":9},{\"id\":\"customfield1\"  ,\"sort\":10}," +
                             "{\"id\":\"customfield2\",\"sort\":9},{\"id\":\"customfield3\"  ,\"sort\":10}," +
                             "{\"id\":\"customfield4\",\"sort\":9},{\"id\":\"customfield5\"  ,\"sort\":10}," +
                              "{\"id\":\"courseicon\",\"sort\":9}," +
                              "{\"id\":\"grade\",\"sort\":9}" +
                "]}";
        }


        public int NewTranscript()
        {
            var Context = new SchoolEntities();
            customtranscript customtrans = new customtranscript();
             //pdfProperties
            customtrans.transtitle = "New Certificate";
            customtrans.transwidth = 612;
            customtrans.transheight = 792;
            customtrans.boundX = 612-40;
            customtrans.boundY = 792-40;
            
            customtrans.marginBottom = 18;
            customtrans.marginTop = 18;
            customtrans.marginLeft = 18;
            customtrans.marginRight = 18;


            //header config
            customtrans.useHeader1 =0;
            customtrans.fontsizeHeader1 = 15;
            customtrans.locheader1 = "200,30,10,10";
           
            //Optional Text
            customtrans.useInfo1 =0;
            customtrans.fontsizeInfo1 = 12;
            customtrans.locinfo1 =  "200,30,10,10";;

            //Student Name
            customtrans.fontsizename1 = 12;
            customtrans.locname1 = "200,30,10,10";
            customtrans.alignsname1 =0;


            //Student Address
            customtrans.useAddress1 =0;
            customtrans.fontsizeAddress1 = 12;
            customtrans.locaddress1 =  "200,30,10,10";
         
            //Custom Field 1
            customtrans.useStudField1 = 0;
            customtrans.fontsizeStudfield1 =12;
            customtrans.locStudField1 = "200,30,10,10";
            customtrans.alignstudfield1 = 0;

            //Custom Field 2
            customtrans.useStudField2 = 0;
            customtrans.fontsizeStudfield2 =12;
            customtrans.locStudField2 = "200,30,10,10";
            customtrans.alignstudfield2 = 0;

            //Custom Field 3
            customtrans.useStudField3 = 0;
            customtrans.fontsizeStudfield3 =12;
            customtrans.locStudField3 = "200,30,10,10";
            customtrans.alignstudfield3 = 0;


            // dateprintconfig
            customtrans.useDatePrint = 0;
            customtrans.fontsizeDateprint = 12;
            customtrans.locDatePrint = "200,30,10,10";

            //Course Area
            customtrans.selectedcoursefieldsettings = "";
            customtrans.locCoursename = "200,30,10,10";
            customtrans.loccourseloc = "200,30,10,10";
            customtrans.coursefieldjsonsettings = "";
            customtrans.isdefaultselected = 0;
            customtrans.fontsizeCoursename = 12;
            customtrans.aligncloc = 0;
            customtrans.disabled = 0;
            customtrans.dateadded = DateTime.Now;
            customtrans.includeCourseNum = 0;
            customtrans.disabled = 0;
            customtrans.fontsizeCoursename = 12;
            customtrans.selectedstudfield1 = "";
            customtrans.selectedstudfield2 = "";
            customtrans.selectedstudfield3 = "";
            customtrans.alignInfo1 = 0;
            customtrans.textInfo1 = "";
            customtrans.backgroundimage = "";
            Context.customtranscripts.Add(customtrans);
            Context.SaveChanges();
            return customtrans.customtranid;
        }






        public List<CustomTranscriptCourseFields> ListSelectedFields
        {
            get;
            set;
        }

        public List<CustomTranscriptCourseFields> ListAvailableFields
        {
            get;
            set;
        }

        public List<customtranscript> CustomTranscriptList
        {
            get;
            set;
        }

        public string SetAlign(int value)
        {
            if (value == 0)
            {
                return "center";
            }
            else if (value == 1)
            {
                return "left";
            }
            else
            {
                return "right";
            }
        }
        public string SetVisibility(int value)
        {
            if (value == 1)
            {
                return "visible";
            }
            else
            {
                return "hidden";
            }
        }
        public int transcriptid
        {
            get;
            set;
        }
        public int DefaultTranscript
        {
            get;
            set;
        }
        public int SelectedUploadeLogo
        {
            get;
            set;
        }
        public string BackgroundImage
        {
            get;
            set;
        }
        public string PdfTitle
        {
            get;
            set;
        }
        public string PdfWidth
        {
            get;
            set;
        }
        public string BoundX
        {
            get;
            set;
        }
        public string BoundY
        {
            get;
            set;
        }
        public string PdfHeight
        {
            get;
            set;
        }
        public string PdfMarginTop
        {
            get;
            set;
        }
        public string PdfMarginLeft
        {
            get;
            set;
        }
        public string PdfMarginBottom
        {
            get;
            set;
        }
        public string PdfMarginRight
        {
            get;
            set;
        }
        public string useHeader
        {
            get;
            set;
        }

        public string HeaderAlign
        {
            get;
            set;
        }

        public string HeaderX
        {
            get;
            set;
        }


        public string HeaderY
        {
            get;
            set;
        }


        public string HeaderW
        {
            get;
            set;
        }

        public string HeaderH
        {
            get;
            set;
        }


        public string HeaderFont
        {
            get;
            set;
        }
        public string OptionalHeaderText
        {
            get;
            set;
        }

        public string useOptionalText
        {
            get;
            set;
        }

        public string OptionalTextFont
        {
            get;
            set;
        }
        public string OptionalTextValue
        {
            get;
            set;
        }

        public string OptionalTextAlign
        {
            get;
            set;
        }
        public string OptionalTextX
        {
            get;
            set;
        }
        public string OptionalTextY
        {
            get;
            set;
        }

        public string OptionalTextW
        {
            get;
            set;
        }
        public string OptionalTextH
        {
            get;
            set;
        }


        public string StudentNameFont
        {
            get;
            set;
        }

        public string StudentNameAlign
        {
            get;
            set;
        }

        public string StudentNameX
        {
            get;
            set;
        }

        public string StudentNameY
        {
            get;
            set;
        }

        public string StudentNameW
        {
            get;
            set;
        }

        public string StudentNameH
        {
            get;
            set;
        }


        public string useStudentAddress
        {
            get;
            set;
        }

        public string StudentAddressFont
        {
            get;
            set;
        }


        public string StudentAddressAlign
        {
            get;
            set;
        }

        public string StudentAddressX
        {
            get;
            set;
        }


        public string StudentAddressY
        {
            get;
            set;
        }

        public string StudentAddressW
        {
            get;
            set;
        }

        public string StudentAddressH
        {
            get;
            set;
        }

        public string useCustomField1
        {
            get;
            set;
        }

        public string CustomField1Font
        {
            get;
            set;
        }
        public string CustomField1Align
        {
            get;
            set;
        }


        public string CustomField1X
        {
            get;
            set;
        }

        public string CustomField1Y
        {
            get;
            set;
        }

        public string CustomField1W
        {
            get;
            set;
        }

        public string CustomField1H
        {
            get;
            set;
        }

        public string CustomField1Selected
        {
            get;
            set;
        }





        public string useCustomField2
        {
            get;
            set;
        }

        public string CustomField2Font
        {
            get;
            set;
        }
        public string CustomField2Align
        {
            get;
            set;
        }


        public string CustomField2X
        {
            get;
            set;
        }

        public string CustomField2Y
        {
            get;
            set;
        }

        public string CustomField2W
        {
            get;
            set;
        }

        public string CustomField2H
        {
            get;
            set;
        }

        public string CustomField2Selected
        {
            get;
            set;
        }
        public string useCustomField3
        {
            get;
            set;
        }

        public string CustomField3Font
        {
            get;
            set;
        }
        public string CustomField3Align
        {
            get;
            set;
        }


        public string CustomField3X
        {
            get;
            set;
        }

        public string CustomField3Y
        {
            get;
            set;
        }

        public string CustomField3W
        {
            get;
            set;
        }

        public string CustomField3H
        {
            get;
            set;
        }

        public string CustomField3Selected
        {
            get;
            set;
        }

        public string useDatePrint
        {
            get;
            set;
        }

        public string DatePrintFont
        {
            get;
            set;
        }

        public string DatePrinttAlign
        {
            get;
            set;
        }
        public string DatePrintX
        {
            get;
            set;
        }
        public string DatePrintY
        {
            get;
            set;
        }

        public string DatePrintW
        {
            get;
            set;
        }
        public string DatePrintH
        {
            get;
            set;
        }


        public string CourseGridX
        {
            get;
            set;
        }
        public string CourseGridY
        {
            get;
            set;
        }

        public string CourseGridW
        {
            get;
            set;
        }
        public string CourseGridH
        {
            get;
            set;
        }

        public string CourseGridDefaultFieldSorting
        {
            get;
            set;
        }



    }


}
