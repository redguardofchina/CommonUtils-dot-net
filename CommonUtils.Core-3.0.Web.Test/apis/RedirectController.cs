using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Test.Web.Api
{
    /// <summary>
    /// 页面重定位
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class RedirectController : Controller
    {
        [HttpGet]
        public IActionResult CodeDeal()
        {
            return Redirect("/CodeDeal");
        }

        [HttpGet]
        public IActionResult OrderAndFilter()
        {
            return Redirect("/OrderAndFilter");
        }

        [HttpGet]
        public IActionResult Repeater()
        {
            return Redirect("/Repeater");
        }
    }
}