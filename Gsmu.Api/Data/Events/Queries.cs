using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.Events.Entities;

namespace Gsmu.Api.Data.Events
{
   public static class Queries
    {
       public static IEnumerable<@event> GetAllEvents()
       {
           try
           {
               using (var db = new EventsEntities())
               {
                   var query = (from a in db.events where (a.Display != "N" || a.Display == null) select a).ToList();
                   return query;

               }
           }
           catch (Exception)
           {
               return null;
           }

       }

       public static @event GetEventDetails(int eventid)
       {
           using (var db = new EventsEntities())
           {
               @event query = (from a in db.events where a.eventid == eventid select a).FirstOrDefault();

               return query;
           }
       }
    }
}
