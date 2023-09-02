using Gsmu.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gsmu.Web
{
    public partial class ChasePayment_Template : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string hheader = Settings.Instance.GetMasterInfo4().PublicHeaderContent;
            divHeader.InnerHtml = hheader;
        }
    }
}