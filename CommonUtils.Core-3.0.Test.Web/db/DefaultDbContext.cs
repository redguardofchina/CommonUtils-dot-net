using CommonUtils.Test.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web
{
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options) { }

        public IQueryable<TEntity> FromSql<TEntity>(string sql) where TEntity : class
        => Set<TEntity>().FromSqlRaw(sql);

        public DbSet<Code> Codes { get; set; }
    }
}
