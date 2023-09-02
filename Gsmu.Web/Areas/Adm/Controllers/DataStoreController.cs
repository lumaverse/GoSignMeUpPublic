using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using web = Gsmu.Api.Web;
using json = Newtonsoft.Json;
using entities = Gsmu.Api.Data.School.Entities;
using data = Gsmu.Api.Data;
using export = Gsmu.Api.Export;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class DataStoreController : Controller
    {
        private static data.IDataStoreHelper GetDataStore(
            string assembly,
            string ns,
            string contextName,
            string entity)
        {
            // the object parameter is not required here, could be anything here because the datastore
            // static method does not need the type parameter, but since the class does we have to 
            // put something there.
            // this is becasue the way it is solved generically is very general although it could have had
            // a DataStoreHelperFactory class but I guess I put that in later on today so it makes more sense
            var ds = data.DataStoreHelper<object>.GetDataStore(assembly, ns, contextName, entity);
            return ds;
        }

        private static data.DataStoreResult GetReadResult(string assembly, string ns, string contextName, string entity, int page, int limit, string sort, string filter)
        {
            var ds = GetDataStore(assembly, ns, contextName, entity);
            var result = ds.List(page, limit, sort, filter);
            return result;
        }


        public ActionResult SetMasterinfoValue(int id, string key, string value)
        {
            Gsmu.Api.Data.Settings.Instance.SetMasterinfoValue(id, key, value);
            var result = new JavaScriptResult();
            result.Script = Request["callback"] + "();";
            return result;
        }

        public ActionResult Read(
            string assembly = "Gsmu.Api", 
            string ns = "Gsmu.Api.Data.School.Entities", 
            string contextName = "SchoolEntities", 
            string entity = "District", 
            int page = 1, 
            int limit = 10, 
            string sort = null,
            string filter = null,
            string query = null)
        {

            // this is for helping combobox based search - kind of off label re-use...
            if (!string.IsNullOrEmpty(query)) {
                switch (entity)
                {
                    case "District":
                        filter = "[{\"property\":\"DISTRICT1\",\"value\":\"" + query + "\"}]";
                        break;

                    case "School":
                        filter = "[{\"property\":\"Location\",\"value\":\"" + query + "\"}]";
                        break;

                    case "Course":
                        filter = "[";
                        filter += "{\"property\":\"COURSENAME\",\"value\":\"" + query + "\"},";
                        filter += "{\"property\":\"COURSENUM\",\"value\":\"" + query + "\"},";
                        filter += "{\"property\":\"COURSEID\",\"value\":\"" + query + "\"}";
                        filter += "]";
                        break;
                }
            }
            
            var result = GetReadResult(assembly, ns, contextName, entity, page, limit, sort, filter);

            return new JsonResult()
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult Update(
            string assembly = "Gsmu.Api", 
            string ns = "Gsmu.Api.Data.School.Entities", 
            string contextName = "SchoolEntities", 
            string entity = "District")
        {
            var requestPayload = this.GetRequestPayload();
            var ds = GetDataStore(assembly, ns, contextName, entity);

            try
            {
                var data = json.JsonConvert.DeserializeObject<Dictionary<string, string>>(requestPayload);
                ds.Update(data);
            }
            catch (json.JsonSerializationException)
            {
                var dataList = json.JsonConvert.DeserializeObject<IList<Dictionary<string, string>>>(requestPayload);
                foreach (var data in dataList)
                {
                    ds.Update(data);
                }
            }

            return Json(null);

        }

        [HttpPost]
        public ActionResult Destroy(
            string assembly = "Gsmu.Api", 
            string ns = "Gsmu.Api.Data.School.Entities", 
            string contextName = "SchoolEntities", 
            string entity = "District")
        {
            var requestPayload = this.GetRequestPayload();
            var ds = GetDataStore(assembly, ns, contextName, entity);

            try
            {
                var data = json.JsonConvert.DeserializeObject<Dictionary<string, string>>(requestPayload);
                ds.Delete(data);
            }
            catch (json.JsonSerializationException)
            {
                var dataList = json.JsonConvert.DeserializeObject<IList<Dictionary<string, string>>>(requestPayload);
                foreach (var data in dataList)
                {
                    ds.Delete(data);
                }
            }

            return Json(null);

        }

        [HttpPost]
        public ActionResult Create(
            string assembly = "Gsmu.Api",
            string ns = "Gsmu.Api.Data.School.Entities",
            string contextName = "SchoolEntities",
            string entity = "District")
        {
            var requestPayload = this.GetRequestPayload();
            var ds = GetDataStore(assembly, ns, contextName, entity);

            try
            {
                var data = json.JsonConvert.DeserializeObject<Dictionary<string, string>>(requestPayload);
                ds.Create(data);
            }
            catch (json.JsonSerializationException)
            {
                var dataList = json.JsonConvert.DeserializeObject<IList<Dictionary<string, string>>>(requestPayload);
                foreach (var data in dataList)
                {
                    ds.Create(data);
                }
            }

            return Json(null);

        }


        public ActionResult Export(
           string assembly = "Gsmu.Api",
           string ns = "Gsmu.Api.Data.School.Entities",
           string contextName = "SchoolEntities",
           string entity = "District",
           int page = 1,
           int limit = 10,
           string sort = null,
           string filter = null,
           string columns = null)
        {
            var result = GetReadResult(assembly, ns, contextName, entity, page, limit, sort, filter);
            var fileData = export.ExtJsCsvExport.GenerateCvsFile(result, columns);
            var fileDataBinary = System.Text.Encoding.UTF8.GetBytes(fileData);
            return File(fileDataBinary, "text/csv", "data-export-result.csv");
        }

 
    }
}