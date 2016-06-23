using Nancy;
using System;
using System.IO;

namespace CT.Schedule.WebApi.SelfHost
{

    /// <summary>
    /// 设置应用程序根目录
    /// </summary>
    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web");
            if (Directory.Exists(path))
            {
                return path;
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }

}