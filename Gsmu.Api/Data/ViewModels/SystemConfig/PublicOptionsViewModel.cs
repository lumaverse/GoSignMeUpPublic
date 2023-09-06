using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.ViewModels.BaseModels;
using System.Web.Mvc;

namespace Gsmu.Api.Data.ViewModels.SystemConfig
{
    public class PublicOptionsViewModel : MasterInfoBaseModel
    {
        public PublicOptionsViewModel()
        {
        }
        //PDFHeaderFooterInfo
        public int ForceLogin
        {
            get;
            set;
        }
        //SAPIntegration
        public int SAPIntegrationON
        {
            get;
            set;
        }
        //FieldMasks

        //Common
        //Select to
        public SelectList SystemTimeZoneHourList { get; set; }
        public SelectList PolarAnswers { get; set; }
        public SelectList PublicHideLinksList { get; set; }
        public SelectList NameDisplayStyleList { get; set; }
        public SelectList SupervisorExcludeInactiveList { get; set; }
    }
}
