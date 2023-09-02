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

namespace Gsmu.Web.Controllers
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

    }
}