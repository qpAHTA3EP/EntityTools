using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mount_Tutorial
{

    interface IMessage
    {
        IAsyncResult Ready { get; set; }
        TResult Result<TResult>();
    }
}
