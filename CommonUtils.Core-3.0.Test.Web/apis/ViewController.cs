using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Test.Web.Api
{
    public class ViewController : ControllerBase
    {
        /// <summary>
        /// 无感转发
        /// </summary>
        [HttpGet("{view}_view/{id}")]
        public IActionResult View(string view, string id)
        {
            var url = Request.UrlRoot(view + "_view?id=" + id);
            var html = HttpUtil.GetString(url);
            return Content(html, ContentType.Html, Encoding.UTF8);
        }
    }
}