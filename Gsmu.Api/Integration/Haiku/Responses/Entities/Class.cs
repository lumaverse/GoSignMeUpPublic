using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using school = Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Integration.Haiku.Responses.Entities
{
    public class Class
    {
        private bool gsmuCourseIdChecked = false;
        private int? gsmuCourseId = null;

        public Class()
        {
            GsmuSynchronizationStatus = SynchronizationStatus.Unset;
        }

        [XmlAttribute("id")]
        public int Id
        {
            get;
            set;
        }

        [XmlAttribute("import_id")]
        public string ImportId
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("shortname")]
        public string ShortName
        {
            get;
            set;
        }

        [XmlAttribute("year")]
        public string Year
        {
            get;
            set;
        }

        [XmlAttribute("teacher_id")]
        public int TeacherId
        {
            get;
            set;
        }

        [XmlAttribute("organization_id")]
        public int OrganizationId
        {
            get;
            set;
        }

        [XmlAttribute("active")]
        public bool Active
        {
            get;
            set;
        }

        [XmlAttribute("end_date")]
        public DateTime EndDate
        {
            get;
            set;
        }

        [XmlAttribute("code")]
        public string Code
        {
            get;
            set;
        }

        [XmlAttribute("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlAttribute("use_dynamic_roster")]
        public bool UseDynamicRoster
        {
            get;
            set;
        }

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
                    gsmuCourseIdChecked = true;
                    using (var db = new school.SchoolEntities())
                    {
                        var course = (from c in db.Courses where c.haiku_course_id == this.Id select c).FirstOrDefault();
                        if (course != null)
                        {
                            gsmuCourseId = course.COURSEID;
                        }
                        db.Database.Connection.Close();
                    }
                }
                return gsmuCourseId;
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
