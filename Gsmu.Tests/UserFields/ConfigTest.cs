using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Gsmu.Api.Data.ViewModels.UserFields;

namespace Gsmu.Tests.UserFields
{
    [TestClass]
    public class ConfigTest
    {

        public ConfigTest()
        {
            var httpContext = HttpHelper.FakeHttpContext("http://localhost/");
            System.Web.HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void ModelTest()
        {
            var model = new UserFieldsModel();
            var district = model.DistrictLabel;

            System.Console.WriteLine(district);

            Assert.AreEqual(district, model.DistrictLabel);

            model.DistrictLabel = district + "!";

            System.Console.WriteLine(model.DistrictLabel);

            Assert.AreNotEqual(district, model.DistrictLabel);

            district = model.DistrictLabel;

            Assert.AreEqual(district, model.DistrictLabel);

            model.Save();

            model = new UserFieldsModel();

            Assert.AreEqual(model.DistrictLabel, district);
        }
    }
}
