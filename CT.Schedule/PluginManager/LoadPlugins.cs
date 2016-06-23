using CT.Schedule.Domain;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

namespace CT.Schedule.Core.PluginManager
{
    /// <summary>
    /// 加载插件
    /// </summary>
    internal class LoadPlugins
    {
        public static readonly LoadPlugins Instance = new LoadPlugins();

        private FileSystemWatcher watcher;
        private string pluginpath;
        private bool isinit = false;  //是否已经初始化
        

        private LoadPlugins()
        {

            this.pluginpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            if (!Directory.Exists(this.pluginpath))
                Directory.CreateDirectory(this.pluginpath);
            watcher = new FileSystemWatcher(pluginpath);
            watcher.NotifyFilter = NotifyFilters.DirectoryName; //只监视目录
            watcher.Created += new FileSystemEventHandler(onWatcher);
            watcher.Deleted += new FileSystemEventHandler(onWatcher);
        }
        private void onWatcher(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                if (!new DirectoryInfo(e.FullPath).Parent.FullName.Equals(this.pluginpath, StringComparison.OrdinalIgnoreCase))
                    return;             //如果父级目录不是plugins则不处理
                LoadPath(e.FullPath);   //加载插件
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                if (!new DirectoryInfo(e.FullPath).Parent.FullName.Equals(this.pluginpath, StringComparison.OrdinalIgnoreCase))
                    return;             //如果父级目录不是plugins则不处理
                //卸载域,将任务从队列中删除
                UnLoadPath(e.FullPath);
            }
        }

        /// <summary>
        /// 插件列表
        /// </summary>
        public List<SchedulePluginsInfo> Plugins
        {
            get
            {
                return typeList;
            }
        }

        private static object lockpad = new object();
        List<SchedulePluginsInfo> typeList = new List<SchedulePluginsInfo>();   //插件列表
        //Dictionary<string, string> appDir = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);   //Key=Type value=Path
        //Dictionary<string, AppDomainInfo> appDomainDic = new Dictionary<string, AppDomainInfo>(StringComparer.OrdinalIgnoreCase);   //Key=Path,Value=AppDomainInfo

        //Dictionary<string, List<SchedulePluginsInfo>> appPlugins = new Dictionary<string, List<SchedulePluginsInfo>>(StringComparer.OrdinalIgnoreCase);

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        List<PluginDomainInfo> pluginDomains = new List<PluginDomainInfo>();
        /// <summary>
        /// 读取插件目录
        /// </summary>
        /// <param name="path"></param>
        private void LoadPath(string path)
        {
            string config = Path.Combine(path, "config.ini");
            if (!File.Exists(config))
                return;     //如果没有config文件，不处理
            string[] files = Directory.GetFiles(path, "*.dll");
            if (files == null || files.Length == 0)
                return;

            //appDomainDic[path] = new AppDomainInfo(path);

            var list = ConfigFileHelper.Read(config);
            if (list == null)
            {
                return;
            }
            PluginDomainInfo pd = new PluginDomainInfo(path);
            list.ForEach(item =>
            {
                bool successed = pd.AddJobPlugin(item);
                if (successed)
                {
                    typeList.Add(item);
                }
            });
            pluginDomains.Add(pd);


            //list.ForEach(item =>
            //{
            //    lock (lockpad)
            //    {
            //        if (appDir.ContainsKey(item.AssemblyInfo))
            //        {
            //            logger.Error(new Exception(item.AssemblyInfo + "已存在于" + appDir[item.AssemblyInfo]));
            //            return;
            //        }
            //        bool f = true;
            //        if (item.Id == 0)
            //        {
            //            try
            //            {
            //                pluginService.Save(item);
            //                ConfigFileHelper.Write(config, item);
            //            }
            //            catch
            //            {
            //                f = false;
            //            }
            //        }
            //        if (f)
            //        {
            //            appDir[item.AssemblyInfo] = path;
            //            typeList.Add(item);
            //            //appDomainDic[path].Plugins.Add(item);
            //        }
            //    }
            //});
        }
        /// <summary>
        /// 卸载程序集域
        /// </summary>
        /// <param name="path"></param>
        private void UnLoadPath(string path)
        {
            lock (lockpad)
            {
                PluginDomainInfo app = pluginDomains.FirstOrDefault(c=>c.Directory.Equals(path, StringComparison.OrdinalIgnoreCase));
                if (app == null) {
                    return;
                }
                typeList.RemoveAll(c => app.Assemblies.Exists(x => x == c.AssemblyInfo));
                if (app.AppDomain != null) {
                    app.AppDomain.UnLoad();
                }
                app.Dispose();
                
               
                //AppDomainInfo app = appDomainDic[path];

                //if (app == null)
                //    return;
                ////app.Plugins.ForEach(item =>
                ////{
                ////    typeList.RemoveAll(c => c.Id == item.Id);
                ////    appDir.Remove(item.AssemblyInfo);
                ////});
                //app.UnLoad();
                //appDomainDic.Remove(path);
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            lock (lockpad)
            {
                if (isinit)
                {
                    return;
                }

                //pluginService = GlobalUnityContainer.Resolve<ISchedulePluginsService>();
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

                string[] dirs = Directory.GetDirectories(this.pluginpath);
                foreach (string dir in dirs)
                {
                    try
                    {
                        LoadPath(dir);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
                if (!watcher.EnableRaisingEvents)
                    watcher.EnableRaisingEvents = true;
                isinit = true;

            }
        }
        bool isexit;
        /// <summary>
        /// 退出主域时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            UnLoad();
        }

        /// <summary>
        /// 卸载所有子域
        /// </summary>
        public void UnLoad()
        {
            if (!isexit)
            {
                lock (lockpad)
                {
                    pluginDomains.ForEach(c =>
                    {
                        UnLoadPath(c.Directory);
                    });
                    //foreach (string dir in appDomainDic.Keys)
                    //{
                    //    AppDomainInfo app = appDomainDic[dir];
                    //    if (app != null)
                    //    {
                    //        try { app.UnLoad(); }
                    //        catch
                    //        {

                    //        }
                    //    }
                    //}
                    //appDir.Clear();
                    //appDomainDic.Clear();
                    pluginDomains.Clear();
                    typeList.Clear();
                }
                isexit = true;
                isinit = false;
            }
        }



        /// <summary>
        /// 创建插件实体
        /// </summary>
        /// <param name="fulltypename"></param>
        /// <returns></returns>
        public IPlugins CreateInstance(string fulltypename)
        {

            if (!typeList.Exists(c=>c.AssemblyInfo==fulltypename))
            {
                logger.Error(new Exception(fulltypename + "依赖文件目录不存在"));
                return null;
            }

            string[] types = fulltypename.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (types.Length != 2)
            {
                logger.Error(new Exception(fulltypename + "不规范"));
                return null;
            }

            PluginDomainInfo pl = pluginDomains.FirstOrDefault(c => c.Assemblies.Exists(x => x == fulltypename));
            if (pl == null) {
                return null;
            }
            //string path = appDir[fulltypename];

            AppDomainInfo app = pl.AppDomain;// appDomainDic[path];
            if (app == null)
            {
                logger.Error(new Exception(fulltypename + "依赖程序集错误"));
                return null;
            }
            try
            {
                ObjectHandle o = (ObjectHandle)app.AppDomain.CreateInstance(types[1].Trim(), types[0].Trim());
                return (IPlugins)o.Unwrap();
            }
            catch (Exception ex)
            {
                logger.Error(fulltypename, ex);
                return null;
            }
        }
    }
}
