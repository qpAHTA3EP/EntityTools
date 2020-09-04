using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mount_Tutorial.Core
{
    internal class QuesterActionRequest<QAction, Result> : IMessage
    {
        internal bool Ready = false;
        internal QAction action;
        internal Result result;

        QuesterActionRequest(QAction a)
        {
            action = a;
        }

        IAsyncResult IMessage.Ready { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        TResult IMessage.Result<TResult>()
        {
            throw new NotImplementedException();
        }
    }
}
