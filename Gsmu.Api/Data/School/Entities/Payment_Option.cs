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
    
    public partial class Payment_Option
    {
        public int PAYID { get; set; }
        public string PAYMENTTYPE { get; set; }
        public Nullable<int> PaymentClass { get; set; }
        public short VisibleTo { get; set; }
        public Nullable<int> paymentNumberNotRequired { get; set; }
        public string ccfee { get; set; }
        public Nullable<int> allowPublicPayPending { get; set; }
        public Nullable<int> autoApprovedPayment { get; set; }
    }
}
