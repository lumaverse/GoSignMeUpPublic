using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gsmu.Api.Networking.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace Gsmu.Tests.Networking
{
    [TestClass]
    public class EmailTest
    {
        public EmailTest()
        {
            var httpContext = HttpHelper.FakeHttpContext("http://localhost/");
            System.Web.HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void TestConfirmation()
        {
            var mail = new EmailFunction();
            mail.SendConfirmationEmail("", null, null,"");
        }
        
        [TestMethod]
        public void TestMaterial()
        {
            var mail = new EmailFunction();
            mail.SendConfirmationEmail("50", "CNA76MXX8651475", "gsmu.softasware.com","");
        }
    }
}
