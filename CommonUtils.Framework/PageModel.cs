using System;
using System.Collections.Generic;

namespace CommonUtils
{
    /// <summary>
    /// 页面
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// 初始化,声明路径标题
        /// </summary>
        /// <param name="path"></param>
        /// <param name="title"></param>
        public PageModel(string path, string title)
        {
            Path = path;
            Title = title;
            Columns = new List<KeyValuePair<string,string>>();
            while (Columns.Count < 15)
            {
                Columns.Add(new KeyValuePair<string,string>("--", "Line"));
            }
            m_columIndex = 0;
            Children = new List<PageModel>();
        }

        /// <summary>
        /// 初始化,声明路径标题,共享键值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="title"></param>
        public PageModel(string path, string title, List<KeyValuePair<string,string>> columns)
        {
            Path = path;
            Title = title;
            Columns = columns;
            Children = new List<PageModel>();
        }

        /// <summary>
        /// 初始化,单标题,多路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        public PageModel(string path, PageModel page)
        {
            Copy(page);
            IfHidden = true;
            Path = path;
        }

        /// <summary>
        /// 初始化,多标题,单路径
        /// </summary>
        /// <param name="page"></param>
        /// <param name="title"></param>
        public PageModel(PageModel page, string title)
        {
            Copy(page);
            Title = title;
        }

        /// <summary>
        /// 初始化,多标题,多路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        public PageModel(string path, string title, PageModel page)
        {
            Copy(page);
            Path = path;
            Title = title;
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IfHidden { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        string m_keywords;
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords
        {
            get
            {
                if (string.IsNullOrEmpty(m_keywords))
                {
                    return Title;
                }
                return m_keywords;
            }
            set
            {
                m_keywords = value;
            }
        }

        string m_description;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(m_description))
                {
                    m_description = Title + "。";
                }
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        /// <summary>
        /// 数据源 
        /// </summary>
        public Func<object> DataSourceDelegate { get; set; }

        /// <summary>
        /// 数据源 
        /// </summary>
        public object DataSource
        {
            get
            {
                return DataSourceDelegate();
            }
        }

        /// <summary>
        /// 列表字典
        /// </summary>
        public List<KeyValuePair<string,string>> Columns { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public PageModel Father { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<PageModel> Children { get; set; }

        /// <summary>
        /// 复制类
        /// </summary>
        /// <param name="page"></param>
        void Copy(PageModel page)
        {
            Path = page.Path;
            Title = page.Title;
            Keywords = page.Keywords;
            Description = page.Description;
            DataSourceDelegate = page.DataSourceDelegate;
            Columns = page.Columns;
            Father = page.Father;
            Children = page.Children;
        }

        /// <summary>
        /// 列序号
        /// </summary>
        int m_columIndex;

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddColumn(string key, string value)
        {
            Columns.Insert(m_columIndex, new KeyValuePair<string,string>(key, value));
            m_columIndex++;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="page"></param>
        public void AddChild(PageModel page)
        {
            Children.Add(page);
            page.Father = this;
        }

        /// <summary>
        /// 路径充当键
        /// </summary>
        public string Key
        {
            get
            {
                return Path;
            }
        }

        /// <summary>
        /// 标题充当值
        /// </summary>
        public string Value
        {
            get
            {
                return Title;
            }
        }

        /// <summary>
        /// 路径充当链接
        /// </summary>
        public string Link
        {
            get
            {
                return Path;
            }
        }

        /// <summary>
        /// 路径充当名称
        /// </summary>
        public string Name
        {
            get
            {
                return Path;
            }
        }

        /// <summary>
        /// 是否包含列表
        /// </summary>
        public bool CanBeDisplayed
        {
            get
            {
                return Columns.Count > 0;
            }
        }

    }
}
