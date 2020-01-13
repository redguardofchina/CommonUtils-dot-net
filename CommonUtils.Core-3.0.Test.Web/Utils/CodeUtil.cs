using CommonUtils.Test.Web;
using CommonUtils.Test.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web
{
    public static class CodeUtil
    {
        public static string GetConsumed(bool state = true)
        {
            return DbList.DbContextOfInit.Codes.Where(m => m.Consumed == state).ToJson();
        }

        public static void Generate()
        {
            var dbContext = DbList.DbContextOfInit;
            for (int index = 0; index < 10; index++)
            {
                dbContext.Add(new Code
                {
                    Value = StringUtil.GetGuid(),
                    CreateTime = DateTime.Now
                });
            }
            dbContext.SaveChanges();
        }

        public static void Clear()
        {
            var dbContext = DbList.DbContextOfInit;
            dbContext.RemoveRange(DbList.DbContextOfInit.Codes);
            dbContext.SaveChanges();
        }
    }
}
