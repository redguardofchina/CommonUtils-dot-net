using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web.Models
{
    public class Code
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Consumed { get; set; }
        public DateTime ConsumTime { get; set; }
    }
}
