using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using json = Newtonsoft.Json;
using System.Web.Script.Serialization;
using school = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    /*
{
    // the unique identifier for the course
    "id": 370663,
 
    // the SIS identifier for the course, if defined
    "sis_course_id": null,
 
    // the full name of the course
    "name": "InstructureCon 2012",
 
    // the course code
    "course_code": "INSTCON12",
 
    // the current state of the course
    // one of "unpublished", "available", "completed", or "deleted"
    "workflow_state": "available",
 
    // the account associated with the course
    "account_id": 81259,
 
    // the root account associated with the course
    "root_account_id": 81259,
 
    // the start date for the course, if applicable
    "start_at": "2012-06-01T00:00:00-06:00",
 
    // the end date for the course, if applicable
    "end_at": "2012-09-01T00:00:00-06:00",
 
    // A list of enrollments linking the current user to the course.
    // for student enrollments, grading information may be included
    // if include[]=total_scores
    "enrollments": [
      {
        "type": "student",
        "role": "StudentEnrollment",
        "computed_final_score": 41.5,
        "computed_current_score": 90,
        "computed_final_grade": "F",
        "computed_current_grade": "A-"
      }
    ],
 
    // course calendar
    "calendar": {
      "ics": "https://canvas.instructure.com/feeds/calendars/course_abcdef.ics"
    },
 
    // the type of page that users will see when they first visit the course
    // - 'feed': Recent Activity Dashboard
    // - 'wiki': Wiki Front Page
    // - 'modules': Course Modules/Sections Page
    // - 'assignments': Course Assignments List
    // - 'syllabus': Course Syllabus Page
    // other types may be added in the future
    "default_view": "feed",
 
    // optional: user-generated HTML for the course syllabus
    "syllabus_body": "<p>syllabus html goes here<\/p>",
 
    // optional: the number of submissions needing grading
    // returned only if the current user has grading rights
    // and include[]=needs_grading_count
    "needs_grading_count": 17,
 
    // optional: the name of the enrollment term for the course
    // returned only if include[]=term
    "term": {
      "id": 1,
      "name": "Default Term",
      "start_at": "2012-06-01T00:00:00-06:00",
      "end_at": null
    },
 
    // weight final grade based on assignment group percentages
    "apply_assignment_group_weights": true,
 
    // optional: the permissions the user has for the course.
    // returned only for a single course and include[]=permissions
    "permissions": {
       "create_discussion_topic": true
    },
 
    "is_public": true,
 
    "public_syllabus": true,
 
    "public_description": "Come one, come all to InstructureCon 2012!",
 
    "storage_quota_mb": 5,
 
    "hide_final_grades": false,
 
    "license": "Creative Commons",
 
    "allow_student_assignment_edits": false,
 
    "allow_wiki_comments": false,
 
    "allow_student_forum_attachments": false,
 
    "open_enrollment": true,
 
    "self_enrollment": false,
 
    "restrict_enrollments_to_course_dates": false
}    

     * */
    public class Course
    {
        private bool gsmuCourseIdChecked = false;
        private bool gsmuSectionChecked = false;
        private int? gsmuCourseId = null;

        public Course()
        {
            Enrollments = new List<Enrollment>();
        }

        [json.JsonProperty("id")]
        public int Id { get; set; }

        [json.JsonProperty("sis_course_id")]
        public string SisCourseId { get; set; }

        [json.JsonProperty("name")]
        public string Name { get; set; }

        [json.JsonProperty("course_code")]
        public string CourseCode { get; set; }

        [json.JsonProperty("workflow_state")]
        [json.JsonConverter(typeof(json.Converters.StringEnumConverter))]
        public CourseWorkflowState WorkflowState { get; set; }

        [json.JsonProperty("account_id")]
        public int? AccountId { get; set; }

        [json.JsonProperty("root_account_id")]
        public int? RootAccountId { get; set; }

        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        [json.JsonProperty("start_at")]
        public DateTime? StartAt { get; set; }

        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        [json.JsonProperty("end_at")]
        public DateTime? EndAt { get; set; }

        [json.JsonProperty("enrollments")]
        public List<Enrollment> Enrollments { get; set; }

        [json.JsonProperty("calendar")]
        public Calendar Calendar { get; set; }

        [json.JsonProperty("default_view")]
        [json.JsonConverter(typeof(json.Converters.StringEnumConverter))]
        public PageType? DefaultView { get; set; }

        [json.JsonProperty("syllabus_body")]
        public string SyllabusBody { get; set; }

        [json.JsonProperty("needs_grading_count")]
        public int? NeedsGradingCount { get; set; }

        [json.JsonProperty("apply_assignment_group_weights")]
        public bool? ApplyAssignmentGroupWeights { get; set; }

        [json.JsonProperty("is_public")]
        public bool? IsPublic { get; set; }

        [json.JsonProperty("public_syllabus")]
        public bool? PublicSyllabus { get; set; }

        [json.JsonProperty("public_description")]
        public string PublicDescription { get; set; }

        [json.JsonProperty("storage_quota_mb")]
        public int? StorageQuotaMb { get; set; }

        [json.JsonProperty("hide_final_grades")]
        public bool? HideFinalGrades { get; set; }

        [json.JsonProperty("license")]
        public string License { get; set; }

        [json.JsonProperty("allow_student_assignment_edits")]
        public bool? AllowStudentAssignmentEdits { get; set; }

        [json.JsonProperty("allow_wiki_comments")]
        public bool? AllowWikiComments { get; set; }

        [json.JsonProperty("allow_student_forum_attachments")]
        public bool? AllowStudentForumAttachments { get; set; }

        [json.JsonProperty("open_enrollment")]
        public bool? OpenEnrollment { get; set; }

        [json.JsonProperty("self_enrollment")]
        public bool? SelfEnrollment { get; set; }

        [json.JsonProperty("restrict_enrollments_to_course_dates")]
        public bool? RestrictEnrollmentsToCourseDates { get; set; }


        public SynchronizationStatus GsmuSynchronizationStatus
        {
            get;
            set;
        }

        public int? GsmuCourseId
        {
            get
            {
                if (!gsmuCourseIdChecked)
                {
                    using (var db = new school.SchoolEntities())
                    {
                        var course = (from c in db.Courses where c.canvas_course_id == this.Id select c).FirstOrDefault();
                        if (course != null)
                        {
                            gsmuCourseId = course.COURSEID;
                        }
                        gsmuCourseIdChecked = true;
                        db.Database.Connection.Close();
                    }
                }
                return gsmuCourseId;
            }
        }

        public string selectedsection
        {
            get
            {
                string canvassectionName = "n/a";
                if (!gsmuSectionChecked)
                {
                    using (var db = new school.SchoolEntities())
                    {
                        var course = (from c in db.Courses where c.canvas_course_id == this.Id select c).FirstOrDefault();
                        if (course != null)
                        {
                            if (course.CourseConfiguration != null)
                            {
                                System.Web.Script.Serialization.JavaScriptSerializer JSSerializeObj = new System.Web.Script.Serialization.JavaScriptSerializer();
                                dynamic CourseInfoconfiguration = JSSerializeObj.Deserialize(course.CourseConfiguration.ToString(), typeof(object));
                                if (CourseInfoconfiguration.ContainsKey("canvassectionName"))
                                {
                                    canvassectionName = CourseInfoconfiguration["canvassectionName"];
                                }
                                if (CourseInfoconfiguration.ContainsKey("canvassectionID"))
                                {
                                    if (!string.IsNullOrEmpty(CourseInfoconfiguration["canvassectionID"]) && CourseInfoconfiguration["canvassectionID"] != "0")
                                    {
                                        gsmuSectionChecked = true;
                                    }
                                }
                            }
                        }
                        db.Database.Connection.Close();
                    }
                }
                return canvassectionName;
            }
        }
        
        public bool IsGsmuCourse
        {
            get
            {
                return GsmuCourseId.HasValue;
            }
        }

    }
}
