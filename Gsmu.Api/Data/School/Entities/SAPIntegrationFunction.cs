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
    
    public partial class SAPIntegrationFunction
    {
        public int SAPIntegrationFunctionsID { get; set; }
        public string Function { get; set; }
        public string GSMUFieldName { get; set; }
        public string SAPFieldName { get; set; }
        public int SAPFieldSize { get; set; }
        public int SAPFieldOnlyNumeric { get; set; }
        public int SAPFieldNoSpaces { get; set; }
        public int SAPFieldNoSingleQuote { get; set; }
        public int SAPFieldNoSpecialCharacters { get; set; }
        public string GSMUTableName { get; set; }
        public string DefaultValue { get; set; }
        public string SAPFieldNameBNA { get; set; }
        public int SAPFieldOnlyDecimal { get; set; }
    }
}