using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PDFjet.NET;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Gsmu.Api.Authorization;

namespace Gsmu.Api.Export.GradeCertificate
{
    public class PdfGradeCertificate : IDisposable
    {
        private entities.SchoolEntities db = null;
        public PdfGradeCertificate(entities.Course course, entities.Course_Roster roster, entities.Transcript transciprt)
        {
            db = new entities.SchoolEntities();
            Course = course;
            CourseCertificate = course.coursecertificate.Value;
            int CourseCeritfication = course.CourseCertificationsId.Value;
            CourseRoster = roster;
            Transciprt = transciprt;
            CourseInstructor = (from inst in db.Instructors where inst.INSTRUCTORID == course.INSTRUCTORID || inst.INSTRUCTORID == course.INSTRUCTORID2 || inst.INSTRUCTORID == course.INSTRUCTORID3 select inst).ToList();
            PdfHeaderFooterInfo = (from pdfinfo in db.PDFHeaderFooterInfoes select pdfinfo).FirstOrDefault();
            PdfFileName = "Certificate" + Guid.NewGuid() + ".pdf";
            var file = System.Web.HttpContext.Current.Server.MapPath("~/Temp/") + PdfFileName;
            PdfOutFile = file;


            CustomCertificate = (from cc in db.customcetificates where cc.customcertid == CourseCertificate select cc).FirstOrDefault();
            if (CustomCertificate == null)
            {
                var certificationProgram = (from certp in db.Certifications where certp.CertificationsId == CourseCeritfication select certp).FirstOrDefault();
                if (certificationProgram != null)
                {
                    CustomCertificate = (from cc in db.customcetificates where cc.customcertid == certificationProgram.CertificationsCustomCertId select cc).FirstOrDefault();
                }
                else
                {
                    CustomCertificate = (from cc in db.customcetificates where cc.defaultcert==1 select cc).FirstOrDefault();
                    if (CustomCertificate == null)
                    {
                        PdfFileName = "";
                    }
                }
            }
        }

        public void Execute()
        {
            var settings = Settings.Instance;
            var mi1 = settings.GetMasterInfo();
            var mi2 = settings.GetMasterInfo2();
            var mi3 = settings.GetMasterInfo3();
            var mi4 = settings.GetMasterInfo4();



            var certtitle = CustomCertificate.certtitle;
            var certheight = Double.Parse(CustomCertificate.certheight.ToString());
            var certwidth = Double.Parse(CustomCertificate.certwidth.ToString());
            var marginTop = Double.Parse(CustomCertificate.marginTop.Value.ToString());
            var marginBottom = Double.Parse(CustomCertificate.marginBottom.Value.ToString());
            var marginLeft = Double.Parse(CustomCertificate.marginLeft.Value.ToString());
            var marginRight = Double.Parse(CustomCertificate.marginRight.Value.ToString());
            var boundX = CustomCertificate.boundX;
            var boundY = CustomCertificate.boundY;
            var useCourseNum = CustomCertificate.useCourseNum;
            var useLocation = CustomCertificate.useLocation;
            var backgroundimage = CustomCertificate.backgroundimage;
            var locheader1 = CustomCertificate.locheader1;
            var useHeader1 = CustomCertificate.useHeader1;
            var fontsizeHeader1 = CustomCertificate.fontsizeHeader1;
            var locheader2 = CustomCertificate.locheader2;
            var useHeader2 = CustomCertificate.useHeader2;
            var fontsizeHeader2 = CustomCertificate.fontsizeHeader2;
            //textInfo1 = CustomCertificate.textInfo1; //DEPRECATED
            var locinfo1 = CustomCertificate.locinfo1; //DEPRECATED (not commented out for backward compatability)
            //useInfo1 = CustomCertificate.useInfo1; //DEPRECATED
            //fontsizeInfo1 = CustomCertificate.fontsizeInfo1; //DEPRECATED
            //textInfo2 = CustomCertificate.textInfo2; //DEPRECATED
            var locinfo2 = CustomCertificate.locinfo2; //DEPRECATED (not commented out for backward compatability)
            //useInfo2 = CustomCertificate.useInfo2; //DEPRECATED
            //fontsizeInfo2 = CustomCertificate.fontsizeInfo2; //DEPRECATED
            var locname1 = CustomCertificate.locname1; //DEPRECATED (not commented out for backward compatability)
            var locname2 = CustomCertificate.locname2;
            var useExtraName = CustomCertificate.useExtraName;
            //alignsname = CustomCertificate.alignsname; //DEPRECATED
            //textCourseinfo1 = CustomCertificate.textCourseinfo1; //DEPRECATED
            var loccourseinfo1 = CustomCertificate.loccourseinfo1; //DEPRECATED (not commented out for backward compatability)
            //useCInfo1 = CustomCertificate.useCInfo1; //DEPRECATED
            //fontsizeCInfo1 = CustomCertificate.fontsizeCInfo1; //DEPRECATED
            var loccourseinfo2 = CustomCertificate.loccourseinfo2; //DEPRECATED (not commented out for backward compatability)
            //useCInfo2= rsCustom("useCInfo2; //DEPRECATED
            //fontsizeCInfo2 = CustomCertificate.fontsizeCInfo2; //DEPRECATED
            //loccoursedate1 = CustomCertificate.loccoursedate1; //DEPRECATED
            var loccoursedate2 = CustomCertificate.loccoursedate2;
            var locInstructor = CustomCertificate.locInstructor;
            var useInstructor = CustomCertificate.useInstructor;
            var fontsizeInstructor = CustomCertificate.fontsizeInstructor;
            var locsig1 = CustomCertificate.locsig1;
            var useSig1 = CustomCertificate.useSig1;
            var locsig2 = CustomCertificate.locsig2;
            var useSig2 = CustomCertificate.useSig2;
            var locfooter = CustomCertificate.locfooter;
            var useFooter = CustomCertificate.useFooter;

            var codefile = CustomCertificate.codefile;
            var useUploadedLogo = CustomCertificate.useUploadedLogo;
            var dateadded = CustomCertificate.dateadded;
            var disabled = CustomCertificate.disabled;
            var locstudfield1 = CustomCertificate.locstudfield1;
            var usestudfield1 = CustomCertificate.usestudfield1;
            var fontsizestudfield1 = CustomCertificate.fontsizestudfield1;
            var selectedstudfield1 = CustomCertificate.selectedstudfield1;
            var locstudfield2 = CustomCertificate.locstudfield2;
            var usestudfield2 = CustomCertificate.usestudfield2;
            var fontsizestudfield2 = CustomCertificate.fontsizestudfield2;
            var selectedstudfield2 = CustomCertificate.selectedstudfield2;
            var locstudfield3 = CustomCertificate.locstudfield3;
            var usestudfield3 = CustomCertificate.usestudfield3;
            var fontsizestudfield3 = CustomCertificate.fontsizestudfield3;
            var selectedstudfield3 = CustomCertificate.selectedstudfield3;
            //loccredits = CustomCertificate.loccredits; //DEPRECATED
            //usecredits = CustomCertificate.usecredits; //DEPRECATED
            //fontsizecredits = CustomCertificate.fontsizecredits; //DEPRECATED
            var loccourseloc = CustomCertificate.loccourseloc;
            var usecourseloc = CustomCertificate.usecourseloc;
            var fontsizecourseloc = CustomCertificate.fontsizecourseloc;
            var loccoursenumber = CustomCertificate.loccoursenumber;
            var usecoursenumber = CustomCertificate.usecoursenumber;
            var fontsizecoursenumber = CustomCertificate.fontsizecoursenumber;
            var loccourseicon = CustomCertificate.loccourseicon;
            var usecourseicon = CustomCertificate.usecourseicon;
            var fontsizecourseicon = CustomCertificate.fontsizecourseicon;
            var loccoursedesc = CustomCertificate.loccoursedesc;
            var usecoursedesc = CustomCertificate.usecoursedesc;
            var fontsizecoursedesc = CustomCertificate.fontsizecoursedesc;
            //usecoursedate = CustomCertificate.usecoursedate;
            //fontsizecoursedate = CustomCertificate.fontsizecoursedate; //DEPRECATED
            var useCourseDuration = CustomCertificate.useCourseDuration;
            //fontsizestudname = CustomCertificate.fontsizestudname; //DEPRECATED
            var showcoursetime = CustomCertificate.showcoursetime;
            var showcourseenddate = CustomCertificate.showcourseenddate;
            var showcourseenddateonly = CustomCertificate.showcourseenddateonly;
            var useLicense = CustomCertificate.useLicense;
            var fontsizelicense = CustomCertificate.fontsizelicense;
            var loccourselicense = CustomCertificate.loccourselicense;
            var surveyusecompdate = CustomCertificate.surveyusecompdate;
            var uselicnumber = CustomCertificate.uselicnumber;
            var loclicnumber = CustomCertificate.loclicnumber;
            var fontsizelicnumber = CustomCertificate.fontsizelicnumber;
            var useDatePrint = CustomCertificate.useDatePrint;
            var locDatePrint = CustomCertificate.locDatePrint;
            var fontsizeDateprint = CustomCertificate.fontsizeDateprint;
            var useStudentAddress = CustomCertificate.useStudentAddress;
            var locStudentAddress = CustomCertificate.locStudentAddress;
            var fontsizeStudentAddress = CustomCertificate.fontsizeStudentAddress;
            var signatureDisplay = CustomCertificate.signatureDisplay;
            var usecustomcredit = CustomCertificate.usecustomcredit;
            var loccustomcredit = CustomCertificate.loccustomcredit1;
            var usestudentid = CustomCertificate.usestudentid;
            var locstudentid = CustomCertificate.locstudentid;
            var fontsizestudid = CustomCertificate.fontsizestudid;

            //begin new json certificate fields
            //dates
            var startdateSettings = CustomCertificate.startdateSettings;
            var enddateSettings = CustomCertificate.enddateSettings;
            var daterangeSettings = CustomCertificate.daterangeSettings;
            var datelistSettings = CustomCertificate.datelistSettings;
            var rosterdateaddedSettings = CustomCertificate.rosterdateaddedSettings;
            //credits
            var credithoursSettings = CustomCertificate.credithoursSettings;
            var inservicehoursSettings = CustomCertificate.inservicehoursSettings;
            var customcredittypeSettings = CustomCertificate.customcredittypeSettings;
            var ceucreditSettings = CustomCertificate.ceucreditSettings;
            var graduatecreditSettings = CustomCertificate.graduatecreditSettings;
            //text
            var customtextSettings = CustomCertificate.customtextSettings;
            var studentnameSettings = CustomCertificate.studentnameSettings;
            var coursenameSettings = CustomCertificate.coursenameSettings;
            var courseaddtextSettings = CustomCertificate.courseaddtextSettings;
            var optionaltext1Settings = CustomCertificate.optionaltext1Settings;
            var optionaltext2Settings = CustomCertificate.optionaltext2Settings;
            var globalsettings = CustomCertificate.miscglobalsettings;

            //alignment, font weight, font style
            var studcustomfield1settings = CustomCertificate.StudCustomField1Settings;
            var studcustomfield2settings = CustomCertificate.StudCustomField2Settings;
            var studcustomfield3settings = CustomCertificate.StudCustomField3Settings;

            string studcustomfield1labelstyle = string.Empty;
            string studcustomfield1alignment = string.Empty;

            string studcustomfield2labelstyle = string.Empty;
            string studcustomfield2alignment = string.Empty;

            string studcustomfield3labelstyle = string.Empty;
            string studcustomfield3alignment = string.Empty;

            if (string.IsNullOrEmpty(studcustomfield1settings))
            {
                studcustomfield1settings = settings.GetFieldValueFromDbByQuery("StudCustomField1Settings", "customcetificate", "where customcertid=" + CourseCertificate);
            }
            if (string.IsNullOrEmpty(studcustomfield2settings))
            {
                studcustomfield2settings = settings.GetFieldValueFromDbByQuery("StudCustomField2Settings", "customcetificate", "where customcertid=" + CourseCertificate);
            }
            if (string.IsNullOrEmpty(studcustomfield3settings))
            {
                studcustomfield3settings = settings.GetFieldValueFromDbByQuery("StudCustomField3Settings", "customcetificate", "where customcertid=" + CourseCertificate);
            }

            if (!string.IsNullOrEmpty(studcustomfield1settings))
            {
                studcustomfield1labelstyle = GetSettingsValue(studcustomfield1settings, "labelstyle");
                studcustomfield1alignment = GetSettingsValue(studcustomfield1settings, "alignment");
            }
            if (!string.IsNullOrEmpty(studcustomfield2settings))
            {
                studcustomfield2labelstyle = GetSettingsValue(studcustomfield2settings, "labelstyle");
                studcustomfield2alignment = GetSettingsValue(studcustomfield2settings, "alignment");
            }
            if (!string.IsNullOrEmpty(studcustomfield3settings))
            {
                studcustomfield3labelstyle = GetSettingsValue(studcustomfield3settings, "labelstyle");
                studcustomfield3alignment = GetSettingsValue(studcustomfield3settings, "alignment");
            }

            //end 

            string hidezerocredits = "0";
            try
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic settingsconfig = j.Deserialize(globalsettings, typeof(object));
                if (settingsconfig.ContainsKey("hidezerocredits"))
                {
                    hidezerocredits = settingsconfig["hidezerocredits"];
                }
            }
            catch { }




