using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Service.BusinessLogic.Instructors;
using Gsmu.Service.Interface.Courses;
using Gsmu.Service.Models.Courses;
using Gsmu.Service.Models.Events;
using Gsmu.Service.Models.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.Courses
{
    public class CourseDetailsManager : ICourseDetails
    {
        public CourseDetailsResultModel GetCourseDetailsById(int courseId)
        {
            CourseDetailsResultModel CourseDetailsResultModel = new CourseDetailsResultModel();
            using (var db = new SchoolEntities())
            {
                var courseDetails = (from _courseDetails in db.Courses where _courseDetails.COURSEID == courseId && _courseDetails.InternalClass==0 && _courseDetails.CANCELCOURSE==0 select _courseDetails).FirstOrDefault();
                if (courseDetails != null)
                {
                    CourseDetailsResultModel.CourseBasicDetails = new CourseBasicDetails
                    {
                        CourseId= courseDetails.COURSEID,
                        CourseName = courseDetails.COURSENAME,
                        CourseNumber = courseDetails.COURSENUM,
                        Description = courseDetails.DESCRIPTION
                    };

                    CourseDetailsResultModel.AccessConfigurationModel = new AccessConfigurationModel
                    {
                        IsOnline = courseDetails.IsOnlineCourse,

                    };
                    CourseDetailsResultModel.CourseCompletePriceDetails = new CoursePriceCompleteDetailsModel
                    {
                        pricemember = courseDetails.DISTPRICE.Value,
                        PricenonMemember = courseDetails.NODISTPRICE.Value,
                        PriceSepecial = courseDetails.SpecialDistPrice1.Value,

                        MemberPriceDescription = Settings.Instance.GetMasterInfo2().membertypememberlabel,
                        NonMemberPriceDescription = Settings.Instance.GetMasterInfo2().membertypenonmemberlabel,
                        SpecialPriceDescription = Settings.Instance.GetMasterInfo2().MemberTypeSpecialMemberLabel1,

                        ListPriceOptionDetails = new List<PriceOptionDetails>
                        {
                        }


                    };
                    var priceoptions = (from coursepriceoptions in db.CoursePricingOptions where coursepriceoptions.CourseId == courseId select coursepriceoptions).ToList();
                    PriceOptionDetails PriceOptionDetails = new PriceOptionDetails();
                    if (priceoptions != null)
                    {
                        foreach (var option in priceoptions)
                        {
                            PriceOptionDetails.Price = option.Price;
                            PriceOptionDetails.Description = option.PriceTypedesc;
                            PriceOptionDetails.PriceOptionId = option.PricingOptionId;
                            PriceOptionDetails.MemberType = option.Type;
                        }
                    }
                    CourseDetailsResultModel.CourseContactDetails = new CourseContactDetailsModel
                    {
                        ContactName = courseDetails.ContactName,
                        PhoneNumber = courseDetails.ContactPhone
                    };
                    InstructorDetailsManager InstructorDetails = new Instructors.InstructorDetailsManager();
                    InstructorBasicDetailsModel Instructor1Model = new InstructorBasicDetailsModel();
                    InstructorBasicDetailsModel Instructor2Model = new InstructorBasicDetailsModel();
                    InstructorBasicDetailsModel Instructor3Model = new InstructorBasicDetailsModel();
                    //Instructor1Model = InstructorDetails.GetInstructorDetailsById(courseDetails.INSTRUCTORID.Value);
                    //Instructor2Model = InstructorDetails.GetInstructorDetailsById(courseDetails.INSTRUCTORID2.Value);
                    //Instructor3Model = InstructorDetails.GetInstructorDetailsById(courseDetails.INSTRUCTORID3.Value);

                    CourseDetailsResultModel.CourseInstructorDetailsModel = new CourseInstructorDetailsModel();
                    //if (Instructor1Model != null)
                    //{
                    //    CourseDetailsResultModel.CourseInstructorDetailsModel.instructorName1 = Instructor1Model.InstructorFirstName + " " + Instructor1Model.InstructorLastName;
                    //}
                    //if (Instructor2Model != null)
                    //{
                    //    CourseDetailsResultModel.CourseInstructorDetailsModel.instructorName2 = Instructor2Model.InstructorFirstName + " " + Instructor2Model.InstructorLastName;
                    //}
                    //if (Instructor3Model != null)
                    //{
                    //    CourseDetailsResultModel.CourseInstructorDetailsModel.instructorName3 = Instructor3Model.InstructorFirstName + " " + Instructor3Model.InstructorLastName;
                    //}

                    CourseDetailsResultModel.CourseLocationCompleteDetails = new CourseLocationCompleteDetailsModel
                    {
                        Address = courseDetails.LOCATION,
                        City = courseDetails.CITY,
                        Location= courseDetails.LOCATION,
                        LocationURL = courseDetails.LOCATIONURL,
                        Room = courseDetails.ROOM,
                        State = courseDetails.STATE,
                        Zip = courseDetails.ZIP
                    };

                    CourseDetailsResultModel.CourseMiscSettings = new CourseMiscSettingsModel
                    {
                        DateTimeComment = courseDetails.StartEndTimeDisplay,
                        PreRequisiteText = courseDetails.PrerequisiteInfo,

                    };

                    CourseDetailsResultModel.CourseStatisticsModel = new CourseStatisticsModel{

                        MaxEnroll = courseDetails.MAXENROLL.Value,
                        MaxWait = courseDetails.MAXWAIT.Value,
                        HideSeatAvailable =false,
                        AvailableSeats = courseDetails.MAXENROLL.Value+ courseDetails.MAXWAIT.Value,

                    };

                    CourseDetailsResultModel.CreditInformationModel = new CreditInformationModel
                    {
                        CEUCredit = courseDetails.CEUCredit,
                        CreditHours = courseDetails.CREDITHOURS,
                        CustomCreditHours = courseDetails.CustomCreditHours,
                        GraduateCredit = courseDetails.GraduateCredit,
                        Optionalcredithours1 = courseDetails.Optionalcredithours1,
                        Optionalcredithours2 = courseDetails.Optionalcredithours2,
                        Optionalcredithours3 = courseDetails.Optionalcredithours3,
                        Optionalcredithours4 = courseDetails.Optionalcredithours4,
                        Optionalcredithours5 = courseDetails.Optionalcredithours5,
                        Optionalcredithours6 = courseDetails.Optionalcredithours6,
                        Optionalcredithours7 = courseDetails.Optionalcredithours7,
                        Optionalcredithours8 = courseDetails.Optionalcredithours8,
                        ShowCreditInPublic = courseDetails.showcreditinpublic

                    };

                    CourseDetailsResultModel.ListMaterialBasicDetails = GetListMaterialsByCourseId(courseDetails);
                    CourseDetailsResultModel.ListDateTime = GetCourseDateList(courseId);
                }
            }
            return CourseDetailsResultModel;
        }

        public List<MaterialBasicDetailsModel> GetListMaterialsByCourseId(Course course)
        {
            using (var db = new SchoolEntities())
            {

                List<int> materialIdList = new List<int>();
                foreach (var id in course.MATERIALS.Trim().Split(','))
                {
                    var validIdString = id.Replace("~", "");
                    int validId;
                    if (int.TryParse(validIdString, out validId))
                    {
                        materialIdList.Add(validId);
                    }
                }
                return   (from _courseMaterials in db.Materials
                                     where materialIdList.Contains(_courseMaterials.productID)
                                     select new MaterialBasicDetailsModel
                                     {
                                         Price = _courseMaterials.price.Value,
                                         ProductId=_courseMaterials.productID,
                                         ProductName=_courseMaterials.product_name
                                     }
                    ).ToList();
            }
           
        }

        public List<CourseDateTimeFormattedModel> GetCourseDateList(int courseId)
        {
            using (var db = new SchoolEntities())
            {
                var cdatetimes =  (from _courseDates in db.Course_Times
                        where _courseDates.COURSEID ==courseId
                        select new EventSessionCourseDateTimeModel
                        {
                            CourseDate = _courseDates.COURSEDATE,
                            EndTime = _courseDates.FINISHTIME.Value,
                            StartTime = _courseDates.STARTTIME
                        }
                    ).ToList();

                List<CourseDateTimeFormattedModel> ListCourseDateTimeFormattedModel = new List<CourseDateTimeFormattedModel>();
                CourseDateTimeFormattedModel CourseDateTimeFormattedModel = new CourseDateTimeFormattedModel();
                foreach (var cdate in cdatetimes)
                {
                    CourseDateTimeFormattedModel.CourseDate =string.Format("{0:MM/dd/yyyy}",cdate.CourseDate.Value);
                    CourseDateTimeFormattedModel.StartTime = cdate.StartTime.Value.TimeOfDay;
                    CourseDateTimeFormattedModel.EndTime = cdate.EndTime.Value.TimeOfDay;
                    CourseDateTimeFormattedModel = new CourseDateTimeFormattedModel();
                    ListCourseDateTimeFormattedModel.Add(CourseDateTimeFormattedModel);

                }
                return ListCourseDateTimeFormattedModel;

            }
        }
        public EventDetailsModel GetEventCourseFullDetailsById(int eventId)
        {
            Gsmu.Service.BusinessLogic.Events.EventDetails EventDetails = new Events.EventDetails();
            return EventDetails.GetEventDetails(eventId,true);
        }
    }
}
