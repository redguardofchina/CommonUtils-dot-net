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
            //todo �ӿ����Ȩ�ޣ����Ʒ��ʣ�������ԭ������ʵ��
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
            //ԭ��Json�������ġ�������ע��֧�ֲ�����л�ΪNewtonsoft�ṩ��Json���񣬱�������Microsoft.AspNetCore.Mvc.NewtonsoftJson������Newtonsoft.Json�˴���Ч
            .AddNewtonsoftJson(options =>
            {
                //����Json����/������Сд
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                // ����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // ����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // ���ֶ�Ϊnullֵ�����ֶβ��᷵�ص�ǰ��
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

            //ע��
            services.AddDbContext<DefaultDbContext>(options => options.UseSqlite(connection));
            DbList.DbContextOfInit = services.Get<DefaultDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CoreUtil.ApplicationBuilder = app;

            //���ų���������һ������Ǩ��
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
