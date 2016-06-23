#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/13 18:59:13
* 文件名：ConfigReader
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using CT.Schedule.Domain;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CT.Schedule.Core.PluginManager
{
    /// <summary>
    /// 配置信息读取器
    /// </summary>
    internal class ConfigFileHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static List<SchedulePluginsInfo> Read(string path)
        {
            List<SchedulePluginsInfo> lst = new List<SchedulePluginsInfo>();
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string line;
                SchedulePluginsInfo sp = null;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) {
                        continue;
                    }
                    bool isStop = false;
                    ReadNoteLine(line);
                    ReadAssemblyInfo(line,ref isStop, ref sp, ref lst);
                    ReadField(line,ref isStop, "Id", ref sp);
                    ReadField(line, ref isStop, "Name", ref sp);
                    ReadField(line, ref isStop, "EventsName", ref sp);
                    ReadField(line, ref isStop, "NotifyUrl", ref sp);                                     
                }
                SaveLastPluginInfo(sp, ref lst);
                return lst;
            }
        }

        /// <summary>
        /// 读取注释信息
        /// </summary>
        /// <param name="line"></param>
        private static void ReadNoteLine(string line) {
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) {
                return;
            }
        }
        /// <summary>
        /// 读取插件程序集信息
        /// </summary>
        /// <param name="line"></param>
        private static void ReadAssemblyInfo(string line,ref bool isStop ,ref SchedulePluginsInfo sp,ref List<SchedulePluginsInfo> lst)
        {
            if (isStop) {
                return;
            }
            if (!line.StartsWith("[") || !line.EndsWith("]"))
            {
                return;
            }
            SaveLastPluginInfo(sp, ref lst);
            sp = new SchedulePluginsInfo() { AssemblyInfo = line.Trim('[', ']') };
            isStop = true;
        }
        /// <summary>
        /// 保存最后读取的插件配置信息
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="lst"></param>
        private static void SaveLastPluginInfo(SchedulePluginsInfo sp, ref List<SchedulePluginsInfo> lst)
        {
            if (sp != null)
            {
                lst.Add(sp);
            }
        }

        /// <summary>
        /// 读取字段信息
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fieldName"></param>
        /// <param name="sp"></param>
        private static void ReadField(string line, ref bool isStop, string fieldName, ref SchedulePluginsInfo sp) {
            if (isStop) {
                return;
            }
            if (sp == null) {
                return;
            }
            if (!line.StartsWith(fieldName + "=", StringComparison.OrdinalIgnoreCase)) {
                return;
            }
            string val = line.Substring(fieldName.Length+1).Trim();
            if (string.IsNullOrEmpty(val)) {
                return;
            }
            PropertyInfo propertyInfo = typeof(SchedulePluginsInfo).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            if (propertyInfo == null) {
                return;
            }
            propertyInfo.SetValue(sp, Convert.ChangeType(val, propertyInfo.PropertyType));
            isStop = true;
        }

        public static void Write(string path, SchedulePluginsInfo sp)
        {
            string txt = string.Empty;
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                txt = sr.ReadToEnd();
            }
            txt = txt.Replace("\r\n\r\n", "\r\n\r\n\r\n");
            int index = txt.IndexOf(sp.AssemblyInfo, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                logger.Error(new Exception(sp.AssemblyInfo + "不存在于" + path));
                return;
            }
            int end = txt.IndexOf('[', index + sp.AssemblyInfo.Length + 3);

            if (end == -1)
                end = txt.Length - 1;
            int i_start = txt.IndexOf("Id=", index + sp.AssemblyInfo.Length + 3, StringComparison.OrdinalIgnoreCase);

            if (i_start == -1 || i_start > end)
            {
                i_start = index + sp.AssemblyInfo.Length + 3;
                txt = txt.Insert(i_start, "\r\nId=" + sp.Id.ToString() + "\r\n");
            }
            else
            {
                int i_end = txt.IndexOf("\r\n", i_start);
                txt = txt.Remove(i_start, i_end - i_start);
                txt = txt.Insert(i_start, "\r\nId=" + sp.Id.ToString() + "\r\n");
            }
            txt = txt.Replace("\r\n\r\n", "\r\n");

            using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.WriteLine(txt);
            }
        }
    }
}
