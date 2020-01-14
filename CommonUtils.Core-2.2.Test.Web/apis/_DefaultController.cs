using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Test.Web.apis
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class _DefaultController : ControllerBase
    {
        /// <summary>
        /// 获取天气信息
        /// </summary>
        [HttpGet]
        public IActionResult GetWeather(string city = "shanghai")
        {
            return ApiResult.Ok(WeatherUtil.Get(city));
        }
    }
}