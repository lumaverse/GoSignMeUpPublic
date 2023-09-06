using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Data.Entity.Infrastructure;

using entity = System.Data.Entity;
using school = Gsmu.Api.Data.School.Entities;
using lang = Gsmu.Api.Language;

namespace Gsmu.Api.Data
{
    public class DataStoreHelper<T> : IDataStoreHelper where T : class, new()
    {
        public static IDataStoreHelper GetDataStore(string assembly, string ns, string contextName, string entity)
        {
            var entityType = Type.GetType(ns + "." + entity + ", " + assembly);
            var dataStoreType = typeof(DataStoreHelper<>);
            var genericDataStoreType = dataStoreType.MakeGenericType(entityType);
            object ds = Activator.CreateInstance(genericDataStoreType, genericDataStoreType, assembly, ns, contextName, entity);
            return ds as IDataStoreHelper;
        }

        public entity.DbContext Context { get; private set; }

        public Type DataStoreType { get; private set; }
        public string AssemblyName {get; private set;}
        public string NameSpace { get; private set; }
        public string ContextName { get; private set; }
        public string EntityName { get; private set; }
        public string PrimaryKeyName { get; private set; }        
        public Type EntityType { get; private set; }

        public DataStoreHelper(Type dataStoreType, string assemblyName, string nameSpace, string contextName, string entityName)
        {
            this.DataStoreType = dataStoreType;
            this.AssemblyName = assemblyName;
            this.NameSpace = nameSpace;
            this.EntityName = entityName;
            this.ContextName = contextName;
            this.EntityType = typeof(T);

            Context = Activator.CreateInstance(Type.GetType(nameSpace + "." + ContextName + ", " + assemblyName)) as entity.DbContext;
            Context.Configuration.ProxyCreationEnabled = false;
            Context.Configuration.LazyLoadingEnabled = false;

            var set = ((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<T>();
            var entitySet = set.EntitySet;
            PrimaryKeyName = entitySet.ElementType.KeyMembers.First().Name;            
        }

        public DataStoreResult List(int page = 1, int pageSize = 0, string sorters = null, string filters = null)
        {

            page = page < 1 ? 1 : page;

            var query = (from e in Context.Set<T>() select e);

            var sorterDetails = ExtJsDataStoreHelper.ParseSorter(sorters);
            foreach (var sort in sorterDetails)
            {
                query = query.OrderBy(sort.Key + " " + sort.Value.ToString());
            }
            var entityType = typeof(T);

            var filterDetails = ExtJsDataStoreHelper.ParseFilter(filters);
            if (filterDetails != null) {
                string whereString = null;
                List<object> whereParams = new List<object>();
                int counter = 0;
                foreach (string filterKey in filterDetails.Keys)
                {
                    //entityType.GetProperty("districtID").PropertyType.Name
                    var propertyType = entityType.GetProperty(filterKey).PropertyType;
                    var nullableType = Nullable.GetUnderlyingType(propertyType);
                    propertyType = nullableType ?? propertyType;
                    string whereDetails = string.Empty;
                    if (nullableType == null)
                    {
                        whereDetails = string.Format("{0}.ToString().Contains(@{1})", filterKey, counter);
                    }
                    else
                    {
                        whereDetails = string.Format("{0}.Value.ToString().Contains(@{1})", filterKey, counter);
                    }

                    if (whereString == null)
                    {
                        whereString = whereDetails;
                    }
                    else
                    {
                        whereString += " OR " + whereDetails;
                    }

                    whereParams.Add(filterDetails[filterKey]);
                    counter++;
                }
                query = query.Where(whereString, whereParams.ToArray());
            }

            var count = query.Count();

            if (pageSize > 0)
            {
                var start = (page - 1) * pageSize;
                query = query.Skip(start).Take(pageSize);
            }

            var result = new DataStoreResult()
            {
                Count = count,
                Data = query.ToList().Cast<object>().ToList()
            };
            return result;
        }

        public T GetEntityByPrimaryKey(Dictionary<string, string> data)
        {
            var query = (from e in Context.Set<T>() select e);
            var pk = data[PrimaryKeyName];

            query = query.Where(
                string.Format(
                    "{0} = {1}",
                    PrimaryKeyName,
                    pk
                )
            );
            var entity = query.First();
            return entity;
        }

        public void Update(Dictionary<string, string> data)        
        {
            var entity = GetEntityByPrimaryKey(data);

            data.Remove(PrimaryKeyName);
            
            var entry = Context.Entry(entity);

            foreach (var key in data.Keys)
            {
                var value = data[key];
                var memberType = this.EntityType.GetProperty(key).PropertyType;
                entry.CurrentValues[key] = lang.ConvertHelper.ChangeTypeForNullable(value, memberType);
            }
            Context.SaveChanges();
        }

        public void Delete(Dictionary<string, string> data)
        {
            var entity = GetEntityByPrimaryKey(data);            
            Context.Set<T>().Remove(entity);
            Context.SaveChanges();
        }

        public void Create(Dictionary<string, string> data)
        {
            data.Remove(PrimaryKeyName);
            var entity = new T();
            Context.Set<T>().Add(entity);
            var entry = Context.Entry(entity);

            foreach (var key in data.Keys)
            {
                var value = data[key];
                var memberType = this.EntityType.GetProperty(key).PropertyType;
                entry.CurrentValues[key] = lang.ConvertHelper.ChangeTypeForNullable(value, memberType);
            }
            Context.SaveChanges();
        }

    }
}
