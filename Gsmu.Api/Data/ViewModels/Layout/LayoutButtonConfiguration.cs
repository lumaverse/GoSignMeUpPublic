using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    /*
    Enrolled = Settings.Instance.GetMasterInfo4().PublicButtonLabelEnrolled
    AddToCart = Settings.Instance.GetMasterInfo4().PublicButtonLabelAddToCart
    Checkout = Settings.Instance.GetMasterInfo4().PublicButtonLabelCheckout
    Class full
    Wait space available
    Empty Cart
    Login
    Create account
    Search ...
     */
    public class LayoutButtonConfiguration
    {
        public LayoutButtonConfiguration()
        {

            ClassFull = "Class full";
            WaitSpaceAvailable = "Wait space available";
            EmptyCart = "Empty cart";
            Login = "Login";
            CreateAccount = "Create account";
            RegisterInstructor = "Register Instructor";
            Search = "Search";
            ClosedEnrollment = "Closed Enrollment";
            Checkout = "Checkout";
            Enrolled = "Enrolled";
            Confirmation = "Confirmation";
            AddToCart = "Add to cart";
            Shibb_Login = "Common Login";
            Cas_Login = "CAS Login";
            Canvas_Login = "Canvas Login";
            OnWaitList = "On Wait List";
            SearchFrom  = "From";
            SearchTo  = "Until";
            ContinueShoppingCourse = "Continue Shopping for Courses";
        }

        public string ClassFull { get; set; }
        public string WaitSpaceAvailable { get; set; }
        public string EmptyCart { get; set; }
        public string Login { get; set; }
        public string CreateAccount { get; set; }
        public string Search { get; set; }
        public string RegisterInstructor { get; set; }
        public string ClosedEnrollment { get; set; }
        public string AddToCart { get; set; }
        public string Confirmation { get; set; }
        public string Enrolled { get; set; }
        public string Checkout { get; set; }
        public string Shibb_Login { get; set; }
        public string Cas_Login { get; set; }
        public string Canvas_Login { get; set; }
        public string OnWaitList { get; set; }
        public string SearchFrom { get; set; }
        public string SearchTo { get; set; }
        public string ContinueShoppingCourse { get; set; }
    }
}
