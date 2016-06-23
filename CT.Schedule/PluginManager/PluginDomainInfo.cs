using CT.Schedule.Domain;
using CT.Schedule.IService;
using System;
using System.Collections.Generic;
using System.IO;

namespace CT.Schedule.Core.PluginManager
{
    /// <summary>
    /// 插件域信息
    /// </summary>
    public class PluginDomainInfo : IDisposable
    {
        private ISchedulePluginsService pluginService;

        private static object lockPad = new object();
        public string Directory { get; private set; }
        /// <summary>
        /// 插件运行域
        /// </summary>
        public AppDomainInfo AppDomain { get; private set; }

        /// <summary>
        /// 任务插件程序集集合
        /// </summary>
        public List<string> Assemblies { get; private set; }


        public PluginDomainInfo(string directory)
        {
            this.Directory = directory;
            Assemblies = new List<string>();
            pluginService = pluginService = GlobalUnityContainer.Resolve<ISchedulePluginsService>();
        }

        public bool AddJobPlugin(SchedulePluginsInfo jobPlugin)
        {
            lock (lockPad)
            {
                if (jobPlugin == null)
                {
                    return false;
                }
                if (Assemblies.Exists(c => c.Equals(jobPlugin.AssemblyInfo, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
                if (jobPlugin.Id == 0)
                {
                    bool successed = pluginService.Save(jobPlugin);
                    if (!successed)
                    {
                        return false;
                    }

                    ConfigFileHelper.Write(Path.Combine(Directory, "config.ini"), jobPlugin);
                }
                Assemblies.Add(jobPlugin.AssemblyInfo);
                if (this.AppDomain == null)
                {
                    this.AppDomain = new AppDomainInfo(Directory);
                }
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PluginDomainInfo)
            {
                return this.Directory.Equals(((PluginDomainInfo)obj).Directory, StringComparison.OrdinalIgnoreCase);

            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public void Dispose()
        {
            if (AppDomain != null)
            {
                AppDomain = null;
            }
        }
    }

    public class PluginCollection : Dictionary<string, PluginDomainInfo>
    {
        public new void Add(string key, PluginDomainInfo value)
        {
            if (this.ContainsKey(key))
            {
                return;
            }
        }
    }
}
