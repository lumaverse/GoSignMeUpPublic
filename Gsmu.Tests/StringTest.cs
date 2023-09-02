using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gsmu.Tests
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void BackSlashForLdap()
        {
            var dn = @"CN=Tester\, Mdm,OU=Systems Management,OU=Dublin,OU=North America,OU=Staff,OU=OCLC,DC=ServiceNowLDSInstance,DC=oa,DC=oclc,DC=org";
            Console.WriteLine(dn);
        }
    }
}
