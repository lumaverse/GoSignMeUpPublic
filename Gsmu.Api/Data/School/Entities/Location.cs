//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gsmu.Api.Data.School.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public Location()
        {
            this.Schools = new HashSet<School>();
        }
    
        public int LocationID { get; set; }
        public string Location1 { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string LocationURL { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string LocationAdditionalInfo { get; set; }
        public string country { get; set; }
    
        public virtual ICollection<School> Schools { get; set; }
    }
}
