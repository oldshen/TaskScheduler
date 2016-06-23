using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;

namespace CT.Schedule.WebApi.Auth
{
    public class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var user = UserData.Find(identifier);
            if (user != null) {
                return new UserIdentity() {
                    UserName = user.UserName,
                    Claims = new[] { "SystemUser" }
                };
            }
            return null;
        }
    }
}