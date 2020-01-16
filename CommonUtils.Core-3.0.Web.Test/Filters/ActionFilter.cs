using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web.Filters
{
    public class ActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }
    }
}
