using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace Gsmu.Api.Data.School.Course
{
    public static class CourseFilesHelper
    {
        public static string[] GetCourseFileList(int courseId)
        {
            string filepath = HttpContext.Current.Server.MapPath(WebConfiguration.DocumentsFolder);
            if (Directory.Exists(filepath))
            {
                string[] filePaths = Directory.GetFiles(filepath, courseId.ToString() + "-*", SearchOption.TopDirectoryOnly);
                return filePaths;
            }
            return new string[0];
        }
    }
}
