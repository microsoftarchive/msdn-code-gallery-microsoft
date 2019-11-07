using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace CSAccessViewFromSqlServer.ViewProvider
{
    public class DBViewDirectoryContents : IDirectoryContents
    {
        private View _directoryInfo;

        public DBViewDirectoryContents(string path)
        {
            using (ViewDBContext db = new ViewDBContext())
            {
                this._directoryInfo = db.Views.FirstOrDefault(m => m.Path == path);
            }
        }

        public bool Exists
        {
            get
            {
                return _directoryInfo != null;
            }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            if (_directoryInfo != null)
            {
                using (ViewDBContext db = new ViewDBContext())
                {
                    return db.Views
                        .Where(m => m.ParentId == _directoryInfo.ParentId)
                        .Select(f => new DBFileInfo(f))
                        .GetEnumerator();
                }
            }
            else
            {
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
