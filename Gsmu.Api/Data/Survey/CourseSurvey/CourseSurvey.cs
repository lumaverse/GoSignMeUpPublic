using Gsmu.Api.Data.Survey.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.Survey.CourseSurvey
{
    class CourseSurvey
    {
        public CourseSurvey() { }
        public CourseSurvey(int intcourseid)
        {
            using (var db = new SurveyEntities())
            {
                try
                {
                    var coursesurvey = (from c in db.CourseSurveys where c.CourseID == intcourseid select c).First();
                    Init(db, coursesurvey);
                }
                catch
                {
                }
            }
        }


        private void Init(SurveyEntities db, Entities.CourseSurvey i)
        {
            CourseSurveyModel = i;
        }
        public Gsmu.Api.Data.Survey.Entities.CourseSurvey CourseSurveyModel
        {
            get;
            set;
        }
    }
}
