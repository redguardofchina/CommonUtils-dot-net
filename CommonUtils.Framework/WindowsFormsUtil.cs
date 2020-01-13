using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonUtils
{
    public static class WindowsFormsUtil
    {
        /// <summary>
        /// 保存文字到ini文件
        /// </summary>
        public static void SaveTextBoxs(Control.ControlCollection controls, params Control[] exceptControls)
        {
            string settingPath = PathUtil.GetFull("setting.ini");
            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (Control control in controls)
            {
                if (control.GetType() == typeof(TextBox) && !string.IsNullOrEmpty(control.Text) && !map.ContainsKey(control.Name))
                {
                    if (exceptControls.Contains(control))
                    {
                        continue;
                    }
                    map.Add(control.Name, control.Text);
                }
            }
            string json = JsonUtil.ToJson(map);
            FileUtil.Save(settingPath, json);
        }

        /// <summary>
        /// 从ini文件读取文字
        /// </summary>
        public static void LoadTextBoxs(Control.ControlCollection controls)
        {
            string settingPath = PathUtil.GetFull("setting.json");
            string json = FileUtil.GetText(settingPath);
            var map = JsonUtil.Deserialize<MapStringString>(json);
            if (map == null)
                return;

            foreach (Control control in controls)
                if (map.ContainsKey(control.Name))
                    control.Text = map[control.Name];
        }

        /// <summary>
        /// 优化TextBox,接受换行符,制表符,ctrl,A等
        /// </summary>
        public static void AcceptHotkeys(TextBox textBox)
        {
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.AcceptsReturn = true;
            textBox.AcceptsTab = true;
            textBox.WordWrap = true;
            textBox.AllowDrop = true;
            textBox.KeyDown += OnKeyDownCtrlA;
        }

        /// <summary>
        /// 输入框定义快捷键
        /// </summary>
        private static void OnKeyDownCtrlA(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
                ((TextBox)sender).SelectAll();
        }
    }
}