            FileStream = new FileStream(PdfOutFile, FileMode.Create);
            BufferedStream = new BufferedStream(FileStream);
            var bs = BufferedStream;
            PDF pdf = new PDF(bs);
            pdf.SetTitle("Certificate Custom");
            pdf.SetSubject("Certificate");
            pdf.SetAuthor("GoSignMeUp!");

            Page page;
            string orientaion = "1";
            if (certwidth > certheight)
            {
                page = new Page(pdf, Letter.LANDSCAPE);
            }
            else
            {
                page = new Page(pdf, Letter.PORTRAIT);
            }

            PDFjet.NET.Font f0 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            PDFjet.NET.Font f1 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            PDFjet.NET.Font f2 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            PDFjet.NET.Font f3 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            PDFjet.NET.Font f4 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);

            f1.SetSize(11);
            f2.SetSize(11);
            f3.SetSize(23);
            f4.SetSize(18);
            f0.SetSize(10);

            if (backgroundimage != null)
            {
                string fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/images/pdfimages/" + backgroundimage.ToString());
                if (File.Exists(fileName))
                {
                    string extension = System.IO.Path.GetExtension(fileName).ToLower();
                    int imagetype = ImageType.JPG;

                    if (extension.ToLower() == ".jpg")
                    {
                        imagetype = ImageType.JPG;

                    }
                    else if (extension.ToLower() == ".bmp")
                    {
                        imagetype = ImageType.BMP;

                    }
                    else if (extension.ToLower() == ".png")
                    {
                        imagetype = ImageType.PNG;

                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        imagetype = ImageType.PDF;

                    }
                    if ((extension.ToLower() != ".gif") && (extension.ToLower() != ".gif"))
                    {
                        FileStream filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        PDFjet.NET.Image bckgroundimage = new PDFjet.NET.Image(pdf, filestream, imagetype);
                        float originalwidth = bckgroundimage.GetWidth();
                        float originalheight = bckgroundimage.GetHeight();
                        bckgroundimage.SetPosition(18, 18);

                        //Compute the Height and width factor of the image.
                        float widthfactor = float.Parse((certwidth - (marginLeft + marginRight)).ToString()) / originalwidth;
                        float heightfactor = float.Parse((certheight - (marginTop + marginBottom)).ToString()) / originalheight;
                        //end

                        bckgroundimage.ScaleBy(widthfactor, heightfactor);
                        bckgroundimage.DrawOn(page);
                        filestream.Close();

                    }
                }
            }

            int verticalpadding = 0;
            int horizontalpadding = 0;
            for (int signcounter = 1; signcounter <= 3; signcounter++)
            {
                string fileName = "";

                if ((Course.ElectronicSignatureFile != "") && (signcounter == 1))
                {
                    fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/Documents/" + Course.ElectronicSignatureFile);

                }
                else if ((Course.ElectronicSignatureFile2 != "") && (signcounter == 2))
                {
                    fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/Documents/" + Course.ElectronicSignatureFile2);

                }
                else if ((Course.ElectronicSignatureFile3 != "") && (signcounter == 3))
                {
                    fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/Documents/" + Course.ElectronicSignatureFile3);

                }


                if (File.Exists(fileName))
                {
                    string extension = System.IO.Path.GetExtension(fileName).ToLower();
                    int imagetype = ImageType.JPG;

                    if (extension.ToLower() == ".jpg")
                    {
                        imagetype = ImageType.JPG;

                    }
                    else if (extension.ToLower() == ".bmp")
                    {
                        imagetype = ImageType.BMP;

                    }
                    else if (extension.ToLower() == ".png")
                    {
                        imagetype = ImageType.PNG;

                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        imagetype = ImageType.PDF;

                    }
                    if ((extension.ToLower() != ".gif") && (extension.ToLower() != ".gif"))
                    {
                        FileStream filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        PDFjet.NET.Image signatureimage = new PDFjet.NET.Image(pdf, filestream, imagetype);
                        float originalwidth = signatureimage.GetWidth();
                        float originalheight = signatureimage.GetHeight();


                        string[] signaturelocation = locsig1.Split(',');
                        double x = double.Parse(signaturelocation[0].Replace(" ", String.Empty));
                        double y = double.Parse(signaturelocation[1].Replace(" ", String.Empty));
                        double w = double.Parse(signaturelocation[2].Replace(" ", String.Empty));
                        double h = double.Parse(signaturelocation[3].Replace(" ", String.Empty));
                        //Compute the Height and width factor of the image.
                                                if (w < 150)
                        {
                            w = 150;
                        }

                        if (h < 30)
                        {
                            h = 30;
                        }
                        signatureimage.SetPosition(x + horizontalpadding + marginLeft, y + verticalpadding + marginTop);
                        float widthfactor = float.Parse((w).ToString()) / originalwidth;
                        float heightfactor = float.Parse((h).ToString()) / originalheight;
                        //end

                        if (heightfactor < 0)
                        {
                            heightfactor = 0.5F;

                        }
                        if (heightfactor < 0.1)
                        {
                            heightfactor = 0.5F;

                        }
                        if (widthfactor < 0.1)
                        {
                            widthfactor = 0.5F;
                        }
                        if (widthfactor < 0)
                        {
                            widthfactor = 1;
                        }

                      signatureimage.ScaleBy(widthfactor, heightfactor);
                        signatureimage.DrawOn(page);
                        if (signatureDisplay == 0)
                        {
                            horizontalpadding = horizontalpadding + int.Parse(signatureimage.GetWidth().ToString()) + 20;

                        }
                        else
                        {
                            verticalpadding = verticalpadding + int.Parse(signatureimage.GetHeight().ToString()) + 20;
                        }
                        filestream.Close();

                    }
                }
            }

