using CT.Schedule.Core;
using CT.Schedule.WebApi.SelfHost;
using System;
using System.ServiceProcess;


namespace CT.Schedule.WindowsService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                //Windows Service
                RunAsService();
                return;
            }
            string exeArg = string.Empty;
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Welcome to CTTaskScheduler!");

                Console.WriteLine("Please press a key to continue...");
                Console.WriteLine("-[r]: Run this application as a console application;");
                Console.WriteLine("-[i]: Install this application as a Windows Service;");
                Console.WriteLine("-[u]: Uninstall this Windows Service application;");

                while (true)
                {
                    exeArg = Console.ReadKey().KeyChar.ToString();// Console.ReadLine().Trim();//
                    Console.WriteLine();

                    if (Run(exeArg, null))
                        break;
                }
            }
            else
            {
                exeArg = args[0];

                if (!string.IsNullOrEmpty(exeArg))
                    exeArg = exeArg.TrimStart('-');

                Run(exeArg, args);
            }
        }
        private static bool Run(string exeArg, string[] startArgs)
        {
            switch (exeArg.ToLower())
            {
                case ("i"):
                    SelfInstaller.InstallMe();
                    return true;

                case ("u"):
                    SelfInstaller.UninstallMe();
                    return true;

                case ("r"):
                    RunAsConsole();
                    return true;
                case ("q"):
                    DisposeAsConsole();
                    return true;

                default:
                    Console.WriteLine("Invalid argument!");
                    return false;
            }
        }
        public static void RunAsService()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CTScheduleService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        static Start start;

        public static void RunAsConsole()
        {
            //Console.WriteLine("start servie...");
            //NotificationCenter.Instance.PostNotificationName<string>("Test", "hello");
            //using (Test test = new Test("test"))
            //{
            //    Test test1 = new Test("test1");
            //    NotificationCenter.Instance.PostNotificationName<string>("Test", "--");
            //    //System.Threading.Thread.Sleep(1000);
            //}
            //NotificationCenter.Instance.PostNotificationName<string>("Test", "world");
            //Test test2 = new Test("test2");

            if (start == null)
            {
                start = new Start();
            }
            start.StartSchedule();
            Console.WriteLine("service running...");
            HttpSelfHost.Instance.Start();
        }

        public static void DisposeAsConsole()
        {
            Console.WriteLine("close servie...");

            try
            {
                if (start != null)
                {
                    start.Dispose();
                }
            }
            catch { }
        }


    }

    public class Test : IDisposable
    {
        string name;
        public Test(string name)
        {
            this.name = name;
            NotificationCenter.Instance.AddObserver(this, "Test", OnAction);
        }

        public void Dispose()
        {
            NotificationCenter.Instance.RemoveObserver(this);
        }

        void OnAction(object obj)
        {
            Console.WriteLine(this.name+"\t"+obj);
        }


    }
}
