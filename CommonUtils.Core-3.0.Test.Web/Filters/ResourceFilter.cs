using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web.Filters
{
    public class ResourceFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }
    }
}
