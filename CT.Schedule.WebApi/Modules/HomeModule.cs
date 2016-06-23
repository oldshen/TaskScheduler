using CT.Schedule.Domain;
using CT.Schedule.IService;
using CT.Schedule.WebApi.Model;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
namespace CT.Schedule.WebApi.Modules
{
    public class HomeModule : Nancy.NancyModule
    {

        public HomeModule(IScheduleJobsService jobsService)
        {

            this.RequiresAuthentication(); //需要身份验证
          
            Get["/"] = x =>
            {
                return View["/Jobs/Index", jobsService.FindAll()];
            };

            Get["/EditJob"] = x =>
            {                
                 return View["/Jobs/AddJob"];
            };

            Get["/EditJob/{id}"] = x =>
            {
                return View["/Jobs/EditJob",jobsService.FindOne(x.id)];
            };


            Post["/EditJob"] = x =>
            {
                JobModel jobModel =this.Bind<JobModel>();

                if (string.IsNullOrEmpty(jobModel.Name)) {
                    return View["/Shared/Error", "任务名不能为空,<a href=\"javascript:history.go(-1);\">重新添加</a>"];
                }
                if (jobModel.PluginID <= 0) {
                    return View["/Shared/Error", "请选择任务插件,<a href=\"javascript:history.go(-1);\">重新添加</a>"];
                }
                ScheduleJobsInfo schJob = jobModel.ToScheduleJobsInfo();

                bool success = jobModel.Id==Guid.Empty? jobsService.AddJob(schJob):jobsService.UpdateJob(schJob);
                if (success)
                {
                    return View["/shared/success", "保存成功,<a href=\"/\">返回列表</a>"];
                }
                return View["/Shared/Error", "保存失败,<a href=\"javascript:history.go(-1);\">重新添加</a>"]; 
            };

            Get["/RemoveJob/{id}"] = x =>
            {
                bool successed = jobsService.Remove(x.id);

                if (successed)
                {
                    return View["/shared/success", "删除成功,<a href=\"/\">返回</a>"];
                }
                return View["/Shared/Error", "删除失败,<a href=\"javascript:history.go(-1);\">返回</a>"];
            };
        }
    }
}