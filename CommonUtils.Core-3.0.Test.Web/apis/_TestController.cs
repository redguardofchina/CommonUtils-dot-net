using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CommonUtils.Test.Web.Api
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class _TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Hello()
        {
            return Ok("hello");
        }

        /// <summary>
        /// 文件上传测试
        /// </summary>
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            return Ok(file.FileName);
        }

        /// <summary>
        /// 文件上传测试
        /// </summary>
        [HttpPost]
        public IActionResult UploadNoButton(IFormFile file)
        {
            return Ok(file.FileName);
        }

        [HttpGet]
        public IActionResult Download()
        {
            var stream = new FileStream("c:/temp/test.txt", FileMode.Open);
            return File(stream, "text/plain", "b.txt");
        }

        /// <summary>
        /// 无符号整数测试
        /// </summary>
        [HttpPost]
        public IActionResult Digit(uint value)
        {
            return ApiResult.Ok(value);
        }

        /// <summary>
        /// 多参数上传
        /// </summary>
        [HttpGet("{value1}/{value2}")]
        public IActionResult MutiGet(string value1, string value2 = "100")
        {
            var res = new JObject();
            res.Put("value1", value1);
            res.Put("value2", value2);
            return ApiResult.Ok(res);
        }

        /// <summary>
        /// 多参数上传
        /// </summary>
        [HttpPost]
        public IActionResult MutiPost(string value1, string value2 = "100")
        {
            var res = new JObject();
            res.Put("value1", value1);
            res.Put("value2", value2);
            return ApiResult.Ok(res);
        }
    }
}