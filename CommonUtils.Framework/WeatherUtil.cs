/*
 * 中国天气网
 * http://flash.weather.com.cn/wmaps/xml/shanghai.xml
 * 
 * 中央气象台
 * http://www.nmc.cn/publish/forecast/ASH/shanghai.html
 * http://www.nmc.cn/f/rest/province
 * http://www.nmc.cn/f/rest/province/ASH
 * http://www.nmc.cn/f/rest/tempchart/58367
 * http://www.nmc.cn/f/rest/real/58367
 * http://www.nmc.cn/f/rest/aqi/58367
 * 
 * json在线 好像有坑 访问多少次后会封锁 好像好像 需要测试
 * http://t.weather.sojson.com/api/weather/city/101130501
 * 
 * 和风天气
 * https://free-api.heweather.net/s6/weather/forecast?location=91.1765600195137,43.1740055728993&key=c5abad46f687462f9fbe5f7ad0a33528
 * 
 */

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommonUtils
{
    /// <summary>
    /// 获取天气    
    /// </summary>
    public class WeatherUtil
    {
        /// <summary>
        /// 获取天气信息,缺少空气信息(假)
        /// </summary>
        public static Weather Get(string cityPinyin = "shanghai")
        {
            string url = string.Format("http://flash.weather.com.cn/wmaps/xml/{0}.xml", cityPinyin);
            try
            {
                var text = HttpUtil.GetString(url);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(text);
                var element = xml.GetElementsByTagName("city")[2];
                Weather weather = new Weather()
                {
                    City = element.Attributes["cityname"].Value,
                    Desc = element.Attributes["stateDetailed"].Value,
                    TempHigh = element.Attributes["tem1"].Value.ToInt(),
                    TempLow = element.Attributes["tem2"].Value.ToInt(),
                    Temp = element.Attributes["temNow"].Value.ToInt(),
                    Wind = element.Attributes["windDir"].Value + " " + element.Attributes["windPower"].Value,
                    Humidity = StringUtil.Remove(element.Attributes["humidity"].Value, '%').ToInt(),
                    Time = element.Attributes["time"].Value,
                    Air = "优"
                };
                weather.SortTemp();
                return weather;
            }
            catch (Exception ex)
            {
                Console.WriteLine("天气信息获取失败!");
                Console.WriteLine("Api:" + url);
                Console.WriteLine("Error:" + ex.Message);
                return null;
            }
        }
    }
}
