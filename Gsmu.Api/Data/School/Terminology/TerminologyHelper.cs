using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.Terminology
{
    public class TerminologyHelper
    {
        public static TerminologyHelper Instance
        {
            get
            {
                var instance = ObjectHelper.GetRequestObject<TerminologyHelper>(WebContextObject.Terminology);
                if (instance == null)
                {
                    ObjectHelper.SetRequestObject<TerminologyHelper>(WebContextObject.Terminology, new TerminologyHelper());
                    return Instance;
                }
                return instance;
            }
        }

        public static void Refresh()
        {
            ObjectHelper.ClearRequestObject(WebContextObject.Terminology);
        }

        private StringDictionary Defaults = new StringDictionary();

        private TerminologyHelper() 
        {
            InitializeDefaults();
            FixConfigBasedOnV3();
            FillDefaultsWithDatabaseValues();
        }

        /*
        function fixterminology
            dim term
            dim rs

            objconn.begintrans
	        for zzzz = 0 to ubound(Terminology)-1 step 2
                term = Terminology(zzzz)
                set rs = objconn.execute("select * from terminology where lower(terminologybaselabel) = '" & lcase(term) & "'")
                if rs.eof then
                    rs.close
                    objconn.execute("INSERT INTO terminology (terminologybaselabel, terminologyaltlabel) values ('" & term & "', '" & term & "')")
                else
                    rs.close
                    objconn.execute("update terminology set terminologyaltlabel = '" & term  & "' where lower(terminologybaselabel) = '" & lcase(term) & "'")
                end if
	        next

            objconn.execute "update masterinfo2 set publicterminology = 1"

            objconn.committrans
            publicterminology = 1
        end function
         */
        private void FixConfigBasedOnV3()
        {
            var publicTerminology = Settings.Instance.GetMasterInfo2().PublicTerminology;
            if (publicTerminology.HasValue || publicTerminology.Value == 1) {
                return;
            }

            using (var db = new SchoolEntities())
            {
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    foreach (string term in Defaults.Keys)
                    {
                        var dbterm = (from t in db.Terminologies where t.TerminologyBaseLabel.ToLower() == term select t).FirstOrDefault();
                        if (dbterm == null)
                        {
                            dbterm = new Entities.Terminology();
                            dbterm.TerminologyAltLabel = term;
                            dbterm.TerminologyBaseLabel = term;
                            db.Terminologies.Add(dbterm);
                        }
                        else
                        {
                            dbterm.TerminologyAltLabel = term;
                        }                        
                    }

                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("UPDATE MasterInfo2 SET PublicTerminology = 1");

                    transaction.Complete();
                }
            }
        }

        private void FillDefaultsWithDatabaseValues()
        {
            using (var db = new SchoolEntities())
            {
                var terms = (from t in db.Terminologies select t).ToDictionary(t => t.TerminologyBaseLabel.ToLower(), t => t.TerminologyAltLabel);

                foreach (var label in terms.Keys)
                {
                    if (Defaults.ContainsKey(label) && !string.IsNullOrWhiteSpace(terms[label]))
                    {
                        Defaults[label] = terms[label];
                    }
                }
            }
        }

        private void InitializeDefaults()
        {
            Defaults.Add("Transcript".ToLower() ,"Record");
            Defaults.Add("Transcribe".ToLower(), "Record");
            Defaults.Add("Transcripted".ToLower(), "Recorded");
            Defaults.Add("Transcripts".ToLower(), "Records");
            Defaults.Add("Take".ToLower(), "Record");
            Defaults.Add("Enroll".ToLower(), "Register");
            Defaults.Add("Enrolled".ToLower(), "Registered");
            Defaults.Add("Enrolls".ToLower(), "Registers");
            Defaults.Add("Enrollment".ToLower(), "Registration");
            Defaults.Add("Supervisor".ToLower(), "Training Coordinator");
            Defaults.Add("Supervisors".ToLower(), "Training Coordinators");
            Defaults.Add("Instructors".ToLower(), "Workshop Facilitators");
            Defaults.Add("Instructor".ToLower(), "Workshop Facilitator");
            Defaults.Add("Student".ToLower(), "Student");
            Defaults.Add("Students".ToLower(), "Students");
            Defaults.Add("Material".ToLower(), "Material");
            Defaults.Add("Materials".ToLower(), "Materials");
            Defaults.Add("Receipt".ToLower(), "Receipt");
            Defaults.Add("Course".ToLower(), "Course(s)");
        }

        public string GetTermLower(TermsEnum term)
        {
            var value = term.ToString().ToLower();
            return Defaults[value].ToLower();
        }

        public string GetTermUpper(TermsEnum term)
        {
            var value = term.ToString().ToLower();
            return Defaults[value].ToUpper();
        }

        public string GetTermCapital(TermsEnum term)
        {
            //var value = term.ToString().ToLower();
            //var word = Defaults[value].Trim().ToLower();
            //return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
            //It will only capitalized the first letter and will not format the succeeding letters or words
            var value = term.ToString().ToLower();
            var word = Defaults[value].Trim();
            return word.Substring(0, 1).ToUpper() + word.Substring(1); 
        }

        public string GetTermCapitalize(TermsEnum term)
        {
            ///capitalizes the first letter of every word in a string
            var value = term.ToString().ToLower();
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Defaults[value].Trim());
        }
        public StringDictionary Terms
        {
            get
            {
                return Defaults;
            }
        }

    }
}
