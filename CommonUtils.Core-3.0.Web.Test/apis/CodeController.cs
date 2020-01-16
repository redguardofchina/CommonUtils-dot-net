using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonUtils.Test.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Test.Web.Api
{
    /// <summary>
    /// 随机码
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class CodeController : ControllerBase
    {
        [HttpGet]
        public void Generate()
        {
            CodeUtil.Generate();
        }

        [HttpGet]
        public void Clear()
        {
            CodeUtil.Clear();
        }
    }
}