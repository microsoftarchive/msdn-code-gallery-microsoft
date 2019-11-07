using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace CSAccessViewFromSqlServer.ViewProvider
{
    public class DBFileInfo : IFileInfo
    {
        private View _view;

        public DBFileInfo(string path)
        {
            using (ViewDBContext db = new ViewDBContext())
            {
                this._view = db.Views.FirstOrDefault(m => m.Path == path);
            }
        }

        public DBFileInfo(View view)
        {
            _view = view;
        }

        public bool Exists
        {
            get { return _view != null; }
        }

        public bool IsDirectory
        {
            get { return _view != null ? _view.IsDirectory : false; }
        }

        public DateTimeOffset LastModified
        {
            get { return _view != null ? _view.LastUpdateTime : DateTimeOffset.MinValue; }
        }

        public long Length
        {
            get
            {
                if (_view != null && _view.Content != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(_view.Content);
                    return bytes.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string Name
        {
            get
            {
                if (_view != null && _view.Path != null)
                {
                    return _view.Path;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string PhysicalPath
        {
            get
            {
                return string.Empty;
            }
        }

        public Stream CreateReadStream()
        {
            if (_view != null && _view.Content != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(_view.Content);
                MemoryStream ms = new MemoryStream(bytes);
                return ms;
            }
            else
            {
                return null;
            }
        }
    }
}
