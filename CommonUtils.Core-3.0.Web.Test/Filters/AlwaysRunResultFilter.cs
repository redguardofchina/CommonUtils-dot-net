using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web.Filters
{
    public class AlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }
    }
}
