using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Networking.Mail
{
    public enum MailHandler
    {
        DotNet = 0,
        JMail = 1,
        EAMail = 7
    }
    public enum StudentRosterMailType
    {
        Confirmation = 0,
        Cancellation = 1,
        WaitListToConfirmation = 2
    }
}
