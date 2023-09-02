using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Students
{
    //@TODO :
    //USE THIS CLASS TO HANDLE THE REST OF THE STUDENT MODELS
    //REMOVE THE School/StudentModels class
    public class StudentFullModel
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string Classification { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public int? Grade { get; set; }
        public string Department { get; set; }
        public int? School { get; set; }
        public int? District { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Fax { get; set; }
        public string Experience { get; set; }
        public short DistEmployee { get; set; }
        public short TraditionalCycle { get; set; }
        public string Notes { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }

    public class StudentNotesModel
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Notes { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class StudentAddressContactInfoModel {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }

    public class StudentAuthModel {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
