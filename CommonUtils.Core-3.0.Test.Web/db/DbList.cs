using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CommonUtils.Test.Web
{
    /// <summary>
    /// 数据库列表
    /// </summary>
    public class DbList
    {
        public static DefaultDbContext DbContextOfInit { get; set; }

        private static DefaultDbContext GetDbContextFromServices()
        => CoreUtil.GetFromServices<DefaultDbContext>();

        private static DefaultDbContext GetDbContextFromBuilder()
        => CoreUtil.GetFromApplicationBuilder<DefaultDbContext>();

        public static DefaultDbContext GetDbContext() => GetDbContextFromServices();
    }
}
