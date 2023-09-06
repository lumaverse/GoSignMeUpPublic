using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.Survey
{
    public class SurveyModel
    {
        public string  SurveyName { get; set; }
        public string CourseName { get; set; }
        public string CourseDate { get; set; }
        public string SurveyId { get; set; }
        public int CourseId { get; set; }
    }
}
