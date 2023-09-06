using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gsmu.Api.Export
{

    public class PDFSigninSheet
    {
        static int policy_char_endposition = 0;
        public static Dictionary<string, string> GenerateDictionary(HttpRequestBase Request)
        {
            Dictionary<string, string> paramdictionary = new Dictionary<string, string>();
            List<string> keys = new List<string>(Request.QueryString.AllKeys);
            paramdictionary.Add("combo1", Request.QueryString["combo1"]);
            paramdictionary.Add("combo2", Request.QueryString["combo2"]);
            paramdictionary.Add("combo3", Request.QueryString["combo3"]);
            paramdictionary.Add("combo4", Request.QueryString["combo4"]);
            paramdictionary.Add("combo5", Request.QueryString["combo5"]);
            paramdictionary.Add("optional", Request.QueryString["optional"]);
            paramdictionary.Add("RosterLayout", Request.QueryString["RosterLayout"]);
            paramdictionary.Add("image", Request.QueryString["selectFilename"]);
            paramdictionary.Add("signindatevalue", Request.QueryString["signindatevalue"]);
            paramdictionary.Add("shownotes", Request.QueryString["shownotes"]);
            paramdictionary.Add("multipledate", Request.QueryString["multipledate"]);
            paramdictionary.Add("papersize", Request.QueryString["papersize"]);
            paramdictionary.Add("addfivelinesvalue", Request.QueryString["addfivelinesvalue"]);
            paramdictionary.Add("extendedheader", Request.QueryString["extendedheader"]);
            //waitlist
            if (keys.Contains("includewaitinglist"))
            {
                paramdictionary.Add("includewaitinglist", Request.QueryString["includewaitinglist"]);
            }
            else
            {
                paramdictionary.Add("includewaitinglist", "false");
            }

            return paramdictionary;
        }
        public static void GenerateSigninSheet(string filename, int courseid, HttpRequestBase Request)
        {
            using (var db = new SchoolEntities())
            {
                var SrtHeaderYpos = 0;
                var pdfconfig = (from p in db.PDFHeaderFooterInfoes select p).FirstOrDefault();
                var course = (from l in db.Courses
                              where l.COURSEID == courseid
                              select l).FirstOrDefault();
                var coursetime = (from ct in db.Course_Times where ct.COURSEID == courseid select ct).ToList();
                var instructors = (from inst in db.Instructors where inst.INSTRUCTORID == course.INSTRUCTORID || inst.INSTRUCTORID == course.INSTRUCTORID2 || inst.INSTRUCTORID == course.INSTRUCTORID3 select inst).ToList();
                string ExtraParticipantLabel = (string.IsNullOrEmpty(Settings.Instance.GetMasterInfo4().ExtraParticipantLabel) ? "Extra Participant Name" : Settings.Instance.GetMasterInfo4().ExtraParticipantLabel);

                Dictionary<string, string> paramdictionary = GenerateDictionary(Request);
                string orientaion = paramdictionary["RosterLayout"];
                string shownotes = paramdictionary["shownotes"];
                string multipledate = paramdictionary["multipledate"];
                string papersize = paramdictionary["papersize"];
                string extendedheader = paramdictionary["extendedheader"];
                if ((papersize == null) || (papersize == "") || papersize == "null")
                {
                    papersize = "letter";
                }
                else
                {
                    papersize = papersize.ToLower();
                }
                short nooflines;
                int addlines = 0;
                bool validblankline = Int16.TryParse(paramdictionary["addfivelinesvalue"], out nooflines);
                if (validblankline)
                {
                    addlines = nooflines;
                }
                FileStream fos = new FileStream(filename, FileMode.Create);
                BufferedStream bos = new BufferedStream(fos);
                PDF pdf = new PDF(bos);
                pdf.SetTitle("Course Signinsheet");
                pdf.SetSubject("Course Roster");
                pdf.SetAuthor("GoSignMeUp!");
                var pageproperty = Letter.PORTRAIT;
                if ((papersize == "letter") || (papersize == "null"))
                {

                    if (orientaion == "1")
                    {
                        pageproperty = Letter.LANDSCAPE;
                    }
                }
                else
                {
                    pageproperty = Legal.PORTRAIT;
                    if (orientaion == "1")
                    {
                        pageproperty = Legal.LANDSCAPE;
                    }
                }
                Page page = new Page(pdf, pageproperty);
                Font f0_bold = new Font(pdf, CoreFont.HELVETICA_BOLD);
                Font f0 = new Font(pdf, CoreFont.HELVETICA);
                Font f1 = new Font(pdf, CoreFont.HELVETICA);
                Font f2 = new Font(pdf, CoreFont.HELVETICA_BOLD);
                Font f3 = new Font(pdf, CoreFont.HELVETICA);
                Font f4 = new Font(pdf, CoreFont.HELVETICA_BOLD);
                Font blankRowFont = new Font(pdf, CoreFont.HELVETICA_BOLD);

                f1.SetSize(11);
                f2.SetSize(9);
                f3.SetSize(9);
                f4.SetSize(18);
                f0.SetSize(9);
                f0_bold.SetSize(9);
                blankRowFont.SetSize(30);

                List<SiginSheetFields> ListStudents = GenerateStudentList(courseid, paramdictionary);
                List<SiginSheetFields> ListWaitingStudents = new List<SiginSheetFields>();
                if (paramdictionary["includewaitinglist"].ToString() == "true")
                {
                    ListWaitingStudents = GenereateWaitingStudentList(courseid, paramdictionary); //WAITING
                }
                int enrolled = ListStudents.Count;
                int waiting = ListWaitingStudents.Count;
                //page = SetHeader(page, pdf, enrolled, waiting, pdfconfig, coursetime, course, instructors, paramdictionary);
                int tablelen = 540;
                if ((papersize == "letter") || (papersize == "null"))
                {
                    if (orientaion == "1")
                    {
                        tablelen = 710;
                    }
                }
                else
                {
                    if (orientaion == "1")
                    {
                        tablelen = 930;
                    }
                }
                int columncount = 2;
                //distribute to each column.

                Table table_student = new Table();
                table_student.SetCellBordersColor(Color.black);
                table_student.SetCellBordersWidth(1f);

                List<List<Cell>> studentdata = new List<List<Cell>>();
                List<Cell> studentrow = new List<Cell>();
                List<Cell> blankstudentrow = new List<Cell>();
                List<Cell> blankstudentrowLast = new List<Cell>();

                #region INIT COLUMN NAMES/TITLES

                Cell field0 = new Cell(f2, "");
                Cell field1 = new Cell(f2, "Name");
                Cell field2 = new Cell(f2, "");
                Cell field3 = new Cell(f2, "");
                Cell field4 = new Cell(f2, "");
                Cell field5 = new Cell(f2, "n/a");
                Cell field6 = new Cell(f2, "n/a");
                Cell field7 = new Cell(f2, "Signature");
                Cell field8 = new Cell(f2, "Custom");
                studentrow.Add(field0);
                studentrow.Add(field1);
                #endregion

                #region INIT COMBOBOX VALUES (CUSTOM COLUMN NAMES/TITLES)

                string comb1 = "";
                if (paramdictionary.ContainsKey("combo1"))
                {
                    string value = paramdictionary["combo1"];
                    if ((value != "") && (value != null) && (value != "undefined"))
                    {
                        if (value.Contains("|"))
                        {
                            value = value.Split('|')[1];
                        }
                        field2 = new Cell(f2, value.Replace("!", " #"));
                        studentrow.Add(field2);
                        columncount = columncount + 1;
                        comb1 = value;
                    }
                }

                string comb2 = "";
                if (paramdictionary.ContainsKey("combo2"))
                {
                    string value = paramdictionary["combo2"];
                    if ((value != "") && (value != null) && (value != "undefined"))
                    {
                        if (value.Contains("|"))
                        {
                            value = value.Split('|')[1];
                        }
                        field3 = new Cell(f2, value.Replace("!", " #"));
                        studentrow.Add(field3);
                        columncount = columncount + 1;
                        comb2 = value;
                    }
                }

                string comb3 = "";
                if (paramdictionary.ContainsKey("combo3"))
                {
                    string value = paramdictionary["combo3"];
                    if ((value != "") && (value != null) && (value != "undefined"))
                    {
                        if (value.Contains("|"))
                        {
                            value = value.Split('|')[1];
                        }
                        field4 = new Cell(f2, value.Replace("!", " #"));
                        studentrow.Add(field4);
                        columncount = columncount + 1;
                        comb3 = value;
                    }
                }


                string comb4 = "";
                if (paramdictionary.ContainsKey("combo4"))
                {
                    string value = paramdictionary["combo4"];
                    if ((value != "") && (value != null) && (value != "undefined"))
                    {
                        if (value.Contains("|"))
                        {
                            value = value.Split('|')[1];
                        }
                        field5 = new Cell(f2, value.Replace("!", " #"));
                        studentrow.Add(field5);
                        columncount = columncount + 1;
                        comb4 = value;
                    }
                }
                string comb5 = "";
                if (paramdictionary.ContainsKey("combo5"))
                {
                    string value = paramdictionary["combo5"];
                    if ((value != "") && (value != null) && (value != "undefined"))
                    {
                        if (value.Contains("|"))
                        {
                            value = value.Split('|')[1];
                        }
                        field6 = new Cell(f2, value.Replace("!", " #"));
                        studentrow.Add(field6);
                        columncount = columncount + 1;
                        comb5 = value;
                    }
                }


                if (paramdictionary.ContainsKey("optional"))
                {
                    string value = paramdictionary["optional"];
                    if ((value != "") && (value != null) && (value != "undefined"))
                    {
                        field8 = new Cell(f2, value);
                        studentrow.Add(field8);
                        columncount = columncount + 1;
                    }
                }
                studentrow.Add(field7);
                studentdata.Add(studentrow);
                #endregion

                #region INIT STUDENTLIST ROW VALUES
                int count = 0;

                foreach (var student in ListStudents)
                {
                    count = count + 1;
                    studentrow = new List<Cell>();
                    blankstudentrow = new List<Cell>();
                    blankstudentrowLast = new List<Cell>();

                    for (int x = 0; x <= columncount; x++)
                    {
                        if (x == 0)
                        {
                            Cell colfield = new Cell(f2, count.ToString());
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if (x == 1)
                        {
                            Cell colfield = new Cell(f2, student.Name.Replace("()", ""));
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if ((x == 2) && (comb1 != ""))
                        {
                            Cell colfield = new Cell(f3, student.Field1);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        else if (x == 2)
                        {
                            Cell colfield = new Cell(f3, "");
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if ((x == 3) && (comb2 != ""))
                        {
                            Cell colfield = new Cell(f3, student.Field2);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        else if (x == 3)
                        {
                            Cell colfield = new Cell(f3, "");
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if ((x == 4) && (comb3 != ""))
                        {
                            Cell colfield = new Cell(f3, student.Field3);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        else if (x == 4)
                        {
                            Cell colfield = new Cell(f3, "");
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if ((x == 5) && (comb4 != ""))
                        {
                            Cell colfield = new Cell(f3, student.Field4);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        else if ((x == 5) && (comb5 != ""))
                        {
                            Cell colfield = new Cell(f3, student.Field5);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                            comb5 = "displayed";
                        }

                        else if ((x == 5))
                        {
                            Cell colfield = new Cell(f3, student.Field4);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if ((x == 6) && (comb5 != "displayed"))
                        {
                            Cell colfield = new Cell(f3, student.Field5);
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        else if (x == 6)
                        {
                            Cell colfield = new Cell(f3, "");
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if (x == 7)
                        {
                            Cell colfield = new Cell(f3, "");
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }
                        if (x == 8)
                        {
                            Cell colfield = new Cell(f3, "");
                            colfield.SetBgColor(Color.lightgray);
                            studentrow.Add(colfield);
                            blankstudentrow.Add(new Cell(blankRowFont, "  "));
                            blankstudentrowLast.Add(new Cell(blankRowFont, "  "));
                        }

                    }

                    studentdata.Add(studentrow);
                    //studentdata.Add(blankstudentrow);
                    //studentdata.Add(blankstudentrowLast);

                    #region STUDENTLIST EXTRAPARTICIIPANT
                    if (!string.IsNullOrWhiteSpace(student.CRExtraParticipant) && student.CRExtraParticipant.Trim() != "[skipnumbering]")
                    {

                        studentrow = new List<Cell>();

                        Cell crextrprtspc = new Cell(f3, "");
                        crextrprtspc.SetBgColor(Color.lightgray);
                        studentrow.Add(crextrprtspc);
                        if (!student.CRExtraParticipant.Contains("[skipnumbering]"))
                        {
                                ExtraParticipantLabel = Settings.Instance.GetMasterInfo2().CollectionStyleLabel;
                                Cell crextrprt = new Cell(f3, ExtraParticipantLabel + ": ");
                                crextrprt.SetBgColor(Color.lightgray);
                                studentrow.Add(crextrprt);
                                studentdata.Add(studentrow);
                        }

                        for (int x = 0; x <= (columncount - 2); x++)
                        {
                            studentrow.Add(crextrprtspc);
                        }
                        //--
                        foreach (string extranames in student.CRExtraParticipant.Split(','))
                        {
                            if (!string.IsNullOrEmpty(extranames)) count++;
                            studentrow = new List<Cell>();
                            Cell crextrprtspc2 = new Cell(f3, "");
                            crextrprtspc2.SetBgColor(Color.lightgray);
                            studentrow.Add(crextrprtspc2);

                            string revisedExtraNames = (!string.IsNullOrEmpty(extranames) ? count.ToString() + ". " + extranames.Substring(1) : string.Empty);
                            if (revisedExtraNames.Contains("[skipnumbering]"))
                            {
                                revisedExtraNames = extranames.Substring(1).Replace("[skipnumbering]", "");
                                count = count - 1;
                                Cell crextrprt = new Cell(f3, ExtraParticipantLabel + ": " + revisedExtraNames);
                                crextrprt.SetBgColor(Color.lightgray);
                                studentrow.Add(crextrprt);
                                studentdata.Add(studentrow);
                            }
                            else 
                            {
                                Cell crextrprt_val = new Cell(f3, revisedExtraNames);
                                crextrprt_val.SetBgColor(Color.lightgray);
                                studentrow.Add(crextrprt_val);
                                studentdata.Add(studentrow);

                                for (int x = 0; x <= (columncount - 2); x++)
                                {
                                    studentrow.Add(crextrprtspc2);
                                }
                            }
                        }
                    }
                    #endregion




                    studentrow = new List<Cell>();
                    string date = paramdictionary["signindatevalue"];
                    multipledate = paramdictionary["multipledate"];
                    int splitcount = 0;
                    if (date != null)
                    {
                        date = date.Replace("[", "").Replace("]", "");

                        splitcount = date.Split(',').Count();
                        if (!date.Contains(','))
                        {
                            date = date + " ";
                        }
                    }

                    string datevalue = "";
                    int spacecount = 0;
                    if (splitcount == 1)
                    {
                        if (orientaion == "1")
                        {
                            spacecount = 180;
                        }
                        else
                        {
                            spacecount = 160;
                        }
                    }
                    if (splitcount == 2)
                    {
                        if (orientaion == "1")
                        {
                            spacecount = 94;
                        }
                        else
                        {
                            spacecount = 82;
                        }
                    }
                    else if (splitcount == 3)
                    {
                        if (orientaion == "1")
                        {
                            spacecount = 65;
                        }
                        else
                        {
                            spacecount = 50;
                        }

                    }
                    else if (splitcount == 4)
                    {
                        if (orientaion == "1")
                        {
                            spacecount = 54;
                        }
                        else
                        {
                            spacecount = 35;
                        }
                    }
                    else if (splitcount == 5)
                    {
                        if (orientaion == "1")
                        {
                            spacecount = 29;
                        }
                        else
                        {
                            spacecount = 21;
                        }
                    }

                    else if (splitcount == 6)
                    {
                        if (orientaion == "1")
                        {
                            spacecount = 25;
                        }
                        else
                        {
                            spacecount = 14;
                        }
                    }




                    string mydate = date;
                    if (date != null)
                    {
                        if ((date == "[]") || (date == "") || (date == " "))
                        {
                            if (multipledate != null && multipledate.Contains('1'))
                            {
                                date = string.Join(",", coursetime.Select(d => d.COURSEDATE.Value.ToString("d")));
                            }
                            //foreach (var dateandtime in coursetime)
                            //{
                            //    //if ((date == "[]") || (date == "") || (date == " "))
                            //    //{
                            //        date += dateandtime.COURSEDATE.Value.ToString("d");
                            //    //}
                            //}
                        }
                        spacecount = spacecount - 14;
                        var singledatevalue = "";
                        foreach (var d in date.Split(','))
                        {
                            singledatevalue = d.Replace("\"", String.Empty);
                            datevalue = datevalue + "" + singledatevalue;
                            datevalue = datevalue + "  __________";

                            for (int x = 0; x <= spacecount; x++)
                            {
                                datevalue = datevalue + " ";

                            }
                        }
                    }
                    if ((date == "[]") || (date == "") || (date == null) || (date == " "))
                    {
                        date = " ";
                    }
                    Cell footfield = new Cell(f3, "Date:" + datevalue);
                    Cell footfieldempty = new Cell(f3, "1");


                    footfield.SetColSpan(columncount + 1);

                    for (int x = 0; x <= columncount; x++)
                    {
                        if (x == 0)
                        {
                            studentrow.Add(footfield);
                        }
                        else
                        {
                            Cell footfield3 = new Cell(f3, " ");
                            studentrow.Add(footfield3);
                        }
                    }
                    if (multipledate != null && multipledate.Contains('1'))
                    {
                        studentdata.Add(studentrow);
                    }

                    studentrow = new List<Cell>();
                    Cell notefield = new Cell(f3, "Note: " + student.Note);
                    Cell blankfield = new Cell(f3, " ");

                    if (shownotes == "1")
                    {
                        notefield.SetColSpan(columncount + 1);
                        blankfield.SetColSpan(columncount + 1);
                        List<Cell> blankrow = new List<Cell>();
                        for (int xo = 0; xo <= columncount; xo++)
                        {
                            if (xo == 0)
                            {
                                if (shownotes == "1")
                                {
                                    studentrow.Add(notefield);
                                    blankrow.Add(blankfield);
                                }
                            }
                            else
                            {
                                Cell footfield3 = new Cell(f3, " ");
                                studentrow.Add(footfield3);
                                blankrow.Add(footfield3);
                            }
                        }

                        studentdata.Add(studentrow);
                        studentdata.Add(blankrow);

                    }

                    else
                    {
                        blankfield.SetColSpan(columncount + 1);

                        for (int xo = 0; xo <= columncount; xo++)
                        {
                            if (xo == 0)
                            {
                                if (shownotes == "1")
                                {
                                    studentrow.Add(blankfield);
                                }
                            }
                            else
                            {
                                Cell footfield3 = new Cell(f3, " ");
                                studentrow.Add(footfield3);
                            }
                        }

                        studentdata.Add(studentrow);
                    }

                    //if (count == ListStudents.Count())
                    //{
                    //    studentrow = new List<Cell>();
                    //    string addspace = "";
                    //    if (orientaion == "1")
                    //    {


                    //        //if (columncount == 6)
                    //        //{
                    //        //    addspace = "_____";
                    //        //}
                    //        //if (columncount == 7)
                    //        //{
                    //        //    addspace = "_________";
                    //        //}
                    //        Cell linefield = new Cell(f3, addspace + "_________________________________________________________________________________________________________________________________________________");
                    //        studentrow.Add(linefield);

                    //    }
                    //    else
                    //    {
                    //        //if (columncount == 6)
                    //        //{
                    //        //    addspace = "__";
                    //        //}
                    //        //if (columncount == 7)
                    //        //{
                    //        //    addspace = "_____";
                    //        //}
                    //        Cell linefield = new Cell(f3, addspace + "_______________________________________________________________________________________________________________");
                    //        studentrow.Add(linefield);
                    //    }

                    //    for (int x = 0; x < addlines; x++)
                    //    {
                    //        studentdata.Add(studentrow);
                    //    }
                    //}
                }
                enrolled = count;
                #endregion

                #region INIT WAITINGLIST ROW VALUES
                //WAITING LIST
                studentrow = new List<Cell>();
                if (paramdictionary.ContainsKey("includewaitinglist"))
                {

                    studentrow = new List<Cell>();
                    blankstudentrow = new List<Cell>();

                    string value = paramdictionary["includewaitinglist"];
                    if (value == "true")
                    {
                        //Adding Line for separator
                        string addspace = "";
                        if (orientaion == "1")
                        {
                            Cell linefield = new Cell(f3, addspace + "_________________________________________________________________________________________________________________________________________________");
                            studentrow.Add(linefield);
                        }
                        else
                        {
                            Cell linefield = new Cell(f3, addspace + "_______________________________________________________________________________________________________________");
                            studentrow.Add(linefield);
                        }

                        studentrow = new List<Cell>();
                        for (int x = 0; x < addlines; x++)
                        {
                            studentdata.Add(studentrow);
                        }
                        studentrow = new List<Cell>();
                        //WAIT LIST TITLE
                        Font waitingTitleFont = new Font(pdf, CoreFont.HELVETICA_BOLD);
                        waitingTitleFont.SetSize(15);

                        Cell titleWaiting = new Cell(waitingTitleFont, "WAIT LIST");
                        titleWaiting.SetBgColor(Color.white);
                        studentrow.Add(titleWaiting);
                        studentdata.Add(studentrow);
                        studentrow = new List<Cell>();
                        //BLANK SPACEs
                        Cell blank2 = new Cell(waitingTitleFont, " ");
                        titleWaiting.SetBgColor(Color.white);
                        studentrow.Add(blank2);
                        studentdata.Add(studentrow);
                        studentrow = new List<Cell>();

                        //Adding Line Ending
                        //Fetch and Write Waiting List Students
                        foreach (var waitingstudent in ListWaitingStudents)
                        {
                            count = count + 1;
                            studentrow = new List<Cell>();
                            blankstudentrow = new List<Cell>();

                            for (int x = 0; x <= columncount; x++)
                            {
                                if (x == 0)
                                {
                                    Cell colfield = new Cell(f2, count.ToString());
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if (x == 1)
                                {
                                    Cell colfield = new Cell(f2, waitingstudent.Name.Replace("()", ""));
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                }
                                if ((x == 2) && (comb1 != ""))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field1);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                else if (x == 2)
                                {
                                    Cell colfield = new Cell(f3, "");
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if ((x == 3) && (comb2 != ""))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field2);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                else if (x == 3)
                                {
                                    Cell colfield = new Cell(f3, "");
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if ((x == 4) && (comb3 != ""))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field3);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                else if (x == 4)
                                {
                                    Cell colfield = new Cell(f3, "");
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if ((x == 5) && (comb4 != ""))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field4);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                else if ((x == 5) && (comb5 != ""))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field5);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                    comb5 = "displayed";
                                }

                                else if ((x == 5))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field4);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if ((x == 6) && (comb5 != "displayed"))
                                {
                                    Cell colfield = new Cell(f3, waitingstudent.Field5);
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                else if (x == 6)
                                {
                                    Cell colfield = new Cell(f3, "");
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if (x == 7)
                                {
                                    Cell colfield = new Cell(f3, "");
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                                if (x == 8)
                                {
                                    Cell colfield = new Cell(f3, "");
                                    colfield.SetBgColor(Color.lightgray);
                                    studentrow.Add(colfield);
                                    blankstudentrow.Add(new Cell(f2, "  "));
                                }
                            }
                            studentdata.Add(studentrow);
                            studentdata.Add(blankstudentrow);

                            studentrow = new List<Cell>();
                            string date = paramdictionary["signindatevalue"];
                            multipledate = paramdictionary["multipledate"];
                            int splitcount = 0;
                            if (date != null)
                            {
                                date = date.Replace("[", "").Replace("]", "");

                                splitcount = date.Split(',').Count();
                                if (!date.Contains(','))
                                {
                                    date = date + " ";
                                }
                            }

                            string datevalue = "";
                            int spacecount = 0;
                            if (splitcount == 1)
                            {
                                if (orientaion == "1")
                                {
                                    spacecount = 180;
                                }
                                else
                                {
                                    spacecount = 160;
                                }
                            }
                            if (splitcount == 2)
                            {
                                if (orientaion == "1")
                                {
                                    spacecount = 94;
                                }
                                else
                                {
                                    spacecount = 82;
                                }
                            }
                            else if (splitcount == 3)
                            {
                                if (orientaion == "1")
                                {
                                    spacecount = 65;
                                }
                                else
                                {
                                    spacecount = 50;
                                }

                            }
                            else if (splitcount == 4)
                            {
                                if (orientaion == "1")
                                {
                                    spacecount = 54;
                                }
                                else
                                {
                                    spacecount = 35;
                                }
                            }
                            else if (splitcount == 5)
                            {
                                if (orientaion == "1")
                                {
                                    spacecount = 29;
                                }
                                else
                                {
                                    spacecount = 21;
                                }
                            }

                            else if (splitcount == 6)
                            {
                                if (orientaion == "1")
                                {
                                    spacecount = 25;
                                }
                                else
                                {
                                    spacecount = 14;
                                }
                            }




                            string mydate = date;
                            if (date != null)
                            {
                                if ((date == "[]") || (date == "") || (date == " "))
                                {
                                    foreach (var dateandtime in coursetime)
                                    {
                                        if ((date == "[]") || (date == "") || (date == " "))
                                        {
                                            date = dateandtime.COURSEDATE.Value.ToString("d");
                                        }
                                    }
                                }
                                spacecount = spacecount - 14;
                                var singledatevalue = "";
                                foreach (var d in date.Split(','))
                                {
                                    singledatevalue = d.Replace("\"", String.Empty);
                                    datevalue = datevalue + "" + singledatevalue;
                                    datevalue = datevalue + "  __________";

                                    for (int x = 0; x <= spacecount; x++)
                                    {
                                        datevalue = datevalue + " ";

                                    }
                                }
                            }
                            if ((date == "[]") || (date == "") || (date == null) || (date == " "))
                            {
                                date = " ";
                            }
                            Cell footfield = new Cell(f3, "Date:" + datevalue);
                            Cell footfieldempty = new Cell(f3, "1");


                            footfield.SetColSpan(columncount + 1);

                            for (int x = 0; x <= columncount; x++)
                            {
                                if (x == 0)
                                {
                                    footfield.SetBorder(Border.TOP, true);
                                    studentrow.Add(footfield);
                                }
                                else
                                {
                                    Cell footfield3 = new Cell(f3, " ");
                                    studentrow.Add(footfield3);
                                }
                            }
                            if (multipledate.Contains('1'))
                            {
                                studentdata.Add(studentrow);
                            }

                            studentrow = new List<Cell>();
                            Cell notefield = new Cell(f3, "Note: " + waitingstudent.Note);
                            Cell blankfield = new Cell(f3, " ");
                            if (shownotes == "1")
                            {
                                notefield.SetColSpan(columncount + 1);
                                blankfield.SetColSpan(columncount + 1);
                                List<Cell> blankrow = new List<Cell>();
                                for (int xo = 0; xo <= columncount; xo++)
                                {
                                    if (xo == 0)
                                    {
                                        if (shownotes == "1")
                                        {
                                            studentrow.Add(notefield);
                                            blankrow.Add(blankfield);

                                        }
                                    }
                                    else
                                    {
                                        Cell footfield3 = new Cell(f3, " ");
                                        studentrow.Add(footfield3);
                                        blankrow.Add(footfield3);
                                    }
                                }

                                studentdata.Add(studentrow);
                                studentdata.Add(blankrow);

                            }

                            else
                            {
                                blankfield.SetColSpan(columncount + 1);

                                for (int xo = 0; xo <= columncount; xo++)
                                {
                                    if (xo == 0)
                                    {
                                        if (shownotes == "1")
                                        {
                                            studentrow.Add(blankfield);
                                        }
                                    }
                                    else
                                    {
                                        Cell footfield3 = new Cell(f3, " ");
                                        studentrow.Add(footfield3);
                                    }
                                }

                                studentdata.Add(studentrow);
                            }

                            //if (count == ListStudents.Count())
                            //{
                            //    studentrow = new List<Cell>();
                            //    string addspace = "";
                            //    if (orientaion == "1")
                            //    {


                            //        //if (columncount == 6)
                            //        //{
                            //        //    addspace = "_____";
                            //        //}
                            //        //if (columncount == 7)
                            //        //{
                            //        //    addspace = "_________";
                            //        //}
                            //        Cell linefield = new Cell(f3, addspace + "_________________________________________________________________________________________________________________________________________________");
                            //        studentrow.Add(linefield);

                            //    }
                            //    else
                            //    {
                            //        //if (columncount == 6)
                            //        //{
                            //        //    addspace = "__";
                            //        //}
                            //        //if (columncount == 7)
                            //        //{
                            //        //    addspace = "_____";
                            //        //}
                            //        Cell linefield = new Cell(f3, addspace + "_______________________________________________________________________________________________________________");
                            //        studentrow.Add(linefield);
                            //    }

                            //    for (int x = 0; x < addlines; x++)
                            //    {
                            //        studentdata.Add(studentrow);
                            //    }
                            //}
                        }
                    }
                }
                #endregion

                studentrow = new List<Cell>();
                table_student.SetData(studentdata, 1);
                for (int x = 0; x <= columncount; x++)
                {
                    if (x != 0)
                    {
                        if (x < columncount - 1)
                        {
                            table_student.SetColumnWidth(x, tablelen / columncount);

                        }
                        else
                        {
                            table_student.SetColumnWidth(x, tablelen / columncount);
                        }
                    }
                    else
                    {
                        table_student.SetColumnWidth(x, 20);

                    }
                }

                int numOfPages = table_student.GetNumberOfPages(page);
                table_student.SetNoCellBorders();
                int tab_pos = 180;
                int starty = 30;
                int colheight = 12;
                int colindex = 0;
                int rowindex = 0;
                int tableheight = 0;
                int header_height = 0;
                int limit_height = 0;
                int text_height = 12;
                bool BlankNext = false;
                object[] TempCol = new object[50];
                bool setheader = false;
                Font font_text = new Font(pdf, CoreFont.HELVETICA);

                foreach (var row in studentdata.AsEnumerable().Skip(1))

                {
                    if (!setheader)
                    {


                        page = SetHeader(page, pdf, enrolled, waiting, pdfconfig, coursetime, course, instructors, paramdictionary);
                        setheader = true;


                        if (policy_char_endposition > 4)
                        {
                            tab_pos = 40 * policy_char_endposition;
                        }

                        if (policy_char_endposition == 4)
                        {
                            tab_pos = tab_pos + 30;
                        }

                        if (studentdata.IndexOf(row) == 1)
                        {
                            SrtHeaderYpos = tab_pos;
                        }
                        else
                        {
                            tab_pos = SrtHeaderYpos;
                        }

                        foreach (var r in studentdata[0])
                        {
                            Font font_head = new Font(pdf, CoreFont.HELVETICA_BOLD);
                            font_head.SetSize(9);
                            TextBox textbox = new TextBox(font_head);
                            textbox.SetText(r.GetText() + " ");
                            textbox.SetWidth(tablelen / columncount);
                            textbox.SetPosition(starty, tab_pos);
                            if (r.GetText().Count() == 0)
                            {
                                starty = starty + 20;
                            }
                            else
                            {
                                textbox.FitHeight();
                                //textbox.FitWidth();
                                starty = starty + (tablelen / columncount);
                            }
                            textbox.SetNoBorders();
                            textbox.DrawOn(page);
                        }
                        tab_pos += 30;
                        starty = 30;


                        Line headerline = new Line(starty, tab_pos - 15, tablelen + 30, tab_pos - 15);
                        headerline.SetColor(Color.black);
                        headerline.DrawOn(page);

                    }

                    // only for identifying the TopHeight
                    int TopHeight = 0;
                    bool WithBg = true;
                    foreach (var col in row)
                    {

                        font_text = new Font(pdf, CoreFont.HELVETICA);
                        font_text.SetSize(9);
                        TextBox textbox = new TextBox(font_text);

                        if ((col.GetText().Contains("Note:")) || (col.GetText().Contains("Date:")) || (col.GetText().Contains("____")))
                        {
                            textbox.SetWidth(tablelen);
                            textbox.SetHeight(24);
                            WithBg = false;
                        }
                        else
                        {
                            textbox.SetWidth(tablelen / columncount);
                        }

                        string studentColumnValue = col.GetText().Replace("Date:", "");
                        if (orientaion != "1" && studentColumnValue.ToString().Length > 15 && columncount > 5 && !studentColumnValue.Contains(" ") && !col.GetText().Contains("____"))
                        {
                            if (studentColumnValue.IndexOf("@") > -1)
                            {
                                studentColumnValue = studentColumnValue.Insert(studentColumnValue.IndexOf("@"), " ");
                            }
                        }

                        textbox.SetText(studentColumnValue);
                        textbox.WrapText();
                        int curTextBoxHeight = Convert.ToInt32(Math.Ceiling(textbox.GetHeight()));
                        if (studentColumnValue == "") { curTextBoxHeight = 0; }
                        if (curTextBoxHeight > TopHeight)
                        {
                            TopHeight = curTextBoxHeight;
                        }

                        textbox.SetSpacing(0);
                        colindex = colindex + 1;

                    }

                    //Backgroud Shade 
                    bool showTxtbxBg = false;
                    TextBox BgShade = new TextBox(font_text);
                    BgShade.SetText("");
                    BgShade.SetPosition(30, tab_pos);
                    BgShade.SetWidth(tablelen);
                    BgShade.SetHeight(TopHeight);
                    BgShade.SetNoBorders();
                    if ((shownotes!= null && multipledate != null) && (shownotes == "1" || multipledate.Contains('1')))
                    {
                        if (WithBg)
                        {
                            if (!BlankNext)
                            {
                                showTxtbxBg = true;
                                BgShade.SetBgColor(16053492);
                            }
                            BlankNext = false;
                        }
                        else
                        {
                            BlankNext = true;
                        }
                    }
                    else
                    {
                        if (!BlankNext)
                        {
                            showTxtbxBg = true;
                            BgShade.SetBgColor(16053492);
                            BlankNext = true;
                        }
                        else
                        {
                            BlankNext = false;
                        }

                    }
                    BgShade.DrawOn(page);


                    // Displaying data
                    starty = 30;
                    colindex = 0;
                    foreach (var col in row)
                    {

                        font_text = new Font(pdf, CoreFont.HELVETICA);
                        font_text.SetSize(9);
                        TextBox textbox = new TextBox(font_text);

                        if ((col.GetText().Contains("Note:")) || (col.GetText().Contains("Date:")) || (col.GetText().Contains("____")))
                        {
                            textbox.SetWidth(tablelen);
                            textbox.SetHeight(24);
                        }
                        else
                        {
                            textbox.SetWidth(tablelen / columncount);
                        }

                        string studentColumnValue = col.GetText().Replace("Date:", "");
                        //studentColumnValue = studentColumnValue.Replace("0", "");

                        if (orientaion != "1" && studentColumnValue.ToString().Length > 15 && columncount > 5 && !studentColumnValue.Contains(" ") && !col.GetText().Contains("____"))
                        {
                            if (studentColumnValue.IndexOf("@") > -1)
                            {
                                studentColumnValue = studentColumnValue.Insert(studentColumnValue.IndexOf("@"), " ");
                            }
                        }

                        textbox.SetText(studentColumnValue);

                        textbox.SetPosition(starty, tab_pos);

                        bool OthrDta = false;
                        if ((col.GetText().Contains("Note:")) || (col.GetText().Contains("Date:")) || (col.GetText().Contains("____")))
                        {
                            OthrDta = true;
                        }
                        else
                        {
                            textbox.WrapText();
                        }

                        if (colindex == 0 && !OthrDta)
                        {
                            starty = starty + 20;
                        }
                        else
                        {
                            starty = starty + (tablelen / columncount);
                        }

                        if (col.GetText().Contains("Note:"))
                        {
                            textbox.WrapText();
                        }

                        int curTextBoxHeight = Convert.ToInt32(Math.Ceiling(textbox.GetHeight()));
                        if (studentColumnValue == "") { curTextBoxHeight = 0; }
                        if (curTextBoxHeight > TopHeight)
                        {
                            TopHeight = curTextBoxHeight;
                        }

                        textbox.SetNoBorders();
                        textbox.SetSpacing(0);
                        textbox.DrawOn(page);
                        colindex = colindex + 1;

                    }

                    tab_pos = tab_pos + TopHeight;
                    tableheight = tableheight + TopHeight;
                    starty = 30;
                    colindex = 0;


                    if (showTxtbxBg)
                    {
                        int rghtLinePos = tablelen - (tablelen / columncount) + starty;
                        Line signatrline = new Line(rghtLinePos, tab_pos, tablelen + starty, tab_pos);
                        signatrline.SetColor(Color.black);
                        signatrline.DrawOn(page);
                    }

                    rowindex = rowindex + 1;
                    if ((papersize == "letter") || (papersize == "null"))
                    {
                        if (orientaion == "1")
                        {
                            limit_height = 540;
                        }
                        else
                        {
                            limit_height = 690;
                        }
                    }
                    else
                    {
                        if (orientaion == "1")
                        {
                            limit_height = 540;
                        }
                        else
                        {
                            limit_height = 920;
                        }
                    }
                    bool got_headerheight = false;
                    if (tab_pos >= limit_height)
                    {
                        if ((papersize == "letter") || (papersize == "null"))
                        {
                            if (orientaion == "1")
                            {
                                page = new Page(pdf, Letter.LANDSCAPE);
                                if (extendedheader == "1")
                                {
                                    setheader = false;
                                }
                                else
                                {
                                    tab_pos = 50;
                                }
                            }
                            else
                            {
                                page = new Page(pdf, Letter.PORTRAIT);
                                if (extendedheader == "1")
                                {
                                    setheader = false;
                                }
                                else
                                {
                                    tab_pos = 50;
                                }
                            }
                            if (header_height == 24)
                            {
                                got_headerheight = true;

                            }
                        }
                        else
                        {
                            if (orientaion == "1")
                            {
                                page = new Page(pdf, Legal.LANDSCAPE);
                                if (extendedheader == "1")
                                {
                                    setheader = false;
                                }
                                else
                                {
                                    tab_pos = 50;
                                }
                            }
                            else
                            {
                                page = new Page(pdf, Legal.PORTRAIT);
                                if (extendedheader == "1")
                                {

                                    setheader = false;
                                }
                                else
                                {
                                    tab_pos = 50;
                                }
                            }
                            if (header_height == 24)
                            {
                                got_headerheight = true;

                            }
                        }

                    }
                }

                for (int x = 0; x < addlines; x++)
                {
                    Line bottomline = new Line(starty, tab_pos + (30 * x), tablelen + starty, tab_pos + (30 * x));
                    bottomline.SetColor(Color.black);
                    bottomline.DrawOn(page);
                }

                table_student.SetPosition(30, tab_pos);
                pdf.Flush();
                bos.Close();
            }
        }

        public static List<SiginSheetFields> GenerateStudentList(int courseId, Dictionary<string, string> param)
        {


            List<SiginSheetFields> ListStudents = new List<SiginSheetFields>();
            using (var db = new SchoolEntities())
            {
                var studentlist = (from cr in db.Course_Rosters
                                   join s in db.Students on cr.STUDENTID equals s.STUDENTID
                                   join c in db.Courses on cr.COURSEID equals c.COURSEID
                                   where cr.COURSEID == courseId && cr.Cancel == 0 && cr.WAITING == 0
                                   select new
                                   {
                                       roster_id = cr.RosterID,
                                       name = s.LAST + ", " + s.FIRST,
                                       vdistrict = s.DISTRICT ?? 0,
                                       vschool = s.SCHOOL ?? 0,
                                       GRADELEVEL = s.GRADE ?? 0,
                                       StudRegField1 = s.StudRegField1 ?? " ",
                                       StudRegField2 = s.StudRegField2 ?? " ",
                                       StudRegField3 = s.StudRegField3 ?? " ",
                                       StudRegField4 = s.StudRegField4 ?? " ",
                                       StudRegField5 = s.StudRegField5 ?? " ",
                                       StudRegField6 = s.StudRegField6 ?? " ",
                                       StudRegField7 = s.StudRegField7 ?? " ",
                                       StudRegField8 = s.StudRegField8 ?? " ",
                                       StudRegField9 = s.StudRegField9 ?? " ",
                                       StudRegField10 = s.StudRegField10 ?? " ",
                                       StudRegField11 = s.StudRegField11 ?? " ",
                                       StudRegField12 = s.StudRegField12 ?? " ",
                                       StudRegField13 = s.StudRegField13 ?? " ",
                                       StudRegField14 = s.StudRegField14 ?? " ",
                                       StudRegField15 = s.StudRegField15 ?? " ",
                                       StudRegField16 = s.StudRegField16 ?? " ",
                                       StudRegField17 = s.StudRegField17 ?? " ",
                                       StudRegField18 = s.StudRegField18 ?? " ",
                                       StudRegField19 = s.StudRegField19 ?? " ",
                                       StudRegField20 = s.StudRegField20 ?? " ",
                                       ReadOnlyStudRegField1 = s.ReadOnlyStudRegField1 ?? " ",
                                       ReadOnlyStudRegField2 = s.ReadOnlyStudRegField2 ?? " ",
                                       ReadOnlyStudRegField3 = s.ReadOnlyStudRegField3 ?? " ",
                                       ReadOnlyStudRegField4 = s.ReadOnlyStudRegField4 ?? " ",
                                       HiddenStudRegField1 = s.HiddenStudRegField1 ?? " ",
                                       HiddenStudRegField2 = s.HiddenStudRegField2 ?? " ",
                                       HiddenStudRegField3 = s.HiddenStudRegField3 ?? " ",
                                       HiddenStudRegField4 = s.HiddenStudRegField4 ?? " ",
                                       CustomCourseField1 = c.CustomCourseField1 ?? " ",
                                       CustomCourseField2 = c.CustomCourseField2 ?? " ",
                                       CustomCourseField3 = c.CustomCourseField3 ?? " ",
                                       CustomCourseField4 = c.CustomCourseField4 ?? " ",
                                       CustomCourseField5 = c.CustomCourseField5 ?? " ",

                                       username = s.USERNAME ?? " ",
                                       email = s.EMAIL ?? " ",
                                       additionalemail = s.additionalemail ?? " ",
                                       address = s.ADDRESS ?? " ",
                                       city = s.CITY ?? " ",
                                       state = s.STATE ?? " ",
                                       zip = s.ZIP ?? " ",
                                       homephone = s.HOMEPHONE ?? " ",
                                       workphone = s.WORKPHONE ?? " ",
                                       fax = s.FAX ?? " ",
                                       ParentsName = s.parentsid.ToString() ?? " ",
                                       BlankTimeIn = "",
                                       BlankTimeOut = "",
                                       Note = cr.CustomSignInNotes ?? "",
                                       CheckOutNote = cr.CheckoutComments,
                                       StudentChoiceCourse = string.IsNullOrEmpty(cr.StudentChoiceCourse.ToString()) ? " " : db.CourseChoices.Where(cc => cc.CourseChoiceId == cr.StudentChoiceCourse).FirstOrDefault().CourseChoice1,
                                       rosterid = cr.RosterID,
                                       extraparticipants = cr.ExtraParticipant ?? " "

                                   }).ToList()
                                   .Select(x => new
                                   {
                                       x.roster_id,
                                       x.name,
                                       x.vdistrict,
                                       x.vschool,
                                       x.GRADELEVEL,
                                       x.StudRegField1,
                                       x.StudRegField2,
                                       x.StudRegField3,
                                       x.StudRegField4,
                                       x.StudRegField5,
                                       x.StudRegField6,
                                       x.StudRegField7,
                                       x.StudRegField8,
                                       x.StudRegField9,
                                       x.StudRegField10,
                                       x.StudRegField11,
                                       x.StudRegField12,
                                       x.StudRegField13,
                                       x.StudRegField14,
                                       x.StudRegField15,
                                       x.StudRegField16,
                                       x.StudRegField17,
                                       x.StudRegField18,
                                       x.StudRegField19,
                                       x.StudRegField20,
                                       x.ReadOnlyStudRegField1,
                                       x.ReadOnlyStudRegField2,
                                       x.ReadOnlyStudRegField3,
                                       x.ReadOnlyStudRegField4,
                                       x.HiddenStudRegField1,
                                       x.HiddenStudRegField2,
                                       x.HiddenStudRegField3,
                                       x.HiddenStudRegField4,
                                       x.CustomCourseField1,
                                       x.CustomCourseField2,
                                       x.CustomCourseField3,
                                       x.CustomCourseField4,
                                       x.CustomCourseField5,
                                       x.username,
                                       x.email,
                                       x.additionalemail,
                                       x.address,
                                       x.city,
                                       x.state,
                                       x.zip,
                                       x.homephone,
                                       x.workphone,
                                       x.fax,
                                       x.ParentsName,
                                       x.BlankTimeIn,
                                       x.BlankTimeOut,
                                       Note = string.IsNullOrEmpty(x.Note) ? x.CheckOutNote : x.Note,
                                       x.StudentChoiceCourse,
                                       CRExtraParticipant = CreateExtraParticipants(x.roster_id, x.extraparticipants),
                                       AmountOwe = "$ " + AmountOwe(x.roster_id, db).ToString("0.00")
                                   });

                studentlist = studentlist.OrderBy(x => x.name).ToList();
                string combo1field = "";
                if (param.ContainsKey("combo1"))
                {
                    string value = param["combo1"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo1field = value.Split('|')[0];
                        }
                    }
                }
                string combo2field = "";
                if (param.ContainsKey("combo2"))
                {
                    string value = param["combo2"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo2field = value.Split('|')[0];
                        }
                    }
                }
                string combo3field = "";
                if (param.ContainsKey("combo3"))
                {
                    string value = param["combo3"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo3field = value.Split('|')[0];
                        }
                    }
                }
                string combo4field = "";
                if (param.ContainsKey("combo4"))
                {
                    string value = param["combo4"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo4field = value.Split('|')[0];
                        }
                    }
                }
                string combo5field = "";
                if (param.ContainsKey("combo5"))
                {
                    string value = param["combo5"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo5field = value.Split('|')[0];
                        }
                    }
                }

                foreach (var student in studentlist)
                {
                    SiginSheetFields SiginSheetFields = new SiginSheetFields();
                    SiginSheetFields.Name = student.name;
                    SiginSheetFields.Note = student.Note;
                    SiginSheetFields.CRExtraParticipant = student.CRExtraParticipant;
                    if (combo1field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field1 = student.GetType().GetProperty(combo1field).GetValue(student, null).ToString();

                            if ((combo1field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field1 = grade;
                                }

                            }

                            if ((combo1field.ToLower() == "vschool") && (SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field1 = school;
                                }

                            }

                            if ((combo1field.ToLower() == "vdistrict") && (SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var school = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field1 = school;
                                }

                            }

                        }
                        catch
                        {
                            SiginSheetFields.Field1 = student.GetType().GetProperty("GRADELEVEL").GetValue(student, null).ToString(); ;
                            if ((SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field1 = grade;
                                }

                            }

                        }
                    }
                    else
                    {
                        SiginSheetFields.Field1 = student.GetType().GetProperty("GRADELEVEL").GetValue(student, null).ToString(); ;
                        if ((SiginSheetFields.Field1 != ""))
                        {
                            int value = int.Parse(SiginSheetFields.Field1);
                            var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                            if (grade != null)
                            {
                                SiginSheetFields.Field1 = grade;
                            }

                        }
                    }
                    if (combo2field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field2 = student.GetType().GetProperty(combo2field).GetValue(student, null).ToString();
                            if ((combo2field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field2 = grade;
                                }

                            }

                            if ((combo2field.ToLower() == "vschool") && (SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field2 = school;
                                }

                            }

                            if ((combo2field.ToLower() == "vdistrict") && (SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var school = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field2 = school;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field2 = student.GetType().GetProperty("vschool").GetValue(student, null).ToString();
                            if ((SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field2 = school;
                                }

                            }
                        }
                    }
                    else
                    {
                        SiginSheetFields.Field2 = student.GetType().GetProperty("vschool").GetValue(student, null).ToString();
                        if ((SiginSheetFields.Field2 != ""))
                        {
                            int value = int.Parse(SiginSheetFields.Field2);
                            var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                            if (school != null)
                            {
                                SiginSheetFields.Field2 = school;
                            }

                        }
                    }
                    if (combo3field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field3 = student.GetType().GetProperty(combo3field).GetValue(student, null).ToString();

                            if ((combo3field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field3 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field3);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field3 = grade;
                                }

                            }

                            if ((combo3field.ToLower() == "vschool") && (SiginSheetFields.Field3 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field3);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field3 = school;
                                }

                            }

                            if ((combo3field.ToLower() == "vdistrict") && (SiginSheetFields.Field3 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field3);
                                var dist = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (dist != null)
                                {
                                    SiginSheetFields.Field3 = dist;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field3 = student.GetType().GetProperty("homephone").GetValue(student, null).ToString();

                        }
                    }
                    else
                    {
                        SiginSheetFields.Field3 = student.GetType().GetProperty("homephone").GetValue(student, null).ToString();

                    }

                    if (combo4field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field4 = student.GetType().GetProperty(combo4field).GetValue(student, null).ToString();
                            if ((combo4field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field4 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field4);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field4 = grade;
                                }

                            }

                            if ((combo4field.ToLower() == "vschool") && (SiginSheetFields.Field4 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field4);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field4 = school;
                                }

                            }

                            if ((combo4field.ToLower() == "vdistrict") && (SiginSheetFields.Field4 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field4);
                                var dist = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (dist != null)
                                {
                                    SiginSheetFields.Field4 = dist;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field4 = "";
                        }
                    }
                    else
                    {
                        SiginSheetFields.Field4 = "";
                    }
                    if (combo5field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field5 = student.GetType().GetProperty(combo5field).GetValue(student, null).ToString();
                            if ((combo5field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field5 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field5);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field5 = grade;
                                }

                            }

                            if ((combo5field.ToLower() == "vschool") && (SiginSheetFields.Field5 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field5);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field5 = school;
                                }

                            }

                            if ((combo5field.ToLower() == "vdistrict") && (SiginSheetFields.Field5 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field5);
                                var dist = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (dist != null)
                                {
                                    SiginSheetFields.Field5 = dist;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field5 = "";
                        }
                    }
                    else
                    {
                        SiginSheetFields.Field5 = "";
                    }

                    ListStudents.Add(SiginSheetFields);
                }
            }


            return ListStudents;
        }

        //WAITINGLIST
        public static List<SiginSheetFields> GenereateWaitingStudentList(int courseId, Dictionary<string, string> param)
        {
            List<SiginSheetFields> ListStudents = new List<SiginSheetFields>();
            using (var db = new SchoolEntities())
            {
                var studentlist = (from cr in db.Course_Rosters
                                   join s in db.Students on cr.STUDENTID equals s.STUDENTID
                                   join c in db.Courses on cr.COURSEID equals c.COURSEID
                                   where cr.COURSEID == courseId && cr.Cancel == 0 && (cr.WAITING == -1 || cr.WAITING == 1)
                                   select new
                                   {
                                       roster_id = cr.RosterID,
                                       name = s.LAST + ", " + s.FIRST,
                                       vdistrict = s.DISTRICT ?? 0,
                                       vschool = s.SCHOOL ?? 0,
                                       GRADELEVEL = s.GRADE ?? 0,
                                       StudRegField1 = s.StudRegField1 ?? " ",
                                       StudRegField2 = s.StudRegField2 ?? " ",
                                       StudRegField3 = s.StudRegField3 ?? " ",
                                       StudRegField4 = s.StudRegField4 ?? " ",
                                       StudRegField5 = s.StudRegField5 ?? " ",
                                       StudRegField6 = s.StudRegField6 ?? " ",
                                       StudRegField7 = s.StudRegField7 ?? " ",
                                       StudRegField8 = s.StudRegField8 ?? " ",
                                       StudRegField9 = s.StudRegField9 ?? " ",
                                       StudRegField10 = s.StudRegField10 ?? " ",
                                       StudRegField11 = s.StudRegField11 ?? " ",
                                       StudRegField12 = s.StudRegField12 ?? " ",
                                       StudRegField13 = s.StudRegField13 ?? " ",
                                       StudRegField14 = s.StudRegField14 ?? " ",
                                       StudRegField15 = s.StudRegField15 ?? " ",
                                       StudRegField16 = s.StudRegField16 ?? " ",
                                       StudRegField17 = s.StudRegField17 ?? " ",
                                       StudRegField18 = s.StudRegField18 ?? " ",
                                       StudRegField19 = s.StudRegField19 ?? " ",
                                       StudRegField20 = s.StudRegField20 ?? " ",
                                       ReadOnlyStudRegField1 = s.ReadOnlyStudRegField1 ?? " ",
                                       ReadOnlyStudRegField2 = s.ReadOnlyStudRegField2 ?? " ",
                                       ReadOnlyStudRegField3 = s.ReadOnlyStudRegField3 ?? " ",
                                       ReadOnlyStudRegField4 = s.ReadOnlyStudRegField4 ?? " ",
                                       HiddenStudRegField1 = s.HiddenStudRegField1 ?? " ",
                                       HiddenStudRegField2 = s.HiddenStudRegField2 ?? " ",
                                       HiddenStudRegField3 = s.HiddenStudRegField3 ?? " ",
                                       HiddenStudRegField4 = s.HiddenStudRegField4 ?? " ",
                                       CustomCourseField1 = c.CustomCourseField1 ?? " ",
                                       CustomCourseField2 = c.CustomCourseField2 ?? " ",
                                       CustomCourseField3 = c.CustomCourseField3 ?? " ",
                                       CustomCourseField4 = c.CustomCourseField4 ?? " ",
                                       CustomCourseField5 = c.CustomCourseField5 ?? " ",

                                       username = s.USERNAME ?? " ",
                                       email = s.EMAIL ?? " ",
                                       address = s.ADDRESS ?? " ",
                                       city = s.CITY ?? " ",
                                       state = s.STATE ?? " ",
                                       zip = s.ZIP ?? " ",
                                       homephone = s.HOMEPHONE ?? " ",
                                       workphone = s.WORKPHONE ?? " ",
                                       fax = s.FAX ?? " ",
                                       ParentsName = s.parentsid.ToString() ?? " ",
                                       BlankTimeIn = "",
                                       BlankTimeOut = "",
                                       Note = cr.CustomSignInNotes ?? "",
                                       CheckOutNote = cr.CheckoutComments,
                                       CRExtraParticipant = "",
                                       StudentChoiceCourse = string.IsNullOrEmpty(cr.StudentChoiceCourse.ToString()) ? " " : db.CourseChoices.Where(cc => cc.CourseChoiceId == cr.StudentChoiceCourse).FirstOrDefault().CourseChoice1,
                                       rosterid = cr.RosterID,
                                       extraparticipants = cr.ExtraParticipant ?? " "
                                   }).ToList()
                                   .Select(x => new
                                   {
                                       x.roster_id,
                                       x.name,
                                       x.vdistrict,
                                       x.vschool,
                                       x.GRADELEVEL,
                                       x.StudRegField1,
                                       x.StudRegField2,
                                       x.StudRegField3,
                                       x.StudRegField4,
                                       x.StudRegField5,
                                       x.StudRegField6,
                                       x.StudRegField7,
                                       x.StudRegField8,
                                       x.StudRegField9,
                                       x.StudRegField10,
                                       x.StudRegField11,
                                       x.StudRegField12,
                                       x.StudRegField13,
                                       x.StudRegField14,
                                       x.StudRegField15,
                                       x.StudRegField16,
                                       x.StudRegField17,
                                       x.StudRegField18,
                                       x.StudRegField19,
                                       x.StudRegField20,
                                       x.ReadOnlyStudRegField1,
                                       x.ReadOnlyStudRegField2,
                                       x.ReadOnlyStudRegField3,
                                       x.ReadOnlyStudRegField4,
                                       x.HiddenStudRegField1,
                                       x.HiddenStudRegField2,
                                       x.HiddenStudRegField3,
                                       x.HiddenStudRegField4,
                                       x.CustomCourseField1,
                                       x.CustomCourseField2,
                                       x.CustomCourseField3,
                                       x.CustomCourseField4,
                                       x.CustomCourseField5,
                                       x.username,
                                       x.email,
                                       x.address,
                                       x.city,
                                       x.state,
                                       x.zip,
                                       x.homephone,
                                       x.workphone,
                                       x.fax,
                                       x.ParentsName,
                                       x.BlankTimeIn,
                                       x.BlankTimeOut,
                                       Note = string.IsNullOrEmpty(x.Note) ? x.CheckOutNote : x.Note,
                                       x.StudentChoiceCourse,
                                       CRExtraParticipant = CreateExtraParticipants(x.roster_id,x.extraparticipants),
                                       AmountOwe = "$ " + AmountOwe(x.roster_id, db).ToString("0.00")
                                   }); ;

                studentlist = studentlist.OrderBy(x => x.name).ToList();
                string combo1field = "";
                if (param.ContainsKey("combo1"))
                {
                    string value = param["combo1"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo1field = value.Split('|')[0];
                        }
                    }
                }
                string combo2field = "";
                if (param.ContainsKey("combo2"))
                {
                    string value = param["combo2"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo2field = value.Split('|')[0];
                        }
                    }
                }
                string combo3field = "";
                if (param.ContainsKey("combo3"))
                {
                    string value = param["combo3"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo3field = value.Split('|')[0];
                        }
                    }
                }
                string combo4field = "";
                if (param.ContainsKey("combo4"))
                {
                    string value = param["combo4"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo4field = value.Split('|')[0];
                        }
                    }
                }
                string combo5field = "";
                if (param.ContainsKey("combo5"))
                {
                    string value = param["combo5"];
                    if ((value != "") && (value != null))
                    {
                        if (value.Contains("|"))
                        {
                            combo5field = value.Split('|')[0];
                        }
                    }
                }

                foreach (var student in studentlist)
                {
                    SiginSheetFields SiginSheetFields = new SiginSheetFields();
                    SiginSheetFields.Name = student.name;
                    SiginSheetFields.Note = student.Note;
                    if (combo1field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field1 = student.GetType().GetProperty(combo1field).GetValue(student, null).ToString();

                            if ((combo1field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field1 = grade;
                                }

                            }

                            if ((combo1field.ToLower() == "vschool") && (SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field1 = school;
                                }

                            }

                            if ((combo1field.ToLower() == "vdistrict") && (SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var school = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field1 = school;
                                }

                            }

                        }
                        catch
                        {
                            SiginSheetFields.Field1 = student.GetType().GetProperty("GRADELEVEL").GetValue(student, null).ToString(); ;
                            if ((SiginSheetFields.Field1 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field1);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field1 = grade;
                                }

                            }

                        }
                    }
                    else
                    {
                        SiginSheetFields.Field1 = student.GetType().GetProperty("GRADELEVEL").GetValue(student, null).ToString(); ;
                        if ((SiginSheetFields.Field1 != ""))
                        {
                            int value = int.Parse(SiginSheetFields.Field1);
                            var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                            if (grade != null)
                            {
                                SiginSheetFields.Field1 = grade;
                            }

                        }
                    }
                    if (combo2field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field2 = student.GetType().GetProperty(combo2field).GetValue(student, null).ToString();
                            if ((combo2field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field2 = grade;
                                }

                            }

                            if ((combo2field.ToLower() == "vschool") && (SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field2 = school;
                                }

                            }

                            if ((combo2field.ToLower() == "vdistrict") && (SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var school = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field2 = school;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field2 = student.GetType().GetProperty("vschool").GetValue(student, null).ToString();
                            if ((SiginSheetFields.Field2 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field2);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field2 = school;
                                }

                            }
                        }
                    }
                    else
                    {
                        SiginSheetFields.Field2 = student.GetType().GetProperty("vschool").GetValue(student, null).ToString();
                        if ((SiginSheetFields.Field2 != ""))
                        {
                            int value = int.Parse(SiginSheetFields.Field2);
                            var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                            if (school != null)
                            {
                                SiginSheetFields.Field2 = school;
                            }

                        }
                    }
                    if (combo3field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field3 = student.GetType().GetProperty(combo3field).GetValue(student, null).ToString();

                            if ((combo3field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field3 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field3);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field3 = grade;
                                }

                            }

                            if ((combo3field.ToLower() == "vschool") && (SiginSheetFields.Field3 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field3);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field3 = school;
                                }

                            }

                            if ((combo3field.ToLower() == "vdistrict") && (SiginSheetFields.Field3 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field3);
                                var dist = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (dist != null)
                                {
                                    SiginSheetFields.Field3 = dist;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field3 = student.GetType().GetProperty("homephone").GetValue(student, null).ToString();

                        }
                    }
                    else
                    {
                        SiginSheetFields.Field3 = student.GetType().GetProperty("homephone").GetValue(student, null).ToString();

                    }

                    if (combo4field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field4 = student.GetType().GetProperty(combo4field).GetValue(student, null).ToString();
                            if ((combo4field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field4 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field4);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field4 = grade;
                                }

                            }

                            if ((combo4field.ToLower() == "vschool") && (SiginSheetFields.Field4 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field4);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field4 = school;
                                }

                            }

                            if ((combo4field.ToLower() == "vdistrict") && (SiginSheetFields.Field4 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field4);
                                var dist = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (dist != null)
                                {
                                    SiginSheetFields.Field4 = dist;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field4 = "";
                        }
                    }
                    else
                    {
                        SiginSheetFields.Field4 = "";
                    }
                    if (combo5field != "")
                    {
                        try
                        {
                            SiginSheetFields.Field5 = student.GetType().GetProperty(combo5field).GetValue(student, null).ToString();
                            if ((combo5field.ToUpper() == "GRADELEVEL") && (SiginSheetFields.Field5 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field5);
                                var grade = (from g in db.Grade_Levels where g.GRADEID == value select g.GRADE).FirstOrDefault();
                                if (grade != null)
                                {
                                    SiginSheetFields.Field5 = grade;
                                }

                            }

                            if ((combo5field.ToLower() == "vschool") && (SiginSheetFields.Field5 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field5);
                                var school = (from g in db.Schools where g.locationid == value select g.LOCATION).FirstOrDefault();
                                if (school != null)
                                {
                                    SiginSheetFields.Field5 = school;
                                }

                            }

                            if ((combo5field.ToLower() == "vdistrict") && (SiginSheetFields.Field5 != ""))
                            {
                                int value = int.Parse(SiginSheetFields.Field5);
                                var dist = (from g in db.Districts where g.DISTID == value select g.DISTRICT1).FirstOrDefault();
                                if (dist != null)
                                {
                                    SiginSheetFields.Field5 = dist;
                                }

                            }
                        }
                        catch
                        {
                            SiginSheetFields.Field5 = "";
                        }
                    }
                    else
                    {
                        SiginSheetFields.Field5 = "";
                    }

                    ListStudents.Add(SiginSheetFields);
                }
            }


            return ListStudents;
        }
        public static Page SetHeader(Page page, PDF pdf, int enrolled, int waiting, PDFHeaderFooterInfo pdfconfig, List<Course_Time> coursetimes, Course course, List<Instructor> Instructor, Dictionary<string, string> paramdictionary)
        {
            string credits = "";

            if (Settings.Instance.GetMasterInfo().DontDisplayCreditHours == 0)
            {
                if (course.CREDITHOURS > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo2().CreditHoursName + ": " + course.CREDITHOURS + " ";
                }
            }

            if ((Settings.Instance.GetMasterInfo().ShowInservice == 1) || (Settings.Instance.GetMasterInfo().ShowInservice == -1))
            {
                if (course.InserviceHours > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo2().InserviceHoursName + ": " + course.InserviceHours + " ";
                }
            }

            if ((Settings.Instance.GetMasterInfo2().ShowCustomCreditType == 1) || (Settings.Instance.GetMasterInfo2().ShowCustomCreditType == -1))
            {
                if (course.CustomCreditHours > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo2().CustomCreditTypeName + ": " + course.CustomCreditHours + " ";
                }
            }
            if ((Settings.Instance.GetMasterInfo2().ShowCEUandGraduateCreditCourses == 1) || (Settings.Instance.GetMasterInfo2().ShowCEUandGraduateCreditCourses == -1))
            {
                if (course.CEUCredit > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo2().CEUCreditLabel + ": " + course.CEUCredit + " ";
                }
                if (course.GraduateCredit > 0)
                {
                    credits = credits + "Graduate Credit" + ": " + course.GraduateCredit + " ";
                }
            }

            if ((Settings.Instance.GetMasterInfo3().OptionalCredithoursvisible1 == 1) || (Settings.Instance.GetMasterInfo3().OptionalCredithoursvisible1 == -1))
            {
                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1 != "" && course.Optionalcredithours1 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel1 + ": " + course.Optionalcredithours1 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2 != "" && course.Optionalcredithours2 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel2 + ": " + course.Optionalcredithours2 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3 != "" && course.Optionalcredithours3 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel3 + ": " + course.Optionalcredithours3 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4 != "" && course.Optionalcredithours4 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel4 + ": " + course.Optionalcredithours4 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5 != "" && course.Optionalcredithours5 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel5 + ": " + course.Optionalcredithours5 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6 != "" && course.Optionalcredithours6 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel6 + ": " + course.Optionalcredithours6 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7 != "" && course.Optionalcredithours7 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel7 + ": " + course.Optionalcredithours7 + " ";
                }

                if (Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8 != "" && course.Optionalcredithours8 > 0)
                {
                    credits = credits + Settings.Instance.GetMasterInfo3().OptionalcredithoursLabel8 + ": " + course.Optionalcredithours8 + " ";
                }
            }
            string image_name = paramdictionary["image"];
            if (string.IsNullOrEmpty(image_name))
            {
                image_name = pdfconfig.NewPDFLogoImage;
            }
            string orientaion = paramdictionary["RosterLayout"];
            string papersize = paramdictionary["papersize"];
            if ((papersize == null) || (papersize == ""))
            {
                papersize = "letter";
            }
            else
            {
                papersize = papersize.ToLower();
            }
            Font f0_bold = new Font(pdf, CoreFont.HELVETICA_BOLD);
            Font f0 = new Font(pdf, CoreFont.HELVETICA);
            Font f1 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            Font f2 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            Font f3 = new Font(pdf, CoreFont.HELVETICA);
            Font f4 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            Font policyfont = new Font(pdf, CoreFont.HELVETICA);
            f1.SetSize(12);
            f2.SetSize(9);
            f3.SetSize(9);
            f4.SetSize(18);
            f0.SetSize(9);
            f0_bold.SetSize(9);
            policyfont.SetSize(9);
            string message = "";
            string header1 = "";
            string header2 = "";

            if (pdfconfig != null)
            {
                message = pdfconfig.signinsheetpdfmessage;
                header1 = pdfconfig.signinsheetpdftitle;
                header2 = pdfconfig.signinsheetpdftitle2;

            }

            policy_char_endposition = message.Count() / 135;


            Paragraph title2 = new Paragraph();
            title2.SetAlignment(Align.LEFT);
            if (header2 == "")
            {
                header2 = " ";
            }
            title2.Add(new TextLine(f2, header2));
            TextColumn column2 = new TextColumn(f2);
            column2.SetLineBetweenParagraphs(true);
            column2.SetSpaceBetweenLines(20);

            column2.AddParagraph(title2);
            if (orientaion == "1")
            {
                column2.SetPosition(200, 35);
                column2.SetSize(300, 200.0);
            }
            else
            {
                column2.SetPosition(200, 35);
                column2.SetSize(800, 200.0);
                //column2.SetAlignment(Align.RIGHT);
            }

            Point point2 = column2.DrawOn(page);

            Paragraph title = new Paragraph();
            title.SetAlignment(Align.LEFT);
            title.Add(new TextLine(f2, header1));
            TextColumn column = new TextColumn(f2);
            column.SetSpaceBetweenLines(20);

            column.AddParagraph(title);
            if (orientaion == "1")
            {
                column.SetPosition(200, 20);
            }

            else
            {
                column.SetPosition(200, 20);
            }
            column.SetSize(200, 100);
            Point point = column.DrawOn(page);
            column.SetSize(200, 100);



            Paragraph title3 = new Paragraph();
            title3.SetAlignment(Align.RIGHT);
            title3.Add(new TextLine(f1, "Sign-in Sheet"));
            TextColumn column3 = new TextColumn(f1);
            column3.SetSpaceBetweenLines(20);

            column3.AddParagraph(title3);
            if ((papersize == "letter") || (papersize == "null"))
            {
                if (orientaion == "1")
                {
                    column3.SetPosition(538, 33);
                }

                else
                {
                    column3.SetPosition(378, 33);
                }
            }
            else
            {
                if (orientaion == "1")
                {
                    column3.SetPosition(780, 33);
                }

                else
                {
                    column3.SetPosition(378, 33);
                }
            }
            column3.SetSize(200, 100);
            Point point3 = column3.DrawOn(page);
            column3.SetSize(200, 100);

            var t = new TextLine(policyfont, message);
            Paragraph policy = new Paragraph();
            policy.SetAlignment(Align.LEFT);
            policy.Add(new TextLine(policyfont, message));
            TextColumn policies = new TextColumn(f1);
            policies.AddParagraph(policy);
            policies.SetPosition(30, 145);
            if ((papersize == "letter") || (papersize == "null"))
            {
                if (orientaion == "1")
                {
                    policy_char_endposition = message.Count() / 175;
                    policies.SetSize(680, 100.0);
                }
                else
                {
                    policy_char_endposition = message.Count() / 135;
                    policies.SetSize(530, 100.0);
                }
            }
            else
            {
                if (orientaion == "1")
                {
                    policy_char_endposition = message.Count() / 320;
                    policies.SetSize(950, 100.0);
                }
                else
                {
                    policy_char_endposition = message.Count() / 135;
                    policies.SetSize(530, 100.0);
                }
            }
            Point point4 = policies.DrawOn(page);
            policies.SetSize(470.0, 100.0);



            if (course != null)
            {
                Table table_coursedetails = new Table();
                List<List<Cell>> tablecourseData = new List<List<Cell>>();
                List<Cell> row = new List<Cell>();
                Cell coursename2 = new Cell(f1, course.COURSENAME);
                row.Add(coursename2);
                tablecourseData.Add(row);
                row = new List<Cell>();
                Cell courseenrolled2 = null;
                if (waiting > 0)
                {
                    courseenrolled2 = new Cell(f0, enrolled.ToString() + " Enrolled" + " / " + waiting.ToString() + " Waiting");
                }
                else
                {
                    courseenrolled2 = new Cell(f0, enrolled.ToString() + " Enrolled");
                }
                Cell coursenum2 = new Cell(f0, course.COURSENUM);
                coursenum2.SetTextAlignment(Align.LEFT);
                row.Add(coursenum2);
                row.Add(courseenrolled2);

                tablecourseData.Add(row);
                row = new List<Cell>();
                if (credits != "")
                {
                    Cell coursecredit = new Cell(f0, credits);
                    row.Add(coursecredit);
                    tablecourseData.Add(row);
                }
                row = new List<Cell>();
                string times = "";
                string date = paramdictionary["signindatevalue"];

                if (date != null)
                {
                    date = date.Replace("[\"", "").Replace("\"]", "");
                    if (date.Contains("\",\""))
                    {
                        date = date.Replace("\",\"", " ");
                    }
                    else
                    {
                        if (!((date == "[]") || (date == "")))
                        {
                            date = date + " " + "[time]";
                        }
                    }

                }
                DateTime temp;
                foreach (var dateandtime in coursetimes)
                {
                    if (dateandtime.STARTTIME.HasValue && dateandtime.FINISHTIME.HasValue)
                    {
                        if ((date == null) || (date == "[]") || (date == ""))
                        {
                            times = dateandtime.STARTTIME.Value.ToString("t") + " - " + dateandtime.FINISHTIME.Value.ToString("t");
                        }
                        else
                        {
                            if (DateTime.TryParse(date.Replace("[time]","").Trim(), out temp))
                            {
                                if (dateandtime.COURSEDATE == temp)
                                {
                                    times = dateandtime.STARTTIME.Value.ToString("t") + " - " + dateandtime.FINISHTIME.Value.ToString("t");
                                }
                            }
                        }

                    }
                    if ((date == null) || (date == "[]") || (date == ""))
                    {
                        date = dateandtime.COURSEDATE.Value.ToString("d") + " " + times;
                    }
                }

                date = date.Replace("[time]", times);


                Cell coursedate2 = new Cell(f0, date.Replace('"', ' '));
                row.Add(coursedate2);
                tablecourseData.Add(row);
                row = new List<Cell>();

                string location = course.LOCATION;
                if ((course.STREET != "") && (course.STREET != null))
                {
                    location = location + ", " + course.STREET;
                }
                if ((course.CITY != "") && (course.CITY != null))
                {
                    location = location + ", " + course.CITY;
                }
                if ((course.STATE != "") && (course.STATE != null))
                {
                    location = location + ", " + course.STATE;
                }
                if ((course.ZIP != "") && (course.ZIP != null))
                {
                    location = location + ", " + course.ZIP;
                }
                Cell courseloc2 = new Cell(f0, location);
                row.Add(courseloc2);
                tablecourseData.Add(row);
                row = new List<Cell>();

                //Instructors
                string Instructors = "";
                string insts = "";
                int numofinstructor = 0;

                foreach (var inst_Name in Instructor)
                {

                    Instructors = "";
                    if ((inst_Name.FIRST != null) && (inst_Name.FIRST != ""))
                    {
                        Instructors = inst_Name.FIRST + " " + inst_Name.LAST;

                    }
                    if (insts == "")
                    {
                        if (Instructors != "")
                        {

                            insts = insts + Instructors;

                        }
                    }
                    else
                    {
                        if (Instructors != "")
                        {

                            insts = insts + " , " + Instructors;

                        }
                    }

                    numofinstructor = numofinstructor + 1;
                }
                if (numofinstructor > 1)
                {
                    insts = Gsmu.Api.Data.School.Terminology.TerminologyHelper.Instance.GetTermCapital(Gsmu.Api.Data.School.Terminology.TermsEnum.Instructors) + ": " + insts;
                }
                else
                {
                    insts = Gsmu.Api.Data.School.Terminology.TerminologyHelper.Instance.GetTermCapital(Gsmu.Api.Data.School.Terminology.TermsEnum.Instructor) + ": " + insts;
                }

                Cell courseInstructors = new Cell(f0, insts);
                row.Add(courseInstructors);
                tablecourseData.Add(row);
                row = new List<Cell>();

                //EVENT
                string space = " ";
                if (orientaion == "1")
                {
                    for (int x = 0; x < 184; x++)
                    {
                        space = space + " ";
                    }
                }
                else
                {
                    for (int x = 0; x < 118; x++)
                    {
                        space = space + " ";
                    }
                }
                if (string.IsNullOrEmpty(course.EVENTNUM) != true)
                {
                    Cell eventnum = new Cell(f0, "Event #: " + course.EVENTNUM);
                    row.Add(eventnum);
                    tablecourseData.Add(row);
                    row = new List<Cell>();
                }
                if (string.IsNullOrEmpty(course.EVENTNUM) != true)
                {
                    Cell accountnum = new Cell(f0, "Account #: " + course.ACCOUNTNUM);
                    row.Add(accountnum);
                    tablecourseData.Add(row);
                    row = new List<Cell>();
                }

                table_coursedetails.SetData(tablecourseData, 0);
                table_coursedetails.SetNoCellBorders();
                if (orientaion == "1")
                {

                    table_coursedetails.SetPosition(200, 50);
                    table_coursedetails.SetColumnWidth(1, 240);
                }

                else
                {
                    table_coursedetails.SetPosition(200, 50);
                    table_coursedetails.SetColumnWidth(1, 180);
                }

                table_coursedetails.DrawOn(page);

                Box box = new Box();
                box.SetColor(Color.red);
                box.SetPosition(700, 160);
                box.DrawOn(page);

            }

            if (image_name != null)
            {
                string fileName = System.Web.HttpContext.Current.Server.MapPath("~/Areas/Public/Images/" + image_name.ToString());
                if (!File.Exists(fileName))
                {
                    fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/Images/" + image_name.ToString());
                    if (!File.Exists(fileName))
                    {
                        fileName = System.Web.HttpContext.Current.Server.MapPath("~/old/Images/" + image_name.ToString());
                    }
                    if (!File.Exists(fileName))
                    {
                        fileName = System.Web.HttpContext.Current.Server.MapPath("~/admin/Images/signinsheet/" + image_name.ToString());
                    }
                    if (!File.Exists(fileName))
                    {
                        fileName = System.Web.HttpContext.Current.Server.MapPath("~/old/Images/signinsheet/" + image_name.ToString());
                    }
                }
                if (File.Exists(fileName))
                {
                    string extension = System.IO.Path.GetExtension(fileName).ToLower();
                    FileStream fis1 = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    System.Type type = fis1.GetType();
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
                        Image image1 = new Image(pdf, fis1, imagetype);
                        image1.GetWidth();
                        image1.GetHeight();
                        image1.SetPosition(35, 30);
                        if (orientaion == "1")
                        {
                            image1.ScaleBy(0.23f, 0.20f);
                        }
                        else
                        {
                            image1.ScaleBy(0.23f, 0.20f);
                        }
                        image1.DrawOn(page);
                    }
                }



            }


            return page;
        }
        private static string CreateExtraParticipants(int roster_id,string RosterExtraparticipants="")
        {
            string dom = string.Empty;
            using (var db = new SchoolEntities())
            {
                var data = (from cep in db.CourseExtraParticipants
                            where cep.RosterId == roster_id
                            select new
                            {
                                Id = cep.CourseExtraParticipantId,
                                FirstName = cep.StudentFirst,
                                LastName = cep.StudentLast,
                                Email = cep.StudentEmail,
                                CustomField = cep.CustomField2
                            }).ToList();
                if (data.Count() > 0)
                {
                    int count = 1;
                    foreach (var p in data)
                    {
                        dom += count + " " + p.FirstName + " " + p.LastName + ",";
                        count++;
                    }
                }
            }
            if (RosterExtraparticipants.Trim() != "")
            {
                dom = dom + " " + "[skipnumbering]" + RosterExtraparticipants;
            }
            return dom;
        }
        //@TODO : Transfer to a common class that can also be accessed across the application
        //would be nice if this is added to Commerce Folder on API
        private static decimal AmountOwe(int rosterId, SchoolEntities db)
        {
            //CASE WHEN (CAST(CourseCost as smallmoney) + (SELECT sum(rm.price) FROM rostermaterials rm WHERE rm.rosterid = cr.rosterid)) - CAST(TotalPaid as smallmoney) < 0 OR couponcode!='' THEN 0 ELSE CASE WHEN paidinfull != 0 THEN CAST(TotalPaid as smallmoney) - (CAST(CourseCost as smallmoney) + (SELECT sum(rm.price) FROM rostermaterials rm WHERE rm.rosterid = cr.rosterid)) ELSE CASE WHEN CAST(TotalPaid as smallmoney) > 0 THEN CAST(TotalPaid as smallmoney) - (CAST(CourseCost as smallmoney) + (SELECT sum(rm.price) FROM rostermaterials rm WHERE rm.rosterid = cr.rosterid)) ELSE (CAST(CourseCost as smallmoney) + (SELECT sum(rm.price) FROM rostermaterials rm WHERE rm.rosterid = cr.rosterid)) END END END
            var roster = db.Course_Rosters.Where(cr => cr.RosterID == rosterId).SingleOrDefault();
            var materials = db.rostermaterials.Where(rm => rm.RosterID == rosterId).ToList();
            decimal courseCost = string.IsNullOrEmpty(roster.CourseCost) ? 0 : decimal.Parse(roster.CourseCost.Replace("$", ""));
            float materialsCost = materials.Count() > 0 ? materials.Sum(rm => rm.price.HasValue ? rm.price.Value : 0) : 0;
            decimal totalPaid = roster.TotalPaid.HasValue ? roster.TotalPaid.Value : 0;
            decimal totalCost = courseCost + decimal.Parse(materialsCost.ToString());
            decimal amountOweRaw = (totalPaid - totalCost);
            decimal amountOwe = (amountOweRaw == 0 || roster.CouponCode != "" || roster.PaidInFull != 0) ? 0 : totalPaid == 0 ? totalCost : amountOweRaw < 0 ? amountOweRaw * -1 : amountOweRaw;
            return Decimal.Parse(amountOwe.ToString("0.00"));
        }
    }
}
public class SiginSheetFields
{
    public string Name { get; set; }
    public string Note { get; set; }
    public string Field1 { get; set; }
    public string Field2 { get; set; }
    public string Field3 { get; set; }
    public string Field4 { get; set; }
    public string Field5 { get; set; }
    public string CRExtraParticipant { get; set; }

}
