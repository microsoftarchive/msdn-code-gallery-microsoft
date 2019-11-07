using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSAccessViewFromSqlServer.ViewProvider
{
    public class NoWatchChangeToken : IChangeToken
    {
        public bool ActiveChangeCallbacks
        {
            get { return false; }
        }

        public bool HasChanged
        {
            get { return false; }
        }

        public IDisposable RegisterChangeCallback(Action<Object> callback, object state)
        {
            return new EmptyDisposable();
        }
    }
}
