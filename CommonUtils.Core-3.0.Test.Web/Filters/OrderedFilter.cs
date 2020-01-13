using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web.Filters
{
    public class OrderedFilter : IOrderedFilter
    {
        public int Order
        {
            get
            {
                ReflectionUtil.CurrentMethodName().Print();
                return 0;
            }
        }
    }
}
