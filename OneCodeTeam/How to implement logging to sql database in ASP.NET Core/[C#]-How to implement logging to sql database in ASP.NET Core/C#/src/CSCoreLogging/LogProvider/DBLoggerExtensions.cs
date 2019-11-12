using CSCoreLogging.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSCoreLogging.LogProvider
{
    public static class DBLoggerExtensions
    {
        public static ILoggerFactory AddContext(this ILoggerFactory factory,
        Func<string, LogLevel, bool> filter = null, string connectionStr = null)
        {
            factory.AddProvider(new DBLoggerProvider(filter, connectionStr));
            return factory;
        }

        public static ILoggerFactory AddContext(this ILoggerFactory factory, LogLevel minLevel, string connectionStr)
        {
            return AddContext(
                factory,
                (_, logLevel) => logLevel >= minLevel, connectionStr);
        }
    }

}
