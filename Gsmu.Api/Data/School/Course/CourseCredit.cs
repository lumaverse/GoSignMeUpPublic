using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Course
{
    public class CourseCredit
    {

        public CourseCredit(CourseCreditType type, double credit, string label)
        {
            CourseCreditType = type;
            Credit = credit;
            Label = label;
        }

        public CourseCreditType CourseCreditType
        {
            get;
            private set;
        }

        public double Credit
        {
            get;
            private set;
        }

        public string Label
        {
            get;
            private set;
        }
    }
}
