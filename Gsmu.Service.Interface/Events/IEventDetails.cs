using Gsmu.Service.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Interface
{
    public interface  IEventDetails
    {
        EventDetailsModel GetEventDetails(int Eventid,bool isAdmin=false);
    }
}
