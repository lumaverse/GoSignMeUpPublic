using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.Student
{
    public class UserStudentModel
    {
        public string last { get; set; }

        public string first { get; set; }

        public string email { get; set; }

        public string id { get; set; }

        public string username { get; set; }

        public string pw { get; set; }
    }

    public class UserAffiliationModel
    {
        public int school { get; set; }

        public int district { get; set; }

        public int grade { get; set; }

        public string gradetext { get; set; }

        public string schooltext { get; set; }

        public string districttext { get; set; }
    }

    public class UserCourseModel
    {

        public string coursename { get; set; }

        public int courseid { get; set; }

        public decimal totalpaid { get; set; }

        public string coursedate { get; set; }

        public string cancel { get; set; }

        public string transcriptcount { get; set; }
    }

    public class CertificateListModel
    {
        public string expiredate { get; set; }

        public int certificationsid { get; set; }

        public int certificationsstudentid { get; set; }

        public string certtitle { get; set; }
    }

    public class CustomFieldListtModel
    {
        public string address { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string country { get; set; }

        public string shipaddress { get; set; }

        public string shipcity { get; set; }

        public string shipstate { get; set; }

        public string shipzip { get; set; }

        public string shipcountry { get; set; }

        public int visiblestudentaddress { get; set; }

        public int visiblestudentcity { get; set; }

        public int visiblestudentstate { get; set; }

        public int visiblestudentzip { get; set; }

        public int visiblestudentcountry { get; set; }

        public int usefedex { get; set; }

        public string studregfield1 { get; set; }
        public string studregfield2 { get; set; }
        public string studregfield3 { get; set; }
        public string studregfield4 { get; set; }
        public string studregfield5 { get; set; }
        public string studregfield6 { get; set; }
        public string studregfield7 { get; set; }
        public string studregfield8 { get; set; }
        public string studregfield9 { get; set; }
        public string studregfield10 { get; set; }
        public string studregfield11 { get; set; }
        public string studregfield12 { get; set; }
        public string studregfield13 { get; set; }
        public string studregfield14 { get; set; }
        public string studregfield15 { get; set; }
        public string studregfield16 { get; set; }
        public string studregfield17 { get; set; }
        public string studregfield18 { get; set; }
        public string studregfield19 { get; set; }
        public string studregfield20 { get; set; }

        public string studregfield1name { get; set; }
        public string studregfield2name { get; set; }
        public string studregfield3name { get; set; }
        public string studregfield4name { get; set; }
        public string studregfield5name { get; set; }
        public string studregfield6name { get; set; }
        public string studregfield7name { get; set; }
        public string studregfield8name { get; set; }
        public string studregfield9name { get; set; }
        public string studregfield10name { get; set; }
        public string studregfield11name { get; set; }
        public string studregfield12name { get; set; }
        public string studregfield13name { get; set; }
        public string studregfield14name { get; set; }
        public string studregfield15name { get; set; }
        public string studregfield16name { get; set; }
        public string studregfield17name { get; set; }
        public string studregfield18name { get; set; }
        public string studregfield19name { get; set; }
        public string studregfield20name { get; set; }

        public string masknumber1 { get; set; }
        public string masknumber2 { get; set; }
        public string masknumber3 { get; set; }
        public string masknumber4 { get; set; }
        public string masknumber5 { get; set; }
        public string masknumber6 { get; set; }
        public string masknumber7 { get; set; }
        public string masknumber8 { get; set; }
        public string masknumber9 { get; set; }
        public string masknumber10 { get; set; }
        public string masknumber11 { get; set; }
        public string masknumber12 { get; set; }
        public string masknumber13 { get; set; }
        public string masknumber14 { get; set; }
        public string masknumber15 { get; set; }
        public string masknumber16 { get; set; }
        public string masknumber17 { get; set; }
        public string masknumber18 { get; set; }
        public string masknumber19 { get; set; }
        public string masknumber20 { get; set; }

        public string hiddenstudregfield1 { get; set; }
        public string hiddenstudregfield2 { get; set; }
        public string hiddenstudregfield3 { get; set; }
        public string hiddenstudregfield4 { get; set; }

        public string readonlystudregfield1 { get; set; }
        public string readonlystudregfield2 { get; set; }
        public string readonlystudregfield3 { get; set; }
        public string readonlystudregfield4 { get; set; }

        public string hiddenstudregfield1name { get; set; }
        public string hiddenstudregfield2name { get; set; }
        public string hiddenstudregfield3name { get; set; }
        public string hiddenstudregfield4name { get; set; }

        public string readonlystudregfield1name { get; set; }
        public string readonlystudregfield2name { get; set; }
        public string readonlystudregfield3name { get; set; }
        public string readonlystudregfield4name { get; set; }


        public string studregfield1required { get; set; }
        public string studregfield2required { get; set; }
        public string studregfield3required { get; set; }
        public string studregfield4required { get; set; }
        public string studregfield5required { get; set; }
        public string studregfield6required { get; set; }
        public string studregfield7required { get; set; }
        public string studregfield8required { get; set; }
        public string studregfield9required { get; set; }
        public string studregfield10required { get; set; }
        public string studregfield11required { get; set; }
        public string studregfield12required { get; set; }
        public string studregfield13required { get; set; }
        public string studregfield14required { get; set; }
        public string studregfield15required { get; set; }
        public string studregfield16required { get; set; }
        public string studregfield17required { get; set; }
        public string studregfield18required { get; set; }
        public string studregfield19required { get; set; }
        public string studregfield20required { get; set; }

        public string FieldSetting1 { get; set; }
        public string FieldSetting2 { get; set; }
        public string FieldSetting3 { get; set; }
        public string FieldSetting4 { get; set; }
        public string FieldSetting5 { get; set; }
        public string FieldSetting6 { get; set; }
        public string FieldSetting7 { get; set; }
        public string FieldSetting8 { get; set; }
        public string FieldSetting9 { get; set; }
        public string FieldSetting10 { get; set; }
        public string FieldSetting11 { get; set; }
        public string FieldSetting12 { get; set; }
        public string FieldSetting13 { get; set; }
        public string FieldSetting14 { get; set; }
        public string FieldSetting15 { get; set; }
        public string FieldSetting16 { get; set; }
        public string FieldSetting17 { get; set; }
        public string FieldSetting18 { get; set; }
        public string FieldSetting19 { get; set; }
        public string FieldSetting20 { get; set; }

        public string FieldSetting21 { get; set; }
        public string FieldSetting22 { get; set; }
        public string FieldSetting23 { get; set; }
        public string FieldSetting24 { get; set; }
        public string FieldSetting25 { get; set; }
        public string FieldSetting26 { get; set; }
        public string FieldSetting27 { get; set; }
        public string FieldSetting28 { get; set; }
        public string FieldSetting29 { get; set; }
        public string FieldSetting30 { get; set; }

        public string FieldListValue1 { get; set; }
        public string FieldListValue2 { get; set; }
        public string FieldListValue3 { get; set; }
        public string FieldListValue4 { get; set; }
        public string FieldListValue5 { get; set; }
        public string FieldListValue6 { get; set; }
        public string FieldListValue7 { get; set; }
        public string FieldListValue8 { get; set; }
        public string FieldListValue9 { get; set; }
        public string FieldListValue10 { get; set; }
        public string FieldListValue11 { get; set; }
        public string FieldListValue12 { get; set; }
        public string FieldListValue13 { get; set; }
        public string FieldListValue14 { get; set; }
        public string FieldListValue15 { get; set; }
        public string FieldListValue16 { get; set; }
        public string FieldListValue17 { get; set; }
        public string FieldListValue18 { get; set; }
        public string FieldListValue19 { get; set; }
        public string FieldListValue20 { get; set; }

        public string mstrInfoID { get; set; }

        public string zip { get; set; }


        public string homephone { get; set; }

        public string workphone { get; set; }

        public string fax { get; set; }

        public short distemployee { get; set; }

        public string ss { get; set; }
    }

    public class SenchafilterResponse
    {
        public string coursetyp { get; set; }

    }

    public class RecoverInfo
    {
        public bool success { get; set; }

        public string msg { get; set; }

    }
}
