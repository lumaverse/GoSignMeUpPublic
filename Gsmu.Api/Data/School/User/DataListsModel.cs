using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.User
{
    public class Gender
    {
        public string vlu { get; set; }

        public string txt { get; set; }

    }

    public class YesNo
    {
        public string vlu { get; set; }

        public string txt { get; set; }
    }

    public class Ethnicity1
    {
        public string vlu { get; set; }

        public string txt { get; set; }
    }

    public class Ethnicity2
    {
        public string vlu { get; set; }

        public string txt { get; set; }
    }

    public class Race
    {
        public string vlu { get; set; }

        public string txt { get; set; }
    }


    public class Distemployee
    {
        public string vlu { get; set; }

        public string txt { get; set; }
    }

    public class State
    {
        public string vlu { get; set; }

        public string txt { get; set; }
    }

    public class AllStudentUserField
    {
        public string FieldName { get; set; }

        public string FieldLabel { get; set; }

        public string FieldGrp { get; set; }

        public int MaskNum { get; set; }

        public int DefaultMaskNumber { get; set; }

        public string MaskTxt { get; set; }

        public int FieldRequired { get; set; }

        public int? FieldListType { get; set; }

        public string FieldCustomList { get; set; }

        public bool BoolFieldRequired { get; set; }

        public bool AllowBlankBool { get; set; }

        public bool FieldVisible { get; set; }

        public string FieldStore { get; set; }

        public bool FieldForceSelection { get; set; }
    }

    public class UserRegFieldSpecs
    {
        public string FieldName { get; set; }

        public string FieldLabel { get; set; }

        public string FieldGrp { get; set; }

        public int MaskNum { get; set; }

        public int DefaultMaskNumber { get; set; }

        public string MaskTxt { get; set; }

        public int FieldRequired { get; set; }

        public int? FieldListType { get; set; }

        public string FieldCustomList { get; set; }

        public bool BoolFieldRequired { get; set; }

        public bool AllowBlankBool { get; set; }

        public bool FieldVisible { get; set; }

        public string FieldStore { get; set; }

        public bool FieldForceSelection { get; set; }

        public string TblFieldName { get; set; }

        public bool FieldReadOnly { get; set; }

        public bool ConfirmRequired { get; set; }

        public string FieldListValue { get; set; }

        public bool BoolFieldRequiredAll { get; set; } // Settings to Set All required in Course Requirement (use by School field)

        public string DefaultValue { get; set; }

        public string StudentSupervisorFieldType { get; set; }
    }



    public class Grade_LevelForFilter
    {
        public string GRADE { get; set; }

        public int GRADEID { get; set; }

        public int? GradeSortOrder { get; set; }

        public int? SCHOOLID { get; set; }

        public string SCHOOL { get; set; }
    }

    public class UserQueryListModel
    {
 
        public string last { get; set; }

        public string first { get; set; }

        public string username { get; set; }


        public int userid { get; set; }

        public string email { get; set; }

        public DateTime? dateadded { get; set; }
        public DateTime? date_modified { get; set; }
        public DateTime? date_bb_integrated { get; set; }
    }


    public class EmailRestriction
    {
        public int Count { get; set; }

        public int OnOff { get; set; }

        public int ShowList { get; set; }

        public string EmailNotification { get; set; }

        public List<EmailRestrictionData> Data { get; set; }
    }

    public class EmailRestrictionData
    {
        public int id { get; set; }

        public string email { get; set; }

        public string grp { get; set; }
    }

}
