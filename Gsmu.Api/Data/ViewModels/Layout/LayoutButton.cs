using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public enum LayoutButton
    {
        // static db values
        AddToCart,
        Checkout,
        Enrolled,
        Confirmation,
        MultipleEnrollment,
        // layout configuration json object
        ClassFull,
        WaitSpaceAvailable,
        EmptyCart,
        Login,
        CreateAccount,
        RegisterInstructor,
        Search,
        ClosedEnrollment,
        Shibb_Login,
        Cas_Login,
        Canvas_Login,
        OnWaitList,
        SearchFrom,
        SearchTo,
        ContinueShoppingCourse
    }

    public enum BGColor
    {
        // background

        clogheaderbg,
        clogheaderbar,
        clogheaderbartext,
        clogshowallbg,
        clogshowalltext,
        clogmaincatbg,
        //clogmaincatactive,
        clogmaincattext,
        clogsubcatbg,
        clogsubcatactive,
        clogsubcattext,
        clogsub2catbg,
        clogsub2catactive,
        clogsub2cattext
        //clogcoursetext,
        //clogleftnavbg,
        //clogspecialmsgcolor,
        //clogimplinkcolor,
        //CatelogCurrentCourseColor

    }


    public class BGColorInfo
    {
        public string color { get; set; }

        public string Title { get; set; }

        public string field { get; set; }
    }

}
