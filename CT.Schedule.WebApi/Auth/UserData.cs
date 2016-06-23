using CT.Schedule.WebApi.Model;
using System;

namespace CT.Schedule.WebApi.Auth
{
    /// <summary>
    /// 用户数据
    /// </summary>
    public class UserData
    {
        static SystemUser user;

        protected static SystemUser User
        {
            get
            {
                if (user == null)
                {
                    string userstr = System.Configuration.ConfigurationManager.AppSettings["CTSchedule.AdminUser"];
                    if (string.IsNullOrEmpty(userstr))
                    {
                        throw new Exception("appSettings 未设置CTSchedule.adminUser节点");
                    }
                    string[] array = userstr.Split(new char[] { ';' });
                    if (array.Length != 2)
                    {
                        throw new Exception("CTSchedule.adminUser值配置错误");
                    }
                    user = new SystemUser()
                    {
                        UserID = Guid.NewGuid(),
                        UserName = array[0].Substring(5).Trim(),
                        Password = array[1].Substring(9).Trim()
                    };
                }
                return user;
            }
        }
        public static SystemUser Find(string userName, string password)
        {
            if (userName == User.UserName && User.Password == password)
            {
                return User;
            }
            return null;
        }

        public static SystemUser Find(Guid id)
        {
            if (User.UserID == id)
            {
                return User;
            }
            return null;
        }
    }
}