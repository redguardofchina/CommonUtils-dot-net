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
    public static class CoreUtil
    {
        /// <summary>
        /// 解除大文件上传限制
        /// </summary>
        public static IWebHostBuilder CancelSizeLimit(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = long.MaxValue;
                options.Limits.MaxRequestBufferSize = long.MaxValue;
                options.Limits.MaxRequestLineSize = int.MaxValue;
            });

            //如果没有这句话，IIS模式下.UseKestrel会引发500.3异常
            webHostBuilder.UseIIS();

            return webHostBuilder;
        }
    }
}
