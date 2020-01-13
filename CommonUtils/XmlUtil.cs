using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace CommonUtils
{
    public static class XmlUtil
    {
        #region 初始化
        /// <summary>
        /// 新建xml
        /// </summary>
        public static XmlDocument New()
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            return xml;
        }

        /// <summary>
        /// 初始化xml
        /// </summary>
        public static XmlDocument Load(Stream stream)
        {
            var xml = new XmlDocument();
            xml.Load(stream);
            return xml;
        }

        /// <summary>
        /// 初始化xml
        /// </summary>
        public static XmlDocument LoadFile(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);
            return xml;
        }

        /// <summary>
        /// 初始化xml
        /// </summary>
        public static XmlDocument LoadXml(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc;
        }
        #endregion

        #region 取值
        /// <summary>
        /// 转换为json
        /// </summary>
        public static object ConvertToJson(XmlDocument xml)
        {
            return JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.Indented, true);
        }

        /// <summary>
        /// 获取某一节点的所有元素的属性字典
        /// </summary>
        public static MapStringString GetMap(string xmlPath, string nodeTag, string nodeKey, string nodeValue, string elementTag, string elementKey1, string elementKey2)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);
            var nodes = xml.GetElementsByTagName(nodeTag);
            var node = GetNodeByAttr(nodes, nodeKey, nodeValue);
            MapStringString map = new MapStringString();
            foreach (XmlNode element in node.ChildNodes)
                if (element.Name == elementTag)
                    map.Add(element.Attributes.GetValue(elementKey1), element.Attributes.GetValue(elementKey2));
            return map;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        private static string GetValue(this XmlAttributeCollection attrs, string key)
        {
            foreach (XmlAttribute attr in attrs)
                if (attr.Name == key)
                    return attr.Value;
            return null;
        }

        /// <summary>
        /// 根据属性获取节点
        /// </summary>
        private static XmlNode GetNodeByAttr(XmlNodeList nodes, string key, string value)
        {
            foreach (XmlNode node in nodes)
                if (node.Attributes.GetValue(key) == value)
                    return node;
            return null;
        }
        #endregion

        #region 操作
        public static XmlElement AddElement(this XmlDocument xml, string tag)
        {
            return xml.AppendChild(xml.CreateElement(tag)) as XmlElement;
        }

        public static XmlElement AddElement(this XmlElement element, string tag)
        {
            return element.AppendChild(element.OwnerDocument.CreateElement(tag)) as XmlElement;
        }

        public static void SetAttribute(this XmlElement element, string key, object value, bool filterEmpty = false)
        {
            string valueString = "";
            if (value != null)
                valueString = value.ToString();
            if (filterEmpty && string.IsNullOrEmpty(valueString))
                return;
            element.SetAttribute(key, valueString);
        }

        public static void AddAttribute(this XmlElement element, string key, object value, bool filterEmpty = false)
        {
            element.SetAttribute(key, value);
        }
        #endregion
    }
}
