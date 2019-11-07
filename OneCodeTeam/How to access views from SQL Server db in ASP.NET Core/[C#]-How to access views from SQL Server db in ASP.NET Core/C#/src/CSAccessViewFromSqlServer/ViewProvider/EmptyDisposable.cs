using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSAccessViewFromSqlServer.ViewProvider
{
    public class EmptyDisposable: IDisposable
    {
        public void Dispose() { }
    }
}
