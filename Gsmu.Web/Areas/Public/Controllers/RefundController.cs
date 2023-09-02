using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Commerce;
using System.Net.Mail;
using System.Web.Mail;
using Gsmu.Api.Data.ViewModels;
using Gsmu.Api.Networking;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using System.Xml;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using Gsmu.Api.Authorization;
using System.Net;


namespace Gsmu.Web.Areas.Public.Controllers
{
    public class RefundController : Controller
    {
        public void RefundPaypal(string session)
        {

            CreditCardPayments payment = new CreditCardPayments();
             payment.RefundPaypal(8344, 119018, "CFLB5FFE7614603",0, 0.00);
        }
       
    }
}
