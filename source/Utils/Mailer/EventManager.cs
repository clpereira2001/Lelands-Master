using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniMail
{
    public class OperationCompletedEventArgs : EventArgs 
    {
        public OperationCompletedEventArgs(string MsgId, bool IsSent)
        {
            IsSucceed = IsSent;
            MessageId = MsgId;
        }

        public bool   IsSucceed;
        public string MessageId;
    }

    class MailEventManager
    {
    }
}