            var student = Gsmu.Api.Data.School.Entities.Student.GetStudent(CourseRoster.STUDENTID.Value);
            //
            string usecustomc = "0";
            int countercustomcredit = 1;

            if (loccustomcredit == null)
            {
                loccustomcredit = "";
            }
            if (usecustomcredit == null)
            {
                usecustomcredit = "";
            }
            string[] customcreditlocations = loccustomcredit.Split(',');

            #region usecustomcredit section
            foreach (var a in usecustomcredit.Split(','))
            {
                usecustomc = a.Replace(" ", string.Empty);
                double x = 0;
                double y = 0;
                double w = 0;
                double h = 0;
                int fontsize = 12;
                string textvalue = "";
                if (usecustomc == "1")
                {

                    if (countercustomcredit == 1)
                    {
                        x = double.Parse(customcreditlocations[0].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[1].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[2].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[3].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours1.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours1.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 2)
                    {
                        x = double.Parse(customcreditlocations[4].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[5].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[6].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[7].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours2.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours2.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 3)
                    {
                        x = double.Parse(customcreditlocations[8].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[9].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[10].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[11].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours3.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours3.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 4)
                    {
                        x = double.Parse(customcreditlocations[12].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[13].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[14].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[15].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours4.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours4.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 5)
                    {
                        x = double.Parse(customcreditlocations[16].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[17].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[18].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[19].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours5.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours5.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 6)
                    {
                        x = double.Parse(customcreditlocations[20].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[21].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[22].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[23].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours6.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours6.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 7)
                    {
                        x = double.Parse(customcreditlocations[24].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[25].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[26].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[27].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours7.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours7.Value.ToString();
                        }
                    }
                    if (countercustomcredit == 8)
                    {
                        x = double.Parse(customcreditlocations[28].Replace(" ", String.Empty));
                        y = double.Parse(customcreditlocations[29].Replace(" ", String.Empty));
                        w = double.Parse(customcreditlocations[30].Replace(" ", String.Empty));
                        h = double.Parse(customcreditlocations[31].Replace(" ", String.Empty));
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.Optionalcredithours8.Value.ToString();
                        }
                        else
                        {
                            textvalue = CourseRoster.Optionalcredithours8.Value.ToString();
                        }
                    }

                }
                f3.SetSize(fontsize);
                Paragraph paragraph = new Paragraph();
                paragraph.Add(new TextLine(f3, textvalue == null ? "" : textvalue));
                paragraph.SetAlignment(Align.CENTER);
                TextColumn columncustom = new TextColumn(f1);
                columncustom.SetLineBetweenParagraphs(true);
                columncustom.SetSpaceBetweenLines(5);

                columncustom.AddParagraph(paragraph);
                columncustom.SetPosition(x + marginTop, y + marginLeft);
                columncustom.SetSize(w, h);
                PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                columncustom.SetSize(certheight, certwidth);
                columncustom.SetLocation(18, 18);
                countercustomcredit = countercustomcredit + 1;


            }
            #endregion
            #region student data printing section
            for (int counter = 0; counter <= 13; counter++)
            {
                int intDsiplay = 0;
                int fontsize = 12;
                string strAlllocation = "";
                string textvalue = "";


                if (counter == 0) //Date Print
                {
                    intDsiplay = useDatePrint.Value;
                    strAlllocation = locDatePrint;
                    textvalue = DateTime.Now.ToShortDateString();
                    fontsize = fontsizeDateprint.Value;
                }
                if (counter == 1) //CourseNumber
                {
                    intDsiplay = usecoursenumber.Value;
                    strAlllocation = loccoursenumber;
                    textvalue = Course.COURSENUM;
                    fontsize = fontsizecoursenumber.Value;
                }
                if (counter == 2) //Course Location
                {
                    intDsiplay = usecourseloc.Value;
                    strAlllocation = loccourseloc;
                    textvalue = Course.LOCATION;
                    fontsize = fontsizecourseloc.Value;
                }
                if (counter == 3) //Student Custom Field 1
                {
                    intDsiplay = usestudfield1.Value;
                    strAlllocation = locstudfield1;
                    textvalue = GetCustomFieldValue(selectedstudfield1, student);
                    fontsize = fontsizestudfield1.Value;
                }
                if (counter == 4) // Header 1
                {
                    intDsiplay = useHeader1.Value;
                    strAlllocation = locheader1;
                    textvalue = PdfHeaderFooterInfo.Header1;
                    fontsize = fontsizeHeader1.Value;
                }
                if (counter == 5)
                {
                    intDsiplay = useHeader2.Value;
                    strAlllocation = locheader2;
                    textvalue = PdfHeaderFooterInfo.Header2;
                    fontsize = fontsizeHeader2.Value;
                }
                if (counter == 6)
                {
                    intDsiplay = useFooter.Value;
                    strAlllocation = locfooter;
                    textvalue = settings.GetMasterInfo3().CertificateFooterText;
                    fontsize = 8;
                }
                if (counter == 7)
                {
                    intDsiplay = usestudentid.Value;
                    strAlllocation = locstudentid;
                    textvalue = student.STUDENTID.ToString();
                    fontsize = fontsizestudid.Value;
                }
                if (counter == 8) //Student Custom Field 2
                {
                    intDsiplay = usestudfield2.Value;
                    strAlllocation = locstudfield2;
                    textvalue = GetCustomFieldValue(selectedstudfield2, student);
                    fontsize = fontsizestudfield2.Value;
                }
                if (counter == 9) //Student Custom Field 3
                {
                    intDsiplay = usestudfield3.Value;
                    strAlllocation = locstudfield3;
                    textvalue = GetCustomFieldValue(selectedstudfield3, student);
                    fontsize = fontsizestudfield3.Value;

                }
                if (counter == 10)
                {
                    intDsiplay = useStudentAddress.Value;
                    strAlllocation = locStudentAddress;
                    if ((student.ADDRESS != "") || (student.ADDRESS != null))
                    {
                        textvalue = textvalue + student.ADDRESS + ",";
                    }
                    if ((student.CITY != "") || (student.CITY != null))
                    {
                        textvalue = textvalue + student.CITY + ",";
                    }
                    if ((student.STATE != "") || (student.STATE != null))
                    {
                        textvalue = textvalue + student.STATE + ",";
                    }
                    if ((student.ZIP != "") || (student.ZIP != null))
                    {
                        textvalue = textvalue + student.ZIP + ",";
                    }
                    textvalue = textvalue.TrimEnd(',').TrimStart(',').Replace(",", ", ");

                    fontsize = fontsizestudfield3.Value;
                }
                if (counter == 11)
                {
                    // this is Extra participant - 
                    intDsiplay = useExtraName.Value;
                    strAlllocation = locname2;
                    textvalue = "";
                    if (!string.IsNullOrEmpty(CourseRoster.ExtraParticipant))
                    {
                        textvalue = CourseRoster.ExtraParticipant.ToString();
                    }

                    // This is  pulling data from Household (need to add configuration for house hold)
                    //foreach (var extraname in CourseRoster.CourseExtraParticipants)
                    //{
                    //    textvalue = textvalue + extraname.StudentFirst + " " + extraname.StudentLast + " ";
                    //}
                }

                if (counter == 12) // Instructor 1
                {
                    intDsiplay = useInstructor.Value;
                    strAlllocation = locInstructor;
                    if (Transciprt != null)
                    {
                        textvalue = Transciprt.InstructorName;
                    }
                    else
                    {
                        foreach (var instuctor in CourseInstructor)
                        {
                            textvalue = textvalue + instuctor.FIRST + " " + instuctor.LAST + "    ";
                        }
                    }
                }

                if (counter == 13)
                {
                    intDsiplay = uselicnumber.Value;
                    strAlllocation = loclicnumber;
                    textvalue = CourseRoster.RosterID.ToString();
                }
                if (intDsiplay == 1)
                {
                    double x = 0;
                    double y = 0;
                    double w = 0;
                    double h = 0;
                    string[] location = strAlllocation.Split(',');
                    try
                    {
                        x = double.Parse(location[0]);
                        y = double.Parse(location[1]);
                        w = double.Parse(location[2]);
                        h = double.Parse(location[3]);
                    }
                    catch
                    {
                        x = 0;
                        y = 0;
                        w = 0;
                        h = 0;
                    }
                    finally
                    {
                    }
                    f3.SetSize(fontsize);
                    Paragraph paragraph = new Paragraph();

                    if (counter == 3)
                    {
                        paragraph = GetSetStudCustomField1Values(pdf, paragraph, textvalue, fontsize, studcustomfield1labelstyle, studcustomfield1alignment);
                    }
                    else if (counter == 8)
                    {
                        paragraph = GetSetStudCustomField2Values(pdf, paragraph, textvalue, fontsize, studcustomfield2labelstyle, studcustomfield2alignment);
                    }
                    else if (counter == 9)
                    {
                        paragraph = GetSetStudCustomField3Values(pdf, paragraph, textvalue, fontsize, studcustomfield3labelstyle, studcustomfield3alignment);
                    }
                    else
                    {
                        paragraph.Add(new TextLine(f3, textvalue == null ? "" : textvalue));
                        paragraph.SetAlignment(Align.CENTER);
                    }

                    TextColumn columncustom = new TextColumn(f1);
                    columncustom.SetLineBetweenParagraphs(true);
                    columncustom.SetSpaceBetweenLines(5);

                    columncustom.AddParagraph(paragraph);
                    columncustom.SetPosition(x + marginTop, y + marginLeft);
                    columncustom.SetSize(w, h);
                    PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                    columncustom.SetSize(certheight, certwidth);
                    columncustom.SetLocation(18, 18);
                }
            }
            #endregion
            #region course info, and course related info section
            for (int jconfigcount = 0; jconfigcount <= 20; jconfigcount++)
            {
                var textconfig = "";
                string textvalue = "";
                string creditlabel = "";
                if (jconfigcount == 0)
                {
                    textconfig = CustomCertificate.startdateSettings;
                    textvalue = Course.CourseTimes.First().COURSEDATE.Value.ToShortDateString();
                }
                if (jconfigcount == 1)
                {
                    textconfig = CustomCertificate.enddateSettings;
                    textvalue = Course.CourseTimes.Last().COURSEDATE.Value.ToShortDateString();
                    if (Course.BBCourseCloned != 0 && Course.OnlineCourse != 0 && (CourseRoster.bb_graded_date.HasValue))
                    {
                        if (CourseRoster.bb_graded_date > Convert.ToDateTime("1990-01-01 00:00:00.000")) {
                            // textvalue = Transciprt.CourseCompletionDate.ToShortDateString();
                            textvalue = CourseRoster.bb_graded_date.Value.ToShortDateString();
                        }
                    }
                    if (Course.BBCourseCloned == 0 && Settings.Instance.GetMasterInfo3().use_onlinetranscribe_date != 0 && Course.OnlineCourse != 0)
                    {
                        if (Transciprt != null)
                        {
                            textvalue = Transciprt.datetranscribed.Value.ToShortDateString();
                        }
                        else
                        {
                            //need a different value here?
                        }
                    }
                }
                if (jconfigcount == 2)
                {
                    textconfig = CustomCertificate.daterangeSettings;

                    textvalue = Course.CourseTimes.First().COURSEDATE.Value.ToShortDateString() + " - " + Course.CourseTimes.Last().COURSEDATE.Value.ToShortDateString();
                }
                if (jconfigcount == 3)
                {
                    textconfig = CustomCertificate.datelistSettings;
                    foreach (var datevalue in Course.CourseTimes)
                    {
                        textvalue = textvalue + datevalue.COURSEDATE.Value.ToShortDateString() + ",";
                    }
                    textvalue = textvalue.TrimEnd(',');
                    textvalue = textvalue.Replace(",", ", ");

                }
                if (jconfigcount == 4)
                {
                    textconfig = CustomCertificate.credithoursSettings;
                    if (Transciprt != null)
                    {
                        textvalue = Transciprt.HOURS.ToString();
                        //creditlabel = Settings.Instance.GetMasterInfo2().CreditHoursName;
                    }
                    else
                    {
                        textvalue = CourseRoster.HOURS.ToString();
                    }
                    creditlabel = Settings.Instance.GetMasterInfo2().CreditHoursName;
                }
                if (jconfigcount == 5)
                {
                    textconfig = CustomCertificate.inservicehoursSettings;
                    if (Transciprt != null)
                    {
                        textvalue = Transciprt.InserviceHours.ToString();

                    }
                    else
                    {
                        textvalue = CourseRoster.InserviceHours.ToString();
                    }
                    creditlabel = Settings.Instance.GetMasterInfo2().InserviceHoursName;
                }
                if (jconfigcount == 6)
                {
                    textconfig = CustomCertificate.customcredittypeSettings;
                    if (Transciprt != null)
                    {
                        try
                        {
                            JavaScriptSerializer j = new JavaScriptSerializer();
                            dynamic configuration = j.Deserialize(Course.CourseConfiguration, typeof(object));
                            var allowcreditpurchase = configuration["purchasecredit"];
                            if (Course.CourseConfiguration != null)
                            {

                                if (Transciprt.IsHoursPaid == 1)
                                {
                                    if (allowcreditpurchase == 1)
                                    {
                                        textvalue = Transciprt.CustomCreditHours.ToString();
                                    }
                                    else
                                    {
                                        textvalue = "0";
                                    }
                                }
                                else
                                {
                                    textvalue = Transciprt.CustomCreditHours.ToString();
                                }
                            }
                        }
                        catch
                        {
                            textvalue = Transciprt.CustomCreditHours.ToString();
                        }


                    }
                    else
                    {
                        try
                        {
                            JavaScriptSerializer j = new JavaScriptSerializer();
                            dynamic configuration = j.Deserialize(Course.CourseConfiguration, typeof(object));
                            var allowcreditpurchase = configuration["purchasecredit"];
                            if (allowcreditpurchase == 1)
                            {
                                textvalue = "0";
                            }
                            else
                            {
                                textvalue = CourseRoster.CustomCreditHours.ToString();
                            }
                        }
                        catch
                        {
                            textvalue = CourseRoster.CustomCreditHours.ToString();
                        }
                    }
                    creditlabel = Settings.Instance.GetMasterInfo2().CustomCreditTypeName;
                }
                if (jconfigcount == 7)
                {
                    textconfig = CustomCertificate.ceucreditSettings;
                    if (Transciprt != null) //allow pre survey
                    {
                        textvalue = Transciprt.ceucredit.ToString();

                    }
                    else
                    {
                        textvalue = CourseRoster.ceucredit.ToString();
                    }
                    creditlabel = Settings.Instance.GetMasterInfo2().CEUCreditLabel;
                }
                if (jconfigcount == 8)
                {
                    textconfig = CustomCertificate.graduatecreditSettings;
                    if (Transciprt != null)
                    {
                        textvalue = Transciprt.graduatecredit.ToString();

                    }
                    else
                    {
                        textvalue = CourseRoster.graduatecredit.ToString();
                    }
                    creditlabel = "Graduate";
                }
                if (jconfigcount == 9)
                {
                    textconfig = CustomCertificate.customtextSettings;
                    textvalue = ""; //No value from Survey module
                }
                if (jconfigcount == 10)
                {
                    textconfig = CustomCertificate.rosterdateaddedSettings;
                    textvalue = CourseRoster.DATEADDED.Value.ToShortDateString();
                }
                if (jconfigcount == 11)
                {
                    textconfig = CustomCertificate.studentnameSettings;

                    textvalue = student.FIRST + " " + student.LAST;
                }

                if (jconfigcount == 12)
                {
                    textconfig = CustomCertificate.coursenameSettings;
                    textvalue = Course.COURSENAME;
                }
                if (jconfigcount == 13)
                {
                    textconfig = CustomCertificate.courseaddtextSettings;
                }
                if (jconfigcount == 14)
                {
                    textconfig = CustomCertificate.optionaltext1Settings;
                }
                if (jconfigcount == 15)
                {
                    textconfig = CustomCertificate.optionaltext2Settings;
                }
                if (jconfigcount == 16)
                {
                    textconfig = CustomCertificate.CustomCourseField1Settings;
                    textvalue = !string.IsNullOrEmpty(Course.CustomCourseField1) ? Course.CustomCourseField1.ToString() : "";
                }
                if (jconfigcount == 17)
                {
                    textconfig = CustomCertificate.CustomCourseField2Settings;
                    textvalue = !string.IsNullOrEmpty(Course.CustomCourseField2) ? Course.CustomCourseField2.ToString() : "";
                }
                if (jconfigcount == 18)
                {
                    textconfig = CustomCertificate.CustomCourseField3Settings;
                    textvalue = !string.IsNullOrEmpty(Course.CustomCourseField3) ? Course.CustomCourseField3.ToString() : "";
                }
                if (jconfigcount == 19)
                {
                    textconfig = CustomCertificate.CustomCourseField4Settings;
                    textvalue = !string.IsNullOrEmpty(Course.CustomCourseField4) ? Course.CustomCourseField4.ToString() : "";
                }
                if (jconfigcount == 20)
                {
                    textconfig = CustomCertificate.CustomCourseField5Settings;
                    textvalue = !string.IsNullOrEmpty(Course.CustomCourseField5) ? Course.CustomCourseField5.ToString() : "";
                }
                if ((textconfig != "") && (textconfig != null))
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(textconfig, typeof(object));
                    string usefield = settingsconfig["usefield"];
                    if (textvalue == "")
                    {
                        if (settingsconfig.ContainsKey("text"))
                        {
                            textvalue = settingsconfig["text"];
                        }
                    }
                    double fontsize = 12;
                    double x = 0;
                    double y = 0;
                    double w = 0;
                    double h = 0;
                    string align = "0";
                    try
                    {
                        if (settingsconfig.ContainsKey("fontsize"))
                        {
                            fontsize = double.Parse(settingsconfig["fontsize"]);
                        }
                        if (settingsconfig.ContainsKey("x"))
                        {
                            x = double.Parse(settingsconfig["x"]);
                        }
                        if (settingsconfig.ContainsKey("y"))
                        {
                            y = double.Parse(settingsconfig["y"]);
                        }
                        if (settingsconfig.ContainsKey("w"))
                        {
                            w = double.Parse(settingsconfig["w"]);
                        }
                        if (settingsconfig.ContainsKey("h"))
                        {
                            h = double.Parse(settingsconfig["h"]);
                        }
                        if (settingsconfig.ContainsKey("alignment"))
                        {
                            align = settingsconfig["alignment"];
                        }

                        if (settingsconfig.ContainsKey("labelstyle"))
                        {
                            string style = settingsconfig["labelstyle"];
                            if (style != "")
                            {
                                //textvalue = GetCertificateTextFormat(style, textvalue, creditlabel,hidezerocredits);
                                textvalue = GetCertificateTextFormat(style, textvalue, creditlabel, hidezerocredits);
                            }

                            if ((jconfigcount == 4))
                            {
                                if (settingsconfig.ContainsKey("useDataOption"))
                                {
                                    string dataOption = settingsconfig["useDataOption"];
                                    if (dataOption == "Course")
                                    {
                                        //textvalue = GetCertificateTextFormat(style, Course.CREDITHOURS.Value.ToString(), creditlabel, hidezerocredits);
                                        textvalue = GetCertificateTextFormat(style, Course.CREDITHOURS.Value.ToString(), creditlabel, hidezerocredits);
                                    }
                                }
                            }
                        }


                    }
                    catch
                    {
                        align = "center";
                    }
                    finally
                    { }

                    if (usefield != "0")
                    {
                        Paragraph paragraph = new Paragraph();
                        if (align == "1")
                        {
                            paragraph.SetAlignment(Align.LEFT);
                        }
                        else if (align == "2")
                        {
                            paragraph.SetAlignment(Align.RIGHT);
                        }
                        else
                        {
                            paragraph.SetAlignment(Align.CENTER);
                        }
                        f3.SetSize(fontsize);
                        paragraph.Add(new TextLine(f3, textvalue == null ? "" : textvalue));

                        TextColumn columncustom = new TextColumn(f1);
                        columncustom.SetLineBetweenParagraphs(true);
                        columncustom.SetSpaceBetweenLines(5);

                        columncustom.AddParagraph(paragraph);
                        columncustom.SetPosition(x + marginTop, y + marginLeft);
                        columncustom.SetSize(w, h);
                        PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                        columncustom.SetSize(certheight, certwidth);
                        columncustom.SetLocation(18, 18);
                    }
                }
            }
            #endregion
            pdf.Flush();
            bs.Close();
        }

        private string GetCertificateTextFormat(string format, string credtivalue, string creditlabel, string hidezerocredit)
        {
            string result = "";
            if (format == "1")
            {
                result = creditlabel + ": " + credtivalue;
            }
            else if (format == "2")
            {
                result = credtivalue + " " + creditlabel;
            }
            else
            {
                result = credtivalue;
            }
            if (hidezerocredit.Trim() == "1" && credtivalue.Trim() == "0")
            {
                result = "";
            }
            return result;

        }

        private string GetCustomFieldValueRoster(string selectedField, entities.Course_Roster courseroster)
        {
            try
            {
                string value = courseroster.GetType().GetProperty(selectedField).GetValue(this.CourseRoster, null).ToString();
                return value;
            }
            catch
            {
                return "";
            }
        }
        private string getCourseDates(int rosterid, entities.Course_Roster courseroster, entities.Transcript transcriptrecords)
        {
            string datevalue = "";
            try
            {
                return datevalue;
            }
            catch
            {
                return "";
            }

        }

        private string GetCustomFieldValue(string selectedField, entities.Student student)
        {
            selectedField = selectedField.ToLower();
            db = new entities.SchoolEntities();
            if (selectedField == "school")
            {
                string school = (from sch in db.Schools where sch.locationid == student.SCHOOL select sch.LOCATION).FirstOrDefault();
                return school;
            }
            if (selectedField == "district")
            {
                string district = (from sch in db.Districts where sch.DISTID == student.DISTRICT select sch.DISTRICT1).FirstOrDefault();
                return district;
            }
            if (selectedField == "grade")
            {
                string grade = (from sch in db.Grade_Levels where sch.GRADEID == student.GRADE select sch.GRADE).FirstOrDefault();
                return grade;
            }
            if (selectedField == "studregfield1")
            {
                return student.StudRegField1;
            }
            if (selectedField == "studregfield2")
            {
                return student.StudRegField2;
            }
            if (selectedField == "studregfield3")
            {
                return student.StudRegField3;
            }
            if (selectedField == "studregfield4")
            {
                return student.StudRegField4;
            }
            if (selectedField == "studregfield5")
            {
                return student.StudRegField5;
            }
            if (selectedField == "studregfield6")
            {
                return student.StudRegField6;
            }
            if (selectedField == "studregfield7")
            {
                return student.StudRegField7;
            }
            if (selectedField == "studregfield8")
            {
                return student.StudRegField8;
            }
            if (selectedField == "studregfield9")
            {
                return student.StudRegField9;
            }
            if (selectedField == "studregfield10")
            {
                return student.StudRegField10;
            }
            if (selectedField == "studregfield11")
            {
                return student.StudRegField11;
            }
            if (selectedField == "studregfield12")
            {
                return student.StudRegField12;
            }
            if (selectedField == "studregfield13")
            {
                return student.StudRegField13;
            }
            if (selectedField == "studregfield14")
            {
                return student.StudRegField14;
            }
            if (selectedField == "studregfield15")
            {
                return student.StudRegField15;
            }
            if (selectedField == "studregfield16")
            {
                return student.StudRegField16;
            }
            if (selectedField == "studregfield17")
            {
                return student.StudRegField17;
            }
            if (selectedField == "studregfield18")
            {
                return student.StudRegField18;
            }
            if (selectedField == "studregfield19")
            {
                return student.StudRegField19;
            }
            if (selectedField == "studregfield20")
            {
                return student.StudRegField20;
            }

            return "";
        }

        private string GetSettingsValue(string settingsJson, string fieldName)
        {
            string settingValue = "0"; //init to zero to avoid errors
            try
            {
                JavaScriptSerializer javascriptSerialize = new JavaScriptSerializer();
                dynamic settingsconfig = javascriptSerialize.Deserialize(settingsJson, typeof(object));
                if (settingsconfig.ContainsKey(fieldName))
                {
                    settingValue = settingsconfig[fieldName];
                }
                return settingValue;
            }
            catch
            {
                return settingValue;
            }
        }

        private Paragraph GetSetStudCustomField1Values(PDF pdf, Paragraph CustomFieldParagraph, string value, int studcustomfield1fontsize, string studcustomfield1labelstyle, string studcustomfield1alignment)
        {
            Paragraph studCustomField1 = new Paragraph();
            PDFjet.NET.Font font;

            if (studcustomfield1labelstyle == "0")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            }
            else if (studcustomfield1labelstyle == "1")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            }
            else if (studcustomfield1labelstyle == "2")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_OBLIQUE);
            }
            else
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD_OBLIQUE);
            }
            font.SetSize(studcustomfield1fontsize);
            CustomFieldParagraph.Add(new TextLine(font, value == null ? "" : value));
            CustomFieldParagraph.SetAlignment(studcustomfield1alignment == "0" ? Align.CENTER : studcustomfield1alignment == "1" ? Align.LEFT : studcustomfield1alignment == "2" ? Align.RIGHT : Align.CENTER);
            return CustomFieldParagraph;
        }
        private Paragraph GetSetStudCustomField2Values(PDF pdf, Paragraph CustomFieldParagraph, string value, int studcustomfield2fontsize, string studcustomfield2labelstyle, string studcustomfield2alignment)
        {
            Paragraph studCustomField2 = new Paragraph();
            PDFjet.NET.Font font;

            if (studcustomfield2labelstyle == "0")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            }
            else if (studcustomfield2labelstyle == "1")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            }
            else if (studcustomfield2labelstyle == "2")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_OBLIQUE);
            }
            else
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD_OBLIQUE);
            }
            font.SetSize(studcustomfield2fontsize);
            CustomFieldParagraph.Add(new TextLine(font, value == null ? "" : value));
            CustomFieldParagraph.SetAlignment(studcustomfield2alignment == "0" ? Align.CENTER : studcustomfield2alignment == "1" ? Align.LEFT : studcustomfield2alignment == "2" ? Align.RIGHT : Align.CENTER);
            return CustomFieldParagraph;
        }
        private Paragraph GetSetStudCustomField3Values(PDF pdf, Paragraph CustomFieldParagraph, string value, int studcustomfield3fontsize, string studcustomfield3labelstyle, string studcustomfield3alignment)
        {
            Paragraph studCustomField3 = new Paragraph();
            PDFjet.NET.Font font;

            if (studcustomfield3labelstyle == "0")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            }
            else if (studcustomfield3labelstyle == "1")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            }
            else if (studcustomfield3labelstyle == "2")
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_OBLIQUE);
            }
            else
            {
                font = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD_OBLIQUE);
            }
            font.SetSize(studcustomfield3fontsize);
            CustomFieldParagraph.Add(new TextLine(font, value == null ? "" : value));
            CustomFieldParagraph.SetAlignment(studcustomfield3alignment == "0" ? Align.CENTER : studcustomfield3alignment == "1" ? Align.LEFT : studcustomfield3alignment == "2" ? Align.RIGHT : Align.CENTER);
            return CustomFieldParagraph;
        }

        public BufferedStream BufferedStream
        {
            get;
            set;
        }

        public FileStream FileStream
        {
            get;
            set;
        }

        public entities.customcetificate CustomCertificate
        {
            get;
            set;
        }

        public string PdfOutFile
        {
            get;
            set;
        }

        public string PdfFileName
        {
            get;
            set;
        }

        public entities.Course Course
        {
            get;
            set;
        }

        public entities.PDFHeaderFooterInfo PdfHeaderFooterInfo
        {
            get;
            set;
        }
        public entities.Course_Roster CourseRoster
        {
            get;
            set;
        }

        public entities.Transcript Transciprt
        {
            get;
            set;
        }

        public List<entities.Instructor> CourseInstructor
        {
            get;
            set;
        }

        public int CourseCertificate
        {
            get;
            set;
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        public PdfGradeCertificate(int CertificateID)
        {
            db = new entities.SchoolEntities();
            CustomCertificate = (from cc in db.customcetificates where cc.customcertid == CertificateID select cc).FirstOrDefault();
            PdfHeaderFooterInfo = (from pdfinfo in db.PDFHeaderFooterInfoes select pdfinfo).FirstOrDefault();
            PdfFileName = "Certification_" + Guid.NewGuid() + ".pdf";
            var file = System.Web.HttpContext.Current.Server.MapPath("~/Temp/") + PdfFileName;
            PdfOutFile = file;

        }
        public void ExecuteCertification()
        {
            var settings = Settings.Instance;
            var mi1 = settings.GetMasterInfo();
            var mi2 = settings.GetMasterInfo2();
            var mi3 = settings.GetMasterInfo3();
            var mi4 = settings.GetMasterInfo4();



            var certtitle = CustomCertificate.certtitle;
            var certheight = Double.Parse(CustomCertificate.certheight.ToString());
            var certwidth = Double.Parse(CustomCertificate.certwidth.ToString());
            var marginTop = Double.Parse(CustomCertificate.marginTop.Value.ToString());
            var marginBottom = Double.Parse(CustomCertificate.marginBottom.Value.ToString());
            var marginLeft = Double.Parse(CustomCertificate.marginLeft.Value.ToString());
            var marginRight = Double.Parse(CustomCertificate.marginRight.Value.ToString());
            var boundX = CustomCertificate.boundX;
            var boundY = CustomCertificate.boundY;
            var useCourseNum = CustomCertificate.useCourseNum;
            var useLocation = CustomCertificate.useLocation;
            var backgroundimage = CustomCertificate.backgroundimage;
            var locheader1 = CustomCertificate.locheader1;
            var useHeader1 = CustomCertificate.useHeader1;
            var fontsizeHeader1 = CustomCertificate.fontsizeHeader1;
            var locheader2 = CustomCertificate.locheader2;
            var useHeader2 = CustomCertificate.useHeader2;
            var fontsizeHeader2 = CustomCertificate.fontsizeHeader2;
            //textInfo1 = CustomCertificate.textInfo1; //DEPRECATED
            var locinfo1 = CustomCertificate.locinfo1; //DEPRECATED (not commented out for backward compatability)
            //useInfo1 = CustomCertificate.useInfo1; //DEPRECATED
            //fontsizeInfo1 = CustomCertificate.fontsizeInfo1; //DEPRECATED
            //textInfo2 = CustomCertificate.textInfo2; //DEPRECATED
            var locinfo2 = CustomCertificate.locinfo2; //DEPRECATED (not commented out for backward compatability)
            //useInfo2 = CustomCertificate.useInfo2; //DEPRECATED
            //fontsizeInfo2 = CustomCertificate.fontsizeInfo2; //DEPRECATED
            var locname1 = CustomCertificate.locname1; //DEPRECATED (not commented out for backward compatability)
            var locname2 = CustomCertificate.locname2;
            var useExtraName = CustomCertificate.useExtraName;
            //alignsname = CustomCertificate.alignsname; //DEPRECATED
            //textCourseinfo1 = CustomCertificate.textCourseinfo1; //DEPRECATED
            var loccourseinfo1 = CustomCertificate.loccourseinfo1; //DEPRECATED (not commented out for backward compatability)
            //useCInfo1 = CustomCertificate.useCInfo1; //DEPRECATED
            //fontsizeCInfo1 = CustomCertificate.fontsizeCInfo1; //DEPRECATED
            var loccourseinfo2 = CustomCertificate.loccourseinfo2; //DEPRECATED (not commented out for backward compatability)
            //useCInfo2= rsCustom("useCInfo2; //DEPRECATED
            //fontsizeCInfo2 = CustomCertificate.fontsizeCInfo2; //DEPRECATED
            //loccoursedate1 = CustomCertificate.loccoursedate1; //DEPRECATED
            var loccoursedate2 = CustomCertificate.loccoursedate2;
            var locInstructor = CustomCertificate.locInstructor;
            var useInstructor = CustomCertificate.useInstructor;
            var fontsizeInstructor = CustomCertificate.fontsizeInstructor;
            var locsig1 = CustomCertificate.locsig1;
            var useSig1 = CustomCertificate.useSig1;
            var locsig2 = CustomCertificate.locsig2;
            var useSig2 = CustomCertificate.useSig2;
            var locfooter = CustomCertificate.locfooter;
            var useFooter = CustomCertificate.useFooter;

            var codefile = CustomCertificate.codefile;
            var useUploadedLogo = CustomCertificate.useUploadedLogo;
            var dateadded = CustomCertificate.dateadded;
            var disabled = CustomCertificate.disabled;
            var locstudfield1 = CustomCertificate.locstudfield1;
            var usestudfield1 = CustomCertificate.usestudfield1;
            var fontsizestudfield1 = CustomCertificate.fontsizestudfield1;
            var selectedstudfield1 = CustomCertificate.selectedstudfield1;
            var locstudfield2 = CustomCertificate.locstudfield2;
            var usestudfield2 = CustomCertificate.usestudfield2;
            var fontsizestudfield2 = CustomCertificate.fontsizestudfield2;
            var selectedstudfield2 = CustomCertificate.selectedstudfield2;
            var locstudfield3 = CustomCertificate.locstudfield3;
            var usestudfield3 = CustomCertificate.usestudfield3;
            var fontsizestudfield3 = CustomCertificate.fontsizestudfield3;
            var selectedstudfield3 = CustomCertificate.selectedstudfield3;
            //loccredits = CustomCertificate.loccredits; //DEPRECATED
            //usecredits = CustomCertificate.usecredits; //DEPRECATED
            //fontsizecredits = CustomCertificate.fontsizecredits; //DEPRECATED
            var loccourseloc = CustomCertificate.loccourseloc;
            var usecourseloc = CustomCertificate.usecourseloc;
            var fontsizecourseloc = CustomCertificate.fontsizecourseloc;
            var loccoursenumber = CustomCertificate.loccoursenumber;
            var usecoursenumber = CustomCertificate.usecoursenumber;
            var fontsizecoursenumber = CustomCertificate.fontsizecoursenumber;
            var loccourseicon = CustomCertificate.loccourseicon;
            var usecourseicon = CustomCertificate.usecourseicon;
            var fontsizecourseicon = CustomCertificate.fontsizecourseicon;
            var loccoursedesc = CustomCertificate.loccoursedesc;
            var usecoursedesc = CustomCertificate.usecoursedesc;
            var fontsizecoursedesc = CustomCertificate.fontsizecoursedesc;
            //usecoursedate = CustomCertificate.usecoursedate;
            //fontsizecoursedate = CustomCertificate.fontsizecoursedate; //DEPRECATED
            var useCourseDuration = CustomCertificate.useCourseDuration;
            //fontsizestudname = CustomCertificate.fontsizestudname; //DEPRECATED
            var showcoursetime = CustomCertificate.showcoursetime;
            var showcourseenddate = CustomCertificate.showcourseenddate;
            var showcourseenddateonly = CustomCertificate.showcourseenddateonly;
            var useLicense = CustomCertificate.useLicense;
            var fontsizelicense = CustomCertificate.fontsizelicense;
            var loccourselicense = CustomCertificate.loccourselicense;
            var surveyusecompdate = CustomCertificate.surveyusecompdate;
            var uselicnumber = CustomCertificate.uselicnumber;
            var loclicnumber = CustomCertificate.loclicnumber;
            var fontsizelicnumber = CustomCertificate.fontsizelicnumber;
            var useDatePrint = CustomCertificate.useDatePrint;
            var locDatePrint = CustomCertificate.locDatePrint;
            var fontsizeDateprint = CustomCertificate.fontsizeDateprint;
            var useStudentAddress = CustomCertificate.useStudentAddress;
            var locStudentAddress = CustomCertificate.locStudentAddress;
            var fontsizeStudentAddress = CustomCertificate.fontsizeStudentAddress;
            var signatureDisplay = CustomCertificate.signatureDisplay;
            var usecustomcredit = CustomCertificate.usecustomcredit;
            var loccustomcredit = CustomCertificate.loccustomcredit1;
            var usestudentid = CustomCertificate.usestudentid;
            var locstudentid = CustomCertificate.locstudentid;
            var fontsizestudid = CustomCertificate.fontsizestudid;

            //begin new json certificate fields
            //dates
            var startdateSettings = CustomCertificate.startdateSettings;
            var enddateSettings = CustomCertificate.enddateSettings;
            var daterangeSettings = CustomCertificate.daterangeSettings;
            var datelistSettings = CustomCertificate.datelistSettings;
            var rosterdateaddedSettings = CustomCertificate.rosterdateaddedSettings;
            //credits
            var credithoursSettings = CustomCertificate.credithoursSettings;
            var inservicehoursSettings = CustomCertificate.inservicehoursSettings;
            var customcredittypeSettings = CustomCertificate.customcredittypeSettings;
            var ceucreditSettings = CustomCertificate.ceucreditSettings;
            var graduatecreditSettings = CustomCertificate.graduatecreditSettings;
            //text
            var customtextSettings = CustomCertificate.customtextSettings;
            var studentnameSettings = CustomCertificate.studentnameSettings;
            var coursenameSettings = CustomCertificate.coursenameSettings;
            var courseaddtextSettings = CustomCertificate.courseaddtextSettings;
            var optionaltext1Settings = CustomCertificate.optionaltext1Settings;
            var optionaltext2Settings = CustomCertificate.optionaltext2Settings;
            var globalsettings = CustomCertificate.miscglobalsettings;

            //alignment, font weight, font style
            var studcustomfield1settings = CustomCertificate.StudCustomField1Settings;
            var studcustomfield2settings = CustomCertificate.StudCustomField2Settings;
            var studcustomfield3settings = CustomCertificate.StudCustomField3Settings;

            string studcustomfield1labelstyle = string.Empty;
            string studcustomfield1alignment = string.Empty;

            string studcustomfield2labelstyle = string.Empty;
            string studcustomfield2alignment = string.Empty;

            string studcustomfield3labelstyle = string.Empty;
            string studcustomfield3alignment = string.Empty;

            if (string.IsNullOrEmpty(studcustomfield1settings))
            {
                studcustomfield1settings = settings.GetFieldValueFromDbByQuery("StudCustomField1Settings", "customcetificate", "where customcertid=" + CourseCertificate);
            }
            if (string.IsNullOrEmpty(studcustomfield2settings))
            {
                studcustomfield2settings = settings.GetFieldValueFromDbByQuery("StudCustomField2Settings", "customcetificate", "where customcertid=" + CourseCertificate);
            }
            if (string.IsNullOrEmpty(studcustomfield3settings))
            {
                studcustomfield3settings = settings.GetFieldValueFromDbByQuery("StudCustomField3Settings", "customcetificate", "where customcertid=" + CourseCertificate);
            }

            if (!string.IsNullOrEmpty(studcustomfield1settings))
            {
                studcustomfield1labelstyle = GetSettingsValue(studcustomfield1settings, "labelstyle");
                studcustomfield1alignment = GetSettingsValue(studcustomfield1settings, "alignment");
            }
            if (!string.IsNullOrEmpty(studcustomfield2settings))
            {
                studcustomfield2labelstyle = GetSettingsValue(studcustomfield2settings, "labelstyle");
                studcustomfield2alignment = GetSettingsValue(studcustomfield2settings, "alignment");
            }
            if (!string.IsNullOrEmpty(studcustomfield3settings))
            {
                studcustomfield3labelstyle = GetSettingsValue(studcustomfield3settings, "labelstyle");
                studcustomfield3alignment = GetSettingsValue(studcustomfield3settings, "alignment");
            }

            //end 

            string hidezerocredits = "0";
            try
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                dynamic settingsconfig = j.Deserialize(globalsettings, typeof(object));
                if (settingsconfig.ContainsKey("hidezerocredits"))
                {
                    hidezerocredits = settingsconfig["hidezerocredits"];
                }
            }
            catch { }




            FileStream = new FileStream(PdfOutFile, FileMode.Create);
            BufferedStream = new BufferedStream(FileStream);
            var bs = BufferedStream;
            PDF pdf = new PDF(bs);
            pdf.SetTitle("Certificate Custom");
            pdf.SetSubject("Certificate");
            pdf.SetAuthor("GoSignMeUp!");

            Page page;
            string orientaion = "1";
            if (certwidth > certheight)
            {
                page = new Page(pdf, Letter.LANDSCAPE);
            }
            else
            {
                page = new Page(pdf, Letter.PORTRAIT);
            }

            PDFjet.NET.Font f0 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            PDFjet.NET.Font f1 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            PDFjet.NET.Font f2 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            PDFjet.NET.Font f3 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            PDFjet.NET.Font f4 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);

            f1.SetSize(11);
            f2.SetSize(11);
            f3.SetSize(23);
            f4.SetSize(18);
            f0.SetSize(10);

            if (backgroundimage != null)
            {
                string fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/images/pdfimages/" + backgroundimage.ToString());
                if (File.Exists(fileName))
                {
                    string extension = System.IO.Path.GetExtension(fileName).ToLower();
                    int imagetype = ImageType.JPG;

                    if (extension.ToLower() == ".jpg")
                    {
                        imagetype = ImageType.JPG;

                    }
                    else if (extension.ToLower() == ".bmp")
                    {
                        imagetype = ImageType.BMP;

                    }
                    else if (extension.ToLower() == ".png")
                    {
                        imagetype = ImageType.PNG;

                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        imagetype = ImageType.PDF;

                    }
                    if ((extension.ToLower() != ".gif") && (extension.ToLower() != ".gif"))
                    {
                        FileStream filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        PDFjet.NET.Image bckgroundimage = new PDFjet.NET.Image(pdf, filestream, imagetype);
                        float originalwidth = bckgroundimage.GetWidth();
                        float originalheight = bckgroundimage.GetHeight();
                        bckgroundimage.SetPosition(18, 18);

                        //Compute the Height and width factor of the image.
                        float widthfactor = float.Parse((certwidth - (marginLeft + marginRight)).ToString()) / originalwidth;
                        float heightfactor = float.Parse((certheight - (marginTop + marginBottom)).ToString()) / originalheight;
                        //end

                        bckgroundimage.ScaleBy(widthfactor, heightfactor);
                        bckgroundimage.DrawOn(page);
                        filestream.Close();

                    }
                }
            }

            int verticalpadding = 0;
            int horizontalpadding = 0;

            var student = Gsmu.Api.Data.School.Entities.Student.GetStudent(AuthorizationHelper.CurrentStudentUser.STUDENTID);

            #region student data printing section
            for (int counter = 0; counter <= 13; counter++)
            {
                int intDsiplay = 0;
                int fontsize = 12;
                string strAlllocation = "";
                string textvalue = "";


                if (counter == 0) //Date Print
                {
                    intDsiplay = useDatePrint.Value;
                    strAlllocation = locDatePrint;
                    textvalue = DateTime.Now.ToShortDateString();
                    fontsize = fontsizeDateprint.Value;
                }


                if (counter == 3) //Student Custom Field 1
                {
                    intDsiplay = usestudfield1.Value;
                    strAlllocation = locstudfield1;
                    textvalue = GetCustomFieldValue(selectedstudfield1, student);
                    fontsize = fontsizestudfield1.Value;
                }
                if (counter == 4) // Header 1
                {
                    intDsiplay = useHeader1.Value;
                    strAlllocation = locheader1;
                    textvalue = PdfHeaderFooterInfo.Header1;
                    fontsize = fontsizeHeader1.Value;
                }
                if (counter == 5)
                {
                    intDsiplay = useHeader2.Value;
                    strAlllocation = locheader2;
                    textvalue = PdfHeaderFooterInfo.Header2;
                    fontsize = fontsizeHeader2.Value;
                }
                if (counter == 6)
                {
                    intDsiplay = useFooter.Value;
                    strAlllocation = locfooter;
                    textvalue = settings.GetMasterInfo3().CertificateFooterText;
                    fontsize = 8;
                }
                if (counter == 7)
                {
                    intDsiplay = usestudentid.Value;
                    strAlllocation = locstudentid;
                    textvalue = student.STUDENTID.ToString();
                    fontsize = fontsizestudid.Value;
                }
                if (counter == 8) //Student Custom Field 2
                {
                    intDsiplay = usestudfield2.Value;
                    strAlllocation = locstudfield2;
                    textvalue = GetCustomFieldValue(selectedstudfield2, student);
                    fontsize = fontsizestudfield2.Value;
                }
                if (counter == 9) //Student Custom Field 3
                {
                    intDsiplay = usestudfield3.Value;
                    strAlllocation = locstudfield3;
                    textvalue = GetCustomFieldValue(selectedstudfield3, student);
                    fontsize = fontsizestudfield3.Value;

                }
                if (counter == 10)
                {
                    intDsiplay = useStudentAddress.Value;
                    strAlllocation = locStudentAddress;
                    if ((student.ADDRESS != "") || (student.ADDRESS != null))
                    {
                        textvalue = textvalue + student.ADDRESS + ",";
                    }
                    if ((student.CITY != "") || (student.CITY != null))
                    {
                        textvalue = textvalue + student.CITY + ",";
                    }
                    if ((student.STATE != "") || (student.STATE != null))
                    {
                        textvalue = textvalue + student.STATE + ",";
                    }
                    if ((student.ZIP != "") || (student.ZIP != null))
                    {
                        textvalue = textvalue + student.ZIP + ",";
                    }
                    textvalue = textvalue.TrimEnd(',').TrimStart(',').Replace(",", ", ");

                    fontsize = fontsizestudfield3.Value;
                }


                if (intDsiplay == 1)
                {
                    double x = 0;
                    double y = 0;
                    double w = 0;
                    double h = 0;
                    string[] location = strAlllocation.Split(',');
                    try
                    {
                        x = double.Parse(location[0]);
                        y = double.Parse(location[1]);
                        w = double.Parse(location[2]);
                        h = double.Parse(location[3]);
                    }
                    catch
                    {
                        x = 0;
                        y = 0;
                        w = 0;
                        h = 0;
                    }
                    finally
                    {
                    }
                    f3.SetSize(fontsize);
                    Paragraph paragraph = new Paragraph();

                    if (counter == 3)
                    {
                        paragraph = GetSetStudCustomField1Values(pdf, paragraph, textvalue, fontsize, studcustomfield1labelstyle, studcustomfield1alignment);
                    }
                    else if (counter == 8)
                    {
                        paragraph = GetSetStudCustomField2Values(pdf, paragraph, textvalue, fontsize, studcustomfield2labelstyle, studcustomfield2alignment);
                    }
                    else if (counter == 9)
                    {
                        paragraph = GetSetStudCustomField3Values(pdf, paragraph, textvalue, fontsize, studcustomfield3labelstyle, studcustomfield3alignment);
                    }
                    else
                    {
                        paragraph.Add(new TextLine(f3, textvalue == null ? "" : textvalue));
                        paragraph.SetAlignment(Align.CENTER);
                    }

                    TextColumn columncustom = new TextColumn(f1);
                    columncustom.SetLineBetweenParagraphs(true);
                    columncustom.SetSpaceBetweenLines(5);

                    columncustom.AddParagraph(paragraph);
                    columncustom.SetPosition(x + marginTop, y + marginLeft);
                    columncustom.SetSize(w, h);
                    PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                    columncustom.SetSize(certheight, certwidth);
                    columncustom.SetLocation(18, 18);
                }
            }
            #endregion
            #region course info, and course related info section
            for (int jconfigcount = 0; jconfigcount <= 20; jconfigcount++)
            {
                var textconfig = "";
                string textvalue = "";
                string creditlabel = "";
                if (jconfigcount == 9)
                {
                    textconfig = CustomCertificate.customtextSettings;
                    textvalue = ""; //No value from Survey module
                }
                if (jconfigcount == 11)
                {
                    textconfig = CustomCertificate.studentnameSettings;

                    textvalue = student.FIRST + " " + student.LAST;
                }

                if (jconfigcount == 14)
                {
                    textconfig = CustomCertificate.optionaltext1Settings;
                }
                if (jconfigcount == 15)
                {
                    textconfig = CustomCertificate.optionaltext2Settings;
                }

                if ((textconfig != "") && (textconfig != null))
                {
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    dynamic settingsconfig = j.Deserialize(textconfig, typeof(object));
                    string usefield = settingsconfig["usefield"];
                    if (textvalue == "")
                    {
                        if (settingsconfig.ContainsKey("text"))
                        {
                            textvalue = settingsconfig["text"];
                        }
                    }
                    double fontsize = 12;
                    double x = 0;
                    double y = 0;
                    double w = 0;
                    double h = 0;
                    string align = "0";
                    try
                    {
                        if (settingsconfig.ContainsKey("fontsize"))
                        {
                            fontsize = double.Parse(settingsconfig["fontsize"]);
                        }
                        if (settingsconfig.ContainsKey("x"))
                        {
                            x = double.Parse(settingsconfig["x"]);
                        }
                        if (settingsconfig.ContainsKey("y"))
                        {
                            y = double.Parse(settingsconfig["y"]);
                        }
                        if (settingsconfig.ContainsKey("w"))
                        {
                            w = double.Parse(settingsconfig["w"]);
                        }
                        if (settingsconfig.ContainsKey("h"))
                        {
                            h = double.Parse(settingsconfig["h"]);
                        }
                        if (settingsconfig.ContainsKey("alignment"))
                        {
                            align = settingsconfig["alignment"];
                        }

                        if (settingsconfig.ContainsKey("labelstyle"))
                        {
                            string style = settingsconfig["labelstyle"];
                            if (style != "")
                            {
                                //textvalue = GetCertificateTextFormat(style, textvalue, creditlabel,hidezerocredits);
                                textvalue = GetCertificateTextFormat(style, textvalue, creditlabel, hidezerocredits);
                            }

                        }


                    }
                    catch
                    {
                        align = "center";
                    }
                    finally
                    { }

                    if (usefield != "0")
                    {
                        Paragraph paragraph = new Paragraph();
                        if (align == "1")
                        {
                            paragraph.SetAlignment(Align.LEFT);
                        }
                        else if (align == "2")
                        {
                            paragraph.SetAlignment(Align.RIGHT);
                        }
                        else
                        {
                            paragraph.SetAlignment(Align.CENTER);
                        }
                        f3.SetSize(fontsize);
                        paragraph.Add(new TextLine(f3, textvalue == null ? "" : textvalue));

                        TextColumn columncustom = new TextColumn(f1);
                        columncustom.SetLineBetweenParagraphs(true);
                        columncustom.SetSpaceBetweenLines(5);

                        columncustom.AddParagraph(paragraph);
                        columncustom.SetPosition(x + marginTop, y + marginLeft);
                        columncustom.SetSize(w, h);
                        PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                        columncustom.SetSize(certheight, certwidth);
                        columncustom.SetLocation(18, 18);
                    }
                }
            }
            #endregion
            pdf.Flush();
            bs.Close();
        }
    }
}
