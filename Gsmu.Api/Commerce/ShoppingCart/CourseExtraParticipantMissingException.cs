using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Commerce.ShoppingCart
{
    public class CourseExtraParticipantMissingException : Exception
    {

        public CourseExtraParticipantMissingException(string message)
            : base(message)
        {

        }
    }
}
