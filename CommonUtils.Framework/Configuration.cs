using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

namespace CommonUtils
{
    public static class Configuration
    {
        #region 通用操作
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static string mConfigPath { get; set; }

        /// <summary>
        /// 配置文件单例,根节点
        /// </summary>
        private static XmlDocument mXmlDocument { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void InitFile()
        {
            //获取配置文件路径
            string appDirPath = PathUtil.Base;
            mConfigPath = appDirPath + "Web.config";
            if (!FileUtil.Exists(mConfigPath))
                mConfigPath = appDirPath + "App.config";

            if (!FileUtil.Exists(mConfigPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(appDirPath);
                foreach (FileInfo fileInfo in dirInfo.GetFiles())
                {
                    if (fileInfo.Extension == ".config" && !fileInfo.Name.Contains("vshost"))
                    {
                        mConfigPath = fileInfo.FullName;
                        break;
                    }
                }
            }
            if (!FileUtil.Exists(mConfigPath))
            {
                string configXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration><appSettings><add key=\"@key\" value=\"@value\" /></appSettings></configuration>";
                FileUtil.Save(mConfigPath, configXml);
            }

            //载入配置文件
            mXmlDocument = new XmlDocument();
            if (FileUtil.Exists(mConfigPath))
            {
                mXmlDocument.Load(mConfigPath);
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        private static void Save()
        {
            mXmlDocument.Save(mConfigPath);
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        private static XmlNode CreateNode(string tag, MapStringString attributes = null)
        {
            XmlNode node = mXmlDocument.CreateNode(XmlNodeType.Element, tag, null);
            if (attributes != null)
                node.AddAttributes(attributes);
            return node;
        }

        /// <summary>
        /// 获取或添加子节点
        /// </summary>
        private static XmlNode GetOrAddChildNode(this XmlNode node, string tag)
        {
            if (node.HasChildNodes)
                foreach (XmlNode childNode in node.ChildNodes)
                    if (childNode.LocalName == tag)
                        return childNode;

            XmlNode newChildNode = mXmlDocument.CreateNode(XmlNodeType.Element, tag, null);
            node.AppendChild(newChildNode);
            return newChildNode;
        }

        /// <summary>
        /// 获取子结点集
        /// </summary>
        private static List<XmlNode> GetChildNodes(this XmlNode node, string tag)
        {
            List<XmlNode> childNodes = new List<XmlNode>();
            if (node.HasChildNodes)
                foreach (XmlNode childNode in node.ChildNodes)
                    if (childNode.LocalName == tag)
                        childNodes.Add(childNode);
            return childNodes;
        }

        /// <summary>
        /// 获取或添加树形节点,节点之间用'-'连接,以configuration开头
        /// </summary>
        private static XmlNode GetOrAddTreeNode(string treeTag)
        {
            string[] tags = StringUtil.ExSplit(treeTag, '-');
            XmlNode node = mXmlDocument;
            for (int index = 0; index < tags.Length; index++)
                node = node.GetOrAddChildNode(tags[index]);
            return node;
        }

        /// <summary>
        /// 节点添加属性
        /// </summary>
        private static void AddAttributes(this XmlNode node, MapStringString attributes)
        {
            foreach (var attribute in attributes)
            {
                XmlAttribute xmlAttribute = mXmlDocument.CreateAttribute(attribute.Key);
                xmlAttribute.Value = attribute.Value;
                node.Attributes.Append(xmlAttribute);
            }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        private static void AddChildNode(this XmlNode node, XmlNode childNode)
        {
            //判断/防重
            bool ifNeedAdd = true;

            for (int index = 0; index < node.ChildNodes.Count; index++)
                if (node.ChildNodes[index].OuterXml == childNode.OuterXml)
                {
                    ifNeedAdd = false;
                    break;
                }

            //添加
            if (ifNeedAdd)
                node.AppendChild(childNode);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        private static void AddNode(string containerTreeTag, string tag, MapStringString attributes)
        {
            //定义
            XmlNode node = mXmlDocument.CreateNode(XmlNodeType.Element, tag, null);
            AddAttributes(node, attributes);

            //附加
            XmlNode containerNode = GetOrAddTreeNode(containerTreeTag);
            containerNode.AddChildNode(node);
        }
        #endregion

        #region ConnectionString
        /// <summary>
        /// 获取配置文件中的ConnectionString
        /// </summary>
        public static ConnectionStringSettings GetConnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name];
        }

        /// <summary>
        /// 获取配置文件中的ConnectionString
        /// </summary>
        public static ConnectionStringSettings GetConnString(int index = 0)
        {
            index += 2;//API如此,位置需要+2
            return ConfigurationManager.ConnectionStrings[index];
        }
        #endregion

        #region AppSetting

        /*由于AppSetting可能会实时更新,故不使用ConfigurationManager,用Xml通用操作处理*/

        /// <summary>
        /// AppSettings
        /// </summary>
        private static MapStringString mAppSettings { get; set; } = new MapStringString();


        /// <summary>
        /// 从配置文件获取调试状态
        /// </summary>
        public static bool IsDebug { get; set; }

        /// <summary>
        /// 初始化AppSetting
        /// </summary>
        public static void InitAppSetting()
        {
            var containerNode = GetOrAddTreeNode("configuration-appSettings");
            var childNodes = containerNode.GetChildNodes("add");
            foreach (var childNode in childNodes)
                mAppSettings.Set(childNode.Attributes["key"].Value, childNode.Attributes["value"].Value);

            //获取调试状态
            IsDebug = ConvertUtil.ToBool(mAppSettings.Get("IsDebug", "false", false));
        }

        /// <summary>
        /// 获取配置文件中的AppSetting
        /// </summary>
        public static string GetAppSetting(string key, bool log = true)
        {
            return mAppSettings.Get(key, null, log);
        }

        /// <summary>
        /// 删除配置文件中的AppSetting
        /// </summary>
        public static void RemoveAppSetting(string key)
        {
            var containerNode = GetOrAddTreeNode("configuration-appSettings");
            var childNodes = containerNode.GetChildNodes("add");
            foreach (var childNode in childNodes)
                if (childNode.Attributes["key"].Value == key)
                    containerNode.RemoveChild(childNode);
            Save();
        }

        /// <summary>
        /// 修改配置文件的APP设置
        /// </summary>
        public static void SetAppSetting(string key, string value)
        {
            RemoveAppSetting(key);

            MapStringString attributes = new MapStringString("key", key);
            attributes.Add("value", value);
            var node = CreateNode("add", attributes);
            var containerNode = GetOrAddTreeNode("configuration-appSettings");
            containerNode.AddChildNode(node);

            Save();
        }

        /// <summary>
        /// 添加禁止BroswerLink的Setting
        /// </summary>
        public static void DisableBrowserLink()
        {
            SetAppSetting("vs:EnableBrowserLink", "false");
        }
        #endregion

        #region 其他
        /// <summary>
        /// 添加允许下载的文件
        /// </summary>
        public static void AddMimes(FileInfo[] fileInfos)
        {
            List<string> fileNames = new List<string>();
            foreach (FileInfo fileInfo in fileInfos)
                fileNames.Add(fileInfo.Name);
            AddMimes(fileNames.ToArray());
        }

        /// <summary>
        /// 添加允许下载的文件
        /// </summary>
        public static void AddMimes(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                //容器节点
                XmlNode containerNode = GetOrAddTreeNode("configuration-system.webServer-staticContent");

                string fileExtension = FileUtil.GetExtension(fileName);
                MapStringString attributes = new MapStringString("fileExtension", fileExtension);
                XmlNode removeNode = CreateNode("remove", attributes);
                containerNode.AddChildNode(removeNode);

                attributes.Add("mimeType", "application/octet-stream");//通用下载格式
                XmlNode mimeMapNode = CreateNode("mimeMap", attributes);
                containerNode.AddChildNode(mimeMapNode);
            }
            Save();
        }

        /// <summary>
        /// 设置主页
        /// </summary>
        public static void SetHomePage(string homePage)
        {
            XmlNode node = GetOrAddTreeNode("configuration-system.webServer-defaultDocument-files-add");
            node.AddAttributes(new MapStringString("value", homePage));
            Save();
        }

        /// <summary>
        /// 设置错误模式
        /// </summary>
        public static void DisplayError()
        {
            XmlNode node = GetOrAddTreeNode("configuration-system.web-customErrors");
            node.AddAttributes(new MapStringString("mode", "Off"));
            Save();
        }

        /// <summary>
        /// 设置错误页
        /// </summary>
        public static void SetErrorPage(string errorPage)
        {
            XmlNode node = GetOrAddTreeNode("configuration-system.web-customErrors");
            node.AddAttributes(new MapStringString("mode", "RemoteOnly"));

            node = GetOrAddTreeNode("configuration-system.web-customErrors-error");
            MapStringString map = new MapStringString("statusCode", "404");
            map.Add("redirect", errorPage);
            node.AddAttributes(map);

            Save();
        }

        /// <summary>
        /// 设置错误页
        /// </summary>
        public static void SetRelocation(string url)
        {
            XmlNode node = GetOrAddTreeNode("configuration-system.webServer-httpRedirect");
            MapStringString map = new MapStringString("enabled", "true");
            map.Add("destination", url);
            node.AddAttributes(map);
            Save();
        }

        /// <summary>
        /// 刷新发布时间，以便更改config，从而强制重启
        /// </summary>
        public static void RefreshPublishTime(bool responseInfo = false)
        {
            SetAppSetting("PublishTime", DateTime.Now.Info());
            if (responseInfo)
                ResponseUtil.Write("更新了发布时间");
        }
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        static Configuration()
        {
            InitFile();
            InitAppSetting();
        }
    }
}
