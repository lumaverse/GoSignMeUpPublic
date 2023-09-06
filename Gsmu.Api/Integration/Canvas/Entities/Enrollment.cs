using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class Enrollment
    {
        [json.JsonProperty("id")]
        public int Id { get; set; }

        [json.JsonProperty("course_id")]
        public int CourseId { get; set; }

        [json.JsonProperty("course_section_id")]
        public int? CourseSectionId { get; set; }

        [json.JsonProperty("enrollment_state")]
        [json.JsonConverter(typeof(json.Converters.StringEnumConverter))]
        public EnrollmentState EnrollmentState { get; set; }

        [json.JsonProperty("limit_privileges_to_course_section")]
        public bool? LimitPrivilegesToCourseSection { get; set; }

        [json.JsonProperty("root_account_id")]
        public int? RootAccountId { get; set; }

        [json.JsonProperty("type")]
        [json.JsonConverter(typeof(json.Converters.StringEnumConverter))]
        public EnrollmenType Type { get; set; }

        [json.JsonProperty("role")]
        public string Role { get; set; }

        [json.JsonProperty("user_id")]
        public Int64 UserId { get; set; }

        [json.JsonProperty("updated_at")]
        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        public DateTime? UpdatedAt { get; set; }

        [json.JsonProperty("created_at")]
        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        [json.JsonProperty("last_activity_at")]
        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        public DateTime? LastActivityAt { get; set; }

        [json.JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [json.JsonProperty("sis_course_id")]
        public string sis_course_id { get; set; }

        [json.JsonProperty("sis_section_id")]
        public string sis_section_id { get; set; }

        [json.JsonProperty("grades")]
        public Grades Grades { get; set; }

        [json.JsonProperty("user")]
        public User User { get; set; }
    }
}
