using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace CSAccessViewFromSqlServer.ViewProvider
{
    public class DBViewProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            string path = ConvertPath(subpath);

            return new DBViewDirectoryContents(path);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            string path = ConvertPath(subpath);

            return new DBFileInfo(path);
        }

        public IChangeToken Watch(string filter)
        {
            return new NoWatchChangeToken();
        }

        private string ConvertPath(string path)
        {
            if (path.StartsWith("/Views/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(7);
            }
            if (path.StartsWith("Views/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(6);
            }
            if (path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(1);
            }
            return path;
        }
    }
}
