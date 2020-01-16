using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonUtils
{
    public static partial class CoreUtil
    {
        //IServiceCollection中取的数据库单例可不可用？
        //添加/删除可以,如果数据被其他实例修改，缓存与数据库不同步，单例是不知道的，造成数据脏读写
        //sql语句没问题，缓存中数据有问题
        //IApplicationBuilder还未验证，有时初始化会失败，可能紧支持部分环境，比如控制器初始化的时候

        #region IServiceCollection
        public static IServiceCollection Services { get; set; }

        public static T Get<T>(this IServiceCollection services)
        {
            if (services == null)
                throw new ExceptionPlus("Services 未赋值");
            return services.BuildServiceProvider().GetService<T>();
        }

        public static T GetFromServices<T>() => Services.Get<T>();

        /// <summary>
        /// 解除大文件上传限制
        /// </summary>
        public static void CancelSizeLimit(this IServiceCollection services)
        {
            //此处会导致IIS模式下api.Request.Form获取失败
            if (!ApplicationUtil.IsIIS)
            {
                services.Configure<FormOptions>(options =>
               {
                   options.BufferBodyLengthLimit = long.MaxValue;
                   options.KeyLengthLimit = int.MaxValue;
                   options.MultipartBodyLengthLimit = long.MaxValue;
                   options.MultipartBoundaryLengthLimit = int.MaxValue;
                   options.ValueLengthLimit = int.MaxValue;
               });
            }
        }
        #endregion

        #region IApplicationBuilder
        public static T Get<T>(this IApplicationBuilder app)
        => app.ApplicationServices.GetService<T>();

        public static IApplicationBuilder ApplicationBuilder { get; set; }

        public static T GetFromApplicationBuilder<T>()
        {
            if (ApplicationBuilder == null)
                throw new ExceptionPlus("ApplicationBuilder 未赋值");
            return ApplicationBuilder.Get<T>();
        }

        /// <summary>
        /// 跨域 3.0版本后ConfigureServices中需包含services.AddControllers();
        /// </summary>
        public static void CrossDomain(this IApplicationBuilder builder)
        => builder.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyMethod().AllowAnyOrigin());
        #endregion

        #region ContentTypeProvider
        //app.UseStaticFiles(new StaticFileOptions() { ContentTypeProvider = CoreUtil.ContentTypeProvider });
        public static FileExtensionContentTypeProvider ContentTypeProvider = new FileExtensionContentTypeProvider();

        public static Dictionary<string, string> GetMimeMappings()
        => ContentTypeProvider.Mappings as Dictionary<string, string>;

        private static void FormatExtension(ref string extension)
        {
            extension = extension.SubstringEndByFirstKey('.');
            //填空就给个网页默认值
            if (string.IsNullOrEmpty(extension))
                extension = ".htm";
            extension = '.' + extension.ToLower();
        }

        public static bool IsMimeExist(string extension)
        {
            var map = GetMimeMappings();
            FormatExtension(ref extension);
            return map.ContainsKey(extension);
        }

        public static bool AddMime(string extension)
        {
            var map = GetMimeMappings();
            FormatExtension(ref extension);

            if (map.ContainsKey(extension))
                return true;

            map[extension] = "application/octet-stream";
            return true;
        }

        public static void AddMimesByFloder(string floder)
        {
            var map = GetMimeMappings();

            var extensions = FloderUtil.GetFiles(floder).Select(m => m.Extension).Distinct().ToArray();
            foreach (var extension in extensions)
            {
                if (map.ContainsKey(extension))
                    continue;
                map[extension] = "application/octet-stream";
            }
        }

        public static bool RemoveMime(string extension)
        {
            var map = GetMimeMappings();
            FormatExtension(ref extension);

            if (!map.ContainsKey(extension))
                return true;

            map.Remove(extension);
            return true;
        }
        #endregion

        #region IApplicationLifetime
        public static void RegisterStartEvent(this Microsoft.Extensions.Hosting.IApplicationLifetime lifeTime, Action callback)
        => lifeTime.ApplicationStarted.Register(callback);

        public static void RegisterStopEvent(this Microsoft.Extensions.Hosting.IApplicationLifetime lifeTime, Action callback)
        => lifeTime.ApplicationStopping.Register(callback);

        public static Microsoft.Extensions.Hosting.IApplicationLifetime ApplicationLifetime { private get; set; }

        public static void RegisterStart(Action callback)
        {
            if (ApplicationLifetime == null)
                throw new ExceptionPlus("LiveTime 未赋值");
            ApplicationLifetime.RegisterStartEvent(callback);
        }

        public static void RegisterStop(Action callback)
        {
            if (ApplicationLifetime == null)
                throw new ExceptionPlus("LiveTime 未赋值");
            ApplicationLifetime.RegisterStopEvent(callback);
        }
        #endregion

        public static void SaveTo(this IFormFile file, DirectoryInfo floder)
        => FileUtil.Save(floder.Combine(file.FileName), file.OpenReadStream());

        public static void SaveTo(this IFormFile file, string path)
        => file.OpenReadStream().SaveTo(path);

        /// <summary>
        /// 原生的仅支持virtualPath，这个支持磁盘路径
        /// </summary>
        public static IActionResult FileResult(this ControllerBase controller, string path, string contentType = null)
        {
            if (contentType == null)
                contentType = ContentType.Get(path);
            return controller.File(FileUtil.GetStream(path), contentType);
        }
    }
}
