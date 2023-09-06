using Gsmu.Api.Data;
using Gsmu.Api.Data.School.CustomTranscriptModel;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Transcripts;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using entities = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Export.StudentTranscript
{
    public class PdfStudentTranscript : IDisposable
    {
        private entities.SchoolEntities db = null;
        public PdfStudentTranscript(int sid, string startdate, string enddate, string caller)
        {
            DateTime start = DateTime.Now.AddYears(-1);
            DateTime end = DateTime.Now.AddDays(2);
            if (!(DateTime.TryParse(startdate, out start)))
            {
                start = DateTime.Now.AddYears(-1);
            }
            if (!(DateTime.TryParse(enddate, out end)))
            {
                end = DateTime.Now.AddDays(2);
            }
            _start = start;
            _end = end;
            db = new entities.SchoolEntities();
            int assignedtranscript = 0;
            System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
            dynamic globalconfiguration = JSSerializeObj.Deserialize(Settings.Instance.GetMasterInfo4().customcreditsettings, typeof(object));
            try
            {
                if (caller != "usercourses")
                {
                    assignedtranscript = int.Parse(globalconfiguration["assignedtranscript"]);
                    DefaultPdfTranscript = (from cc in db.customtranscripts where cc.customtranid == assignedtranscript select cc).FirstOrDefault();
                }
                else
                {
                    DefaultPdfTranscript = (from cc in db.customtranscripts where cc.isdefaultselected == 1 select cc).FirstOrDefault();
                }
            }
            catch {
                DefaultPdfTranscript = (from cc in db.customtranscripts where cc.isdefaultselected == 1 select cc).FirstOrDefault();
            }
            
            studentid = sid ;
            Random random = new Random();
            var file = System.Web.HttpContext.Current.Server.MapPath("~/Temp/") +sid.ToString()+random.Next(0,1000)+ "_CustomTranscript.pdf";
            PdfOutFile = file;
            RequestCaller = caller;
        }

        public void Execute()
        {

            FileStream = new FileStream(PdfOutFile, FileMode.Create);
            BufferedStream = new BufferedStream(FileStream);
            var bs = BufferedStream;
            PDF pdf = new PDF(bs);
            pdf.SetTitle("Custom Transcript");
            pdf.SetSubject("Certificate");
            pdf.SetAuthor("GoSignMeUp!");
            var student = Gsmu.Api.Data.School.Entities.Student.GetStudent(studentid);
            Page page = BuildHeader(pdf, bs, student);
            double starty = table_x;
           int colheight = 12;
           int colindex = 0;
           int rowindex = 0;
           int tableheight = 0;
           int header_height = 0;
           int limit_height = 0;
           int text_height = 12;
           var SelectedFields = DefaultPdfTranscript.coursefieldjsonsettings;
           Font font_text = new Font(pdf, CoreFont.HELVETICA);
           PDFjet.NET.Font f0 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
           f0.SetSize(8);
           PDFjet.NET.Font fTotal = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
           f0.SetSize(8);

           Transcripts transcripts = new Transcripts();
           List<Transcript> transcriptsList = transcripts.StudentTranscriptedCourse(student.STUDENTID,_start,_end);
           List<Cell>  courserow = new List<Cell>();
           List<CustomTranscriptCourseFields> ListAllfields = ProcessSelectedFields(SelectedFields);
           List<List<Cell>> coursedata = new List<List<Cell>>();
           int counter = 0;
           double yindex= table_y;
           double customcredithoursTotal = 0;
           double credithoursTotal = 0;
           double InServiceTotal = 0;
           double sbceuTotal = 0;
           double GraduateTotal = 0;
           double och1Total = 0;
           double och2Total = 0;
           double och3Total = 0;
           double och4Total = 0;
           double och5Total = 0;
           double och6Total = 0;
           double och7Total = 0;
           double och8Total = 0;

           bool showcustomcredithoursTotal = false;
           bool showcredithoursTotal = false;
           bool showInServiceTotal = false;
           bool showsbceuTotal = false;
           bool showGraduateTotal = false;
           bool showoch1Total = false;
           bool showoch2Total = false;
           bool showoch3Total = false;
           bool showoch4Total = false;
           bool showoch5Total = false;
           bool showoch6Total = false;
           bool showoch7Total = false;
           bool showoch8Total = false;
           Course c = new Course();

           var coursefrriddefaultsortedfield = "startdate";
           if (DefaultPdfTranscript.coursegridconfiguration != null && DefaultPdfTranscript.coursegridconfiguration != "")
           {
               coursefrriddefaultsortedfield = GetGridDefaultFieldSorting(DefaultPdfTranscript.coursegridconfiguration);
           }
           transcriptsList = SortTranscriptGrid(transcriptsList, coursefrriddefaultsortedfield);
           foreach (Transcript data in transcriptsList)
           {
               if (RequestCaller != "usercourses")
               {
                   if (data.IsHoursPaid == 1)
                   {
                       courserow = new List<Cell>();
                       c = GetDataForTranscript(data.CourseId.Value);
                       foreach (var field in ListAllfields)
                       {
                           if (field.id == "coursename")
                           {
                               Cell fieldx = new Cell(f0, data.CourseName);
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "coursenum")
                           {
                               Cell fieldx = new Cell(f0, data.CourseNum);
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "credithours")
                           {
                               Cell fieldx = new Cell(f0, data.CreditHours.ToString());
                               credithoursTotal = credithoursTotal + (data.CreditHours.HasValue ? data.CreditHours.Value : 0);
                               showcredithoursTotal = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "InService")
                           {
                               Cell fieldx = new Cell(f0, data.InserviceHours.ToString());
                               InServiceTotal = InServiceTotal + data.InserviceHours;
                               showInServiceTotal = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "customcredit")
                           {
                               try
                               {
                                   JavaScriptSerializer j = new JavaScriptSerializer();
                                   dynamic configuration = j.Deserialize(c.CourseConfiguration, typeof(object));
                                   var allowcreditpurchase = configuration["purchasecredit"];
                                   if (c.CourseConfiguration != null)
                                   {
                                       if (allowcreditpurchase == 1)
                                       {
                                           if (data.IsHoursPaid == 1)
                                           {
                                               Cell fieldx = new Cell(f0, data.CustomCreditHours.ToString());
                                               customcredithoursTotal = customcredithoursTotal + (data.CustomCreditHours.HasValue ? data.CustomCreditHours.Value : 0);
                                               showcustomcredithoursTotal = true;
                                               courserow.Add(fieldx);
                                           }
                                       }
                                       else
                                       {
                                           Cell fieldx = new Cell(f0, data.CustomCreditHours.ToString());
                                           customcredithoursTotal = customcredithoursTotal + (data.CustomCreditHours.HasValue ? data.CustomCreditHours.Value : 0);
                                           showcustomcredithoursTotal = true;
                                           courserow.Add(fieldx);
                                       }
                                   }
                               }
                               catch
                               {

                                       Cell fieldx = new Cell(f0, data.CustomCreditHours.ToString());
                                       customcredithoursTotal = customcredithoursTotal + (data.CustomCreditHours.HasValue ? data.CustomCreditHours.Value : 0);
                                       showcustomcredithoursTotal = true;
                                       courserow.Add(fieldx);
                                   
                               }

                           }
                           else if (field.id == "sbceu")
                           {
                               Cell fieldx = new Cell(f0, data.ceucredit.ToString());
                               sbceuTotal = sbceuTotal + (data.ceucredit.HasValue ? data.ceucredit.Value : 0);
                               showsbceuTotal = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "Graduate")
                           {
                               Cell fieldx = new Cell(f0, data.graduatecredit.ToString());
                               GraduateTotal = GraduateTotal + (data.graduatecredit.HasValue ? data.graduatecredit.Value : 0);
                               showGraduateTotal = true;
                               courserow.Add(fieldx);
                           }


                           else if (field.id == "och1")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours1.ToString());
                               och1Total = och1Total + (data.Optionalcredithours1.HasValue ? data.Optionalcredithours1.Value : 0);
                               showoch1Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och2")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours2.ToString());
                               och2Total = och2Total + (data.Optionalcredithours2.HasValue ? data.Optionalcredithours2.Value : 0);
                               showoch2Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och3")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours3.ToString());
                               och3Total = och3Total + (data.Optionalcredithours3.HasValue ? data.Optionalcredithours3.Value : 0);
                               showoch3Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och4")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours4.ToString());
                               och4Total = och4Total + (data.Optionalcredithours4.HasValue ? data.Optionalcredithours4.Value : 0);
                               showoch4Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och5")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours5.ToString());
                               och5Total = och5Total + (data.Optionalcredithours5.HasValue ? data.Optionalcredithours5.Value : 0);
                               showoch5Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och6")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours6.ToString());
                               och6Total = och6Total + (data.Optionalcredithours6.HasValue ? data.Optionalcredithours6.Value : 0);
                               showoch6Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och7")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours7.ToString());
                               och7Total = och7Total + (data.Optionalcredithours7.HasValue ? data.Optionalcredithours7.Value : 0);
                               showoch7Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "och8")
                           {
                               Cell fieldx = new Cell(f0, data.Optionalcredithours8.ToString());
                               och8Total = och8Total + (data.Optionalcredithours8.HasValue ? data.Optionalcredithours8.Value : 0);
                               showoch8Total = true;
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "customfield1")
                           {
                               try
                               {
                                   Cell fieldx = new Cell(f0, c.CustomCourseField1.ToString());
                                   courserow.Add(fieldx);
                               }
                               catch
                               {
                                   Cell fieldx = new Cell(f0, "");
                                   courserow.Add(fieldx);
                               }
                           }
                           else if (field.id == "customfield2")
                           {
                               try
                               {
                                   Cell fieldx = new Cell(f0, c.CustomCourseField2.ToString());
                                   courserow.Add(fieldx);
                               }
                               catch {
                                   Cell fieldx = new Cell(f0, "");
                                   courserow.Add(fieldx);
                               }
                           }
                           else if (field.id == "customfield3")
                           {
                               try
                               {
                                   Cell fieldx = new Cell(f0, c.CustomCourseField3.ToString());
                                   courserow.Add(fieldx);
                               }
                               catch {
                                   Cell fieldx = new Cell(f0, "dealerss");
                                   courserow.Add(fieldx);
                               }
                           }
                           else if (field.id == "customfield4")
                           {
                               try
                               {
                                   Cell fieldx = new Cell(f0, c.CustomCourseField4.ToString());
                                   courserow.Add(fieldx);
                               }
                               catch {
                                   Cell fieldx = new Cell(f0, "");
                                   courserow.Add(fieldx);
                               }
                           }
                           else if (field.id == "customfield5")
                           {
                               try
                               {
                                   Cell fieldx = new Cell(f0, c.CustomCourseField5.ToString());
                                   courserow.Add(fieldx);
                               }
                               catch {
                                   Cell fieldx = new Cell(f0, "");
                                   courserow.Add(fieldx);
                               }
                           }
                           else if (field.id == "startdate")
                           {
                               Cell fieldx = new Cell(f0, GetStartDate(data.CourseId.Value));
                               courserow.Add(fieldx);
                           }
                           else if (field.id == "enddate")
                           {
                               Cell fieldx = new Cell(f0, GetEndDate(data.CourseId.Value));
                               courserow.Add(fieldx);
                           }
                       }

                       coursedata.Add(courserow);
                   }
               }
               else
               {
                   courserow = new List<Cell>();
                   c = GetDataForTranscript(data.CourseId.Value);
                   foreach (var field in ListAllfields)
                   {
                       if (field.id == "coursename")
                       {
                           Cell fieldx = new Cell(f0, data.CourseName);
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "coursenum")
                       {
                           Cell fieldx = new Cell(f0, data.CourseNum);
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "credithours")
                       {
                           Cell fieldx = new Cell(f0, data.CreditHours.ToString());
                           credithoursTotal = credithoursTotal + (data.CreditHours.HasValue ? data.CreditHours.Value : 0);
                           showcredithoursTotal = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "InService")
                       {
                           Cell fieldx = new Cell(f0, data.InserviceHours.ToString());
                           InServiceTotal = InServiceTotal + data.InserviceHours;
                           showInServiceTotal = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "customcredit")
                       {
                           try
                           {
                               JavaScriptSerializer j = new JavaScriptSerializer();
                               dynamic configuration = j.Deserialize(c.CourseConfiguration, typeof(object));
                               var allowcreditpurchase = configuration["purchasecredit"];
                               if (c.CourseConfiguration != null)
                               {
                                   if (allowcreditpurchase == "1")
                                   {
                                       if (data.IsHoursPaid == 1)
                                       {
                                           Cell fieldx = new Cell(f0, data.CustomCreditHours.ToString());
                                           customcredithoursTotal = customcredithoursTotal + (data.CustomCreditHours.HasValue ? data.CustomCreditHours.Value : 0);
                                           showcustomcredithoursTotal = true;
                                           courserow.Add(fieldx);
                                       }
                                       else
                                       {
                                           Cell fieldx = new Cell(f0,"0");
                                           showcustomcredithoursTotal = true;
                                           courserow.Add(fieldx);
                                       }
                                   }
                                   else
                                   {
                                       Cell fieldx = new Cell(f0, data.CustomCreditHours.ToString());
                                       customcredithoursTotal = customcredithoursTotal + (data.CustomCreditHours.HasValue ? data.CustomCreditHours.Value : 0);
                                       showcustomcredithoursTotal = true;
                                       courserow.Add(fieldx);
                                   }
                               }
                           }
                           catch
                           {

                               Cell fieldx = new Cell(f0, data.CustomCreditHours.ToString());
                               customcredithoursTotal = customcredithoursTotal + (data.CustomCreditHours.HasValue ? data.CustomCreditHours.Value : 0);
                               showcustomcredithoursTotal = true;
                               courserow.Add(fieldx);

                           }
                       }
                       else if (field.id == "sbceu")
                       {
                           Cell fieldx = new Cell(f0, data.ceucredit.ToString());
                           sbceuTotal = sbceuTotal + (data.ceucredit.HasValue ? data.ceucredit.Value : 0);
                           showsbceuTotal = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "Graduate")
                       {
                           Cell fieldx = new Cell(f0, data.graduatecredit.ToString());
                           GraduateTotal = GraduateTotal + (data.graduatecredit.HasValue ? data.graduatecredit.Value : 0);
                           showGraduateTotal = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och1")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours1.ToString());
                           och1Total = och1Total + (data.Optionalcredithours1.HasValue ? data.Optionalcredithours1.Value : 0);
                           showoch1Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och2")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours2.ToString());
                           och2Total = och2Total + (data.Optionalcredithours2.HasValue ? data.Optionalcredithours2.Value : 0);
                           showoch2Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och3")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours3.ToString());
                           och3Total = och3Total + (data.Optionalcredithours3.HasValue ? data.Optionalcredithours3.Value : 0);
                           showoch3Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och4")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours4.ToString());
                           och4Total = och4Total + (data.Optionalcredithours4.HasValue ? data.Optionalcredithours4.Value : 0);
                           showoch4Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och5")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours5.ToString());
                           och5Total = och5Total + (data.Optionalcredithours5.HasValue ? data.Optionalcredithours5.Value : 0);
                           showoch5Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och6")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours6.ToString());
                           och6Total = och6Total + (data.Optionalcredithours6.HasValue ? data.Optionalcredithours6.Value : 0);
                           showoch6Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och7")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours7.ToString());
                           och7Total = och7Total + (data.Optionalcredithours7.HasValue ? data.Optionalcredithours7.Value : 0);
                           showoch7Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "och8")
                       {
                           Cell fieldx = new Cell(f0, data.Optionalcredithours8.ToString());
                           och8Total = och8Total + (data.Optionalcredithours8.HasValue ? data.Optionalcredithours8.Value : 0);
                           showoch8Total = true;
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "customfield1")
                       {
                           try
                           {
                               Cell fieldx = new Cell(f0, c.CustomCourseField1.ToString());
                               courserow.Add(fieldx);
                           }
                           catch
                           {
                               Cell fieldx = new Cell(f0, "");
                               courserow.Add(fieldx);
                           }
                       }
                       else if (field.id == "customfield2")
                       {
                           try
                           {
                               Cell fieldx = new Cell(f0, c.CustomCourseField2.ToString());
                               courserow.Add(fieldx);
                           }
                           catch
                           {
                               Cell fieldx = new Cell(f0, "");
                               courserow.Add(fieldx);
                           }
                       }
                       else if (field.id == "customfield3")
                       {
                           try
                           {
                               Cell fieldx = new Cell(f0, c.CustomCourseField3.ToString());
                               courserow.Add(fieldx);
                           }
                           catch
                           {
                               Cell fieldx = new Cell(f0, "");
                               courserow.Add(fieldx);
                           }
                       }
                       else if (field.id == "customfield4")
                       {
                           try
                           {
                               Cell fieldx = new Cell(f0, c.CustomCourseField4.ToString());
                               courserow.Add(fieldx);
                           }
                           catch
                           {
                               Cell fieldx = new Cell(f0, "");
                               courserow.Add(fieldx);
                           }
                       }
                       else if (field.id == "customfield5")
                       {
                           try
                           {
                               Cell fieldx = new Cell(f0, c.CustomCourseField5.ToString());
                               courserow.Add(fieldx);
                           }
                           catch
                           {
                               Cell fieldx = new Cell(f0, "");
                               courserow.Add(fieldx);
                           }
                       }
                       else if (field.id == "startdate")
                       {
                           Cell fieldx = new Cell(f0, GetStartDate(data.CourseId.Value));
                           courserow.Add(fieldx);
                       }
                       else if (field.id == "enddate")
                       {
                           Cell fieldx = new Cell(f0, GetEndDate(data.CourseId.Value));
                           courserow.Add(fieldx);
                       }
                   }
                   coursedata.Add(courserow);
               }
           }
           foreach (var row in coursedata)
           {
               counter = 0;
               foreach (var col in row)
               {
                   int textcount = col.GetText().Count();
                   if (cnameindex == counter)
                   {
                       if ((((textcount) * (colounmcount)) >= ((ListIndexwithCustomWidth.Count * (cnamelenbuffer))+(tablelen / (colounmcount)) * 2)))
                       {
                           text_height = 25;
                       }
                   }
                   else
                   {
                       if ((((textcount) * colounmcount) >= (tablelen / (colounmcount))))
                       {
                           if (col.GetText().Contains(" "))
                           {
                               text_height = 25;
                           }
                       }
                   }
                   font_text = new Font(pdf, CoreFont.HELVETICA);
                   font_text.SetSize(10);
                   
                   header_height = text_height;
                   TextBox textbox = new TextBox(font_text);
                   if(cnameindex==counter){
                       textbox.SetWidth((ListIndexwithCustomWidth.Count*cnamelenbuffer) + (tablelen / colounmcount) * 2);
                   }
                   else{
                   textbox.SetWidth(tablelen / colounmcount);
                   }
                   textbox.SetText(col.GetText());
                  // textbox.SetHeight(text_height);
                
                   textbox.SetPosition(starty, yindex+20);
                   textbox.SetNoBorders();
                   textbox.SetSpacing(0);
                   textbox.DrawOn(page);
                   if (cnameindex == counter)
                   {
                       starty = starty + ((tablelen / colounmcount) * 2) + (ListIndexwithCustomWidth.Count * cnamelenbuffer);
                   }
                   else
                   {
                       if (ListIndexwithCustomWidth.Contains(counter))
                       {
                           starty = starty + 50;
                       }
                       else
                       {
                           starty = starty + (tablelen / colounmcount);
                       }
                       
                   }
                   counter = counter + 1;
               }
               if (text_height == 25)
               {
                   colheight = 25;
               }
               else
               {
                   colheight = 12;
               }
               yindex = yindex + colheight;
               tableheight = tableheight + colheight+20;
               starty   = table_x;
               colindex = 0;
               rowindex = rowindex + 1;
               text_height = 12;
               if (orientation == "1")
               {
                   limit_height = 500;
               }
               else
               {
                   limit_height = 690;
               }
               if (yindex >= limit_height)
               {
                   page = BuildHeader(pdf, bs, student);
                   yindex = table_y;
               }
               Paragraph paragraph = new Paragraph();
               Paragraph paragraphcredithours = new Paragraph();
               Paragraph paragraphinservice = new Paragraph();
               Paragraph paragraphgraduate = new Paragraph();
               Paragraph paragraphceu = new Paragraph();
               Paragraph paragraph1 = new Paragraph();
               Paragraph paragraph2 = new Paragraph();
               Paragraph paragraph3 = new Paragraph();
               Paragraph paragraph4 = new Paragraph();
               Paragraph paragraph5 = new Paragraph();
               Paragraph paragraph6 = new Paragraph();
               Paragraph paragraph7 = new Paragraph();
               Paragraph paragraph8 = new Paragraph();
               string totalinText = "";
               if (showcustomcredithoursTotal)
               {
                   totalinText = "Total " + Settings.Instance.GetMasterInfo2().CustomCreditTypeName + ": " + customcredithoursTotal.ToString();
                   paragraph.Add(new TextLine(fTotal, totalinText));
                   paragraph.SetAlignment(Align.RIGHT);
                  
               }
               if (showcredithoursTotal)
               {
                   totalinText ="Total " + Settings.Instance.GetMasterInfo2().CreditHoursName + ": " + credithoursTotal.ToString();
                   paragraphcredithours.Add(new TextLine(fTotal, totalinText));
                   paragraphcredithours.SetAlignment(Align.RIGHT);
               }
               if (showGraduateTotal)
               {
                   totalinText =  "Total Graduate: " + GraduateTotal.ToString();
                   paragraphgraduate.Add(new TextLine(fTotal, totalinText));
                   paragraphgraduate.SetAlignment(Align.RIGHT);
               }
               if (showsbceuTotal)
               {
                   totalinText = "Total " +Settings.Instance.GetMasterInfo2().CEUCreditLabel +": " + GraduateTotal.ToString();
                   paragraphceu.Add(new TextLine(fTotal, totalinText));
                   paragraphceu.SetAlignment(Align.RIGHT);
               }
               if (showInServiceTotal)
               {
                   totalinText = "Total " + Settings.Instance.GetMasterInfo2().InserviceHoursName + ": " + InServiceTotal.ToString();
                   paragraphinservice.Add(new TextLine(fTotal, totalinText));
                   paragraphinservice.SetAlignment(Align.RIGHT);
               }
               if (showoch1Total)
               {
                   totalinText = "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1 + ": " + och1Total.ToString();
                   paragraph1.Add(new TextLine(fTotal, totalinText));
                   paragraph1.SetAlignment(Align.RIGHT);
               }
               if (showoch2Total)
               {
                   totalinText =  "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2 + ": " + och2Total.ToString();
                   paragraph2.Add(new TextLine(fTotal, totalinText));
                   paragraph2.SetAlignment(Align.RIGHT);
               }
               if (showoch3Total)
               {
                   totalinText =  "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3 + ": " + och3Total.ToString();
                   paragraph3.Add(new TextLine(fTotal, totalinText));
                   paragraph3.SetAlignment(Align.RIGHT);
               }
               if (showoch4Total)
               {
                   totalinText = "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4 + ": " + och4Total.ToString();
                   paragraph4.Add(new TextLine(fTotal, totalinText));
                   paragraph4.SetAlignment(Align.RIGHT);
               }
               if (showoch5Total)
               {
                   totalinText ="Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5 + ": " + och5Total.ToString();
                   paragraph5.Add(new TextLine(fTotal, totalinText));
                   paragraph5.SetAlignment(Align.RIGHT);
               }
               if (showoch6Total)
               {
                   totalinText = "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6 + ": " + och6Total.ToString();
                   paragraph6.Add(new TextLine(fTotal, totalinText));
                   paragraph6.SetAlignment(Align.RIGHT);
               }
               if (showoch7Total)
               {
                   totalinText ="Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7 + ": " + och7Total.ToString();
                   paragraph7.Add(new TextLine(fTotal, totalinText));
                   paragraph7.SetAlignment(Align.RIGHT);
               }
               if (showoch8Total)
               {
                   totalinText = "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8 + ": " + och8Total.ToString();
                   paragraph8.Add(new TextLine(fTotal, totalinText));
                   paragraph8.SetAlignment(Align.RIGHT);
               }

                            
                            TextColumn columncustom = new TextColumn(fTotal);
                           // columncustom.SetLineBetweenParagraphs(true);
                            columncustom.SetSpaceBetweenLines(1);

                            columncustom.AddParagraph(paragraph);
                            columncustom.AddParagraph(paragraphcredithours);
                            columncustom.AddParagraph(paragraphinservice);
                            columncustom.AddParagraph(paragraphgraduate);
                            columncustom.AddParagraph(paragraphceu);
                            columncustom.AddParagraph(paragraph1);
                            columncustom.AddParagraph(paragraph2);
                            columncustom.AddParagraph(paragraph3);
                            columncustom.AddParagraph(paragraph4);
                            columncustom.AddParagraph(paragraph5);
                            columncustom.AddParagraph(paragraph6);
                            columncustom.AddParagraph(paragraph7);
                            columncustom.AddParagraph(paragraph8);
                            columncustom.SetPosition(30, limit_height+30);
                            columncustom.SetSize(page.GetWidth()-60, 100);
                            PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                            columncustom.SetSize(limit_height, limit_height);
                            columncustom.SetLocation(18, 18);
               

           }
           pdf.Flush();
           bs.Close();

        } //end execute()
        public Course GetDataForTranscript(int cid)
        {
            
            using (var db = new SchoolEntities())
            {
               Course course = (from c in db.Courses
                                  where c.COURSEID==cid 
                                  select c).FirstOrDefault();

               return course;
            }
        }
        public string GetEndDate(int cid)
        {

            using (var db = new SchoolEntities())
            {
                Course_Time course = (from c in db.Course_Times
                                 where c.COURSEID == cid orderby c.COURSEDATE descending
                                 select c).FirstOrDefault();

                return String.Format("{0:M/d/yyyy}", course.COURSEDATE);
            }
        }
        public string GetStartDate(int cid)
        {

            using (var db = new SchoolEntities())
            {
                Course_Time course = (from c in db.Course_Times
                                      where c.COURSEID == cid
                                      orderby c.COURSEDATE
                                      select c).FirstOrDefault();

                return String.Format("{0:M/d/yyyy}", course.COURSEDATE);
            }
        }
        
        public Page BuildHeader(PDF pdf,BufferedStream bs, Student student)
        {
            double totalcredits = 0;
            int fontsize_total = 12;
            double x_total = 0;
            double y_total = 0;
            double h_total = 0;
            double w_total = 0;
            var align_total = "0";
            var use_total = "0";
            var settings = Settings.Instance;
            var mi1 = settings.GetMasterInfo();
            var mi2 = settings.GetMasterInfo2();
            var mi3 = settings.GetMasterInfo3();
            var mi4 = settings.GetMasterInfo4();
            var tranheight = DefaultPdfTranscript.transheight;
            var tranwidth = DefaultPdfTranscript.transwidth;
            var tranboundx = DefaultPdfTranscript.boundX;
            var tranboundy = DefaultPdfTranscript.boundY;
            var tranmargintop = DefaultPdfTranscript.marginTop;
            var tranmarginbottom = DefaultPdfTranscript.marginBottom;
            var tranmarginleft= DefaultPdfTranscript.marginLeft;
            var tranmarginright = DefaultPdfTranscript.marginRight;
            var background = DefaultPdfTranscript.backgroundimage;


            //Header
            var HeaderText = DefaultPdfTranscript.OptionalHeaderText;
            var headerlocation = DefaultPdfTranscript.locheader1;
            var useheader = DefaultPdfTranscript.useHeader1;
            var fontheader = DefaultPdfTranscript.fontsizeHeader1;

            //Optional Text
            var optionalText = DefaultPdfTranscript.textInfo1;
            var optionalTextlocation = DefaultPdfTranscript.locinfo1;
            var useoptionalText = DefaultPdfTranscript.useInfo1;
            var fontOptionalText = DefaultPdfTranscript.fontsizeInfo1;

            //Student Info
            var namelocation = DefaultPdfTranscript.locname1;
            var usestudentName = true;
            var fontstudentnaME= DefaultPdfTranscript.fontsizename1;
            var alignSname = DefaultPdfTranscript.alignsname1;

            //Address
            //Student Info
            var addresslocation = DefaultPdfTranscript.locaddress1;
            var useaddress = DefaultPdfTranscript.useAddress1;
            var fontaddress = DefaultPdfTranscript.fontsizeAddress1;

            //CustomField 1
            //Student Info
            var cf1location = DefaultPdfTranscript.locStudField1;
            var usecf1 = DefaultPdfTranscript.useStudField1;
            var fontcf1 = DefaultPdfTranscript.fontsizeStudfield1;
            var cf1Text = DefaultPdfTranscript.selectedstudfield1;
            var alignStudCust1 = DefaultPdfTranscript.alignstudfield1;

            //CustomField 2
            //Student Info
            var cf2location = DefaultPdfTranscript.locStudField2;
            var usecf2 = DefaultPdfTranscript.useStudField2;
            var fontcf2 = DefaultPdfTranscript.fontsizeStudfield2;
            var cf2Text = DefaultPdfTranscript.selectedstudfield2;
            var alignStudCust2 = DefaultPdfTranscript.alignstudfield2;

            //CustomField 3
            //Student Info
            var cf3location = DefaultPdfTranscript.locStudField3;
            var usecf3 = DefaultPdfTranscript.useStudField3;
            var fontcf3 = DefaultPdfTranscript.fontsizeStudfield3;
            var cf3Text = DefaultPdfTranscript.selectedstudfield3;
            var alignStudCust3 = DefaultPdfTranscript.alignstudfield3;

            //CourseInfo
            var cnamelocation = DefaultPdfTranscript.locCoursename;
            var usecname = DefaultPdfTranscript.useCourseName;
            var fontcname = DefaultPdfTranscript.fontsizeCoursename;
            var includeCourseNumber = DefaultPdfTranscript.includeCourseNum;
            var SelectedFields = DefaultPdfTranscript.coursefieldjsonsettings;
            //Misc
            //DatePrinted
            var printeddatelocation = DefaultPdfTranscript.locDatePrint;
            var useprinteddate = DefaultPdfTranscript.useDatePrint;
            var fontprinteddate = DefaultPdfTranscript.fontsizeDateprint;
            var alignDatePrinted = DefaultPdfTranscript.aligndateprinted;
            //Student Custom Field

            Page page;
            orientation = "1";
            if (tranheight < tranwidth)
            {
                page = new Page(pdf, Letter.LANDSCAPE);
            }
            else
            {
                page = new Page(pdf, Letter.PORTRAIT);
                orientation = "0";
            }
            PDFjet.NET.Font f0 = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA);
            f0.SetSize(10);

            if (background != null)
            {
                string fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/images/pdfimages/" + background.ToString());
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
                        float widthfactor = float.Parse((tranwidth - (tranmarginleft + tranmarginright)).ToString()) / originalwidth;
                        float heightfactor = float.Parse((tranheight - (tranmargintop + tranmarginbottom)).ToString()) / originalheight;
                        //end

                        bckgroundimage.ScaleBy(widthfactor, heightfactor);
                        bckgroundimage.DrawOn(page);
                        filestream.Close();
                    }
                }
            }

            //Basic configuration objects
            for (int counter = 0; counter < 8; counter++)
            {
                int intDsiplay = 0;
                int fontsize = 12;
                string strAlllocation = "";
                string textvalue = "";
                int textalignment = Align.CENTER;

                if (counter == 0) //Date Print
                {
                    intDsiplay = useprinteddate.Value;
                    strAlllocation = printeddatelocation;
                    textvalue = DateTime.Now.ToShortDateString();
                    fontsize = fontprinteddate.Value;
                    textalignment = SetAlignment(alignDatePrinted.Value);
                }

                if (counter == 1) //Optional Text
                {
                    intDsiplay = useoptionalText.Value;
                    strAlllocation = optionalTextlocation;
                    textvalue = optionalText;
                    fontsize = fontOptionalText.Value;
                }

                if (counter == 2) //Header
                {
                    intDsiplay = useheader.Value;
                    strAlllocation = headerlocation;
                    textvalue = HeaderText;
                    fontsize = fontheader.Value;
                }

                if (counter == 3) //Student Name
                {
                    intDsiplay = 1;
                    strAlllocation = namelocation;
                    textvalue = student.FIRST + " " + student.LAST;
                    fontsize = fontstudentnaME.Value;
                    textalignment = SetAlignment(alignSname.Value);
                            
                }
                if (counter == 4) //CustomField 1
                {
                    intDsiplay = usecf1.Value;
                    strAlllocation = cf1location;
                    textvalue = GetDropdownSelectionValue(cf1Text,student,null,null);
                    fontsize = fontcf1.Value;
                    textalignment = SetAlignment(alignStudCust1.Value);
                }
                if (counter == 5) //Custom Field 2
                {
                    intDsiplay = usecf2.Value;
                    strAlllocation = cf2location;
                    textvalue = GetDropdownSelectionValue(cf2Text, student, null, null);
                    fontsize = fontcf2.Value;
                    textalignment = SetAlignment(alignStudCust2.Value);
                }
                if (counter == 6) //Custom Field 3
                {
                    intDsiplay = usecf3.Value;
                    strAlllocation = cf3location;
                    textvalue = GetDropdownSelectionValue(cf3Text, student, null, null);
                    fontsize = fontcf3.Value;
                    textalignment = SetAlignment(alignStudCust3.Value);
                }
                if (counter == 7) //Student Address
                {
                    intDsiplay = useaddress.Value;
                    strAlllocation = addresslocation;
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

                    fontsize = fontaddress.Value;
                    textalignment = SetAlignment(alignSname.Value);
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
                    f0.SetSize(fontsize);
                    Paragraph paragraph = new Paragraph();
                    if (textvalue == null)
                    {
                        textvalue = "";
                    }
                    paragraph.Add(new TextLine(f0, textvalue));
                    paragraph.SetAlignment(textalignment);
                    TextColumn columncustom = new TextColumn(f0);
                    columncustom.SetLineBetweenParagraphs(true);
                    columncustom.SetSpaceBetweenLines(5);

                    columncustom.AddParagraph(paragraph);
                    columncustom.SetPosition(x + tranmargintop.Value, y + tranmarginbottom.Value);
                    columncustom.SetSize(w, h);
                    PDFjet.NET.Point point1 = columncustom.DrawOn(page);
                    columncustom.SetSize(tranheight.Value, tranwidth.Value);
                    columncustom.SetLocation(18, 18);
                }
            }

            Table table_courses = new Table();
            List<List<Cell>> coursedata = new List<List<Cell>>();
            List<Cell> courserow = new List<Cell>();
            List<CustomTranscriptCourseFields> ListAllfields = ProcessSelectedFields(SelectedFields);
            ListIndexwithCustomWidth = new List<int>();
            colounmcount = 0;
            cnameindex = 0;
            PDFjet.NET.Font ftableHead = new PDFjet.NET.Font(pdf, CoreFont.HELVETICA_BOLD);
            ftableHead.SetSize(12);
            string textDisplay = "";
            foreach (var field in ListAllfields)
            {
                textDisplay = field.textDisplay.Replace('/', ' ').Replace('-',' ');
                Cell field0 = new Cell(ftableHead, textDisplay);              
                if (field.textDisplay.Length <= 5)
                {
                    if (field.id == "coursename")
                    {
                        field0.SetWidth(10);
                    }
                    else
                    {
                        field0.SetWidth(50);
                    }
                    ListIndexwithCustomWidth.Add(colounmcount);
                }
                courserow.Add(field0);

                if (field.id == "coursename")
                {
                    cnameindex = colounmcount;
                }
                colounmcount = colounmcount + 1;
            }
            coursedata.Add(courserow);
            
            tablelen = 540;
            cnamelenbuffer = 10;
            if (orientation == "1")
            {
                tablelen = 710;
                if (ListAllfields.Count > 7)
                {
                    cnamelenbuffer = 30;
                }
                else if (ListAllfields.Count > 5)
                {
                    cnamelenbuffer = 70;
                }
                else if (ListAllfields.Count > 3)
                {
                    cnamelenbuffer = 100;
                }
            }
            else
            {
                if (ListAllfields.Count > 7)
                {
                    cnamelenbuffer = 30;
                }
                else if (ListAllfields.Count > 5)
                {
                    cnamelenbuffer = 50;
                }
                else if (ListAllfields.Count > 3)
                {
                    cnamelenbuffer = 60;
                }
            }
            table_courses.SetData(coursedata, 1);
            string[] location_c =cnamelocation.Split(',');
            double x_c = 0;
            double y_c = 0;
            double w_c = 0;
            double h_c = 0;
            try
            {
                x_c = double.Parse(location_c[0]);
                y_c = double.Parse(location_c[1]);
                w_c = double.Parse(location_c[2]);
                h_c = double.Parse(location_c[3]);
            }
            catch
            {
                x_c = 0;
                y_c = 0;
                w_c = 0;
                h_c = 0;
            }
            
            
            
            var tby = y_c + 100;
            table_x = x_c;
            table_y = y_c;
            colounmcount = colounmcount + 1;
            
            for (int countx_c = 0; countx_c < colounmcount; countx_c++)
            {
                if (countx_c == cnameindex)
                {
                    table_courses.SetColumnWidth(cnameindex, ((tablelen / colounmcount) * 2) + (ListIndexwithCustomWidth.Count * cnamelenbuffer));
                }
                else
                {

                    if (ListIndexwithCustomWidth.Contains(countx_c))
                    {
                              
                    }
                    else
                    {
                       table_courses.SetColumnWidth(countx_c, (tablelen / colounmcount));
                      
                    }
                               
                            
                }
            }

            table_courses.SetCellBordersColor(16777215);

                table_courses.WrapAroundCellText();
            table_courses.SetNoCellBorders();
            table_courses.SetPosition(x_c, y_c - 30);
            table_courses.DrawOn(page);

            PdfOutFile =PdfOutFile.Replace(System.Web.HttpContext.Current.Server.MapPath("~/Temp/"), Settings.Instance.GetMasterInfo4().DotNetSiteRootUrl+"Temp/");
            return page;
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

        public List<Transcript> SortTranscriptGrid(List<Transcript> Transcripts, string sortedfield)
        {
            Transcripts = Transcripts.Select(tran => { tran.CourseCompletionDate = DateTime.Parse(GetEndDate(tran.CourseId.Value)); return tran; }).ToList();
            if (sortedfield == "startdate")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }

            else if (sortedfield == "coursename")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseName).ToList();
            }
            else if (sortedfield == "coursenum")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseNum).ToList();
            }
            else if (sortedfield == "customfield2")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }
            else if (sortedfield == "customcredit")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CustomCreditHours).ToList();
            }
            else if (sortedfield == "customfield3")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }
            else if (sortedfield == "enddate")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseCompletionDate).ToList();
            }
            else if (sortedfield == "customfield1")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }
            else if (sortedfield == "credithours")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CreditHours).ToList();
            }
            else if (sortedfield == "courseicon")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }
            else if (sortedfield == "Graduate")
            {
                return Transcripts.OrderBy(transctipt => transctipt.graduatecredit).ToList();
            }
            else if (sortedfield == "InService")
            {
                return Transcripts.OrderBy(transctipt => transctipt.InserviceHours).ToList();
            }
            else if (sortedfield == "customfield5")
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }
            else if (sortedfield == "sbceu")
            {
                return Transcripts.OrderBy(transctipt => transctipt.ceucredit).ToList();
            }

            else
            {
                return Transcripts.OrderBy(transctipt => transctipt.CourseStartDate).ToList();
            }

        }
        public int SetAlignment(int alignSname)
        {
            if (alignSname == 0)
            {
                return Align.CENTER;
            }
            else if (alignSname == 1)
            {
                return Align.LEFT;
            }
            else
            {
                return Align.RIGHT;
            }
        }
        public List<CustomTranscriptCourseFields> ProcessSelectedFields(string jsonfields)
        {

            CustomTranscriptModel TranscriptModel = new CustomTranscriptModel();
            if (jsonfields.Trim() == "")
            {
                jsonfields = TranscriptModel.DefaultSelectedField();
            }
            CustomTranscriptCourseFields fields = new CustomTranscriptCourseFields();
            List<CustomTranscriptCourseFields>ListSelectedFields = new List<CustomTranscriptCourseFields>();
            JavaScriptSerializer j = new JavaScriptSerializer();
            dynamic settingsconfig = j.Deserialize(jsonfields, typeof(object));
            foreach (var field in settingsconfig["selectedfield"])
            {
                fields = new CustomTranscriptCourseFields();
                fields.id = field["id"];
                fields.sort = field["sort"];
                fields.textDisplay = TranscriptModel.SetCourseHeaderDisplay(field["id"]);
                ListSelectedFields.Add(fields);
            }

            return ListSelectedFields;
        }

        private string GetDropdownSelectionValue(string objectname, Student student,Transcript transcript,Course course)
        {
            switch (objectname){
                case "address":
                    return student.ADDRESS;
                    
                case "city":
                    return student.CITY;
                case "state":
                    return student.STATE;
                case "zip":
                    return student.ZIP;
                case "district":
                    using (var db = new SchoolEntities())
                    {
                        var district = (from d in db.Districts where d.DISTID==student.DISTRICT select d.DISTRICT1).FirstOrDefault();
                        return district;
                    }
                    
                case "school":
                    using (var db = new SchoolEntities())
                    {
                        var school = (from d in db.Schools where d.ID == student.SCHOOL select d.LOCATION).FirstOrDefault();
                        return school;
                    }
                case "grade":
                    return student.GRADE.Value.ToString();
                case "StudRegField1":
                    return student.StudRegField1;
                case "StudRegField2":
                    return student.StudRegField2;
                case "StudRegField3":
                    return student.StudRegField3;
                case "StudRegField4":
                    return student.StudRegField4;
                case "StudRegField5":
                    return student.StudRegField5;
                case "StudRegField6":
                    return student.StudRegField6;
                case "StudRegField7":
                    return student.StudRegField7;
                case "StudRegField8":
                    return student.StudRegField8;
                case "StudRegField9":
                    return student.StudRegField9;
                case "StudRegField10":
                    return student.StudRegField10;
                case "ReadOnlyStudRegField1":
                    return student.ReadOnlyStudRegField1;
                case "ReadOnlyStudRegField2":
                    return student.ReadOnlyStudRegField2;
                case "ReadOnlyStudRegField4":
                    return student.ReadOnlyStudRegField4;
                case "ReadOnlyStudRegField3":
                    return student.ReadOnlyStudRegField3;
                case "HiddenStudRegField1":
                    return student.HiddenStudRegField1;
                case "HiddenStudRegField2":
                    return student.HiddenStudRegField2;
                case "HiddenStudRegField3":
                    return student.HiddenStudRegField3;
                case "HiddenStudRegField4":
                    return student.HiddenStudRegField4;
                case "districtextra":
                    using (var db = new SchoolEntities())
                    {
                        var district = (from d in db.Districts where d.DISTID == student.DISTRICT select d).FirstOrDefault();
                        var distExtraInfo=(from extra in db.DistrictExtraInfoes where extra.districtID== student.DISTRICT select extra).FirstOrDefault();
                        return district.DISTRICT1 + " " + distExtraInfo.daddress;
                    }

                case "schoolextra":
                    break;
                case "gradeextra":
                    break;


                //Course Credit
                case "credithours":
                    return transcript.CreditHours.Value.ToString();
                case "InService":
                    return transcript.InserviceHours.ToString();
                case "customcredit":
                    return transcript.CustomCreditHours.ToString();
                case "Graduate":
                    return transcript.graduatecredit.Value.ToString();
                case "sbceu":
                    return transcript.ceucredit.Value.ToString();
                case "och1":
                    return transcript.Optionalcredithours1.Value.ToString();
                case "och2":
                    return transcript.Optionalcredithours2.Value.ToString();
                case "och3":
                    return transcript.Optionalcredithours3.Value.ToString();
                case "och4":
                    return transcript.Optionalcredithours4.Value.ToString();
                case "och5":
                    return transcript.Optionalcredithours5.Value.ToString();
                case "och6":
                    return transcript.Optionalcredithours6.Value.ToString();
                case "och7":
                    return transcript.Optionalcredithours7.Value.ToString();
                case "och8":
                    return transcript.Optionalcredithours8.Value.ToString();

            //Custom Fields
                case "customfield1":
                    return course.CustomCourseField1;
                case "customfield2":
                    return course.CustomCourseField2;
                case "customfield3":
                    return course.CustomCourseField3;
                case "customfield4":
                    return course.CustomCourseField4;
                case "customfield5":
                    return course.CustomCourseField5;

            }
            return "";
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
        public int studentid
        {
            get;
            set;
        }
        public DateTime _start
        {
            get;
            set;
        }

        public DateTime _end
        {
            get;
            set;
        }

        public entities.customtranscript DefaultPdfTranscript
        {
            get;
            set;
        }

        public string PdfOutFile
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }

        public string RequestCaller
        {
            get;
            set;
        }


        public entities.PDFHeaderFooterInfo PdfHeaderFooterInfo
        {
            get;
            set;
        }

        public int tablelen
        {
            get;
            set;
        }
        public int cnamelenbuffer
        {
            get;
            set;
        }
        public int colounmcount
        {
            get;
            set;
        }
        public double table_x
        {
            get;
            set;
        }
        public double table_y
        {
            get;
            set;
        }
        public int cnameindex
        {
            get;
            set;
        }
        public List<int> ListIndexwithCustomWidth
        {
            get;
            set;
        }
        public string orientation
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
    }
}
