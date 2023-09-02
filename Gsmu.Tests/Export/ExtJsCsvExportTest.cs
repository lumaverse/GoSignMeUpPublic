using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using web = Gsmu.Api.Web;
using json = Newtonsoft.Json;
using entities = Gsmu.Api.Data.School.Entities;
using data = Gsmu.Api.Data;
using export = Gsmu.Api.Export;

namespace Gsmu.Tests.Export
{
    [TestClass]
    public class ExtJsCsvExportTest
    {
        public ExtJsCsvExportTest()
        {
            var httpContext = HttpHelper.FakeHttpContext("http://localhost/");
            System.Web.HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void ExtJsCsvExportExecutionTest()
        {
            string assembly = "Gsmu.Api";
            string ns = "Gsmu.Api.Data.School.Entities";
            string contextName = "SchoolEntities";
            string entity = "District";
            int page = 1;
            int limit = 10;
            string sort = "[{\"property\":\"DISTRICT1\",\"direction\":\"ASC\"}]";
            string filter = "[{\"property\":\"DISTRICT1\",\"value\":\"buck\"}]";
            string columns = "[{\"column\":\"DISTRICT1\",\"text\":\"Company Title\"},{\"column\":\"SortOrder\",\"text\":\"Sort order\"},{\"column\":\"HideInPublicArea\",\"text\":\"Hide in public area\"},{\"column\":\"NoTaxShipping\",\"text\":\"No tax or shipping\"},{\"column\":\"AltEmailConfirmation\",\"text\":\"Alternate confirmation e-mail\"},{\"column\":\"MembershipFlag\",\"text\":\"Force Membership\"}]";

            var ds = data.DataStoreHelper<object>.GetDataStore(assembly, ns, contextName, entity);
            var result = ds.List(page, limit, sort, filter);
            var exportResult = export.ExtJsCsvExport.GenerateCvsFile(result, columns);
            Console.Write(exportResult);

        }
    }
}
