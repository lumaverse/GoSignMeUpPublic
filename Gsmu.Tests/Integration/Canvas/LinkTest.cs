using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gsmu.Api.Integration.Canvas;

namespace Gsmu.Tests
{
    [TestClass]
    public class LinkTest
    {   
        [TestMethod]
        public void ExtractTest()
        {
            var lastLink = "<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=10&per_page=10>; rel=\"current\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=9&per_page=10>; rel=\"prev\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=1&per_page=10>; rel=\"first\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=10&per_page=10>; rel=\"last\"";

            TestLink(lastLink);

            lastLink = "<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=5&per_page=10>; rel=\"current\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=6&per_page=10>; rel=\"next\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=4&per_page=10>; rel=\"prev\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=1&per_page=10>; rel=\"first\",<https://mccb.beta.instructure.com/api/v1/accounts/16/courses?state%5B%5D=created&state%5B%5D=claimed&state%5B%5D=available&with_enrollments=true&page=10&per_page=10>; rel=\"last\"";

            TestLink(lastLink);
        }

        private void TestLink(string lastLink)
        {
            var pages = new PaginationHelper(lastLink);
            Console.WriteLine(lastLink);
            DebugResult(pages);
            Console.WriteLine("----------------------------");
        }

        private void DebugResult(PaginationHelper pages)
        {
            foreach(var item in pages.Pages) {
                Console.WriteLine(item.Key.ToString() + ": " + item.Value);
            }
        }
    }
}
