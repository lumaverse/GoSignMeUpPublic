using Gsmu.Api.Authorization;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Transcripts;
using Gsmu.Api.Data.School.User;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Export
{
    public class PDFReporting
    {

        public void GenerateStudentTranscript(string filename, UserModel userModel, string coursetype)
        {
            float imglogoHT = 35;

            FileStream fos = new FileStream(filename, FileMode.Create);
            BufferedStream bos = new BufferedStream(fos);

            PDF pdf = new PDF(bos);
            pdf.SetTitle("Student Transcript");
            pdf.SetSubject("Course Roster");
            pdf.SetAuthor("GoSignMeUp!");
            Page page = new Page(pdf, Letter.PORTRAIT);
            Font f0 = new Font(pdf, CoreFont.HELVETICA);
            Font f1 = new Font(pdf, CoreFont.HELVETICA);
            Font f2 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            Font f3 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            Font f4 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            f1.SetSize(11);
            f2.SetSize(11);
            f3.SetSize(23);
            f4.SetSize(18);
            f0.SetSize(10);

            Paragraph title = new Paragraph();
            title.SetAlignment(Align.LEFT);
            title.Add(new TextLine(f3, "Continuing Education Transcript"));
            Paragraph studinfo = new Paragraph();
            studinfo.SetAlignment(Align.LEFT);
            studinfo.Add(new TextLine(f4, userModel.Student.LAST + ", " + userModel.Student.FIRST));
            TextColumn column = new TextColumn(f1);
            column.SetLineBetweenParagraphs(true);
            column.SetSpaceBetweenLines(20);

            column.AddParagraph(title);
            column.AddParagraph(studinfo);
            column.SetPosition(230, 46);
            column.SetSize(520, 100.0);
            Point point = column.DrawOn(page);
            column.SetSize(470.0, 100.0);



            Paragraph date_address_info = new Paragraph();
            date_address_info.SetAlignment(Align.LEFT);
            if (Settings.Instance.GetMasterInfo3().useLicenseMod == 1)
            {
                using (var db = new SchoolEntities())
                {
                    var license = (from l in db.licenseinfoes where l.courseid == 0 && l.studentid == userModel.Student.STUDENTID
                                       select l).FirstOrDefault();
                    if (license!=null)
                    {
                        date_address_info.Add(new TextLine(f0, "State License:" + " " + license.licensestate));
                    }
                }
            }

            if (Settings.Instance.GetMasterInfo3().showAddressInTranscript == 1)
            {
                date_address_info.Add(new TextLine(f0, userModel.Student.ADDRESS + ", " + userModel.Student.CITY + ", "));
            }
            date_address_info.Add(new TextLine(f0, "Print Date:" + DateTime.Now.ToShortDateString()));
            
            TextColumn columndate_address = new TextColumn(f1);
            columndate_address.SetLineBetweenParagraphs(true);
            columndate_address.SetSpaceBetweenLines(10);
            columndate_address.AddParagraph(date_address_info);
            columndate_address.SetPosition(230, 143);
            columndate_address.SetSize(520, 100.0);
            Point pointdate = columndate_address.DrawOn(page);
            columndate_address.SetSize(470.0, 100.0);

            if (Settings.Instance.GetMasterInfo3().showAddressInTranscript == 1)
            {
                Paragraph date_address_info2 = new Paragraph();
                date_address_info2.SetAlignment(Align.LEFT);
                date_address_info2.Add(new TextLine(f0, userModel.Student.STATE + ", " + userModel.Student.ZIP + ", " + userModel.Student.COUNTRY));

                TextColumn columndate_address2 = new TextColumn(f1);
                columndate_address2.SetLineBetweenParagraphs(true);
                columndate_address2.SetSpaceBetweenLines(10);
                columndate_address2.AddParagraph(date_address_info2);
                columndate_address2.SetPosition(230, 163);
                columndate_address2.SetSize(520, 100.0);
                Point pointdate2 = columndate_address2.DrawOn(page);
                columndate_address2.SetSize(470.0, 100.0);
            }

            if (Settings.Instance.GetMasterInfo2().LogoOnTranscript == 1)
            {
                using (var db = new SchoolEntities())
                {
                    var image_name = (from p in db.PDFHeaderFooterInfoes
                                   select p.NewPDFLogoImage).First();
                    if (image_name!=null)
                    {
                        string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Areas/Public/Images/" +image_name.ToString());
                        if (File.Exists(fileName) == false)
                        {
                            fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/Images/" + image_name.ToString());
                            if (File.Exists(fileName) == false)
                            {
                                fileName = System.Web.HttpContext.Current.Server.MapPath("~/old/Images/" + image_name.ToString());
                            }
                        }
                        FileStream fis1 = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        Image image1 = new Image(pdf, fis1, ImageType.JPG);

                        image1.SetPosition(35, 25);
                        imglogoHT = image1.GetHeight();
                        image1.DrawOn(page);
                    }
                }
            }
            Line line = new Line(
                      230, 140,
                      570, 140
                      );
            line.DrawOn(page);

            Table table = new Table();
            List<List<Cell>> tableData = new List<List<Cell>>();
            List<Cell> row = new List<Cell>();
            Cell headinstructor = new Cell(f2, "Instructor");
            Cell headdate = new Cell(f2, "Completion  Date");
            headdate.SetTopPadding(-16);

            Cell headcredit = new Cell(f2, "Credit");
            Cell headgrade = new Cell(f2, "Grade");
            Cell headattendance = new Cell(f2, "Attendance");
            headinstructor.SetTextAlignment(Align.CENTER);
            headdate.SetTextAlignment(Align.CENTER);
            headgrade.SetTextAlignment(Align.CENTER);
            headattendance.SetTextAlignment(Align.CENTER);
            headcredit.SetTextAlignment(Align.CENTER);
            row.Add(headinstructor);
            row.Add(headdate);
            row.Add(headcredit);
            row.Add(headgrade);
            row.Add(headattendance);
            tableData.Add(row);
            Transcripts transcripts = new Transcripts();
            float CreditHours = 0;
            float ceucredit = 0;
            float graduatecredit = 0;
            float InserviceHours = 0;
            float CustomCreditHours = 0;
            float Optionalcredithours1 = 0;
            float Optionalcredithours2 = 0;
            float Optionalcredithours3 = 0;
            float Optionalcredithours4 = 0;
            float Optionalcredithours5 = 0;
            float Optionalcredithours6 = 0;
            float Optionalcredithours7 = 0;
            float Optionalcredithours8 = 0;



            foreach (Transcript data in transcripts.StudentTranscriptedCourse(userModel.Student.STUDENTID))
            {

                row = new List<Cell>();
                Cell c1 = new Cell(f1, " ");
                row.Add(c1);
                row.Add(c1);
                c1.SetBgColor(Color.lightgrey);
                Cell cournum = new Cell(f1, "Course No.: " + data.CourseNum);
                cournum.SetBgColor(Color.lightgrey);
                cournum.SetColSpan(3);
                cournum.SetLeftPadding(-160);
                row.Add(cournum);
                row.Add(new Cell(f1, ""));
                row.Add(new Cell(f1, ""));
                tableData.Add(row);
                row = new List<Cell>();

                c1 = new Cell(f1, " ");
                c1.SetBgColor(Color.lightgrey);
                row.Add(c1);
                row.Add(c1);
                Cell courname = new Cell(f1, "Course Name: " + data.CourseName);
                courname.SetBgColor(Color.lightgrey);
                courname.SetColSpan(3);
                courname.SetLeftPadding(-160);
                row.Add(courname);
                row.Add(new Cell(f1, ""));
                row.Add(new Cell(f1, ""));
                tableData.Add(row);
                row = new List<Cell>();

                Cell datecell = new Cell(f1, data.InstructorName);
                row.Add(datecell);
                Cell paidcell = new Cell(f1, data.CourseCompletionDate.ToShortDateString());
                row.Add(paidcell);
                string creditlist = "";
                if (data.CreditHours > 0)
                {
                    creditlist = Settings.Instance.GetMasterInfo2().CreditHoursName + ": " + data.CreditHours.ToString();
                    CreditHours = CreditHours + float.Parse(data.CreditHours.ToString());
                }
                if (data.ceucredit > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo2().CEUCreditLabel + ": " + data.ceucredit.ToString();
                    ceucredit = ceucredit + float.Parse(data.ceucredit.ToString());
                }

                if (data.InserviceHours > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo2().InserviceHoursName + ": " + data.InserviceHours.ToString();
                    InserviceHours = InserviceHours + float.Parse(data.InserviceHours.ToString());
                }
                if (data.graduatecredit > 0)
                {
                    creditlist = creditlist + " " + "Graduate Credit" + ": " + data.graduatecredit.ToString();
                    CreditHours = CreditHours + float.Parse(data.CreditHours.ToString());
                }

                if (data.CustomCreditHours > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo2().CustomCreditTypeName + ": " + data.CustomCreditHours.ToString();
                    CustomCreditHours = CustomCreditHours + float.Parse(data.CustomCreditHours.ToString());
                }
                if (data.Optionalcredithours1 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1 + ": " + data.Optionalcredithours1.ToString();
                    Optionalcredithours1 = Optionalcredithours1 + float.Parse(data.Optionalcredithours1.ToString());
                }
                if (data.Optionalcredithours2 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2 + ": " + data.Optionalcredithours2.ToString();
                    Optionalcredithours2 = Optionalcredithours2 + float.Parse(data.Optionalcredithours2.ToString());
                }
                if (data.Optionalcredithours3 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3 + ": " + data.Optionalcredithours3.ToString();
                    Optionalcredithours3 = Optionalcredithours3 + float.Parse(data.Optionalcredithours3.ToString());
                }
                if (data.Optionalcredithours4 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4 + ": " + data.Optionalcredithours4.ToString();
                    Optionalcredithours4 = Optionalcredithours4 + float.Parse(data.Optionalcredithours4.ToString());
                }
                if (data.Optionalcredithours5 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5 + ": " + data.Optionalcredithours5.ToString();
                    Optionalcredithours5 = Optionalcredithours5 + float.Parse(data.Optionalcredithours5.ToString());
                }
                if (data.Optionalcredithours6 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6 + ": " + data.Optionalcredithours6.ToString();
                    Optionalcredithours6 = Optionalcredithours6 + float.Parse(data.Optionalcredithours6.ToString());

                }
                if (data.Optionalcredithours7 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7 + ": " + data.Optionalcredithours7.ToString();
                    Optionalcredithours7 = Optionalcredithours7 + float.Parse(data.Optionalcredithours7.ToString());

                }
                if (data.Optionalcredithours8 > 0)
                {
                    creditlist = creditlist + " " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8 + ": " + data.Optionalcredithours8.ToString();
                    Optionalcredithours8 = Optionalcredithours8 + float.Parse(data.Optionalcredithours8.ToString());

                }
                Cell creditcell = new Cell(f1, creditlist);
                row.Add(creditcell);
                Cell gradecell = new Cell(f1, data.StudentGrade);
                row.Add(gradecell);
                row.Add(new Cell(f1, " "));
                tableData.Add(row);
            }
            Cell padd = new Cell(f1, " ");
            if (CreditHours > 0)
            {
                row = new List<Cell>();
                row.Add(padd);
                row.Add(padd);
                Cell total1 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo2().CreditHoursName + ": " + CreditHours.ToString());
                total1.SetLeftPadding(200);
                row.Add(total1);
                tableData.Add(row);
            }
            if (ceucredit > 0)
            {
                row = new List<Cell>();
                Cell total2 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo2().CEUCreditLabel + ": " + ceucredit.ToString());
                total2.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total2);
                tableData.Add(row);

            }
            if (InserviceHours > 0)
            {
                row = new List<Cell>();
                Cell total3 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo2().InserviceHoursName + ": " + InserviceHours.ToString());
                total3.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd); 
                row.Add(total3);
                tableData.Add(row);
            }
            if (graduatecredit > 0)
            {
                row = new List<Cell>();
                Cell total4 = new Cell(f1, "Total " + "Total Graduate Credit" + ": " + graduatecredit.ToString());
                total4.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total4);
                tableData.Add(row);
            }
            if (CustomCreditHours > 0)
            {
                row = new List<Cell>();
                Cell total5 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo2().CustomCreditTypeName + ": " + CustomCreditHours.ToString());
                total5.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total5);
                tableData.Add(row);
            }
            if (Optionalcredithours1 > 0)
            {
                row = new List<Cell>();
                Cell total6 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1 + ": " + Optionalcredithours1.ToString());
                total6.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total6);
                tableData.Add(row);
            }
            if (Optionalcredithours2 > 0)
            {
                row = new List<Cell>();
                Cell total7 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2 + ": " + Optionalcredithours2.ToString());
                total7.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total7);
                tableData.Add(row);
            }
            if (Optionalcredithours3 > 0)
            {
                row = new List<Cell>();
                Cell total8 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3 + ": " + Optionalcredithours3.ToString());
                total8.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total8);
                tableData.Add(row);
            }
            if (Optionalcredithours4 > 0)
            {
                row = new List<Cell>();
                Cell total9 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4 + ": " + Optionalcredithours4.ToString());
                total9.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total9);
                tableData.Add(row);
            }
            if (Optionalcredithours5 > 0)
            {
                row = new List<Cell>();
                Cell total10 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5 + ": " + Optionalcredithours5.ToString());
                total10.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total10);
                tableData.Add(row);
            }
            if (Optionalcredithours6 > 0)
            {
                row = new List<Cell>();
                Cell total11 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6 + ": " + Optionalcredithours6.ToString());
                total11.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total11);
                tableData.Add(row);
            }
            if (Optionalcredithours7 > 0)
            {
                Cell total12 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7 + ": " + Optionalcredithours7.ToString());
                total12.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total12);
                tableData.Add(row);
            }
            if (Optionalcredithours8 > 0)
            {
                row = new List<Cell>();
                Cell total14 = new Cell(f1, "Total " + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8 + ": " + Optionalcredithours8.ToString());
                total14.SetLeftPadding(200);
                row.Add(padd);
                row.Add(padd);
                row.Add(total14);
                tableData.Add(row);
            }

            table.SetData(tableData, 1);
            table.AutoAdjustColumnWidths();
            table.SetNoCellBorders();

            table.SetColumnWidth(0, 80);
            table.SetColumnWidth(1, 80);
            table.SetColumnWidth(2, 260);
            table.SetColumnWidth(3, 70);
            table.SetColumnWidth(4, 70);
            table.WrapAroundCellText();
            //table.RightAlignNumbers();

            int numOfPages = table.GetNumberOfPages(page);
           // table.SetPosition(30, 200);
            table.SetPosition(30, 135 + imglogoHT);
            int xpage = 0;
            while (true)
            {
                xpage = xpage + 1;
                TextColumn column2 = new TextColumn(f1);
                column2.SetLineBetweenParagraphs(true);
                column2.SetLineSpacing(1.0);
                Paragraph p1 = new Paragraph();
                p1.SetAlignment(Align.CENTER);
                p1.Add(new TextLine(f1, "Page " + xpage + " of " + numOfPages));
                Paragraph p2 = new Paragraph();
                p2.SetAlignment(Align.RIGHT);
                column2.AddParagraph(p2);
                column2.AddParagraph(p1);
                column2.SetPosition(30, 700);
                column2.SetSize(520, 100.0);
                column2.DrawOn(page);
                table.SetBottomMargin(100);
                table.DrawOn(page);
                if (!table.HasMoreData())
                {
                    // Allow the table to be drawn again later:
                    table.ResetRenderedPagesCount();
                    break;
                }
                table.SetPosition(30, 40);
                page = new Page(pdf, Letter.PORTRAIT);
            }

            pdf.Flush();
            bos.Close();
        }
        public void GenerateStudentRosterReport(string filename, UserModel userModel, string coursetype)
        {
            FileStream fos = new FileStream(filename, FileMode.Create);
            BufferedStream bos = new BufferedStream(fos);

            PDF pdf = new PDF(bos);
            pdf.SetTitle("Student Enrollment");
            pdf.SetSubject("Course Roster");
            pdf.SetAuthor("GoSignMeUp!");
            Page page = new Page(pdf, Letter.PORTRAIT);
            Font f1 = new Font(pdf, CoreFont.HELVETICA);
            Font f2 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            f1.SetSize(10);
            f2.SetSize(10);

            Paragraph title = new Paragraph();
            title.SetAlignment(Align.CENTER);
            title.Add(new TextLine(f2, "STUDENT " + coursetype.ToUpper() + " COURSES"));
            Paragraph studinfo = new Paragraph();
            studinfo.SetAlignment(Align.LEFT);
            studinfo.Add(new TextLine(f2, "STUDENT NAME:" + userModel.Student.LAST + ", " + userModel.Student.FIRST));
            TextColumn column = new TextColumn(f1);
            column.SetLineBetweenParagraphs(true);
            column.SetLineSpacing(1.0);

            column.AddParagraph(title);
            column.AddParagraph(studinfo);
            column.SetPosition(30, 20);
            column.SetSize(520.0, 100.0);
            Point point = column.DrawOn(page);
            column.SetSize(470.0, 100.0);


            Table table = new Table();
            List<List<Cell>> tableData = new List<List<Cell>>();
            List<Cell> row = new List<Cell>();
            row.Add(new Cell(f2, "Course Number"));
            row.Add(new Cell(f2, "Course Name"));
            row.Add(new Cell(f2, "Course Date"));
            row.Add(new Cell(f2, "Paid"));
            tableData.Add(row);
            bool alternate = true;
            decimal totalpaid = 0;
            foreach (UserCourses data in userModel.CommonUserInfo.courses)
            {
                if (coursetype == data.CourseType)
                {
                    if (alternate)
                    {
                        alternate = !alternate;
                    }
                    else
                    {
                        alternate = true;
                    }
                    row = new List<Cell>();
                    Cell cournum = new Cell(f1, data.COURSENUM);
                    Cell courname = new Cell(f1, data.COURSENAME);
                    if (!alternate)
                    {
                        cournum.SetBgColor(Color.lightgrey);
                        courname.SetBgColor(Color.lightgrey);
                    }

                    row.Add(cournum);
                    row.Add(courname);
                    try
                    {
                        Cell datecell = new Cell(f1, DateTime.Parse(data.MaxDate.ToString()).ToShortDateString());
                        if (!alternate)
                        {

                            datecell.SetBgColor(Color.lightgrey);
                        }
                        row.Add(datecell);
                    }
                    catch
                    {
                        Cell datecell = new Cell(f1, "Not Available");
                        if (!alternate)
                        {

                            datecell.SetBgColor(Color.lightgrey);
                        }
                        row.Add(datecell);
                    }
                    try
                    {
                        Cell paidcell = new Cell(f1, "$" + Math.Round(Decimal.Parse(data.TotalPaid.ToString()), 2).ToString());
                        if (!alternate)
                        {
                            paidcell.SetBgColor(Color.lightgrey);
                        }
                        totalpaid = totalpaid + Math.Round(Decimal.Parse(data.TotalPaid.ToString()), 2);
                        row.Add(paidcell);
                    }
                    catch
                    {
                        Cell paidcell = new Cell(f1, "0.00");
                        if (!alternate)
                        {
                            paidcell.SetBgColor(Color.lightgrey);
                        }
                        row.Add(paidcell);
                    }
                    tableData.Add(row);

                }
            }
            table.SetData(tableData, 1);
            table.AutoAdjustColumnWidths();
            table.SetNoCellBorders();
            table.SetColumnWidth(0, 100);
            table.SetColumnWidth(1, 300);
            table.SetColumnWidth(2, 100);
            //table.RightAlignNumbers();

            int numOfPages = table.GetNumberOfPages(page);
            table.SetPosition(30, 60);
            int xpage = 0;
            while (true)
            {
                xpage = xpage + 1;
                TextColumn column2 = new TextColumn(f1);
                column2.SetLineBetweenParagraphs(true);
                column2.SetLineSpacing(1.0);
                Paragraph p1 = new Paragraph();
                p1.SetAlignment(Align.CENTER);
                p1.Add(new TextLine(f1, "Page " + xpage + " of " + numOfPages));
                Paragraph p2 = new Paragraph();
                p2.SetAlignment(Align.RIGHT);
                TextLine texttotalline = new TextLine(f1);
                texttotalline.SetText("Total Paid: $" + totalpaid.ToString());
                texttotalline.SetTextEffect(12);
                p2.Add(texttotalline);
                column2.AddParagraph(p2);
                column2.AddParagraph(p1);
                column2.SetPosition(30, 700);
                column2.SetSize(520, 100.0);
                column2.DrawOn(page);
                table.SetBottomMargin(100);
                table.DrawOn(page);
                if (!table.HasMoreData())
                {
                    // Allow the table to be drawn again later:
                    table.ResetRenderedPagesCount();
                    break;
                }
                table.SetPosition(30, 20);
                page = new Page(pdf, Letter.PORTRAIT);
            }

            pdf.Flush();
            bos.Close();
        }
    }
}
