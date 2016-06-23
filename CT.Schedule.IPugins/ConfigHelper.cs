#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/21 9:18:17
* 文件名：ConfigHelper
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Configuration;
using System.Xml;
using log4net;

namespace CT.Schedule
{
    /// <summary>
    /// 实现子域读取到主域的配置信息，主要是  AppSettings 和 ConnectionStrings
    /// </summary>
    internal static class ConfigHelper
    {
        static bool isinited = false;
        static object lockpad = new object();

        /// <summary>
        /// 设置配置文件信息
        /// </summary>
        public static void SetConfiguration()
        {
            string configPath = GetMainAppDomainConfigPath();
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            {
                return;
            }
            lock (lockpad)
            {
                if (isinited)
                {
                    return;
                }
                isinited = true;
                Configuration config = GetConfiguration(configPath);
                if (config == null)
                {
                    return;
                }
                configPath = Path.Combine(AppDomain.CurrentDomain.FriendlyName, "config.ini.1.config");
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                }
                config.SaveAs(configPath, ConfigurationSaveMode.Full);
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath); //更改配文件
                ResetConfigMechanism();
                //LogManager.ResetConfiguration();
                //CopyLog4netConfig();
            }
        }

        /// <summary>
        /// 获取主域配置文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetMainAppDomainConfigPath()
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.config");
            if (files == null || files.Length == 0)
            {
                return null;
            }
            foreach (string file in files)
            {
                if (file.IndexOf(".vshost.", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    continue;
                }
                if (IsConfigFile(file))
                {
                    return file;
                }
            }
            return null;
        }

        /// <summary>
        /// 刷新  System.Configuration.ConfigurationManager 中的缓存数据
        /// </summary>
        private static void ResetConfigMechanism()
        {
            typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 0);

            typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);

            typeof(ConfigurationManager)
                .Assembly.GetTypes()
                .Where(x => x.FullName ==
                            "System.Configuration.ClientConfigPaths")
                .First()
                .GetField("s_current", BindingFlags.NonPublic |
                                       BindingFlags.Static)
                .SetValue(null, null);
        }

        private static Configuration GetConfiguration(string mainDomainConfigPath)
        {
            Configuration config = null;
            string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (File.Exists(configPath))
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = configPath;
                config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }
            if (config == null)
            {
                if (!File.Exists(mainDomainConfigPath))
                {
                    return null;
                }
            }
            Configuration mainConfig = GetParentDomainConfig(mainDomainConfigPath);
            MergeDomainConfig(ref config, mainConfig);
            return config;

        }

        private static Configuration GetParentDomainConfig(string mainConfigPath)
        {
            if (!File.Exists(mainConfigPath))
            {
                return null;
            }
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = mainConfigPath;
            Configuration conf = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            return conf;
        }
        
        /// <summary>
        /// 合并主域和子域配置信息
        /// </summary>
        /// <param name="privateConfig"></param>
        private static void MergeDomainConfig(ref Configuration privateConfig, Configuration config)
        {
            if (privateConfig == null)
            {
                privateConfig = config;
                return;
            }
            if (config == null)
            {
                return;
            }

            if (config.AppSettings.Settings.Count > 0)
            {
                foreach (var setting in config.AppSettings.Settings.AllKeys)
                {
                    if (privateConfig.AppSettings.Settings.AllKeys.Contains(setting))
                    {
                        continue;
                    }
                    privateConfig.AppSettings.Settings.Add(setting, config.AppSettings.Settings[setting].Value);
                }
            }

            if (config.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                int len = config.ConnectionStrings.ConnectionStrings.Count;
                for (var i = 0; i < len; i++)
                {
                    ConnectionStringSettings setting = config.ConnectionStrings.ConnectionStrings[i];
                    if (privateConfig.ConnectionStrings.ConnectionStrings[setting.Name] == null)
                    {
                        privateConfig.ConnectionStrings.ConnectionStrings.Add(setting);
                    }
                }
            }

        }

        /// <summary>
        /// 判断是否为配置文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsConfigFile(string filePath)
        {
            if (!filePath.EndsWith(".exe.config", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            try
            {
                ConfigXmlDocument doc = new ConfigXmlDocument();
                doc.Load(filePath);
                XmlNode node = doc.DocumentElement;
                return node.Name == "configuration";
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //private static void CopyLog4netConfig() {

        //    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
        //    if (!File.Exists(filePath)) {
        //        return;
        //    }

        //    string privateFilePath= Path.Combine(AppDomain.CurrentDomain.FriendlyName, "log4net.config");
        //    if (File.Exists(privateFilePath)) {
        //        return;
        //    }
        //    File.Copy(filePath, privateFilePath);

        //}
    }
}
