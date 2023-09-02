using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Tests.Data
{
    [TestClass]
    public class DataTest
    {
        public DataTest()
        {
            var httpContext = HttpHelper.FakeHttpContext("http://localhost/");
            System.Web.HttpContext.Current = httpContext;
        }
        
        [TestMethod]
        public void School()
        {
            using(var db = new SchoolEntities()) {
                var schools = (from s in db.Schools select s ).ToList();

                Console.Write(string.Format("There are {0} schools", schools.Count));
            }
        }
    }
}
