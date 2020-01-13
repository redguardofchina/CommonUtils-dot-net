using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Test.Web.Api
{
    /// <summary>
    /// 默认接口
    /// </summary>
    [Route("api/[action]")]
    public class _DefaultController : ControllerBase
    {
        [HttpGet]
        public IActionResult HttpGetTest()
        {
            return ApiResult.Ok(0, Request.GetUrl());
        }

        [HttpPost]
        public IActionResult HttpPostTest()
        {
            return ApiResult.Ok(0, Request.GetUrl());
        }

        [HttpDelete]
        public IActionResult HttpDeleteTest()
        {
            return ApiResult.Ok(0, Request.GetUrl());
        }

        [HttpGet]
        public IActionResult Hello()
        {
            return ApiResult.Ok(0, Request.GetUrlHead());
        }

        /// <summary>
        /// Base64字符串解码
        /// </summary>
        [HttpPost]
        public IActionResult Base64Decode(string base64)
        {
            return Ok(base64.Base64Decode());
        }

        /// <summary>
        /// 时间转换为时间戳
        /// </summary>
        [HttpPost]
        public IActionResult ConvertToTime(long timestamp)
        {
            return Ok(TimeUtil.FromStamp(timestamp));
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        [HttpGet]
        public IActionResult GetNowTimestamp()
        {
            return Ok(DateTime.Now.Stamp());
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        [HttpGet]
        public IActionResult GetNowLongTimestamp()
        {
            return Ok(DateTime.Now.LongStamp());
        }

        /// <summary>
        /// 获取天气信息
        /// </summary>
        [HttpGet]
        public IActionResult GetWeather()
        {
            return ApiResult.Ok(WeatherUtil.Get());
        }
    }
}