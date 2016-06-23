using CT.Schedule.WebApi.Auth;
using CT.Schedule.WebApi.Model;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;

namespace CT.Schedule.WebApi.Modules
{
    public class AccountModule:Nancy.NancyModule
    {
        public AccountModule() {
             
           
            Get["/login"] = x => View["Account/Index"];

            Get["/logout"]=x=>{
                return this.Logout("/login");
            };

            Post["/login"] = x => {
                SystemUser user = this.Bind<SystemUser>();
                if (user == null) {
                    return View["Account/Index", "用户名密码错误"];
                }
                user = UserData.Find(user.UserName, user.Password);
                if (user == null) {
                    return View["Account/Index", "用户名密码错误"];
                }
                return this.LoginAndRedirect(user.UserID,fallbackRedirectUrl:(string)x["returnUrl"]??"/");
            };
        }
    }
}