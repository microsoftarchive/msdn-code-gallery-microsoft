using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CSAccessViewFromSqlServer
{
    public class ViewDBContext : DbContext
    {
        public static string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public virtual DbSet<View> Views { get; set; }
    }

    public class View
    {
        public Guid Id { get; set; }

        public string Path { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public bool IsDirectory { get; set; }

        public Guid ParentId { get; set; }
    }
}
