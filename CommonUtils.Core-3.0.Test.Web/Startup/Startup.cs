using CommonUtils.Test.Web.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonUtils.Test.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //todo 接口添加权限，控制访问，尝试用原生代码实现
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CoreUtil.Services = services;

            services.AddRazorPages();
            services.AddControllers()
            //services.AddControllers(options =>
            //{
            //    options.Filters.Add<ActionFilter>();
            //    options.Filters.Add<AlwaysRunResultFilter>();
            //    options.Filters.Add<AuthorizationFilter>();
            //    options.Filters.Add<ExceptionFilter>();
            //    options.Filters.Add<OrderedFilter>();
            //    options.Filters.Add<PageFilter>();
            //    options.Filters.Add<ResourceFilter>();
            //    options.Filters.Add<ResultFilter>();
            //})
            //原生Json服务中文、变量、注释支持差，这里切换为Newtonsoft提供的Json服务，必须引用Microsoft.AspNetCore.Mvc.NewtonsoftJson，引用Newtonsoft.Json此处无效
            .AddNewtonsoftJson(options =>
            {
                //保持Json属性/变量大小写
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                // 忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // 设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 如字段为null值，该字段不会返回到前端
                // options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; 
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("doc", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "CommonUtils.Test.Web", Version = "v1.0" });
                options.IncludeXmlComments("comment.xml");
            });

            var dbPath1 = PathUtil.GetFull("db/release.db");
            var dbPath2 = PathUtil.GetFull("db/debug.db");

            if (!FileUtil.Exists(dbPath1))
                FileUtil.Copy(dbPath2, dbPath1);

            var connection = DbSqlite.ConnectStrings.FromPath(dbPath1);

            //注册
            services.AddDbContext<DefaultDbContext>(options => options.UseSqlite(connection));
            DbList.DbContextOfInit = services.Get<DefaultDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CoreUtil.ApplicationBuilder = app;

            //趁着程序启动做一下数据迁移
            //DbList.DbContextOfInit = DbList.GetDbContextFromBuilder();
            //DbList.DbContextOfInit.Database.Migrate();

            app.CrossDomain();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(config =>
            {
                config.MapRazorPages();
                config.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/doc/swagger.json", null));
        }
    }
}
