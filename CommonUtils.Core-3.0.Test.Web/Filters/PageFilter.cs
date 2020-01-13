using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web.Filters
{
    public class PageFilter : IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            ReflectionUtil.CurrentMethodName().Print();
        }
    }
}
