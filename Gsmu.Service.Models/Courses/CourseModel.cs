using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Courses
{

    public  class CourseModel
    {
        public int CourseId { get; set; }
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public List<EventSessionCourseDateTimeModel> DateTime { get; set; }

    }

    public class CourseGridDetailResultModel
    {
        public CourseBasicDetails CourseBasicDetails { get; set; }
        public CoursePriceDetails CoursePriceDetails { get; set; }
        public LocationDetailsModel LocationDetailsModel { get; set; }
        public EventSessionCourseDateTimeModel DateTime { get; set; }
        public AccessConfigurationModel AccessConfiguration { get; set; }
        public CourseSingleCategoryDetails CourseSingleCategoryDetails { get; set; }

    }

    public class CourseDetailsResultModel
    {
        public CourseBasicDetails CourseBasicDetails { get; set; }
        public CoursePriceCompleteDetailsModel CourseCompletePriceDetails { get; set; }
        public CourseLocationCompleteDetailsModel CourseLocationCompleteDetails { get; set; }
        public List<MaterialBasicDetailsModel> ListMaterialBasicDetails { get; set; }
        public AccessConfigurationModel AccessConfigurationModel { get; set; }
        public List<CourseDateTimeFormattedModel> ListDateTime { get; set; }
        public CourseMiscSettingsModel CourseMiscSettings { get; set; }
        public CreditInformationModel CreditInformationModel { get; set; }
        public CourseContactDetailsModel CourseContactDetails { get; set; }
        public CourseStatisticsModel CourseStatisticsModel { get; set; }
        public CourseInstructorDetailsModel CourseInstructorDetailsModel { get; set; }

    }
    public class CourseBasicDetails
    {
        public int CourseId { get; set; }
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string TileImage { get; set; }
        public string CourseCost { get; set; }
        public DateTime CourseStartDate { get; set; }
        public DateTime CourseEndDate { get; set; }
    }
    public class CoursePriceDetails
    {
        public decimal price { get; set; }
        public string PriceDescription { get; set; }
    }
    public class CourseInstructorDetailsModel
    {
        public string instructorName1 { get; set; }
        public string instructorName2 { get; set; }
        public string instructorName3 { get; set; }

    }
    public class CoursePriceCompleteDetailsModel
    {
        public decimal pricemember { get; set; }
        public string MemberPriceDescription { get; set; }
        public decimal PricenonMemember { get; set; }
        public string NonMemberPriceDescription { get; set; }
        public decimal PriceSepecial{ get; set; }
        public string SpecialPriceDescription { get; set; }

        public bool DisplayPrice { get; set; }
        public List<PriceOptionDetails> ListPriceOptionDetails { get; set; }
    }
    public class PriceOptionDetails
    {
        public int PriceOptionId { get; set; }
        public decimal Price { get; set; }
        public string  Description { get; set; }
        public Nullable<int> rangestart { get; set; }
        public Nullable<int> rangeend { get; set; }
        public int MemberType { get; set; }
        public int CourseId { get; set; }
    }
    public class LocationDetailsModel
    {
        public string Location { get; set; }
        public string LocationURL { get; set; }
    }
    public class CourseLocationCompleteDetailsModel
    {
        public string Location { get; set; }
        public string LocationURL { get; set; }
        public string Room { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
    public class AccessConfigurationModel
    {
       public  int internalClass { get; set; }
       public  string AccessCode { get; set; }
       public bool IsOnline { get; set; }
    }

    public class CreditInformationModel
    {
        public Nullable<double> CreditHours { get; set; }
        public Nullable<float> Optionalcredithours1 { get; set; }
        public Nullable<double> Optionalcredithours2 { get; set; }
        public Nullable<double> Optionalcredithours3 { get; set; }
        public Nullable<double> Optionalcredithours4 { get; set; }
        public Nullable<double> Optionalcredithours5 { get; set; }
        public Nullable<double> Optionalcredithours6 { get; set; }
        public Nullable<double> Optionalcredithours7 { get; set; }
        public Nullable<double> Optionalcredithours8 { get; set; }

        public Nullable<double> CustomCreditHours { get; set; }
        public Nullable<double> CEUCredit { get; set; }
        public Nullable<double> GraduateCredit { get; set; }

        public Nullable<int> ShowCreditInPublic { get; set; }
    }
    public class CourseContactDetailsModel{
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
    }
    public class CourseMiscSettingsModel
    {
        public string DateTimeComment { get; set; }
        public string PreRequisiteText { get; set; }
    }

    public class CourseStatisticsModel
    {
        public int MaxEnroll { get; set; }
        public int MaxWait { get; set; }
        public int AvailableSeats { get; set; }
        public bool HideSeatAvailable { get; set; }
    }
    public class CourseSingleCategoryDetails
    {
        public string MainCategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string SubSubCategoryId { get; set; }
    }
    public class CourseMainCategoryModel
    {
        public string MainCategoryName { get; set; }
        public string MainCategoryId { get; set; }
        public List<CourseSubCategoryModel> SubCategories { get; set; }

    }
    public class CourseSubCategoryModel
    {
        public string SubCategoryName { get; set; }
        public string SubCategoryId { get; set; }
        public List<CourseSubSubCategoryModel> SubSubCategories { get; set; }

    }
    public class CourseSubSubCategoryModel
    {
        public string SubCategoryName { get; set; }
        public string SubSubCategoryId { get; set; }

    }

    public class CourseCategoriesModel {
        public int CourseId { get; set; }
        public int MainCategoryId { get; set; }
        public string MainCategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string SubSubCategoryName { get; set; }

        public string MainCategoryName2 { get; set; }
        public string SubCategoryName2 { get; set; }
        public string SubSubCategoryName2 { get; set; }

        public string MainCategoryName3 { get; set; }
        public string SubCategoryName3 { get; set; }
        public string SubSubCategoryName3 { get; set; }

        public int MainOrder { get; set; }
    }

    public class GenericNameIdModel{
        public string Name { get; set; }
        public string Id { get; set; }
    }
}

