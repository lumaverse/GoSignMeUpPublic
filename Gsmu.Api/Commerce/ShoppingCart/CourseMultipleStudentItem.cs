using Gsmu.Api.Data.School.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Commerce.ShoppingCart
{
    public class CourseMultipleStudentItem
    {
        public int StudentId
        {
            get;
            set;
        }
        public int CourseId
        {
            get;
            set;
        }

        public string CourseName
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
        public string OrderNumber
        {
            get;
            set;
        }
        public PricingModel PricingModel
        {
            get;
            set;
        }

        public decimal CourseTotal
        {
            get;
            set;
        }
        public int useAdminPricing
        {
            get;
            set;
        }
        public short IsWaiting
        {
            get;
            set;
        }
        public short DISTEMPLOYEE
        {
            get;
            set;
        }

        public string DiscountAmountPerCourse
        {
            get;
            set;
        }

        public string DiscountCouponPerCourse
        {
            get;
            set;
        }

    }
}
