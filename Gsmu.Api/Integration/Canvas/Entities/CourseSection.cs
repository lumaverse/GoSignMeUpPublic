using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using json = Newtonsoft.Json;

namespace Gsmu.Api.Integration.Canvas.Entities
{
    public class CourseSection
    {
        [json.JsonProperty("id")]
        public int SectionId { get; set; }

        [json.JsonProperty("account_id")]
        public int? AccountId { get; set; }

        [json.JsonProperty("course_id")]
        public string SectionCourseId { get; set; }

        [json.JsonProperty("name")]
        public string Name { get; set; }

        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        [json.JsonProperty("start_at")]
        public DateTime? StartAt { get; set; }

        [json.JsonConverter(typeof(json.Converters.IsoDateTimeConverter))]
        [json.JsonProperty("end_at")]
        public DateTime? EndAt { get; set; }

        [json.JsonProperty("restrict_enrollments_to_section_dates")]
        public bool? restrict_enrollments_to_section_dates { get; set; }

        [json.JsonProperty("sis_section_id")]
        public string sis_section_id { get; set; }

        [json.JsonProperty("sis_course_id")]
        public string sis_course_id { get; set; }

        [json.JsonProperty("sis_import_id")]
        public string sis_import_id { get; set; }
    }
}
