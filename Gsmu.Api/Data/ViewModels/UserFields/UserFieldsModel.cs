using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels.UserFields
{
    public class UserFieldsModel
    {
        public UserFieldsModel()
        {

        }

        public string DistrictLabel
        {
            get
            {
                return Settings.Instance.GetMasterInfo().Field3Name;
            }
            set
            {
                Settings.Instance.GetMasterInfo().Field3Name = value;
            }
        }

        public string SchoolLabel
        {
            get
            {
                return Settings.Instance.GetMasterInfo().Field2Name;
            }
            set
            {
                Settings.Instance.GetMasterInfo().Field2Name = value;
            }
        }

        public string GradeLevelLabel
        {
            get
            {
                return Settings.Instance.GetMasterInfo().Field1Name;
            }
            set
            {
                Settings.Instance.GetMasterInfo().Field1Name = value;
            }
        }

        public void Save()
        {
            Settings.Instance.SaveChanges();
        }
    }
}
