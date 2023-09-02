using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Data.School.Entities;
using System.Web.Script.Serialization;
using Gsmu.Api.Data.School.CustomTranscriptModel;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class CustomTranscriptController : Controller
    {
        // GET: Adm/CustomTranscript
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TranscriptGenerator()
        {
            int transcriptid = 0;
            int.TryParse(Request.QueryString["tid"], out transcriptid);
            CustomTranscriptModel transcript = new CustomTranscriptModel(transcriptid);



            return View(transcript);
        }
        public string UpdateTranscriptTemplate(string DefaultTranscript,string customfield1, string customfield2, string customfield3, string optionaltext, string pdfproperty, string selectedfields, string studentaddress, string studentname, string header,string dateprint, string coursedetails, int transcriptid = 0)
        {
            customtranscript customtrans = new customtranscript();

            dynamic pdfpropertyconfig = BuildTranscriptForUpdate(pdfproperty);
            dynamic headeconfig = BuildTranscriptForUpdate(header);
            dynamic customfield1config = BuildTranscriptForUpdate(customfield1);
            dynamic customfield2config = BuildTranscriptForUpdate(customfield2);
            dynamic customfield3config = BuildTranscriptForUpdate(customfield3);
            dynamic optionaltextconfig = BuildTranscriptForUpdate(optionaltext);
            dynamic studentaddressconfig = BuildTranscriptForUpdate(studentaddress);
            dynamic studentnameconfig = BuildTranscriptForUpdate(studentname);
            dynamic dateprintconfig = BuildTranscriptForUpdate(dateprint);
            customtrans.coursegridconfiguration = coursedetails;
            dynamic coursedetailsconfig = BuildTranscriptForUpdate(coursedetails);

            //pdfProperties
            customtrans.transtitle = pdfpropertyconfig["description"];
            customtrans.transwidth = int.Parse(pdfpropertyconfig["width"]);
            customtrans.transheight = int.Parse(pdfpropertyconfig["height"]);
            customtrans.boundX = int.Parse(pdfpropertyconfig["width"]);
            customtrans.boundY = int.Parse(pdfpropertyconfig["height"]);
            
            customtrans.transtitle = pdfpropertyconfig["description"];
            customtrans.transtitle = pdfpropertyconfig["description"];
            customtrans.transtitle = pdfpropertyconfig["description"];
            customtrans.transtitle = pdfpropertyconfig["description"];
            customtrans.isdefaultselected = Int16.Parse(DefaultTranscript);


            //header config
            customtrans.useHeader1 = ConvertboolToInt(headeconfig["visible"]);
            customtrans.fontsizeHeader1 = BuildFontProperty(headeconfig["font"]);
            customtrans.locheader1 = BuildBasicFieldProperties(headeconfig["width"], headeconfig["height"], headeconfig["x"], headeconfig["y"]);
            customtrans.OptionalHeaderText = headeconfig["additionalinfo"];
            //Optional Text
            customtrans.useInfo1 = ConvertboolToInt(optionaltextconfig["visible"]);
            customtrans.fontsizeInfo1 = BuildFontProperty(optionaltextconfig["font"]);
            customtrans.locinfo1 = BuildBasicFieldProperties(optionaltextconfig["width"], optionaltextconfig["height"], optionaltextconfig["x"], optionaltextconfig["y"]);
            customtrans.textInfo1 = optionaltextconfig["additionalinfo"];
            //Student Name
            customtrans.fontsizename1 = BuildFontProperty(studentnameconfig["font"]);
            customtrans.locname1 = BuildBasicFieldProperties(studentnameconfig["width"], studentnameconfig["height"], studentnameconfig["x"], studentnameconfig["y"]);
            customtrans.alignsname1 = ConvertAlignmenttoInt(studentnameconfig["align"]);


            //Student Address
            customtrans.useAddress1 = ConvertboolToInt(studentaddressconfig["visible"]);
            customtrans.fontsizeAddress1 = BuildFontProperty(studentaddressconfig["font"]);
            customtrans.locaddress1 = BuildBasicFieldProperties(studentaddressconfig["width"], studentaddressconfig["height"], studentaddressconfig["x"], studentaddressconfig["y"]);


            //Custom Field 1
            customtrans.useStudField1 = ConvertboolToInt(customfield1config["visible"]);
            customtrans.fontsizeStudfield1 = BuildFontProperty(customfield1config["font"]);
            customtrans.locStudField1 = BuildBasicFieldProperties(customfield1config["width"], customfield1config["height"], customfield1config["x"], customfield1config["y"]);
            customtrans.alignstudfield1 = ConvertAlignmenttoInt(customfield1config["align"]);
            customtrans.selectedstudfield1 = customfield1config["additionalinfo"];
            //Custom Field 2
            customtrans.useStudField2 = ConvertboolToInt(customfield2config["visible"]);
            customtrans.fontsizeStudfield2 = BuildFontProperty(customfield2config["font"]);
            customtrans.locStudField2= BuildBasicFieldProperties(customfield2config["width"], customfield2config["height"], customfield2config["x"], customfield2config["y"]);
            customtrans.alignstudfield2 = ConvertAlignmenttoInt(customfield2config["align"]);
            customtrans.selectedstudfield2 = customfield2config["additionalinfo"];

            //Custom Field 3
            customtrans.useStudField3 = ConvertboolToInt(customfield3config["visible"]);
            customtrans.fontsizeStudfield3 = BuildFontProperty(customfield3config["font"]);
            customtrans.locStudField3 = BuildBasicFieldProperties(customfield3config["width"], customfield3config["height"], customfield3config["x"], customfield3config["y"]);
            customtrans.alignstudfield3 = ConvertAlignmenttoInt(customfield3config["align"]);
            customtrans.selectedstudfield3 = customfield3config["additionalinfo"];

            // dateprintconfig
            customtrans.useDatePrint = ConvertboolToInt(dateprintconfig["visible"]);
            customtrans.fontsizeDateprint = BuildFontProperty(dateprintconfig["font"]);
            customtrans.locDatePrint = BuildBasicFieldProperties(dateprintconfig["width"], dateprintconfig["height"], dateprintconfig["x"], dateprintconfig["y"]);
            customtrans.aligndateprinted = ConvertAlignmenttoInt(dateprintconfig["align"]);

            //Course Area
            customtrans.selectedcoursefieldsettings = selectedfields;
            customtrans.locCoursename = BuildBasicFieldProperties(coursedetailsconfig["width"], coursedetailsconfig["height"], coursedetailsconfig["x"], coursedetailsconfig["y"]);
            

            customtrans.customtranid = transcriptid;
            if (customtrans.isdefaultselected == 1)
            {
                using (var db = new SchoolEntities())
                {
                    var some = db.customtranscripts.Where(transcript => transcript.isdefaultselected==1).ToList();
                    some.ForEach(a => a.isdefaultselected = 0);
                    db.SaveChanges();
                }
            }
            CustomTranscriptModel model = new CustomTranscriptModel(transcriptid);          
            model.SaveChanges(customtrans);
            return "Update Successful";

        }

        public dynamic BuildTranscriptForUpdate(string json)
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
            dynamic settingsconfig = j.Deserialize(json, typeof(object));
            return settingsconfig;
        }
        public int ConvertboolToInt(bool value)
        {
            if (value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int ConvertAlignmenttoInt(string value)
        {

            if (value.ToLower() == "left")
            {
                return 1;
            }
            else if (value.ToLower() == "right")
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
        public string BuildBasicFieldProperties(decimal w, decimal h, string x, string y)
        {
            return x.Replace("px", "") + "," + y.Replace("px", "") + "," + w.ToString() + "," + h.ToString();
        }

        public int BuildFontProperty(string value)
        {
            return int.Parse(value.Replace("px", ""));
        }

        public ActionResult UploadImage(HttpPostedFileBase file, string transid)
        {
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/admin/images/pdfimages"), pic);
                // file is uploaded
                file.SaveAs(path);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
                customtranscript customtrans = new customtranscript();
                CustomTranscriptModel model = new CustomTranscriptModel(int.Parse(transid));
                customtrans.backgroundimage = pic;
                customtrans.customtranid = int.Parse(transid);
                model.SaveBackgroundImage(customtrans);


            }
            return RedirectToAction("TranscriptGenerator", "customtranscript", new { tid = transid });
        }

        public int CreateNewTranscript()
        {
            CustomTranscriptModel model = new CustomTranscriptModel(0);
            return model.NewTranscript();
        }
    }
}