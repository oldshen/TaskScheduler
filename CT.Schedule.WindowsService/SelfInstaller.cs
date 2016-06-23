using System;
using System.Configuration.Install;
using System.IO;

namespace CT.Schedule.WindowsService
{
    /// <summary>
    /// 安装服务
    /// </summary>
    public static class SelfInstaller
    {
        private static readonly string _exePath =Path.Combine( AppDomain.CurrentDomain.BaseDirectory,AppDomain.CurrentDomain.FriendlyName);//Assembly.GetExecutingAssembly().Location;

        public static bool InstallMe()
        {
            try
            {                
                ManagedInstallerClass.InstallHelper(new string[] { _exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool UninstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", _exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
