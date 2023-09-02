using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Students
{
    class StudentAuthenticationModel
    {
    }

    public class StudentAuthenticationResponseModel
    {
        public List<string> Message { get; set; }
        public int Studentid { get; set; }
        
    }
}
