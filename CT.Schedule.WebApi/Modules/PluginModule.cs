using CT.Schedule.IService;
using Nancy;
using Nancy.Security;
namespace CT.Schedule.WebApi.Modules
{
    public class PluginModule : Nancy.NancyModule
    {
        public PluginModule(ISchedulePluginsService pluginService)
        {
            this.RequiresAuthentication();

            Get["/plugin"] = x=> {
                var list = pluginService.FindAll();
                return View["/Jobs/Plugin", list];
            };

            Get["/pluginsData"] = x =>
            {
               
                var list = pluginService.FindAll();
                return Response.AsJson(list);
            };
        }
     
    }
}