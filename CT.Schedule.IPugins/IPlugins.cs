using log4net;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CT.Schedule
{

    /// <summary>
    /// 执行结果
    /// </summary>
    [Serializable]
    public class ExecuteResult
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Successed { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

    }

    /// <summary>
    /// 执行完成事件
    /// </summary>
    /// <param name="e"></param>
    public delegate void ExecuteCompletedHandler(IPlugins sender, ExecuteResult e);

    /// <summary>
    /// 任务执行接口
    /// 
    /// 插件的配置信息文件为 config.ini 必须存在
    /// 插件的config文件为 config.ini.config
    /// 注意事项：在插件中请使用 ConfigHelper.Configuration 代替   System.Configuration.ConfigurationManager
    ///           使用  ConfigHelper.Configuration.AppSettings.Settings["ScheduleHost"].Value 代替 ConfigHelper.Configuration.AppSettings["ScheduleHost"]
    /// </summary>
    public abstract class IPlugins : MarshalByRefObject
    {


        public IPlugins() {
            ConfigHelper.SetConfiguration();
        }


        protected static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid JobID { get; private set; }
        /// <summary>
        /// 回调通知URL
        /// </summary>
        public string NotifyUrl { get; private set; }
        /// <summary>
        /// 任务执行模式
        /// </summary>
        public JobMode Mode { get; private set; }


        public TaskExecuteState State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobid"></param>
        /// <param name="notifyurl"></param>
        /// <param name="once"></param>
        /// <param name="successed"></param>
        /// <param name="args"></param>
        public async void Execute(Guid jobid, string notifyurl, JobMode mode, TaskExecuteState state, ExecuteCompletedHandler successed/*, params string[] args*/)
        {
            this.JobID = jobid;
            this.NotifyUrl = notifyurl;
            this.Mode = mode;
            this.State = state;
            ExecuteResult result = null;
            try
            {
                result = await ExecuteAsync();
                ThreadPool.QueueUserWorkItem(q => { OnNotify((ExecuteResult)q); }, result);
            }
            catch (Exception ex)
            {
                result = new ExecuteResult() { Successed = false, Message = ex.Message };
                logger.Error(ex);
            }
            if (mode == JobMode.OnTimeEveryTime)
            {
                successed(this, result);
            }
            if (state != null)
            {
                state.SetCompleted(true, string.IsNullOrEmpty(result.Message) ? null : new Exception(result.Message));
            }
        }
        public Task<ExecuteResult> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                return OnExecute();
            });
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="jobid">任务Id</param>
        /// <param name="notifyurl">任务完成通知地址</param>
        /// <param name="args">执行参数</param>
        /// <param name="once">是否仅执行一次</param>
        /// <param name="successed">执行完成事件,在Execute执行完成，必须收到</param>
        protected abstract ExecuteResult OnExecute();

        /// <summary>
        /// 通知目标，告知任务执行完成
        /// </summary>
        /// <param name="id">任务Id</param>
        /// <param name="notifyurl">通知地址</param>
        /// <param name="e">结果</param>
        protected virtual void OnNotify(ExecuteResult result)
        {
            logger.Info("回调通知:" + JobID);
            if (string.IsNullOrEmpty(NotifyUrl))
            {
                return;
            }
            string url = NotifyUrl + "?jobid=" + JobID.ToString();

            Uri uri = new Uri(url);
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = true;
            handler.AllowAutoRedirect = true;
            HttpClient client = new HttpClient(handler);

            client.BaseAddress = new Uri(uri.Scheme + "://" + uri.Host);
            var task = client.GetAsync(uri.AbsolutePath);
            var cbresult = task.Result.Content.ReadAsStringAsync().Result;
            if (cbresult.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                //通知成功
            }
            else {
                //考虑重试机制
            }

            //logger.Info(result);
        }
    }
}
