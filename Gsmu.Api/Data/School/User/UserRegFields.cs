using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.User
{
    public class UserRegFields
    {
        public string FieldLabel { get; set; }

        public int FieldSpecsId { get; set; }

        public string FieldName { get; set; }

        public int FieldRequired { get; set; }

        public string test { get; set; }

        public bool BoolFieldRequired { get; set; }

        public string FieldGrp { get; set; }

        public int MaskNum { get; set; }

        public bool AllowBlankBool { get; set; }

        public string FieldCustomList { get; set; }

        public int? FieldListType { get; set; }

        public int DefaultMaskNumber { get; set; }

        public string MaskTxt { get; set; }

        public string GroupWidget { get; set; }
    }
}
