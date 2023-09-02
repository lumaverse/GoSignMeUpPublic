using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Admin.Portal
{
    public class AdminCanvasSectionCourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Coursenumber { get; set; }
        public int CanvasCourseId { get; set; }
        public int CanvasSectionID { get; set; }
        public string CanvasSectionNAme { get; set; }
        public DateTime? CourseStartDate { get; set; }
    }

    public class ListAdminCanvasSectionCourses
    {
        public List<AdminCanvasSectionCourseModel> AdminCanvasSectionCourses { get; set; }
        public int totalCount { get; set; }
    }
}
