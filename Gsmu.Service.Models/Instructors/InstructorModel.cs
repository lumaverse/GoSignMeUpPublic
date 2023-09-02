using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Instructors
{
    public class InstructorBasicDetailsModel
    {
        public int InstructorId { get; set; }
        public string InstructorFirstName { get; set; }
        public string InstructorLastName { get; set; }
        public string InstructorNumber { get; set; }
    }

    public class InstructorFullAddress
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
    public class InstructorModel //Please rename it appropriately.
    {
        public InstructorBasicDetailsModel InstructorBasicDetailsModel { get; set; }
       // public int InstructorId { get; set; }
       // public string InstructorNumber { get; set; }
       // public string FirstName { get; set; }
       // public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SSN { get; set; }
        public string GradeLevel { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictId { get; set; }
        public short Classified { get; set; }
        public short Certified { get; set; }
        public short Other { get; set; }
        public string Content { get; set; }
        public string Email { get; set; }

        public InstructorFullAddress InstructorFullAddress { get; set; }
       // public string Address { get; set; }
       // public string City { get; set; }
      //  public string State { get; set; }
      //  public string Zip { get; set; }
      //  public string Country { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Fax { get; set; }
        public string Experience { get; set; }
        public short DistEmployee { get; set; }
        public short Disabled { get; set; }
        public string Bio { get; set; }
        public string PhotoImage { get; set; }
        public string AdminNotes { get; set; }
    }
}
