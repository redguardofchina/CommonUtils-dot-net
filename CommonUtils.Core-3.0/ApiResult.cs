using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// Api返回结果
    /// </summary>
    public class ApiResult
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ApiResult(HttpStatusCode code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        ///// <summary>
        ///// 标准数据结构
        ///// </summary>
        //public static ApiResult GetStandardStruct(int code = 0, string message = _successMessage, object data = null)
        //=> new ApiResult(0, _successMessage, data);

        //字段
        private static string _fieldCode = "Code";
        private static string _fieldMessage = "Message";
        private static string _fieldData = "Data";

        //todo intCode - HttpStatusCode

        /// <summary>
        /// 标准数据结构
        /// </summary>
        public static JObject GetStandardStruct(int code = 0, string message = _successMessage, object data = null)
        => new JObject { { _fieldCode, code }, { _fieldMessage, message }, { _fieldData, data.ToJToken() } };

        /// <summary>
        /// 消息
        /// </summary>
        private const string _successMessage = "Success";

        /// <summary>
        /// 正常
        /// </summary>
        public static OkObjectResult OkResult { get; private set; }

        /// <summary>
        /// 异常
        /// </summary>
        public static OkObjectResult ErrorResult { get; private set; }

        /// <summary>
        /// 未登录
        /// </summary>
        public static OkObjectResult Unlogin { get; private set; }

        /// <summary>
        /// 参数异常
        /// </summary>
        public static OkObjectResult ArgsError { get; private set; }

        /// <summary>
        /// 参数异常
        /// </summary>
        public static OkObjectResult ParamsError { get { return ArgsError; } }

        /// <summary>
        /// 无数据
        /// </summary>
        public static OkObjectResult NoDataResult { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
            OkResult = new OkObjectResult(GetStandardStruct());
            ErrorResult = new OkObjectResult(GetStandardStruct(-1, "服务器异常"));
            Unlogin = new OkObjectResult(GetStandardStruct(-2, "登录状态失效"));
            ArgsError = new OkObjectResult(GetStandardStruct(-3, "参数异常"));
            NoDataResult = new OkObjectResult(GetStandardStruct(-4, "无数据"));
        }

        /// <summary>
        /// 构造
        /// </summary>
        static ApiResult()
        {
            Init();
        }

        /// <summary>
        /// 重定义字段
        /// </summary>
        public static void SetFields(string fieldCode, string fieldMessage, string fieldData)
        {
            ConsoleUtil.Print("ApiResult SetFields-> Code:{0} Message:{1} Data:{2}", fieldCode, fieldMessage, fieldData);
            _fieldCode = fieldCode;
            _fieldMessage = fieldMessage;
            _fieldData = fieldData;
            Init();
        }

        /// <summary>
        /// 权限不足
        /// </summary>
        public static StatusCodeResult Status401Unauthorized { get; } = new StatusCodeResult(StatusCodes.Status401Unauthorized);

        /// <summary>
        /// 成功
        /// </summary>
        public static StatusCodeResult Status200OK { get; } = new StatusCodeResult(StatusCodes.Status200OK);

        ///// <summary>
        ///// 正常
        ///// </summary>
        //public static IActionResult Ok(string message = null)
        //{
        //    if (string.IsNullOrEmpty(message))
        //        return OkResult;

        //    return new OkObjectResult(GetStandardStruct(0, message));
        //}

        /// <summary>
        /// 正常
        /// </summary>
        public static IActionResult Ok(object data = null)
        {
            var result1 = GetStandardStruct(0, _successMessage, data);
            var result2 = new OkObjectResult(result1);
            return result2;
        }

        public static IActionResult Ok(int code, object data, string msg = _successMessage)
        {
            return new OkObjectResult(GetStandardStruct(code, msg, data));
        }

        /// <summary>
        /// 异常
        /// </summary>
        public static IActionResult Error(string message = null)
        {
            if (string.IsNullOrEmpty(message))
                return ErrorResult;

            return new OkObjectResult(GetStandardStruct(-1, message));
        }

        /// <summary>
        /// 异常
        /// </summary>
        public static IActionResult Error(int code, string message)
        {
            return new OkObjectResult(GetStandardStruct(code, message));
        }
    }
}
