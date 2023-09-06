using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Shibboleth
{
   public  class ShibbolethHelper
    {

        public bool CheckExistingStudent(string userName)
        {
            return false;
        }

        public static string GetShibbolethUserNameField(string fieldName)
        {
            using (var db = new SchoolEntities())
            {
                string exist = (from op in db.FieldSpecs where op.FieldName == fieldName && op.TableName == "Students" select op.shibboleth_attribute).FirstOrDefault();
                return exist;
            }
        }
    }
}
