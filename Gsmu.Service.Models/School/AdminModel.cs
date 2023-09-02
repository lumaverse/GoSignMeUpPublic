using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.School
{
    public class AdminModel
    {
        public int AdminId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime ? DateAdded { get; set; }
        public int Disabled { get; set; }
        public string PortalSettings { get; set; }
        public string WidgetSettings { get; set; }
    }

    public class AdminCredsModel {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
