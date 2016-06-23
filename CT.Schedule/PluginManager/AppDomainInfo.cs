#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/14 13:00:18
* 文件名：AppDomainInfo
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CT.Schedule.Core.PluginManager
{
    /// <summary>
    /// 
    /// </summary>
    public class AppDomainInfo
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static IDictionary<string, AppDomain> applist = new Dictionary<string, AppDomain>(StringComparer.OrdinalIgnoreCase);    //不区分大小写
        private static object lockpad = new object();


        public AppDomain AppDomain
        {
            get
            {
                return LoadAppDomain();
            }
        }
        /// <summary>
        /// 插件文件夹目录
        /// </summary>
        public string DirPath { get; private set; }

        public AppDomainInfo(string dirPath)
        {
            this.DirPath = dirPath;
        }
        /// <summary>
        /// 加载域
        /// </summary>
        /// <returns></returns>
        private AppDomain LoadAppDomain()
        {
            if (!applist.ContainsKey(this.DirPath))
            {
                lock (lockpad)
                {
                    if (!applist.ContainsKey(this.DirPath))
                    {
                        SetUpAppDomain();
                    }
                }
            }
            return applist[this.DirPath];
        }

        /// <summary>
        /// 创建域
        /// </summary>
        /// <returns></returns>
        private void SetUpAppDomain()
        {
            AppDomainSetup setup = new AppDomainSetup();
           
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.PrivateBinPath = "Plugins\\" + Path.GetFileName(this.DirPath.TrimEnd('/', '\\'));
            setup.ShadowCopyFiles = "true";
            setup.ApplicationName = this.DirPath;
           
            string configfile = Path.Combine(this.DirPath, "config.ini.config");
            if (File.Exists(configfile))
            {   //加载配置文件
                setup.ConfigurationFile = configfile;
            }
            AppDomain appDomain = AppDomain.CreateDomain(this.DirPath, null, setup);

            if (appDomain != null)
            {
                applist[this.DirPath] = appDomain;
            }
        }

        /// <summary>
        /// 卸载程序域
        /// </summary>
        public void UnLoad()
        {
            lock (lockpad)
            {
                if (applist.ContainsKey(this.DirPath))
                {
                    try
                    {
                        AppDomain.Unload(applist[this.DirPath]);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                    applist.Remove(this.DirPath);
                }
            }
        }

        
    }
}
